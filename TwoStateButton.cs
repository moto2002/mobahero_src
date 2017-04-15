using System;
using System.Collections.Generic;
using UnityEngine;

public class TwoStateButton : MonoBehaviour
{
	public bool TextureTwoState = true;

	public List<UISprite> uiSprite = new List<UISprite>();

	private List<string> NormalTexture = new List<string>();

	private List<string> PressTexure = new List<string>();

	public VoiceType voice = VoiceType.Btn;

	private string VoiceName;

	private bool audioOpen = true;

	private BetterList<string> GetListOfSprites = new BetterList<string>();

	private List<string> normalName = new List<string>
	{
		"backbutton-1",
		"buttom_r_di",
		"button_jnts-1",
		"button-arrow",
		"phb_button_Level1_Selection",
		"button-1",
		"yz_button_next",
		"suspend-1",
		"liujiaoanniu_0000_anniu"
	};

	private List<string> pressName = new List<string>
	{
		"backbutton-2",
		"button_jnts-2",
		"button_jnts-2",
		"button-arrow-1",
		"phb_button_Level1_normal",
		"button-2",
		"button_jnts-2",
		"suspend-2",
		"button_jnts-2"
	};

	private void Start()
	{
		this.audioChoice();
		for (int i = 0; i < this.uiSprite.Count; i++)
		{
			this.uiSprite[i] = base.transform.GetComponent<UISprite>();
		}
		this.GetNormalAndPressTexName();
	}

	private void OnPress(bool onpress)
	{
		if (onpress)
		{
			if (this.VoiceName != null && this.audioOpen)
			{
				this.audioOpen = false;
			}
			if (this.TextureTwoState)
			{
				for (int i = 0; i < this.uiSprite.Count; i++)
				{
					this.uiSprite[i].spriteName = this.PressTexure[i];
				}
			}
		}
		else
		{
			for (int j = 0; j < this.uiSprite.Count; j++)
			{
				this.uiSprite[j].spriteName = this.NormalTexture[j];
			}
			this.audioOpen = true;
		}
	}

	private void audioChoice()
	{
		switch (this.voice)
		{
		case VoiceType.None:
			this.VoiceName = null;
			break;
		case VoiceType.Open:
			this.VoiceName = "sd_int_open";
			break;
		case VoiceType.Switch:
			this.VoiceName = "sd_hero_switch";
			break;
		case VoiceType.Btn:
			this.VoiceName = "sd_ui_btn";
			break;
		case VoiceType.Close:
			this.VoiceName = "sd_int_close";
			break;
		default:
			this.VoiceName = null;
			break;
		}
	}

	public static void Get(GameObject object_1)
	{
		if (!object_1.transform.GetComponent<TwoStateButton>())
		{
			object_1.AddComponent<TwoStateButton>();
		}
	}

	public void AddSprite(GameObject[] objectAll)
	{
		for (int i = 0; i < objectAll.Length; i++)
		{
			this.uiSprite.Add(objectAll[i].GetComponent<UISprite>());
		}
	}

	public void AddSprite(GameObject object1)
	{
		this.uiSprite.Add(object1.GetComponent<UISprite>());
	}

	private void GetNormalAndPressTexName()
	{
		this.NormalTexture.Clear();
		this.PressTexure.Clear();
		for (int i = 0; i < this.uiSprite.Count; i++)
		{
			this.NormalTexture.Add(this.uiSprite[i].spriteName);
			this.PressTexure.Add(this.ReturnPressTexName(this.uiSprite[i]));
		}
	}

	private string ReturnPressTexName(UISprite standard)
	{
		int index = this.normalName.IndexOf(standard.spriteName);
		string text = this.pressName[index];
		if (this.GetListOfSprites == null || this.GetListOfSprites.size == 0)
		{
			this.GetListOfSprites = standard.atlas.GetListOfSprites();
		}
		if (!this.GetListOfSprites.Contains(text))
		{
			text = standard.spriteName;
		}
		return text;
	}
}
