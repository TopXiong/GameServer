﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF.Tools;
using Unity;
using UnityEngine;

namespace HauntedHouse
{

    [Serializable]
    public class PositionChange : HauntedHouseNetObject
    {
        public float x, y, z;

        public PositionChange(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }

        public static implicit operator PositionChange(Vector3 pos)
        {
            return new PositionChange(pos);
        }

        public static implicit operator Vector3(PositionChange pos)
        {
            return new Vector3(pos.x,pos.y,pos.z);
        }
    }

    [Serializable]
    public class RotationChange : HauntedHouseNetObject
    {
        public float x, y, z;

        public RotationChange(Vector3 position)
        {

            x = position.x;
            y = position.y;
            z = position.z;
        }

        public static implicit operator RotationChange(Vector3 pos)
        {
            return new RotationChange(pos);
        }

        public static implicit operator Vector3(RotationChange pos)
        {
            return new Vector3(pos.x, pos.y, pos.z);
        }
    }

    [Serializable]
    public class ScaleChange : HauntedHouseNetObject
    {
        public float x, y, z;

        public ScaleChange(Vector3 position)
        {

            x = position.x;
            y = position.y;
            z = position.z;
        }

        public static implicit operator ScaleChange(Vector3 pos)
        {
            return new ScaleChange(pos);
        }

        public static implicit operator Vector3(ScaleChange pos)
        {
            return new Vector3(pos.x, pos.y, pos.z);
        }
    }


}
