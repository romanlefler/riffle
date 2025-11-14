
namespace Riffle.Utilities {

    public class BoundedQueue<T> : Queue<T>
    {
        private int _maxCount;

        public BoundedQueue(int maxCount) : base()
        {
            MaxCount = maxCount;
        }

        public BoundedQueue(int maxCount, int initialCapacity) : base(initialCapacity)
        {
            MaxCount = maxCount;
        }

        public BoundedQueue(int maxCount, IEnumerable<T> collection) : base(collection)
        {
            MaxCount = maxCount;
        }

        public int MaxCount
        {
            get => _maxCount; set
            {
                if(value < 1) throw new ArgumentException("Max queue count cannot be less than 1.");
                _maxCount = value;
            }
        }

        public void EnqueueMany(IEnumerable<T> collection)
        {
            foreach(T k in collection) Enqueue(k);
        }

        public void EnsureCount()
        {
            while(Count > MaxCount) Dequeue();
        }

        public void CheckedEnqueue(T item)
        {
            Enqueue(item);
            EnsureCount();
        }

        public void CheckedEnqueueMany(IEnumerable<T> collection)
        {
            EnqueueMany(collection);
            EnsureCount();
        }

    }
}
