using System;
using System.Collections.Generic;
using System.Linq;
using Sample;
using Common;
using Common.Tools;
using Common.Room;
using Common.NetObject;

namespace TF.GameClient
{
    class MainProgram
    {
        static void Main(string[] args)
        {

            GameClient gameClient = new GameClient("127.0.0.1", 1234, Handle);
            var socketError = gameClient.Connent();
            gameClient.PlayerJoin += (id) => { Console.WriteLine("Player: " + id + " Join"); };
            gameClient.PlayerLeave += (id) => { Console.WriteLine("Player: " + id + " Leave"); };
            gameClient.StartAction += () => Console.WriteLine("GameStart");
            while (true)
            {
                try
                {
                    Console.WriteLine("Input Command");
                    string s = Console.ReadLine();
                    switch (s)
                    {
                        case "SendMsg":
                            Console.Write("Input Message : ");
                            gameClient.SendMsg(Console.ReadLine());
                            break;
                        case "CreateRoom":
                            Console.Write("Input ID : ");
                            int id = int.Parse(Console.ReadLine());
                            SampleRoomDesc srd = new SampleRoomDesc(id);
                            var playerInRoom = gameClient.CreateRoom(srd, string.Empty);      
                            Console.WriteLine("Success " + playerInRoom + "RoomId : " + srd.ID);
                            break;
                        case "JoinRoom":
                            Console.WriteLine("input RoomId");
                            int rid = int.Parse(Console.ReadLine());
                            int playerId;
                            var b = gameClient.JoinRoom(rid, out playerId);
                            Console.WriteLine("JoinRoom " + b + "\n playerId : " + playerId);
                            Console.WriteLine(StringTools.ArrayToString(b.UserDatas));
                            break;
                        case "GetRoomList":
                            Console.WriteLine("Searching");
                            List<RoomState> rooms = gameClient.GetRoomList();
                            Console.WriteLine("Find " + rooms.Count + " Room");
                            foreach (var baseRoom in rooms)
                            {
                                Console.WriteLine(baseRoom.ToString());
                            }
                            break;
                        case "LeaveRoom":
                            gameClient.LeaveRoom();
                            break;
                        case "Start":
                            gameClient.GameStart();
                            break;
                        case "ChangeUserData":
                            Console.WriteLine("Input Your Name");
                            string name = Console.ReadLine();
                            gameClient.ChangeName(name);
                            break;
                        default:
                            Console.WriteLine("Error Command");
                            break;
                    }
                    Console.WriteLine("-----------");
                    Console.WriteLine("-----------");
                    Console.WriteLine("-----------");
                }
                catch (Exception e) { Console.WriteLine(e); }
            }
        }
        static void Handle(GameNetObject hauntedHouseNetObject)
        {
            Console.WriteLine(hauntedHouseNetObject);
        }
    }
}
