using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Tools;

namespace GameServer.Core.NetWork
{
    public class ServerService
    {
        public event Action<UserToken,byte[]> ReceiveClientData;
        private Socket listenSocket;

        /// <summary>
        /// TODO 改为配置文件读取
        /// </summary>
        public ServerService()
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any,1234);
            listenSocket.Bind(iPEndPoint);
            listenSocket.Listen(20);

            ReceiveClientData += (UserToken userToken, byte[] bytes) =>
            {
                Console.WriteLine("Receive " + Encoding.ASCII.GetString(bytes));
                SendMessage(userToken, bytes);
            };

            StartAccept(null);


        }

        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += ProcessAccept;
            }
            else
            {
                // socket must be cleared since the context object is being reused  
                acceptEventArg.AcceptSocket = null;
            }
            Console.WriteLine("StartAccept");
            if (!listenSocket.AcceptAsync(acceptEventArg))
            {
                Console.WriteLine("Accepted");
                ProcessAccept(null,acceptEventArg);
            }
        }

        public void ProcessAccept(object sender,SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
            acceptArgs.Completed += IO_Completed;
            UserToken userToken = new UserToken();
            userToken.usernamr = "Mtt";
            userToken.Socket = e.AcceptSocket;
            userToken.ConnectTime = DateTime.Now;
            userToken.Remote = e.AcceptSocket.RemoteEndPoint;
            userToken.IPAddress = ((IPEndPoint)(e.AcceptSocket.RemoteEndPoint)).Address;
            acceptArgs.UserToken = userToken;
            acceptArgs.SetBuffer(new byte[1024], 0, 1024);
            if (!e.AcceptSocket.ReceiveAsync(acceptArgs))
            {
                ProcessReceive(acceptArgs);
            }
            StartAccept(e);
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // check if the remote host closed the connection  
                UserToken token = (UserToken)e.UserToken;
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据  
                    byte[] data = new byte[e.BytesTransferred];
                    Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                    lock (token.Buffer)
                    {
                        token.Buffer.AddRange(data);
                    }
                    do
                    {
                        //判断包的长度  
                        byte[] lenBytes = token.Buffer.GetRange(0, 4).ToArray();
                        int packageLen = BitConverter.ToInt32(lenBytes, 0);
                        if (packageLen > token.Buffer.Count - 4)
                        {   //长度不够时,退出循环,让程序继续接收  
                            break;
                        }

                        //包够长时,则提取出来,交给后面的程序去处理  
                        byte[] rev = token.Buffer.GetRange(4, packageLen).ToArray();
                        //从数据池中移除这组数据  
                        lock (token.Buffer)
                        {
                            token.Buffer.RemoveRange(0, packageLen + 4);
                        }
                        ReceiveClientData?.Invoke(token, rev);

                    } while (token.Buffer.Count > 4);
                    if (!token.Socket.ReceiveAsync(e))
                        this.ProcessReceive(e);
                }
                else
                {
                    //CloseClientSocket(e);
                }
            }
            catch (Exception xe)
            {
                Console.WriteLine(xe.Message + "\r\n" + xe.StackTrace);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // done echoing data back to the client  
                UserToken token = (UserToken)e.UserToken;
                // read the next block of data send from the client  
                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                //CloseClientSocket(e);
            }
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler  
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        /// <summary>  
        /// 对数据进行打包,然后再发送  
        /// </summary>  
        /// <param name="token"></param>  
        /// <param name="message"></param>  
        /// <returns></returns>  
        public bool SendMessage(UserToken token, byte[] message)
        {
            bool isSuccess = false;
            if (token == null || token.Socket == null || !token.Socket.Connected)
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
                sendArg.UserToken = token;
                sendArg.SetBuffer(buff, 0, buff.Length);  //将数据放置进去.  
                isSuccess = token.Socket.SendAsync(sendArg);
            }
            catch (Exception e)
            {
                Console.WriteLine("SendMessage - Error:" + e.Message);
            }
            return isSuccess;
        }

    }
}