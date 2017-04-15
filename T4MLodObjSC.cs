using System;
using UnityEngine;

public class T4MLodObjSC : MonoBehaviour
{
	[HideInInspector]
	public Renderer LOD1;

	[HideInInspector]
	public Renderer LOD2;

	[HideInInspector]
	public Renderer LOD3;

	[HideInInspector]
	public float Interval = 0.5f;

	[HideInInspector]
	public Transform PlayerCamera;

	[HideInInspector]
	public int Mode;

	private Vector3 OldPlayerPos;

	[HideInInspector]
	public int ObjLodStatus;

	[HideInInspector]
	public float MaxViewDistance = 60f;

	[HideInInspector]
	public float LOD2Start = 20f;

	[HideInInspector]
	public float LOD3Start = 40f;

	public void ActivateLODScrpt()
	{
		if (this.Mode != 2)
		{
			return;
		}
		if (this.PlayerCamera == null)
		{
			this.PlayerCamera = Camera.main.transform;
		}
		base.InvokeRepeating("AFLODScrpt", UnityEngine.Random.Range(0f, this.Interval), this.Interval);
	}

	public void ActivateLODLay()
	{
		if (this.Mode != 2)
		{
			return;
		}
		if (this.PlayerCamera == null)
		{
			this.PlayerCamera = Camera.main.transform;
		}
		base.InvokeRepeating("AFLODLay", UnityEngine.Random.Range(0f, this.Interval), this.Interval);
	}

	public void AFLODLay()
	{
		if (this.OldPlayerPos == this.PlayerCamera.position)
		{
			return;
		}
		this.OldPlayerPos = this.PlayerCamera.position;
		float num = Vector3.Distance(new Vector3(base.transform.position.x, this.PlayerCamera.position.y, base.transform.position.z), this.PlayerCamera.position);
		int layer = base.gameObject.layer;
		if (num <= this.PlayerCamera.camera.layerCullDistances[layer] + 5f)
		{
			if (num < this.LOD2Start && this.ObjLodStatus != 1)
			{
				Renderer arg_D7_0 = this.LOD3;
				bool enabled = false;
				this.LOD2.enabled = enabled;
				arg_D7_0.enabled = enabled;
				this.LOD1.enabled = true;
				this.ObjLodStatus = 1;
			}
			else if (num >= this.LOD2Start && num < this.LOD3Start && this.ObjLodStatus != 2)
			{
				Renderer arg_130_0 = this.LOD1;
				bool enabled = false;
				this.LOD3.enabled = enabled;
				arg_130_0.enabled = enabled;
				this.LOD2.enabled = true;
				this.ObjLodStatus = 2;
			}
			else if (num >= this.LOD3Start && this.ObjLodStatus != 3)
			{
				Renderer arg_17D_0 = this.LOD1;
				bool enabled = false;
				this.LOD2.enabled = enabled;
				arg_17D_0.enabled = enabled;
				this.LOD3.enabled = true;
				this.ObjLodStatus = 3;
			}
		}
	}

	public void AFLODScrpt()
	{
		if (this.OldPlayerPos == this.PlayerCamera.position)
		{
			return;
		}
		this.OldPlayerPos = this.PlayerCamera.position;
		float num = Vector3.Distance(new Vector3(base.transform.position.x, this.PlayerCamera.position.y, base.transform.position.z), this.PlayerCamera.position);
		if (num <= this.MaxViewDistance)
		{
			if (num < this.LOD2Start && this.ObjLodStatus != 1)
			{
				Renderer arg_B8_0 = this.LOD3;
				bool flag = false;
				this.LOD2.enabled = flag;
				arg_B8_0.enabled = flag;
				this.LOD1.enabled = true;
				this.ObjLodStatus = 1;
			}
			else if (num >= this.LOD2Start && num < this.LOD3Start && this.ObjLodStatus != 2)
			{
				Renderer arg_111_0 = this.LOD1;
				bool flag = false;
				this.LOD3.enabled = flag;
				arg_111_0.enabled = flag;
				this.LOD2.enabled = true;
				this.ObjLodStatus = 2;
			}
			else if (num >= this.LOD3Start && this.ObjLodStatus != 3)
			{
				Renderer arg_15E_0 = this.LOD1;
				bool flag = false;
				this.LOD2.enabled = flag;
				arg_15E_0.enabled = flag;
				this.LOD3.enabled = true;
				this.ObjLodStatus = 3;
			}
		}
		else if (this.ObjLodStatus != 0)
		{
			Renderer arg_1AF_0 = this.LOD1;
			bool flag = false;
			this.LOD3.enabled = flag;
			flag = flag;
			this.LOD2.enabled = flag;
			arg_1AF_0.enabled = flag;
			this.ObjLodStatus = 0;
		}
	}
}
