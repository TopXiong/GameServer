using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TF.Tools;
using Timer = System.Timers.Timer;
using TF.Log;
namespace TF.GameClient
{
    public class GameClient
    {

        private const float WaitTime = 5f;
        //private Timer waitTimer;
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
        private Action<GameNetObject> m_action;

        public Guid MyID { get; private set; }

        /// <summary>
        /// IP,端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="action">收到数据后的回调</param>
        public GameClient(String ip, Int32 port, Action<GameNetObject> action)
        {
            m_clientSocket = new ClientSocket(ip, port);
            m_action = action;
            m_clientSocket.ServerDataHandler += Datahandle;
            //waitTimer = new Timer();
            //waitTimer.Interval = WaitTime;
            //waitTimer.Elapsed += (o, e) =>
            //{
            //    wait.Set();
            //    waitTimer.Stop();
            //};
            wait = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public void Datahandle(byte[] bytes)
        {
            BaseNetObject bno = NetBaseTool.BytesToObject(bytes) as BaseNetObject;
            if (bno is SystemNetObject)
            {
                SystemNetObject systemNetObject = bno as SystemNetObject;
                if(systemNetObject.GetType() == typeof(Msg))
                {
                    Logger.WriteLog(systemNetObject);
                }
                else if(systemNetObject.GetType() == typeof(GetMyID))
                {
                    MyID = (systemNetObject as GetMyID).playerId;
                }
                else if (systemNetObject.GetType() == typeof(CreateRoomS2C))
                {
                    transmit = (systemNetObject as CreateRoomS2C).PlayerId;
                    wait.Set();
                }
                else if (systemNetObject.GetType() == typeof(JoinRoomS2C))
                {
                    transmit = (systemNetObject as JoinRoomS2C);
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
                GameNetObject gno = bno as GameNetObject;
                if (gno == null)
                {
                    throw new ArgumentException(bno.GetType() + " Can't Used");
                }
                m_action(gno);
            }
            //防止死锁
            // wait.Set();
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
        public void Send(GameNetObject gno)
        {
            Send(gno);
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
        /// 登入后发送用户信息
        /// </summary>
        /// <param name="s"></param>
        //public void SendUserData(UserDate userData)
        //{
        //    Send(new SetUserData(userData));
        //}

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <returns></returns>
        public void LeaveRoom()
        {
            Send(new LeaveRoomC2S());
        }

        /// <summary>
        /// 获取房间List
        /// </summary>
        /// <returns>房间List</returns>
        public List<RoomState> GetRoomList()
        {
            Send(new GetRoomListC2S());
            //waitTimer.Start();
            wait.WaitOne();
            return (List<RoomState>)transmit;
        }

        /// <summary>
        /// 创建房间，交给服务器查询房间号是否重复
        /// </summary>
        /// <param name="room">房间号及其配置</param>
        /// <returns>在房间中的id,-1为不成功</returns>
        public int CreateRoom(RoomDesc room,string password)
        {
            CreateRoomC2S createRoom = new CreateRoomC2S(room, password);
            Send(createRoom);
            //waitTimer.Start();
            wait.WaitOne();
            return (int)transmit;
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="room">房间号</param>
        /// <returns>在房间中的id,-1为不成功</returns>
        public RoomState JoinRoom(int roomID,out int playerInRoomID, string password = "")
        {
            JoinRoomC2S joinRoom = new JoinRoomC2S(roomID, password);
            Send(joinRoom);
            //waitTimer.Start();
            wait.WaitOne();
            playerInRoomID = ((JoinRoomS2C)transmit).PlayerId;
            return ((JoinRoomS2C)transmit).Room;
        }

        #endregion

    }
}
