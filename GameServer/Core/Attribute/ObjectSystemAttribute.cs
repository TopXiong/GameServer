using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.Base;

namespace GameServer.Core.Attribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObjectSystemAttribute : BaseAttribute
    {
    }
}
