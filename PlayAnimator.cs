using System;
using UnityEngine;

public class PlayAnimator : MonoBehaviour
{
	public string[] animations;

	private Animator curAnimator;

	private void Awake()
	{
		this.curAnimator = base.GetComponentInChildren<Animator>();
		int num = UnityEngine.Random.Range(0, this.animations.Length);
		this.curAnimator.Play(this.animations[num]);
	}

	private void OnSpawned()
	{
		int num = UnityEngine.Random.Range(0, this.animations.Length);
		this.curAnimator.Play(this.animations[num]);
	}
}
