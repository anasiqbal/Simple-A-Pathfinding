using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	public Transform target;
	public float speed = 5;
	Vector3[] path;

	public float lookDuration = 1f;
	float startTime;

	int targetIndex;

	Vector3 initDirection;

	void Start()
	{
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
	}

	void OnDrawGizmos()
	{
		if(path != null)
		{
			for(int i = targetIndex; i < path.Length; i++)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path [i], Vector3.one);

				if(i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path [i]);
				}
				else
				{
					Gizmos.DrawLine(path [i - 1], path [i]);
				}
			}
		}
	}

	public void OnPathFound(Vector3[] _path, bool _success)
	{
		if(_success)
		{
			path = _path;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path [0];
		initDirection = transform.forward;
		Vector3 targetDirection = (currentWaypoint - transform.position).normalized;

		startTime = Time.time;

		while(true)
		{
			if(transform.position == currentWaypoint)
			{
				targetIndex++;
				if(targetIndex >= path.Length)
				{
					yield break;
				}

				currentWaypoint = path [targetIndex];
				initDirection = transform.forward;
				targetDirection = (currentWaypoint - transform.position).normalized;
				startTime = Time.time;
			}

			transform.forward = Vector3.Lerp(initDirection, targetDirection, (Time.time - startTime) / lookDuration);
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;
		}
	}
}
