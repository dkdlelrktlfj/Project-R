using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode
{
    public TreeNode parent;
    public TreeNode leftLeaf;
    public TreeNode rightLeaf;

    public TreeNode()
    {

    }
}

public class RoomNode : TreeNode
{
    public RectInt treeSize;
    public RectInt roomSize;

    public RoomNode(int _x, int _y, int _width, int _height)
    {
        this.treeSize.x = _x;
        this.treeSize.y = _y;
        this.treeSize.width = _width;
        this.treeSize.height = _height;
    }
}