using System;
using System.Collections.Generic;
using System.Text;
using TF.Tools;

namespace TF.Tools.HauntedHouse
{
    [Serializable]
    [ProtoBuf.ProtoContract]
    public class HauntedHouseRoom : BaseRoom
    {
        protected HauntedHouseRoom()
        {
            
        }

        public HauntedHouseRoom(int id ,string password="") :base(2,password)
        {
            this.m_id = id;
        }

        public override void DataHandle(Guid userToken,GameNetObject gameNetObject)
        {
            var list = new List<Guid>();
            list.Add(userToken);
            Console.WriteLine(gameNetObject);
            SendDataToRoomPlayer(gameNetObject, list);
        }
    }
}
