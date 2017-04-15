using System;
using UnityEngine;

public class NcAttachSound : NcEffectBehaviour
{
	public enum PLAY_TYPE
	{
		StopAndPlay,
		UniquePlay,
		MultiPlay
	}

	public NcAttachSound.PLAY_TYPE m_PlayType;

	public bool m_bSharedAudioSource = true;

	public bool m_bPlayOnActive;

	public float m_fDelayTime;

	public float m_fRepeatTime;

	public int m_nRepeatCount;

	public AudioClip m_AudioClip;

	public int m_nPriority = 128;

	public bool m_bLoop;

	public float m_fVolume = 1f;

	public float m_fPitch = 1f;

	protected AudioSource m_AudioSource;

	protected float m_fStartTime;

	protected int m_nCreateCount;

	protected bool m_bStartAttach;

	protected bool m_bEnable = true;

	public override int GetAnimationState()
	{
		if ((base.enabled && NcEffectBehaviour.IsActive(base.gameObject)) || (this.m_AudioSource != null && (this.m_AudioSource.isPlaying || NcEffectBehaviour.GetEngineTime() < this.m_fStartTime + this.m_fDelayTime)))
		{
			return 1;
		}
		return 0;
	}

	public void Replay()
	{
		this.m_bStartAttach = false;
		this.m_bEnable = true;
		base.enabled = true;
		this.m_nCreateCount = 0;
	}

	private void OnEnable()
	{
		if (this.m_bPlayOnActive)
		{
			this.Replay();
		}
	}

	private void Update()
	{
		if (this.m_AudioClip == null)
		{
			base.enabled = false;
			return;
		}
		if (!this.m_bEnable)
		{
			return;
		}
		if (!this.m_bStartAttach)
		{
			this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
			this.m_bStartAttach = true;
		}
		if (this.m_fStartTime + ((this.m_nCreateCount != 0) ? this.m_fRepeatTime : this.m_fDelayTime) <= NcEffectBehaviour.GetEngineTime())
		{
			this.CreateAttachSound();
			if (0f < this.m_fRepeatTime && (this.m_nRepeatCount == 0 || this.m_nCreateCount < this.m_nRepeatCount))
			{
				this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
			}
			else
			{
				this.m_bEnable = false;
			}
		}
	}

	public void CreateAttachSound()
	{
		if (this.m_PlayType == NcAttachSound.PLAY_TYPE.MultiPlay || !this.m_bSharedAudioSource)
		{
			if (this.m_AudioSource == null)
			{
				this.m_AudioSource = base.gameObject.AddComponent<AudioSource>();
			}
			this.m_AudioSource.clip = this.m_AudioClip;
			this.m_AudioSource.priority = this.m_nPriority;
			this.m_AudioSource.loop = this.m_bLoop;
			this.m_AudioSource.volume = this.m_fVolume;
			this.m_AudioSource.pitch = this.m_fPitch;
			this.m_AudioSource.playOnAwake = false;
			this.m_AudioSource.Play();
		}
		else
		{
			NsSharedManager.inst.PlaySharedAudioSource(this.m_PlayType == NcAttachSound.PLAY_TYPE.UniquePlay, this.m_AudioClip, this.m_nPriority, this.m_bLoop, this.m_fVolume, this.m_fPitch);
		}
		this.m_nCreateCount++;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime /= fSpeedRate;
		this.m_fRepeatTime /= fSpeedRate;
	}

	public override void OnSetReplayState()
	{
		base.OnSetReplayState();
	}

	public override void OnResetReplayStage(bool bClearOldParticle)
	{
		base.OnResetReplayStage(bClearOldParticle);
		this.Replay();
	}
}
