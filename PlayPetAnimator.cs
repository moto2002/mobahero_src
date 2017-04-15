using System;
using UnityEngine;

public class PlayPetAnimator : MonoBehaviour
{
	private Animator anim;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		if (null == this.anim)
		{
			this.anim = base.transform.GetComponent<Animator>();
		}
		if (null != this.anim)
		{
			this.anim.Play("HeroBreath1");
		}
	}
}
