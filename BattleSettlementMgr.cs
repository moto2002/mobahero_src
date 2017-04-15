using Assets.Scripts.Model;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

public class BattleSettlementMgr : Singleton<BattleSettlementMgr>
{
	public enum SettlementState
	{
		eNotBegin = 1,
		ePreparing,
		eUpdating,
		eFailedRet,
		eSuccessRet
	}

	private BattleSettlementMgr.SettlementState settlementState = BattleSettlementMgr.SettlementState.eNotBegin;

	private bool isPass;

	private int battleType;

	private string battle_id = string.Empty;

	private string scene_id = string.Empty;

	private string[] herolist;

	private List<TBCHeroStateInfo> myTBCHeroStateInfoList;

	private List<TBCHeroStateInfo> targetCHeroStateInfoList;

	private int StarNums;

	public XLDVictoryData xdVictoryData = new XLDVictoryData();

	public XLDVictoryData xldData
	{
		get
		{
			return this.xdVictoryData;
		}
	}

	public BattleSettlementMgr.SettlementState State
	{
		get
		{
			return this.settlementState;
		}
		private set
		{
			if (value != this.settlementState)
			{
				this.settlementState = value;
				if (this.settlementState == BattleSettlementMgr.SettlementState.eSuccessRet || this.settlementState == BattleSettlementMgr.SettlementState.eFailedRet)
				{
					new BattleSettlementMsg(this.settlementState);
				}
				else if (this.settlementState == BattleSettlementMgr.SettlementState.ePreparing)
				{
					ModelManager.Instance.Set_Settle_SummonerExpRecord();
					ModelManager.Instance.Set_Settle_BottleExpRecord();
				}
				else if (this.settlementState == BattleSettlementMgr.SettlementState.eSuccessRet)
				{
				}
			}
		}
	}

	public BattleSettlementMgr()
	{
		MVC_MessageManager.AddListener_view(MobaGameCode.UploadFightResult, new MobaMessageFunc(this.OnGetUploadFightResult));
	}

	public void Init()
	{
		this.settlementState = BattleSettlementMgr.SettlementState.eNotBegin;
	}

	public void OnGameOver()
	{
		this.State = BattleSettlementMgr.SettlementState.ePreparing;
		this.isPass = GameManager.IsVictory.Value;
		this.scene_id = LevelManager.CurLevelId;
		this.battle_id = LevelManager.CurBattleId;
		this.herolist = GameManager.Instance.Spawner.GetHeroNames(TeamType.LM).ToArray();
		this.battleType = LevelManager.CurBattleType;
		long exp = ModelManager.Instance.Get_userData_filed_X("Exp");
		CharacterDataMgr.instance.SaveNowUserLevel(exp);
		this.StarNums = GameManager.Instance.StarManager.GetStartNum(true);
		GameTimer.NormalTimeScale();
		if (this.battleType == 11)
		{
			this.State = BattleSettlementMgr.SettlementState.eSuccessRet;
			return;
		}
		if (this.battleType == 12)
		{
			string text = string.Concat(new string[]
			{
				"PVP_",
				DateTime.Now.Year.ToString(),
				DateTime.Now.Month.ToString(),
				DateTime.Now.Ticks.ToString(),
				".trep"
			});
			Util.mode = CompressMode.LZMALow;
			this.State = BattleSettlementMgr.SettlementState.eSuccessRet;
			return;
		}
		this.UpdateToServer();
	}

	public void UpdateToServer()
	{
		this.State = BattleSettlementMgr.SettlementState.eUpdating;
		if (this.battleType == 9)
		{
			this.RegisterUpdateHandler_XLD();
		}
		else if (this.battleType != 6)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在上传战斗结果", true, 15f);
			if (!SendMsgManager.Instance.SendMsg(MobaGameCode.UploadFightResult, param, new object[]
			{
				this.isPass,
				(byte)this.StarNums,
				this.battle_id,
				this.scene_id,
				this.herolist
			}))
			{
			}
		}
	}

	private void RegisterUpdateHandler_XLD()
	{
	}

	private void OnGetUploadFightResult(MobaMessage msg)
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
		int num = (int)operationResponse.Parameters[1];
		MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
		if (mobaErrorCode != MobaErrorCode.Ok)
		{
			this.OnGetUploadFightResult(num, operationResponse.DebugMessage, null);
		}
		else
		{
			this.OnGetUploadFightResult(num, operationResponse.DebugMessage, ModelManager.Instance.Get_equipmentList_X());
		}
	}

	private void OnGetUploadFightResult(int Ret, string DebugMessage, List<EquipmentInfoData> data)
	{
		Singleton<TipView>.Instance.GetErrorInformation(Ret);
		if (Ret == 0)
		{
			this.State = BattleSettlementMgr.SettlementState.eSuccessRet;
		}
		else
		{
			this.State = BattleSettlementMgr.SettlementState.eFailedRet;
		}
	}
}
