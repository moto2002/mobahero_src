using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobaHeros.Pvp
{
	public class TeamSignalManager : MonoBehaviour
	{
		private class CdKeeper
		{
			private const float MaxCd = 15f;

			private readonly List<float> _cdList = new List<float>();

			public void Trigger()
			{
				float now = Time.realtimeSinceStartup;
				this._cdList.RemoveAll((float x) => now - x >= 15f);
				this._cdList.Add(now);
			}

			public bool IsCd()
			{
				float now = Time.realtimeSinceStartup;
				return this._cdList.Count((float x) => now - x < 15f) >= 3;
			}
		}

		private static readonly Dictionary<TeamSignalType, string> _signal2FxMap = new Dictionary<TeamSignalType, string>
		{
			{
				TeamSignalType.Danger,
				"Fx_Signal_Danger"
			},
			{
				TeamSignalType.Miss,
				"Fx_Signal_EnemyGone"
			},
			{
				TeamSignalType.Goto,
				"Fx_Signal_WhereToGo"
			},
			{
				TeamSignalType.Converge,
				"Fx_Signal_HelpMe"
			},
			{
				TeamSignalType.Fire,
				"Fx_Signal_Fire"
			},
			{
				TeamSignalType.Defense,
				"Fx_Signal_Defense"
			}
		};

		private static bool _isBegin = false;

		private static readonly TeamSignalManager.CdKeeper _cdKeeper = new TeamSignalManager.CdKeeper();

		private static Dictionary<int, PlayEffectAction> _lastAttackSignals = new Dictionary<int, PlayEffectAction>();

		public static TeamSignalType CurMode
		{
			get;
			private set;
		}

		public static bool IsBegin
		{
			get
			{
				return TeamSignalManager._isBegin;
			}
			private set
			{
				if (TeamSignalManager._isBegin != value)
				{
					TeamSignalManager._isBegin = value;
					MobaMessageManager.ExecuteMsg(ClientC2C.TeamSignalStateChanged, TeamSignalManager._isBegin, 0f);
				}
			}
		}

		public static void Begin(TeamSignalType mode)
		{
			TeamSignalManager.IsBegin = true;
			TeamSignalManager.CurMode = mode;
			CtrlManager.OpenWindow(WindowID.TeamSignalView, null);
			Singleton<MiniMapView>.Instance.BeginTeamSignal();
		}

		public static void End()
		{
			TeamSignalManager.IsBegin = false;
			CtrlManager.CloseWindow(WindowID.TeamSignalView);
			Singleton<MiniMapView>.Instance.EndTeamSignal();
		}

		public static string GetSignalPerform(TeamSignalType type)
		{
			return (!TeamSignalManager._signal2FxMap.ContainsKey(type)) ? string.Empty : TeamSignalManager._signal2FxMap[type];
		}

		private static void TryUpdateCd(int senderId, TeamSignalType signalType)
		{
			if (senderId == Singleton<PvpManager>.Instance.MyLobbyUserId)
			{
				TeamSignalManager._cdKeeper.Trigger();
				if (TeamSignalManager._cdKeeper.IsCd())
				{
					MobaMessageManager.ExecuteMsg(ClientC2C.TeamSignalCoolDown, null, 0f);
				}
			}
		}

		public static bool TrySendTeamPosNotify(TeamSignalType type, Vector3 pos)
		{
			PvpEvent.SendTeamPosNotify(type, pos);
			return true;
		}

		public static bool TrySendTeamTargetNotify(TeamSignalType type, int unitId)
		{
			PvpEvent.SendTeamTargetNotify(type, unitId);
			return true;
		}

		private static string GetFriendlyText(TeamSignalType signalType)
		{
			switch (signalType)
			{
			case TeamSignalType.Danger:
				return LanguageManager.Instance.GetStringById("BattleTeamSignalType_Danger");
			case TeamSignalType.Miss:
				return LanguageManager.Instance.GetStringById("BattleTeamSignalType_TheEnemyDisappeared");
			case TeamSignalType.Converge:
				return LanguageManager.Instance.GetStringById("BattleTeamSignalType_Set");
			case TeamSignalType.Fire:
				return LanguageManager.Instance.GetStringById("BattleTeamSignalType_Attack");
			case TeamSignalType.Defense:
				return LanguageManager.Instance.GetStringById("BattleTeamSignalType_Defense");
			case TeamSignalType.Goto:
				return LanguageManager.Instance.GetStringById("BattleTeamSignalType_Flee");
			default:
				return string.Empty;
			}
		}

		public static void Process(NotifyTeamTarget info)
		{
			int senderId = info.senderId;
			TeamSignalType signalType = (TeamSignalType)info.signalType;
			int targetId = info.targetId;
			if (!Singleton<PvpManager>.Instance.IsOurPlayer(senderId))
			{
				return;
			}
			TeamSignalManager.TryUpdateCd(senderId, signalType);
			switch (signalType)
			{
			case TeamSignalType.Danger:
				AudioMgr.Play("Play_SignalRisk", null, false, false);
				AudioMgr.Play("Play_CN_Risk", null, false, false);
				break;
			case TeamSignalType.Miss:
				AudioMgr.Play("Play_SignalMiss", null, false, false);
				break;
			case TeamSignalType.Converge:
				AudioMgr.Play("Play_SignalHelp", null, false, false);
				AudioMgr.Play("Play_CN_Help", null, false, false);
				break;
			case TeamSignalType.Fire:
				AudioMgr.Play("Play_SignalAttack", null, false, false);
				AudioMgr.Play("Play_CN_Gather", null, false, false);
				break;
			case TeamSignalType.Defense:
				AudioMgr.Play("Play_SignalDefense", null, false, false);
				break;
			case TeamSignalType.Goto:
				AudioMgr.Play("Play_SignalOntheWay", null, false, false);
				break;
			}
			Singleton<MiniMapView>.Instance.ShowTeamSignal(senderId, signalType, targetId);
			string playerHero = Singleton<PvpManager>.Instance.GetPlayerHero(senderId);
			HUDModuleMsgTools.CallBattleMsg_SiderTipsModule_Signal(playerHero, signalType);
		}

		private static void CheckSignalOverride(int senderId, TeamSignalType signalType, PlayEffectAction action)
		{
			PlayEffectAction playEffectAction;
			if (TeamSignalManager._lastAttackSignals.TryGetValue(senderId, out playEffectAction) && playEffectAction != null)
			{
				playEffectAction.Stop();
			}
			TeamSignalManager._lastAttackSignals[senderId] = action;
		}

		private static void CreateNotifyEffect(TeamSignalType signalType, Position pos)
		{
			string signalPerform = TeamSignalManager.GetSignalPerform(signalType);
			if (signalType == TeamSignalType.Fire || signalType == TeamSignalType.Defense)
			{
				return;
			}
			if (string.IsNullOrEmpty(signalPerform))
			{
				return;
			}
			SysSkillPerformVo dataById = BaseDataMgr.instance.GetDataById<SysSkillPerformVo>(signalPerform);
			if (dataById == null)
			{
				ClientLogger.Error("cannot found SysSkillPerformVo #" + signalPerform);
			}
			else
			{
				ResourceHandle resourceHandle = MapManager.Instance.SpawnResourceHandle(dataById.effect_id, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity, 0);
				if (resourceHandle != null)
				{
					resourceHandle.DelayRelease(1.8f);
				}
				else
				{
					ClientLogger.Error("SpawnEffect failed #" + dataById.effect_id);
				}
			}
		}

		public static void Process(NotifyTeamPos info)
		{
			int senderId = info.senderId;
			TeamSignalType signalType = (TeamSignalType)info.signalType;
			Position pos = info.pos;
			if (pos == null)
			{
				return;
			}
			if (!Singleton<PvpManager>.Instance.IsOurPlayer(senderId))
			{
				return;
			}
			TeamSignalManager.TryUpdateCd(senderId, signalType);
			TeamSignalManager.CreateNotifyEffect(signalType, pos);
			switch (signalType)
			{
			case TeamSignalType.Danger:
				AudioMgr.Play("Play_SignalRisk", null, false, false);
				AudioMgr.Play("Play_CN_Risk", null, false, false);
				break;
			case TeamSignalType.Miss:
				AudioMgr.Play("Play_SignalMiss", null, false, false);
				break;
			case TeamSignalType.Converge:
				AudioMgr.Play("Play_SignalHelp", null, false, false);
				AudioMgr.Play("Play_CN_Help", null, false, false);
				break;
			case TeamSignalType.Fire:
				AudioMgr.Play("Play_SignalAttack", null, false, false);
				AudioMgr.Play("Play_CN_Gather", null, false, false);
				break;
			case TeamSignalType.Defense:
				AudioMgr.Play("Play_SignalDefense", null, false, false);
				break;
			case TeamSignalType.Goto:
				AudioMgr.Play("Play_SignalOntheWay", null, false, false);
				break;
			}
			Singleton<MiniMapView>.Instance.ShowTeamSignal(senderId, signalType, new Vector3(pos.x, pos.y, pos.z));
			string playerHero = Singleton<PvpManager>.Instance.GetPlayerHero(senderId);
			HUDModuleMsgTools.CallBattleMsg_SiderTipsModule_Signal(playerHero, signalType);
		}

		public static void Clear()
		{
			TeamSignalManager._lastAttackSignals.Clear();
		}
	}
}
