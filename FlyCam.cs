using System;
using UnityEngine;

public class FlyCam : MonoBehaviour
{
	private int currentWaypoint;

	public float rotateSpeed = 1f;

	public float moveSpeed = 10f;

	public float magnitudeMax = 10f;

	private void Update()
	{
		if (WaypointCam.waypoints.Length > 0)
		{
			Vector3 vector = base.transform.InverseTransformPoint(new Vector3(WaypointCam.waypoints[this.currentWaypoint].position.x, WaypointCam.waypoints[this.currentWaypoint].position.y, WaypointCam.waypoints[this.currentWaypoint].position.z));
			Vector3 a = new Vector3(WaypointCam.waypoints[this.currentWaypoint].position.x, WaypointCam.waypoints[this.currentWaypoint].position.y, WaypointCam.waypoints[this.currentWaypoint].position.z);
			Quaternion to = Quaternion.LookRotation(a - base.transform.position);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, to, Time.deltaTime * this.rotateSpeed);
			Vector3 a2 = base.transform.TransformDirection(Vector3.forward);
			base.transform.position += a2 * this.moveSpeed * Time.deltaTime;
			if (vector.magnitude < this.magnitudeMax)
			{
				this.currentWaypoint++;
				if (this.currentWaypoint >= WaypointCam.waypoints.Length)
				{
					this.currentWaypoint = 0;
				}
			}
		}
	}
}
