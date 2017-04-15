using Com.Game.Data;
using Com.Game.Manager;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Model
{
	internal class Model_BottleSystem : ModelBase<MagicBottleData>
	{
		internal static int Exp_Max;

		internal static string extraCost;

		private List<string> list_key = new List<string>();

		private Dictionary<MobaMessageType, List<int>> Dic_Bottle;

		public Model_BottleSystem()
		{
			base.Init(EModelType.Model_BottleSystem);
			this.Dic_Bottle = new Dictionary<MobaMessageType, List<int>>();
			this.Dic_Bottle.Add(MobaMessageType.GameCode, new List<int>());
			base.Data = new MagicBottleData();
		}

		public override void RegisterMsgHandler()
		{
			List<MobaGameCode> list = new List<MobaGameCode>
			{
				MobaGameCode.GetMagicBottleInfo,
				MobaGameCode.UseProps
			};
			foreach (MobaGameCode current in list)
			{
				this.Dic_Bottle[MobaMessageType.GameCode].Add((int)current);
				MVC_MessageManager.AddListener_model(current, new MobaMessageFunc(this.OnGetMsg));
			}
		}

		public override void UnRegisterMsgHandler()
		{
			foreach (MobaMessageType current in this.Dic_Bottle.Keys)
			{
				List<int> list = this.Dic_Bottle[current];
				foreach (int current2 in list)
				{
					MobaMessageType mobaMessageType = current;
					if (mobaMessageType == MobaMessageType.GameCode)
					{
						MVC_MessageManager.RemoveListener_model((MobaGameCode)current2, new MobaMessageFunc(this.OnGetMsg));
					}
				}
			}
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			int num = 0;
			MobaMessageType mobaMessageType = MVC_MessageManager.ClientMsg_to_RawCode((ClientMsg)msg.ID, out num);
			if (this.Dic_Bottle.ContainsKey(mobaMessageType) && this.Dic_Bottle[mobaMessageType].Contains(num))
			{
				string text = "OnGetMsg_" + mobaMessageType.ToString() + "_";
				MobaMessageType mobaMessageType2 = mobaMessageType;
				if (mobaMessageType2 == MobaMessageType.GameCode)
				{
					text += ((MobaGameCode)num).ToString();
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
			}
		}

		private bool PreHandel(MobaMessage msg, out OperationResponse res)
		{
			res = null;
			res = (msg.Param as OperationResponse);
			return res != null;
		}

		private void OnGetMsg_GameCode_GetMagicBottleInfo(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				if (base.LastError == 0)
				{
					MagicBottleData magicBottleData = SerializeHelper.Deserialize<MagicBottleData>(operationResponse.Parameters[236] as byte[]);
					MagicBottleData magicBottleData2 = base.Data as MagicBottleData;
					if (magicBottleData != null)
					{
						magicBottleData2 = magicBottleData;
						base.Data = magicBottleData2;
						Dictionary<string, SysMagicbottleExpVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleExpVo>();
						uint num = this.Level_Check(uint.Parse(magicBottleData2.curlevel.ToString()), typeDicByType);
						if (!typeDicByType.ContainsKey(num.ToString()))
						{
							Debug.LogError("小魔瓶的等级范围在字典中不存在，DK找策划看看。");
						}
						else
						{
							Model_BottleSystem.Exp_Max = typeDicByType[num.ToString()].exp;
							Model_BottleSystem.extraCost = typeDicByType[num.ToString()].ExtraCost;
						}
					}
					MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)23054, magicBottleData2, 0f);
					MobaMessageManager.ExecuteMsg(message);
				}
			}
			base.TriggerListners();
		}

		private uint Level_Check(uint level, Dictionary<string, SysMagicbottleExpVo> bottle_info)
		{
			uint result = 0u;
			Dictionary<string, SysMagicbottleExpVo>.KeyCollection keys = bottle_info.Keys;
			this.list_key.Clear();
			foreach (string current in keys)
			{
				this.list_key.Add(current);
			}
			for (int i = this.list_key.Count - 1; i >= 0; i--)
			{
				if (level >= uint.Parse(this.list_key[i]))
				{
					result = uint.Parse(this.list_key[i]);
					break;
				}
			}
			return result;
		}

		private void OnGetMsg_GameCode_UseProps(MobaMessage msg)
		{
			base.LastError = 505;
			OperationResponse operationResponse;
			if (this.PreHandel(msg, out operationResponse))
			{
				base.LastError = (int)operationResponse.Parameters[1];
				if (base.LastError == 0)
				{
					MagicBottleData magicBottleData = SerializeHelper.Deserialize<MagicBottleData>(operationResponse.Parameters[236] as byte[]);
					int num = (int)operationResponse.Parameters[131];
					MagicBottleData magicBottleData2 = base.Data as MagicBottleData;
					if (magicBottleData != null)
					{
						magicBottleData2 = magicBottleData;
						base.Data = magicBottleData2;
						Dictionary<string, SysMagicbottleExpVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleExpVo>();
						uint num2 = this.Level_Check(uint.Parse(magicBottleData2.curlevel.ToString()), typeDicByType);
						if (!typeDicByType.ContainsKey(num2.ToString()))
						{
							Debug.LogError("小魔瓶的等级范围在字典中不存在，DK找策划看看。");
						}
						else
						{
							Model_BottleSystem.Exp_Max = typeDicByType[num2.ToString()].exp;
						}
						if (num != 1)
						{
							MobaMessageManagerTools.SendClientMsg(ClientC2V.OpenBottleView, magicBottleData2, false);
						}
					}
				}
			}
			base.TriggerListners();
		}
	}
}
