using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using Tools.HauntedHouse;
namespace GameClient
{
    public class GameClient
    {
        private EventWaitHandle wait;
        /// <summary>
        /// 玩家加入事件
        /// </summary>
        public event Action<Guid> PlayerJoin;
        /// <summary>
        /// 玩家离开事件
        /// </summary>
        public event Action<Guid> PlayerLeave;

        private ClientSocket m_clientSocket;

        private object transmit = null;

        /// <summary>
        /// 收到数据后的回调
        /// </summary>
        private Action<HauntedHouseNetObject> m_action;


        /// <summary>
        /// IP,端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="action">收到数据后的回调</param>
        public GameClient(String ip, Int32 port, Action<HauntedHouseNetObject> action)
        {
            m_clientSocket = new ClientSocket(ip, port);
            m_action = action;
            m_clientSocket.ServerDataHandler += Datahandle;
            wait = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public void Datahandle(byte[] bytes)
        {
            BaseNetObject bno = NetBaseTool.BytesToObject(bytes) as BaseNetObject;
            if (bno.m_netObjectType == NetObjectType.SystemNetObject)
            {
                SystemNetObject systemNetObject = bno as SystemNetObject;
                if(systemNetObject.GetType() == typeof(Msg))
                {
                    Console.WriteLine(systemNetObject);
                }
                else if (systemNetObject.GetType() == typeof(CreateRoomS2C))
                {
                    transmit = (systemNetObject as CreateRoomS2C).Success;
                    wait.Set();
                }
                else if (systemNetObject.GetType() == typeof(JoinRoomS2C))
                {
                    transmit = (systemNetObject as JoinRoomS2C).Success;
                    wait.Set();
                }
                else if (systemNetObject.GetType() == typeof(GetRoomListS2C))
                {
                    transmit = (systemNetObject as GetRoomListS2C).rooms;
                    wait.Set();
                }
                else if (systemNetObject.GetType() == typeof(PlayerJoinS2C))
                {
                    PlayerJoin?.Invoke((systemNetObject as PlayerJoinS2C).playerId);
                }
                else if (systemNetObject.GetType() == typeof(PlayerLeaveS2C))
                {
                    PlayerLeave?.Invoke((systemNetObject as PlayerLeaveS2C).playerId);
                }
            }
            else
            {
                HauntedHouseNetObject hauntedHouseNetObject = bno as HauntedHouseNetObject;
                if (hauntedHouseNetObject == null)
                {
                    throw new ArgumentException(bno.m_netObjectType + " Can't Used");
                }
                m_action(hauntedHouseNetObject);
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <returns>SocketError.Success标识成功</returns>
        public SocketError Connent()
        {
            return m_clientSocket.Connent();
        }

        /// <summary>
        /// 构建一个HauntedHouseNetObject对象并发送,HauntedHouseNetObject类型放在Tools/Games/HauntedHouse下
        /// </summary>
        /// <param name="hauntedHouseNetObject"></param>
        public void Send(HauntedHouseNetObject hauntedHouseNetObject)
        {
            Send(hauntedHouseNetObject);
        }

        private void Send(BaseNetObject baseNetObject)
        {
            m_clientSocket.Send(NetBaseTool.ObjectToBytes(baseNetObject));
        }

        #region SystemNetObject
        


        /// <summary>
        /// Debug 用发送消息
        /// </summary>
        /// <param name="s"></param>
        public void SendMsg(string s)
        {
            Send(new Msg(s));
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <returns>返回带ID号的房间</returns>
        public void LeaveRoom()
        {
            Send(new LeaveRoomC2S());
        }

        /// <summary>
        /// 获取房间List
        /// </summary>
        /// <returns>房间List</returns>
        public List<BaseRoom> GetRoomList()
        {
            Send(new GetRoomListC2S());
            wait.WaitOne();
            return (List<BaseRoom>)transmit;
        }

        /// <summary>
        /// 创建房间，交给服务器查询房间号是否重复
        /// </summary>
        /// <param name="room">房间号及其配置</param>
        /// <returns>是否创建成功</returns>
        public bool CreateRoom(BaseRoom room)
        {
            CreateRoomC2S createRoom = new CreateRoomC2S(room);
            Console.WriteLine("Reade to Send createRoom");
            Send(createRoom);
            Console.WriteLine("Send createRoom");
            wait.WaitOne();
            Console.WriteLine("Recevice createRoom");
            return ((bool)transmit);
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="room">房间号</param>
        /// <returns>返回带ID号的房间</returns>
        public bool JoinRoom(int roomID, string password = "")
        {
            JoinRoomC2S joinRoom = new JoinRoomC2S(roomID, password);
            Send(joinRoom);
            wait.WaitOne();
            return (bool)transmit;
        }

        #endregion

    }
}
