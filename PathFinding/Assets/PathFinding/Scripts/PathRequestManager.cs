using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathRequestManager : MonoBehaviour {

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;

	static PathRequestManager instance;
	PathFinding pathFinding;

	bool isProcessingPath;

	void Awake()
	{
		instance = this;
		pathFinding = GetComponent<PathFinding>();
	}

	public static void RequestPath(Vector3 _pathStart, Vector3 _pathEnd, Action<Vector3[], bool> _callback)
	{
		PathRequest newRequest = new PathRequest(_pathStart, _pathEnd, _callback);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}

	void TryProcessNext()
	{
		if(!isProcessingPath && pathRequestQueue.Count > 0)
		{
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinnishedProcessingPath(Vector3[] _path, bool _success)
	{
		currentPathRequest.callback(_path, _success);
		isProcessingPath = false;
		TryProcessNext();
	}

	struct PathRequest{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
		{
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}
	}
}
