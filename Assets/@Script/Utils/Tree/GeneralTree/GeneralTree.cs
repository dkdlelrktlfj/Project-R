using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GeneralTree<TKey, TValue> where TValue : INode<TKey, TValue>
{
    //루트 노드
    private GeneralTreeNode<TKey, TValue> rootNode;

    public GeneralTree(TValue _root)
    {
        rootNode = new GeneralTreeNode<TKey, TValue>(_root, null);
    }

    #region Find
    public bool HasKey(TKey _key)
    {
        var node = FindNode(_key);
        if (node != null)
        {
            return true;
        }

        Utility.Log($"{_key} 데이터를 찾을 수 없음");
        return false;
    }

    /// <summary>
    /// Key에 해당하는 노드의 데이터를 반환, 최악의 경우 O(n)의 검색속도
    /// </summary>
    /// <param name="_key">키</param>
    /// <returns>데이터</returns>
    public bool Find(TKey _key, out TValue _result)
    {
        var node = FindNode(_key);
        if (node != null)
        {
            _result = node.data;
            return true;
        }

        Utility.Log($"{_key} 데이터를 찾을 수 없음");
        _result = default;
        return false;
    }

    private GeneralTreeNode<TKey, TValue> FindNode(TKey _key)
    {
        //찾고자 하는 Key의 데이터가 현재 Data와 같음
        if (rootNode.data.CompareTo(_key) == 0)
            return rootNode;

        Queue<GeneralTreeNode<TKey, TValue>> queue = new Queue<GeneralTreeNode<TKey, TValue>>();
        queue.Enqueue(rootNode);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            if (node.data.CompareTo(_key) == 0)
                return node;

            for (int index = 0; index < node.children.Count; index++)
                queue.Enqueue(node.children[index]);
        }

        Utility.Log($"{_key}노드를 찾을 수 없음");
        return null;
    }
    #endregion

    #region Add
    //데이터 추가
    public void Add(TKey _key, TValue _value, bool _swapData = true)
    {
        var node = FindNode(_key);
        if (node != null)
        {
            //이미 노드가 존재함
            Utility.Log($"이미 존재하는 노드 : {_key}", Color.red);

            //노드가 존재하므로 데이터 교체
            if (_swapData == true)
                node.data = _value;

            return;
        }

        //찾고하자는 Key 노드가 없으므로 현재 노드에 새로운 노드를 추가
        GeneralTreeNode<TKey, TValue> newNode = new GeneralTreeNode<TKey, TValue>(_value, rootNode);
        Utility.Log($"{this.rootNode.data.GetKey()}에 {newNode.data.GetKey()}의 노드가 추가됨");
        rootNode.children.Add(newNode);
    }

    public void AddChild(TKey _parentKey, TValue _value)
    {
        var node = FindNode(_parentKey);
        if (node != null)
        {
            var childNode = node.children.Find(f => f.data.CompareTo(_value.GetKey()) == 0);
            if (childNode == null)
            {
                //찾고하자는 Key 노드가 없으므로 현재 노드에 새로운 노드를 추가
                GeneralTreeNode<TKey, TValue> newNode = new GeneralTreeNode<TKey, TValue>(_value, node);
                Utility.Log($"{node.data.GetKey()}에 {newNode.data.GetKey()}의 노드가 추가됨");
                node.children.Add(newNode);
            }
            return;
        }
    }
    #endregion

    #region Remove
    public void RemoveNode(TKey _key)
    {
        var node = FindNode(_key);

        //키에 해당하는 노드가 없음
        if (node == null)
        {
            Utility.Log($"{_key}에 해당하는 노드를 찾을 수 없음", Color.red);
            return;
        }

        //키에 해당하는 노드가 루트임
        if (node.parent == null)
        {
            Utility.Log($"{_key} 루트 노드는 삭제할 수 없습니다.", Color.red);
            return;
        }

        Utility.Log($"{node.parent.data.GetKey()}에서 {_key}노드를 삭제하였습니다.", Color.blue);
        node.parent.children.Remove(node);
    }
    #endregion

    /// <summary>
    /// Traval
    /// </summary>
    /// <returns>Nodes</returns>
    public List<TValue> GetAllNode()
    {
        List<TValue> result = new List<TValue>();

        Queue<GeneralTreeNode<TKey, TValue>> explorationQueue = new Queue<GeneralTreeNode<TKey, TValue>>();
        explorationQueue.Enqueue(rootNode);
        while (explorationQueue.Count > 0)
        {
            var node = explorationQueue.Dequeue();
            result.Add(node.data);
            Utility.Log($"{node.data.GetKey()}");

            for (int index = 0; index < node.children.Count; index++)
                explorationQueue.Enqueue(node.children[index]);
        }

        return result;
    }

    public List<TValue> GetAllNode(GeneralTreeNode<TKey, TValue> _node)
    {
        List<TValue> result = new List<TValue>();

        Queue<GeneralTreeNode<TKey, TValue>> explorationQueue = new Queue<GeneralTreeNode<TKey, TValue>>();
        explorationQueue.Enqueue(_node);
        while (explorationQueue.Count > 0)
        {
            var node = explorationQueue.Dequeue();
            result.Add(node.data);
            Utility.Log($"{node.data.GetKey()}");

            for (int index = 0; index < node.children.Count; index++)
                explorationQueue.Enqueue(node.children[index]);
        }

        return result;
    }

    /// <summary>
    /// root 노드를 기준으로 depth 만큼 Traval하여 결과값 리턴
    /// </summary>
    public List<TValue> GetAllNodeByDepth(uint _depth = 0)
    {
        if (_depth == 0)
            return GetAllNode();

        List<TValue> result = new List<TValue>();

        int depth = -1;

        Queue<GeneralTreeNode<TKey, TValue>> explorationQueue = new Queue<GeneralTreeNode<TKey, TValue>>();

        explorationQueue.Enqueue(rootNode);
        while (depth < _depth)
        {
            int allOfQueueSize = explorationQueue.Count;
            while (allOfQueueSize > 0)
            {
                var node = explorationQueue.Dequeue();
                if (node.data.CompareTo(rootNode.data.GetKey()) != 0)
                {
                    result.Add(node.data);
                    Utility.Log($"{node.data.GetKey()}");
                }

                for (int index = 0; index < node.children.Count; index++)
                    explorationQueue.Enqueue(node.children[index]);

                allOfQueueSize--;
            }

            depth++;
            if (depth > _depth)
                break;
        }

        //Root - size = 1, depth = 0
        //A B C - size = 3, depth = 1
        //B C A-1 A-2 A-3 - size = 2, depth = 1
        //C A-1 A-2 A-3 B-1 B-2 B-3 - size = 1, depth = 1
        //A-1 A-2 A-3 B-1 B-2 B-3 C-1 C-2 C-3 - size = 0, depth = 1

        return result;
    }

    /// <summary>
    /// Key에 해당하는 노드를 기준으로 depth 만큼 Traval하여 결과값 리턴
    /// </summary>
    public List<TValue> GetAllNodeByKeyWithDepth(TKey _key, uint _depth = 0)
    {
        var root = FindNode(_key);

        if (_depth == 0)
            return GetAllNode(root);

        List<TValue> result = new List<TValue>();

        int depth = -1;

        Queue<GeneralTreeNode<TKey, TValue>> explorationQueue = new Queue<GeneralTreeNode<TKey, TValue>>();

        explorationQueue.Enqueue(root);
        while (depth < _depth)
        {
            int allOfQueueSize = explorationQueue.Count;
            while (allOfQueueSize > 0)
            {
                var node = explorationQueue.Dequeue();
                if (node.data.CompareTo(_key) != 0)
                {
                    result.Add(node.data);
                }

                for (int index = 0; index < node.children.Count; index++)
                    explorationQueue.Enqueue(node.children[index]);

                allOfQueueSize--;
            }

            depth++;
            if (depth > _depth)
                break;
        }

        return result;
    }
}