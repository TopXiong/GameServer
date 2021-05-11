﻿using System;
using System.Collections.Generic;
using System.Linq;
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
                            BaseRoom dpr = null;//= new BaseRoom(int.Parse(Console.ReadLine()));
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
                            Console.WriteLine(StringTools.ArrayToString(b.UserData));
                            break;
                        case "GetRoomList":
                            Console.WriteLine("Searching");
                            List<BaseRoom> rooms = gameClient.GetRoomList();
                            Console.WriteLine("Find " + rooms.Count + " Room");
                            foreach (var baseRoom in rooms)
                            {
                                Console.WriteLine(baseRoom.ToString());
                                Console.WriteLine(StringTools.ArrayToString(baseRoom.UserData));
                            }
                            break;
                        case "LeaveRoom":
                            gameClient.LeaveRoom();
                            break;
                        case "Start":
                            //gameClient.Send(new GameStartMessage());
                            break;
                        case "ChangeUserData":
                            Console.WriteLine("Input Your Name");
                            string name = Console.ReadLine();
                            //gameClient.SendUserData(new HauntedHouseUserData(name,EntityType.GhostCat));
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
