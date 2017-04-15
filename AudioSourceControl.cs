using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class AudioSourceControl : MonoBehaviour
{
	public AudioSource audioSource;

	public GameObject audioSourceObject;

	public bool isPlaying;

	private bool _isPaused;

	private Transform m_targetTrans;

	private Transform m_audioSourceTrans;

	private float m_timer;

	private void Awake()
	{
		this.m_audioSourceTrans = this.audioSourceObject.transform;
	}

	public void stopPlaying()
	{
		if (this.isPlaying)
		{
			base.StopAllCoroutines();
			base.enabled = true;
			this.audioSource.enabled = true;
			this.audioSource.Stop();
			this.audioSource.clip = null;
			this.m_timer = 0f;
			this.isPlaying = false;
			this.audioSourceObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
		if (this.isPlaying)
		{
			this.stopPlaying();
		}
	}

	private void OnDestroy()
	{
		this.audioSourceObject = null;
	}

	public void Clear()
	{
		base.StopAllCoroutines();
		this.audioSource.clip = null;
		this.audioSourceObject.SetActive(true);
		this.isPlaying = false;
		UnityEngine.Object.DestroyImmediate(this.audioSourceObject);
	}

	private void Update()
	{
		if (this._isPaused)
		{
			return;
		}
		if (this.m_timer > 0f)
		{
			this.m_timer -= Time.unscaledDeltaTime;
			if (this.m_timer <= 0f)
			{
				this.stopPlaying();
			}
		}
		if (this.m_targetTrans != null)
		{
			this.m_audioSourceTrans.position = this.m_targetTrans.position;
		}
	}

	public void Mute(bool isMute)
	{
		base.enabled = true;
		this.audioSource.enabled = true;
		this.audioSource.mute = isMute;
	}

	public void Play(AudioClip clip, Transform target, float volume = 0.3f)
	{
		this.isPlaying = true;
		this._isPaused = false;
		this.m_targetTrans = target;
		volume = Mathf.Clamp01(volume);
		this.audioSource.volume = volume;
		this.audioSource.clip = clip;
		base.enabled = true;
		this.audioSource.enabled = true;
		this.audioSourceObject.SetActive(true);
		if (!this.audioSource.loop)
		{
			this.m_timer = clip.length;
		}
	}

	public void Pause(bool isPause)
	{
		base.enabled = true;
		this.audioSource.enabled = true;
		if (isPause)
		{
			this.audioSource.Pause();
			this._isPaused = true;
		}
		else if (this.audioSourceObject.activeInHierarchy)
		{
			this.audioSource.Play();
			this._isPaused = false;
		}
	}

	public void FadeOut(float speed = 1f)
	{
		base.enabled = true;
		this.audioSource.enabled = true;
		base.StartCoroutine(this.FadeOut_Coroutine(speed));
	}

	[DebuggerHidden]
	private IEnumerator FadeOut_Coroutine(float speed)
	{
		AudioSourceControl.<FadeOut_Coroutine>c__Iterator14 <FadeOut_Coroutine>c__Iterator = new AudioSourceControl.<FadeOut_Coroutine>c__Iterator14();
		<FadeOut_Coroutine>c__Iterator.speed = speed;
		<FadeOut_Coroutine>c__Iterator.<$>speed = speed;
		<FadeOut_Coroutine>c__Iterator.<>f__this = this;
		return <FadeOut_Coroutine>c__Iterator;
	}

	public void FadeIn(AudioClip clip, Transform target, float speed, float targetVolume)
	{
		base.enabled = true;
		this.audioSource.enabled = true;
		this.Play(clip, target, 0f);
		base.StartCoroutine(this.FadeIn_Coroutine(speed, targetVolume));
	}

	[DebuggerHidden]
	private IEnumerator FadeIn_Coroutine(float speed, float targetVolume)
	{
		AudioSourceControl.<FadeIn_Coroutine>c__Iterator15 <FadeIn_Coroutine>c__Iterator = new AudioSourceControl.<FadeIn_Coroutine>c__Iterator15();
		<FadeIn_Coroutine>c__Iterator.speed = speed;
		<FadeIn_Coroutine>c__Iterator.targetVolume = targetVolume;
		<FadeIn_Coroutine>c__Iterator.<$>speed = speed;
		<FadeIn_Coroutine>c__Iterator.<$>targetVolume = targetVolume;
		<FadeIn_Coroutine>c__Iterator.<>f__this = this;
		return <FadeIn_Coroutine>c__Iterator;
	}
}
