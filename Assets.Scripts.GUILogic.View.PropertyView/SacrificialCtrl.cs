using Assets.Scripts.GUILogic.View.Runes;
using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class SacrificialCtrl
	{
		private static SacrificialCtrl instance;

		private static object obj_lock = new object();

		public CollectionState collectionState;

		private object[] mgs;

		private string heroNPC = string.Empty;

		private int skinID;

		private GameObject smdoel;

		public GameObject Smodel
		{
			get
			{
				return this.Smodel;
			}
		}

		public string HeroNPC
		{
			get
			{
				return this.heroNPC;
			}
		}

		private SacrificialCtrl()
		{
		}

		public static SacrificialCtrl GetInstance()
		{
			if (SacrificialCtrl.instance == null)
			{
				object obj = SacrificialCtrl.obj_lock;
				lock (obj)
				{
					if (SacrificialCtrl.instance == null)
					{
						SacrificialCtrl.instance = new SacrificialCtrl();
						return SacrificialCtrl.instance;
					}
				}
			}
			return SacrificialCtrl.instance;
		}

		public void Init()
		{
			this.mgs = new object[]
			{
				ClientC2V.OpenSacrificial,
				ClientC2V.OpenProperty,
				ClientC2V.CloseSynthesis,
				ClientV2C.sacriviewChangeHero,
				ClientV2C.propviewReplaceSkin,
				ClientV2C.propviewBuySkinSuccess,
				ClientV2C.propviewInlayRunes,
				ClientV2C.propviewDemountRunes,
				ClientV2C.propviewDemountAll,
				ClientV2C.propviewUseEffectItem,
				ClientV2C.propviewSynthesisRunes
			};
			this.Register();
		}

		public void UnInit()
		{
			this.UnRegister();
			this.collectionState = CollectionState.Nothing;
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void UnRegister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void OnMsg_OpenSacrificial(MobaMessage msg)
		{
			if (msg != null)
			{
				CtrlManager.OpenWindow(WindowID.SacrificialView, null);
				CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			}
		}

		private void OnMsg_OpenProperty(MobaMessage msg)
		{
			if (msg != null)
			{
				string param = string.Empty;
				param = (string)msg.Param;
				this.heroNPC = param;
				GL.Clear(false, true, Color.black);
				CtrlManager.OpenWindow(WindowID.PropertyView, null);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.sacriviewChangeHero, param, false);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInitToggle, PropertyType.Info, false);
				CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			}
		}

		private void OnMsg_sacriviewChangeHero(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string text = string.Empty;
				text = (string)msg.Param;
				this.heroNPC = text;
			}
		}

		private void OnMsg_CloseSynthesis(MobaMessage msg)
		{
			if (msg != null)
			{
				CtrlManager.CloseWindow(WindowID.SynthesisPopupView);
			}
		}

		private void OnMsg_propviewReplaceSkin(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				int num = (int)msg.Param;
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.heroNPC);
				if (heroInfoData == null)
				{
					Singleton<TipView>.Instance.ShowViewSetText("请先购买该英雄!!!", 1f);
					return;
				}
				long heroId = heroInfoData.HeroId;
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "正在穿戴...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.ChangeSkin, param, new object[]
				{
					heroId.ToString(),
					num.ToString()
				});
			}
		}

		private void OnMsg_propviewBuySkinSuccess(MobaMessage msg)
		{
			if (msg != null)
			{
				long summonerId = ModelManager.Instance.GetData<UserData>(EModelType.Model_userData).SummonerId;
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "正在获取皮肤...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.GetSummSkinList, param, new object[]
				{
					summonerId
				});
			}
		}

		private void OnMsg_propviewInlayRunes(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string text = string.Empty;
				long num = 0L;
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.heroNPC);
				if (heroInfoData != null)
				{
					num = heroInfoData.HeroId;
				}
				RunesOperaInfo runesOperaInfo = default(RunesOperaInfo);
				runesOperaInfo = (RunesOperaInfo)msg.Param;
				if (runesOperaInfo.runesPosition <= 0)
				{
					return;
				}
				text = runesOperaInfo.equipID;
				int runesPosition = runesOperaInfo.runesPosition;
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在镶嵌符文...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.UsingEquipment, param, new object[]
				{
					text,
					runesPosition.ToString(),
					num.ToString()
				});
			}
		}

		private void OnMsg_propviewDemountRunes(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string text = string.Empty;
				long num = 0L;
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.heroNPC);
				if (heroInfoData != null)
				{
					num = heroInfoData.HeroId;
				}
				RunesOperaInfo runesOperaInfo = default(RunesOperaInfo);
				runesOperaInfo = (RunesOperaInfo)msg.Param;
				if (runesOperaInfo.runesPosition <= 0)
				{
					return;
				}
				text = runesOperaInfo.modelID;
				int runesPosition = runesOperaInfo.runesPosition;
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在卸下符文...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.DischargeRune, param, new object[]
				{
					text,
					runesPosition.ToString(),
					num.ToString(),
					1
				});
			}
		}

		private void OnMsg_propviewDemountAll(MobaMessage msg)
		{
			if (msg != null)
			{
				if (ModelManager.Instance.IsRunesEquipNull(this.heroNPC))
				{
					Singleton<TipView>.Instance.ShowViewSetText("没有可卸下的符文", 1f);
					return;
				}
				string text = "0";
				int num = 0;
				long num2 = 0L;
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.heroNPC);
				if (heroInfoData != null)
				{
					num2 = heroInfoData.HeroId;
				}
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在卸下符文...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.DischargeRune, param, new object[]
				{
					text,
					num.ToString(),
					num2.ToString(),
					2
				});
			}
		}

		private void OnMsg_propviewSynthesisRunes(MobaMessage msg)
		{
		}

		private void OnMsg_propviewUseEffectItem(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				int[] array = (int[])msg.Param;
				if (array.Count<int>() != 2)
				{
					return;
				}
				long num = 0L;
				if (!string.IsNullOrEmpty(this.heroNPC))
				{
					HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.heroNPC);
					if (heroInfoData != null)
					{
						num = heroInfoData.HeroId;
					}
				}
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在使用道具...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.WearPrivateEffect, param, new object[]
				{
					num,
					array[0],
					array[1]
				});
			}
		}
	}
}
