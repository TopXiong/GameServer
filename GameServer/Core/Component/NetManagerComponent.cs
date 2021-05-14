using Common.NetObject;
using GameServer.Core.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Component
{
    /// <summary>
    /// 在每个场景初始化时会绑定在Manager物体上
    /// 通过这个脚本同步物体
    /// </summary>
    [GameManager(GameType.Sample)]
    public abstract class NetManagerComponent : BaseComponentObject
    {
        public abstract void DataHandle(GameNetObject gno);

    }
}
