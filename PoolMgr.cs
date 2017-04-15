using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgr : MonoBehaviour
{
	[SerializeField]
	private Transform m_audioRoot;

	private string m_audioPrefabPath;

	private Dictionary<eAudioSourceType, UnityEngine.Object> m_audioPrefabDict = new Dictionary<eAudioSourceType, UnityEngine.Object>();

	private Dictionary<eAudioSourceType, AudioPool> m_audioPoolDict = new Dictionary<eAudioSourceType, AudioPool>();

	private static PoolMgr m_instance;

	public static PoolMgr Instance
	{
		get
		{
			if (PoolMgr.m_instance == null)
			{
			}
			return PoolMgr.m_instance;
		}
	}

	public Transform AudioRoot
	{
		get
		{
			return this.m_audioRoot;
		}
	}

	private void Awake()
	{
		PoolMgr.m_instance = this;
		this.LoadAllAudioPrefab();
	}

	private void Start()
	{
		MobaMessageManager.RegistMessage((ClientMsg)25010, new MobaMessageFunc(this.FreeMemory));
	}

	private void FreeMemory(MobaMessage msg)
	{
		this.ReleaseAudioPools();
	}

	private void LoadAllAudioPrefab()
	{
		this.m_audioPrefabPath = "Prefab/Audio/";
		for (int i = 0; i <= 5; i++)
		{
			eAudioSourceType eAudioSourceType = (eAudioSourceType)i;
			if (!this.m_audioPrefabDict.ContainsKey(eAudioSourceType))
			{
				this.m_audioPrefabDict.Add(eAudioSourceType, Resources.Load(this.m_audioPrefabPath + eAudioSourceType.ToString()));
			}
			this.InitPoolOfAudioType(eAudioSourceType, 0);
		}
	}

	private void InitPoolOfAudioType(eAudioSourceType audioType, int audioCount)
	{
		if (!this.m_audioPoolDict.ContainsKey(audioType))
		{
			AudioPool audioPool = new AudioPool();
			audioPool.Init(this.m_audioPrefabDict[audioType], this.m_audioRoot, audioCount);
			this.m_audioPoolDict.Add(audioType, audioPool);
		}
	}

	public AudioPool GetAudioPoolByType(eAudioSourceType audioType)
	{
		if (this.m_audioPoolDict.ContainsKey(audioType))
		{
			return this.m_audioPoolDict[audioType];
		}
		AudioPool audioPool = new AudioPool();
		audioPool.Init(this.m_audioPrefabDict[audioType], this.m_audioRoot, 1);
		this.m_audioPoolDict.Add(audioType, audioPool);
		return audioPool;
	}

	public void ReleaseAudioPools()
	{
		foreach (KeyValuePair<eAudioSourceType, AudioPool> current in this.m_audioPoolDict)
		{
			current.Value.ReleaseUnusedAudioSourceControl();
		}
	}
}
