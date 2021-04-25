using System;
using System.Collections.Generic;
using System.Text;

namespace TF.Tools
{
    [Serializable]
    public enum NetObjectType
    {
        SystemNetObject,
        GameNetObject,
    }
    [Serializable]
    public enum GameType
    {
        DirtyPig,
        HauntedHouse,
    }
    /// <summary>
    /// 通讯信息基类
    /// </summary>
    [Serializable]
    public abstract class BaseNetObject
    {
        public NetObjectType m_netObjectType;
    }
    /// <summary>
    /// 系统消息基类
    /// </summary>
    [Serializable]
    public abstract class SystemNetObject : BaseNetObject
    {
        public SystemNetObject()
        {
            m_netObjectType = NetObjectType.SystemNetObject;
        }
    }
    [Serializable]
    public abstract class GameNetObject : BaseNetObject
    {
        public GameType gameType;
        public GameNetObject()
        {
            m_netObjectType = NetObjectType.GameNetObject;
        }
    }

    [Serializable]
    public class HauntedHouseNetObject : GameNetObject
    {
        public HauntedHouseNetObject()
        {
            gameType = GameType.HauntedHouse;
        }
    }

}
