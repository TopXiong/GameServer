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
using GameServer;
using Lister13;
using TF.Log;

namespace GameServer
{

    class MainProgram
    {
        static void Main(string[] args)
        {
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
