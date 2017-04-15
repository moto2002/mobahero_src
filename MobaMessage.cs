using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaProtocol;
using System;
using UnityEngine;

public sealed class MobaMessage
{
	private MobaMessageType mType;

	private int mID;

	private object mParam;

	private float mDelayTime;

	private float mDelayElapsedTime;

	private object _protoObj;

	public long svrTime;

	public MobaMessageType MessageType
	{
		get
		{
			return this.mType;
		}
	}

	public int ID
	{
		get
		{
			return this.mID;
		}
	}

	public object Param
	{
		get
		{
			return this.mParam;
		}
	}

	public object ProtoMsg
	{
		get
		{
			return this._protoObj;
		}
	}

	public MobaMessage(MobaMessageType type, int id, object param, float delayTime = 0f, object protoObj = null)
	{
		this.mParam = param;
		this.mID = id;
		this.mType = type;
		this.mDelayElapsedTime = 0f;
		this.mDelayTime = 0f;
		this._protoObj = protoObj;
	}

	public bool IsDelayExec()
	{
		bool result = false;
		this.mDelayElapsedTime += Time.deltaTime;
		if (this.mDelayElapsedTime < this.mDelayTime)
		{
			result = true;
		}
		return result;
	}

	public T GetProbufMsg<T>() where T : class
	{
		T result;
		try
		{
			if (this.Param is byte[])
			{
				byte[] buffer = this.Param as byte[];
				T t = SerializeHelper.Deserialize<T>(buffer);
				result = t;
			}
			else
			{
				OperationResponse operationResponse = this.Param as OperationResponse;
				byte[] buffer2 = (byte[])operationResponse.Parameters[0];
				T t2 = SerializeHelper.Deserialize<T>(buffer2);
				result = t2;
			}
		}
		catch (Exception ex)
		{
			ClientLogger.Error(string.Concat(new object[]
			{
				"pvp GetProbufMsg error:",
				this.mID,
				"  type:",
				typeof(T),
				"   ",
				ex.ToString()
			}));
			result = (T)((object)null);
		}
		return result;
	}
}
