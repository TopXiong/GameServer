using System;
using System.Collections.Generic;
using System.Text;
using NetWorkTools;

namespace Tools
{
    [Serializable]
    public class BaseRoom
    {
        //最大容纳人数
        protected int maxPlayerNum;
        //房间Id号
        protected int id;
        ////玩家
        protected int[] playersID;
        //房间密码
        protected string password;
        //房主
        protected int roomOwner;
        public BaseRoom(int maxPlayerNum, string password)
        {
            playersID = new int[maxPlayerNum];
            this.maxPlayerNum = maxPlayerNum;
            this.Password = password;
            roomOwner = 0;
        }
        /// <summary>
        /// 房主位置
        /// </summary>
        public int RoomOwnerIndex { get => roomOwner; private set => roomOwner = value; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get => password; private set => password = value; }
        public int[] Players { get => playersID; private set => playersID = value; }
        public int MaxPlayerNum 
        { 
            get => maxPlayerNum; 
            set 
            { 
                //更改数组长度
                maxPlayerNum = value; 
                int[] temp = new int[maxPlayerNum]; 
                Array.Copy(playersID, temp, playersID.Length);
                playersID = temp; 
            } 
        }
        public int Id { get => id; set => id = value; }
        public BaseRoom() { }

        public virtual void PlayerLeave(int player)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == player)
                {
                    Players[i] = 0;
                    return;
                }
            }
        }

        public virtual bool PlayerJoin(int player)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == 0)
                {
                    Players[i] = player;
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return  "Id --- Pass : " + Id +" --- " + Password + '\n' + "Type :" + GetType();
        }
    }
}
