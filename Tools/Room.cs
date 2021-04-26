using System;
using System.Collections.Generic;
using System.Text;

namespace TF.Tools
{
    [Serializable]
    public class NetObjectState
    {
    }

    [Serializable]
    public abstract class BaseRoom
    {
        //最大容纳人数
        protected int m_maxPlayerNum;
        //房间Id号
        protected int m_id;
        //玩家
        protected Guid[] m_playersID;
        //房间密码
        protected string m_password;
        //房主
        protected int m_roomOwner;

        /// <summary>
        /// 玩家字典
        /// </summary>
        protected Dictionary<int, NetObjectState> m_playerDic = new Dictionary<int, NetObjectState>();

        public Dictionary<int, NetObjectState> PlayerDic
        {
            get => m_playerDic;
        }

        protected BaseRoom(int maxPlayerNum, string password)
        {
            m_playersID = new Guid[maxPlayerNum];
            this.m_maxPlayerNum = maxPlayerNum;
            this.Password = password;
            m_roomOwner = 0;
        }
        /// <summary>
        /// 房主位置
        /// </summary>
        public int RoomOwnerIndex { get => m_roomOwner; private set => m_roomOwner = value; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get => m_password; private set => m_password = value; }
        public Guid[] Players { get => m_playersID; private set => m_playersID = value; }
        public int MaxPlayerNum 
        { 
            get => m_maxPlayerNum; 
        }
        /// <summary>
        /// 房间ID
        /// </summary>
        public int Id { get => m_id; set => m_id = value; }
        protected BaseRoom() { }

        /// <summary>
        /// 游戏数据处理
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="gameNetObject"></param>
        public virtual void DataHandle(Guid userToken, GameNetObject gameNetObject)
        {
            var list = new List<Guid>();
            list.Add(userToken);
            Console.WriteLine(gameNetObject);
            SendDataToRoomPlayer(gameNetObject, list);
        }

        public static Action<Guid,GameNetObject> Send;

        /// <summary>
        /// 对房间所有的玩家发送消息
        /// </summary>
        /// <param name="room"></param>
        /// <param name="baseNetObject"></param>
        public void SendDataToRoomPlayer(GameNetObject gameNetObject, List<Guid> notSends)
        {
            foreach (var playerId in Players)
            {
                if (!notSends.Contains(playerId) && playerId != Guid.Empty)
                {
                    //发送玩家加入消息
                    Send(playerId, gameNetObject);
                }
            }
        }

        /// <summary>
        /// 给房主发消息
        /// </summary>
        /// <param name="gameNetObject"></param>
        /// <param name="notSends"></param>
        public void SendDataToRoomOwner(GameNetObject gameNetObject)
        {
            Send(Players[RoomOwnerIndex], gameNetObject);
        }

        /// <summary>
        /// 对房间所有的玩家发送消息
        /// </summary>
        /// <param name="room"></param>
        /// <param name="baseNetObject"></param>
        public void SendDataToRoomAllPlayer(GameNetObject gameNetObject)
        {
            SendDataToRoomPlayer(gameNetObject, new List<Guid>());
        }

        public virtual void GameStart()
        {

        }

        /// <summary>
        /// 玩家离开
        /// </summary>
        /// <param name="player"></param>
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

        /// <summary>
        /// 返回玩家在房间中的ID
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual int ContainsPlayer(Guid player)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == player)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 返回玩家在房间中的ID
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual int PlayerJoin(Guid player)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == Guid.Empty)
                {
                    Players[i] = player;
                    return i;
                }
            }
            return -1;
        }

        public override string ToString()
        {
            return  "Id --- Pass : " + Id +" --- " + Password + '\n' + "Type :" + GetType();
        }
    }
}
