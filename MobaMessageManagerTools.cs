using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaMessageData;
using MobaProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class MobaMessageManagerTools
{
	private static Dictionary<int, List<string>> dicPvpCode;

	static MobaMessageManagerTools()
	{
		MobaMessageManagerTools.InitPVPCode();
	}

	public static void RegistMsg(object obj, IEnumerable listMsg, bool regist = true, string prefix = "OnMsg_")
	{
		if (obj == null || listMsg == null)
		{
			return;
		}
		Type type = obj.GetType();
		Type typeFromHandle = typeof(MobaMessageManager);
		string text = (!regist) ? "UnRegistMessage" : "RegistMessage";
		IEnumerator enumerator = listMsg.GetEnumerator();
		while (enumerator.MoveNext())
		{
			object current = enumerator.Current;
			if (current != null)
			{
				List<string> list = new List<string>();
				MethodInfo methodInfo = null;
				if (current is PvpCode && MobaMessageManagerTools.dicPvpCode.ContainsKey(current.GetHashCode()))
				{
					list.AddRange(MobaMessageManagerTools.dicPvpCode[current.GetHashCode()]);
				}
				else
				{
					list.Add(current.ToString());
				}
				for (int i = 0; i < list.Count; i++)
				{
					methodInfo = type.GetMethod(prefix + list[i], BindingFlags.Instance | BindingFlags.NonPublic);
					if (methodInfo != null)
					{
						break;
					}
				}
				if (methodInfo == null)
				{
					ClientLogger.Error("RegistMsg:Get handler methodInfo " + current.ToString() + " failed");
				}
				else
				{
					Type type2 = current.GetType();
					Delegate @delegate = Delegate.CreateDelegate(typeof(MobaMessageFunc), obj, methodInfo, false);
					if (@delegate == null)
					{
						ClientLogger.Error("RegistMsg:Create handler delegate" + list + " failed");
					}
					else
					{
						MethodInfo method = typeFromHandle.GetMethod(text, BindingFlags.Static | BindingFlags.Public, null, new Type[]
						{
							type2,
							typeof(MobaMessageFunc)
						}, null);
						if (method == null)
						{
							ClientLogger.Error("RegistMsg:Get Regist/Unregist MethodInfo " + text + " failed");
						}
						else
						{
							method.Invoke(null, new object[]
							{
								current,
								(MobaMessageFunc)@delegate
							});
						}
					}
				}
			}
		}
	}

	private static void InitPVPCode()
	{
		if (MobaMessageManagerTools.dicPvpCode == null)
		{
			MobaMessageManagerTools.dicPvpCode = new Dictionary<int, List<string>>();
			Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
			string[] names = Enum.GetNames(typeof(PvpCode));
			for (int i = 0; i < names.Length; i++)
			{
				PvpCode pvpCode = (PvpCode)((byte)Enum.Parse(typeof(PvpCode), names[i]));
				int key = (int)pvpCode;
				if (dictionary.ContainsKey(key))
				{
					dictionary[key].Add(names[i]);
				}
				else
				{
					dictionary.Add((int)pvpCode, new List<string>());
					dictionary[key].Add(names[i]);
				}
			}
			Dictionary<int, List<string>>.Enumerator enumerator = dictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, List<string>> current = enumerator.Current;
				if (current.Value.Count > 1)
				{
					Dictionary<int, List<string>> arg_E4_0 = MobaMessageManagerTools.dicPvpCode;
					KeyValuePair<int, List<string>> current2 = enumerator.Current;
					int arg_E4_1 = current2.Key;
					KeyValuePair<int, List<string>> current3 = enumerator.Current;
					arg_E4_0.Add(arg_E4_1, current3.Value);
				}
			}
		}
	}

	public static void SendClientMsg(object code, object param = null, bool dispatch = false)
	{
		ClientMsg msgID = (ClientMsg)((int)code);
		MobaMessage message = MobaMessageManager.GetMessage(msgID, param, 0f);
		if (dispatch)
		{
			MobaMessageManager.DispatchMsg(message);
		}
		else
		{
			MobaMessageManager.ExecuteMsg(message);
		}
	}

	public static void BeginWaiting_manual(string key, string text = "", bool bNormal = true, float time = 15f, bool delayShow = true)
	{
		WaitingViewMng.Instance.Waiting(key, text, bNormal, time, delayShow);
	}

	public static void EndWaiting_manual(string key)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2V.WaitingView_manual, key, false);
	}

	public static void BattleShop_recommendToggle(bool isOn)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.RecommendEquipToggle, isOn, false);
	}

	public static void BattleShop_detailedShopToggle(bool isOn)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.DetailedShopToggle, isOn, false);
	}

	public static void BattleShop_openBattleShop(EBattleShopType shopType, EBattleShopOpenType openType)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_openShop, new object[]
		{
			shopType,
			openType
		}, false);
	}

	public static void BattleShop_setCurMenu(object menu)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_setCurMenu, menu, false);
	}

	public static void BattleShop_setCurShopItem(object curshopItem)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_setCurShopItem, curshopItem, true);
	}

	public static void BattleShop_setCurPossessItem(object obj)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_setCurPossessItem, obj, false);
	}

	public static void BattleShop_setMoney(object obj)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_setMoney, obj, false);
	}

	public static void BattleShop_initData(bool reBack, EBattleShopContex e, string levelID, EBattleShopType teamType, bool bQuickR)
	{
		MsgData_BattleShop_initData param = new MsgData_BattleShop_initData
		{
			reBack = reBack,
			eBattleType = e,
			levelID = levelID,
			teamType = teamType,
			enableQuickR = bQuickR
		};
		MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_initData, param, false);
	}

	public static void BattleShop_clearData()
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2C.BattleShop_clearData, null, false);
	}

	public static void Settle_showSummonerExp()
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23030, null, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void Settle_showRank()
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23035, null, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void Settle_showPve()
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23031, null, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void Settle_showNormal(object isVictory)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23032, isVictory, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void Settle_showAchievement(object _achiList)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23037, _achiList, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void Settle_showChaosInfo()
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23036, null, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void Settle_showInfo(object battleType)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23033, battleType, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void Settle_showSurprise()
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23034, null, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void Settle_showCommon(object isVictory)
	{
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23029, isVictory, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	public static void GetItems_Speaker(int count)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetSpeaker, count, false);
	}

	public static void GetItems_GameBuff(int modelId)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetGameBuff, modelId, false);
	}

	public static void GetItems_Coupon(string modelId)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetCoupon, modelId, false);
	}

	public static void GetItems_GameItem(string modelId)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetGameItem, modelId, false);
	}

	public static void GetItems_Bottle(int num)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetBottle, num, false);
	}

	public static void GetItems_Coin(int num)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetCoin, num, false);
	}

	public static void GetItems_Diamond(int num)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetDiamonds, num, false);
	}

	public static void GetItems_Caps(int num)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetCaps, num, false);
	}

	public static void GetItems_Hero(string modelId)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetHero, modelId, false);
	}

	public static void GetItems_HeroSkin(int skinId)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetHeroSkin, skinId, false);
	}

	public static void GetItems_Rune(int runeId)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetRune, runeId, false);
	}

	public static void GetItems_HeadPortrait(int headPorId)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetHeadPortrait, headPorId, false);
	}

	public static void GetItems_PortraitFrame(string frameId)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetPortraitFrame, frameId, false);
	}

	public static void GetItems_Exchange(Com.Game.Module.ItemType _type, string _id, bool _writeInModel = true)
	{
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetExchange, new object[]
		{
			_type,
			_id,
			_writeInModel
		}, false);
	}

	public static void GetItems_Exp(int expDelta, long expFrom = 0L)
	{
		object[] param = new object[]
		{
			expDelta,
			expFrom
		};
		MobaMessageManagerTools.SendClientMsg(ClientV2V.GetExp, param, false);
	}

	public static void VedioPlay_Vedio_setActive(int id, bool active = true)
	{
		MsgData_Vedio_setActive param = new MsgData_Vedio_setActive(id, active);
		MobaMessageManagerTools.SendClientMsg(ClientC2C.Vedio_setActive, param, false);
	}

	public static void VedioPlay_Vedio_creatPlayer(int id, bool create = true)
	{
		MsgData_Vedio_creatPlayer param = new MsgData_Vedio_creatPlayer(id, create);
		MobaMessageManagerTools.SendClientMsg(ClientC2C.Vedio_createPlayer, param, false);
	}

	public static void VedioPlay_Vedio_setName(int id, string name)
	{
		MsgData_Vedio_setName param = new MsgData_Vedio_setName(id, name);
		MobaMessageManagerTools.SendClientMsg(ClientC2C.Vedio_setResource, param, false);
	}

	public static void VedioPlay_Vedio_play(int id)
	{
		MsgData_Vedio_play param = new MsgData_Vedio_play(id);
		MobaMessageManagerTools.SendClientMsg(ClientC2C.Vedio_play, param, false);
	}

	public static void VedioPlay_Vedio_loop(int id, bool loop)
	{
		MsgData_Vedio_loop param = new MsgData_Vedio_loop(id, loop);
		MobaMessageManagerTools.SendClientMsg(ClientC2C.Vedio_loop, param, false);
	}

	public static void VedioPlay_Vedio_stop(int id)
	{
		MsgData_Vedio_stop param = new MsgData_Vedio_stop(id);
		MobaMessageManagerTools.SendClientMsg(ClientC2C.Vedio_stop, param, false);
	}

	public static void LoadView2_setText2(string str, bool dispatch = false)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2V.LoadView_setText2, str, dispatch);
	}

	public static void LoadView2_setProgress2(int sub, int total, int cur, string str = null, MsgData_LoadView2_setProgress.SetType e = MsgData_LoadView2_setProgress.SetType.targetNum)
	{
		MsgData_LoadView2_setProgress param = new MsgData_LoadView2_setProgress(e, sub, total, cur, str);
		MobaMessageManagerTools.SendClientMsg(ClientC2V.LoadView_setProgress2, param, false);
	}

	public static void BgView_setBgViewBg(string bgPathName)
	{
		MobaMessageManagerTools.SendClientMsg(ClientC2V.BgView_setBg, bgPathName, false);
	}
}
