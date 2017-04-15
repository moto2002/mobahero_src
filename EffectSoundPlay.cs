using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class EffectSoundPlay : MonoBehaviour
{
	public bool screenDecay = true;

	public AudioClip sound;

	public bool isLoop;

	public float delay;

	public float exitTime;

	public float volume = 50f;

	private Transform mTransform;

	private AudioSourceControl m_audioSourceControl;

	[DebuggerHidden]
	private IEnumerator PlaySound()
	{
		EffectSoundPlay.<PlaySound>c__Iterator92 <PlaySound>c__Iterator = new EffectSoundPlay.<PlaySound>c__Iterator92();
		<PlaySound>c__Iterator.<>f__this = this;
		return <PlaySound>c__Iterator;
	}

	public void OnSpawned()
	{
		if (this.sound == null)
		{
			return;
		}
		if (GlobalObject.Instance != null)
		{
			GlobalObject.Instance.StopCoroutine(this.PlaySound());
			GlobalObject.Instance.StartCoroutine(this.PlaySound());
		}
	}

	public void OnDespawned()
	{
		if (this.m_audioSourceControl != null)
		{
			this.m_audioSourceControl.FadeOut(2f);
			this.m_audioSourceControl = null;
		}
	}

	private void Awake()
	{
		this.mTransform = base.transform;
	}

	private void OnDestroy()
	{
		this.OnDespawned();
	}
}
