using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaHeros.Pvp.State
{
	public class PveStateLoad : PvpStateBase
	{
		private string _pveServerIp = "180.150.189.10";

		private int _pveServerPort = 28001;

		private int _roomId;

		private string _roomGuId = string.Empty;

		public PveStateLoad(string inServerIp, int inServerPort, int inRoomId, string inRoomGuId) : base(PvpStateCode.PveLoad)
		{
			string pveServerIp = GlobalSettings.Instance.PvpSetting.pveServerIp;
			this._pveServerIp = ((pveServerIp == null || pveServerIp.Length <= 0) ? inServerIp : pveServerIp);
			this._pveServerPort = ((inServerPort <= 0) ? 28001 : inServerPort);
			this._roomId = inRoomId;
			this._roomGuId = inRoomGuId;
		}

		private void RegMsgs(PvpStateBase.MsgRegFn func)
		{
			func(PvpCode.C2P_LoginPve, new MobaMessageFunc(this.P2C_LoginPve));
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			Singleton<PveManager>.Instance.ResetState();
			NetWorkHelper.Instance.DisconnectFromGateServer(false);
			NetWorkHelper.Instance.DisconnectLobbyServer();
			if (!StringUtils.CheckValid(this._pveServerIp) || this._pveServerPort <= 0)
			{
				ClientLogger.Error("PveServerIp or PveServerPort invalid");
			}
			Singleton<PvpManager>.Instance.LoginInfo = new PvpStartGameInfo
			{
				serverIp = this._pveServerIp,
				serverPort = this._pveServerPort,
				serverName = "MobaServer.pvpserver"
			};
			NetWorkHelper.Instance.ConnectToPvpServer();
		}

		protected override void RegistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.RegistMessage));
		}

		protected override void UnregistCallbacks()
		{
			this.RegMsgs(new PvpStateBase.MsgRegFn(MobaMessageManager.UnRegistMessage));
		}

		protected override void OnConnectServer(MobaPeerType type)
		{
			string curLevelId = LevelManager.CurLevelId;
			base.OnConnectServer(type);
			if (type == MobaPeerType.C2PvpServer)
			{
				List<EntityVo> heroes = LevelManager.GetHeroes(TeamType.LM);
				int battleId = int.Parse(curLevelId);
				List<PvePlayerInfo> list = new List<PvePlayerInfo>();
				foreach (EntityVo current in heroes)
				{
					HeroInfoData hero = new HeroInfoData
					{
						EpMagic = string.Empty,
						ModelId = current.npc_id,
						SkillList = null,
						Skins = string.Empty
					};
					HeroInfo heroInfo = new HeroInfo
					{
						heroId = current.npc_id,
						Hero = hero
					};
					PvePlayerInfo item = new PvePlayerInfo
					{
						heroInfo = heroInfo,
						heroSkinId = string.Empty,
						selfDefSkillId = string.Empty,
						talentInfo = null,
						userName = current.npc_id
					};
					list.Add(item);
				}
				SendMsgManager.Instance.SendPvpLoginMsgBase<C2PLoginPve>(PvpCode.C2P_LoginPve, new C2PLoginPve
				{
					battleId = battleId,
					pvePlayer = list,
					roomId = this._roomId
				}, this._roomId);
			}
		}

		protected override void OnDisconnectServer(MobaPeerType type)
		{
			base.OnDisconnectServer(type);
			if (type == MobaPeerType.C2PvpServer)
			{
			}
		}

		private void P2C_LoginPve(MobaMessage msg)
		{
			PveLoginRetaMsg probufMsg = msg.GetProbufMsg<PveLoginRetaMsg>();
			byte[] data = probufMsg.data;
			byte retaCode = probufMsg.retaCode;
			PvpStateBase.LogState("receive P2C_LoginPve " + retaCode);
			if (retaCode == 0)
			{
				PveBattlePreloadInfo pveBattlePreloadInfo = SerializeHelper.Deserialize<PveBattlePreloadInfo>(data);
				PvpStateBase.LogState("receive P2C_LoginPve ok" + StringUtils.DumpObject(pveBattlePreloadInfo));
				Singleton<PveManager>.Instance.SetBattleInfo(pveBattlePreloadInfo);
				Singleton<PvpManager>.Instance.SetRoomInfoOnServerPve(Singleton<PveManager>.Instance.MyLobbyUserId);
				Singleton<PveManager>.Instance.LoadPvpSceneBegin();
			}
		}
	}
}
