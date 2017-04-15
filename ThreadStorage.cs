using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

public class ThreadStorage
{
	private readonly List<MsgData> _msgList = new List<MsgData>(100);

	public void AddMsg(int msgID, object msgParam, MsgType msgType, long svrTime)
	{
		object protoObj = null;
		if (msgType == MsgType.PvpMsg)
		{
			PvpCode pvpCode = (PvpCode)msgID;
			if (pvpCode == PvpCode.P2C_Packages)
			{
				this.UnpackPackages(msgID, msgParam, msgType);
				return;
			}
			protoObj = this.GetProtoObject(pvpCode, msgParam);
		}
		lock (this)
		{
			this._msgList.Add(new MsgData
			{
				msgID = msgID,
				svrTime = svrTime,
				msgParam = msgParam,
				msgType = msgType,
				protoObj = protoObj
			});
		}
	}

	private void UnpackPackages(int msgID, object msgParam, MsgType msgType)
	{
		OperationResponse operationResponse = msgParam as OperationResponse;
		byte[] buffer;
		if (operationResponse == null)
		{
			buffer = (byte[])msgParam;
		}
		else
		{
			buffer = (byte[])operationResponse.Parameters[0];
		}
		Packages packages = SerializeHelper.Deserialize<Packages>(buffer);
		if (packages.packages != null)
		{
			for (int i = 0; i < packages.packages.Length; i++)
			{
				RelpayCmd relpayCmd = packages.packages[i];
				this.AddMsg((int)relpayCmd.code, relpayCmd.paramListBytes, msgType, packages.svrTime);
			}
		}
	}

	private object GetProtoObject(PvpCode code, object msgParam)
	{
		switch (code)
		{
		case PvpCode.C2P_ReadySkill:
			return this.GetProbufMsg<ReadySkillInfo>(msgParam);
		case PvpCode.C2P_StartSkill:
			return this.GetProbufMsg<StartSkillInfo>(msgParam);
		case PvpCode.C2P_DoSkill:
		case PvpCode.C2P_FlashTo:
			IL_2E:
			if (code != PvpCode.C2P_DoHighEffect)
			{
				return null;
			}
			return this.GetProbufMsg<HighEffInfo>(msgParam);
		case PvpCode.C2P_HitSkill:
			return this.GetProbufMsg<HitSkillInfo>(msgParam);
		case PvpCode.C2P_StopSkill:
			return this.GetProbufMsg<StopSkillInfo>(msgParam);
		case PvpCode.C2P_EndSkill:
			return this.GetProbufMsg<EndSkillInfo>(msgParam);
		case PvpCode.C2P_UnitsSnap:
			return this.GetProbufMsg<UnitSnapInfo>(msgParam);
		}
		goto IL_2E;
	}

	public T GetProbufMsg<T>(object param) where T : class
	{
		T result;
		try
		{
			if (param is byte[])
			{
				byte[] buffer = param as byte[];
				T t = SerializeHelper.Deserialize<T>(buffer);
				result = t;
			}
			else
			{
				OperationResponse operationResponse = param as OperationResponse;
				byte[] buffer2 = (byte[])operationResponse.Parameters[0];
				T t2 = SerializeHelper.Deserialize<T>(buffer2);
				result = t2;
			}
		}
		catch (Exception var_5_50)
		{
			result = (T)((object)null);
		}
		return result;
	}

	public void GetAllMsgs(List<MsgData> msgList)
	{
		lock (this)
		{
			if (this._msgList.Count != 0)
			{
				msgList.AddRange(this._msgList);
				this._msgList.Clear();
			}
		}
	}
}
