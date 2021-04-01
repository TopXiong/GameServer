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
        private readonly Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
        /// <summary>
        /// 特性类型对应的类型
        /// </summary>
        private readonly UnOrderMultiMapSet<Type,Type> typesByAttribute = new UnOrderMultiMapSet<Type, Type>();
        /// <summary>
        /// 系统接口对应的实例
        /// </summary>    
        private readonly List<BaseGameObject> systemObjects = new List<BaseGameObject>();

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
            assemblies[assembly.ManifestModule.ScopeName] = assembly;
            //重新加载types表
            typesByAttribute.Clear();
            //遍历所有程序集
            foreach (var value in assemblies.Values)
            {
                //遍历所有类型
                foreach (var type in value.GetTypes())
                {
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    if (type.IsAssignableFrom(typeof(BaseGameObject)))
                    {
                        systemObjects.Add((BaseGameObject)Activator.CreateInstance(type));
                    }

                    //加到特性表
                    var objects =  type.GetCustomAttributes<BaseAttribute>(true);
                    foreach (var baseAttribute in objects)
                    {
                        typesByAttribute.Add(baseAttribute.AttributeType,type);
                    }
                }
            }

            Logger.WriteLog(GetTypesByAttribute(typeof(ObjectSystemAttribute)).Count);

            foreach (Type type in this.GetTypesByAttribute(typeof(ObjectSystemAttribute)))
            {

            }

        }


        /// <summary>
        /// 获取程序集
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Assembly GetAssembly(string name)
        {
            return this.assemblies[name];
        }

        /// <summary>
        /// 获取实现了对应特性的全部类型
        /// </summary>
        /// <param name="systemAttributeType"></param>
        /// <returns></returns>
        public HashSet<Type> GetTypesByAttribute(Type systemAttributeType)
        {
            if (!this.typesByAttribute.ContainsKey(systemAttributeType))
            {
                return new HashSet<Type>();
            }
            return typesByAttribute[systemAttributeType];
        }
        /// <summary>
        /// 获取全部程序集的所有类型
        /// </summary>
        /// <returns></returns>
        public List<Type> GetTypesByAttribute()
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in this.assemblies.Values)
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
            foreach (Assembly assembly in this.assemblies.Values)
            {
                allTypes.AddRange(assembly.GetTypes());
            }
            return allTypes;
        }

    }
}
