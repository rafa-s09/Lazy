namespace Lazy.Buffer;

public ref struct BufferList<T> where T : unmanaged
{
    private Span<T> _buffer;
    private Span<bool> _hasValue; // Controla se o slot está preenchido ou "vazio" (Release)
    private int _head;
    private int _tail;
    private int _count;
    private int _maxAllowedSize;

    public readonly int Count => _count;
    public readonly int Capacity => _maxAllowedSize;
    public readonly bool IsFull => _count == _maxAllowedSize;
    public readonly bool IsEmpty => _count == 0;

    // Construtor corrigido recebendo obrigatoriamente os buffers da Stack ou Heap
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

    // Adiciona um elemento no final do buffer
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

    // Remove e retorna o elemento mais antigo (do início)
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

    // Busca usando um delegado de alta performance
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

    // Retorna o primeiro item do buffer mesmo se o slot estiver vazio
    public readonly T PeekFirst()
    {
        if (TryPeekFirst(out T value))
            return value;

        throw new InvalidOperationException("The buffer is empty.");
    }

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


    // Retorna o primeiro item nao vazio do buffer
    public readonly T PeekFirstValid()
    {
        for (int i = 0; i < _maxAllowedSize; i++)
        {
            int idx = (_head + i) % _maxAllowedSize;
            if (_hasValue[idx]) return _buffer[idx];
        }

        throw new InvalidOperationException("No valid items found.");
    }

    // Retorna o ultimo item do buffer mesmo se o slot estiver vazio
    public readonly T PeekLast()
    {
        if (TryPeekLast(out T value))
            return value;

        throw new InvalidOperationException("The buffer is empty.");
    }

    // Retorna o ultimo item do buffer mesmo se o slot estiver vazio
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

    // Retorna o ultimo item nao vazio do buffer
    public readonly T PeekLastValid()
    {
        for (int i = _maxAllowedSize - 1; i >= 0; i--)
        {
            int idx = (_head + i) % _maxAllowedSize;
            if (_hasValue[idx]) return _buffer[idx];
        }
        throw new InvalidOperationException("No valid items found.");
    }

    // Insere um elemento no primeiro slot vazio encontrado, se nao encontrar retorna exception
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

    // Insere um elemento e amplia o tamanho se estiver cheio (Estratégia de substituição de buffer externo)
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

    // Atualiza o elemento no índice lógico
    public void Update(int logicalIndex, T newItem)
    {
        if (logicalIndex < 0 || logicalIndex >= _count) throw new IndexOutOfRangeException();
        int actualIndex = (_head + logicalIndex) % _maxAllowedSize;
        _buffer[actualIndex] = newItem;
    }

    // Atualiza o elemento conforme a condicao
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

    // Remove o item que esta no slot lógico, e reorganiza a lista
    public void Remove(int logicalIndex, bool purge = false)
    {
        Release(logicalIndex);        
        if (purge)
            Purge();    
        else
            Organize();
    }

    // Remove o item que esta no slot lógico, deixa o slot vazio
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

    // Retorna o índice lógico do primeiro elemento que der MATCH
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

    public readonly bool Contains(T item) => IndexOf(item) != -1;

    // limpa todos os slots
    public void Clear()
    {
        _buffer.Clear();
        _hasValue.Clear();
        _head = 0;
        _tail = 0;
        _count = 0;
    }

    // Sorteia/Ordena o buffer baseado em uma expressão de comparação
    public void Sort(Comparison<T> comparison)
    {
        // Para ordenar um buffer circular sem corromper a lógica, primeiro linearizamos os dados válidos
        Organize();
        _buffer[.._count].Sort(comparison);
    }

    // Organiza os slots para deixar os vazios no fim do buffer (Linearização)
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

    // Retorna um array tradicional alocado no heap
    public readonly T[] ToArray()
    {
        T[] array = new T[_count];
        CopyTo(array);
        return array;
    }

    // Reduz o tamanho lógico da lista para o novo tamanho especificado
    public void Truncate(int newMaxCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(newMaxCount);

        if (newMaxCount >= _count)
            return;

        int toRemove = _count - newMaxCount;
        _head = (_head + toRemove) % _maxAllowedSize;
        _count = newMaxCount;
    }

    // Amplia o tamanho/capacidade injetando uma nova memória externa maior
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

    // Remove todos os slots vazios internos e trunca o buffer para o tamanho dos válidos
    public void Purge()
    {
        Organize();
        _maxAllowedSize = _count;
    }


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


    // Enumerador nativo para habilitar o uso de foreach
    public readonly Enumerator GetEnumerator() => new();

    public ref struct Enumerator
    {
        private readonly BufferList<T> _source;
        private int _index;

        internal Enumerator(BufferList<T> source)
        {
            _source = source;
            _index = -1;
        }

        public bool MoveNext()
        {
            while (++_index < _source._maxAllowedSize)
            {
                int actualIdx = (_source._head + _index) % _source._maxAllowedSize;
                if (_source._hasValue[actualIdx]) return true;
            }
            return false;
        }

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