using System;
using UnityEngine;

[AddComponentMenu("Audio/完全播放")]
public class CompleteVoicePlayer : MonoBehaviour
{
	[SerializeField]
	private AudioClip _clip;

	[SerializeField]
	private float _delay;

	private void Awake()
	{
		GameObject gameObject = new GameObject();
		DelayedVoicePlayer delayedVoicePlayer = gameObject.AddComponent<DelayedVoicePlayer>();
		delayedVoicePlayer.play(this._clip, 1f, this._delay);
	}
}
