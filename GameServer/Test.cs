using GameServer.Core.Attribute;
using GameServer.Core.Interface;
using GameServer.Log;

namespace GameServer
{
    [ObjectSystem]
    public class Test
    {
        public void Start()
        {
            Logger.WriteLog("Test Start");
        }
    }
}