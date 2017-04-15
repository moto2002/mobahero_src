using System;
using UnityEngine;

public class shadowrevealer : MonoBehaviour
{
	public FOWRevealer m_fowrevealer;

	public float fog_range = 5f;

	public bool isstatic;

	private void Awake()
	{
		if (this.m_fowrevealer == null)
		{
			this.m_fowrevealer = new FOWRevealer();
			if (this.isstatic)
			{
				this.m_fowrevealer.Create(base.transform, this.fog_range, FOWSystem.LOSChecks.Static, false);
			}
			else
			{
				this.m_fowrevealer.Create(base.transform, this.fog_range, FOWSystem.LOSChecks.EveryUpdate, false);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.m_fowrevealer != null)
		{
			this.m_fowrevealer.DoUpdate();
		}
		if (Input.GetKeyDown(KeyCode.C) && this.m_fowrevealer != null)
		{
			this.m_fowrevealer.DoDestroy();
		}
	}

	private void Destroy()
	{
		if (this.m_fowrevealer != null)
		{
			this.m_fowrevealer.DoDestroy();
		}
	}
}
