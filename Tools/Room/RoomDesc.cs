using Common.NetObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Room
{
    /// <summary>
    /// 表示房间的配置信息
    /// </summary>
    [Serializable]
    public class RoomDesc
    {
        //最大容纳人数
        public int MaxPlayerNum;

        //房间Id号
        public int ID;
        //游戏类型
        public GameType GameType;

        public RoomDesc(int ID)
        {
            this.ID = ID;
        }

        public override string ToString()
        {
            return " RoomID : " + ID;
        }

    }
}
