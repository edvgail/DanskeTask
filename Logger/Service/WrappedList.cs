namespace Company.Logger.Service
{
    internal class WrappedList<T>
    {
        private List<T> list = new();
        private object sync = new();

        public int Count
        {
            get
            {
                lock (sync)
                {
                    return this.list.Count;
                }
            }
        }

        public void Add(T value)
        {
            lock (sync)
            {
                this.list.Add(value);
            }
        }

        public bool Remove(T item)
        {
            lock (sync)
            {
                return this.list.Remove(item);
            }
        }

        public T? TakeOne()
        {
            lock (sync)
            {
                return this.list.FirstOrDefault();
            }
        }

        public void Clean()
        {
            lock (sync)
            {
                this.list.Clear();
            }
        }
    }
}