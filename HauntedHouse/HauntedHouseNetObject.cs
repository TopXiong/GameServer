using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Unity;
using UnityEngine;

namespace HauntedHouse
{
    [Serializable]
    public class HauntedHouseNetObject:GameNetObject
    {
        public HauntedHouseNetObject()
        {
            gameType = GameType.HauntedHouse;
        }
    }

    [Serializable]
    public class TransformChange : HauntedHouseNetObject
    {
        //public Transform transform;

        //public TransformChange(Transform transform)
        //{
        //    this.transform = transform;
        //}

    }

}
