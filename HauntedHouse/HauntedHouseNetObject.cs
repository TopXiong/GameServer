using System;
using TF.Tools;

namespace HauntedHouse
{
    [Serializable]
    public class InstantiateNetObjectMessage : GameMessage
    {
        public string PrefabName;

        public Vector3 Position;

        public Quaternion Rotation;

        public InstantiateNetObjectMessage(string prefabName, Vector3 position, Quaternion rotation) : base(GameMessageType.Instantiate)
        {
            PrefabName = prefabName;
            Position = position;
            Rotation = rotation;
        }

        public override string ToString()
        {
            return base.ToString() + $"[Instantiate] : {PrefabName}";
        }
    }

    [Serializable]
    public class DestroyNetObjectMessage : GameMessage
    {
        public object ObjectRef;

        public DestroyNetObjectMessage(object objectRef) : base(GameMessageType.Destroy)
        {
            ObjectRef = objectRef;
        }

        public override string ToString()
        {
            return base.ToString() + $"[Destroy] : {ObjectRef}";
        }
    }

    [Serializable]
    public class GameStartMessage : GameMessage
    {
        public GameStartMessage() : base(GameMessageType.GameStart) { }
    }

    [Serializable]
    public class PlayerSelectRole : GameMessage
    {
        /// <summary>
        /// 自己在房间中的ID
        /// </summary>
        public int PlayerID;

        public HauntedHouseUserData Player;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerid">自己在房间中的ID</param>
        /// <param name="entity">选的角色类型</param>
        public PlayerSelectRole(int playerid, HauntedHouseUserData player) : base(GameMessageType.PlayerSelectRole)
        {
            Player = player;
            PlayerID = playerid;
        }

        public override string ToString()
        {
            return base.ToString() + $", [Id]: {PlayerID}, [PlayerData]: {Player}";
        }
    }

    [Serializable]
    public class GameMessage : HauntedHouseNetObject
    {
        protected GameMessageType m_messageType;

        public GameMessage(GameMessageType type) : base((int)type)
        {
            m_messageType = type;
        }

        public override string ToString()
        {
            return m_messageType.ToString();
        }
    }

    [Serializable]
    public class PositionChange : HauntedHouseNetObject
    {
        public float x, y, z;

        public PositionChange(int id, Vector3 position) : base(id)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }

        //public static implicit operator PositionChange(Vector3 pos)
        //{
        //    return new PositionChange(netObjectId, pos);
        //}

        //public static implicit operator Vector3(PositionChange pos)
        //{
        //    return new Vector3(pos.x,pos.y,pos.z);
        //}

        public override string ToString()
        {
            return base.ToString() + $", [Position] = ({x}, {y}, {z})";
        }
    }

    [Serializable]
    public class RotationChange : HauntedHouseNetObject
    {
        public float x, y, z;

        public RotationChange(int id, Vector3 position) : base(id)
        {

            x = position.x;
            y = position.y;
            z = position.z;
        }

        //public static implicit operator RotationChange(Vector3 pos)
        //{
        //    return new RotationChange(pos);
        //}

        //public static implicit operator Vector3(RotationChange pos)
        //{
        //    return new Vector3(pos.x, pos.y, pos.z);
        //}

        public override string ToString()
        {
            return base.ToString() + $", [Rotation] = ({x}, {y}, {z})";
        }
    }

    [Serializable]
    public class ScaleChange : HauntedHouseNetObject
    {
        public float x, y, z;

        public ScaleChange(int id, Vector3 position) : base(id)
        {

            x = position.x;
            y = position.y;
            z = position.z;
        }

        //public static implicit operator ScaleChange(Vector3 pos)
        //{
        //    return new ScaleChange(, pos);
        //}

        //public static implicit operator Vector3(ScaleChange pos)
        //{
        //    return new Vector3(pos.x, pos.y, pos.z);
        //}
        public override string ToString()
        {
            return base.ToString() + $", [Scale] = ({x}, {y}, {z})";
        }
    }

    [Serializable]
    public class AnimationChange : HauntedHouseNetObject
    {
        public float animationSpeed;

        public AnimationChange(int id, float speed) : base(id)
        {
            animationSpeed = speed;
        }

        public override string ToString()
        {
            return base.ToString() + $", [AnimationSpeed] = ({animationSpeed})";
        }
    }
}
