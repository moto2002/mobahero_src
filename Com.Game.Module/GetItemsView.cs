using Assets.Scripts.GUILogic.View.BattleSettlement;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros;
using MobaProtocol;
using MobaProtocol.Data;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class GetItemsView : BaseView<GetItemsView>
	{
		public class ExchangeItemData
		{
			public string id;

			public Texture rawTexture;

			public Texture targetTexture;

			public ItemType type;

			public string rawName = "N/A";

			public string rawDescript = "N/A";

			public string targetName = "N/A";

			public string targetDescript = "N/A";

			public int number;

			public SysCouponVo couponItem;

			public ExchangeItemData(ItemType _type, string _id, bool _writeInModel)
			{
				int num = 0;
				string unikey = string.Empty;
				string text = string.Empty;
				this.type = _type;
				this.id = _id;
				switch (_type)
				{
				case ItemType.HeadPortrait:
				{
					SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(_id);
					if (dataById == null)
					{
						return;
					}
					this.rawName = LanguageManager.Instance.GetStringById("Currency_HeadPortrait");
					this.rawDescript = LanguageManager.Instance.GetStringById(dataById.headportrait_name);
					this.rawTexture = ResourceManager.Load<Texture>(dataById.headportrait_icon, true, true, null, 0, false);
					num = dataById.convert_reward;
					break;
				}
				case ItemType.Hero:
				{
					SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(_id);
					if (heroMainData == null)
					{
						return;
					}
					this.rawName = LanguageManager.Instance.GetStringById("Currency_Hero");
					this.rawDescript = LanguageManager.Instance.GetStringById(heroMainData.name);
					this.rawTexture = ResourceManager.Load<Texture>(heroMainData.Loading_icon, true, true, null, 0, false);
					num = heroMainData.convert_reward;
					break;
				}
				case ItemType.HeroSkin:
				{
					SysHeroSkinVo dataById2 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(_id);
					if (dataById2 == null)
					{
						return;
					}
					this.rawName = LanguageManager.Instance.GetStringById("Currency_Skin");
					this.rawDescript = LanguageManager.Instance.GetStringById(dataById2.name);
					this.rawTexture = ResourceManager.Load<Texture>(dataById2.Loading_icon, true, true, null, 0, false);
					num = dataById2.convert_reward;
					break;
				}
				case ItemType.Coupon:
				{
					SysCouponVo dataById3 = BaseDataMgr.instance.GetDataById<SysCouponVo>(_id);
					if (dataById3 == null)
					{
						return;
					}
					if (dataById3.mother_type == 1)
					{
						SysHeroMainVo heroMainData2 = BaseDataMgr.instance.GetHeroMainData(dataById3.mother_id);
						if (heroMainData2 == null)
						{
							return;
						}
						this.rawTexture = ResourceManager.Load<Texture>(heroMainData2.Loading_icon, true, true, null, 0, false);
					}
					else if (dataById3.mother_type == 2)
					{
						SysHeroSkinVo dataById4 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(dataById3.mother_id);
						if (dataById4 == null)
						{
							return;
						}
						this.rawTexture = ResourceManager.Load<Texture>(dataById4.Loading_icon, true, true, null, 0, false);
					}
					this.rawName = "打折卡";
					this.rawDescript = LanguageManager.Instance.GetStringById(dataById3.name);
					num = dataById3.convert_reward;
					this.couponItem = dataById3;
					break;
				}
				case ItemType.PortraitFrame:
				{
					SysSummonersPictureframeVo dataById5 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(_id);
					if (dataById5 == null)
					{
						return;
					}
					this.rawName = LanguageManager.Instance.GetStringById("Currency_PictureFrame");
					this.rawDescript = LanguageManager.Instance.GetStringById(dataById5.pictureframe_name);
					this.rawTexture = ResourceManager.Load<Texture>(dataById5.pictureframe_icon, true, true, null, 0, false);
					num = dataById5.convert_reward;
					break;
				}
				}
				unikey = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(num.ToString()).drop_items;
				text = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(unikey).rewards;
				if (text.StartsWith("2|7777"))
				{
					this.targetTexture = ResourceManager.Load<Texture>("Get_EXP_bottles", true, true, null, 0, false);
					this.targetName = LanguageManager.Instance.GetStringById("GameItems_Name_7777");
					this.targetDescript = LanguageManager.Instance.GetStringById("GameItems_Describe_7777");
					this.number = Convert.ToInt32(text.Replace("2|7777|", string.Empty));
					if (_writeInModel)
					{
						EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == 7777);
						if (equipmentInfoData != null)
						{
							equipmentInfoData.Count += this.number;
						}
						else
						{
							SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获取转换奖励...", true, 15f);
							SendMsgManager.Instance.SendMsg(MobaGameCode.GetEquipmentList, param, new object[0]);
						}
					}
				}
				else if (text.StartsWith("1|1"))
				{
					this.targetTexture = ResourceManager.Load<Texture>("Get_coin", true, true, null, 0, false);
					this.targetName = LanguageManager.Instance.GetStringById("Currency_Gold");
					this.targetDescript = LanguageManager.Instance.GetStringById("Currency_Gold_Desc");
					this.number = Convert.ToInt32(text.Replace("1|1|", string.Empty));
					if (_writeInModel)
					{
						UserData userData = ModelManager.Instance.Get_userData_X();
						if (userData != null)
						{
							userData.Money += (long)this.number;
							if (Singleton<MenuTopBarView>.Instance.gameObject)
							{
								Singleton<MenuTopBarView>.Instance.RefreshUI();
							}
						}
						else
						{
							ClientLogger.Error("Can't find UserData in Client. [Get a repeat gold-exchanged item]");
						}
					}
				}
				else if (text.StartsWith("1|9"))
				{
					this.targetTexture = ResourceManager.Load<Texture>("Get_Magic_bottle_cap", true, true, null, 0, false);
					this.targetName = LanguageManager.Instance.GetStringById("Currency_MagicBottle");
					this.targetDescript = LanguageManager.Instance.GetStringById("Currency_MagicBottle_Desc");
					this.number = Convert.ToInt32(text.Replace("1|9|", string.Empty));
					if (_writeInModel)
					{
						UserData userData2 = ModelManager.Instance.Get_userData_X();
						if (userData2 != null)
						{
							userData2.SmallCap += this.number;
							if (Singleton<MenuTopBarView>.Instance.gameObject)
							{
								Singleton<MenuTopBarView>.Instance.RefreshUI();
							}
						}
						else
						{
							ClientLogger.Error("Can't find UserData in Client. [Get a repeat cap-exchanged item]");
						}
					}
				}
			}
		}

		private Transform mStaticAnchor;

		private Transform mCenterItemAnchor;

		private TweenColor mShineTween;

		private UITexture mShine;

		private UILabel mExchangeTip;

		private GameObject mCirEffCache;

		private GameObject mRetEffCache;

		private GameObject mSqarEffCache;

		private GameObject mRuneEffCache;

		private GameObject mExchangeEffCache;

		private GameObject mSummonerExpCache;

		private GameObject mBG;

		private Queue<int> mDiamondNum = new Queue<int>();

		private Queue<int> mCapNum = new Queue<int>();

		private Queue<int> mRune = new Queue<int>();

		private Queue<string> mHero = new Queue<string>();

		private Queue<int> mHeroSkin = new Queue<int>();

		private Queue<string> mHeadIcon = new Queue<string>();

		private Queue<string> mHeadFrame = new Queue<string>();

		private Queue<GetItemsView.ExchangeItemData> mExchangeItem = new Queue<GetItemsView.ExchangeItemData>();

		private Queue<string> mGameItem = new Queue<string>();

		private Queue<string> mCoupon = new Queue<string>();

		private Queue<int> mGameBuff = new Queue<int>();

		private Queue<int> mCoinNum = new Queue<int>();

		private Queue<int> mBottleNum = new Queue<int>();

		private Queue<int> mSpeaker = new Queue<int>();

		private int mSummonerExp;

		private long mSummonerExp_from;

		private CoroutineManager cMgr = new CoroutineManager();

		private Transform tempEffTrans;

		public Callback onFinish;

		public GetItemsView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Victory/GetItemsView");
		}

		public override void Init()
		{
			base.Init();
			this.mStaticAnchor = this.transform.Find("StaticAnchor");
			this.mCenterItemAnchor = this.transform.Find("CenterItemAnchor");
			this.mBG = this.mStaticAnchor.FindChild("BG").gameObject;
			this.mShineTween = this.mStaticAnchor.FindChild("shine").GetComponent<TweenColor>();
			this.mShine = this.mShineTween.GetComponent<UITexture>();
			this.mCirEffCache = this.transform.Find("CenterItem_Cir").gameObject;
			this.mRetEffCache = this.transform.Find("CenterItem_Ret").gameObject;
			this.mSqarEffCache = this.transform.Find("CenterItem_Sqar").gameObject;
			this.mRuneEffCache = this.transform.Find("CenterItem_Rune").gameObject;
			this.mExchangeEffCache = this.transform.Find("CenterItem_Exchange").gameObject;
			this.mSummonerExpCache = this.transform.Find("CenterItem_Exp").gameObject;
		}

		public override void HandleAfterOpenView()
		{
			AutoTestController.InvokeTestLogic(AutoTestTag.Home, delegate
			{
				this.onClickContinue(null);
			}, 1f);
		}

		public override void HandleBeforeCloseView()
		{
			base.HandleBeforeCloseView();
			this.mCoinNum.Clear();
			this.mDiamondNum.Clear();
			this.mCapNum.Clear();
			this.mBottleNum.Clear();
			this.mSummonerExp = 0;
			this.mRune.Clear();
			this.mHero.Clear();
			this.mHeroSkin.Clear();
			this.mHeadIcon.Clear();
			this.mHeadFrame.Clear();
			this.mGameItem.Clear();
			this.mCoupon.Clear();
			this.mGameBuff.Clear();
			this.mSpeaker.Clear();
			this.cMgr.StopAllCoroutine();
			if (this.onFinish != null)
			{
				this.onFinish();
				this.onFinish = null;
			}
			else if (Singleton<BattleSettlementView>.Instance.gameObject != null)
			{
				Singleton<BattleSettlementView>.Instance.BackToLobby();
			}
			NewbieManager.Instance.TryHandleGetItemsFinished();
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)26003, new MobaMessageFunc(this.onMsg_GetDiamonds));
			MobaMessageManager.RegistMessage((ClientMsg)26004, new MobaMessageFunc(this.onMsg_GetCaps));
			MobaMessageManager.RegistMessage((ClientMsg)26005, new MobaMessageFunc(this.onMsg_GetHero));
			MobaMessageManager.RegistMessage((ClientMsg)26006, new MobaMessageFunc(this.onMsg_GetHeroSkin));
			MobaMessageManager.RegistMessage((ClientMsg)26007, new MobaMessageFunc(this.onMsg_GetHeadPortrait));
			MobaMessageManager.RegistMessage((ClientMsg)26008, new MobaMessageFunc(this.onMsg_GetPortraitFrame));
			MobaMessageManager.RegistMessage((ClientMsg)26009, new MobaMessageFunc(this.onMsg_GetRune));
			MobaMessageManager.RegistMessage((ClientMsg)26010, new MobaMessageFunc(this.onMsg_GetCoin));
			MobaMessageManager.RegistMessage((ClientMsg)26011, new MobaMessageFunc(this.onMsg_GetBottle));
			MobaMessageManager.RegistMessage((ClientMsg)26012, new MobaMessageFunc(this.onMsg_GetExchange));
			MobaMessageManager.RegistMessage((ClientMsg)26013, new MobaMessageFunc(this.onMsg_GetExp));
			MobaMessageManager.RegistMessage((ClientMsg)26014, new MobaMessageFunc(this.onMsg_GetGameItem));
			MobaMessageManager.RegistMessage((ClientMsg)26015, new MobaMessageFunc(this.onMsg_GetCoupon));
			MobaMessageManager.RegistMessage((ClientMsg)26016, new MobaMessageFunc(this.onMsg_GetGameBuff));
			MobaMessageManager.RegistMessage((ClientMsg)26017, new MobaMessageFunc(this.onMsg_GetSpeaker));
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)26003, new MobaMessageFunc(this.onMsg_GetDiamonds));
			MobaMessageManager.UnRegistMessage((ClientMsg)26004, new MobaMessageFunc(this.onMsg_GetCaps));
			MobaMessageManager.UnRegistMessage((ClientMsg)26005, new MobaMessageFunc(this.onMsg_GetHero));
			MobaMessageManager.UnRegistMessage((ClientMsg)26006, new MobaMessageFunc(this.onMsg_GetHeroSkin));
			MobaMessageManager.UnRegistMessage((ClientMsg)26007, new MobaMessageFunc(this.onMsg_GetHeadPortrait));
			MobaMessageManager.UnRegistMessage((ClientMsg)26008, new MobaMessageFunc(this.onMsg_GetPortraitFrame));
			MobaMessageManager.UnRegistMessage((ClientMsg)26009, new MobaMessageFunc(this.onMsg_GetRune));
			MobaMessageManager.UnRegistMessage((ClientMsg)26010, new MobaMessageFunc(this.onMsg_GetCoin));
			MobaMessageManager.UnRegistMessage((ClientMsg)26011, new MobaMessageFunc(this.onMsg_GetBottle));
			MobaMessageManager.UnRegistMessage((ClientMsg)26012, new MobaMessageFunc(this.onMsg_GetExchange));
			MobaMessageManager.UnRegistMessage((ClientMsg)26013, new MobaMessageFunc(this.onMsg_GetExp));
			MobaMessageManager.UnRegistMessage((ClientMsg)26014, new MobaMessageFunc(this.onMsg_GetGameItem));
			MobaMessageManager.UnRegistMessage((ClientMsg)26015, new MobaMessageFunc(this.onMsg_GetCoupon));
			MobaMessageManager.UnRegistMessage((ClientMsg)26016, new MobaMessageFunc(this.onMsg_GetGameBuff));
			MobaMessageManager.UnRegistMessage((ClientMsg)26017, new MobaMessageFunc(this.onMsg_GetSpeaker));
		}

		private void PlayNext()
		{
			this.cMgr.StopAllCoroutine();
			if (this.mCenterItemAnchor.childCount > 0)
			{
				UnityEngine.Object.Destroy(this.mCenterItemAnchor.GetChild(0).gameObject);
			}
			ItemType nextType = this.GetNextType();
			GameObject gameObject = null;
			Transform transform = null;
			switch (nextType)
			{
			case ItemType.None:
				CtrlManager.CloseWindow(WindowID.GetItemsView);
				return;
			case ItemType.Rune:
			{
				gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mRetEffCache);
				this.tempEffTrans = gameObject.transform;
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				transform = gameObject.transform.FindChild("Fx_GetHeroCard");
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/Heroportrait").gameObject.SetActive(false);
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/RuneBg").gameObject.SetActive(true);
				int num = this.mRune.Dequeue();
				this.CheckQuality(ItemType.Rune, num.ToString());
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(num.ToString());
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/RuneBg/Texture").GetComponent<UISprite>().spriteName = dataById.icon;
				gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_Runes");
				gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.name);
				this.SetLabelColor(gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>(), dataById.quality);
				string[] array = dataById.attribute.Split(new char[]
				{
					'|'
				});
				string text = (!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? float.Parse(array[1]).ToString(array[2]) : array[1];
				if (dataById.rune_type != 2)
				{
					gameObject.transform.FindChild("text/ItemDesc").GetComponent<UILabel>().text = "+" + text + " " + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName);
				}
				else
				{
					gameObject.transform.FindChild("text/ItemDesc").GetComponent<UILabel>().text = string.Concat(new string[]
					{
						"+",
						text,
						" ",
						LanguageManager.Instance.GetStringById("HeroRunsUI_GrowthRuns"),
						LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)
					});
				}
				gameObject.transform.FindChild("text/ItemDesc").gameObject.SetActive(true);
				break;
			}
			case ItemType.Diamond:
			{
				gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mCirEffCache);
				transform = gameObject.transform.FindChild("Fx_GetCap");
				int num2 = this.mDiamondNum.Dequeue();
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				this.CheckQuality(ItemType.Diamond, string.Empty);
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/Num").GetComponent<UILabel>().text = "x" + num2;
				transform.FindChild("GetReward_float/GetReward/GoldenFrame").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>("Get_diamond", true, true, null, 0, false);
				gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Diamond");
				gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Diamond_Desc");
				break;
			}
			case ItemType.Cap:
			{
				gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mCirEffCache);
				transform = gameObject.transform.FindChild("Fx_GetCap");
				int num3 = this.mCapNum.Dequeue();
				this.tempEffTrans = gameObject.transform;
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				this.CheckQuality(ItemType.Cap, string.Empty);
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/Num").GetComponent<UILabel>().text = "x" + num3;
				transform.FindChild("GetReward_float/GetReward/GoldenFrame").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>("Get_Magic_bottle_cap", true, true, null, 0, false);
				gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_MagicBottle");
				gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_MagicBottle_Desc");
				break;
			}
			case ItemType.HeadPortrait:
			{
				gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mSqarEffCache);
				this.tempEffTrans = gameObject.transform;
				transform = gameObject.transform.FindChild("Fx_GetAvatar");
				string text2 = this.mHeadIcon.Dequeue();
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				this.CheckQuality(ItemType.HeadPortrait, text2);
				SysSummonersHeadportraitVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(text2);
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/HeroAvatar").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById2.headportrait_icon, true, true, null, 0, false);
				gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_HeadAvatar");
				gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById2.headportrait_name);
				break;
			}
			case ItemType.Hero:
			{
				gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mRetEffCache);
				transform = gameObject.transform.FindChild("Fx_GetHeroCard");
				string text3 = this.mHero.Dequeue();
				this.tempEffTrans = gameObject.transform;
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				this.CheckQuality(ItemType.Hero, text3);
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(text3);
				if (heroMainData != null)
				{
					transform.FindChild("GetReward_float/GetReward/GoldenFrame/Heroportrait").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(heroMainData.Loading_icon, true, true, null, 0, false);
					gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_Hero");
					gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(heroMainData.name);
				}
				break;
			}
			case ItemType.HeroSkin:
			{
				gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mRetEffCache);
				transform = gameObject.transform.FindChild("Fx_GetHeroCard");
				int num4 = this.mHeroSkin.Dequeue();
				this.tempEffTrans = gameObject.transform;
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				this.CheckQuality(ItemType.HeroSkin, num4.ToString());
				SysHeroSkinVo dataById3 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(num4.ToString());
				if (dataById3 != null)
				{
					transform.FindChild("GetReward_float/GetReward/GoldenFrame/Heroportrait").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById3.Loading_icon, true, true, null, 0, false);
					gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_Skin");
					gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById3.name);
					gameObject.transform.FindChild("text/CharmContainer").GetComponent<GetItems_ShowCharm>().ShowCharmIncrement(dataById3.charm);
				}
				break;
			}
			case ItemType.Coin:
			{
				gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mCirEffCache);
				transform = gameObject.transform.FindChild("Fx_GetCap");
				int num5 = this.mCoinNum.Dequeue();
				this.tempEffTrans = gameObject.transform;
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				this.CheckQuality(ItemType.Coin, string.Empty);
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/Num").GetComponent<UILabel>().text = "x" + num5;
				transform.FindChild("GetReward_float/GetReward/GoldenFrame").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>("Get_coin", true, true, null, 0, false);
				gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Gold");
				gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Gold_Desc");
				break;
			}
			case ItemType.Exchange:
			{
				GetItemsView.ExchangeItemData data = this.mExchangeItem.Dequeue();
				this.cMgr.StartCoroutine(this.ShowExchangeItem(data), true);
				break;
			}
			case ItemType.Bottle:
			{
				gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mCirEffCache);
				transform = gameObject.transform.FindChild("Fx_GetCap");
				int num6 = this.mBottleNum.Dequeue();
				this.tempEffTrans = gameObject.transform;
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				this.CheckQuality(ItemType.Bottle, string.Empty);
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/Num").GetComponent<UILabel>().text = "x" + num6;
				transform.FindChild("GetReward_float/GetReward/GoldenFrame").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>("Get_EXP_bottles", true, true, null, 0, false);
				gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("GameItems_Name_7777");
				gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("GameItems_Describe_7777");
				break;
			}
			case ItemType.Exp:
				this.CheckQuality(ItemType.Exp, string.Empty);
				this.ShowExp();
				break;
			case ItemType.NormalGameItem:
				this.ShowGameItem();
				break;
			case ItemType.Coupon:
				this.ShowCoupon();
				break;
			case ItemType.PortraitFrame:
			{
				gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mSqarEffCache);
				this.tempEffTrans = gameObject.transform;
				transform = gameObject.transform.FindChild("Fx_GetAvatar");
				string text4 = this.mHeadFrame.Dequeue();
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				transform.FindChild("GetReward_float/GetReward/GoldenFrame").GetComponent<UISprite>().spriteName = "Settlement_get_head_frame_0" + this.CheckQuality(ItemType.PortraitFrame, text4);
				transform.FindChild("GetReward_float/GetReward/GoldenFrame").GetComponent<UISprite>().SetDimensions(256, 256);
				SysSummonersPictureframeVo dataById4 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(text4);
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/HeroAvatar").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById4.pictureframe_icon, true, true, null, 0, false);
				gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_PictureFrame");
				gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById4.pictureframe_name);
				break;
			}
			case ItemType.GameBuff:
				this.ShowGameBuff();
				break;
			case ItemType.Speaker:
				this.ShowSpeaker();
				break;
			}
			if (gameObject != null && !gameObject.activeInHierarchy)
			{
				gameObject.SetActive(true);
			}
			if (transform != null)
			{
				transform.GetComponent<EffectPlayTool>().Play();
			}
		}

		[DebuggerHidden]
		private IEnumerator ClickCD(float time)
		{
			GetItemsView.<ClickCD>c__Iterator120 <ClickCD>c__Iterator = new GetItemsView.<ClickCD>c__Iterator120();
			<ClickCD>c__Iterator.time = time;
			<ClickCD>c__Iterator.<$>time = time;
			<ClickCD>c__Iterator.<>f__this = this;
			return <ClickCD>c__Iterator;
		}

		private void SetLabelColor(UILabel label, int quality)
		{
			Color32 c;
			Color32 c2;
			switch (quality)
			{
			case 1:
				c = new Color32(17, 138, 23, 255);
				c2 = new Color32(30, 203, 53, 255);
				break;
			case 2:
				c = new Color32(15, 16, 251, 255);
				c2 = new Color32(35, 211, 253, 255);
				break;
			case 3:
				c = new Color32(126, 0, 99, 255);
				c2 = new Color32(250, 0, 239, 255);
				break;
			case 4:
				c = new Color32(204, 84, 23, 255);
				c2 = new Color32(253, 217, 48, 255);
				break;
			case 5:
				c = new Color32(252, 0, 25, 255);
				c2 = new Color32(253, 121, 33, 255);
				break;
			default:
				c = new Color32(250, 170, 33, 255);
				c2 = new Color32(246, 207, 48, 255);
				break;
			}
			label.gradientTop = c;
			label.gradientBottom = c2;
		}

		public void Play()
		{
			this.mStaticAnchor.gameObject.SetActive(true);
			if (this.GetNextType() != ItemType.None)
			{
				this.cMgr.StartCoroutine(this.Play_IEnumerator(0.1f), true);
			}
			else
			{
				CtrlManager.CloseWindow(WindowID.GetItemsView);
			}
		}

		public void PlayRune(int modelId)
		{
			this.mStaticAnchor.gameObject.SetActive(true);
			GameObject gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mRuneEffCache);
			Transform transform = gameObject.transform.FindChild("Fx_GetFuwen");
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(modelId.ToString());
			if (dataById == null)
			{
				return;
			}
			transform.FindChild("GetReward_float/GetComposition/GoldenFrame/RuneBg/Texture").GetComponent<UISprite>().spriteName = dataById.icon;
			gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_Runes");
			gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.name);
			this.SetLabelColor(gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>(), dataById.quality);
			string[] array = dataById.attribute.Split(new char[]
			{
				'|'
			});
			string text = (!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? float.Parse(array[1]).ToString(array[2]) : array[1];
			if (dataById.rune_type != 2)
			{
				gameObject.transform.FindChild("text/ItemDesc").GetComponent<UILabel>().text = "+" + text + " " + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName);
			}
			else
			{
				gameObject.transform.FindChild("text/ItemDesc").GetComponent<UILabel>().text = string.Concat(new string[]
				{
					"+",
					text,
					" ",
					LanguageManager.Instance.GetStringById("HeroRunsUI_GrowthRuns"),
					LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)
				});
			}
			gameObject.transform.FindChild("text/ItemDesc").gameObject.SetActive(true);
			this.CheckQuality(ItemType.Rune, modelId.ToString());
			gameObject.SetActive(true);
			transform.GetComponent<EffectPlayTool>().Play();
		}

		[DebuggerHidden]
		private IEnumerator Play_IEnumerator(float seconds)
		{
			GetItemsView.<Play_IEnumerator>c__Iterator121 <Play_IEnumerator>c__Iterator = new GetItemsView.<Play_IEnumerator>c__Iterator121();
			<Play_IEnumerator>c__Iterator.seconds = seconds;
			<Play_IEnumerator>c__Iterator.<$>seconds = seconds;
			<Play_IEnumerator>c__Iterator.<>f__this = this;
			return <Play_IEnumerator>c__Iterator;
		}

		private ItemType GetNextType()
		{
			if (this.mSummonerExp > 0)
			{
				return ItemType.Exp;
			}
			if (this.mCoinNum.Count > 0)
			{
				return ItemType.Coin;
			}
			if (this.mSpeaker.Count > 0)
			{
				return ItemType.Speaker;
			}
			if (this.mRune.Count > 0)
			{
				return ItemType.Rune;
			}
			if (this.mBottleNum.Count > 0)
			{
				return ItemType.Bottle;
			}
			if (this.mDiamondNum.Count > 0)
			{
				return ItemType.Diamond;
			}
			if (this.mCapNum.Count > 0)
			{
				return ItemType.Cap;
			}
			if (this.mGameItem.Count > 0)
			{
				return ItemType.NormalGameItem;
			}
			if (this.mExchangeItem.Count > 0)
			{
				return ItemType.Exchange;
			}
			if (this.mHeadIcon.Count > 0)
			{
				return ItemType.HeadPortrait;
			}
			if (this.mHeadFrame.Count > 0)
			{
				return ItemType.PortraitFrame;
			}
			if (this.mCoupon.Count > 0)
			{
				return ItemType.Coupon;
			}
			if (this.mGameBuff.Count > 0)
			{
				return ItemType.GameBuff;
			}
			if (this.mHero.Count > 0)
			{
				return ItemType.Hero;
			}
			if (this.mHeroSkin.Count > 0)
			{
				return ItemType.HeroSkin;
			}
			return ItemType.None;
		}

		private void onClickContinue(GameObject obj = null)
		{
			this.PlayNext();
		}

		private void onMsg_GetSpeaker(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (num > 0)
			{
				this.mSpeaker.Enqueue(num);
			}
		}

		private void onMsg_GetGameBuff(MobaMessage msg)
		{
			int item = (int)msg.Param;
			this.mGameBuff.Enqueue(item);
		}

		private void onMsg_GetCoin(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (num > 0)
			{
				this.mCoinNum.Enqueue(num);
			}
		}

		private void onMsg_GetDiamonds(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (num > 0)
			{
				this.mDiamondNum.Enqueue(num);
			}
		}

		private void onMsg_GetCaps(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (num > 0)
			{
				this.mCapNum.Enqueue(num);
			}
		}

		private void onMsg_GetHero(MobaMessage msg)
		{
			string item = (string)msg.Param;
			this.mHero.Enqueue(item);
		}

		private void onMsg_GetHeroSkin(MobaMessage msg)
		{
			int item = (int)msg.Param;
			this.mHeroSkin.Enqueue(item);
		}

		private void onMsg_GetHeadPortrait(MobaMessage msg)
		{
			int num = (int)msg.Param;
			this.mHeadIcon.Enqueue(num.ToString());
		}

		private void onMsg_GetPortraitFrame(MobaMessage msg)
		{
			string item = (string)msg.Param;
			this.mHeadFrame.Enqueue(item);
		}

		private void onMsg_GetRune(MobaMessage msg)
		{
			int item = (int)msg.Param;
			this.mRune.Enqueue(item);
		}

		private void onMsg_GetBottle(MobaMessage msg)
		{
			int num = (int)msg.Param;
			if (num > 0)
			{
				this.mBottleNum.Enqueue(num);
			}
		}

		private void onMsg_GetExchange(MobaMessage msg)
		{
			object[] array = (object[])msg.Param;
			if (array.Length != 3)
			{
				return;
			}
			ItemType type = (ItemType)((int)array[0]);
			string id = (string)array[1];
			bool writeInModel = (bool)array[2];
			this.mExchangeItem.Enqueue(new GetItemsView.ExchangeItemData(type, id, writeInModel));
		}

		private void onMsg_GetExp(MobaMessage msg)
		{
			object[] array = (object[])msg.Param;
			if (array.Length >= 1)
			{
				this.mSummonerExp = (int)array[0];
			}
			if (array.Length >= 2)
			{
				this.mSummonerExp_from = (long)array[1];
			}
		}

		private void onMsg_GetGameItem(MobaMessage msg)
		{
			string item = (string)msg.Param;
			this.mGameItem.Enqueue(item);
		}

		private void onMsg_GetCoupon(MobaMessage msg)
		{
			string item = (string)msg.Param;
			this.mCoupon.Enqueue(item);
		}

		private int CheckQuality(ItemType _type, string _id = "")
		{
			int num = 1;
			switch (_type)
			{
			case ItemType.Rune:
			case ItemType.Bottle:
			case ItemType.NormalGameItem:
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(_id);
				if (dataById != null)
				{
					num = dataById.quality;
				}
				break;
			}
			case ItemType.HeadPortrait:
			{
				SysSummonersHeadportraitVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(_id);
				if (dataById2 != null)
				{
					num = dataById2.headportrait_quality;
				}
				break;
			}
			case ItemType.Hero:
			{
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(_id);
				if (heroMainData != null)
				{
					num = heroMainData.quality;
				}
				break;
			}
			case ItemType.HeroSkin:
			{
				SysHeroSkinVo dataById3 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(_id);
				if (dataById3 != null)
				{
					num = dataById3.quality;
				}
				break;
			}
			case ItemType.Coupon:
			{
				SysCouponVo dataById4 = BaseDataMgr.instance.GetDataById<SysCouponVo>(_id);
				if (dataById4 != null && dataById4.off_number < 5)
				{
					num = 4;
				}
				else
				{
					num = 3;
				}
				break;
			}
			case ItemType.PortraitFrame:
			{
				SysSummonersPictureframeVo dataById5 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(_id);
				if (dataById5 != null)
				{
					num = dataById5.pictureframe_quality;
				}
				break;
			}
			case ItemType.GameBuff:
			{
				SysGameBuffVo dataById6 = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(_id);
				if (dataById6 != null)
				{
					num = dataById6.quality;
				}
				break;
			}
			case ItemType.Speaker:
				num = 3;
				break;
			}
			int result = num;
			switch (num)
			{
			case 2:
				AudioMgr.PlayUI("Play_UI_Reward", null, false, false);
				this.cMgr.StartCoroutine(this.ClickCD(0.8f), true);
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_default", true, true, null, 0, false);
				break;
			case 3:
				AudioMgr.PlayUI("Play_UI_Reward_L1", null, false, false);
				this.cMgr.StartCoroutine(this.ClickCD(1f), true);
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_purple", true, true, null, 0, false);
				break;
			case 4:
				AudioMgr.PlayUI("Play_UI_Reward_L2", null, false, false);
				this.cMgr.StartCoroutine(this.ClickCD(1.2f), true);
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_gold", true, true, null, 0, false);
				break;
			case 5:
				AudioMgr.PlayUI("Play_UI_Reward_L3", null, false, false);
				this.cMgr.StartCoroutine(this.ClickCD(1.5f), true);
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_red", true, true, null, 0, false);
				break;
			default:
				AudioMgr.PlayUI("Play_UI_Reward", null, false, false);
				num = 2;
				this.cMgr.StartCoroutine(this.ClickCD(0.6f), true);
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_default", true, true, null, 0, false);
				break;
			}
			Transform transform = null;
			if (this.tempEffTrans != null)
			{
				transform = this.tempEffTrans.FindChild("Fx_" + num);
			}
			if (transform != null)
			{
				transform.gameObject.SetActive(true);
				transform.GetComponent<EffectPlayTool>().Play();
				this.tempEffTrans = null;
			}
			this.mShineTween.gameObject.SetActive(true);
			this.mShineTween.ResetToBeginning();
			this.mShineTween.PlayForward();
			return result;
		}

		private void ShowExp()
		{
			GameObject gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mSummonerExpCache);
			gameObject.SetActive(true);
			gameObject.transform.GetComponent<Settlement_Summoner>().Show(this.mSummonerExp, this.mSummonerExp_from);
			this.mSummonerExp = 0;
			this.mSummonerExp_from = 0L;
		}

		[DebuggerHidden]
		private IEnumerator ShowExchangeItem(GetItemsView.ExchangeItemData data)
		{
			GetItemsView.<ShowExchangeItem>c__Iterator122 <ShowExchangeItem>c__Iterator = new GetItemsView.<ShowExchangeItem>c__Iterator122();
			<ShowExchangeItem>c__Iterator.data = data;
			<ShowExchangeItem>c__Iterator.<$>data = data;
			<ShowExchangeItem>c__Iterator.<>f__this = this;
			return <ShowExchangeItem>c__Iterator;
		}

		private void ShowGameItem()
		{
			GameObject gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mRetEffCache);
			Transform transform = gameObject.transform.FindChild("Fx_GetHeroCard");
			transform.FindChild("GetReward_float/GetReward/GoldenFrame/Heroportrait").gameObject.SetActive(true);
			transform.FindChild("GetReward_float/GetReward/GoldenFrame/RuneBg").gameObject.SetActive(false);
			string text = this.mGameItem.Dequeue();
			this.tempEffTrans = gameObject.transform;
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			this.CheckQuality(ItemType.NormalGameItem, text);
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(text.ToString());
			if (dataById == null)
			{
				ClientLogger.Error("modelId error: " + text);
				return;
			}
			transform.FindChild("GetReward_float/GetReward/GoldenFrame/Heroportrait").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById.Long_icon, true, true, null, 0, false);
			if (dataById.type == 10)
			{
				gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.role);
			}
			else
			{
				gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("Currency_Items");
			}
			gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.name);
			this.SetLabelColor(gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>(), dataById.quality);
			if (dataById.attribute != "[]")
			{
				string[] array = dataById.attribute.Split(new char[]
				{
					'|'
				});
				string text2 = (!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? float.Parse(array[1]).ToString(array[2]) : array[1];
				if (dataById.rune_type != 2)
				{
					gameObject.transform.FindChild("text/ItemDesc").GetComponent<UILabel>().text = "+" + text2 + " " + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName);
				}
				else
				{
					gameObject.transform.FindChild("text/ItemDesc").GetComponent<UILabel>().text = string.Concat(new string[]
					{
						"+",
						text2,
						" ",
						LanguageManager.Instance.GetStringById("HeroRunsUI_GrowthRuns"),
						LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)
					});
				}
				gameObject.transform.FindChild("text/ItemDesc").gameObject.SetActive(true);
			}
			if (gameObject != null)
			{
				gameObject.SetActive(true);
				gameObject.transform.FindChild("text/CharmContainer").GetComponent<GetItems_ShowCharm>().ShowCharmIncrement(dataById.charm);
			}
			if (transform != null)
			{
				transform.GetComponent<EffectPlayTool>().Play();
			}
		}

		private void ShowCoupon()
		{
			GameObject gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mRetEffCache);
			Transform transform = gameObject.transform.FindChild("Fx_GetHeroCard");
			string text = this.mCoupon.Dequeue();
			this.tempEffTrans = gameObject.transform;
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			this.CheckQuality(ItemType.Coupon, text);
			SysCouponVo dataById = BaseDataMgr.instance.GetDataById<SysCouponVo>(text);
			if (dataById == null)
			{
				return;
			}
			Transform transform2;
			if (dataById.off_number == 3)
			{
				transform2 = transform.FindChild("GetReward_float/GetReward/3Coupon");
			}
			else
			{
				transform2 = transform.FindChild("GetReward_float/GetReward/7Coupon");
			}
			transform2.gameObject.SetActive(true);
			if (dataById.currency_type == 1)
			{
				transform2.FindChild("Tip").GetComponent<UILabel>().text = "金币折扣券";
			}
			else if (dataById.currency_type == 2)
			{
				transform2.FindChild("Tip").GetComponent<UILabel>().text = "钻石折扣券";
			}
			else if (dataById.currency_type == 9)
			{
				transform2.FindChild("Tip").GetComponent<UILabel>().text = "瓶盖折扣券";
			}
			gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = "打折卡";
			gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.name);
			if (dataById.mother_type == 1)
			{
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(dataById.mother_id);
				if (heroMainData == null)
				{
					return;
				}
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/Heroportrait").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(heroMainData.Loading_icon, true, true, null, 0, false);
			}
			else if (dataById.mother_type == 2)
			{
				SysHeroSkinVo dataById2 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(dataById.mother_id);
				if (dataById2 == null)
				{
					return;
				}
				transform.FindChild("GetReward_float/GetReward/GoldenFrame/Heroportrait").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById2.Loading_icon, true, true, null, 0, false);
			}
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			if (transform != null)
			{
				transform.GetComponent<EffectPlayTool>().Play();
			}
		}

		private void ShowGameBuff()
		{
			GameObject gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mRetEffCache);
			Transform transform = gameObject.transform.FindChild("Fx_GetHeroCard");
			transform.FindChild("GetReward_float/GetReward/GoldenFrame/Heroportrait").gameObject.SetActive(true);
			transform.FindChild("GetReward_float/GetReward/GoldenFrame/RuneBg").gameObject.SetActive(false);
			int num = this.mGameBuff.Dequeue();
			this.tempEffTrans = gameObject.transform;
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			this.CheckQuality(ItemType.GameBuff, num.ToString());
			SysGameBuffVo dataById = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(num.ToString());
			if (dataById == null)
			{
				ClientLogger.Error("modelId error: " + num);
				return;
			}
			transform.FindChild("GetReward_float/GetReward/GoldenFrame/Heroportrait").GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById.long_icon, true, true, null, 0, false);
			gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = "双倍卡";
			gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.name);
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			if (transform != null)
			{
				transform.GetComponent<EffectPlayTool>().Play();
			}
		}

		private void ShowSpeaker()
		{
			GameObject gameObject = NGUITools.AddChild(this.mCenterItemAnchor.gameObject, this.mCirEffCache);
			Transform transform = gameObject.transform.FindChild("Fx_GetCap");
			SysCurrencyVo dataById = BaseDataMgr.instance.GetDataById<SysCurrencyVo>("11");
			int num = this.mSpeaker.Dequeue();
			this.tempEffTrans = gameObject.transform;
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			this.CheckQuality(ItemType.Speaker, string.Empty);
			transform.FindChild("GetReward_float/GetReward/GoldenFrame/Num").GetComponent<UILabel>().text = "x" + num;
			transform.FindChild("GetReward_float/GetReward/GoldenFrame").GetComponent<UITexture>().mainTexture = Resources.Load<Texture>("Texture/Equipment/Get_little_trumpet");
			gameObject.transform.FindChild("text/ItemClass").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.name);
			gameObject.transform.FindChild("text/ItemName").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(dataById.description);
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			if (transform != null)
			{
				transform.GetComponent<EffectPlayTool>().Play();
			}
		}
	}
}
