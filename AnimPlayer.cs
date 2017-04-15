using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimPlayer : MonoBehaviour
{
	public List<string> anis_;

	public string MeshAnimationRes;

	public static bool useMeshAnimation;

	public MeshAnimation[] meshanim;

	public bool createdModelAnim;

	public GameObject model;

	private Animator curranimator;

	private SkinnedMeshRenderer[] currskrenderers;

	private MeshRenderer mr;

	private MeshAnimation ma;

	public static bool useMeshanim;

	public string playingAnimation = string.Empty;

	public void disableAnimator()
	{
		if (this.curranimator != null)
		{
			this.curranimator.gameObject.SetActive(false);
			this.curranimator.enabled = false;
		}
		if (this.currskrenderers != null)
		{
			for (int i = 0; i < this.currskrenderers.Length; i++)
			{
				this.currskrenderers[i].gameObject.SetActive(false);
				this.currskrenderers[i].enabled = false;
			}
		}
		if (this.meshanim[0] != null)
		{
			this.meshanim[0].gameObject.SetActive(true);
			this.mr.enabled = true;
			this.ma.enabled = true;
		}
	}

	public void showAnimator()
	{
		if (this.curranimator != null)
		{
			this.curranimator.gameObject.SetActive(true);
			this.curranimator.enabled = true;
		}
		if (this.currskrenderers != null)
		{
			for (int i = 0; i < this.currskrenderers.Length; i++)
			{
				this.currskrenderers[i].gameObject.SetActive(true);
				this.currskrenderers[i].enabled = true;
			}
		}
		if (this.meshanim[0] != null)
		{
			this.meshanim[0].gameObject.active = false;
			this.mr.enabled = false;
			this.ma.enabled = false;
		}
	}

	public bool IsPlayingAnimationContain(string name)
	{
		return !string.IsNullOrEmpty(this.playingAnimation) && this.playingAnimation.Contains(name);
	}

	public void PlayAnimate(string animationName, WrapMode wrapMode)
	{
		if (AnimPlayer.useMeshAnimation)
		{
			this.playingAnimation = animationName;
			ModelAnimation.Play(base.gameObject, this.meshanim, this.MeshAnimationRes, animationName, wrapMode, 1.2f, false);
		}
	}

	public void StopAnim(string animationName)
	{
		if (AnimPlayer.useMeshAnimation)
		{
			ModelAnimation.Stop(base.gameObject, this.meshanim, this.MeshAnimationRes, animationName);
		}
	}

	private void Awake()
	{
		if (!AnimPlayer.useMeshanim)
		{
			return;
		}
		this.curranimator = base.gameObject.GetComponentInChildren<Animator>();
		this.currskrenderers = base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		this.mr = this.meshanim[0].GetComponent<MeshRenderer>();
		this.ma = this.meshanim[0].GetComponent<MeshAnimation>();
	}

	private void Start()
	{
		if (!AnimPlayer.useMeshanim)
		{
			return;
		}
	}
}
