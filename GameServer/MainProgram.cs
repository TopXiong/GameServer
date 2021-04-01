using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core;

namespace GameServer
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true,Inherited =true)]
    class Att:Attribute
    {

    }
    [Att]
    interface baseInter { }
    interface Inter:baseInter { }

    class MainProgram: Inter
    {
        static void Main(string[] args)
        {
            Game.EventSystem.LoadAssembly(typeof(Game).Assembly);
            
            foreach (var item in typeof(Inter ).CustomAttributes)
            {
                Console.WriteLine(item);
            }


            Console.ReadLine();
        }

    }
}
