using System.Collections.Generic;


namespace GameServer.Core.Tools
{
    /// <summary>
    /// 多重映射表
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class MultiMap<K, V> : SortedDictionary<K, List<V>>
    {
        private readonly List<V> Empty = new List<V>();

        public void Add(K key, V value)
        {
            TryGetValue(key, out List<V> list);
            if (list == null)
            {
                list = new List<V>();
                Add(key, list);
            }
            list.Add(value);
        }

        public bool Remove(K key, V value)
        {
            TryGetValue(key, out List<V> list);
            if (list == null)
            {
                return false;
            }
            if (!list.Remove(value))
            {
                return false;
            }
            if (list.Count == 0)
            {
                Remove(key);
            }
            return true;
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public V[] GetAll(K k)
        {
            TryGetValue(k, out List<V> list);
            if (list == null)
            {
                return new V[0];
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public new List<V> this[K t]
        {
            get
            {
                TryGetValue(t, out List<V> list);
                return list ?? Empty;
            }
        }

        public V GetOne(K t)
        {
            TryGetValue(t, out List<V> list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default;
        }

        public bool Contains(K t, V k)
        {
            this.TryGetValue(t, out List<V> list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(k);
        }

    }
}
