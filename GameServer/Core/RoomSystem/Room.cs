using System;
using System.Collections.Generic;
using System.Text;
using Tools.User;
using System.Linq;
using TF.Tools;

namespace GameServer.Core.RoomSystem
{
    [Serializable]
    public class NetObjectState
    {
    }

    [Serializable]
    public class BaseRoom
    {
        //房间密码
        protected string m_password;
        protected Guid[] m_playersGuid;

        public RoomDesc RoomDesc => RoomState.RoomDesc;

        /// <summary>
        /// 房间的状态
        /// </summary>
        public RoomState RoomState;

        public BaseRoom(RoomDesc roomDesc, string password)
        {
            RoomState = new RoomState(roomDesc);
            RoomState.RoomOwner = 0;
            m_playersGuid = new Guid[roomDesc.MaxPlayerNum];
            this.Password = password;
        }
        /// <summary>
        /// 房主位置
        /// </summary>
        public int RoomOwnerIndex { get => RoomState.RoomOwner; private set => RoomState.RoomOwner = value; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get => m_password; private set => m_password = value; }
        public Guid[] Players => m_playersGuid;

        public int CurrentPlayCount { get => RoomState.CurrentPlayCount; protected set => RoomState.CurrentPlayCount=value; }

        public UserData[] UserData => RoomState.UserDatas;

        public int MaxPlayerNum 
        { 
            get => RoomDesc.MaxPlayerNum; 
        }
        /// <summary>
        /// 房间ID
        /// </summary>
        public int Id { get => RoomDesc.ID;}
        protected BaseRoom() { }

        /// <summary>
        /// 游戏数据处理
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="gameNetObject"></param>
        public virtual void DataHandle(Guid userToken, GameNetObject gameNetObject)
        {
            Console.WriteLine(gameNetObject);
            
            SendDataToOtherRoomPlayer(gameNetObject, userToken);
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
        /// 对房间所有的玩家发送消息
        /// </summary>
        /// <param name="room"></param>
        /// <param name="baseNetObject"></param>
        public void SendDataToOtherRoomPlayer(GameNetObject gameNetObject, Guid notSends)
        {
            foreach (var playerId in Players)
            {
                if (notSends != playerId && playerId != Guid.Empty)
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
                    UserData[i] = null;
                    CurrentPlayCount--;
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
        public virtual int PlayerJoin(UserData userdata)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] == Guid.Empty)
                {
                    CurrentPlayCount++;
                    Players[i] = userdata.Guid;
                    UserData[i] = userdata;
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
