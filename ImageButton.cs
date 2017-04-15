using System;
using UnityEngine;

public class ImageButton : MonoBehaviour
{
	public enum VoiceType
	{
		None,
		Open,
		Switch,
		Btn,
		Close
	}

	[SerializeField]
	public bool TextureTwoState = true;

	[SerializeField]
	public UISprite uiSprite;

	private string NormalTexture;

	private string PressTexure;

	[SerializeField]
	public bool Color32TwoSate;

	[SerializeField]
	public UILabel uiLabel;

	private Color32 NormalColor32;

	[SerializeField]
	public Color32 PressColor32;

	[SerializeField]
	public bool ButtonTextureTwoState = true;

	[SerializeField]
	public UISprite uiButtonSprite;

	private string NormalButtonTexture;

	private string PressButtonTexure;

	[SerializeField]
	public ImageButton.VoiceType voice = ImageButton.VoiceType.Btn;

	private string VoiceName;

	private bool audioOpen = true;

	private void audioChoice()
	{
		switch (this.voice)
		{
		case ImageButton.VoiceType.None:
			this.VoiceName = null;
			break;
		case ImageButton.VoiceType.Open:
			this.VoiceName = "sd_int_open";
			break;
		case ImageButton.VoiceType.Switch:
			this.VoiceName = "sd_hero_switch";
			break;
		case ImageButton.VoiceType.Btn:
			this.VoiceName = "sd_ui_btn";
			break;
		case ImageButton.VoiceType.Close:
			this.VoiceName = "sd_int_close";
			break;
		default:
			this.VoiceName = null;
			break;
		}
	}

	private void Start()
	{
		this.audioChoice();
		if (!this.uiSprite)
		{
			this.uiSprite = base.transform.GetComponent<UISprite>();
		}
		this.NormalTexture = this.uiSprite.spriteName;
		this.PressTexure = this.NormalTexture + "-1";
		if (this.uiLabel)
		{
			this.NormalColor32 = this.uiLabel.color;
		}
		if (this.uiButtonSprite)
		{
			this.NormalButtonTexture = this.uiButtonSprite.spriteName;
			this.PressButtonTexure = this.NormalButtonTexture + "-1";
		}
	}

	private void OnPress(bool onpress)
	{
		if (onpress)
		{
			if (this.VoiceName != null && this.audioOpen)
			{
				this.audioOpen = false;
				AudioMgr.Play(new AudioClipInfo
				{
					clipName = this.VoiceName,
					audioSourceType = eAudioSourceType.UI,
					audioPriority = 128,
					volume = 0.8f
				}, null);
			}
			if (this.TextureTwoState && this.uiSprite)
			{
				this.uiSprite.spriteName = this.PressTexure;
			}
			if (this.Color32TwoSate && this.uiLabel)
			{
				this.uiLabel.color = this.PressColor32;
			}
			if (this.ButtonTextureTwoState && this.uiButtonSprite)
			{
				this.uiButtonSprite.spriteName = this.PressButtonTexure;
			}
		}
		else
		{
			if (this.uiSprite)
			{
				this.uiSprite.spriteName = this.NormalTexture;
			}
			if (this.uiLabel)
			{
				this.uiLabel.color = this.NormalColor32;
			}
			if (this.uiButtonSprite)
			{
				this.uiButtonSprite.spriteName = this.NormalButtonTexture;
			}
			this.audioOpen = true;
		}
	}
}
