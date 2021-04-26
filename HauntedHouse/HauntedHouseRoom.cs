using System;
using TF.Tools;

namespace HauntedHouse
{
    [Serializable]
    public class HauntedHousePlayerState : PlayerState
    {
        //TODO 添加更多玩家状态
    }

    [Serializable]
    public class HauntedHouseRoom : BaseRoom
    {
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
                        new HauntedHousePlayerState()
                        {
                            Position = new Vector3(positionNetObj.x, positionNetObj.y, positionNetObj.z)
                        });
                }
            }
            base.DataHandle(userToken, gameNetObject);
        }


    }
}
