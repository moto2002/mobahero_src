using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Model
{
	internal class Model_heroInfoList : ModelBase<List<HeroInfoData>>
	{
		private List<MobaGameCode> listCode = new List<MobaGameCode>();

		public Model_heroInfoList()
		{
			base.Init(EModelType.Model_heroInfoList);
		}

		public override void RegisterMsgHandler()
		{
			this.listCode = new List<MobaGameCode>
			{
				MobaGameCode.GetHeroList,
				MobaGameCode.UseSoulstone,
				MobaGameCode.UseProps,
				MobaGameCode.UsingSkillPoint,
				MobaGameCode.UploadArenaAtc,
				MobaGameCode.LuckyDraw,
				MobaGameCode.Enchant,
				MobaGameCode.HeroAdvance,
				MobaGameCode.UploadFightResult,
				MobaGameCode.UsingEquipment,
				MobaGameCode.DischargeRune,
				MobaGameCode.BuyShopGoodsNew,
				MobaGameCode.ChangeSkin,
				MobaGameCode.WearPrivateEffect,
				MobaGameCode.SignDay,
				MobaGameCode.ReceiveMailAttachment
			};
			foreach (MobaGameCode current in this.listCode)
			{
				MVC_MessageManager.AddListener_model(current, new MobaMessageFunc(this.OnGetMsg));
			}
		}

		public override void UnRegisterMsgHandler()
		{
			foreach (MobaGameCode current in this.listCode)
			{
				MVC_MessageManager.RemoveListener_model(current, new MobaMessageFunc(this.OnGetMsg));
			}
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			int num = 0;
			MobaMessageType mobaMessageType = MVC_MessageManager.ClientMsg_to_RawCode((ClientMsg)msg.ID, out num);
			if (this.listCode.Contains((MobaGameCode)num))
			{
				string name = "OnGetMsg_" + mobaMessageType.ToString() + "_" + ((MobaGameCode)num).ToString();
				MethodInfo method = base.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					object[] parameters = new object[]
					{
						msg
					};
					method.Invoke(this, parameters);
					base.TriggerListners();
				}
			}
		}

		private bool PreHandel(MobaMessage msg, out OperationResponse res)
		{
			res = null;
			if (msg == null)
			{
				return false;
			}
			res = (msg.Param as OperationResponse);
			return res != null;
		}

		private void OnGetMsg_GameCode_GetHeroList(MobaMessage msg)
		{
			base.Deserialize(88, msg);
			base.TriggerListners();
		}

		private void OnGetMsg_GameCode_UseSoulstone(MobaMessage msg)
		{
			base.LastError = 505;
			List<HeroInfoData> list = base.Data as List<HeroInfoData>;
			if (list == null)
			{
				list = new List<HeroInfoData>();
				base.Data = list;
			}
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				byte[] buffer = operationResponse.Parameters[88] as byte[];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						base.DebugMessage = "===>MobaGameClientPeer:TryUseSoulstonesResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>灵魂石使用失败，钱不够！！！";
					}
				}
				else
				{
					HeroInfoData data = SerializeHelper.Deserialize<HeroInfoData>(buffer);
					bool check = false;
					list.ForEach(delegate(HeroInfoData obj)
					{
						if (obj.HeroId == data.HeroId)
						{
							obj = data;
							check = true;
						}
					});
					if (!check)
					{
						list.Add(data);
					}
					else
					{
						HeroInfoData heroInfoData = list.Find((HeroInfoData obj) => obj.HeroId == data.HeroId);
						if (heroInfoData != null)
						{
							heroInfoData.Level++;
						}
					}
					base.DebugMessage = "===>灵魂石使用成功，ModelId：" + data.ModelId;
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private void OnGetMsg_GameCode_UseProps(MobaMessage msg)
		{
		}

		private void OnGetMsg_GameCode_UsingSkillPoint(MobaMessage msg)
		{
			base.LastError = 505;
			List<HeroInfoData> list = base.Data as List<HeroInfoData>;
			if (list == null)
			{
				list = new List<HeroInfoData>();
				base.Data = list;
			}
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						base.DebugMessage = "===>TryUsingSkillPointResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>TryUsingSkillPointResponse->金币不足！";
					}
				}
				else
				{
					string s = operationResponse.Parameters[89] as string;
					int num = (int)operationResponse.Parameters[114];
					int num2 = (int)operationResponse.Parameters[101];
					long hid = long.Parse(s);
					HeroInfoData heroInfoData = list.Find((HeroInfoData obj) => obj.HeroId == hid);
					if (heroInfoData != null)
					{
						heroInfoData.SkillList[num - 1].Level += num2;
					}
					base.DebugMessage = "===>TryUsingSkillPointResponse" + operationResponse.OperationCode;
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private void OnGetMsg_GameCode_UploadArenaAtc(MobaMessage msg)
		{
			base.LastError = 505;
			List<HeroInfoData> list = base.Data as List<HeroInfoData>;
			if (list == null)
			{
				list = new List<HeroInfoData>();
				base.Data = list;
			}
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryUploadArenaAtcResultResponse" + operationResponse.OperationCode;
				}
				else
				{
					Model_heroInfoList.<OnGetMsg_GameCode_UploadArenaAtc>c__AnonStorey2EA <OnGetMsg_GameCode_UploadArenaAtc>c__AnonStorey2EA = new Model_heroInfoList.<OnGetMsg_GameCode_UploadArenaAtc>c__AnonStorey2EA();
					long num = (long)operationResponse.Parameters[68];
					<OnGetMsg_GameCode_UploadArenaAtc>c__AnonStorey2EA.reqHeroExp = (operationResponse.Parameters[161] as long[]);
					string[] array = operationResponse.Parameters[88] as string[];
					<OnGetMsg_GameCode_UploadArenaAtc>c__AnonStorey2EA.index = 0;
					string[] array2 = array;
					string str;
					for (int i = 0; i < array2.Length; i++)
					{
						str = array2[i];
						list.ForEach(delegate(HeroInfoData obj)
						{
							if (obj.HeroId == long.Parse(str))
							{
								obj.Exp += <OnGetMsg_GameCode_UploadArenaAtc>c__AnonStorey2EA.reqHeroExp[<OnGetMsg_GameCode_UploadArenaAtc>c__AnonStorey2EA.index];
							}
						});
						<OnGetMsg_GameCode_UploadArenaAtc>c__AnonStorey2EA.index++;
					}
					base.DebugMessage = "===>TryUploadArenaAtcResultResponse->上报成功，当前名次:" + num;
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private void OnGetMsg_GameCode_LuckyDraw(MobaMessage msg)
		{
			base.LastError = 505;
			if (!(base.Data is List<HeroInfoData>))
			{
				List<HeroInfoData> data = new List<HeroInfoData>();
				base.Data = data;
			}
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryLuckyDrawResponse" + operationResponse.OperationCode;
				}
				else
				{
					byte[] buffer = operationResponse.Parameters[88] as byte[];
					List<HeroInfoData> collection = SerializeHelper.Deserialize<List<HeroInfoData>>(buffer);
					List<HeroInfoData> data = new List<HeroInfoData>(collection);
					base.Data = data;
					base.DebugMessage = "===>TryLuckyDrawResponse->抽奖成功！";
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private void OnGetMsg_GameCode_Enchant(MobaMessage msg)
		{
			base.LastError = 505;
			List<HeroInfoData> list = base.Data as List<HeroInfoData>;
			if (list == null)
			{
				list = new List<HeroInfoData>();
				base.Data = list;
			}
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						base.DebugMessage = "===>TryEnchantEquipmentResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>>MobaErrorCode.MoneyShortage->金钱不足！";
					}
				}
				else
				{
					long heroId = (long)operationResponse.Parameters[89];
					string epMagic = operationResponse.Parameters[153] as string;
					list.ForEach(delegate(HeroInfoData obj)
					{
						if (obj.HeroId == heroId)
						{
							obj.EpMagic = epMagic;
						}
					});
					base.DebugMessage = "===>>装备附魔成功！";
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private void OnGetMsg_GameCode_HeroAdvance(MobaMessage msg)
		{
			base.LastError = 505;
			List<HeroInfoData> list = base.Data as List<HeroInfoData>;
			if (list == null)
			{
				list = new List<HeroInfoData>();
				base.Data = list;
			}
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>HeroAdvanceResponse" + operationResponse.OperationCode;
				}
				else
				{
					string value = operationResponse.Parameters[89] as string;
					string text = operationResponse.Parameters[114] as string;
					long id = Convert.ToInt64(value);
					HeroInfoData heroInfoData = list.Find((HeroInfoData obj) => obj.HeroId == id);
					if (heroInfoData == null)
					{
						base.DebugMessage = "===>数据有误！";
					}
					else
					{
						heroInfoData.Grade++;
						heroInfoData.Ep_1 = 0;
						heroInfoData.Ep_2 = 0;
						heroInfoData.Ep_3 = 0;
						heroInfoData.Ep_4 = 0;
						heroInfoData.Ep_5 = 0;
						heroInfoData.Ep_6 = 0;
						heroInfoData.EpMagic = "0|0|0|0|0|0";
						if (!string.IsNullOrEmpty(text))
						{
							heroInfoData.SkillList.Add(new SkillModel
							{
								Level = 1,
								SkillId = text
							});
						}
						base.DebugMessage = "===>MobaGameClientPeer:HeroAdvanceResponse:进阶成功!";
					}
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private void OnGetMsg_GameCode_UploadFightResult(MobaMessage msg)
		{
			base.LastError = 505;
			List<HeroInfoData> list;
			if (base.Data == null)
			{
				list = new List<HeroInfoData>();
				base.Data = list;
			}
			else
			{
				list = (base.Data as List<HeroInfoData>);
			}
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				Log.debug("===>MobaGameClientPeer:TryUploadFightResponse" + operationResponse.OperationCode);
				if (operationResponse.Parameters.ContainsKey(1))
				{
					base.LastError = (int)operationResponse.Parameters[1];
				}
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryUploadFightResponse" + operationResponse.OperationCode;
				}
				else
				{
					string[] array = new string[0];
					if (operationResponse.Parameters.ContainsKey(112))
					{
						array = (operationResponse.Parameters[112] as string[]);
					}
					long num = 0L;
					if (operationResponse.Parameters.ContainsKey(67))
					{
						num = (long)operationResponse.Parameters[67];
					}
					string[] array2 = new string[0];
					if (operationResponse.Parameters.ContainsKey(161))
					{
						array2 = (operationResponse.Parameters[161] as string[]);
					}
					if (num > 0L)
					{
						int num2 = 0;
						string[] array3 = array;
						string item;
						for (int i = 0; i < array3.Length; i++)
						{
							item = array3[i];
							HeroInfoData heroInfoData = list.Find((HeroInfoData obj) => obj.ModelId == item);
							if (heroInfoData != null)
							{
								if (array2.Length > num2)
								{
									heroInfoData.Exp += (long)int.Parse(array2[num2]);
								}
								num2++;
							}
						}
					}
					base.DebugMessage = "===>成功";
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private void OnGetMsg_GameCode_UsingEquipment(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (msg == null)
			{
				return;
			}
			long equipid = 0L;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				if (mobaErrorCode == MobaErrorCode.HeroNotFound)
				{
					CtrlManager.ShowMsgBox(((MobaErrorCode)num).ToString(), "未找到英雄" + operationResponse.OperationCode, delegate
					{
					}, PopViewType.PopOneButton, "确定", "取消", null);
				}
			}
			else
			{
				string s = operationResponse.Parameters[93] as string;
				equipid = long.Parse(s);
				int modelId = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.EquipmentId == equipid).ModelId;
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23038, modelId, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}
		}

		private void OnGetMsg_GameCode_DischargeRune(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (msg == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			int num2 = (int)operationResponse.Parameters[109];
			if (num == 0)
			{
				if (num2 == 1)
				{
					EquipmentInfoData equipmentInfoData = SerializeHelper.Deserialize<EquipmentInfoData>(operationResponse.Parameters[90] as byte[]);
					long equipmentId = equipmentInfoData.EquipmentId;
					int modelId = equipmentInfoData.ModelId;
					MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23039, modelId, 0f);
					MobaMessageManager.ExecuteMsg(message);
				}
				if (num2 == 2)
				{
					MobaMessageManagerTools.SendClientMsg(ClientC2V.RunesAllDemount, null, false);
				}
			}
			else
			{
				CtrlManager.ShowMsgBox(((MobaErrorCode)num).ToString(), "玩家不存在" + operationResponse.OperationCode, delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
			}
		}

		private void OnGetMsg_GameCode_BuyShopGoods(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (msg == null)
			{
				return;
			}
			if ((int)operationResponse.Parameters[1] == 0)
			{
				int num = (int)operationResponse.Parameters[242];
				int num2 = num;
				if (num2 != 1)
				{
					if (num2 != 2)
					{
						Log.debug("===>MobaGameClientPeer:TryBuyGoodsResponse->商品类型不存在！");
					}
					else
					{
						byte[] buffer = operationResponse.Parameters[88] as byte[];
						HeroInfoData item = SerializeHelper.Deserialize<HeroInfoData>(buffer);
						List<HeroInfoData> list = this.Data as List<HeroInfoData>;
						if (list.Contains(item))
						{
							Singleton<TipView>.Instance.ShowViewSetText("The hero has contained!", 1f);
							return;
						}
						list.Add(item);
						base.Data = list;
						Singleton<TipView>.Instance.ShowViewSetText("Purchase Success!", 1f);
					}
				}
			}
		}

		private void OnGetMsg_GameCode_BuyShopGoodsNew(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (msg == null)
			{
				return;
			}
			if ((int)operationResponse.Parameters[1] == 0)
			{
				int num = (int)operationResponse.Parameters[242];
				if (num == 1)
				{
					byte[] buffer = operationResponse.Parameters[88] as byte[];
					HeroInfoData heroInfoData = SerializeHelper.Deserialize<HeroInfoData>(buffer);
					if (heroInfoData != null)
					{
						List<HeroInfoData> list = this.Data as List<HeroInfoData>;
						if (list.Contains(heroInfoData))
						{
							return;
						}
						list.Add(heroInfoData);
						base.Data = list;
					}
				}
			}
		}

		private void OnGetMsg_GameCode_ChangeSkin(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (msg == null)
			{
				return;
			}
			if ((int)operationResponse.Parameters[1] == 0)
			{
				int num = (int)operationResponse.Parameters[208];
				long heroId = (long)operationResponse.Parameters[89];
				List<HeroInfoData> list = base.Data as List<HeroInfoData>;
				list.Find((HeroInfoData obj) => obj.HeroId == heroId).CurrSkin = num;
				base.Data = list;
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23048, num, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}
		}

		private void OnGetMsg_GameCode_WearPrivateEffect(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (msg == null)
			{
				return;
			}
			if ((int)operationResponse.Parameters[1] == 0)
			{
				int num = (int)operationResponse.Parameters[10];
				int num2 = (int)operationResponse.Parameters[93];
				long heroId = (long)operationResponse.Parameters[89];
				int num3 = (int)operationResponse.Parameters[92];
				List<HeroInfoData> list = base.Data as List<HeroInfoData>;
				HeroInfoData heroInfoData = list.Find((HeroInfoData obj) => obj.HeroId == heroId);
				switch (num3)
				{
				case 1:
					heroInfoData.petId = ((num != 1) ? 0 : num2);
					break;
				case 2:
					heroInfoData.tailEffectId = ((num != 1) ? 0 : num2);
					break;
				case 3:
					heroInfoData.levelEffectId = ((num != 1) ? 0 : num2);
					break;
				case 4:
					heroInfoData.backEffectId = ((num != 1) ? 0 : num2);
					break;
				case 5:
					heroInfoData.birthEffectId = ((num != 1) ? 0 : num2);
					break;
				case 6:
					heroInfoData.deathEffectId = ((num != 1) ? 0 : num2);
					break;
				case 7:
					heroInfoData.eyeUnitSkinId = ((num != 1) ? 0 : num2);
					break;
				}
				base.Data = list;
			}
		}

		private void OnGetMsg_GameCode_SignDay(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (msg == null)
			{
				return;
			}
			if ((int)operationResponse.Parameters[1] == 0)
			{
				object obj = null;
				operationResponse.Parameters.TryGetValue(88, out obj);
				if (obj != null)
				{
					byte[] buffer = operationResponse.Parameters[88] as byte[];
					List<HeroInfoData> list = SerializeHelper.Deserialize<List<HeroInfoData>>(buffer);
					List<HeroInfoData> list2 = this.Data as List<HeroInfoData>;
					if (list.Count > 0)
					{
						if (list2.Contains(list[0]))
						{
							return;
						}
						list2.Add(list[0]);
					}
					base.Data = list2;
				}
			}
		}

		private void OnGetMsg_GameCode_ReceiveMailAttachment(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (msg == null)
			{
				return;
			}
			if ((int)operationResponse.Parameters[1] == 0)
			{
				object obj = null;
				operationResponse.Parameters.TryGetValue(88, out obj);
				if (obj != null)
				{
					byte[] buffer = operationResponse.Parameters[88] as byte[];
					List<HeroInfoData> heroinfodata = SerializeHelper.Deserialize<List<HeroInfoData>>(buffer);
					List<HeroInfoData> list = this.Data as List<HeroInfoData>;
					if (heroinfodata.Count > 0)
					{
						int i;
						for (i = 0; i < heroinfodata.Count; i++)
						{
							if (list.Find((HeroInfoData q) => q.ModelId == heroinfodata[i].ModelId) == null)
							{
								list.Add(heroinfodata[i]);
							}
						}
					}
					base.Data = list;
				}
			}
		}
	}
}
