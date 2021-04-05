using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using NetWorkTools;
using NetWorkTools.Games.DirtyPig;

namespace GameClient
{
    public class GameClient
    {
        private EventWaitHandle wait;
        private Channel channel;

        public delegate void RoomPlayer(int id);
        /// <summary>
        /// 玩家加入事件
        /// </summary>
        public event RoomPlayer PlayerJoin;
        /// <summary>
        /// 玩家离开事件
        /// </summary>
        public event RoomPlayer PlayerLeave;

        private object transmit =null;

        public GameClient(string ip,int port)
        {
            wait = new EventWaitHandle(false, EventResetMode.AutoReset);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ip, port);
            channel = new Channel(clientSocket, false);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveHandle));
        }

        public void SendMsg(string s)
        {
            Msg m = new Msg(s);
            channel.SendBaseNetObjects.Enqueue(m);
        }

        /// <summary>
        /// 创建房间，交给服务器配置房间ID
        /// </summary>
        /// <param name="room">房间除了ID以外的配置</param>
        /// <returns>返回带ID号的房间</returns>
        public void LeaveRoom()
        {
            LeaveRoomC2S leaveRoom = new LeaveRoomC2S();
            channel.SendBaseNetObjects.Enqueue(leaveRoom);
        }

        /// <summary>
        /// 获取房间List
        /// </summary>
        /// <returns>房间List</returns>
        public List<BaseRoom> GetRoomList()
        {
            GetRoomListC2S netObject = new GetRoomListC2S();
            channel.SendBaseNetObjects.Enqueue(netObject);
            wait.WaitOne();
            return (List<BaseRoom>)transmit;
        }

        /// <summary>
        /// 创建房间，交给服务器配置房间ID
        /// </summary>
        /// <param name="room">房间除了ID以外的配置</param>
        /// <returns>返回带ID号的房间</returns>
        public BaseRoom CreateRoom(BaseRoom room)
        {
            CreateRoom createRoom = new CreateRoom(room);
            channel.SendBaseNetObjects.Enqueue(createRoom);
            wait.WaitOne();
            return (BaseRoom)transmit;
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="room">房间号</param>
        /// <returns>返回带ID号的房间</returns>
        public bool JoinRoom(int roomID,string password = "")
        {
            JoinRoomC2S joinRoom = new JoinRoomC2S(roomID,password);
            channel.SendBaseNetObjects.Enqueue(joinRoom);
            wait.WaitOne();
            return (bool)transmit;
        }

        private void ReceiveHandle(object o)
        {
            while (true)
            {
                if (channel.ReceiveBaseNetObjects.Count > 0)
                {
                    BaseNetObject baseNetObject = channel.ReceiveBaseNetObjects.Dequeue();
                    if (baseNetObject.GetType() == typeof(Msg))
                    {
                        Msg msg = baseNetObject as Msg;
                        Console.WriteLine("Receive Message : "+msg.context);
                    }else if (baseNetObject.GetType() == typeof(CreateRoom))
                    {
                        transmit = (baseNetObject as CreateRoom).room;
                        wait.Set();
                    }
                    else if (baseNetObject.GetType() == typeof(JoinRoomS2C))
                    {
                        transmit = (baseNetObject as JoinRoomS2C).success;
                        wait.Set();
                    }
                    else if (baseNetObject.GetType() == typeof(GetRoomListS2C))
                    {
                        transmit = (baseNetObject as GetRoomListS2C).rooms;                       
                        wait.Set();
                    }
                    else if (baseNetObject.GetType() == typeof(PlayerJoinS2C))
                    {
                        PlayerJoin?.Invoke((baseNetObject as PlayerJoinS2C).playerId);
                    }
                    else if (baseNetObject.GetType() == typeof(PlayerLeaveS2C))
                    {
                        PlayerLeave?.Invoke((baseNetObject as PlayerLeaveS2C).playerId);
                    }
                }
                Thread.Sleep(1000);
            }
        }


        static void Main(string[] args)
        {
            SocketAsyncEventArgs Args = new SocketAsyncEventArgs();
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", 1234);
            Console.WriteLine("Start");
            SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
            while (true)
            {
                Console.Write("Input Message : ");
            }
            /*
            GameClient gameClient = new GameClient("127.0.0.1", 1234);
           // GameClient gameClient = new GameClient("192.168.1.106", 1234);
            gameClient.PlayerJoin += delegate (int id) { Console.WriteLine("Player: " + id + "Join"); };
            gameClient.PlayerLeave +=  (id) => { Console.WriteLine("Player: " + id + "Leave"); };
            while (true)
            {
                string s = Console.ReadLine();
                switch (s)
                {
                    case "SendMsg":
                        Console.Write("Input Message : ");
                        gameClient.SendMsg(Console.ReadLine());
                        break;
                    case "CreateRoom":
                        DirtyPigRoom dpr = new DirtyPigRoom(3, 4, 3, string.Empty);
                        dpr = (DirtyPigRoom)gameClient.CreateRoom(dpr);
                        Console.WriteLine("RoomId : "+dpr.Id);
                        break;
                    case "JoinRoom":
                        Console.WriteLine("input RoomId");
                        int id = int.Parse(Console.ReadLine());
                        bool b= gameClient.JoinRoom(id);
                        Console.WriteLine(b);
                        break;
                    case "GetRoomList":
                        Console.WriteLine("Searching");
                        List<BaseRoom> rooms = gameClient.GetRoomList();
                        Console.WriteLine("Find " + rooms.Count + " Room");
                        foreach(var baseRoom in rooms)
                        {
                            Console.WriteLine(baseRoom.ToString());
                        }
                        break;
                    case "LeaveRoom":
                        gameClient.LeaveRoom();
                        break;

                }
            
            }
            */
        }
    }
}
