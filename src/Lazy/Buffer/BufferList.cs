namespace Lazy.Buffer;

using System;
using System.Buffers;
using System.Collections.Generic;

/// <summary>
/// A high-performance, allocation-free circular buffer list that operates on <see cref="Span{T}"/>.
/// It supports operations like append, insert, remove, update, and search, designed to minimize GC allocations.
/// </summary>
/// <typeparam name="T">The unmanaged type of elements stored in the buffer.</typeparam>
/// <example>
/// <code>
/// Span&lt;int&gt; buffer = stackalloc int[5];
/// Span&lt;bool&gt; validation = stackalloc bool[5];
/// var list = new BufferList&lt;int&gt;(buffer, validation);
/// list.Append(1);
/// list.Append(2);
/// </code>
/// </example>
public ref struct BufferList<T> where T : unmanaged
{
    private Span<T> _buffer;
    private Span<bool> _hasValue; // Controla se o slot está preenchido ou "vazio" (Release)
    private int _head;
    private int _tail;
    private int _count;
    private int _maxAllowedSize;

    /// <summary>
    /// Gets the number of elements contained in the <see cref="BufferList{T}"/>.
    /// </summary>
    public readonly int Count => _count;

    /// <summary>
    /// Gets the maximum number of elements the <see cref="BufferList{T}"/> can hold.
    /// </summary>
    public readonly int Capacity => _maxAllowedSize;

    /// <summary>
    /// Gets a value indicating whether the <see cref="BufferList{T}"/> is full.
    /// </summary>
    public readonly bool IsFull => _count == _maxAllowedSize;

    /// <summary>
    /// Gets a value indicating whether the <see cref="BufferList{T}"/> is empty.
    /// </summary>
    public readonly bool IsEmpty => _count == 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferList{T}"/> struct using the specified buffers.
    /// </summary>
    /// <param name="buffer">The span to store the data elements.</param>
    /// <param name="validationBuffer">The span to store the validation flags (must be the same size as <paramref name="buffer"/>).</param>
    /// <exception cref="ArgumentException">Thrown when the sizes of the data and validation buffers do not match.</exception>
    /// <example>
    /// <code>
    /// Span&lt;int&gt; buffer = stackalloc int[5];
    /// Span&lt;bool&gt; validation = stackalloc bool[5];
    /// var list = new BufferList&lt;int&gt;(buffer, validation);
    /// </code>
    /// </example>
    public BufferList(Span<T> buffer, Span<bool> validationBuffer)
    {
        if (buffer.Length != validationBuffer.Length)
            throw new ArgumentException("Os buffers de dados e validação devem ter o mesmo tamanho.");

        _buffer = buffer;
        _hasValue = validationBuffer;
        _hasValue.Clear(); // Garante que começa tudo vazio
        _head = 0;
        _tail = 0;
        _count = 0;
        _maxAllowedSize = buffer.Length;
    }

    /// <summary>
    /// Adds an item to the end of the buffer. If the buffer is full, the oldest item is overwritten.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <example>
    /// <code>
    /// list.Append(10);
    /// </code>
    /// </example>
    public void Append(T item)
    {
        if (IsFull)
        {
            // Estratégia de rotação: avança o head descartando o antigo
            _hasValue[_head] = false;
            _head = (_head + 1) % _maxAllowedSize;
            _count--;
        }

        _buffer[_tail] = item;
        _hasValue[_tail] = true;
        _tail = (_tail + 1) % _maxAllowedSize;
        _count++;
    }

    /// <summary>
    /// Removes and returns the oldest item (from the beginning) of the buffer.
    /// </summary>
    /// <returns>The removed item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the buffer is empty.</exception>
    /// <example>
    /// <code>
    /// int first = list.RemoveFirst();
    /// </code>
    /// </example>
    public T RemoveFirst()
    {
        if (IsEmpty) 
            throw new InvalidOperationException("The buffer is empty.");

        T item = _buffer[_head];
        _hasValue[_head] = false;
        _head = (_head + 1) % _maxAllowedSize;
        _count--;
        return item;
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="match">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
    /// <param name="result">When this method returns, contains the element if found; otherwise, the default value.</param>
    /// <returns><c>true</c> if an element is found; otherwise, <c>false</c>.</returns>
    /// <example>
    /// <code>
    /// bool found = list.TryFind(x =&gt; x == 5, out int val);
    /// </code>
    /// </example>
    public readonly bool TryFind(Func<T, bool> match, out T result)
    {
        for (int i = 0; i < _maxAllowedSize; i++)
        {
            int idx = (_head + i) % _maxAllowedSize;
            if (_hasValue[idx] && match(_buffer[idx]))
            {
                result = _buffer[idx];
                return true;
            }
        }
        result = default!;
        return false;
    }

    /// <summary>
    /// Returns the first item of the buffer even if the slot is empty.
    /// </summary>
    /// <returns>The first valid item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the buffer is empty.</exception>
    /// <example>
    /// <code>
    /// int first = list.PeekFirst();
    /// </code>
    /// </example>
    public readonly T PeekFirst()
    {
        if (TryPeekFirst(out T value))
            return value;

        throw new InvalidOperationException("The buffer is empty.");
    }

    /// <summary>
    /// Tries to return the first valid item of the buffer.
    /// </summary>
    /// <param name="value">When this method returns, contains the first valid element if found; otherwise, the default value.</param>
    /// <returns><c>true</c> if an element is found; otherwise, <c>false</c>.</returns>
    /// <example>
    /// <code>
    /// if (list.TryPeekFirst(out int val)) { ... }
    /// </code>
    /// </example>
    public readonly bool TryPeekFirst(out T value)
    {
        for (int i = 0; i < _maxAllowedSize; i++)
        {
            int idx = (_head + i) % _maxAllowedSize;

            if (_hasValue[idx])
            {
                value = _buffer[idx];
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Returns the first non-empty item of the buffer.
    /// </summary>
    /// <returns>The first valid item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no valid items are found.</exception>
    /// <example>
    /// <code>
    /// int validFirst = list.PeekFirstValid();
    /// </code>
    /// </example>
    public readonly T PeekFirstValid()
    {
        for (int i = 0; i < _maxAllowedSize; i++)
        {
            int idx = (_head + i) % _maxAllowedSize;
            if (_hasValue[idx]) return _buffer[idx];
        }

        throw new InvalidOperationException("No valid items found.");
    }

    /// <summary>
    /// Returns the last item of the buffer.
    /// </summary>
    /// <returns>The last valid item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the buffer is empty.</exception>
    /// <example>
    /// <code>
    /// int last = list.PeekLast();
    /// </code>
    /// </example>
    public readonly T PeekLast()
    {
        if (TryPeekLast(out T value))
            return value;

        throw new InvalidOperationException("The buffer is empty.");
    }

    /// <summary>
    /// Tries to return the last valid item of the buffer.
    /// </summary>
    /// <param name="value">When this method returns, contains the last valid element if found; otherwise, the default value.</param>
    /// <returns><c>true</c> if an element is found; otherwise, <c>false</c>.</returns>
    /// <example>
    /// <code>
    /// if (list.TryPeekLast(out int val)) { ... }
    /// </code>
    /// </example>
    public readonly bool TryPeekLast(out T value)
    {
        for (int i = _maxAllowedSize - 1; i >= 0; i--)
        {
            int idx = (_head + i) % _maxAllowedSize;

            if (_hasValue[idx])
            {
                value = _buffer[idx];
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Returns the last non-empty item of the buffer.
    /// </summary>
    /// <returns>The last valid item.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no valid items are found.</exception>
    /// <example>
    /// <code>
    /// int validLast = list.PeekLastValid();
    /// </code>
    /// </example>
    public readonly T PeekLastValid()
    {
        for (int i = _maxAllowedSize - 1; i >= 0; i--)
        {
            int idx = (_head + i) % _maxAllowedSize;
            if (_hasValue[idx]) return _buffer[idx];
        }
        throw new InvalidOperationException("No valid items found.");
    }

    /// <summary>
    /// Inserts an element into the first empty slot found in the buffer.
    /// </summary>
    /// <param name="item">The element to insert.</param>
    /// <exception cref="OverflowException">Thrown when there are no empty slots available.</exception>
    /// <example>
    /// <code>
    /// list.Insert(42);
    /// </code>
    /// </example>
    public void Insert(T item)
    {
        for (int i = 0; i < _maxAllowedSize; i++)
        {
            if (!_hasValue[i])
            {
                _buffer[i] = item;
                _hasValue[i] = true;
                _count++;
                return;
            }
        }
        throw new OverflowException("No empty slot available for insertion.");
    }

    /// <summary>
    /// Appends an item to the buffer, resizing it using the provided new buffers if the current buffer is full.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="newBuffer">The new buffer span to use if resizing is needed.</param>
    /// <param name="newValidation">The new validation span to use if resizing is needed.</param>
    /// <example>
    /// <code>
    /// list.AppendGrow(item, newBuffer, newValidation);
    /// </code>
    /// </example>
    public void AppendGrow(T item, Span<T> newBuffer, Span<bool> newValidation)
    {
        if (!IsFull)
        {
            Append(item);
            return;
        }

        Resize(newBuffer.Length, newBuffer, newValidation);
        Append(item);
    }

    /// <summary>
    /// Updates an element at a logical index in the buffer.
    /// </summary>
    /// <param name="logicalIndex">The logical index of the element to update.</param>
    /// <param name="newItem">The new item value.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of bounds.</exception>
    /// <example>
    /// <code>
    /// list.Update(0, 99);
    /// </code>
    /// </example>
    public void Update(int logicalIndex, T newItem)
    {
        if (logicalIndex < 0 || logicalIndex >= _count) throw new IndexOutOfRangeException();
        int actualIndex = (_head + logicalIndex) % _maxAllowedSize;
        _buffer[actualIndex] = newItem;
    }

    /// <summary>
    /// Updates elements that match a specified condition.
    /// </summary>
    /// <param name="condition">A delegate that defines the condition for the elements to update.</param>
    /// <param name="newItem">The new item value.</param>
    /// <example>
    /// <code>
    /// list.UpdateWhere(x =&gt; x &lt; 0, 0);
    /// </code>
    /// </example>
    public void UpdateWhere(Func<T, bool> condition, T newItem)
    {
        for (int i = 0; i < _maxAllowedSize; i++)
        {
            if (_hasValue[i] && condition(_buffer[i]))
            {
                _buffer[i] = newItem;
            }
        }
    }

    /// <summary>
    /// Removes an item at a logical index and optionally reorganizes the list.
    /// </summary>
    /// <param name="logicalIndex">The logical index of the item to remove.</param>
    /// <param name="purge">If true, truncates the buffer to its valid elements after removing; otherwise, just reorganizes it.</param>
    /// <example>
    /// <code>
    /// list.Remove(0);
    /// </code>
    /// </example>
    public void Remove(int logicalIndex, bool purge = false)
    {
        Release(logicalIndex);        
        if (purge)
            Purge();    
        else
            Organize();
    }

    /// <summary>
    /// Removes an item at a logical index, leaving an empty slot.
    /// </summary>
    /// <param name="logicalIndex">The logical index of the item to release.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of bounds.</exception>
    /// <example>
    /// <code>
    /// list.Release(0);
    /// </code>
    /// </example>
    public void Release(int logicalIndex)
    {
        if (logicalIndex < 0 || logicalIndex >= _count) throw new IndexOutOfRangeException();
        int actualIndex = (_head + logicalIndex) % _maxAllowedSize;
        if (_hasValue[actualIndex])
        {
            _hasValue[actualIndex] = false;
            _count--;
        }
    }

    /// <summary>
    /// Returns the logical index of the first occurrence of a specific item.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    /// <returns>The zero-based logical index of the first occurrence of the item, if found; otherwise, -1.</returns>
    /// <example>
    /// <code>
    /// int idx = list.IndexOf(42);
    /// </code>
    /// </example>
    public readonly int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < _count; i++)
        {
            int idx = (_head + i) % _maxAllowedSize;
            if (_hasValue[idx] && comparer.Equals(_buffer[idx], item)) return i;
        }
        return -1;
    }

    /// <summary>
    /// Determines whether the buffer contains a specific value.
    /// </summary>
    /// <param name="item">The item to locate.</param>
    /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
    /// <example>
    /// <code>
    /// bool hasValue = list.Contains(42);
    /// </code>
    /// </example>
    public readonly bool Contains(T item) => IndexOf(item) != -1;

    /// <summary>
    /// Clears the contents of the buffer, leaving all slots empty.
    /// </summary>
    /// <example>
    /// <code>
    /// list.Clear();
    /// </code>
    /// </example>
    public void Clear()
    {
        _buffer.Clear();
        _hasValue.Clear();
        _head = 0;
        _tail = 0;
        _count = 0;
    }

    /// <summary>
    /// Sorts the elements in the buffer using a specified comparison.
    /// </summary>
    /// <param name="comparison">The comparison delegate used to compare elements.</param>
    /// <example>
    /// <code>
    /// list.Sort((x, y) =&gt; x.CompareTo(y));
    /// </code>
    /// </example>
    public void Sort(Comparison<T> comparison)
    {
        // Para ordenar um buffer circular sem corromper a lógica, primeiro linearizamos os dados válidos
        Organize();
        _buffer[.._count].Sort(comparison);
    }

    /// <summary>
    /// Reorganizes the buffer to make the valid items linear and shift empty slots to the end.
    /// </summary>
    /// <example>
    /// <code>
    /// list.Organize();
    /// </code>
    /// </example>
    public void Organize()
    {
        T[] rented = ArrayPool<T>.Shared.Rent(_maxAllowedSize);

        try
        {
            int tempCount = 0;

            Span<T> tempBuffer = rented.AsSpan(0, _maxAllowedSize);

            for (int i = 0; i < _maxAllowedSize; i++)
            {
                int idx = (_head + i) % _maxAllowedSize;

                if (_hasValue[idx])
                    tempBuffer[tempCount++] = _buffer[idx];
            }

            tempBuffer[..tempCount].CopyTo(_buffer);

            _hasValue.Clear();
            _hasValue[..tempCount].Fill(true);

            _head = 0;
            _tail = tempCount % _maxAllowedSize;
            _count = tempCount;
        }
        finally
        {
            ArrayPool<T>.Shared.Return(rented);
        }
    }

    /// <summary>
    /// Copies the valid elements of the buffer to a destination span.
    /// </summary>
    /// <param name="destination">The span to copy elements into.</param>
    /// <exception cref="ArgumentException">Thrown when the destination span is too small.</exception>
    /// <example>
    /// <code>
    /// Span&lt;int&gt; dest = stackalloc int[list.Count];
    /// list.CopyTo(dest);
    /// </code>
    /// </example>
    public readonly void CopyTo(Span<T> destination)
    {
        if (destination.Length < _count) throw new ArgumentException("A very small destination.");
        int destIdx = 0;
        for (int i = 0; i < _count; i++)
        {
            int idx = (_head + i) % _maxAllowedSize;
            if (_hasValue[idx]) destination[destIdx++] = _buffer[idx];
        }
    }

    /// <summary>
    /// Returns the elements of the buffer as an array.
    /// </summary>
    /// <returns>An array containing the valid elements.</returns>
    /// <example>
    /// <code>
    /// int[] array = list.ToArray();
    /// </code>
    /// </example>
    public readonly T[] ToArray()
    {
        T[] array = new T[_count];
        CopyTo(array);
        return array;
    }

    /// <summary>
    /// Truncates the buffer, reducing its logical size to a specific maximum count.
    /// </summary>
    /// <param name="newMaxCount">The new maximum count.</param>
    /// <example>
    /// <code>
    /// list.Truncate(2);
    /// </code>
    /// </example>
    public void Truncate(int newMaxCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(newMaxCount);

        if (newMaxCount >= _count)
            return;

        Organize();

        int toRemove = _count - newMaxCount;
        for (int i = 0; i < toRemove; i++)
        {
            int actualIdx = (_head + i) % _maxAllowedSize;
            _hasValue[actualIdx] = false;
        }

        _head = (_head + toRemove) % _maxAllowedSize;
        _count = newMaxCount;
    }

    /// <summary>
    /// Resizes the buffer capacity by moving items to a larger external buffer.
    /// </summary>
    /// <param name="newSize">The new maximum allowed size.</param>
    /// <param name="newBuffer">The new underlying buffer to hold items.</param>
    /// <param name="newValidation">The new validation buffer.</param>
    /// <example>
    /// <code>
    /// list.Resize(newSize, newBufferSpan, newValidationSpan);
    /// </code>
    /// </example>
    public void Resize(int newSize, Span<T> newBuffer, Span<bool> newValidation)
    {
        if (newSize <= _maxAllowedSize) return;

        Organize(); // Garante dados lineares antes de mover
        _buffer[.._count].CopyTo(newBuffer);
        _hasValue[.._count].CopyTo(newValidation);

        _buffer = newBuffer;
        _hasValue = newValidation;
        _maxAllowedSize = newSize;
        _tail = _count;
    }

    /// <summary>
    /// Removes internal empty slots and sets the buffer's capacity to the current number of valid items.
    /// </summary>
    /// <example>
    /// <code>
    /// list.Purge();
    /// </code>
    /// </example>
    public void Purge()
    {
        Organize();
        _maxAllowedSize = _count;
    }

    /// <summary>
    /// Returns a generic list containing all valid items.
    /// </summary>
    /// <returns>A new <see cref="List{T}"/> containing the items.</returns>
    /// <example>
    /// <code>
    /// List&lt;int&gt; l = list.ToList();
    /// </code>
    /// </example>
    public readonly List<T> ToList()
    {
        // Aloca a lista diretamente com a capacidade exata
        var list = new List<T>(_count);
        for (int i = 0; i < _count; i++)
        {
            int idx = (_head + i) % _maxAllowedSize;
            if (_hasValue[idx]) list.Add(_buffer[idx]);
        }

        return list;
    }


    /// <summary>
    /// Returns an enumerator that iterates through the buffer list.
    /// </summary>
    /// <returns>An enumerator for the buffer list.</returns>
    /// <example>
    /// <code>
    /// foreach (var item in list) { ... }
    /// </code>
    /// </example>
    public readonly Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Enumerates the elements of a <see cref="BufferList{T}"/>.
    /// </summary>
    public ref struct Enumerator
    {
        private readonly BufferList<T> _source;
        private int _index;

        internal Enumerator(BufferList<T> source)
        {
            _source = source;
            _index = -1;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the buffer list.
        /// </summary>
        /// <returns><c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            while (++_index < _source._maxAllowedSize)
            {
                int actualIdx = (_source._head + _index) % _source._maxAllowedSize;
                if (_source._hasValue[actualIdx]) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        public readonly ref T Current
        {
            get
            {
                int actualIdx = (_source._head + _index) % _source._maxAllowedSize;
                return ref _source._buffer[actualIdx];
            }
        }
    }
}