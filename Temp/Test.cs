
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;


namespace Lister13
{
    #region server端
    public class Server
    {
        private int m_maxConnectNum;    //最大连接数  
        private int m_revBufferSize;    //最大接收字节数  
        BufferManager m_bufferManager;
        const int opsToAlloc = 2;
        Socket listenSocket;            //监听Socket  
        public SocketEventPool m_pool;
        int m_clientCount;              //连接的客户端数量  
        Semaphore m_maxNumberAcceptedClients;

        List<AsyncUserToken> m_clients; //客户端列表  


        #region
        /// <summary>  
        /// 客户端连接数量变化时触发  
        /// </summary>  
        /// <param name="num">当前增加客户的个数(用户退出时为负数,增加时为正数,为1)</param>  
        /// <param name="token">增加用户的信息</param>  
        public delegate void OnClientNumberChange(int num, AsyncUserToken token);
        /// <summary>  
        /// 接收到客户端的数据  
        /// </summary>  
        /// <param name="token">客户端</param>  
        /// <param name="buff">客户端数据</param>  
        public delegate void OnReceiveData(AsyncUserToken token, byte[] buff);

        public delegate void stopedDel();
        public event stopedDel ServerStopedEvent;



        /// <summary>  
        /// 客户端连接数量变化事件  
        /// </summary>  
        public event OnClientNumberChange ClientNumberChange;

        /// <summary>  
        /// 接收到客户端的数据事件  
        /// </summary>  
        public event OnReceiveData ReceiveClientData;


        #endregion

        #region 定义属性

        /// <summary>  
        /// 获取客户端列表  
        /// </summary>  
        public List<AsyncUserToken> ClientList { get { return m_clients; } }

        #endregion

        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="numConnections">最大连接数</param>  
        /// <param name="receiveBufferSize">缓存区大小</param>  
        public Server(int numConnections, int receiveBufferSize)
        {
            m_clientCount = 0;
            m_maxConnectNum = numConnections;
            m_revBufferSize = receiveBufferSize;
            // allocate buffers such that the maximum number of sockets can have one outstanding read and   
            //write posted to the socket simultaneously    
            m_bufferManager = new BufferManager(receiveBufferSize * numConnections * opsToAlloc, receiveBufferSize);

            m_pool = new SocketEventPool(numConnections);
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
        }

        /// <summary>  
        /// 初始化  
        /// </summary>  
        public void Init()
        {
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds   
            // against memory fragmentation  
            m_bufferManager.InitBuffer();
            m_clients = new List<AsyncUserToken>();
            // preallocate pool of SocketAsyncEventArgs objects  
            SocketAsyncEventArgs readWriteEventArg;

            for (int i = 0; i < m_maxConnectNum; i++)
            {
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.UserToken = new AsyncUserToken();

                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object  
                m_bufferManager.SetBuffer(readWriteEventArg);
                // add SocketAsyncEventArg to the pool  
                m_pool.Push(readWriteEventArg);
            }
        }


        /// <summary>  
        /// 启动服务  
        /// </summary>  
        /// <param name="localEndPoint"></param>  
        public bool Start(IPEndPoint localEndPoint)
        {
            try
            {
                m_clients.Clear();
                listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(localEndPoint);
                // start the server with a listen backlog of 100 connections  
                listenSocket.Listen(m_maxConnectNum);
                // post accepts on the listening socket  
                StartAccept(null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>  
        /// 停止服务  
        /// </summary>  
        public void Stop()
        {
            AsyncUserToken[] clients = m_clients.ToArray();
            for (int i = 0; i < clients.Length; i++)
            {
                try
                {
                    AsyncUserToken token = clients[i];
                    token.Socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception) { }
            }

            try
            {
                if (m_clients.Count > 0)
                    listenSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)//当socket 无连接时（即没有可用已连接的client）
            {
                // listenSocket.Disconnect(false);///因为无可用的socket连接，此方法不可用
                Console.WriteLine(e.Message);
            }
            finally
            {
                listenSocket.Close();
                //通知界面，server已经停止
                if (ServerStopedEvent != null)
                    ServerStopedEvent();
                lock (m_clients)
                {
                    m_clients.Clear();
                }

            }


        }


        public void CloseClient(AsyncUserToken token)
        {
            try
            {
                token.Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
        }


        // Begins an operation to accept a connection request from the client   
        //  
        // <param name="acceptEventArg">The context object to use when issuing   
        // the accept operation on the server's listening socket</param>  
        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // socket must be cleared since the context object is being reused  
                acceptEventArg.AcceptSocket = null;
            }
            m_maxNumberAcceptedClients.WaitOne();
            Console.WriteLine("StartAccept");
            if (!listenSocket.AcceptAsync(acceptEventArg))
            {
                Console.WriteLine("Accepted");
                ProcessAccept(acceptEventArg);
            }
        }

        // This method is the callback method associated with Socket.AcceptAsync   
        // operations and is invoked when an accept operation is complete  
        //  
        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                Console.WriteLine("Accept");
                Interlocked.Increment(ref m_clientCount);
                // Get the socket for the accepted client connection and put it into the   
                //ReadEventArg object user token  
                SocketAsyncEventArgs readEventArgs = m_pool.Pop();
                AsyncUserToken userToken = (AsyncUserToken)readEventArgs.UserToken;

                userToken.usernamr = "Mtt";
                userToken.Socket = e.AcceptSocket;
                userToken.ConnectTime = DateTime.Now;
                userToken.Remote = e.AcceptSocket.RemoteEndPoint;
                userToken.IPAddress = ((IPEndPoint)(e.AcceptSocket.RemoteEndPoint)).Address;
                ClientList.Add(userToken);

                lock (m_clients) { m_clients.Add(userToken); }

                if (ClientNumberChange != null)
                    ClientNumberChange(1, userToken);
                if (!e.AcceptSocket.ReceiveAsync(readEventArgs))
                {
                    ProcessReceive(readEventArgs);
                }
            }
            catch (Exception me)
            {
                Console.WriteLine(me.Message);

            }

            // Accept the next connection request  
            if (e.SocketError == SocketError.OperationAborted) return;
            StartAccept(e);
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


        // This method is invoked when an asynchronous receive operation completes.   
        // If the remote host closed the connection, then the socket is closed.    
        // If data was received then the data is echoed back to the client.  
        //  
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // check if the remote host closed the connection  
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
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

                        if (ReceiveClientData != null)
                            ReceiveClientData(token, rev);

                    } while (token.Buffer.Count > 4);
                    if (!token.Socket.ReceiveAsync(e))
                        this.ProcessReceive(e);
                }
                else
                {
                    CloseClientSocket(e);
                }
            }
            catch (Exception xe)
            {
                Console.WriteLine(xe.Message + "\r\n" + xe.StackTrace);
            }
        }

        // This method is invoked when an asynchronous send operation completes.    
        // The method issues another receive on the socket to read any additional   
        // data sent from the client  
        //  
        // <param name="e"></param>  
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // done echoing data back to the client  
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                // read the next block of data send from the client  
                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }

        //关闭客户端  
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;
            ClientList.Remove(token);

            lock (m_clients) { m_clients.Remove(token); }
            //如果有事件,则调用事件,发送客户端数量变化通知  
            if (ClientNumberChange != null)
                ClientNumberChange(-1, token);
            // close the socket associated with the client  
            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception) { }
            token.Socket.Close();
            // decrement the counter keeping track of the total number of clients connected to the server  
            Interlocked.Decrement(ref m_clientCount);
            m_maxNumberAcceptedClients.Release();
            // Free the SocketAsyncEventArg so they can be reused by another client  
            e.UserToken = new AsyncUserToken();
            m_pool.Push(e);
        }

        /// <summary>  
        /// 对数据进行打包,然后再发送  
        /// </summary>  
        /// <param name="token"></param>  
        /// <param name="message"></param>  
        /// <returns></returns>  
        public bool SendMessage(AsyncUserToken token, byte[] message)
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
    #endregion

    #region BufferManager
    class BufferManager
    {
        int m_numBytes;                 // the total number of bytes controlled by the buffer pool  
        byte[] m_buffer;                // the underlying byte array maintained by the Buffer Manager  
        Stack<int> m_freeIndexPool;     //   
        int m_currentIndex;
        int m_bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        // Allocates buffer space used by the buffer pool  
        public void InitBuffer()
        {
            // create one big large buffer and divide that   
            // out to each SocketAsyncEventArg object  
            m_buffer = new byte[m_numBytes];
        }

        // Assigns a buffer from the buffer pool to the   
        // specified SocketAsyncEventArgs object  
        //  
        // <returns>true if the buffer was successfully set, else false</returns>  
        public bool SetBuffer(SocketAsyncEventArgs args)
        {

            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex += m_bufferSize;
            }
            return true;
        }

        // Removes the buffer from a SocketAsyncEventArg object.    
        // This frees the buffer back to the buffer pool  
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
    #endregion

    #region SocketEventPool
    public class SocketEventPool
    {
        Stack<SocketAsyncEventArgs> m_pool;


        public SocketEventPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }

        // Removes a SocketAsyncEventArgs instance from the pool  
        // and returns the object removed from the pool  
        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        // The number of SocketAsyncEventArgs instances in the pool  
        public int Count
        {
            get { return m_pool.Count; }
        }

        public void Clear()
        {
            m_pool.Clear();
        }
    }
    #endregion

    #region AsyncUserToken
    public class AsyncUserToken
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
        public string usernamr { get; set; }

        public AsyncUserToken()
        {
            this.Buffer = new List<byte>();
        }
    }
    #endregion


}

