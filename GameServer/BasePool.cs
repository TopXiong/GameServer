using NetWorkTools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
namespace GameServer
{

    internal class ChannelPool : BaseObjectPool<Channel>
    {
        protected override Channel Create(params object[] o)
        {
            switch (o.Length)
            {
                case 1:
                    return new Channel((Socket)o[0]);
                case 2:
                    return new Channel((Socket)o[0], (bool)o[1]);
                default:
                    return null;
            }
            
        }

    }


    internal abstract class BaseObjectPool<T>
    {
        
        //可用池
        private static Queue<T> available;
        private static List<T> inUse;

        protected abstract T Create(params object[] o);

        static BaseObjectPool()            
        {
            available = new Queue<T>();
            inUse = new List<T>();
        }

        /// <summary>
        /// 取出
        /// </summary>
        /// <returns></returns>
        public T Get(params object[] o)
        {
            lock (available)
            {
                T instance;
                if (available.Count == 0)
                {
                    instance = Create(o);
                }
                else
                {
                    instance = available.Dequeue();
                    instance = Create(o); 
                }
                inUse.Add(instance);
                return instance;
            }
        }
        /// <summary>
        /// 放回
        /// </summary>
        public void Set(T instance)
        {
            available.Enqueue(instance);
            inUse.Remove(instance);
        }

    }
}
