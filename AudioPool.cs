using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool
{
	private UnityEngine.Object m_audioSourcePrefab;

	private Transform m_targetRoot;

	private List<AudioSourceControl> m_audioSourceControlList = new List<AudioSourceControl>();

	public void Init(UnityEngine.Object prefab, Transform targetRoot, int num)
	{
		this.m_audioSourcePrefab = prefab;
		this.m_targetRoot = targetRoot;
		for (int i = 0; i < num; i++)
		{
			this.AddAudioSourceControl();
		}
	}

	private AudioSourceControl AddAudioSourceControl()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_audioSourcePrefab) as GameObject;
		gameObject.transform.parent = this.m_targetRoot;
		gameObject.transform.localPosition = Vector3.zero;
		AudioSourceControl component = gameObject.GetComponent<AudioSourceControl>();
		gameObject.SetActive(false);
		this.m_audioSourceControlList.Add(component);
		return component;
	}

	public AudioSourceControl GetFreeAudioSourceControl()
	{
		AudioSourceControl audioSourceControl = null;
		int num = -1;
		for (int i = 0; i < this.m_audioSourceControlList.Count; i++)
		{
			if (!this.m_audioSourceControlList[i].isPlaying)
			{
				audioSourceControl = this.m_audioSourceControlList[i];
				num = i;
				break;
			}
		}
		if (audioSourceControl == null || audioSourceControl.audioSourceObject == null || audioSourceControl.audioSource == null)
		{
			if (num != -1)
			{
				this.m_audioSourceControlList.RemoveAt(num);
			}
			audioSourceControl = this.AddAudioSourceControl();
		}
		if (audioSourceControl.audioSourceObject.activeSelf)
		{
			audioSourceControl.audioSourceObject.SetActive(false);
		}
		return audioSourceControl;
	}

	public void ReleaseAll()
	{
		for (int i = 0; i < this.m_audioSourceControlList.Count; i++)
		{
			this.m_audioSourceControlList[i].Clear();
		}
		this.m_audioSourceControlList.Clear();
	}

	public List<AudioSourceControl> GetUsingAudioSourceControlList()
	{
		List<AudioSourceControl> list = new List<AudioSourceControl>();
		for (int i = this.m_audioSourceControlList.Count - 1; i >= 0; i--)
		{
			AudioSourceControl audioSourceControl = this.m_audioSourceControlList[i];
			if (audioSourceControl == null || audioSourceControl.audioSourceObject == null)
			{
				this.m_audioSourceControlList.RemoveAt(i);
			}
			else if (audioSourceControl.isPlaying)
			{
				list.Add(audioSourceControl);
			}
		}
		return list;
	}

	public void ReleaseUnusedAudioSourceControl()
	{
		for (int i = this.m_audioSourceControlList.Count - 1; i >= 0; i--)
		{
			AudioSourceControl audioSourceControl = this.m_audioSourceControlList[i];
			if (audioSourceControl == null || audioSourceControl.audioSourceObject == null)
			{
				this.m_audioSourceControlList.RemoveAt(i);
			}
			else if (!audioSourceControl.isPlaying)
			{
				audioSourceControl.Clear();
				this.m_audioSourceControlList.RemoveAt(i);
			}
		}
	}
}
