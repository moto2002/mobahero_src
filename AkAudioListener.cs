using System;
using UnityEngine;

[AddComponentMenu("Wwise/AkAudioListener")]
public class AkAudioListener : MonoBehaviour
{
	public int listenerId;

	private Vector3 m_Position;

	private Vector3 m_Top;

	private Vector3 m_Front;

	private void Update()
	{
		if (this.m_Position == base.transform.position && this.m_Front == base.transform.forward && this.m_Top == base.transform.up)
		{
			return;
		}
		this.m_Position = base.transform.position;
		this.m_Front = base.transform.forward;
		this.m_Top = base.transform.up;
		AkSoundEngine.SetListenerPosition(0f, 0f, 0f, 0f, 0f, 0f, base.transform.position.x, base.transform.position.y, base.transform.position.z, (uint)this.listenerId);
	}
}
