using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public enum GameType
    {
        DirtyPig
    }


    [Serializable]
    public class BaseNetObject
    {
        
    }
    [Serializable]
    public class Msg : BaseNetObject
    {

        public string context;

        public Msg(string context)
        {
            this.context = context;
        }

        public override string ToString()
        {
            return context;
        }
    }
    [Serializable]
    public class HeartBeat : BaseNetObject
    {
    }
    [Serializable]
    public class CreateRoom : BaseNetObject
    {
        public BaseRoom room;

        public CreateRoom(BaseRoom room)
        {
            this.room = room;
        }
    }
    [Serializable]
    public class JoinRoomC2S : BaseNetObject
    {
        public int roomId;
        public string password;

        public JoinRoomC2S(int roomId, string password)
        {
            this.roomId = roomId;
            this.password = password;
        }
    }
    [Serializable]
    public class JoinRoomS2C : BaseNetObject
    {
        public bool success;

        public JoinRoomS2C(bool success)
        {
            this.success = success;
        }
    }
    [Serializable]
    public class LeaveRoomC2S : BaseNetObject
    {
        
    }
    [Serializable]
    public class GetRoomListC2S : BaseNetObject
    {
        
    }
    [Serializable]
    public class GetRoomListS2C : BaseNetObject
    {
        public List<BaseRoom> rooms;

        public GetRoomListS2C(List<BaseRoom> rooms)
        {
            this.rooms = rooms;
        }
    }
    [Serializable]
    public class PlayerJoinS2C : BaseNetObject
    {
        public int playerId;

        public PlayerJoinS2C(int playerId)
        {
            this.playerId = playerId;
        }
    }
    [Serializable]
    public class PlayerLeaveS2C : BaseNetObject
    {
        public int playerId;

        public PlayerLeaveS2C(int playerId)
        {
            this.playerId = playerId;
        }
    }
}
