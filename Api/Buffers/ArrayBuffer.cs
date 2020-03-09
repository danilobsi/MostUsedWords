using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyMostUsedWords.Buffers
{
    public class ArrayBuffer<T> : IDisposable, IList<T>
    {
        T[] _items;
        public int Count { get; private set; }
        public bool IsReadOnly => false;

        public ArrayBuffer(int size)
        {
            Count = 0;
            _items = ArrayPool<T>.Shared.Rent(size);
        }

        public void Add(T item) => _items[Count++] = item;

        public bool TryAdd(T item)
        {
            if (Count >= _items.Length)
                return false;

            Add(item);
            return true;
        }

        public void AddRange(T[] newItems)
        {
            Array.Copy(newItems, 0, _items, Count, newItems.Length);
            Count += newItems.Length;
        }

        public bool TryAddRange(T[] newItems)
        {
            if (Count + newItems.Length > _items.Length)
                return false;

            AddRange(newItems);
            return true;
        }

        public void Clear() => Count = 0;

        public T[] ToArray() => _items.Take(Count).ToArray();

        public T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public bool Contains(T item) => IndexOf(item) > -1;

        public int IndexOf(T item) => Array.IndexOf(_items, item, 0, Count);

        public void Insert(int index, T item)
        {
            Array.Copy(_items, index, _items, index + 1, Count++ - index);
            _items[index] = item;
        }

        public void RemoveAt(int index) => Array.Copy(_items, index + 1, _items, index, --Count - index);

        public void CopyTo(T[] array, int arrayIndex) => Array.Copy(_items, 0, array, arrayIndex, Count);

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0) 
                return false;

            RemoveAt(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator() => new ArrayBufferEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            ArrayPool<T>.Shared.Return(_items);
            Clear();
        }
    }

    public class ArrayBufferEnumerator<T> : IEnumerator<T>
    {
        ArrayBuffer<T> _buffer { get; set; }
        public int _index { get; set; }

        public ArrayBufferEnumerator(ArrayBuffer<T> buffer) {
            _buffer = buffer;
            _index = 0;
        }
        public T Current => _buffer[_index];

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_index + 1 >= _buffer.Count)
                return false;
            
            _index++;
            return true;
        }

        public void Reset() => _index = 0;

        public void Dispose() { }
    }
}