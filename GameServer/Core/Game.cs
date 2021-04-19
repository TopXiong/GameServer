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

        public static void Awake()
        {
            EventSystem.Awake();
        }

        public static void Start()
        {
            EventSystem.Start();
        }

        public static void Update()
        {
            EventSystem.Update();
        }

        public static void LateUpdate()
        {
            EventSystem.LateUpdate();
        }



    }
}