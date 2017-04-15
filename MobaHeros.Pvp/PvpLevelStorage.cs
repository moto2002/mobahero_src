using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaHeros.Pvp
{
	internal sealed class PvpLevelStorage
	{
		private static LevelStorage? _lastStorage;

		private static LevelStorage _curStorage;

		public static bool CanFightAgain
		{
			get
			{
				if (PvpLevelStorage._curStorage.memberSummIds != null && PvpLevelStorage._curStorage.memberSummIds.Count > 0)
				{
					return PvpLevelStorage._curStorage.roomOwnerSummId == Singleton<PvpManager>.Instance.RoomInfo.MySummId.ToString();
				}
				if (LevelManager.m_CurLevel.IsLeague(PvpLevelStorage._curStorage.battleId))
				{
					EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == 8000);
					if (equipmentInfoData == null || equipmentInfoData.Count <= 1)
					{
						return false;
					}
				}
				return true;
			}
		}

		public static LevelStorage? FetchLast()
		{
			return PvpLevelStorage._lastStorage;
		}

		public static void FightAgain()
		{
			PvpLevelStorage._lastStorage = new LevelStorage?(PvpLevelStorage._curStorage);
			PvpUtils.GoHome();
		}

		public static void ClearLast()
		{
			PvpLevelStorage._lastStorage = null;
		}

		public static void CheckFightAgain()
		{
			if (!PvpLevelStorage._lastStorage.HasValue)
			{
				return;
			}
			CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
		}

		public static void SetLevel(MatchType matchType, string battleId)
		{
			PvpLevelStorage._curStorage.matchType = matchType;
			PvpLevelStorage._curStorage.battleId = battleId;
		}

		public static void JoinAsRoomMember(string roomOwner, List<string> members)
		{
			PvpLevelStorage._curStorage.roomOwnerSummId = roomOwner;
			PvpLevelStorage._curStorage.memberSummIds = null;
			if (members != null)
			{
				PvpLevelStorage._curStorage.memberSummIds = new List<string>(members);
			}
		}

		public static void JoinAsSingle()
		{
			PvpLevelStorage._curStorage.roomOwnerSummId = string.Empty;
			PvpLevelStorage._curStorage.memberSummIds = null;
		}

		public static void DispatchChooseGameMsg()
		{
			LevelStorage? levelStorage = PvpLevelStorage.FetchLast();
			if (levelStorage.HasValue)
			{
				string battleId = levelStorage.Value.battleId;
				int num;
				if (int.TryParse(battleId, out num))
				{
					MobaMessageManager.DispatchMsg((ClientMsg)26002, new object[]
					{
						num,
						levelStorage.Value.matchType
					}, 0f);
				}
				else
				{
					ClientLogger.Error("cannot parse BattleId " + battleId);
				}
			}
		}
	}
}
