using System;
using System.Collections.Generic;
using UnityEngine;

public class iTweenPath : MonoBehaviour
{
	public string pathName = string.Empty;

	public Color pathColor = Color.cyan;

	public List<Vector3> nodes = new List<Vector3>
	{
		Vector3.zero,
		Vector3.zero
	};

	public int nodeCount;

	public static Dictionary<string, iTweenPath> paths = new Dictionary<string, iTweenPath>();

	public bool initialized;

	public string initialName = string.Empty;

	private void OnEnable()
	{
		if (!iTweenPath.paths.ContainsKey(this.pathName))
		{
			iTweenPath.paths.Add(this.pathName.ToLower(), this);
		}
	}

	private void OnDisable()
	{
		iTweenPath.paths.Remove(this.pathName.ToLower());
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled && this.nodes.Count > 0)
		{
			iTween.DrawPath(this.nodes.ToArray(), this.pathColor);
		}
	}

	public static Vector3[] GetPath(string requestedName)
	{
		requestedName = requestedName.ToLower();
		if (iTweenPath.paths.ContainsKey(requestedName))
		{
			return iTweenPath.paths[requestedName].nodes.ToArray();
		}
		Debug.Log("No path with that name exists! Are you sure you wrote it correctly?");
		return null;
	}
}
