using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Common.NetObject;

namespace GameServer.Core.Component
{
    public class Transform: BaseComponentObject
    {
        public Vector3 Position;

        public Vector4 Rotation;

        public Vector3 Scale;

        public override void DataHandle(ComponentSynNetObject csno)
        {
            TransfromChange transfromChange = csno as TransfromChange;
        }

        public override void Update()
        {
            Console.WriteLine(ToString());
        }
        public override string ToString()
        {
            return "Position : " + Position + "\n Rotation : " + Rotation + "\n Scale : " + Scale;
        }

    }
}
