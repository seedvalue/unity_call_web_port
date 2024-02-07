using System.Collections.Generic;
using UnityEngine;

public class WaypointGroup : MonoBehaviour
{
	[Tooltip("True if waypoints in this group and lines between them should be drawn in editor.")]
	public bool drawWaypoints = true;

	[HideInInspector]
	public List<Transform> wayPoints = new List<Transform>();

	private Transform myTransform;

	private void Start()
	{
		myTransform = base.transform;
		wayPoints.Clear();
		for (int i = 0; i < myTransform.childCount; i++)
		{
			wayPoints.Add(myTransform.GetChild(i));
		}
	}

	private void OnDrawGizmos()
	{
		if (!drawWaypoints)
		{
			return;
		}
		myTransform = base.transform;
		wayPoints.Clear();
		for (int i = 0; i < myTransform.childCount; i++)
		{
			wayPoints.Add(myTransform.GetChild(i));
		}
		if (wayPoints == null || wayPoints.Count == 0)
		{
			return;
		}
		for (int j = 0; j < wayPoints.Count; j++)
		{
			if (!(wayPoints[j] != null))
			{
				continue;
			}
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(wayPoints[j].position, 0.3f);
			if (j != wayPoints.Count - 1 && wayPoints[j + 1] != null && wayPoints.Count > 1)
			{
				if (Physics.Linecast(wayPoints[j].position, wayPoints[j + 1].position))
				{
					Gizmos.color = Color.red;
					Gizmos.DrawLine(wayPoints[j].position, wayPoints[j + 1].position);
				}
				else
				{
					Gizmos.color = Color.green;
					Gizmos.DrawLine(wayPoints[j].position, wayPoints[j + 1].position);
				}
			}
		}
	}
}
