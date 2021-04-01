using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetWorkTools;
using GameServer.Log;
using System.Collections.Generic;
using GameServer.Core.Tools;

namespace GameServer
{
    class GameServer
    {
        //  玩家通信管道池
        static ChannelPool channels = new ChannelPool();
        //  玩家ID号
        static HashTable<Channel> idToChannel = new HashTable<Channel>();
        //  房间ID-房间
        static HashTable<BaseRoom> rooms = new HashTable<BaseRoom>();
        //  玩家所在房间号
        static Dictionary<Channel, int> channelToRoomId = new Dictionary<Channel, int>();

        //static void Main(string[] args)
        //{
        //    IPAddress ipadr = IPAddress.Parse("0.0.0.0");
        //    IPEndPoint iPEnd = new IPEndPoint(ipadr, 1234);
        //    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    serverSocket.Bind(iPEnd);
        //    serverSocket.Listen(20);
        //    ThreadPool.QueueUserWorkItem(new WaitCallback(Listen), serverSocket);
        //    Console.ReadLine();
        //}


        static void Listen(object o)
        {
            var serverSocket = o as Socket;
            while (true)
            {
                
                Socket client = serverSocket.Accept();
                string clientip = client.RemoteEndPoint.ToString();
                Logger.WriteLog("收到 ："+clientip+" 的连接");                
                Channel channel = channels.Get(client,false);
                idToChannel.Add(channel);
                ThreadPool.QueueUserWorkItem(new WaitCallback(Handle), channel);
            }
        }

        static void Handle(object o)
        {
            Channel channel = o as Channel;
            while (true)
            {
                if (channel.ReceiveBaseNetObjects.Count > 0)
                {
                    Console.WriteLine("Receive msg");
                    BaseNetObject baseNetObject = channel.ReceiveBaseNetObjects.Dequeue();
                    //String 消息——————测试用
                    if(baseNetObject.GetType() == typeof(Msg))
                    {
                        Msg msg = baseNetObject as Msg;
                        msg.context = "Receive: " + msg.context;
                        channel.SendBaseNetObjects.Enqueue(msg);
                    }
                    // 创建房间
                    else if (baseNetObject.GetType() == typeof(CreateRoom))
                    {
                        CreateRoom createRoom = baseNetObject as CreateRoom;
                        int id = rooms.Add(createRoom.room);
                        rooms.GetByID(id).Id = id;
                        createRoom.room.Id = id;
                        createRoom.room.PlayerJoin(idToChannel.GetID(channel));
                        channel.SendBaseNetObjects.Enqueue(createRoom);
                        channelToRoomId.Add(channel, id);                        
                    }
                    // 加入房间
                    else if (baseNetObject.GetType() == typeof(JoinRoomC2S))
                    {
                        // Cheak if in room
                        if(channelToRoomId.ContainsKey(channel))
                        {
                            BaseRoom baseRoom = rooms.GetByID(channelToRoomId[channel]);
                            baseRoom.PlayerLeave(channelToRoomId[channel]);
                            channelToRoomId.Remove(channel);
                            foreach (int playerId in baseRoom.Players)
                            {
                                if (playerId != 0)
                                {
                                    idToChannel.GetByID(playerId).SendBaseNetObjects.Enqueue(new PlayerLeaveS2C(channelToRoomId[channel]));
                                }
                            }
                        }

                        JoinRoomC2S jr = baseNetObject as JoinRoomC2S;
                        BaseRoom room = rooms.GetByID(jr.roomId);
                        int currentPlayId = idToChannel.GetID(channel);

                        if (!room.Password.Equals(jr.password))
                        {
                            channel.SendBaseNetObjects.Enqueue(new JoinRoomS2C(false));
                            continue;
                        }
                        //加入是否成功
                        bool found = room.PlayerJoin(currentPlayId);

                        if (found)
                        {
                            channelToRoomId.Add(channel, jr.roomId);
                            foreach(int playerId in room.Players)
                            {
                                Console.WriteLine(playerId + "---" + currentPlayId);
                                if(currentPlayId != playerId && playerId!=0)
                                {
                                    idToChannel.GetByID(playerId).SendBaseNetObjects.Enqueue(new PlayerJoinS2C(currentPlayId));
                                }
                            }
                        }
                        channel.SendBaseNetObjects.Enqueue(new JoinRoomS2C(found));
                        
                    }
                    //返回房间列表
                    else if (baseNetObject.GetType() == typeof(GetRoomListC2S))
                    {
                        GetRoomListS2C grl = new GetRoomListS2C(rooms.GetList());
                        channel.SendBaseNetObjects.Enqueue(grl);
                    }
                    //离开房间
                    else if (baseNetObject.GetType() == typeof(LeaveRoomC2S))
                    {
                        int currentPlayId = idToChannel.GetID(channel);
                        BaseRoom baseRoom = rooms.GetByID(channelToRoomId[channel]);
                        baseRoom.PlayerLeave(currentPlayId);
                        channelToRoomId.Remove(channel);
                        foreach (int playerId in baseRoom.Players)
                        {
                            if (playerId != 0)
                            {
                                idToChannel.GetByID(playerId).SendBaseNetObjects.Enqueue(new PlayerLeaveS2C(currentPlayId));
                            }                            
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }

}
