using System;
using System.Collections.Generic;
using HauntedHouse;
using TF.Tools;

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
                            HauntedHouseRoom dpr = new HauntedHouseRoom(int.Parse(Console.ReadLine()));
                            var bb = gameClient.CreateRoom(dpr);
                            Console.WriteLine("CreateRoom Success? " + bb);
                            Console.WriteLine("Success " + bb + "RoomId : " + dpr.Id);
                            break;
                        case "JoinRoom":
                            Console.WriteLine("input RoomId");
                            int id = int.Parse(Console.ReadLine());
                            int playerId;
                            var b = gameClient.JoinRoom(id,out playerId);
                            Console.WriteLine("JoinRoom " + b + "\n playerId : " + playerId);
                            break;
                        case "GetRoomList":
                            Console.WriteLine("Searching");
                            List<BaseRoom> rooms = gameClient.GetRoomList();
                            Console.WriteLine("Find " + rooms.Count + " Room");
                            foreach (var baseRoom in rooms)
                            {
                                Console.WriteLine(baseRoom.ToString());
                            }
                            break;
                        case "LeaveRoom":
                            gameClient.LeaveRoom();
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
        static void Handle(HauntedHouseNetObject hauntedHouseNetObject)
        {
            Console.WriteLine(hauntedHouseNetObject);
        }
    }
}
