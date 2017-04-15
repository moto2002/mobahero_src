using System;
using UnityEngine;

public class fogsys_art : MonoBehaviour
{
	private FOWSystem instance;

	private void Start()
	{
		this.instance = base.GetComponent<FOWSystem>();
		this.instance.CreateInstance();
		this.instance.enableFog(true);
		this.instance.Init();
		FOWEffect fOWEffect = this.instance.BindCam(Camera.main.camera);
		fOWEffect.manualstart();
		this.instance.DoStart("map16");
	}

	private void Update()
	{
		if (this.instance != null)
		{
			this.instance.DoUpdate();
		}
	}
}
