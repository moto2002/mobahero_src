using System;
using UnityEngine;

public class NcRotation : NcEffectBehaviour
{
	public bool m_bWorldSpace;

	public Vector3 m_vRotationValue = new Vector3(0f, 360f, 0f);

	private void Update()
	{
		base.transform.Rotate(NcEffectBehaviour.GetEngineDeltaTime() * this.m_vRotationValue.x, NcEffectBehaviour.GetEngineDeltaTime() * this.m_vRotationValue.y, NcEffectBehaviour.GetEngineDeltaTime() * this.m_vRotationValue.z, (!this.m_bWorldSpace) ? Space.Self : Space.World);
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_vRotationValue *= fSpeedRate;
	}
}
