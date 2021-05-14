using Common.NetObject;
using GameServer.Core.Attribute;
using GameServer.Core.Component;
using GameServer.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Game
{
    /// <summary>
    /// 场景，管理所有的脚本更新，和GameObject层次
    /// TODO 改回使用构造器创建
    /// </summary>
    public class Scene:IMonoInterface
    {
        public static Scene Instance => m_instance;

        private static Scene m_instance;

        private List<GameObject> m_gameObjects;

        public int PlayerCount { get; private set; }

        public GameType GameType;

        public void AddGameObject(GameObject gameObject)
        {
            m_gameObjects.Add(gameObject);
        }


        public void Init(int playerCount,GameType gameType)
        {
            PlayerCount = playerCount;
            GameType = gameType;
        }

        public virtual void DateHandle(int i,GameNetObject gno)
        {
            Console.WriteLine("PlayerNum : "  + i + " Send " + gno);
        }

        public void Awake()
        {
            m_instance = this;
            m_gameObjects = new List<GameObject>();
            
            GameObject gameObject = new GameObject("Manager");
            AddGameObject(gameObject);
            var types = EventSystem.Instance.GetTypesByAttribute(new GameManagerAttribute(GameType));
            Type t = types.First();
        }

        public void LateUpdate()
        {

        }

        public void Start()
        {

        }

        public void Update()
        {

        }
    }
}
