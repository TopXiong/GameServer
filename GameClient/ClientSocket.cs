using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TF.GameClient
{
    internal class ClientSocket
    {
        private Socket clientSocket;
        private IPEndPoint hostEndPoint;
        /// <summary>
        /// 是否连接
        /// </summary>
        private bool connected = false;

        private AutoResetEvent autoConnectEvent = new AutoResetEvent(false);
        internal event Action<byte[]> ServerDataHandler;
        internal ClientSocket(String ip, Int32 port)
        {
            hostEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_buffer = new List<byte>();
        }

        // Calback for connect operation  
        private void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            // Signals the end of connection.  
            autoConnectEvent.Set(); //释放阻塞.  
            // Set the flag for socket connected.  
            SocketError re = e.SocketError;
            connected = (e.SocketError == SocketError.Success);
            //如果连接成功,则初始化socketAsyncEventArgs  
            if (connected)
            {
                SocketAsyncEventArgs receiveEventArgs = new SocketAsyncEventArgs();
                //接收参数  
                receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                receiveEventArgs.UserToken = e.UserToken;
                receiveEventArgs.SetBuffer(new byte[1024], 0,1024);
                //启动接收,不管有没有,一定得启动.否则有数据来了也不知道.  
                if (!e.ConnectSocket.ReceiveAsync(receiveEventArgs))
                    ProcessReceive(receiveEventArgs);
            }
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs mys = e;
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

        internal SocketError Connent()
        {
            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
            connectArgs.UserToken = clientSocket;
            connectArgs.RemoteEndPoint = hostEndPoint;
            connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);

            clientSocket.ConnectAsync(connectArgs);
            autoConnectEvent.WaitOne(); //阻塞. 让程序在这里等待,直到连接响应后再返回连接结果  

            SocketError re = connectArgs.SocketError;

            return re;
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                //ProcessError(e);
            }
        }
        //定义接收数据的对象  
        List<byte> m_buffer = new List<byte>();

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // check if the remote host closed the connection  
                Socket token = (Socket)e.UserToken;
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据  
                    byte[] data = new byte[e.BytesTransferred];
                    Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                    lock (m_buffer)
                    {
                        m_buffer.AddRange(data);
                    }

                    do
                    {
                        //一个完整的包是包长(4字节)+包数据,  
                        //判断包的长度,前面4个字节.  
                        byte[] lenBytes = m_buffer.GetRange(0, 4).ToArray();
                        int packageLen = BitConverter.ToInt32(lenBytes, 0);
                        if (packageLen <= m_buffer.Count - 4)
                        {
                            //包够长时,则提取出来,交给后面的程序去处理  
                            byte[] rev = m_buffer.GetRange(4, packageLen).ToArray();
                            //从数据池中移除这组数据,为什么要lock,你懂的  
                            lock (m_buffer)
                            {
                                m_buffer.RemoveRange(0, packageLen + 4);
                            }
                            //将数据包交给前台去处理  
                            ServerDataHandler?.Invoke(rev);
                        }
                        else
                        {   //长度不够,还得继续接收,需要跳出循环  
                            break;
                        }
                    } while (m_buffer.Count > 4);

                    if (!token.ReceiveAsync(e))
                        this.ProcessReceive(e);
                }
                else
                {
                    //ProcessError(e); ///服务器断开
                }
            }
            catch (Exception xe)
            {
                Console.WriteLine(xe.Message);
            }
        }

        // Exchange a message with the host.  
        internal void Send(byte[] sendBuffer)
        {
            if (connected)
            {
                //先对数据进行包装,就是把包的大小作为头加入
                byte[] buff = new byte[sendBuffer.Length + 4];
                Array.Copy(BitConverter.GetBytes(sendBuffer.Length), buff, 4);
                Array.Copy(sendBuffer, 0, buff, 4, sendBuffer.Length);
                //查找有没有空闲的发送MySocketEventArgs
                SocketAsyncEventArgs sendArg = new SocketAsyncEventArgs();
                sendArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                sendArg.UserToken = clientSocket;
                sendArg.SetBuffer(buff, 0, buff.Length);
                clientSocket.SendAsync(sendArg);
            }
            else
            {
                throw new SocketException((Int32)SocketError.NotConnected);
            }
        }

        

    }
}