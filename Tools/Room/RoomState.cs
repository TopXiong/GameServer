using Common.Tools;
using Common.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Room
{
   /// <summary>
   /// 表示房间的当前状态
   /// </summary>
    [Serializable]
    public class RoomState
    {
        /// <summary>
        /// 房主的房间位置
        /// </summary>
        public int RoomOwner;
        /// <summary>
        /// 房间内玩家数量
        /// </summary>
        public int CurrentPlayCount;

        //玩家数据
        public UserData[] UserDatas;
        /// <summary>
        /// 房间的配置
        /// </summary>
        public RoomDesc RoomDesc;

        public RoomState(RoomDesc roomDesc)
        {
            RoomDesc = roomDesc;
            UserDatas = new UserData[roomDesc.MaxPlayerNum];
        }

        public override string ToString()
        {
            return "[ RoomState : \n    RoomDesc: " + RoomDesc + " UserData : " + StringTools.ArrayToString(UserDatas) + " ]";
        }

    }
}
