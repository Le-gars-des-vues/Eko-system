using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : IHeapItem<PathNode>
{
    public bool isWalkable;
    public Vector2 worldPos;
    public int gridPosX;
    public int gridPosY;
    public bool isCloseToGround;
    public int movementPenalty;
    public int airPenalty;

    public int gCost;
    public int hCost;
    public PathNode cameFromNode;
    int heapIndex;

    public PathNode(bool _isWalkable, Vector2 _worldPos, int _gridPosX, int _gridPosY, bool _closeToGRound, int _penalty, int __airPenalty)
    {
        isWalkable = _isWalkable;
        worldPos = _worldPos;
        gridPosX = _gridPosX;
        gridPosY = _gridPosY;
        isCloseToGround = _closeToGRound;
        movementPenalty = _penalty;
        airPenalty = __airPenalty;
    }

    public int fCost
    {
        get { return hCost + gCost; }
    }

    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int CompareTo(PathNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}