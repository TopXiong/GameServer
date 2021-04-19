using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameServer.Core;
using GameServer.Core.NetWork;
using GameServer.Core.Tools;
using GameServer.Log;
using Lister13;

namespace GameServer
{

    class MainProgram
    {
        static void Main(string[] args)
        {
            //Server server = new Server(10, 10);
            //server.Init();
            //server.Start(new IPEndPoint(IPAddress.Any, 1234));
            //server.ReceiveClientData += (AsyncUserToken token, byte[] buff) =>
            //{
            //    Console.WriteLine(Encoding.ASCII.GetString(buff));
            //    server.SendMessage(token, Encoding.ASCII.GetBytes("Got it"));
            //};
            //Console.Read();
            Game.EventSystem.LoadAssembly(typeof(Game).Assembly);
            Game.Awake();
            Game.Start();
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    Game.Update();
                    Game.LateUpdate();
                }
                catch (Exception e)
                {
                    Logger.WriteException(e);
                }
            }
        }

    }
}
