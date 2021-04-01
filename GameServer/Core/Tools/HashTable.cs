using System.Collections.Generic;

namespace GameServer.Core.Tools
{
    internal class HashTable<T>
    {

        private Dictionary<int,T> list; 

        public HashTable() { list = new Dictionary<int,T>(); }

        public int Add(T instance)
        {
            int index = instance.GetHashCode();
            while (list.ContainsKey(index))
            {
                index = index.GetHashCode();
            }
            list.Add(index, instance);
            return index;
        }

        public T GetByID(int index)
        {
            return list[index];
        }

        /// <summary>
        /// 查询ID，-1为没找到
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public int GetID(T instance)
        {
            foreach(var keyValue in list)
            {
                if (keyValue.Value.Equals(instance))
                {
                    return keyValue.Key;
                }                 
            }
            return -1;
        }

        public List<T> GetList()
        {
            List<T> temp = new List<T>();
            foreach(var keyValye in list)
            {
                temp.Add(keyValye.Value);
            }
            return temp;
        }
    }
}