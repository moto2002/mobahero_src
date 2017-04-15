using System;

[Serializable]
public struct AudioClipInfo
{
	public string clipName;

	public eAudioSourceType audioSourceType;

	public float volume;

	public float fadeInSpeed;

	public float fadeOutSpeed;

	public int audioPriority;
}
