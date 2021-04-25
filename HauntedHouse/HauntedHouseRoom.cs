using System;
using System.Collections.Generic;
using System.Text;
using TF.Tools;

namespace TF.Tools.HauntedHouse
{
    [Serializable]
    public class HauntedHouseRoom : BaseRoom
    {
        protected HauntedHouseRoom()
        {
            
        }

        public HauntedHouseRoom(int id ,string password="") :base(2,password)
        {
            this.m_id = id;
        }
    }
}
