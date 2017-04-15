using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class DelayedVoicePlayer : VoicePlayer
{
	protected override void Start()
	{
		base.Start();
		base.name = "DELAYED_VOICE_PLAYER";
	}

	public void play(AudioClip clip, float vol)
	{
		base.setInitVol(vol);
		float t = (!(clip == null)) ? clip.length : 0f;
		if (AudioMgr.Instance != null)
		{
			if (clip != null)
			{
				base.audioSource.clip = clip;
				base.audioSource.volume = ((!AudioMgr.Instance.muteSound) ? vol : 0f);
				base.audioSource.Play();
			}
		}
		else
		{
			base.audioSource.clip = clip;
			base.audioSource.Play();
		}
		UnityEngine.Object.Destroy(base.gameObject, t);
	}

	public void play(AudioClip clip, float vol, float delay)
	{
		base.StartCoroutine(this.play2(clip, vol, delay));
	}

	[DebuggerHidden]
	private IEnumerator play2(AudioClip clip, float vol, float delay)
	{
		DelayedVoicePlayer.<play2>c__Iterator16 <play2>c__Iterator = new DelayedVoicePlayer.<play2>c__Iterator16();
		<play2>c__Iterator.delay = delay;
		<play2>c__Iterator.clip = clip;
		<play2>c__Iterator.vol = vol;
		<play2>c__Iterator.<$>delay = delay;
		<play2>c__Iterator.<$>clip = clip;
		<play2>c__Iterator.<$>vol = vol;
		<play2>c__Iterator.<>f__this = this;
		return <play2>c__Iterator;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		AudioClip clip = base.audioSource.clip;
		if (clip == null)
		{
			return;
		}
		base.audioSource.clip = null;
		Resources.UnloadAsset(clip);
	}

	public static void genPlayer(Vector3 pos_w, AudioClip clip, float value)
	{
		if (clip == null)
		{
			return;
		}
		DelayedVoicePlayer delayedVoicePlayer = new GameObject
		{
			transform = 
			{
				position = pos_w
			}
		}.AddComponent<DelayedVoicePlayer>();
		delayedVoicePlayer.play(clip, value);
	}
}
