using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF.Tools
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


    }
}
