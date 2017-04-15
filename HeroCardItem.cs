using System;
using UnityEngine;

public class HeroCardItem : MonoBehaviour
{
	public UISprite Texture;

	public UITexture HeroTexture;

	public UILabel Grade;

	public UISprite TypeTexture;

	public UILabel HeroName;

	public Transform StarTrans;

	public Transform[] Star = new Transform[5];

	public Transform SoulStone;

	public UILabel SoulStoneLabel;

	public UISlider ForeBG;

	public Transform Label;

	public UISprite[] TypeSign = new UISprite[6];

	public Transform HeroEquipTrans;

	public UITexture[] HeroEquip = new UITexture[6];
}
