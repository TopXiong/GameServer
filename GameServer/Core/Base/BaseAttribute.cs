using System;

namespace GameServer.Core.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BaseAttribute : System.Attribute
    {
        public Type AttributeType { get; }

        public BaseAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }
}