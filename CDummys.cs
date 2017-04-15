using System;
using System.Collections.Generic;
using UnityEngine;

public class CDummys
{
	public List<Vector3> dummylist_pos = new List<Vector3>();

	public List<Quaternion> dummylist_rot = new List<Quaternion>();

	public void clear()
	{
		this.dummylist_pos.Clear();
		this.dummylist_rot.Clear();
	}

	public bool isEmpty()
	{
		return this.dummylist_pos.Count == 0;
	}
}
