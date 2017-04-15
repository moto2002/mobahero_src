using System;
using System.Collections.Generic;
using UnityEngine;

public class NsSharedManager : MonoBehaviour
{
	protected static NsSharedManager _inst;

	protected List<GameObject> m_SharedPrefabs = new List<GameObject>();

	protected List<GameObject> m_SharedGameObjects = new List<GameObject>();

	protected List<AudioClip> m_SharedAudioClip = new List<AudioClip>();

	protected List<List<AudioSource>> m_SharedAudioSources = new List<List<AudioSource>>();

	public static NsSharedManager inst
	{
		get
		{
			if (NsSharedManager._inst == null)
			{
				NsSharedManager._inst = NcEffectBehaviour.GetRootInstanceEffect().AddComponent<NsSharedManager>();
			}
			return NsSharedManager._inst;
		}
	}

	public GameObject GetSharedParticleGameObject(GameObject originalParticlePrefab)
	{
		int num = this.m_SharedPrefabs.IndexOf(originalParticlePrefab);
		if (num >= 0 && !(this.m_SharedGameObjects[num] == null))
		{
			return this.m_SharedGameObjects[num];
		}
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(originalParticlePrefab);
		gameObject.transform.parent = NcEffectBehaviour.GetRootInstanceEffect().transform;
		if (0 <= num)
		{
			this.m_SharedGameObjects[num] = gameObject;
		}
		else
		{
			this.m_SharedPrefabs.Add(originalParticlePrefab);
			this.m_SharedGameObjects.Add(gameObject);
		}
		NcParticleSystem component = gameObject.GetComponent<NcParticleSystem>();
		if (component)
		{
			component.enabled = false;
		}
		if (gameObject.particleEmitter)
		{
			gameObject.particleEmitter.emit = false;
			gameObject.particleEmitter.useWorldSpace = true;
			ParticleAnimator component2 = gameObject.GetComponent<ParticleAnimator>();
			if (component2)
			{
				component2.autodestruct = false;
			}
		}
		NcParticleSystem component3 = gameObject.GetComponent<NcParticleSystem>();
		if (component3)
		{
			component3.m_bBurst = false;
		}
		ParticleSystem component4 = gameObject.GetComponent<ParticleSystem>();
		if (component4)
		{
			component4.enableEmission = false;
		}
		return gameObject;
	}

	public void EmitSharedParticleSystem(GameObject originalParticlePrefab, int nEmitCount, Vector3 worldPos)
	{
		GameObject sharedParticleGameObject = this.GetSharedParticleGameObject(originalParticlePrefab);
		if (sharedParticleGameObject == null)
		{
			return;
		}
		sharedParticleGameObject.transform.position = worldPos;
		if (sharedParticleGameObject.particleEmitter != null)
		{
			sharedParticleGameObject.particleEmitter.Emit(nEmitCount);
		}
		else
		{
			ParticleSystem component = sharedParticleGameObject.GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Emit(nEmitCount);
			}
		}
	}

	public AudioSource GetSharedAudioSource(AudioClip audioClip, int nPriority, bool bLoop, float fVolume, float fPitch)
	{
		int num = this.m_SharedAudioClip.IndexOf(audioClip);
		if (num >= 0)
		{
			foreach (AudioSource current in this.m_SharedAudioSources[num])
			{
				if (current.volume == fVolume && current.pitch == fPitch && current.loop == bLoop && current.priority == nPriority)
				{
					return current;
				}
			}
			return this.AddAudioSource(this.m_SharedAudioSources[num], audioClip, nPriority, bLoop, fVolume, fPitch);
		}
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		List<AudioSource> list = new List<AudioSource>();
		this.m_SharedAudioClip.Add(audioClip);
		this.m_SharedAudioSources.Add(list);
		return this.AddAudioSource(list, audioClip, nPriority, bLoop, fVolume, fPitch);
	}

	private AudioSource AddAudioSource(List<AudioSource> sourceList, AudioClip audioClip, int nPriority, bool bLoop, float fVolume, float fPitch)
	{
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		sourceList.Add(audioSource);
		audioSource.clip = audioClip;
		audioSource.priority = nPriority;
		audioSource.loop = bLoop;
		audioSource.volume = fVolume;
		audioSource.pitch = fPitch;
		audioSource.playOnAwake = false;
		return audioSource;
	}

	public void PlaySharedAudioSource(bool bUniquePlay, AudioClip audioClip, int nPriority, bool bLoop, float fVolume, float fPitch)
	{
		AudioSource sharedAudioSource = this.GetSharedAudioSource(audioClip, nPriority, bLoop, fVolume, fPitch);
		if (sharedAudioSource == null)
		{
			return;
		}
		if (sharedAudioSource.isPlaying)
		{
			if (bUniquePlay)
			{
				return;
			}
			sharedAudioSource.Stop();
		}
		sharedAudioSource.Play();
	}
}
