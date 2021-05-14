using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
namespace Common.NetObject
{
    public class TransfromChange : ComponentSynNetObject
    {
        public Vector3 Position;

        public Vector4 Rotation;

        public TransfromChange(Guid netComponentID,Vector3 position,Vector4 rotation,string path):base(netComponentID,path)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
