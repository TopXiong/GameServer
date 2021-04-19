using GameServer.Core.Attribute;
using GameServer.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Core.Base
{
    [ObjectSystem]
    public abstract class BaseGameObject : IAwakeInterface, IStartInterface,IUpdateInterface,ILateUpdateInterface
    {

        protected BaseGameObject m_instance;

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
    }
}
