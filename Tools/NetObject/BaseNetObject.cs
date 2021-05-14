using System;

namespace Common.NetObject
{

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

    /// <summary>
    /// 游戏消息
    /// </summary>
    [Serializable]
    public abstract class GameNetObject : BaseNetObject
    {
        /// <summary>
        /// 游戏类型
        /// </summary>
        //public GameType gameType;

        public GameNetObject()
        {

        }
    }

    /// <summary>
    /// 游戏组件同步消息
    /// 将转发给对应组件处理
    /// </summary>
    [Serializable]
    public abstract class ComponentSynNetObject : GameNetObject
    {
        /// <summary>
        /// 游戏类型
        /// </summary>
        public Guid ComponentId;

        /// <summary>
        /// 组件所在位置的全名
        /// </summary>
        public string ComponentName;

        public ComponentSynNetObject(Guid id,string componentName)
        {
            ComponentId = id;
            ComponentName = componentName;
        }
    }
}
