using System;
using TF.Tools;

namespace HauntedHouse
{
    [Serializable]
    public class HauntedHouseUserData : UserToken.UserData
    {

        /// <summary>
        /// 玩家名字
        /// </summary>
        public string Name { get; set; }
    
        /// <summary>
        /// 玩家角色
        /// </summary>
        public EntityType EntityType { get; set; }

        public HauntedHouseUserData(string name,EntityType entityType)
        {
            Name = name;
            EntityType = entityType;
        }

        public override string ToString()
        {
            return "[ Name : " + Name + " EntityType : " + EntityType + " ]";
        }
    }
}
