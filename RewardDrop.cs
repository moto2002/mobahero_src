using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaHeros.Pvp.State;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardDrop : Singleton<RewardDrop>
{
	private string[] selectheros;

	private List<string>[] rewards = new List<string>[6];

	private string[] RewardDropData;

	private Units m_Player;

	public Callback callBackSuccess;

	public Callback callBackFail;

	private bool isRegister;

	private void RegisterStart()
	{
		if (this.isRegister)
		{
			return;
		}
		this.isRegister = true;
		MVC_MessageManager.AddListener_model(MobaGameCode.GetEquipmentDrop, new MobaMessageFunc(this.OnGetMsg_GetEquipmentDrop));
	}

	public string[] GetRewardResult()
	{
		this.RegisterStart();
		return this.RewardDropData;
	}

	public void RequestRewardDrop(int battle_type)
	{
		this.RegisterStart();
		this.selectheros = CharacterDataMgr.instance.GetSelectedHeros(CharacterDataMgr.instance.ChangeStrUserKey(battle_type.ToString())).ToArray();
		for (int i = 0; i < this.rewards.Length; i++)
		{
			this.rewards[i] = new List<string>();
		}
		string curBattleId = LevelManager.CurBattleId;
		string curLevelId = LevelManager.CurLevelId;
		if (curLevelId.ToCharArray()[0] != curBattleId.ToCharArray()[0])
		{
			Singleton<TipView>.Instance.ShowViewSetText("关卡号与关卡类型不匹配", 1f);
			ClientLogger.Error("关卡号与关卡类型不匹配");
			return;
		}
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
		SendMsgManager.Instance.SendMsg(MobaGameCode.GetEquipmentDrop, param, new object[]
		{
			curBattleId,
			curLevelId,
			this.selectheros
		});
	}

	private void ReInit()
	{
		for (int i = 0; i < 6; i++)
		{
			this.rewards[i] = new List<string>();
		}
		string curBattleId = LevelManager.CurBattleId;
		string curLevelId = LevelManager.CurLevelId;
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
		SendMsgManager.Instance.SendMsg(MobaGameCode.GetEquipmentDrop, param, new object[]
		{
			curBattleId,
			curLevelId,
			this.selectheros
		});
	}

	private void OnGetMsg_GetEquipmentDrop(MobaMessage msg)
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
		if (num == 201)
		{
			byte[] buffer = operationResponse.Parameters[2] as byte[];
			PveBattleRoomInfo pveBattleRoomInfo = SerializeHelper.Deserialize<PveBattleRoomInfo>(buffer);
			PvpStateManager.Instance.ChangeState(new PveStateLoad(pveBattleRoomInfo.ip, pveBattleRoomInfo.port, pveBattleRoomInfo.roomId, pveBattleRoomInfo.roomGid));
			return;
		}
		this.GetRewardDropCallback(num, operationResponse.DebugMessage);
	}

	private void GetRewardDropCallback(int Ret, string DebugMessage)
	{
		if (Ret == 0)
		{
			this.GetRewardSuccess();
		}
		else
		{
			this.GetRewardFail();
			ClientLogger.Error("掉落失败");
			CtrlManager.ShowMsgBox("提示", "初始化掉落获得错误码，将导致战斗结果提交失败，请呼叫王传伟 Error：" + Ret, null, PopViewType.PopOneButton, "确定", "取消", null);
		}
	}

	private void GetRewardSuccess()
	{
		if (this.callBackSuccess != null)
		{
			this.callBackSuccess();
		}
		this.callBackSuccess = null;
	}

	private void GetRewardFail()
	{
		if (this.callBackFail != null)
		{
			this.callBackFail();
		}
		this.callBackFail = null;
	}

	public void DropItem(Units attacker, Units target)
	{
		this.RegisterStart();
		float n = 0f;
		float roll = UnityEngine.Random.Range(0f, 1f);
		this.m_Player = attacker;
		if (target.isHero)
		{
			n = 0.3f;
		}
		else if (target.isMonster)
		{
			n = 0.1f;
		}
		else if (target.isHome)
		{
			n = 1f;
		}
		this.GetItem(roll, n);
	}

	private void GetItem(float roll, float n)
	{
		if (roll < n)
		{
			if (this.rewards == null)
			{
				return;
			}
			for (int i = 0; i < this.rewards.Length; i++)
			{
				if (this.rewards[i] == null)
				{
					return;
				}
				for (int j = 0; j < this.rewards[i].Count; j++)
				{
					SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.rewards[i][j]);
					this.m_Player.rewardNum++;
					this.rewards[i].Remove(this.rewards[i][j]);
					if (UnityEngine.Random.Range(0f, 1f) <= 0.9f)
					{
						break;
					}
					this.GetItem(roll, n);
				}
			}
		}
	}
}
