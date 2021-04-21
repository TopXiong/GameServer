using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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
            Game.EventSystem.LoadAssembly(typeof(Game).Assembly);
            var assembly = Assembly.LoadFile(
                @"C:\Users\xiongshangfeng\source\repos\GameProject_A\SourceCodes\GameServer-master\HauntedHouse\bin\Debug\HauntedHouse.dll"); 
            Game.EventSystem.LoadAssembly(assembly);
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
