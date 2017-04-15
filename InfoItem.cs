using System;
using UnityEngine;

public class InfoItem : MonoBehaviour
{
	[SerializeField]
	private UISprite sideBG;

	[SerializeField]
	private UILabel itemQuality;

	[SerializeField]
	private UILabel itemName;

	[SerializeField]
	private Transform mark;

	[SerializeField]
	private UISprite markSprite;

	[SerializeField]
	private UILabel markLabel;

	[SerializeField]
	private UILabel sourceLabel;

	[SerializeField]
	private Transform buttonItem;

	[SerializeField]
	private UILabel typeLabel;

	private ButtonType bt;

	public ButtonType BT
	{
		get
		{
			return this.bt;
		}
	}

	private void Awake()
	{
	}

	private void OnEnable()
	{
		for (int num = 0; num != base.transform.childCount; num++)
		{
			base.transform.GetChild(num).gameObject.SetActive(false);
		}
	}

	public void Init(bool isActive)
	{
		for (int num = 0; num != base.transform.childCount; num++)
		{
			base.transform.GetChild(num).gameObject.SetActive(isActive);
		}
	}

	public void CheckInfoState(EffectItem item)
	{
		this.ChangeQualityAndBG(item.Quality);
		this.ChangeName(item.Name);
		this.ChangeSourceAndButton(item);
	}

	private void ChangeQualityAndBG(int quality)
	{
		this.itemQuality.text = LanguageManager.Instance.GetStringById("Customization_Quality_0" + quality);
		switch (quality)
		{
		case 1:
			this.sideBG.color = new Color32(10, 150, 0, 255);
			this.itemQuality.color = new Color32(0, 255, 192, 255);
			break;
		case 2:
			this.sideBG.color = new Color32(0, 152, 242, 255);
			this.itemQuality.color = new Color32(0, 228, 255, 255);
			break;
		case 3:
			this.sideBG.color = new Color32(227, 0, 174, 255);
			this.itemQuality.color = new Color32(255, 30, 252, 255);
			break;
		case 4:
			this.sideBG.color = new Color32(223, 106, 0, 255);
			this.itemQuality.color = new Color32(232, 198, 11, 255);
			break;
		case 5:
			this.sideBG.color = new Color32(255, 0, 47, 255);
			this.itemQuality.color = new Color32(255, 0, 126, 255);
			break;
		}
	}

	private void ChangeName(string name)
	{
		this.itemName.text = name;
	}

	private void ChangeSourceAndButton(EffectItem item)
	{
		UISprite component = this.buttonItem.GetComponent<UISprite>();
		UIButton component2 = this.buttonItem.GetComponent<UIButton>();
		if (!item.IsOwned || (item.IsOwned && !item.IsCurrentHero && item.Count == 0))
		{
			this.mark.gameObject.SetActive(false);
			this.sourceLabel.gameObject.SetActive(true);
			string text = string.Empty;
			string text2 = string.Empty;
			switch (item.Source)
			{
			case 1:
				component.spriteName = "Hero_personal_btn_05";
				component2.normalSprite = "Hero_personal_btn_05";
				component2.hoverSprite = "Hero_personal_btn_05";
				component2.pressedSprite = "Hero_personal_btn_06";
				text = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Buy");
				text2 = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Fromshop");
				this.typeLabel.text = text;
				this.sourceLabel.text = text2;
				this.bt = ButtonType.CapShop;
				goto IL_302;
			case 2:
				component.spriteName = "Hero_personal_btn_07";
				component2.normalSprite = "Hero_personal_btn_07";
				component2.hoverSprite = "Hero_personal_btn_07";
				component2.pressedSprite = "Hero_personal_btn_08";
				text = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Tobottle");
				text2 = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Frombottle");
				this.typeLabel.text = text;
				this.sourceLabel.text = text2;
				this.bt = ButtonType.MagicBottle;
				goto IL_302;
			case 4:
				component.spriteName = "Hero_personal_btn_07";
				component2.normalSprite = "Hero_personal_btn_07";
				component2.hoverSprite = "Hero_personal_btn_07";
				component2.pressedSprite = "Hero_personal_btn_08";
				text = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Toachievement");
				text2 = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Fromachievement");
				this.typeLabel.text = text;
				this.sourceLabel.text = text2;
				this.bt = ButtonType.Achievement;
				goto IL_302;
			case 5:
				component.spriteName = "Hero_personal_btn_07";
				component2.normalSprite = "Hero_personal_btn_07";
				component2.hoverSprite = "Hero_personal_btn_07";
				component2.pressedSprite = "Hero_personal_btn_08";
				text = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Torank");
				text2 = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Fromrank");
				this.typeLabel.text = text;
				this.sourceLabel.text = text2;
				this.bt = ButtonType.RankList;
				goto IL_302;
			case 6:
				component.spriteName = "Hero_personal_btn_07";
				component2.normalSprite = "Hero_personal_btn_07";
				component2.hoverSprite = "Hero_personal_btn_07";
				component2.pressedSprite = "Hero_personal_btn_08";
				text = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Topvp");
				text2 = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Frompvp");
				this.typeLabel.text = text;
				this.sourceLabel.text = text;
				this.bt = ButtonType.Battle;
				goto IL_302;
			}
			text2 = LanguageManager.Instance.GetStringById("HeroAltar_HeroSource_0");
			this.sourceLabel.text = text2;
			this.buttonItem.gameObject.SetActive(false);
			IL_302:;
		}
		else
		{
			this.mark.gameObject.SetActive(true);
			this.sourceLabel.gameObject.SetActive(false);
			bool isUsing = item.IsUsing;
			bool isCurrentHero = item.IsCurrentHero;
			string text3 = string.Empty;
			string text4 = string.Empty;
			text3 = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Apparel");
			text4 = LanguageManager.Instance.GetStringById("HeroAltar_Customization_Discharge");
			component.spriteName = ((!isUsing) ? "Hero_personal_btn_03" : ((!isCurrentHero) ? "Hero_personal_btn_03" : "Hero_personal_btn_01"));
			component2.normalSprite = ((!isUsing) ? "Hero_personal_btn_03" : ((!isCurrentHero) ? "Hero_personal_btn_03" : "Hero_personal_btn_01"));
			component2.hoverSprite = ((!isUsing) ? "Hero_personal_btn_03" : ((!isCurrentHero) ? "Hero_personal_btn_03" : "Hero_personal_btn_01"));
			component2.pressedSprite = ((!isUsing) ? "Hero_personal_btn_04" : ((!isCurrentHero) ? "Hero_personal_btn_04" : "Hero_personal_btn_02"));
			this.typeLabel.text = ((!isUsing) ? text3 : ((!isCurrentHero) ? text3 : text4));
			this.markSprite.gameObject.SetActive(false);
			this.markLabel.gameObject.SetActive(true);
			this.bt = ((!isUsing) ? ButtonType.Wear : ((!isCurrentHero) ? ButtonType.Wear : ButtonType.Discharge));
		}
	}
}
