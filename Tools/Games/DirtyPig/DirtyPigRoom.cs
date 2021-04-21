//using System;
//using System.Collections.Generic;
//using System.Text;
//using Tools;

//namespace NetWorkTools.Games.DirtyPig
//{
//    [Serializable]
//    public class DirtyPigRoom : BaseRoom
//    {
//        private int pigNum;

//        private int cardNum;

//        public int PigNum { get => pigNum;private set => pigNum = value; }
//        public int CardNum { get => cardNum; private set => cardNum = value; }

//        public DirtyPigRoom()
//        {
            
//        }

//        public DirtyPigRoom(int pigNum, int cardNum , int maxPlayerNum, string password):base(maxPlayerNum,password)
//        {
//            this.pigNum = pigNum;
//            this.cardNum = cardNum;
//        }

//        public override void DataHandle(Guid userToken,GameNetObject gameNetObject)
//        {
            
//        }
//    }
//}
