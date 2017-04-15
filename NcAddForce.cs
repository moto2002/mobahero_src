using System;
using UnityEngine;

public class NcAddForce : NcEffectBehaviour
{
	public Vector3 m_AddForce = new Vector3(0f, 300f, 0f);

	public Vector3 m_RandomRange = new Vector3(100f, 100f, 100f);

	public ForceMode m_ForceMode;

	private void Start()
	{
		if (!base.enabled)
		{
			return;
		}
		this.AddForce();
	}

	private void AddForce()
	{
		if (base.rigidbody != null)
		{
			Vector3 force = new Vector3(UnityEngine.Random.Range(-this.m_RandomRange.x, this.m_RandomRange.x) + this.m_AddForce.x, UnityEngine.Random.Range(-this.m_RandomRange.y, this.m_RandomRange.y) + this.m_AddForce.y, UnityEngine.Random.Range(-this.m_RandomRange.z, this.m_RandomRange.z) + this.m_AddForce.z);
			base.rigidbody.AddForce(force, this.m_ForceMode);
		}
	}
}
