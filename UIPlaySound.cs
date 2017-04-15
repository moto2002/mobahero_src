using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Play Sound")]
public class UIPlaySound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		Custom
	}

	public AudioClip audioClip;

	public UIPlaySound.Trigger trigger;

	private bool mIsOver;

	public string wwiseSound;

	[Range(0f, 1f)]
	public float volume = 1f;

	[Range(0f, 2f)]
	public float pitch = 1f;

	private void OnHover(bool isOver)
	{
		if (this.trigger == UIPlaySound.Trigger.OnMouseOver)
		{
			if (this.mIsOver == isOver)
			{
				return;
			}
			this.mIsOver = isOver;
		}
		if (base.enabled && ((isOver && this.trigger == UIPlaySound.Trigger.OnMouseOver) || (!isOver && this.trigger == UIPlaySound.Trigger.OnMouseOut)))
		{
			if (!AudioMgr.Instance.isUsingWWise() && this.audioClip != null)
			{
				AudioMgr.Play_UI(this.audioClip, this.volume, this.pitch, null);
			}
			else if (string.IsNullOrEmpty(this.wwiseSound))
			{
				if (this.audioClip != null)
				{
					AudioMgr.Play_UI(this.audioClip, this.volume, this.pitch, null);
				}
			}
			else
			{
				AudioMgr.PlayUI(this.wwiseSound, null, false, false);
			}
		}
	}

	private void OnPress(bool isPressed)
	{
		if (this.trigger == UIPlaySound.Trigger.OnPress)
		{
			if (this.mIsOver == isPressed)
			{
				return;
			}
			this.mIsOver = isPressed;
		}
		if (base.enabled && ((isPressed && this.trigger == UIPlaySound.Trigger.OnPress) || (!isPressed && this.trigger == UIPlaySound.Trigger.OnRelease)))
		{
			if (!AudioMgr.Instance.isUsingWWise() && this.audioClip != null)
			{
				AudioMgr.Play_UI(this.audioClip, this.volume, this.pitch, null);
			}
			else if (string.IsNullOrEmpty(this.wwiseSound))
			{
				if (this.audioClip != null)
				{
					AudioMgr.Play_UI(this.audioClip, this.volume, this.pitch, null);
				}
			}
			else
			{
				AudioMgr.PlayUI(this.wwiseSound, null, false, false);
			}
		}
	}

	private void OnClick()
	{
		if (base.enabled && this.trigger == UIPlaySound.Trigger.OnClick)
		{
			if (!AudioMgr.Instance.isUsingWWise() && this.audioClip != null)
			{
				AudioMgr.Play_UI(this.audioClip, this.volume, this.pitch, null);
			}
			else if (string.IsNullOrEmpty(this.wwiseSound))
			{
				if (this.audioClip != null)
				{
					AudioMgr.Play_UI(this.audioClip, this.volume, this.pitch, null);
				}
			}
			else
			{
				AudioMgr.PlayUI(this.wwiseSound, null, false, false);
			}
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	public void Play()
	{
		if (!AudioMgr.Instance.isUsingWWise() && this.audioClip != null)
		{
			AudioMgr.Play_UI(this.audioClip, this.volume, this.pitch, null);
		}
		else if (string.IsNullOrEmpty(this.wwiseSound))
		{
			if (this.audioClip != null)
			{
				AudioMgr.Play_UI(this.audioClip, this.volume, this.pitch, null);
			}
		}
		else
		{
			AudioMgr.PlayUI(this.wwiseSound, null, false, false);
		}
	}
}
