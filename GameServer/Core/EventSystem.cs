using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GameServer.Core.Attribute;
using GameServer.Core.Base;
using GameServer.Core.Interface;
using GameServer.Core.Tools;
using GameServer.Log;

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
        private readonly UnOrderMultiMapSet<Type,Type> m_typesByAttribute = new UnOrderMultiMapSet<Type, Type>();
        /// <summary>
        /// 系统接口对应的实例
        /// </summary>    
        private readonly List<BaseGameObject> m_systemObjects = new List<BaseGameObject>();

        /// <summary>
        /// 系统接口对应的实例
        /// </summary>    
        private readonly List<IAwakeInterface> m_awakeList = new List<IAwakeInterface>();

        /// <summary>
        /// 系统接口对应的实例
        /// </summary>    
        private readonly List<IStartInterface> m_startList = new List<IStartInterface>();

        /// <summary>
        /// 系统接口对应的实例
        /// </summary>    
        private readonly List<IUpdateInterface> m_updateList = new List<IUpdateInterface>();

        /// <summary>
        /// 系统接口对应的实例
        /// </summary>    
        private readonly List<ILateUpdateInterface> m_lateUpdateList = new List<ILateUpdateInterface>();



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

                    //加载系统管理的对象，并创建对象
                    if (typeof(BaseGameObject).IsAssignableFrom(type))
                    {
                        m_systemObjects.Add((BaseGameObject)Activator.CreateInstance(type));
                    }

                    //加到特性表
                    var objects =  type.GetCustomAttributes<BaseAttribute>(true);
                    foreach (var baseAttribute in objects)
                    {
                        m_typesByAttribute.Add(baseAttribute.AttributeType,type);
                    }
                }
            }

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
        public HashSet<Type> GetTypesByAttribute(Type systemAttributeType)
        {
            if (!this.m_typesByAttribute.ContainsKey(systemAttributeType))
            {
                return new HashSet<Type>();
            }
            return m_typesByAttribute[systemAttributeType];
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

        public void Awake()
        {
            foreach(var item in m_systemObjects)
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
        }

        public void LateUpdate()
        {
            foreach (var item in m_systemObjects)
            {
                item.LateUpdate();
            }
        }

    }
}
