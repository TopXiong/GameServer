//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Net;
//using System.Net.Sockets;

//namespace GameClient
//{
//    #region Client 连接到server，收发消息逻辑
//    class ClientSocket : IDisposable
//    {
//        private const Int32 BuffSize = 1024;

//        // The socket used to send/receive messages.  
//        private Socket clientSocket;

//        // Flag for connected socket.  
//        private Boolean connected = false;

//        // Listener endpoint.  
//        private IPEndPoint hostEndPoint;

//        // Signals a connection.  
//        private static AutoResetEvent autoConnectEvent = new AutoResetEvent(false);

//        //定义接收数据的对象  
//        List<byte> m_buffer; 

//        /// <summary>  
//        /// 当前连接状态  
//        /// </summary>  
//        public bool Connected { get { return clientSocket != null && clientSocket.Connected; } }

//        //服务器主动发出数据受理委托及事件  
//        public delegate void OnServerDataReceived(byte[] receiveBuff);
//        public event OnServerDataReceived ServerDataHandler;

//        //服务器主动关闭连接委托及事件  
//        public delegate void OnServerStop();
//        public event OnServerStop ServerStopEvent;


//        // Create an uninitialized client instance.  
//        // To start the send/receive processing call the  
//        // Connect method followed by SendReceive method.  
//        internal ClientSocket(String ip, Int32 port)
//        {
//            // Instantiates the endpoint and socket.  
//            hostEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
//            clientSocket = new Socket(hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
//            m_buffer = new List<byte>();
//        }

//        /// <summary>  
//        /// 连接到主机  
//        /// </summary>  
//        /// <returns>0.连接成功, 其他值失败,参考SocketError的值列表</returns>  
//        internal SocketError Connect()
//        {
//            SocketError socketError = SocketError.ConnectionRefused;


//            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
//            connectArgs.UserToken = clientSocket;
//            connectArgs.RemoteEndPoint = hostEndPoint;
//            connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);

//            clientSocket.ConnectAsync(connectArgs);
//            autoConnectEvent.WaitOne(); //阻塞. 让程序在这里等待,直到连接响应后再返回连接结果  

//            SocketError re = connectArgs.SocketError;

//            return re;
//        }

//        /// Disconnect from the host.  
//        internal void Disconnect()
//        {
//            clientSocket.Disconnect(true);//true--这个socket会重用，false--这个socket不再重用。
//        }

//        // Calback for connect operation  
//        private void OnConnect(object sender, SocketAsyncEventArgs e)
//        {
//            // Signals the end of connection.  
//            autoConnectEvent.Set(); //释放阻塞.  
//            // Set the flag for socket connected.  
//            SocketError re = e.SocketError;
//            connected = (e.SocketError == SocketError.Success);
//            //如果连接成功,则初始化socketAsyncEventArgs  
//            if (connected)
//            {
//                SocketAsyncEventArgs receiveEventArgs = new SocketAsyncEventArgs();
//                //接收参数  
//                receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
//                receiveEventArgs.UserToken = e.UserToken;

//                //启动接收,不管有没有,一定得启动.否则有数据来了也不知道.  
//                if (!e.ConnectSocket.ReceiveAsync(receiveEventArgs))
//                    ProcessReceive(receiveEventArgs);
//            }
//        }


//        #region args

//        void IO_Completed(object sender, SocketAsyncEventArgs e)
//        {
//            SocketAsyncEventArgs mys = e;
//            // determine which type of operation just completed and call the associated handler  
//            switch (e.LastOperation)
//            {
//                case SocketAsyncOperation.Receive:
//                    ProcessReceive(e);
//                    break;
//                case SocketAsyncOperation.Send:
//                    ProcessSend(e);
//                    break;
//                default:
//                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
//            }
//        }

//        // This method is invoked when an asynchronous receive operation completes.   
//        // If the remote host closed the connection, then the socket is closed.    
//        // If data was received then the data is echoed back to the client.  
//        //  
//        private void ProcessReceive(SocketAsyncEventArgs e)
//        {
//            try
//            {
//                // check if the remote host closed the connection  
//                Socket token = (Socket)e.UserToken;
//                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
//                {
//                    //读取数据  
//                    byte[] data = new byte[e.BytesTransferred];
//                    Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
//                    lock (m_buffer)
//                    {
//                        m_buffer.AddRange(data);
//                    }

//                    do
//                    {
//                        //一个完整的包是包长(4字节)+包数据,  
//                        //判断包的长度,前面4个字节.  
//                        byte[] lenBytes = m_buffer.GetRange(0, 4).ToArray();
//                        int packageLen = BitConverter.ToInt32(lenBytes, 0);
//                        if (packageLen <= m_buffer.Count - 4)
//                        {
//                            //包够长时,则提取出来,交给后面的程序去处理  
//                            byte[] rev = m_buffer.GetRange(4, packageLen).ToArray();
//                            //从数据池中移除这组数据,为什么要lock,你懂的  
//                            lock (m_buffer)
//                            {
//                                m_buffer.RemoveRange(0, packageLen + 4);
//                            }
//                            //将数据包交给前台去处理  
//                            DoReceiveEvent(rev);
//                        }
//                        else
//                        {   //长度不够,还得继续接收,需要跳出循环  
//                            break;
//                        }
//                    } while (m_buffer.Count > 4);

//                    if (!token.ReceiveAsync(e))
//                        this.ProcessReceive(e);
//                }
//                else
//                {
//                    //Console.WriteLine("服务器断开");
//                    //ProcessError(e); ///服务器断开
//                }
//            }
//            catch (Exception xe)
//            {
//                Console.WriteLine(xe.Message);
//            }
//        }

//        // This method is invoked when an asynchronous send operation completes.    
//        // The method issues another receive on the socket to read any additional   
//        // data sent from the client  
//        //  
//        // <param name="e"></param>  
//        private void ProcessSend(SocketAsyncEventArgs e)
//        {
//            if (e.SocketError != SocketError.Success)
//            {
//                ProcessError(e);
//            }
//        }

//        #endregion

//        #region read write

//        // Close socket in case of failure and throws  
//        // a SockeException according to the SocketError.  
//        private void ProcessError(SocketAsyncEventArgs e)
//        {
//            Console.WriteLine("Error");
//            Socket s = (Socket)e.UserToken;
//            if (s.Connected)
//            {
//                // close the socket associated with the client  
//                try
//                {
//                    s.Shutdown(SocketShutdown.Both);
//                }
//                catch (Exception ex)
//                {
//                    // throws if client process has already closed  
//                    Console.WriteLine(ex.Message);
//                }
//                finally
//                {
//                    if (s.Connected)
//                    {
//                        s.Close();
//                    }
//                    connected = false;
//                }
//            }

//            if (ServerStopEvent != null)
//                ServerStopEvent(); //服务器断开事件
//        }

//        // Exchange a message with the host.  
//        internal void Send(byte[] sendBuffer)
//        {
//            if (connected)
//            {
//                //先对数据进行包装,就是把包的大小作为头加入
//                byte[] buff = new byte[sendBuffer.Length + 4];
//                Array.Copy(BitConverter.GetBytes(sendBuffer.Length), buff, 4);
//                Array.Copy(sendBuffer, 0, buff, 4, sendBuffer.Length);
//                //查找有没有空闲的发送MySocketEventArgs
//                SocketAsyncEventArgs sendArg = new SocketAsyncEventArgs();
//                sendArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
//                sendArg.UserToken = clientSocket;
//                sendArg.RemoteEndPoint = hostEndPoint;
//                lock (sendArg) //要锁定,
//                {
//                    sendArg.SetBuffer(buff, 0, buff.Length);
//                }
//                clientSocket.SendAsync(sendArg);
//            }
//            else
//            {
//                throw new SocketException((Int32)SocketError.NotConnected);
//            }
//        }

//        /// <summary>  
//        /// 使用新进程通知事件回调  
//        /// </summary>  
//        /// <param name="buff"></param>  
//        private void DoReceiveEvent(byte[] buff)
//        {
//            if (ServerDataHandler == null) return;
//            //ServerDataHandler(buff); //可直接调用.  
//            //但我更喜欢用新的线程,这样不拖延接收新数据.  
//            Thread thread = new Thread(new ParameterizedThreadStart((obj) =>
//            {
//                ServerDataHandler((byte[])obj);
//            }));
//            thread.IsBackground = true;
//            thread.Start(buff);
//        }

//        #endregion

//        #region IDisposable Members

//        // Disposes the instance of SocketClient.

//        public void Dispose()
//        {
//            autoConnectEvent.Close();
//            if (clientSocket.Connected)
//            {
//                clientSocket.Close();
//            }
//        }
//        #endregion
//    }
//    #endregion

//    #region Request
//    class Request
//    {
//        //定义,最好定义成静态的, 因为我们只需要一个就好  
//        static ClientSocket smanager = null;


//        //定义事件与委托  
//        public delegate void ReceiveData(byte[] message);
//        public delegate void ServerClosed();
//        public static event ReceiveData OnReceiveData;
//        public static event ServerClosed OnServerClosed;

//        /// <summary>  
//        /// 心跳定时器  
//        /// </summary>  
//        static System.Timers.Timer heartTimer = null;


//        /// <summary>  
//        /// 判断是否已连接  
//        /// </summary>  
//        public static bool Connected
//        {
//            get { return smanager != null && smanager.Connected; }
//        }



//        #region 基本方法

//        /// <summary>  
//        /// 连接到服务器  
//        /// </summary>  
//        /// <returns></returns>  
//        public static SocketError Connect(string ip, int port)
//        {
//            if (Connected) return SocketError.Success;
//            //string ip = "192.168.1.158";
//            //int port = 50000;
//            if (string.IsNullOrWhiteSpace(ip) || port <= 1000) return SocketError.Fault;
//            //创建连接对象, 连接到服务器  
//            smanager = new ClientSocket(ip, port);
//            // SocketError error = smanager.Connect();
//            SocketError socketError = TryConnect();

//            if (socketError == SocketError.Success)
//            {
//                //连接成功后,就注册事件. 最好在成功后再注册.  
//                smanager.ServerDataHandler += OnReceivedServerData;
//                smanager.ServerStopEvent += OnServerStopEvent;

//            }
//            return socketError;
//        }

//        /// <summary>  
//        /// 断开连接  
//        /// </summary>  
//        public static void Disconnect()
//        {
//            try
//            {
//                smanager.Disconnect();
//                if (heartTimer != null)
//                    heartTimer = null;
//            }
//            catch (Exception)
//            {
//                Console.WriteLine("未能关闭socket连接");
//            }
//        }

//        /// <summary>
//        /// 尝试连接server，成功则返回true
//        /// </summary>
//        /// <returns></returns>
//        public static SocketError TryConnect()
//        {
//            SocketError socketError = SocketError.ConnectionRefused;
//            try
//            {
//                do
//                {
//                    socketError = smanager.Connect();
//                    if (socketError == SocketError.Success)
//                    {
//                        //触发事件
//                        break;
//                    }
//                } while (socketError != SocketError.Success);
//            }
//            catch (Exception)
//            {


//            }
//            return socketError;

//        }
//        /// <summary>  
//        /// 发送消息  
//        /// </summary>  
//        /// <param name="message">消息实体</param>  
//        /// <returns>True.已发送; False.未发送</returns>  
//        public static bool Send(string message)
//        {
//            if (!Connected) return false;

//            byte[] buff = Encoding.UTF8.GetBytes(message);
//            //加密,根据自己的需要可以考虑把消息加密  
//            //buff = AESEncrypt.Encrypt(buff, m_aesKey);  
//            smanager.Send(buff);
//            return true;
//        }


//        /// <summary>  
//        /// 发送字节流  
//        /// </summary>  
//        /// <param name="buff"></param>  
//        /// <returns></returns>  
//        static bool Send(byte[] buff)
//        {
//            if (!Connected) return false;
//            smanager.Send(buff);
//            return true;
//        }



//        /// <summary>  
//        /// 接收消息  
//        /// </summary>  
//        /// <param name="buff"></param>  
//        private static void OnReceivedServerData(byte[] buff)
//        {
//            //To do something  
//            //你要处理的代码,可以实现把buff转化成你具体的对象, 再传给前台  
//            if (OnReceiveData != null)
//            {
//                OnReceiveData(buff);
//                Console.WriteLine(Encoding.ASCII.GetString(buff, 0, buff.Length));
//            }
//        }

//        static void Main(string[] args)
//        {
//            ClientSocket clientSocket = new ClientSocket("127.0.0.1", 1234);
//            var hr = clientSocket.Connect();
//            Console.WriteLine(hr);
//            clientSocket.ServerDataHandler += (byte[] receiveBuff) =>
//            {
//                Console.WriteLine(Encoding.ASCII.GetString(receiveBuff));
//            };
//            while (true)
//            {
//                var s = Console.ReadLine();
//                clientSocket.Send(Encoding.ASCII.GetBytes(s));
//            }


//            Console.Read();
//        }

//        /// <summary>  
//        /// 服务器已断开  
//        /// </summary>  
//        private static void OnServerStopEvent()
//        {
//            if (OnServerClosed != null)
//                OnServerClosed();
//        }

//        #endregion

//    }
//    #endregion

//}

