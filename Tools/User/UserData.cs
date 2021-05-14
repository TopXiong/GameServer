using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.User
{
    [Serializable]
    public class UserData
    {
        public Guid Guid;

        public string Name;

        public UserData():this("NoName")
        {
            
        }

        public UserData(string name)
        {
            Guid = Guid.Empty;
            this.Name = name;
        }

        public override string ToString()
        {
            return "PlayerID : " + Guid + " Name: " + Name;
        }

    }
}
