using System;
using System.Buffers;
using System.Linq;

namespace MyMostUsedWords.Buffers
{
    public class ArrayBuffer<T> : IDisposable
    {
        T[] _items;
        public int Length { get; private set; }

        public ArrayBuffer(int maximumSize)
        {
            Clear();
            _items = ArrayPool<T>.Shared.Rent(maximumSize);
        }

        public bool Add(T item)
        {
            if (Length >= _items.Length)
                return false;

            _items[Length++] = item;
            return true;
        }

        public bool AddRange(T[] newItems)
        {
            foreach (var item in newItems)
            {
                if (!Add(item))
                {
                    return false;
                }
            }
            return true;
        }

        public void Clear()
        {
            Length = 0;
        }

        public T[] ToArray()
        {
            return _items.Take(Length).ToArray();
        }

        public T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public bool Contains(T item) => Array.IndexOf(_items, item, 0, Length) > -1;

        public void Dispose()
        {
            ArrayPool<T>.Shared.Return(_items);
            Clear();
        }
    }
}
