using System;
using UnityEngine;

public class MeshbendWindctrl : MonoBehaviour
{
	private Material mat;

	private Vector3 pos = Vector3.zero;

	private void Start()
	{
		this.pos = base.transform.position;
		this.mat = base.GetComponent<Renderer>().sharedMaterial;
	}

	private void Update()
	{
		if (this.mat != null)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = this.pos - position;
			float d = vector.magnitude / 2f;
			vector = vector.normalized * d;
			this.mat.SetVector("_Wind", new Vector4(vector.x, vector.y, vector.z, 1f));
			this.pos = position;
		}
	}
}
