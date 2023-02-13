using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapData
{
    public Vector2Int dungeonSize;
    public int maxNode;

    public float minDivideSize;
    public float maxDivideSize;

    public int minDungeonSize;
}

public class BSPMapGenerator
{
    public BSPMapGenerator(MapData _mapData)
    {
        //Todo Default Map Generate
        var roomNode = new RoomNode(0, 0, _mapData.dungeonSize.x, _mapData.dungeonSize.y);

        //트리를 분할하여 방을 만드는 과정
        DivideBinaryTree(roomNode, 0, _mapData);
        GenerateDungeon(roomNode, 0, _mapData);
    }

    private RectInt GenerateDungeon(RoomNode _root, int _n, MapData _mapData)
    {
        var debugger = MapDebugger.Instance;
        if (_n == _mapData.maxNode)
        {
            RectInt size = _root.treeSize;
            int width = Mathf.Max(UnityEngine.Mathf.Max(size.width / 2, size.width - 1));
            int height = Mathf.Max(UnityEngine.Mathf.Max(size.height / 2, size.height - 1));

            int x = _root.treeSize.x + UnityEngine.Random.Range(1, size.width - width);
            int y = _root.treeSize.y + UnityEngine.Random.Range(1, size.height - height);

            var rect = new RectInt(x, y, width, height);
            debugger.AddRect(rect);
            return rect;
        }

        ((RoomNode)_root.leftLeaf).roomSize = GenerateDungeon(_root.leftLeaf as RoomNode, _n + 1, _mapData);
        ((RoomNode)_root.rightLeaf).roomSize = GenerateDungeon(_root.rightLeaf as RoomNode, _n + 1, _mapData);

        return ((RoomNode)_root.rightLeaf).roomSize;
    }

    private void DivideBinaryTree(RoomNode _rootNode, int _n, MapData _mapData)
    {
        if(_n < _mapData.maxNode) // 노드의 최댓값이 될 때 까지 반복
        {
            var debugger = MapDebugger.Instance;

            RectInt size = _rootNode.treeSize;
            int length = size.width >= size.height ? size.width : size.height; // 트리의 기준선 작업, 세로 또는 가로 중 긴 축을 사용
            int split = Mathf.RoundToInt(UnityEngine.Random.Range(length * _mapData.minDivideSize, length * _mapData.maxDivideSize));

            //가로 선
            if (size.width >= size.height)
            {
                //기준선을 반으로 나눠 split을 가로 길이로, 이전 트리의 height 값을 세로 길이로 사용
                _rootNode.leftLeaf = new RoomNode(size.x, size.y, split, size.height);

                //x 값에 split값을 더하여 좌표를 설정, 이전 트리의 width값에 split값을 빼 가로 길이를 설정
                _rootNode.rightLeaf = new RoomNode(size.x + split, size.y, size.width - split, size.height);

                debugger.AddLine(new DebugLine(new Vector2Int(size.x + split, size.y), new Vector2Int(size.x + split, size.y + size.height)));
            }
            //세로 선
            else
            {
                _rootNode.leftLeaf = new RoomNode(size.x, size.y, size.width, split);
                _rootNode.rightLeaf = new RoomNode(size.x, size.y + split, size.width, size.height - split);
                debugger.AddLine(new DebugLine(new Vector2Int(size.x, size.y + split), new Vector2Int(size.x + size.width, size.y + split)));
            }

            //분할된 트리의 부모 트리를 매개변수로 받아온 노드로 할당
            _rootNode.leftLeaf.parent = _rootNode;
            _rootNode.rightLeaf.parent = _rootNode;

            //재귀 호출로 _mapData의 maxNode값까지 트리 분할
            DivideBinaryTree(_rootNode.leftLeaf as RoomNode, _n + 1, _mapData);
            DivideBinaryTree(_rootNode.rightLeaf as RoomNode, _n + 1, _mapData);
        }
    }
}