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

        public override int PlayerJoin(Guid player)
        {
            int value = base.PlayerJoin(player);
            return value;
        }

        public override void PlayerLeave(Guid player)
        {
            base.PlayerLeave(player);
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
            base.DataHandle(userToken, gameNetObject);
        }


    }
}
