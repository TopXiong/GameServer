using Common.NetObject;
using GameServer.Core.Attribute;
using GameServer.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Core.Component
{
    public abstract class BaseComponentObject : IMonoInterface
    {

        private Guid ComponentID;

        public virtual void DataHandle(ComponentSynNetObject csno) { }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
    }
}
