using Common.NetObject;
using GameServer.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Attribute
{
    /// <summary>
    /// 场景创建时会将此manager组件加载在manager GameObject上
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true,Inherited = true)]
    public class GameManagerAttribute: BaseAttribute
    {
        public GameType GameType;

        public GameManagerAttribute(GameType gameType)
        {
            GameType = gameType;
        }
    }
}
