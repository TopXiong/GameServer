using GameServer.Core.Attribute;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Core.Base
{
    [ObjectSystem]
    public class BaseGameObject
    {
        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void Update() { }
        protected virtual void LateUpdate() { }
    }
}
