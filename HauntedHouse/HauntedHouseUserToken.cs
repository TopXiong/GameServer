using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF.Tools;

namespace HauntedHouse
{
    public class HauntedHouseUserToken : UserToken
    {

        public UserToken m_userToken;

        public EntityType EntityType{ get; set; }

        public HauntedHouseUserToken(UserToken userToken, EntityType entityType)
        {
            m_userToken = userToken;
            EntityType = entityType;
        }

    }
}
