using Assets.Scripts.Model;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
	private List<TriggerCondition> taskList = new List<TriggerCondition>();

	private int recordIndex = -1;

	private int state;

	private void _Register()
	{
		MVC_MessageManager.AddListener_model(MobaGameCode.GetTBCEnemyInfo, new MobaMessageFunc(this.OnGetTBCEnemyInfo));
		MVC_MessageManager.AddListener_model(MobaGameCode.GetEquipmentList, new MobaMessageFunc(this.OnGetEquipmentList));
		MVC_MessageManager.AddListener_model(MobaGameCode.GetTalent, new MobaMessageFunc(this.OnGetTalent));
	}

	private void _Cancel()
	{
		MVC_MessageManager.RemoveListener_model(MobaGameCode.GetTBCEnemyInfo, new MobaMessageFunc(this.OnGetTBCEnemyInfo));
		MVC_MessageManager.RemoveListener_model(MobaGameCode.GetEquipmentList, new MobaMessageFunc(this.OnGetEquipmentList));
		MVC_MessageManager.RemoveListener_model(MobaGameCode.GetTalent, new MobaMessageFunc(this.OnGetTalent));
	}

	private void AddCoroutine()
	{
		this.taskList.Add(new TriggerCondition(this.TryUpdateDefFightTeam));
		this.taskList.Add(new TriggerCondition(this.GetEquipmentList));
		this.taskList.Add(new TriggerCondition(this.GetMyTalentList));
	}

	private bool TryUpdateDefFightTeam()
	{
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
		return SendMsgManager.Instance.SendMsg(MobaGameCode.UpdateDefFight, param, new object[0]);
	}

	private bool GetEquipmentList()
	{
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
		return SendMsgManager.Instance.SendMsg(MobaGameCode.GetEquipmentList, param, new object[0]);
	}

	private bool GetMyTalentList()
	{
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
		return SendMsgManager.Instance.SendMsg(MobaGameCode.GetTalent, param, new object[0]);
	}

	public void Init()
	{
		this._Register();
		this.AddCoroutine();
		base.StartCoroutine(this.DoTask());
	}

	[DebuggerHidden]
	private IEnumerator DoTask()
	{
		GameDataManager.<DoTask>c__Iterator159 <DoTask>c__Iterator = new GameDataManager.<DoTask>c__Iterator159();
		<DoTask>c__Iterator.<>f__this = this;
		return <DoTask>c__Iterator;
	}

	private void OnGetTBCEnemyInfo(MobaMessage msg)
	{
	}

	private void TBCEnemyInfoEventCallback(int Ret, string DebugMessage, List<TBCEnemyInfo> TBCEnemyInfoList)
	{
	}

	private void OnGetTalent(MobaMessage msg)
	{
	}

	public void OnGetMyTalentListCallback(int Ret, string DebugMessage, List<TalentInfoData> data)
	{
	}

	private void OnGetEquipmentList(MobaMessage msg)
	{
	}

	public void OnGetEquipmentListCallback(int Ret, string DebugMessage, List<EquipmentInfoData> data)
	{
	}

	private void SetTaskState(int index)
	{
		this.state = index;
	}

	public int GetTaskState(TriggerCondition _event)
	{
		if (!this.taskList.Contains(_event))
		{
			return -2;
		}
		if (this.taskList.IndexOf(_event) == this.recordIndex)
		{
			return 0;
		}
		if (this.taskList.IndexOf(_event) < this.recordIndex)
		{
			return 1;
		}
		return -1;
	}
}
