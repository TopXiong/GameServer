using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameServer.Log;

namespace NetWorkTools
{
    public class Channel
    {
        private static readonly int DELAY = 500;
        //连接Socket
        protected Socket socket;

        protected Queue< BaseNetObject> receiveBaseNetObjects;

        protected Queue<BaseNetObject> sendBaseNetObjects;

        private bool used;

        //心跳包检测
        private System.Timers.Timer t;

        private bool receiveHeartBeat;
        private bool useHeartBeat;
        /// <summary>
        /// 管道类型，封装Socket，使用两个队列传递消息
        /// </summary>
        /// <param name="socket">通讯socket</param>
        /// <param name="useHeartBeat">是否启用心跳包，客户端固定填false</param>
        public Channel(Socket socket,bool useHeartBeat = true)
        {
            used = true;
            this.useHeartBeat = useHeartBeat;
            receiveHeartBeat = true;
            this.socket = socket;
            receiveBaseNetObjects = new Queue<BaseNetObject>();
            sendBaseNetObjects = new Queue<BaseNetObject>();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Send));
            ThreadPool.QueueUserWorkItem(new WaitCallback(Receive));
            //实例化Timer类，设置间隔时间为5000毫秒；
            if (useHeartBeat)
            {
                t = new System.Timers.Timer(5000);
                t.Elapsed += new System.Timers.ElapsedEventHandler(SendHeartBeat);
                //到达时间的时候执行事件； 
                t.AutoReset = true;
                t.Start();
            }
        }

        private void Send(object o)
        {
            try
            {
                while (used)
                {
                    if (SendBaseNetObjects.Count > 0)
                    {
                        BaseNetObject baseNetObject = SendBaseNetObjects.Dequeue();
                        NetBaseTool.SendBaseNetObject(socket, baseNetObject);
                    }
                    Thread.Sleep(DELAY);
                }
            }
            catch(SocketException e)
            {
                if (t != null)
                {
                    t.Stop();
                }              
                Logger.WriteException(e, "心跳中断");
                used = false;
                return;
            }
        }

        private void Receive(object o)
        {
            try
            {
                while (used)
                {
                    BaseNetObject baseNetObject = NetBaseTool.ReceiveBaseNetObject(socket);
                    if(baseNetObject is HeartBeat)
                    {
                        receiveHeartBeat = true;
                        Logger.WriteLog("receive heartbeat confirm alive");
                        if(!useHeartBeat)
                            SendBaseNetObjects.Enqueue(new HeartBeat());
                    }
                    else
                    {
                        ReceiveBaseNetObjects.Enqueue(baseNetObject);
                    }
                    Thread.Sleep(DELAY);
                }
            }
            catch (SocketException e)
            {
                if (t != null)
                {
                    t.Stop();
                }
                Logger.WriteException(e, "心跳中断");
                used = false;
                return;
            }
        }

        //发送心跳包
        private void SendHeartBeat(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (receiveHeartBeat)
            {
                Console.WriteLine("确认存活");
                receiveHeartBeat = false;
                SendBaseNetObjects.Enqueue(new HeartBeat());
            }
            else
            {
                Console.WriteLine("心跳中断");
                socket.Close();               
            }
        }

        public Queue<BaseNetObject> ReceiveBaseNetObjects { get { lock (receiveBaseNetObjects) { return receiveBaseNetObjects; } }  }
        public Queue<BaseNetObject> SendBaseNetObjects { get { lock (sendBaseNetObjects) { return sendBaseNetObjects; } } }
    }
}
