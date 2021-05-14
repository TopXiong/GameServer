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
using GameServer.Core.Log;

namespace GameServer
{

    class MainProgram
    {
        static void Main(string[] args)
        {
            Server.EventSystem.LoadAssembly(typeof(Server).Assembly);
            Server.EventSystem.LoadAssembly(typeof(Sample.Sample).Assembly);
            Server.Awake();
            Server.Start();
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    Server.Update();
                    Server.LateUpdate();
                }
                catch (Exception e)
                {
                    Logger.WriteException(e);
                }
            }
        }

    }
}
