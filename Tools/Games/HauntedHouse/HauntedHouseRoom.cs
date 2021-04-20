using System;
using System.Collections.Generic;
using System.Text;
using Tools;

namespace Tools.HauntedHouse
{
    [Serializable]
    public class HauntedHouseRoom : BaseRoom
    {
        protected HauntedHouseRoom()
        {
            
        }

        public HauntedHouseRoom(int id ,string password="") :base(2,password)
        {
            this.id = id;
        }

    }
}
