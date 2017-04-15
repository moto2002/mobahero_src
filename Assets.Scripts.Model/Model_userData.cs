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
	internal class Model_userData : ModelBase<UserData>
	{
		private Dictionary<MobaMessageType, List<int>> mDicMobaMsg;

		public Model_userData()
		{
			base.Init(EModelType.Model_userData);
			this.mDicMobaMsg = new Dictionary<MobaMessageType, List<int>>();
			this.mDicMobaMsg.Add(MobaMessageType.ChatCode, new List<int>());
			this.mDicMobaMsg.Add(MobaMessageType.GameCode, new List<int>());
		}

		public override void RegisterMsgHandler()
		{
			List<MobaChatCode> list = new List<MobaChatCode>
			{
				MobaChatCode.Chat_Login
			};
			foreach (MobaChatCode current in list)
			{
				this.mDicMobaMsg[MobaMessageType.ChatCode].Add((int)current);
			}
			List<MobaGameCode> list2 = new List<MobaGameCode>
			{
				MobaGameCode.UseSoulstone,
				MobaGameCode.UseProps,
				MobaGameCode.BuyRunePag,
				MobaGameCode.Coalesce,
				MobaGameCode.BuySkillPoint,
				MobaGameCode.UsingSkillPoint,
				MobaGameCode.ReceiveTBCReward,
				MobaGameCode.UpdateArenaDefTeam,
				MobaGameCode.ArenaAtcCheck,
				MobaGameCode.UploadArenaAtc,
				MobaGameCode.LuckyDraw,
				MobaGameCode.BuyShopGoodsNew,
				MobaGameCode.Attendance,
				MobaGameCode.VipAttendance,
				MobaGameCode.MagicBottleItem,
				MobaGameCode.ModfiyAvatar,
				MobaGameCode.SellProps,
				MobaGameCode.ExchangeByDimond,
				MobaGameCode.CreateUnion,
				MobaGameCode.GetUnionInfo,
				MobaGameCode.DissolveUnion,
				MobaGameCode.JoinUnion,
				MobaGameCode.LeaveUnion,
				MobaGameCode.Enchant,
				MobaGameCode.SweepBattle,
				MobaGameCode.RestTodayBattlesCount,
				MobaGameCode.SystemNotice,
				MobaGameCode.CompleteTask,
				MobaGameCode.ReceiveMailAttachment,
				MobaGameCode.GuestUpgrade,
				MobaGameCode.Register,
				MobaGameCode.Login,
				MobaGameCode.GetEquipmentDrop,
				MobaGameCode.UploadFightResult,
				MobaGameCode.ChangeTalent,
				MobaGameCode.BuyTalentPag,
				MobaGameCode.RestTalentPag,
				MobaGameCode.ChangeSummonerSKill,
				MobaGameCode.RestArenaCD
			};
			foreach (MobaGameCode current2 in list2)
			{
				this.mDicMobaMsg[MobaMessageType.GameCode].Add((int)current2);
				MVC_MessageManager.AddListener_model(current2, new MobaMessageFunc(this.OnGetMsg));
			}
		}

		public override void UnRegisterMsgHandler()
		{
			foreach (MobaMessageType current in this.mDicMobaMsg.Keys)
			{
				List<int> list = this.mDicMobaMsg[current];
				foreach (int current2 in list)
				{
					switch (current)
					{
					case MobaMessageType.GameCode:
						MVC_MessageManager.RemoveListener_model((MobaGameCode)current2, new MobaMessageFunc(this.OnGetMsg));
						break;
					}
				}
			}
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			int num = 0;
			MobaMessageType mobaMessageType = MVC_MessageManager.ClientMsg_to_RawCode((ClientMsg)msg.ID, out num);
			if (this.mDicMobaMsg.ContainsKey(mobaMessageType) && this.mDicMobaMsg[mobaMessageType].Contains(num))
			{
				string text = "OnGetMsg_" + mobaMessageType.ToString() + "_";
				switch (mobaMessageType)
				{
				case MobaMessageType.GameCode:
					text += ((MobaGameCode)num).ToString();
					break;
				case MobaMessageType.ChatCode:
					text += ((MobaChatCode)num).ToString();
					break;
				}
				MethodInfo method = base.GetType().GetMethod(text, BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					object[] parameters = new object[]
					{
						msg
					};
					method.Invoke(this, parameters);
				}
				base.TriggerListners();
			}
		}

		private UserData GetUserData()
		{
			if (base.Data == null)
			{
				base.Data = new UserData();
			}
			return base.Data as UserData;
		}

		private bool PreHandel(MobaMessage msg, out OperationResponse res)
		{
			res = null;
			res = (msg.Param as OperationResponse);
			return res != null;
		}

		private void OnGetMsg_ChatCode_ChatLoin(MobaMessage msg)
		{
			base.Deserialize(86, msg);
		}

		private void OnGetMsg_GameCode_UseSoulstone(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				if (base.LastError == 0)
				{
					int num = Convert.ToInt32(operationResponse.Parameters[98]);
					userData.Money -= (long)num;
					base.Valid = (base.LastError == 0);
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_UseProps(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				int num = Convert.ToInt32(operationResponse.Parameters[101]);
				int num2 = (int)operationResponse.Parameters[130];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_Coalesce(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError == MobaErrorCode.Ok)
				{
					int num = (int)operationResponse.Parameters[98];
					userData.Money -= (long)num;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_UsingSkillPoint(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
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
					int num = (int)operationResponse.Parameters[98];
					string text = operationResponse.Parameters[89] as string;
					int num2 = (int)operationResponse.Parameters[101];
					userData.Money -= (long)num;
					userData.SkillPoint -= num2;
					userData.SkillPointFullTimeleft += 300 * num2;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_ReceiveTBCReward(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryReceiveTBCRewardResponse" + operationResponse.OperationCode;
				}
				else
				{
					int num = (int)operationResponse.Parameters[98];
					int num2 = (int)operationResponse.Parameters[211];
					userData.Money += (long)num;
					userData.TBCMoney += num2;
					base.DebugMessage = "领取远征奖励成功！";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_UpdateArenaDefTeam(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>:TryUpdateArenaDefTeamResponse" + operationResponse.OperationCode;
				}
				else
				{
					long rankId = (long)operationResponse.Parameters[68];
					userData.RankId = rankId;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_ArenaAtcCheck(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.CDTimeError)
				{
					if (lastError != MobaErrorCode.DayCountError)
					{
						if (lastError != MobaErrorCode.Ok)
						{
							if (lastError != MobaErrorCode.EnergyShortage)
							{
								base.DebugMessage = "===>MobaGameClientPeer:TryArenaAtcCheckResponse" + operationResponse.OperationCode;
							}
							else
							{
								base.DebugMessage = "===>TryArenaAtcCheckResponse->体力不足，不允许执行挑战！";
							}
						}
						else
						{
							base.DebugMessage = "===>:TryArenaAtcCheckResponse->校验成功，允许执行挑战！";
						}
					}
					else
					{
						base.DebugMessage = "===>MobaGameClientPeer:TryArenaAtcCheckResponse->次数不足，不允许执行挑战！";
					}
				}
				else
				{
					base.DebugMessage = "===>TryArenaAtcCheckResponse->CD时间未到，不允许执行挑战！";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_UploadArenaAtc(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>MobaGameClientPeer:TryUploadArenaAtcResultResponse" + operationResponse.OperationCode;
				}
				else
				{
					long num = (long)operationResponse.Parameters[68];
					int num2 = (int)operationResponse.Parameters[164];
					int num3 = (int)operationResponse.Parameters[154];
					int num4 = (int)operationResponse.Parameters[203];
					int num5 = (int)operationResponse.Parameters[204];
					userData.Exp += (long)num3;
					CharacterDataMgr.instance.SaveNowUserLevel(userData.Exp);
					userData.Diamonds += (long)num5;
					userData.Money += (long)num4;
					base.DebugMessage = "===>MobaGameClientPeer:TryUploadArenaAtcResultResponse->上报成功，当前名次:" + num;
					userData.RankId = num;
					if (userData.BestRank > num || userData.BestRank == 0L)
					{
						userData.BestRank = num;
					}
					userData.ArenaMoney += num2;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_LuckyDraw(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>MobaGameClientPeer:TryLuckyDrawResponse" + operationResponse.OperationCode;
				}
				else
				{
					userData.Money = (long)operationResponse.Parameters[98];
					userData.Diamonds = (long)operationResponse.Parameters[62];
					base.DebugMessage = "===>MobaGameClientPeer:TryLuckyDrawResponse->抽奖成功！";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_BuyShopGoodsNew(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryBuyGoodsNewResponse" + operationResponse.OperationCode;
				}
				else
				{
					byte b = (byte)operationResponse.Parameters[234];
					int num = (int)operationResponse.Parameters[103];
					byte b2 = b;
					if (b2 != 1)
					{
						if (b2 != 2)
						{
							if (b2 != 9)
							{
								Log.debug("===>MobaGameClientPeer:TryBuyGoodsNewResponse->商品类型不存在！");
							}
							else
							{
								userData.SmallCap -= num;
							}
						}
						else
						{
							userData.Diamonds -= (long)num;
						}
					}
					else
					{
						userData.Money -= (long)num;
					}
					switch ((int)operationResponse.Parameters[242])
					{
					case 4:
					{
						int num2 = (int)operationResponse.Parameters[61];
						string text = (string)operationResponse.Parameters[11];
						if (text.Equals("1"))
						{
							userData.Money += (long)num2;
						}
						else if (text.Equals("11"))
						{
							userData.Speaker += num2;
						}
						break;
					}
					case 5:
					{
						string id = (string)operationResponse.Parameters[11];
						ModelManager.Instance.GetNewAvatar("3", id);
						break;
					}
					case 6:
					{
						string id2 = (string)operationResponse.Parameters[11];
						ModelManager.Instance.GetNewAvatar("4", id2);
						break;
					}
					case 7:
						SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
						break;
					}
					base.DebugMessage = "===>:TryBuyGoodsNewResponse->购买指定商品成功";
				}
			}
			base.Valid = ((base.LastError == 0 || base.LastError == 10) && base.Data != null);
		}

		private void OnGetMsg_GameCode_BuyShopGoods(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryBuyGoodsResponse" + operationResponse.OperationCode;
				}
				else
				{
					int num = (int)operationResponse.Parameters[242];
					byte b = (byte)operationResponse.Parameters[234];
					int num2 = (int)operationResponse.Parameters[103];
					byte b2 = b;
					if (b2 != 1)
					{
						if (b2 != 2)
						{
							Log.debug("===>MobaGameClientPeer:TryBuyGoodsResponse->商品类型不存在！");
						}
						else
						{
							userData.Diamonds -= (long)num2;
						}
					}
					else
					{
						userData.Money -= (long)num2;
					}
					base.DebugMessage = "===>:TryBuyGoodsResponse->购买指定商品成功";
				}
			}
			base.Valid = ((base.LastError == 0 || base.LastError == 10) && base.Data != null);
		}

		private void OnGetMsg_GameCode_RestShop(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						base.DebugMessage = "===>TryRestShopByShopTypeResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>TryRestShopByShopTypeResponse->钻石不足！";
					}
				}
				else
				{
					int num = (int)operationResponse.Parameters[99];
					userData.Diamonds -= (long)num;
					base.DebugMessage = "===>TryRestShopByShopTypeResponse->刷新商店成功";
				}
			}
			base.Valid = ((base.LastError == 0 || base.LastError == 10) && base.Data != null);
		}

		private void OnGetMsg_GameCode_Attendance(MobaMessage msg)
		{
			this.OnGetMsg_GameCode_VipAttendance(msg);
		}

		private void OnGetMsg_GameCode_VipAttendance(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryAttendanceByTypeResponse" + operationResponse.OperationCode;
				}
				else
				{
					string text = operationResponse.Parameters[91] as string;
					if (!string.IsNullOrEmpty(text))
					{
						string[] array = text.Split(new char[]
						{
							'|'
						});
						string text2 = array[0];
						if (text2 != null)
						{
							if (Model_userData.<>f__switch$map2E == null)
							{
								Model_userData.<>f__switch$map2E = new Dictionary<string, int>(3)
								{
									{
										"1",
										0
									},
									{
										"3",
										0
									},
									{
										"2",
										1
									}
								};
							}
							int num;
							if (Model_userData.<>f__switch$map2E.TryGetValue(text2, out num))
							{
								if (num != 0)
								{
									if (num == 1)
									{
										int num2;
										if (int.Parse(array[2]) > 0 && userData.VIP >= int.Parse(array[2]))
										{
											num2 = int.Parse(array[2]) * 2;
										}
										else
										{
											num2 = int.Parse(array[2]);
										}
										userData.Diamonds += (long)num2;
									}
								}
							}
						}
					}
					userData.AttendanceCount++;
					userData.DayIsAttendance = true;
					base.DebugMessage = "===>TryAttendanceByTypeResponse->签到成功";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_MagicBottleItem(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.NickNameExist)
					{
						base.DebugMessage = "===>TryModfiyNickNameResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>TryModfiyNickNameResponse->昵称已经存在";
					}
				}
				else
				{
					string nickName = operationResponse.Parameters[59] as string;
					int changNickNameCount = (int)operationResponse.Parameters[101];
					userData.NickName = nickName;
					userData.ChangNickNameCount = changNickNameCount;
				}
			}
			base.Valid = true;
		}

		private void OnGetMsg_GameCode_ModfiyAvatar(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryModfiyIconResponse" + operationResponse.OperationCode;
				}
				else
				{
					int num = (int)operationResponse.Parameters[58];
					byte b = (byte)operationResponse.Parameters[10];
					if (b == 1)
					{
						userData.Avatar = num;
					}
					else
					{
						userData.PictureFrame = num;
					}
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_SellProps(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TrySellPropsResponse" + operationResponse.OperationCode;
				}
				else
				{
					long money = (long)operationResponse.Parameters[61];
					userData.Money = money;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_ExchangeByDimond(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>MobaGameClientPeer:TryExchangeByDimondResponse" + operationResponse.OperationCode;
				}
				else
				{
					byte[] buffer = operationResponse.Parameters[136] as byte[];
					int num = (int)operationResponse.Parameters[62];
					List<ExchangeData> list = SerializeHelper.Deserialize<List<ExchangeData>>(buffer);
					userData.Diamonds = (long)num;
					foreach (ExchangeData current in list)
					{
						byte type = current.Type;
						if (type != 1)
						{
							if (type != 2)
							{
								base.DebugMessage = "===>MobaGameClientPeer:TryExchangeByDimondResponse->未知的类型：" + current.Type;
							}
						}
						else
						{
							userData.Money += (long)current.Count;
							userData.BuyCoinCount++;
						}
					}
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_CreateUnion(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						if (lastError != MobaErrorCode.NickNameExist)
						{
							base.DebugMessage = "===>MobaGameClientPeer:TryCreateUnionResponse" + operationResponse.OperationCode;
						}
						else
						{
							base.DebugMessage = "===>MobaGameClientPeer:TryCreateUnionResponse->MobaErrorCode.NickNameExist->工会名称已经存在！";
						}
					}
					else
					{
						base.DebugMessage = "===>MobaGameClientPeer:TryCreateUnionResponse->MobaErrorCode.MoneyShortage->钻石不足！";
					}
				}
				else
				{
					userData.UnionId = (int)operationResponse.Parameters[142];
					userData.Diamonds -= 500L;
					base.DebugMessage = "===>MobaGameClientPeer:TryCreateUnionResponse->创建工会成功，工会ID：" + userData.UnionId;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_GetUnionInfo(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>GetUnionInfoResponse" + operationResponse.OperationCode;
				}
				else
				{
					byte[] buffer = operationResponse.Parameters[151] as byte[];
					UnionInfoData unionInfoData = SerializeHelper.Deserialize<UnionInfoData>(buffer);
					userData.LastUnionId = unionInfoData.Id;
					userData.UnionId = unionInfoData.Id;
					base.DebugMessage = "===>GetUnionInfoResponse->获取工会 信息成功";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_DissolveUnion(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryDissolveUnionResponse" + operationResponse.OperationCode;
				}
				else
				{
					userData.UnionId = 0;
					userData.LastExitUnionTime = DateTime.Now.ToString();
					base.DebugMessage = "===>TryDissolveUnionResponse->解散工会成功！";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_JoinUnion(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.UnionFull)
				{
					if (lastError != MobaErrorCode.UnionReqFull)
					{
						if (lastError != MobaErrorCode.Ok)
						{
							base.DebugMessage = "===>TryJoinUnionResponse" + operationResponse.OperationCode;
						}
						else
						{
							base.DebugMessage = "===>MobaGameClientPeer:TryJoinUnionResponse-> 加入/申请工会成功！";
							int num = (int)operationResponse.Parameters[142];
							if (num > 0)
							{
								userData.LastUnionId = num;
								userData.UnionId = num;
								userData.JoinUnionCount += 1L;
							}
						}
					}
					else
					{
						base.DebugMessage = "===>TryJoinUnionResponse-> MobaErrorCode.UnionReqFull-> 申请列表已满！";
					}
				}
				else
				{
					base.DebugMessage = "===>TryJoinUnionResponse->MobaErrorCode.UnionFull-> 申请的工会已经满员！";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_LeaveUnion(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryLeaveUnionResponse" + operationResponse.OperationCode;
				}
				else
				{
					userData.LastExitUnionTime = DateTime.Now.ToString();
					userData.UnionId = 0;
					base.DebugMessage = "===>TryLeaveUnionResponse-> 离开工会成功！";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_Enchant(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
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
						base.DebugMessage = "===>TryEnchantEquipmentResponse->MobaErrorCode.MoneyShortage->金钱不足！";
					}
				}
				else
				{
					int num = (int)operationResponse.Parameters[98];
					userData.Money -= (long)num;
					base.DebugMessage = "===>TryEnchantEquipmentResponse->装备附魔成功！";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_SweepBattle(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						base.DebugMessage = "===>TrySweepBattleResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>TrySweepBattleResponse->MobaErrorCode.MoneyShortage->金钱不足！";
					}
				}
				else
				{
					int num = (int)operationResponse.Parameters[99];
					int num2 = (int)operationResponse.Parameters[67];
					userData.Diamonds -= (long)num;
					userData.Exp += (long)num2;
					CharacterDataMgr.instance.SaveNowUserLevel(userData.Exp);
					byte[] buffer = operationResponse.Parameters[156] as byte[];
					List<SweepData> list = SerializeHelper.Deserialize<List<SweepData>>(buffer);
					foreach (SweepData current in list)
					{
						foreach (RewardModel current2 in current.Reward)
						{
							switch (current2.Type)
							{
							case 1:
								userData.Money += (long)current2.Count;
								continue;
							case 4:
								continue;
							}
							base.DebugMessage = "未知类型！" + current2.Type;
						}
					}
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_RestTodayBattlesCount(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryRestTodayBattlesCountResponse" + operationResponse.OperationCode;
				}
				else
				{
					int num = (int)operationResponse.Parameters[62];
					userData.Diamonds -= (long)num;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_SystemNotice(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>SystemNoticeResponse" + operationResponse.OperationCode;
				}
				else
				{
					byte[] buffer = operationResponse.Parameters[40] as byte[];
					NotificationData notificationData = SerializeHelper.Deserialize<NotificationData>(buffer);
					switch (notificationData.Type)
					{
					case 1:
						userData.UnionId = 0;
						userData.LastExitUnionTime = DateTime.Now.AddHours(-48.0).ToString();
						base.DebugMessage = "===> SystemNoticeResponse->踢出工会通知";
						goto IL_1C5;
					case 2:
						userData.UnionId = int.Parse(notificationData.Content);
						userData.LastUnionId = int.Parse(notificationData.Content);
						userData.JoinUnionCount += 1L;
						base.DebugMessage = "===> SystemNoticeResponse->允许加入工会通知->";
						goto IL_1C5;
					case 3:
						base.DebugMessage = "===> SystemNoticeResponse->加入工会被拒绝通知->";
						goto IL_1C5;
					case 4:
						base.DebugMessage = "===> SystemNoticeResponse->修改工会职位通知->";
						goto IL_1C5;
					case 7:
						base.DebugMessage = "===> SystemNoticeResponse->同意好友申请->";
						goto IL_1C5;
					case 8:
						base.DebugMessage = "===> SystemNoticeResponse->拒绝好友申请->";
						goto IL_1C5;
					case 9:
						base.DebugMessage = "===> SystemNoticeResponse->好友私信->";
						goto IL_1C5;
					case 11:
						base.DebugMessage = "===> SystemNoticeResponse->邀请加入游戏->";
						goto IL_1C5;
					case 14:
						base.DebugMessage = "===> SystemNoticeResponse->拉黑->";
						goto IL_1C5;
					case 15:
						base.DebugMessage = "===> SystemNoticeResponse->删除好友->";
						goto IL_1C5;
					}
					base.DebugMessage = "===> SystemNoticeResponse->未知的通知类型";
					IL_1C5:;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_CompleteTask(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>MobaGameClientPeer:TryCompleteTaskResponse" + operationResponse.OperationCode;
				}
				else
				{
					string str = operationResponse.Parameters[123] as string;
					string text = operationResponse.Parameters[91] as string;
					if (!string.IsNullOrEmpty(text))
					{
						string[] array = text.Split(new char[]
						{
							','
						});
						string[] array2 = array;
						int i = 0;
						while (i < array2.Length)
						{
							string text2 = array2[i];
							string[] array3 = text2.Split(new char[]
							{
								'|'
							});
							string text3 = array3[0];
							switch (text3)
							{
							case "1":
							{
								int num2 = Convert.ToInt32(array3[array3.Length - 1]);
								string text4 = array3[1];
								switch (text4)
								{
								case "1":
									userData.Money += (long)num2;
									break;
								case "2":
									userData.Diamonds += (long)num2;
									break;
								case "3":
									userData.ArenaMoney += num2;
									break;
								case "4":
									userData.TBCMoney += num2;
									break;
								case "5":
									userData.BigFightMoney += num2;
									break;
								case "6":
									userData.UnioMoney += num2;
									break;
								}
								break;
							}
							case "2":
							{
								int num4 = Convert.ToInt32(array3[1]);
								userData.Exp += (long)num4;
								CharacterDataMgr.instance.SaveNowUserLevel(userData.Exp);
								break;
							}
							}
							IL_2B8:
							i++;
							continue;
							goto IL_2B8;
						}
					}
					base.DebugMessage = "===>TryCompleteTaskResponse->完成任务成功，任务ID" + str;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_ReceiveMailAttachment(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>TryReceiveMailAttachmentResponse" + operationResponse.OperationCode;
				}
				else
				{
					long num = (long)operationResponse.Parameters[73];
					if (num == 60403L)
					{
						UserData userData = this.GetUserData();
						userData.ReportCount++;
					}
					base.DebugMessage = "===>TryReceiveMailAttachmentResponse->领取邮件奖励成功,邮件ID：" + num;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void ReceiveReward(List<RewardModel> mailReward)
		{
		}

		private void OnGetMsg_GameCode_GuestUpgrade(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError == MobaErrorCode.Ok)
				{
					base.DebugMessage = " RegisterUserResponse : 注册成功! 获取数据...";
					byte[] buffer = operationResponse.Parameters[86] as byte[];
					UserData userData2 = SerializeHelper.Deserialize<UserData>(buffer);
					if (userData2 != null)
					{
						userData = userData2;
						base.Data = userData;
						base.DebugMessage = " PhotonClient : 注册成功! accountid = " + userData.UserId;
					}
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_Register(MobaMessage msg)
		{
			this.OnGetMsg_GameCode_GuestUpgrade(msg);
		}

		private void OnGetMsg_GameCode_Login(MobaMessage msg)
		{
			base.Valid = true;
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError == MobaErrorCode.Ok)
				{
					byte[] buffer = operationResponse.Parameters[86] as byte[];
					UserData userData2 = SerializeHelper.Deserialize<UserData>(buffer);
					if (userData != null)
					{
						userData = userData2;
						base.Data = userData;
						AccountData accountData = ModelManager.Instance.Get_accountData_X();
						if (accountData != null)
						{
							int userLevel = CharacterDataMgr.instance.GetUserLevel(userData.Exp);
							CharacterDataMgr.instance.SaveNowUserLevel(userData.Exp);
							AnalyticsToolManager.SetLevel(accountData.AccountId, userLevel);
						}
						base.DebugMessage = " MobaClient : 登录游戏账号成功!!";
					}
					else
					{
						base.DebugMessage = "数据获取错误!";
						base.Valid = false;
					}
				}
			}
		}

		private void OnGetMsg_GameCode_GetEquipmentDrop(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError == MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>GetRewardDropResponse:获取掉落成功！已加入m_user豪华晚餐！！！";
					base.DebugMessage = "===>服務器回調沒有包含CurrEnergy 字段！！！";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_UploadFightResult(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>MobaGameClientPeer:TryUploadFightResponse" + operationResponse.OperationCode;
				}
				else
				{
					long num = 0L;
					if (operationResponse.Parameters.ContainsKey(154))
					{
						num = (long)operationResponse.Parameters[154];
					}
					long num2 = 0L;
					if (operationResponse.Parameters.ContainsKey(98))
					{
						num2 = (long)operationResponse.Parameters[98];
					}
					userData.Exp += num;
					CharacterDataMgr.instance.SaveNowUserLevel(userData.Exp);
					userData.Money += num2;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_ChangeTalent(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						base.DebugMessage = "===>TryChangeTalentResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>TryChangeTalentResponse 钱不够！！！";
					}
				}
				else
				{
					int num = Convert.ToInt32(operationResponse.Parameters[98]);
					userData.Money -= (long)num;
					base.DebugMessage = "===>===>TryChangeTalentResponse 天赋修改成功m_user数据，消耗金币：" + num;
				}
			}
			base.Valid = ((base.LastError == 0 || base.LastError == 10) && base.Data != null);
		}

		private void OnGetMsg_GameCode_BuyTalentPag(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						base.DebugMessage = "===>TryBuyTalentPagResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>TryBuyTalentPagResponse  购买天赋页的钻石不够！！！";
					}
				}
				else
				{
					int num = Convert.ToInt32(operationResponse.Parameters[99]);
					userData.Diamonds -= (long)num;
					base.DebugMessage = "===>TryBuyTalentPagResponse天赋页购买成功，消耗钻石：" + num;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_RestTalentPag(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.MoneyShortage)
					{
						base.DebugMessage = "===>MobaGameClientPeer:TryRestCurrUseTalentPagResponse" + operationResponse.OperationCode;
					}
					else
					{
						base.DebugMessage = "===>天赋页重置失败，钱不够！！！";
					}
				}
				else
				{
					int num = (int)operationResponse.Parameters[98];
					base.DebugMessage = "===>天赋页重置成功，消耗Monery：" + num;
					userData.Diamonds -= (long)num;
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_ChangeSummonerSKill(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>修改召唤师技能失败";
				}
				else
				{
					userData.SummSkills = (string)operationResponse.Parameters[217];
					base.DebugMessage = "===>修改召唤师技能成功";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_RestArenaCD(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					base.DebugMessage = "===>小奇袭CD失败";
				}
				else
				{
					int num = (int)operationResponse.Parameters[99];
					userData.Diamonds -= (long)num;
					base.DebugMessage = "===>小奇袭CD成功";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}

		private void OnGetMsg_GameCode_BuySkin(MobaMessage msg)
		{
			base.LastError = 505;
			UserData userData = this.GetUserData();
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				MobaErrorCode lastError = (MobaErrorCode)base.LastError;
				if (lastError != MobaErrorCode.Ok)
				{
					if (lastError != MobaErrorCode.SkinNotBelongToHero)
					{
						base.DebugMessage = "===>购买皮肤失败";
					}
					else
					{
						Singleton<TipView>.Instance.ShowViewSetText("MobaErrorCode错误码 " + 20206 + " 可能是bindata问题，请找策划", 1f);
					}
				}
				else
				{
					int num = (int)operationResponse.Parameters[99];
					userData.Diamonds -= (long)num;
					base.DebugMessage = "===>购买皮肤成功";
				}
			}
			base.Valid = (base.LastError == 0 && base.Data != null);
		}
	}
}
