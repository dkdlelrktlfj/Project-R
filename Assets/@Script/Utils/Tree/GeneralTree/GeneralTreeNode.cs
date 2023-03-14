using System;
using System.Collections.Generic;

public interface INode<TKey, TValue> : IComparable<TKey>
{
    TKey GetKey();
}

public sealed class GeneralTreeNode<TKey, TValue> where TValue : INode<TKey, TValue>
{
    internal TValue data;
    internal GeneralTreeNode<TKey, TValue> parent = null;
    internal List<GeneralTreeNode<TKey, TValue>> children;
    public GeneralTreeNode(TValue _data, GeneralTreeNode<TKey, TValue> parent = null)
    {
        this.parent = parent;
        this.data = _data;
        children = new List<GeneralTreeNode<TKey, TValue>>();
    }
}