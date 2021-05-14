using GameServer.Core.Component;
using GameServer.Core.Interface;
using System;
using System.Collections.Generic;


namespace GameServer.Core.Game
{
    public sealed class GameObject
    {

        public GameObject Parent { get; private set; }

        public List<GameObject> Child { get; }

        public void ParentSet(GameObject parent)
        {
            parent.Child.Add(this);
            Parent = parent;
            Scene.Instance.AddGameObject(this);
        }

        public string Name { get; set; }

        private List<BaseComponentObject> m_baseComponentObjects;

        public GameObject(string name = "GameObject")
        {
            Name = name;
            m_baseComponentObjects = new List<BaseComponentObject>();
            Child = new List<GameObject>();
        }

        public T AddComponent<T>()
            where T : BaseComponentObject
        {
            T t = EventSystem.Instance.GetMonoInterface<T>();
            m_baseComponentObjects.Add(t);
            return t;
        }

        public BaseComponentObject AddComponent(Type type)
        {
            BaseComponentObject bc = (BaseComponentObject)EventSystem.Instance.GetMonoInterface(type);
            m_baseComponentObjects.Add(bc);
            return bc;
        }

        public T GetComponent<T>()
            where T : BaseComponentObject
        {
            foreach(var component in m_baseComponentObjects)
            {
                if(component.GetType() == typeof(T))
                {
                    return (T)component;
                }
            }
            return null;
        }

    }
}
