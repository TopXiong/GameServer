using System;
using System.Collections.Generic;
using System.Text;

namespace TF.Tools
{
    [Serializable]
    public enum EntityType
    {
        /// <summary>
        /// 幽灵猫
        /// </summary>
        GhostCat,

        /// <summary>
        /// 人
        /// </summary>
        Human,

        /// <summary>
        /// 
        /// </summary>
        Others,
    }

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

        public int netObjectId;

        public GameNetObject(int id)
        {
            m_netObjectType = NetObjectType.GameNetObject;
            netObjectId = id;
        }
    }

    [Serializable]
    public class HauntedHouseNetObject : GameNetObject
    {
        public EntityType EType { get; set; }

        public HauntedHouseNetObject(int id) : base(id)
        {
            gameType = GameType.HauntedHouse;
            netObjectId = id;
        }

        public override string ToString()
        {
            return $"[ID] = {netObjectId}, [EntityType] = {EType}";
        }
    }

}
