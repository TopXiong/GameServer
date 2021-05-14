using Common.Room;
using Common.User;
using System;
using System.Collections.Generic;


namespace Common.NetObject
{


    [Serializable]
    public class Msg : SystemNetObject
    {
        public string Context;

        public Msg() { }

        public Msg(string context)
        {
            this.Context = context;
        }

        public override string ToString()
        {
            return Context;
        }
    }
    
    [Serializable]
    public class GetMyUserData : SystemNetObject
    {

        public UserData userData;

        public GetMyUserData(UserData userData)
        {
            this.userData = userData;
        }
    }

    [Serializable]
    public class HeartBeat : SystemNetObject
    {
    }

    [Serializable]
    public class SetUserData: SystemNetObject
    {
        public UserData UserData;

        public SetUserData(UserData userData)
        {
            UserData = userData;
        }

    }

    [Serializable]
    public class GameStart : SystemNetObject
    {

    }

    [Serializable]
    public class CreateRoomC2S : SystemNetObject
    {

        public RoomDesc RoomDesc;

        public String Password;

        public CreateRoomC2S() { }

        public CreateRoomC2S(RoomDesc room,string password)
        {
            this.RoomDesc = room;
            this.Password = password;
        }
    }

    [Serializable]
    public class CreateRoomS2C : SystemNetObject
    {
        /// <summary>
        /// 在房间中的序号,-1表示不成功
        /// </summary>
        public int PlayerId;

        public CreateRoomS2C() { }

        public CreateRoomS2C(int playerId)
        {
            this.PlayerId = playerId;
        }
    }

    [Serializable]
    public class JoinRoomC2S : SystemNetObject
    {

        public int RoomId;

        public string Password;

        public JoinRoomC2S(int roomId, string password)
        {
            this.RoomId = roomId;
            this.Password = password;
        }
    }

    [Serializable]
    public class JoinRoomS2C : SystemNetObject
    {
        public int PlayerId;
        public RoomState Room;

        public JoinRoomS2C(int playerId, RoomState room)
        {
            this.Room = room;
            this.PlayerId = playerId;
        }
    }

    [Serializable]
    public class LeaveRoomC2S : SystemNetObject
    {

    }

    [Serializable]
    public class GetRoomListC2S : SystemNetObject
    {

    }

    [Serializable]
    public class GetRoomListS2C : SystemNetObject
    {
        public List<RoomState> rooms;

        public GetRoomListS2C(List<RoomState> rooms)
        {
            this.rooms = rooms;
        }
    }
    [Serializable]
    public class PlayerJoinS2C : SystemNetObject
    {

        public Guid playerId;

        public PlayerJoinS2C(Guid playerId)
        {
            this.playerId = playerId;
        }
    }

    [Serializable]
    public class PlayerLeaveS2C : SystemNetObject
    {

        public Guid playerId;

        public PlayerLeaveS2C(Guid playerId)
        {
            this.playerId = playerId;
        }
    }
}
