using GameServer.Core.Attribute;
using GameServer.Core.Base;
using GameServer.Core.Interface;
using GameServer.Log;

namespace GameServer
{
    [ObjectSystem]
    public class Test: BaseGameObject
    {
        public override void Start()
        {
            Logger.WriteLog("Test Start");
        }
    }
}