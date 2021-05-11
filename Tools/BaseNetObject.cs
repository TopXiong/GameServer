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
    public enum GameType
    {
        Sample,
        DirtyPig,
        HauntedHouse,
    }
    /// <summary>
    /// 通讯信息基类
    /// </summary>
    [Serializable]
    public abstract class BaseNetObject
    {
        
    }
    /// <summary>
    /// 系统消息基类
    /// </summary>
    [Serializable]
    public abstract class SystemNetObject : BaseNetObject
    {

    }
    [Serializable]
    public abstract class GameNetObject : BaseNetObject
    {
        public GameType gameType;

        public Guid netObjectId;

        public GameNetObject(Guid id)
        {
            netObjectId = id;
        }
    }

    //[Serializable]
    //public class HauntedHouseNetObject : GameNetObject
    //{
    //    public EntityType EType { get; set; }

    //    public HauntedHouseNetObject(int id) : base(id)
    //    {
    //        gameType = GameType.HauntedHouse;
    //        netObjectId = id;
    //    }

    //    public override string ToString()
    //    {
    //        return $"[ID] = {netObjectId}, [EntityType] = {EType}";
    //    }
    //}

}
