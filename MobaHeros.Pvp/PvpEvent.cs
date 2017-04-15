using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace MobaHeros.Pvp
{
	public class PvpEvent
	{
		public static void SendC2PKill(int attackerId, int deadId)
		{
		}

		public static void SendReadySkillEvent(ReadySkillInfo info)
		{
			byte[] args = SerializeHelper.Serialize<ReadySkillInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_ReadySkill, args);
		}

		public static void SendReadySkillCheckEvent(ReadySkillCheckInfo info)
		{
			byte[] args = SerializeHelper.Serialize<ReadySkillCheckInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_ReadySkillCheck, args);
		}

		public static void SendUseSkill(UseSkillInfo info)
		{
			byte[] args = SerializeHelper.Serialize<UseSkillInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_UseSkill, args);
		}

		public static void SendStartSkillEvent(StartSkillInfo info)
		{
			DateTime dateTime = new DateTime(UnitsSnapReporter.Instance.SyncTicks);
			byte[] args = SerializeHelper.Serialize<StartSkillInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_StartSkill, args);
		}

		public static void SendStopSkillEvent(StopSkillInfo info)
		{
			byte[] args = SerializeHelper.Serialize<StopSkillInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_StopSkill, args);
		}

		public static void SendHitSkillEvent(HitSkillInfo info)
		{
		}

		public static void SendEndSkillEvent(EndSkillInfo info)
		{
		}

		public static void SendDoSkillEvent(int inUnitId, string inSkillId)
		{
			byte[] args = SerializeHelper.Serialize<DoSkillInfo>(new DoSkillInfo
			{
				unitId = inUnitId,
				skillId = inSkillId
			});
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_DoSkill, args);
		}

		public static void SendDataUpdateEvent(DataUpdateInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<DataUpdateInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_DataUpdate, args);
		}

		public static void SendWoundEvent(WoundInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<WoundInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_Wound, args);
		}

		public static void SendDataChangeEvent(DataChangeInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<DataChangeInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_DataChange, args);
		}

		public static void SendJumFontEvent(JumpFontInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<JumpFontInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_JumpFont, args);
		}

		public static void SendAddBuffEvent(BuffInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<BuffInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_AddBuff, args);
		}

		public static void SendDoBuffEvent(BuffInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<BuffInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_DoBuff, args);
		}

		public static void SendRemoveBuffEvent(BuffInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<BuffInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_RemoveBuff, args);
		}

		public static void SendAddHighEffEvent(HighEffInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<HighEffInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_AddHighEffect, args);
		}

		public static void SendDoHighEffEvent(HighEffInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<HighEffInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_DoHighEffect, args);
		}

		public static void SendStartHighEffEvent(HighEffInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<HighEffInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_DoHighEffect, args);
		}

		public static void SendRemoveHightEvent(RemoveHighEffInfo info)
		{
			if (info == null)
			{
				return;
			}
			byte[] args = SerializeHelper.Serialize<RemoveHighEffInfo>(info);
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_RemoveHighEffect, args);
		}

		public static void SendGetMapItemEvent(int unitId, int itemId)
		{
		}

		public static void SendLoadingProcessEvent(byte num)
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_LoadProcess, SerializeHelper.Serialize<C2PLoadProcess>(new C2PLoadProcess
			{
				newUid = Singleton<PvpManager>.Instance.MyLobbyUserId,
				process = num
			}));
		}

		public static void SendSkillLevelUp(string skillID)
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_UpSkillLevel, SerializeHelper.Serialize<UpSkillInfo>(new UpSkillInfo
			{
				skillId = skillID
			}));
		}

		public static void SendTeamPosNotify(TeamSignalType type, Vector3 pos)
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_NotifyTeamPos, SerializeHelper.Serialize<NotifyTeamPos>(new NotifyTeamPos
			{
				senderId = Singleton<PvpManager>.Instance.MyLobbyUserId,
				signalType = (byte)type,
				pos = new Position
				{
					x = pos.x,
					y = pos.y,
					z = pos.z
				}
			}));
		}

		public static void SendTeamTargetNotify(TeamSignalType type, int unitId)
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_NotifyTeamTarget, SerializeHelper.Serialize<NotifyTeamTarget>(new NotifyTeamTarget
			{
				senderId = Singleton<PvpManager>.Instance.MyLobbyUserId,
				signalType = (byte)type,
				targetId = unitId
			}));
		}

		public static void SendMoveToPos(byte[] info)
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_MoveToPos, info);
		}

		public static void SendMoveToTarget(byte[] info)
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_MoveToTarget, info);
		}

		public static void SendStopMove(byte[] info)
		{
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_StopMove, info);
		}

		public static void SendFlashTo(int unitId, Vector3 pos)
		{
		}
	}
}
