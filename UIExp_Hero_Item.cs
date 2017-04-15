using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class UIExp_Hero_Item : MonoBehaviour
{
	private NewHeroItem newHeroData;

	[SerializeField]
	private UISprite BG;

	[SerializeField]
	private UISprite Slider;

	[SerializeField]
	private UILabel Name;

	[SerializeField]
	private UILabel Level;

	[SerializeField]
	private UILabel Label;

	[SerializeField]
	private Transform HeroItem;

	public HeroInfoData HeroID;

	private int RecordNumber;

	private int recordNumber2;

	public string EuiqpmentID;

	private void Awake()
	{
		MVC_MessageManager.AddListener_model(MobaGameCode.UseProps, new MobaMessageFunc(this.OnUseProps));
	}

	private void Start()
	{
	}

	private void Init()
	{
		if (this.newHeroData == null)
		{
			this.newHeroData = NGUITools.AddChild(this.HeroItem.gameObject, NewHeroItem.GetHeroItemPrefab()).GetComponent<NewHeroItem>();
		}
		if (!base.gameObject.GetComponent<UIEventListener>())
		{
			UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.click);
			UIEventListener.Get(base.gameObject).LongPress = new UIEventListener.VoidDelegate(this.click);
			base.gameObject.GetComponent<UIEventListener>().isCanLongPress = true;
		}
	}

	public void AddItem(HeroInfoData hero)
	{
		this.Init();
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(hero.ModelId);
		this.HeroID = hero;
		this.Name.text = CharacterDataMgr.instance.ShowHeroName(hero.Grade, heroMainData);
		this.ShowSlider(hero.Exp, 0);
		HeroData herosData = new HeroData
		{
			Quality = hero.Grade,
			LV = CharacterDataMgr.instance.GetLevel(hero.Exp),
			Stars = hero.Level,
			HeroID = hero.ModelId
		};
		this.newHeroData.Init(herosData, NewHeroItem.CardType.HeroAvator, true, true);
	}

	private void ShowSlider(long exp, int number)
	{
		long exp2 = ModelManager.Instance.Get_userData_filed_X("Exp");
		int userLevel = CharacterDataMgr.instance.GetUserLevel(exp2);
		SysSummonersLevelVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersLevelVo>(userLevel.ToString());
		bool flag = CharacterDataMgr.instance.IsFullExp(userLevel, exp);
		if (flag)
		{
			this.Level.text = "lv.[d19c00]" + dataById.hero_main;
			this.newHeroData.ShowLV(dataById.hero_main);
			this.Label.text = "经验已满";
			this.Label.alpha = 1f;
			this.Slider.fillAmount = 1f;
			this.BG.alpha = 0.7f;
		}
		else
		{
			this.Level.text = "lv.[d19c00]" + CharacterDataMgr.instance.GetLevel(exp);
			this.newHeroData.ShowLV(CharacterDataMgr.instance.GetLevel(exp));
			this.Label.text = "X" + number;
			base.StartCoroutine(this.ShowNumber(number));
			this.BG.alpha = 1f;
		}
	}

	public void click(GameObject object_1 = null)
	{
		if (this.Label.text == "经验已满")
		{
			Singleton<TipView>.Instance.ShowViewSetText("该英雄等级达到上限", 1f);
			return;
		}
	}

	[DebuggerHidden]
	private IEnumerator ShowNumber(int number)
	{
		UIExp_Hero_Item.<ShowNumber>c__IteratorA3 <ShowNumber>c__IteratorA = new UIExp_Hero_Item.<ShowNumber>c__IteratorA3();
		<ShowNumber>c__IteratorA.number = number;
		<ShowNumber>c__IteratorA.<$>number = number;
		<ShowNumber>c__IteratorA.<>f__this = this;
		return <ShowNumber>c__IteratorA;
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		MVC_MessageManager.RemoveListener_model(MobaGameCode.UseProps, new MobaMessageFunc(this.OnUseProps));
	}

	private void OnUseProps(MobaMessage msg)
	{
		if (msg == null)
		{
			return;
		}
		OperationResponse operationResponse = msg.Param as OperationResponse;
		if (operationResponse == null)
		{
			return;
		}
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode != MobaErrorCode.Ok)
		{
			this.OnDisableCallBack(num, operationResponse.DebugMessage, 0);
		}
		else
		{
			string s = operationResponse.Parameters[102] as string;
			this.OnDisableCallBack(num, operationResponse.DebugMessage, int.Parse(s));
		}
	}

	private void OnDisableCallBack(int i = 0, string str = null, int j = 0)
	{
	}
}
