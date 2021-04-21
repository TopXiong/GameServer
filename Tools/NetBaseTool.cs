using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TF.Tools
{
    public static class NetBaseTool
    {

        public static void SendBaseNetObject(Socket s,BaseNetObject baseNetObject)
        {
            SendVarData(s, ObjectToBytes(baseNetObject));
        }

        public static BaseNetObject ReceiveBaseNetObject(Socket s)
        {
            return (BaseNetObject)BytesToObject(ReceiveVarData(s));
        }

        /// <summary> 
        /// 将一个object对象序列化，返回一个byte[]         
        /// </summary> 
        /// <param name="obj">能序列化的对象</param>         
        /// <returns></returns> 
        public static byte[] ObjectToBytes(BaseNetObject obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter(); formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原         
        /// </summary>
        /// <param name="bytes"></param>         
        /// <returns></returns> 
        public static object BytesToObject(byte[] bytes)
        {
            Console.WriteLine(bytes.Length);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// 基础通讯发
        /// </summary>
        /// <param name="s">通讯Socket</param>
        /// <param name="data">字节数据</param>
        /// <returns>发送的数据长度</returns>
        private static int SendVarData(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;
            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            sent = s.Send(datasize);
            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            return total;
        }

        /// <summary>
        /// 基础通讯收
        /// </summary>
        /// <param name="s">通讯Socket</param>
        /// <returns>接收的字节数组</returns>
        private static byte[] ReceiveVarData(Socket s)
        {
            int total = 0;
            int recv;
            byte[] datasize = new byte[4];
            recv = s.Receive(datasize, 0, 4, SocketFlags.None);
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];
            while (total < size)
            {
                recv = s.Receive(data, total, dataleft, SocketFlags.None);
                if (recv == 0)
                {
                    data = null;
                    break;
                }
                total += recv;
                dataleft -= recv;
            }
            return data;
        }
    }
}
