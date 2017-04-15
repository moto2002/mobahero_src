using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_serverNotice : ModelBase<ServerNoticeModelData>
	{
		private ServerNoticeModelData data;

		public Model_serverNotice()
		{
			base.Init(EModelType.Model_serverNotice);
			this.data = new ServerNoticeModelData();
			base.Data = this.data;
		}

		public override void RegisterMsgHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23066, new MobaMessageFunc(this.OnGetMsg_GameCode));
			MobaMessageManager.RegistMessage((ClientMsg)25030, new MobaMessageFunc(this.OnGetMsg_StartGame));
			MobaMessageManager.RegistMessage(LobbyCode.L2C_BattleNotification, new MobaMessageFunc(this.OnGetMsg));
			MobaMessageManager.RegistMessage(MobaUserDataCode.UD_GameNotification, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23066, new MobaMessageFunc(this.OnGetMsg_GameCode));
			MobaMessageManager.UnRegistMessage((ClientMsg)25030, new MobaMessageFunc(this.OnGetMsg_StartGame));
			MobaMessageManager.UnRegistMessage(LobbyCode.L2C_BattleNotification, new MobaMessageFunc(this.OnGetMsg));
			MobaMessageManager.UnRegistMessage(MobaUserDataCode.UD_GameNotification, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
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
			base.LastMsgType = (int)msg.MessageType;
			base.LastMsgID = msg.ID;
			byte b = (byte)operationResponse.Parameters[255];
			MobaChannel mobaChannel = (MobaChannel)b;
			if (!Singleton<MenuView>.Instance.IsOpen && this.data.otherMsgQueue.Count >= 5)
			{
				this.data.otherMsgQueue.Dequeue();
			}
			if (Singleton<MenuView>.Instance.IsOpen && this.data.otherMsgQueue.Count >= 99)
			{
				return;
			}
			MobaChannel mobaChannel2 = mobaChannel;
			if (mobaChannel2 != MobaChannel.Lobby)
			{
				if (mobaChannel2 == MobaChannel.UserData)
				{
					this.OnGetMsg_UserDataCode(operationResponse);
				}
			}
			else
			{
				this.OnGetMsg_LobbyCode(operationResponse);
			}
		}

		private void OnGetMsg_UserDataCode(OperationResponse operationResponse)
		{
			string arg = operationResponse[10] as string;
			string arg2 = operationResponse[59] as string;
			byte[] buffer = operationResponse[246] as byte[];
			List<DropItemData> list = SerializeHelper.Deserialize<List<DropItemData>>(buffer);
			if (list == null || list.Count < 1)
			{
				return;
			}
			foreach (DropItemData current in list)
			{
				Com.Game.Module.ItemType dropItemType = ToolsFacade.Instance.GetDropItemType(current);
				string text = ToolsFacade.Instance.GetDropItemTypeName(dropItemType);
				Com.Game.Module.ItemType itemType = dropItemType;
				if (itemType != Com.Game.Module.ItemType.HeroSkin)
				{
					if (itemType == Com.Game.Module.ItemType.NormalGameItem)
					{
						SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(current.itemId.ToString());
						SysCustomizationVo dataById2 = BaseDataMgr.instance.GetDataById<SysCustomizationVo>(current.itemId.ToString());
						if (dataById2 != null && dataById2.customization_type != 1 && dataById.quality < 5)
						{
							break;
						}
						string text2;
						switch (dataById.quality)
						{
						case 3:
							text2 = "[C138F9]史诗级";
							break;
						case 4:
							text2 = "[ECC16F]传说级";
							break;
						case 5:
							text2 = "[f81841]典藏级";
							break;
						default:
							text2 = string.Empty;
							break;
						}
						text = string.Concat(new string[]
						{
							text2,
							text,
							"【",
							LanguageManager.Instance.GetStringById(dataById.name),
							"】"
						});
					}
				}
				else
				{
					SysHeroSkinVo dataById3 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(current.itemCount.ToString());
					text = string.Concat(new string[]
					{
						(dataById3.quality != 5) ? "[ECC16F]传说级" : "[f81841]典藏级",
						text,
						"【",
						LanguageManager.Instance.GetStringById(dataById3.name),
						"】"
					});
				}
				this.data.otherMsgQueue.Enqueue(string.Format("天降鸿运！恭喜[e3905b]{0}[-]在{1}中获得{2}", arg2, arg, text));
			}
		}

		private void OnGetMsg_LobbyCode(OperationResponse operationResponse)
		{
			if (operationResponse.Parameters.Count < 3)
			{
				return;
			}
			string arg = (string)operationResponse.Parameters[0];
			string unikey = (string)operationResponse.Parameters[1];
			int num = (int)operationResponse.Parameters[2];
			KillType killType = (KillType)num;
			string arg2 = string.Empty;
			KillType killType2 = killType;
			switch (killType2)
			{
			case KillType.TripleKill:
				arg2 = "[2EB1F7]三杀";
				break;
			case KillType.QuadraKill:
				arg2 = "[ECC16F]四杀";
				break;
			case KillType.PentaKill:
				arg2 = "[f81841]五杀";
				break;
			default:
				if (killType2 == KillType.GodLike)
				{
					arg2 = "[C138F9]超神";
				}
				break;
			}
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(unikey);
			this.data.otherMsgQueue.Enqueue(string.Format("杀神附体！[e3905b]{0}[-]在{1}中获得{2}", arg, LanguageManager.Instance.GetStringById(dataById.scene_map_id), arg2));
		}

		private void OnGetMsg_GameCode(MobaMessage msg)
		{
			NotificationData item = (NotificationData)msg.Param;
			this.data.gmMsgQueue.Enqueue(item);
		}

		private void OnGetMsg_StartGame(MobaMessage msg)
		{
			this.data.otherMsgQueue.Clear();
		}
	}
}
