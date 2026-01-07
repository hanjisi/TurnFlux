using System.Collections.Generic;

namespace TurnFlux
{
    internal class SimplePriorityQueue<T>
    {
        private readonly SortedDictionary<int, Queue<T>> _dict = new SortedDictionary<int, Queue<T>>();
        public int Count { get; private set; } = 0;

        public void Enqueue(T item, int priority)
        {
            if (!_dict.TryGetValue(priority, out var q))
            {
                q = new Queue<T>();
                _dict[priority] = q;
            }
            q.Enqueue(item);
            Count++;
        }

        public bool TryDequeue(out T item)
        {
            // 直接从小到大遍历 Keys，0 会在 100 之前被取出来
            foreach (var key in _dict.Keys)
            {
                var q = _dict[key];
                if (q.Count > 0)
                {
                    item = q.Dequeue();
                    Count--;
                    if (q.Count == 0) _dict.Remove(key);
                    return true;
                }
            }
            item = default;
            return false;
        }
    }
}
