using Common;
using Common.Tools;
using Common.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GameServer.Core.Log;
using Common.NetObject;

namespace GameServer.Core.User
{
    public class UserToken
    {
        /// <summary>  
        /// 客户端IP地址  
        /// </summary>  
        public IPAddress IPAddress { get; set; }

        /// <summary>  
        /// 远程地址  
        /// </summary>  
        public EndPoint Remote { get; set; }

        /// <summary>  
        /// 通信SOKET  
        /// </summary>  
        public Socket Socket { get; set; }

        /// <summary>  
        /// 连接时间  
        /// </summary>  
        public DateTime ConnectTime { get; set; }

        /// <summary>  
        /// 数据缓存区  
        /// </summary>  
        public List<byte> Buffer { get; set; }
        public UserData PlayerData { get; set; }

        /// <summary>  
        /// 对数据进行打包,然后再发送  
        /// </summary>  
        /// <param name="token"></param>  
        /// <param name="message"></param>  
        /// <returns></returns>  
        public bool Send(BaseNetObject baseNetObject)
        {
            byte[] message = NetBaseTool.ObjectToBytes(baseNetObject);
            bool isSuccess = false;
            if (Socket == null || !Socket.Connected)
                return isSuccess;
            try
            {
                //对要发送的消息,制定简单协议,头4字节指定包的大小,方便客户端接收(协议可以自己定)  
                byte[] buff = new byte[message.Length + 4];
                byte[] len = BitConverter.GetBytes(message.Length);
                Array.Copy(len, buff, 4);
                Array.Copy(message, 0, buff, 4, message.Length);
                //token.Socket.Send(buff);  //
                //新建异步发送对象, 发送消息  
                SocketAsyncEventArgs sendArg = new SocketAsyncEventArgs();
                sendArg.UserToken = this;
                sendArg.SetBuffer(buff, 0, buff.Length);  //将数据放置进去.  
                isSuccess = Socket.SendAsync(sendArg);
            }
            catch (Exception e)
            {
                Logger.WriteLog("SendMessage - Error:" + e.Message);
            }
            return isSuccess;
        }

        public UserToken()
        {
            PlayerData = new UserData();
            PlayerData.Guid = Guid.NewGuid();
            this.Buffer = new List<byte>();
        }
    }
}
