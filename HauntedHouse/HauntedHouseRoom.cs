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
    public class HauntedHouseRoom : BaseRoom
    {

        private int m_currentPlayerCount;

        public override void GameStart()
        {
            base.GameStart();
            SendDataToRoomAllPlayer(new GameStart());
        }

        protected HauntedHouseRoom()
        {
            
        }

        public override int PlayerJoin(Guid player)
        {
            int value = base.PlayerJoin(player);
            m_currentPlayerCount++;
            if(m_currentPlayerCount == MaxPlayerNum)
            {
                GameStart();
            }
            return value;
        }

        public override void PlayerLeave(Guid player)
        {
            m_currentPlayerCount--;
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
