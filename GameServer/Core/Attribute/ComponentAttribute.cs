using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.Base;

namespace GameServer.Core.Attribute
{
    /// <summary>
    /// 系统启动时候会创建此类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true,Inherited = true)]
    public class ComponentAttribute : BaseAttribute
    {
    }
}
