using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GameServer.Core;
using GameServer.Log;

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
