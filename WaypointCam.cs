using System;
using UnityEngine;

public class WaypointCam : MonoBehaviour
{
	public Color WaypointsColor = new Color(1f, 0f, 0f, 1f);

	public bool draw = true;

	public static Transform[] waypoints;

	private void Awake()
	{
		WaypointCam.waypoints = base.gameObject.GetComponentsInChildren<Transform>();
	}

	private void OnDrawGizmos()
	{
		if (this.draw)
		{
			WaypointCam.waypoints = base.gameObject.GetComponentsInChildren<Transform>();
			Transform[] array = WaypointCam.waypoints;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				Gizmos.color = this.WaypointsColor;
				Gizmos.DrawSphere(transform.position, 1f);
				Gizmos.color = this.WaypointsColor;
				Gizmos.DrawWireSphere(transform.position, 6f);
			}
		}
	}
}
