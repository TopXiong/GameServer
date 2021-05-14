using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GameServer.Core.Attribute;
using GameServer.Core.Base;
using GameServer.Core.Interface;
using GameServer.Core.Tools;
using GameServer;
using GameServer.Core.Game;
using GameServer.Core.Component;

namespace GameServer.Core
{
    public class EventSystem
    {

        private static EventSystem _sInstance;

        public static EventSystem Instance
        {
            get
            {
                return _sInstance ?? (_sInstance = new EventSystem());
            }
        }

        /// <summary>
        /// 加载的所有程序集
        /// </summary>
        private readonly Dictionary<string, Assembly> m_assemblies = new Dictionary<string, Assembly>();
        /// <summary>
        /// 特性类型对应的类型
        /// </summary>
        private readonly UnOrderMultiMapSet<BaseAttribute, Type> m_typesByAttribute = new UnOrderMultiMapSet<BaseAttribute, Type>();

        /// <summary>
        /// 系统单例对应的实例
        /// </summary>    
        private readonly List<BaseSystemManager> m_systemObjects = new List<BaseSystemManager>();

        /// <summary>
        /// 组件名对应组件类型
        /// </summary>
        private readonly Dictionary<string, Type> m_components = new Dictionary<string, Type>();

        /// <summary>
        /// 游戏接口对应的实例
        /// </summary>    
        private readonly List<IMonoInterface> m_awakeList = new List<IMonoInterface>();

        /// <summary>
        /// 游戏接口对应的实例
        /// </summary>    
        private readonly List<IMonoInterface> m_startList = new List<IMonoInterface>();

        /// <summary>
        /// 游戏接口对应的实例
        /// </summary>    
        private readonly List<IMonoInterface> m_updateList = new List<IMonoInterface>();

        /// <summary>
        /// 游戏接口对应的实例
        /// </summary>    
        private readonly List<IMonoInterface> m_lateUpdateList = new List<IMonoInterface>();



        private EventSystem()
        {
        }

        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="assembly"></param>
        public void LoadAssembly(Assembly assembly)
        {
            // 储存程序集
            m_assemblies[assembly.ManifestModule.ScopeName] = assembly;
            //重新加载types表
            m_typesByAttribute.Clear();
            //遍历所有程序集
            foreach (var value in m_assemblies.Values)
            {
                //遍历所有类型
                foreach (var type in value.GetTypes())
                {
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    //加载系统管理单例的对象，并创建对象
                    if (typeof(BaseSystemManager).IsAssignableFrom(type))
                    {
                        m_systemObjects.Add((BaseSystemManager)Activator.CreateInstance(type));
                    }

                    //加载系统管理单例的对象，并创建对象
                    if (typeof(BaseComponentObject).IsAssignableFrom(type))
                    {
                        m_components.Add(type.Name, type);
                    }

                    //加到特性表
                    var objects =  type.GetCustomAttributes<BaseAttribute>(true);
                    foreach (var baseAttribute in objects)
                    {
                        m_typesByAttribute.Add(baseAttribute,type);
                    }
                }
            }

        }
        
        /// <summary>
        /// 通过名字获取组件类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaseComponentObject GetComponent(string name)
        {
            if (m_components.ContainsKey(name))
            {
                return (BaseComponentObject)Activator.CreateInstance(m_components[name]);
            }
            return null;
        }

        /// <summary>
        /// 通过名字获取组件类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetComponent<T>()
            where T:BaseComponentObject
        {
            if (m_components.ContainsKey(nameof(T)))
            {
                return Activator.CreateInstance<T>();
            }
            return null;
        }

        /// <summary>
        /// 创建Mono的实例,并加入管理
        /// 等待执行Awake
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetMonoInterface<T>()
            where T: IMonoInterface
        {
            T t = Activator.CreateInstance<T>();
            m_awakeList.Add(t);
            return t;
        }

        /// <summary>
        /// 创建Mono的实例,并加入管理
        /// 等待执行Awake
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMonoInterface GetMonoInterface(Type type)
        {
            IMonoInterface mono = (IMonoInterface)Activator.CreateInstance(type);
            m_awakeList.Add(mono);
            return mono;
        }


        /// <summary>
        /// 获取程序集
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Assembly GetAssembly(string name)
        {
            return this.m_assemblies[name];
        }

        /// <summary>
        /// 获取实现了对应特性的全部类型
        /// </summary>
        /// <param name="systemAttributeType"></param>
        /// <returns></returns>
        public HashSet<Type> GetTypesByAttribute(BaseAttribute systemAttribute)
        {
            if (!this.m_typesByAttribute.ContainsKey(systemAttribute))
            {
                return new HashSet<Type>();
            }
            return m_typesByAttribute[systemAttribute];
        }
        /// <summary>
        /// 获取全部程序集的所有类型
        /// </summary>
        /// <returns></returns>
        public List<Type> GetTypesByAttribute()
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in this.m_assemblies.Values)
            {
                allTypes.AddRange(assembly.GetTypes());
            }
            return allTypes;
        }

        /// <summary>
        /// 获取全部程序集的所有类型
        /// </summary>
        /// <returns></returns>
        public List<Type> GetTypesByInterface()
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in this.m_assemblies.Values)
            {
                allTypes.AddRange(assembly.GetTypes());
            }
            return allTypes;
        }



        #region LifeLine

        public void Awake()
        {
            foreach (var item in m_systemObjects)
            {
                item.Awake();
            }           
        }

        public void Start()
        {
            foreach (var item in m_systemObjects)
            {
                item.Start();
            }
        }

        public void Update()
        {
            foreach (var item in m_systemObjects)
            {
                item.Update();
            }

            //单帧
            foreach (var item in m_awakeList)
            {
                item.Awake();
            }

            foreach (var item in m_startList)
            {
                item.Start();
            }

            foreach (var item in m_updateList)
            {
                item.Update();
            }
            // 更改生命周期
            //执行完Start后开始Update
            m_updateList.AddRange(m_startList);
            //清空start后，将awake加入到start并清理awake
            m_startList.Clear();
            m_startList.AddRange(m_awakeList);
            m_awakeList.Clear();
        }

        public void LateUpdate()
        {
            foreach (var item in m_systemObjects)
            {
                item.LateUpdate();
            }
            foreach (var item in m_lateUpdateList)
            {
                item.LateUpdate();
            }
        }
        #endregion
    }
}
