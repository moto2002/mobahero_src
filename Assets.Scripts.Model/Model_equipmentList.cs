using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_equipmentList : ModelBase<List<EquipmentInfoData>>
	{
		public Model_equipmentList()
		{
			base.Init(EModelType.Model_equipmentList);
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaGameCode.UseSoulstone, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.UseProps, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.ChangeRune, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.Coalesce, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.ReceiveTBCReward, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.LuckyDraw, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.SellProps, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.ReceiveMailAttachment, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.Enchant, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.SweepBattle, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.GetEquipmentList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.UsingEquipment, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.DischargeRune, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.HeroAdvance, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.UploadFightResult, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.MagicBottleItem, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.BuyShopGoodsNew, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.WearPrivateEffect, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaGameCode.OneKeyCompose, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaGameCode.UseSoulstone, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.UseProps, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.ChangeRune, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.Coalesce, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.ReceiveTBCReward, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.LuckyDraw, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.SellProps, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.ReceiveMailAttachment, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.Enchant, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.SweepBattle, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.GetEquipmentList, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.UsingEquipment, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.DischargeRune, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.HeroAdvance, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.UploadFightResult, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.MagicBottleItem, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.BuyShopGoodsNew, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.WearPrivateEffect, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaGameCode.OneKeyCompose, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			UserData userData = ModelManager.Instance.Get_userData_X();
			base.LastError = 505;
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				MobaGameCode mobaGameCode = MVC_MessageManager.NotifyModel_to_Game((ClientMsg)msg.ID);
				MobaGameCode mobaGameCode2 = mobaGameCode;
				switch (mobaGameCode2)
				{
				case MobaGameCode.HeroAdvance:
					this.OnGetMsg_HeroAdvance(operationResponse);
					goto IL_1C2;
				case MobaGameCode.Enchant:
					this.OnGetMsg_Enchant(operationResponse);
					goto IL_1C2;
				case MobaGameCode.SellProps:
					this.OnGetMsg_SellProps(operationResponse);
					goto IL_1C2;
				case MobaGameCode.Coalesce:
					this.OnGetMsg_Coalesce(operationResponse);
					goto IL_1C2;
				case MobaGameCode.GetEquipmentList:
					this.OnGetMsg_GetEquipmentList(operationResponse);
					goto IL_1C2;
				case MobaGameCode.UsingEquipment:
					this.OnGetMsg_UsingEquipment(operationResponse);
					goto IL_1C2;
				case MobaGameCode.GetEquipmentDrop:
				case MobaGameCode.GetTalent:
				case MobaGameCode.ChangeTalent:
				case MobaGameCode.BuyTalentPag:
				case MobaGameCode.ModfiyTalentPag:
				case MobaGameCode.RestTalentPag:
				case MobaGameCode.GetRune:
					IL_86:
					switch (mobaGameCode2)
					{
					case MobaGameCode.SweepBattle:
						this.OnGetMsg_SweepBattle(operationResponse);
						goto IL_1C2;
					case MobaGameCode.RestTodayBattlesCount:
					case MobaGameCode.ReconnectToGameServer:
					case MobaGameCode.LuckyDrawState:
						IL_AB:
						if (mobaGameCode2 == MobaGameCode.BuyShopGoodsNew)
						{
							this.OnGetMsg_BuyShopGoodsNew(operationResponse);
							goto IL_1C2;
						}
						if (mobaGameCode2 == MobaGameCode.OneKeyCompose)
						{
							this.OnGetMsg_OneKeyCompose(operationResponse);
							goto IL_1C2;
						}
						if (mobaGameCode2 == MobaGameCode.ReceiveMailAttachment)
						{
							this.OnGetMsg_ReceiveMailAttachment(operationResponse);
							goto IL_1C2;
						}
						if (mobaGameCode2 == MobaGameCode.WearPrivateEffect)
						{
							this.OnGetMsg_WearPrivateEffect(operationResponse);
							goto IL_1C2;
						}
						if (mobaGameCode2 != MobaGameCode.ReceiveTBCReward)
						{
							goto IL_1C2;
						}
						this.OnGetMsg_ReceiveTBCReward(operationResponse);
						goto IL_1C2;
					case MobaGameCode.MagicBottleItem:
						this.OnGetMsg_UsingChangeNameCard(operationResponse);
						goto IL_1C2;
					case MobaGameCode.LuckyDraw:
						this.OnGetMsg_LuckyDraw(operationResponse);
						goto IL_1C2;
					case MobaGameCode.DischargeRune:
						this.OnGetMsg_DischargeRune(operationResponse);
						goto IL_1C2;
					}
					goto IL_AB;
				case MobaGameCode.UploadFightResult:
					this.OnGetMsg_UploadFightResult(operationResponse);
					goto IL_1C2;
				case MobaGameCode.UseSoulstone:
					this.OnGetMsg_UseSoulstone(operationResponse);
					goto IL_1C2;
				case MobaGameCode.UseProps:
					this.OnGetMsg_UseProps(operationResponse);
					goto IL_1C2;
				case MobaGameCode.ChangeRune:
					this.OnGetMsg_ChangeRune(operationResponse);
					goto IL_1C2;
				}
				goto IL_86;
			}
			IL_1C2:
			base.TriggerListners();
		}

		private void OnGetMsg_UseSoulstone(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			int soulId = Convert.ToInt32(res.Parameters[100]);
			int num = Convert.ToInt32(res.Parameters[103]);
			byte[] array = res.Parameters[88] as byte[];
			byte[] array2 = res.Parameters[90] as byte[];
			if (base.LastError == 0)
			{
				List<EquipmentInfoData> collection = null;
				if (array2 != null)
				{
					collection = SerializeHelper.Deserialize<List<EquipmentInfoData>>(array2);
				}
				HeroInfoData data = null;
				if (array != null)
				{
					data = SerializeHelper.Deserialize<HeroInfoData>(array);
				}
				bool check = false;
				ModelManager.Instance.Get_heroInfo_list_X().ForEach(delegate(HeroInfoData obj)
				{
					if (obj.HeroId == data.HeroId)
					{
						obj = data;
						check = true;
					}
				});
				if (!check)
				{
					ModelManager.Instance.Get_heroInfo_list_X().Add(data);
				}
				else
				{
					EquipmentInfoData equipmentInfoData = ((List<EquipmentInfoData>)base.Data).Find((EquipmentInfoData obj) => obj.EquipmentId == (long)soulId);
					equipmentInfoData.Count -= num;
				}
				base.Data = new List<EquipmentInfoData>(collection);
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>UseSoulstone" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_UseProps(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			int equip = (int)res.Parameters[93];
			int num = (int)res.Parameters[131];
			int num2 = Convert.ToInt32(res.Parameters[101]);
			int num3 = (int)res.Parameters[130];
			string value = res.Parameters[102] as string;
			long exp = (long)res.Parameters[67];
			List<EquipmentInfoData> list = new List<EquipmentInfoData>();
			if (base.Data != null)
			{
				list = (base.Data as List<EquipmentInfoData>);
			}
			if (base.LastError == 0)
			{
				if (num3 == 1)
				{
					long tId = Convert.ToInt64(value);
					HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_list_X().Find((HeroInfoData obj) => obj.HeroId == tId);
					heroInfoData.Exp = exp;
				}
				EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.EquipmentId == (long)equip);
				if (equipmentInfoData.Count > num)
				{
					equipmentInfoData.Count -= num;
				}
				else
				{
					list.Remove(equipmentInfoData);
				}
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23052, num, 0f);
				MobaMessageManager.ExecuteMsg(message);
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>UseProps" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_ChangeRune(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			List<EquipmentInfoData> list = base.Data as List<EquipmentInfoData>;
			MobaErrorCode lastError = (MobaErrorCode)base.LastError;
			if (lastError == MobaErrorCode.Ok)
			{
				byte[] buffer = res.Parameters[104] as byte[];
				byte[] buffer2 = res.Parameters[107] as byte[];
				byte[] buffer3 = res.Parameters[108] as byte[];
				List<RuneInfoData> list2 = SerializeHelper.Deserialize<List<RuneInfoData>>(buffer);
				RuneChangeModel equipInput = SerializeHelper.Deserialize<RuneChangeModel>(buffer2);
				RuneChangeModel runeChangeModel = SerializeHelper.Deserialize<RuneChangeModel>(buffer3);
				if (equipInput.EquipmentId > 0L)
				{
					EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.EquipmentId == equipInput.EquipmentId);
					Log.debug("===>MobaGameClientPeer:TryChangeRuneResponse->" + equipmentInfoData.ModelId + "旧符文放入背包！");
					if (equipmentInfoData != null)
					{
						equipmentInfoData.Count++;
					}
					else
					{
						list.Add(new EquipmentInfoData
						{
							EquipmentId = equipmentInfoData.EquipmentId,
							Count = 1,
							ModelId = equipmentInfoData.ModelId
						});
					}
				}
				if (runeChangeModel.EquipmentId > 0L)
				{
					EquipmentInfoData equipmentInfoData2 = list.Find((EquipmentInfoData obj) => obj.EquipmentId == equipInput.EquipmentId);
					Log.debug("===>MobaGameClientPeer:TryChangeRuneResponse->" + equipmentInfoData2.ModelId + "符文从背包中移除一个！");
					if (equipmentInfoData2 != null && equipmentInfoData2.Count > 0)
					{
						if (equipmentInfoData2.Count == 1)
						{
							list.Remove(equipmentInfoData2);
						}
						else
						{
							equipmentInfoData2.Count--;
						}
					}
				}
				list2.ForEach(delegate(RuneInfoData obj)
				{
					if (obj.List == null)
					{
						obj.List = new List<RuneModel>();
					}
				});
			}
		}

		private void OnGetMsg_Coalesce(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			List<EquipmentInfoData> list = base.Data as List<EquipmentInfoData>;
			UserData userData = ModelManager.Instance.Get_userData_X();
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[84] as byte[];
				List<EquipmentInfoData> data = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
				if (data != null)
				{
					int i;
					for (i = 0; i < data.Count; i++)
					{
						EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.ModelId == data[i].ModelId);
						if (equipmentInfoData == null)
						{
							if (data[i].Count != 0)
							{
								list.Add(data[i]);
							}
						}
						else if (data[i].Count == 0)
						{
							list.Remove(equipmentInfoData);
						}
						else
						{
							equipmentInfoData.Count = data[i].Count;
						}
					}
				}
				base.DebugMessage = "====>OK " + res.OperationCode;
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23042, true, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}
			else if (base.LastError == 500)
			{
				Singleton<TipView>.Instance.ShowViewSetText("服务器正忙，请稍后再试", 1f);
				base.DebugMessage = "====>Coalesce" + res.OperationCode;
			}
			else if (base.LastError == 506)
			{
				long money = (long)res.Parameters[98];
				userData.Money = money;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.RunesErrorRefreshMoney, null, false);
			}
			else if (base.LastError == 200 || base.LastError == 505)
			{
				byte[] buffer2 = res.Parameters[84] as byte[];
				List<EquipmentInfoData> data = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer2);
				if (data != null)
				{
					int i;
					for (i = 0; i < data.Count; i++)
					{
						EquipmentInfoData equipmentInfoData2 = list.Find((EquipmentInfoData obj) => obj.ModelId == data[i].ModelId);
						if (equipmentInfoData2 == null)
						{
							if (data[i].Count != 0)
							{
								list.Add(data[i]);
							}
						}
						else if (data[i].Count == 0)
						{
							list.Remove(equipmentInfoData2);
						}
						else
						{
							equipmentInfoData2.Count = data[i].Count;
						}
					}
				}
				MobaMessageManagerTools.SendClientMsg(ClientC2V.RunesErrorRefreshEquipment, null, false);
			}
			else if (base.LastError == 10)
			{
				long money2 = (long)res.Parameters[98];
				userData.Money = money2;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.RunesErrorRefreshMoney, null, false);
			}
			else
			{
				base.DebugMessage = "====>Coalesce" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_ReceiveTBCReward(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[202] as byte[];
				List<EquipmentInfoData> collection = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
				if (base.Data == null)
				{
					base.Data = new List<EquipmentInfoData>();
				}
				List<EquipmentInfoData> list = (List<EquipmentInfoData>)base.Data;
				list.AddRange(collection);
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>ReceiveTBCReward" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_LuckyDraw(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[90] as byte[];
				List<EquipmentInfoData> collection = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
				base.Data = new List<EquipmentInfoData>(collection);
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>LuckyDraw" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_SellProps(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[90] as byte[];
				List<EquipmentInfoData> collection = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
				base.Data = new List<EquipmentInfoData>(collection);
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>SellProps" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_ReceiveMailAttachment(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[90] as byte[];
				List<EquipmentInfoData> collection = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
				base.Data = new List<EquipmentInfoData>(collection);
			}
			else
			{
				base.DebugMessage = "====>ReceiveMailAttachment" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_Enchant(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[90] as byte[];
				List<EquipmentInfoData> collection = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
				base.Data = new List<EquipmentInfoData>(collection);
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>Enchant" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_SweepBattle(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				List<EquipmentInfoData> list = (List<EquipmentInfoData>)base.Data;
				int num = (int)res.Parameters[101];
				EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.ModelId == 53001);
				if (equipmentInfoData != null && equipmentInfoData.Count > num)
				{
					equipmentInfoData.Count -= num;
				}
				else
				{
					list.Remove(equipmentInfoData);
				}
				byte[] buffer = res.Parameters[156] as byte[];
				List<SweepData> list2 = SerializeHelper.Deserialize<List<SweepData>>(buffer);
				foreach (SweepData current in list2)
				{
					foreach (RewardModel subItem in current.Reward)
					{
						int type = subItem.Type;
						if (type != 4)
						{
							Log.debug("未知类型！" + subItem.Type);
						}
						else
						{
							EquipmentInfoData equipmentInfoData2 = list.Find((EquipmentInfoData obj) => obj.ModelId == int.Parse(subItem.Id));
							if (equipmentInfoData2 != null)
							{
								equipmentInfoData2.Count += subItem.Count;
							}
							else
							{
								list.Add(new EquipmentInfoData
								{
									Count = subItem.Count,
									EquipmentId = subItem.OnlyId,
									Grade = 1,
									Level = 1,
									ModelId = int.Parse(subItem.Id)
								});
							}
						}
					}
				}
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>SweepBattle" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_GetEquipmentList(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				byte[] array = res.Parameters[90] as byte[];
				List<EquipmentInfoData> data = null;
				if (array != null)
				{
					data = SerializeHelper.Deserialize<List<EquipmentInfoData>>(array);
				}
				base.Data = data;
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>GetEquipmentList" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_UsingEquipment(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			List<EquipmentInfoData> list = (List<EquipmentInfoData>)base.Data;
			if (base.LastError == 0)
			{
				string value = res.Parameters[93] as string;
				long eid = Convert.ToInt64(value);
				EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.EquipmentId == eid);
				int modelId = equipmentInfoData.ModelId;
				if (equipmentInfoData != null && equipmentInfoData.Count > 1)
				{
					equipmentInfoData.Count--;
				}
				else
				{
					list.Remove(equipmentInfoData);
				}
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23040, modelId, 0f);
				MobaMessageManager.ExecuteMsg(message);
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText("网络数据错误", 1f);
				base.DebugMessage = "====>UsingEquipment" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_WearPrivateEffect(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			List<EquipmentInfoData> list = (List<EquipmentInfoData>)base.Data;
			if (base.LastError == 0)
			{
				int num = (int)res.Parameters[10];
				int modelid = (int)res.Parameters[93];
				List<EquipmentInfoData> list2 = base.Data as List<EquipmentInfoData>;
				if (num == 1)
				{
					EquipmentInfoData equipmentInfoData = list2.Find((EquipmentInfoData obj) => obj.ModelId == modelid);
					equipmentInfoData.isweared = 1;
					if (equipmentInfoData != null && equipmentInfoData.Count > 1)
					{
						equipmentInfoData.Count--;
					}
					else
					{
						list2.Remove(equipmentInfoData);
					}
					MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23058, equipmentInfoData, 0f);
					MobaMessageManager.ExecuteMsg(message);
				}
				if (num == 2)
				{
					EquipmentInfoData equipmentinfodata = SerializeHelper.Deserialize<EquipmentInfoData>(res.Parameters[90] as byte[]);
					EquipmentInfoData equipmentInfoData2 = list2.Find((EquipmentInfoData obj) => obj.EquipmentId == equipmentinfodata.EquipmentId);
					if (equipmentInfoData2 != null)
					{
						equipmentInfoData2.isweared = 1;
						equipmentInfoData2.Count++;
					}
					else
					{
						equipmentInfoData2 = equipmentinfodata;
						list2.Add(equipmentInfoData2);
					}
					MobaMessage message2 = MobaMessageManager.GetMessage((ClientMsg)23059, equipmentInfoData2, 0f);
					MobaMessageManager.ExecuteMsg(message2);
				}
				base.Data = list2;
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>Demount" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_OneKeyCompose(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			List<EquipmentInfoData> data = (List<EquipmentInfoData>)base.Data;
			UserData userData = ModelManager.Instance.Get_userData_X();
			if (base.LastError == 0)
			{
				List<EquipmentInfoData> list = SerializeHelper.Deserialize<List<EquipmentInfoData>>(res.Parameters[84] as byte[]);
				int num = (int)res.Parameters[98];
				if (list != null)
				{
					list = list.FindAll((EquipmentInfoData obj) => obj.Count > 0);
					data = list;
					base.Data = data;
					userData.Money -= (long)num;
				}
				MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewOnCompoundAll, null, false);
			}
			else if (base.LastError == 500)
			{
				Singleton<TipView>.Instance.ShowViewSetText("服务器正忙，请稍后再试", 1f);
				base.DebugMessage = "====>Coalesce" + res.OperationCode;
			}
			else if (base.LastError == 506)
			{
				long money = (long)res.Parameters[98];
				userData.Money = money;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.RunesErrorRefreshMoney, null, false);
			}
			else if (base.LastError == 200)
			{
				byte[] buffer = res.Parameters[84] as byte[];
				List<EquipmentInfoData> list2 = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
				list2 = list2.FindAll((EquipmentInfoData obj) => obj.Count > 0);
				MobaMessageManagerTools.SendClientMsg(ClientC2V.RunesErrorRefreshEquipment, null, false);
			}
			else if (base.LastError == 10)
			{
				long money2 = (long)res.Parameters[98];
				userData.Money = money2;
				MobaMessageManagerTools.SendClientMsg(ClientC2V.RunesErrorRefreshMoney, null, false);
			}
		}

		private void OnGetMsg_DischargeRune(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			List<EquipmentInfoData> list = (List<EquipmentInfoData>)base.Data;
			int num = (int)res.Parameters[109];
			if (base.LastError == 0)
			{
				int num2 = num;
				if (num2 != 1)
				{
					if (num2 == 2)
					{
						List<EquipmentInfoData> listEquip = SerializeHelper.Deserialize<List<EquipmentInfoData>>(res.Parameters[84] as byte[]);
						if (list != null && listEquip.Count > 0)
						{
							int i;
							for (i = 0; i < listEquip.Count; i++)
							{
								EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.EquipmentId == listEquip[i].EquipmentId);
								if (equipmentInfoData != null)
								{
									equipmentInfoData.Count++;
								}
								else
								{
									list.Add(listEquip[i]);
								}
							}
							MobaMessageManagerTools.SendClientMsg(ClientC2V.RunesAllDemountEquip, null, false);
						}
					}
				}
				else
				{
					EquipmentInfoData equipmentInfoData2 = SerializeHelper.Deserialize<EquipmentInfoData>(res.Parameters[90] as byte[]);
					long eid = equipmentInfoData2.EquipmentId;
					EquipmentInfoData equipmentInfoData3 = list.Find((EquipmentInfoData obj) => obj.EquipmentId == eid);
					if (equipmentInfoData3 != null)
					{
						equipmentInfoData3.Count++;
					}
					else
					{
						list.Add(equipmentInfoData2);
					}
					MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23041, equipmentInfoData2, 0f);
					MobaMessageManager.ExecuteMsg(message);
					base.DebugMessage = "====>OK " + res.OperationCode;
				}
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText("网络数据错误", 1f);
				base.DebugMessage = "====>Demount" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_HeroAdvance(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			List<EquipmentInfoData> list = (List<EquipmentInfoData>)base.Data;
			if (base.LastError == 0)
			{
				byte[] buffer = res.Parameters[90] as byte[];
				List<EquipmentInfoData> list2 = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer);
				foreach (EquipmentInfoData equip in list2)
				{
					EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.EquipmentId == equip.EquipmentId);
					if (equipmentInfoData != null)
					{
						equipmentInfoData.Count = equip.Count;
					}
					else
					{
						list.Add(new EquipmentInfoData
						{
							Count = equip.Count,
							EquipmentId = equip.EquipmentId,
							Grade = equip.Grade,
							Level = equip.Level,
							ModelId = equip.ModelId
						});
					}
				}
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				base.DebugMessage = "====>HeroAdvance" + res.OperationCode;
			}
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_UploadFightResult(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			base.Valid = (base.LastError == 0);
		}

		private void OnGetMsg_UsingChangeNameCard(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (base.LastError == 0)
			{
				int num = (int)res.Parameters[101];
				List<EquipmentInfoData> list = (List<EquipmentInfoData>)base.Data;
				EquipmentInfoData equipmentInfoData = list.Find((EquipmentInfoData obj) => obj.ModelId == 6666);
				if (num > 0)
				{
					if (equipmentInfoData != null && equipmentInfoData.Count > 1)
					{
						equipmentInfoData.Count--;
					}
					else
					{
						list.Remove(equipmentInfoData);
					}
				}
				base.DebugMessage = "====>OK " + res.OperationCode;
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText("修改失败请重新改名", 1f);
				base.DebugMessage = "====>UsingChangeNameCard" + res.OperationCode;
			}
			base.Valid = true;
		}

		private void OnGetMsg_BuyShopGoods(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (this.LastError == 0)
			{
				int num = (int)res.Parameters[242];
				int num2 = num;
				if (num2 != 1)
				{
					if (num2 != 2)
					{
						Log.debug("===>MobaGameClientPeer:TryBuyGoodsResponse->商品类型不存在！");
					}
				}
				else
				{
					byte[] buffer = res.Parameters[90] as byte[];
					EquipmentInfoData item = SerializeHelper.Deserialize<EquipmentInfoData>(buffer);
					List<EquipmentInfoData> list = this.Data as List<EquipmentInfoData>;
					if (list.Contains(item))
					{
						Singleton<TipView>.Instance.ShowViewSetText("The hero has contained!", 1f);
						return;
					}
					list.Add(item);
					base.Data = list;
				}
			}
		}

		private void OnGetMsg_BuyShopGoodsNew(OperationResponse res)
		{
			base.LastError = (int)res.Parameters[1];
			if (this.LastError == 0)
			{
				int num = (int)res.Parameters[242];
				if (num == 3)
				{
					byte[] buffer = res.Parameters[90] as byte[];
					EquipmentInfoData equipmentInfoData = SerializeHelper.Deserialize<EquipmentInfoData>(buffer);
					List<EquipmentInfoData> list = base.Data as List<EquipmentInfoData>;
					foreach (EquipmentInfoData current in list)
					{
						if (current.EquipmentId == equipmentInfoData.EquipmentId && current.ModelId == equipmentInfoData.ModelId)
						{
							current.Count += equipmentInfoData.Count;
							base.Data = list;
							return;
						}
					}
					list.Add(equipmentInfoData);
					base.Data = list;
				}
			}
		}
	}
}
