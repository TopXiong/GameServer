using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.User
{
    [Serializable]
    public class UserData
    {
        public Guid Guid { get; set; }

        public UserData()
        {
            Guid = Guid.Empty;
        }

        public override string ToString()
        {
            return "PlayerID : " + Guid;
        }

    }
}
