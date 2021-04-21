using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TF.Tools
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
    public class HeartBeat : SystemNetObject
    {
    }

    [Serializable]
    public class CreateRoomC2S : SystemNetObject
    {

        public BaseRoom room;

        public GameType gameType;

        public CreateRoomC2S() { }

        public CreateRoomC2S(BaseRoom room)
        {
            this.room = room;
        }
    }

    [Serializable]
    public class CreateRoomS2C : SystemNetObject
    {
        public bool Success;

        public CreateRoomS2C() { }

        public CreateRoomS2C(bool success)
        {
            this.Success = success;
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
        public bool Success;

        public JoinRoomS2C(bool success)
        {
            this.Success = success;
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
        public List<BaseRoom> rooms;

        public GetRoomListS2C(List<BaseRoom> rooms)
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
