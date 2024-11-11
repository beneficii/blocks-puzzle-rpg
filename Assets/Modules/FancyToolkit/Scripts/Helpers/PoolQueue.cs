using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace FancyToolkit
{
    public class PoolQueue <T>
    {
        List<T> initialList;
        Queue<T> queue;

        public PoolQueue(IEnumerable<T> list)
        {
            Assert.IsTrue(list.Count() > 0);

            initialList = list.ToList();
            GenerateQueue();
        }

        void GenerateQueue()
        {
            queue = new Queue<T>(initialList.OrderBy(x => System.Guid.NewGuid()));
        }

        public void Add(T item)
        {
            initialList.Add(item);
        }

        public T Get()
        {
            if (queue.Count == 0) GenerateQueue();

            return queue.Dequeue();
        }

        public List<T> Get(int count)
        {
            var list = new List<T>();
            for (int i = 0; i < count; i++) list.Add(Get());
            
            return list;
        }
    }
}