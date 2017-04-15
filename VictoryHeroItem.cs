using System;
using UnityEngine;

public class VictoryHeroItem : MonoBehaviour
{
	[SerializeField]
	private Transform V_HeroItem;

	[SerializeField]
	private UISprite V_Slider;

	[SerializeField]
	private UILabel V_Label;

	private GameObject heroItem;

	public void Init(HeroData heroData, float precent, string str)
	{
		this.heroItem = (Resources.Load("Prefab/NewUI/SelectHero/SelectHeroItem") as GameObject);
		GameObject gameObject = NGUITools.AddChild(this.V_HeroItem.gameObject, this.heroItem);
		gameObject.GetComponent<NewHeroItem>().Init(heroData, NewHeroItem.CardType.HeroAvator, true, true);
		this.V_Slider.fillAmount = precent;
		this.V_Label.text = str;
	}
}
