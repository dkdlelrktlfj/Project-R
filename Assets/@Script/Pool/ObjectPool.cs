using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : IPoolable, new()
{
    protected Stack<T> pool = null;
    public int Size => pool.Count;

    public ObjectPool(int _size = 10)
    {
        pool = new Stack<T>(_size);
        Initilize(_size);
    }

    public void Resize(int _size)
    {
        pool.Clear();
        pool = new Stack<T>(_size);
        Initilize(_size);
    }

    public void Clear()
    {
        pool.Clear();
    }

    public T Pop()
    {
        if(pool.Count > 0)
        {
            T result = pool.Pop();
            result.OnReuse();
            return result;
        }

        T createResult = new T();
        createResult.OnReuse();
        return createResult;
    }

    public void Push(T _item)
    {
        _item.OnRelease();
        pool.Push(_item);
    }

    protected virtual void Initilize(int _size)
    {
        for (int index = 0; index < _size; index++)
        {
            Push(new T());
        }
    }
}
