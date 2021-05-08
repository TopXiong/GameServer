using GameServer.Core.NetWork;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameServer.Core
{
    public static class Game
    {
        public static EventSystem EventSystem => EventSystem.Instance;

        public static ServerService serverService = new ServerService();

        //static PhysXTestEngine engine = new PhysXTestEngine();


        public static void Awake()
        {
            //engine.Awake();
            EventSystem.Awake();
        }

        public static void Start()
        {
            EventSystem.Start();
        }

        public static void Update()
        {
            //engine.Update();
            EventSystem.Update();
        }

        public static void LateUpdate()
        {
            EventSystem.LateUpdate();
        }



    }
}