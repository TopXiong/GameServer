using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GameServer.Core.User;
using Common.User;
using Common.Room;
using Common;
using GameServer.Core.Game;
using Common.NetObject;

namespace GameServer.Core.RoomSystem
{
    public class BaseRoom
    {

        public Scene Scene;

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
            Users = new UserToken[roomDesc.MaxPlayerNum];
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

        /// <summary>
        /// 会发给客户端的数据
        /// </summary>
        public UserData[] UserData => RoomState.UserDatas;

        /// <summary>
        /// 玩家的网络数据
        /// </summary>
        public UserToken[] Users { get; }

        public int MaxPlayerNum 
        { 
            get => RoomDesc.MaxPlayerNum; 
        }
        /// <summary>
        /// 房间ID
        /// </summary>
        public int Id { get => RoomDesc.ID;}

        /// <summary>
        /// 游戏数据处理
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="gameNetObject"></param>
        public virtual void DataHandle(Guid userToken, GameNetObject gameNetObject)
        {
            Console.WriteLine(gameNetObject);
            int i = ContainsPlayer(userToken);
            Scene.DateHandle(i, gameNetObject);
            //SendDataToOtherRoomPlayer(gameNetObject, userToken);
        }

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
                    Send(playerId,gameNetObject);
                }
            }
        }

        protected bool Send(Guid playerId,GameNetObject gameNetObject)
        {
            int i = ContainsPlayer(playerId);
            //发送玩家加入消息
            return Users[i].Send(gameNetObject);
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

        /// <summary>
        /// 开始游戏
        /// </summary>
        public virtual void GameStart()
        {
            Scene = EventSystem.Instance.GetMonoInterface<Scene>();
            Scene.Init(RoomState.CurrentPlayCount,RoomDesc.GameType);
            foreach (var user in Users)
            {
                if(user != null)
                    user.Send(new GameStart());
            }
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
                    Users[i] = null;
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
        public virtual int PlayerJoin(UserToken user)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                //Find Enpty Sit
                if (Players[i] == Guid.Empty)
                {
                    CurrentPlayCount++;
                    Users[i] = user;
                    Players[i] = user.PlayerData.Guid;
                    UserData[i] = user.PlayerData;
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
