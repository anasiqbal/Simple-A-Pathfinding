using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class PathFinding : MonoBehaviour {

	#region Member Variables
	PathRequestManager requestManager;
	Grid grid;

	#endregion

	#region Unity Methods
	void Awake()
	{
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}

	#endregion

	#region Helper Functions
	public void StartFindPath(Vector3 _startPos, Vector3 _targetPos)
	{
		StartCoroutine(FindPath(_startPos, _targetPos));
	}

	/* Path Finding Algorithm
	 * 
	 * openSet (set of nodes to be evaluated)
	 * closeSet (set of nodes already evaluated)
	 * add start node to openSet
	 * 
	 * loop
	 * 	currentNode = node in openSet with lowest f_Cost
	 * 	remove currentNode from openSet
	 * 	add currentNode to closeSet
	 * 	
	 * 	if currentNode is the targetNode (path has been found)
	 * 		return
	 * 	foreach neighbour of currentNode
	 * 		if neighbour is not traversable or neighbour is in closeSet
	 * 			skip to next neighbour
	 * 		if new path to neighbour is shorter OR neighbour is not in openSet
	 * 			set f_Cost of neighbour
	 * 			set parent of neighbour to currentNode
	 * 			if neighbour is not in openSet
	 * 				add neighbour to openSet
	*/
	IEnumerator FindPath(Vector3 _startPos, Vector3 _targetPos)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();

		Vector3[] wayPoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint(_startPos);
		Node targetNode = grid.NodeFromWorldPoint(_targetPos);

		if(startNode.walkable && targetNode.walkable)
		{
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closeSet = new HashSet<Node>();

			openSet.Add(startNode);

			while (openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();
				closeSet.Add(currentNode);

				if (currentNode == targetNode)
				{
					sw.Stop();
					print("Path found: " + sw.ElapsedMilliseconds + " ms");
					pathSuccess = true;
					break;
				}

				List<Node> neighbours = grid.GetNeighbours(currentNode);
				foreach(Node neighbour in neighbours)
				{
					if (!neighbour.walkable || closeSet.Contains(neighbour))
						continue;

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
					if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
						else
							openSet.UpdateItem(neighbour);
					}
				}
			}
		}
		yield return null;
		if(pathSuccess)
		{
			wayPoints = RetracePath(startNode, targetNode);
		}
		requestManager.FinnishedProcessingPath(wayPoints, pathSuccess);
	}

	Vector3[] RetracePath(Node _startNode, Node _endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = _endNode;

		while(currentNode != _startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);

		return waypoints;
	}

	Vector3[] SimplifyPath(List<Node> _path)
	{
		List<Vector3> wayPoints = new List<Vector3>();
		Vector2 oldDirection = Vector2.zero;

		for(int i = 1; i < _path.Count; i++)
		{
			Vector2 newDirection = new Vector2(_path [i - 1].gridX - _path [i].gridX, _path [i - 1].gridY - _path [i].gridY);
			if(newDirection != oldDirection)
			{
				wayPoints.Add(_path [i].worldPosition);
			}

			oldDirection = newDirection;
		}

		return wayPoints.ToArray();
	}

	int GetDistance(Node _nodeA, Node _nodeB)
	{
		int dstX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
		int dstY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);

		if(dstX > dstY)
			return (14 * dstY) + 10 * (dstX - dstY);

		return (14 * dstX) + 10 * (dstY - dstX);
	}

	#endregion
}
