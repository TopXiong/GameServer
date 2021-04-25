using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using TF.Tools;
using TF.Log;

namespace GameServer.Core.NetWork
{
    public class ServerService
    {
        /// <summary>
        /// 收到数据后的回调,每个游戏的逻辑处理在这里加上
        /// </summary>
        public event Action<UserToken,byte[]> ReceiveClientData;
        /// <summary>
        /// 监听Socket
        /// </summary>
        private Socket listenSocket;

        /// <summary>
        /// 房间ID对应房间
        /// </summary>
        private Dictionary<int,BaseRoom> id2rooms = new Dictionary<int,BaseRoom>();
        /// <summary>
        /// 玩家ID对应玩家
        /// </summary>
        private Dictionary<Guid, UserToken> id2players = new Dictionary<Guid, UserToken>();
        /// <summary>
        /// TODO 改为配置文件读取
        /// </summary>
        public ServerService()
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any,1234);
            listenSocket.Bind(iPEndPoint);
            listenSocket.Listen(20);
            ReceiveClientData += DataHandle;
            //绑定房间的send方法
            BaseRoom.Send += (id, gameNetObject) => { SendData(id2players[id], gameNetObject); };
            StartAccept(null);
        }

        #region DataHandleMethod

        private void Msg(UserToken userToken,Msg msg)
        {
            msg.Context = "Receive: " + msg.Context;
            SendData(userToken, msg);
        }

        private void CreateRoom(UserToken userToken, CreateRoomC2S createRoom)
        {
            int roomId = createRoom.room.Id;
            if (id2rooms.ContainsKey(roomId))
            {
                SendData(userToken, new CreateRoomS2C(-1));
                return;
            }
            id2rooms.Add(roomId, createRoom.room);
            int playerIndex = createRoom.room.PlayerJoin(userToken.Guid);
            SendData(userToken, new CreateRoomS2C(playerIndex));
        }

        private void JoinRoom(UserToken userToken, JoinRoomC2S joinRoomC2S)
        {            
            // Cheak if in room
            var linqrooms = from linqroom in id2rooms where linqroom.Value.ContainsPlayer(userToken.Guid)!=-1 select linqroom;
            foreach (var Troom in linqrooms)
            {
                Troom.Value.PlayerLeave(userToken.Guid);
            }
            //房间不存在
            if (!id2rooms.ContainsKey(joinRoomC2S.RoomId))
            {
                SendData(userToken, new JoinRoomS2C(-1));
                return;
            }
            //通过Id获取Room
            BaseRoom room = id2rooms[joinRoomC2S.RoomId];
            //验证密码
            if (!room.Password.Equals(joinRoomC2S.Password))
            {
                SendData(userToken,new JoinRoomS2C(-1));
                return;
            }
            //加入是否成功
            int foundIndex = room.PlayerJoin(userToken.Guid);
            if (foundIndex != -1)
            {                
                foreach (var playerId in room.Players)
                {
                    if (userToken.Guid != playerId && playerId != Guid.Empty)
                    {
                        //发送玩家加入消息
                        SendData(id2players[playerId],(new PlayerJoinS2C(userToken.Guid)));
                    }
                }
            }
            SendData(userToken,new JoinRoomS2C(foundIndex));
        }

        private void GetRoomList(UserToken userToken)
        {            
            SendData(userToken, new GetRoomListS2C(id2rooms.Values.ToList()));
        }

        private void LeaveRoom(UserToken userToken)
        {
            Guid currentPlayId = userToken.Guid;
            //找到含有玩家的房间 
            var linqrooms = from linqroom in id2rooms where linqroom.Value.ContainsPlayer(userToken.Guid)!=-1 select linqroom;

            foreach(var baseRoom in linqrooms)
            {
                baseRoom.Value.PlayerLeave(currentPlayId);
                foreach (var playerId in baseRoom.Value.Players)
                {
                    if (playerId != Guid.Empty)
                    {
                        SendData(id2players[playerId], (new PlayerLeaveS2C(userToken.Guid)));
                    }
                }
            }
           
            
        }

        #endregion




        #region 网络相关
        private void DataHandle(UserToken userToken, byte[] bytes)
        {
            var baseNetObject = NetBaseTool.BytesToObject(bytes) as BaseNetObject;
            if(baseNetObject == null) return;
            //如果是游戏数据,就交给游戏房间处理
            if (baseNetObject is GameNetObject)
            {
                var linqrooms = from linqroom in id2rooms where linqroom.Value.ContainsPlayer(userToken.Guid) != -1 select linqroom;
                if (linqrooms.Count() > 0)
                {
                    linqrooms.First().Value.DataHandle(userToken.Guid, baseNetObject as GameNetObject);
                }
                return;
            }
            //系统数据就地处理
            var systemNeObject = baseNetObject as SystemNetObject;
            if (systemNeObject == null)
            {
                return;
            }
            Console.WriteLine(systemNeObject);
            //String 消息——————测试用
            if (systemNeObject.GetType() == typeof(Msg))
            {
                Msg(userToken,systemNeObject as Msg);
            }
            // 创建房间
            else if (systemNeObject.GetType() == typeof(CreateRoomC2S))
            {
                CreateRoom(userToken, systemNeObject as CreateRoomC2S);
            }
            // 加入房间
            else if (systemNeObject.GetType() == typeof(JoinRoomC2S))
            {
                JoinRoom(userToken,systemNeObject as JoinRoomC2S);
            }
            //返回房间列表
            else if (systemNeObject.GetType() == typeof(GetRoomListC2S))
            {
                GetRoomList(userToken);
            }
            //离开房间
            else if (systemNeObject.GetType() == typeof(LeaveRoomC2S))
            {
                LeaveRoom(userToken);
            }
        }

        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += ProcessAccept;
            }
            else
            {
                // socket must be cleared since the context object is being reused  
                acceptEventArg.AcceptSocket = null;
            }
            Console.WriteLine("StartAccept");
            if (!listenSocket.AcceptAsync(acceptEventArg))
            {
                Console.WriteLine("Accepted");
                ProcessAccept(null,acceptEventArg);
            }
        }

        public void ProcessAccept(object sender,SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
            acceptArgs.Completed += IO_Completed;
            UserToken userToken = new UserToken();
            userToken.username = "Mtt";
            userToken.Socket = e.AcceptSocket;
            userToken.ConnectTime = DateTime.Now;
            userToken.Remote = e.AcceptSocket.RemoteEndPoint;
            userToken.IPAddress = ((IPEndPoint)(e.AcceptSocket.RemoteEndPoint)).Address;
            id2players[userToken.Guid] = userToken;
            acceptArgs.UserToken = userToken;
            acceptArgs.SetBuffer(new byte[1024], 0, 1024);
            if (!e.AcceptSocket.ReceiveAsync(acceptArgs))
            {
                ProcessReceive(acceptArgs);
            }
            StartAccept(e);
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // check if the remote host closed the connection  
                UserToken token = (UserToken)e.UserToken;
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据  
                    byte[] data = new byte[e.BytesTransferred];
                    Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                    lock (token.Buffer)
                    {
                        token.Buffer.AddRange(data);
                    }
                    do
                    {
                        //判断包的长度  
                        byte[] lenBytes = token.Buffer.GetRange(0, 4).ToArray();
                        int packageLen = BitConverter.ToInt32(lenBytes, 0);
                        if (packageLen > token.Buffer.Count - 4)
                        {   //长度不够时,退出循环,让程序继续接收  
                            break;
                        }

                        //包够长时,则提取出来,交给后面的程序去处理  
                        byte[] rev = token.Buffer.GetRange(4, packageLen).ToArray();
                        //从数据池中移除这组数据  
                        lock (token.Buffer)
                        {
                            token.Buffer.RemoveRange(0, packageLen + 4);
                        }
                        ReceiveClientData?.Invoke(token, rev);

                    } while (token.Buffer.Count > 4);
                    if (!token.Socket.ReceiveAsync(e))
                        this.ProcessReceive(e);
                }
                else
                {
                    //CloseClientSocket(e);
                }
            }
            catch (Exception xe)
            {
                Console.WriteLine(xe.Message + "\r\n" + xe.StackTrace);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // done echoing data back to the client  
                UserToken token = (UserToken)e.UserToken;
                // read the next block of data send from the client  
                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                //CloseClientSocket(e);
            }
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler  
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        #endregion

        /// <summary>
        /// 对房间所有的玩家发送消息
        /// </summary>
        /// <param name="room"></param>
        /// <param name="baseNetObject"></param>
        public void SendDataToRoomAllPlayer(BaseRoom room,BaseNetObject baseNetObject)
        {
            SendDataToRoomPlayer(room, baseNetObject,new List<Guid>());
        }

        /// <summary>
        /// 对房间所有的玩家发送消息
        /// </summary>
        /// <param name="room"></param>
        /// <param name="baseNetObject"></param>
        public void SendDataToRoomPlayer(BaseRoom room, BaseNetObject baseNetObject,List<Guid> notSends)
        {
            foreach (var playerId in room.Players)
            {
                if (!notSends.Contains(playerId) && playerId != Guid.Empty)
                {
                    //发送玩家加入消息
                    SendData(id2players[playerId], baseNetObject);
                }
            }
        }

        /// <summary>
        /// 对token发送baseNetObject
        /// </summary>
        /// <param name="token">用户</param>
        /// <param name="baseNetObject">消息</param>
        /// <returns></returns>
        public bool SendData(UserToken token, BaseNetObject baseNetObject)
        {
            return SendData(token, NetBaseTool.ObjectToBytes(baseNetObject));
        }

        /// <summary>  
        /// 对数据进行打包,然后再发送  
        /// </summary>  
        /// <param name="token"></param>  
        /// <param name="message"></param>  
        /// <returns></returns>  
        public bool SendData(UserToken token, byte[] message)
        {
            bool isSuccess = false;
            if (token == null || token.Socket == null || !token.Socket.Connected)
                return isSuccess;

            try
            {
                //对要发送的消息,制定简单协议,头4字节指定包的大小,方便客户端接收(协议可以自己定)  
                byte[] buff = new byte[message.Length + 4];
                byte[] len = BitConverter.GetBytes(message.Length);
                Array.Copy(len, buff, 4);
                Array.Copy(message, 0, buff, 4, message.Length);
                //token.Socket.Send(buff);  //
                //新建异步发送对象, 发送消息  
                SocketAsyncEventArgs sendArg = new SocketAsyncEventArgs();
                sendArg.UserToken = token;
                sendArg.SetBuffer(buff, 0, buff.Length);  //将数据放置进去.  
                isSuccess = token.Socket.SendAsync(sendArg);
            }
            catch (Exception e)
            {
                Console.WriteLine("SendMessage - Error:" + e.Message);
            }
            return isSuccess;
        }

    }
}