﻿using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {

	public bool walkable;
	public Vector3 worldPosition;
	public int gridX, gridY;
	public int movementPenalty;

	public Node parent;

	public int gCost;
	public int hCost;
	public int fCost { get { return gCost + hCost; } }

	int heapIndex;

	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		movementPenalty = _penalty;
	}

	public int _HeapIndex {
		get { return heapIndex; }
		set { heapIndex = value; }
	}

	// we want to return ` if "to compare" is lower
	public int CompareTo(Node _nodeToCompare)
	{
		int compare = fCost.CompareTo(_nodeToCompare.fCost);
		if (compare == 0)
			compare = hCost.CompareTo(_nodeToCompare.hCost);

		return -compare;
	}
}
