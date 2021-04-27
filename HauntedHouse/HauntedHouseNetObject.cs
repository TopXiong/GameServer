﻿using System;
using TF.Tools;

namespace HauntedHouse
{
    [Serializable]
    public class GameStartMessage : GameMessage
    {
        public GameStartMessage() : base(GameMessageType.GameStart) { }
    }

    [Serializable]
    public class GameMessage : HauntedHouseNetObject
    {
        public GameMessage(GameMessageType type) : base((int)type)
        {
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
    }


}
