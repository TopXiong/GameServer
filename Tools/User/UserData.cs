using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.User
{
    [Serializable]
    public abstract class UserData
    {
        public Guid Guid { get; set; }

    }
}
