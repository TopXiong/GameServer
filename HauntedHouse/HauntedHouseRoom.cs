using System;
using TF.Tools;

namespace HauntedHouse
{
    
    [Serializable]
    public class HauntedHouseNetObjectState : NetObjectState
    {
        //TODO 添加更多玩家状态
        public Vector3 Position { get; set; }

        public EntityType EType { get; set; }
    }

    [Serializable]
    public enum GameMessageType
    {
        /// <summary>
        /// 游戏开始
        /// </summary>
        GameStart = 666,
        /// <summary>
        /// 选择角色
        /// </summary>
        SelectRole,
        /// <summary>
        /// 其他玩家选择角色
        /// </summary>
        PlayerSelectRole,
    }

    [Serializable]
    public class HauntedHouseRoom : BaseRoom
    {


        public override void GameStart()
        {
            base.GameStart();
            SendDataToRoomAllPlayer(new GameStartMessage());
        }

        protected HauntedHouseRoom()
        {
            
        }

        public HauntedHouseRoom(int id ,string password="") :base(2,password)
        {
            this.m_id = id;
        }

        public override void DataHandle(Guid userToken, GameNetObject gameNetObject)
        {
            if (gameNetObject is PositionChange)
            {
                var positionNetObj = gameNetObject as PositionChange;
                int playerId = ContainsPlayer(userToken);
                if (!m_playerDic.ContainsKey(playerId))
                {
                    m_playerDic.Add(playerId, 
                        new HauntedHouseNetObjectState()
                        {
                            Position = new Vector3(positionNetObj.x, positionNetObj.y, positionNetObj.z),
                            EType = positionNetObj.EType
                        });
                }
                else
                {
                    m_playerDic[playerId] = new HauntedHouseNetObjectState()
                    {
                        Position = new Vector3(positionNetObj.x, positionNetObj.y, positionNetObj.z),
                        EType = positionNetObj.EType
                    };
                }
            }
            //保存玩家信息
            if (gameNetObject is PlayerSelectRole)
            {
                PlayerSelectRole psr = gameNetObject as PlayerSelectRole;
                UserData[psr.PlayerID] = psr.Player;
            }
            base.DataHandle(userToken, gameNetObject);
        }
    }
}
