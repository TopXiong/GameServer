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
        protected Guid[] playersID;
        //房间密码
        protected string password;
        //房主
        protected int roomOwner;
        public BaseRoom(int maxPlayerNum, string password)
        {
            playersID = new Guid[maxPlayerNum];
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
        public Guid[] Players { get => playersID; private set => playersID = value; }
        public int MaxPlayerNum 
        { 
            get => maxPlayerNum; 
        }
        public int Id { get => id; set => id = value; }
        protected BaseRoom() { }

        public virtual void PlayerLeave(Guid player)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == player)
                {
                    Players[i] = Guid.Empty;
                    return;
                }
            }
        }

        public virtual bool ContainsPlayer(Guid player)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == player)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool PlayerJoin(Guid player)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == Guid.Empty)
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
