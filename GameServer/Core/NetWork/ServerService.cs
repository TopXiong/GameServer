using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GameServer.Core.NetWork
{
    public class ServerService
    {
        private Socket socket;
        private SocketAsyncEventArgs saea;
        private StringBuilder buffers;

        public ServerService(string ip,int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            socket.Listen(10);
            saea = new SocketAsyncEventArgs();
            saea.Completed += Accept;
            BeginAccept(saea);
        }

        void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    Receive(e);
                    break;
                case SocketAsyncOperation.Send:
                    Send(e);
                    break;
                default:
                    throw new ArgumentException(e.ToString());
            }
        }


        /// <summary>
        /// 开始接受客户端连接·
        /// </summary>
        /// <param name="e"></param>
        private void BeginAccept(SocketAsyncEventArgs e)
        {
            e.AcceptSocket = null;
            //异步接受连接完
            if (!socket.AcceptAsync(e))
                Accept(socket, e);
        }

        void Accept(object sender, SocketAsyncEventArgs e)
        {
            Socket s = e.AcceptSocket;
            e.AcceptSocket = null;
            int bufferSize = 10;
            var args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
            args.SetBuffer(new byte[bufferSize], 0, bufferSize);
            args.AcceptSocket = s;
            if (!s.ReceiveAsync(args))
                Receive(args);
            BeginAccept(e);
        }

        void Send(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                if (!e.AcceptSocket.ReceiveAsync(e))
                {
                    Receive(e);
                }
            }
        }

        void Receive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
                    var data = Encoding.ASCII.GetString(e.Buffer, e.Offset, e.BytesTransferred);

                    buffers.Append(data);
                    Console.WriteLine("Received:{0}", data);
                    // // ReceiveAll
                    if (e.AcceptSocket.Available == 0)
                    {
                        Console.WriteLine("Receive Complete.Data:{0}", buffers.ToString());
                        buffers = new StringBuilder();
                        //send return;

                        Byte[] sendBuffer = Encoding.ASCII.GetBytes("result from server");
                        e.SetBuffer(sendBuffer, 0, sendBuffer.Length);
                        if (!e.AcceptSocket.SendAsync(e))
                        {
                            Send(e);
                        }
                    }
                }
                else if (!e.AcceptSocket.ReceiveAsync(e))
                {
                    Receive(e);
                }
            }
        }

    }
}
