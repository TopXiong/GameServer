using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Room;
using Common.NetObject;

namespace Sample
{
    /// <summary>
    /// 可以设置房间的相关属性，最大人数
    /// 也可以添加自定义的属性
    /// </summary>
    [Serializable]
    public class SampleRoomDesc : RoomDesc
    {
        public SampleRoomDesc(int ID) : base(ID)
        {
            GameType = GameType.Sample;
            MaxPlayerNum = 2;
        }

    }
}
