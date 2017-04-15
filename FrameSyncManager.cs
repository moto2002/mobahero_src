using MobaProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FrameSyncManager : MonoBehaviour
{
	public bool WaitFrameTime = true;

	public bool UseFrame;

	public double OneFrameTime = 0.03;

	public double TestSpeedUpFact = 60.0;

	public float DynamicDelaySpeed;

	private float DynamicDelayFact;

	public double Mode2DelayAdd = 0.06;

	public double Mode2DelayDec = 0.02;

	public double mLocalTime;

	public double mFrameTime;

	public double mNetTime;

	public double mPredictNetTime;

	public long newNetFrameNum;

	public long mNetFrameNum;

	public float mNetDeltaTime;

	public double TestTime;

	public float ScaleTime = 1f;

	private static readonly Queue<MobaMessage> mMessageQueue = new Queue<MobaMessage>();

	private static FrameSyncManager m_instance;

	private long swStartTime = Stopwatch.GetTimestamp();

	private long startTime = DateTime.Now.Ticks;

	public static FrameSyncManager Instance
	{
		get
		{
			if (FrameSyncManager.m_instance == null)
			{
				GameObject gameObject = GlobalObject.Instance.transform.Find("Tools").gameObject;
				if (gameObject != null && gameObject.GetComponent<FrameSyncManager>() == null)
				{
					FrameSyncManager.m_instance = gameObject.AddComponent<FrameSyncManager>();
				}
				else
				{
					UnityEngine.Debug.LogError("FrameSyncManager创建失败！");
				}
			}
			return FrameSyncManager.m_instance;
		}
	}

	private void Awake()
	{
		FrameSyncManager.m_instance = this;
	}

	public void setDelayTime(float min, float max, float speed = 0f)
	{
		this.Mode2DelayDec = (double)min;
		this.Mode2DelayAdd = (double)max;
		this.DynamicDelaySpeed = speed;
		this.DynamicDelayFact = 0f;
	}

	public void ReceiveFrame(long frameCnt)
	{
		this.newNetFrameNum = frameCnt;
		MobaMessage message = MobaMessageManager.GetMessage(PvpCode.P2C_FrameSync, frameCnt, 0f, null);
		this.ReceiveMsg(message);
	}

	public void ReceiveMsg(MobaMessage msg)
	{
		if (msg.ID == 83 || msg.ID == 72)
		{
			MobaMessageManager.ExecuteMsg(msg);
			return;
		}
		if (!FrameSyncManager.Instance.WaitFrameTime)
		{
			MobaMessageManager.ExecuteMsg(msg);
			return;
		}
		FrameSyncManager.mMessageQueue.Enqueue(msg);
	}

	public void SendOrder()
	{
	}

	public void ResetInfoOnOnBattleStart()
	{
		this.DynamicDelayFact = 0f;
		this.mLocalTime = 0.0;
		this.mFrameTime = 0.0;
		this.mNetTime = 0.0;
		this.mPredictNetTime = 0.0;
		this.newNetFrameNum = 0L;
		this.mNetFrameNum = 0L;
		this.mNetDeltaTime = 0f;
		this.TestTime = 0.0;
		this.ScaleTime = 1f;
	}

	private void Update()
	{
		GlobalObject.Instance.UpdateComps();
		float num = Time.deltaTime;
		if (Time.timeScale < 1f)
		{
			num /= Time.timeScale;
		}
		if (FrameSyncManager.Instance.WaitFrameTime)
		{
			this.mPredictNetTime += (double)(Time.deltaTime * 0.99f);
		}
		if (this.mNetFrameNum != this.newNetFrameNum)
		{
			if (this.mNetFrameNum > this.newNetFrameNum)
			{
				this.mLocalTime = 0.0;
			}
			this.mNetFrameNum = this.newNetFrameNum;
			if (FrameSyncManager.Instance.UseFrame)
			{
				this.mNetTime = (double)this.mNetFrameNum * this.OneFrameTime;
			}
			else
			{
				this.mNetTime = (double)this.mNetFrameNum * 1E-07;
			}
			if (this.mPredictNetTime < this.mNetTime)
			{
				this.mPredictNetTime = this.mNetTime;
			}
		}
		if (!FrameSyncManager.Instance.WaitFrameTime)
		{
			this.mPredictNetTime += (double)(Time.deltaTime * 0.99f);
		}
		if (this.mLocalTime == 0.0)
		{
			this.mLocalTime = this.mFrameTime;
			this.mPredictNetTime = this.mNetTime;
			this.mNetDeltaTime = 0f;
		}
		else
		{
			float num2 = 0f;
			this.TestTime = this.mPredictNetTime - this.mLocalTime - (double)num;
			if (this.mNetTime - this.mLocalTime - (double)num < this.Mode2DelayDec)
			{
				this.mNetDeltaTime = (float)((double)num - (this.Mode2DelayDec - (this.mNetTime - this.mLocalTime - (double)num)) * (double)num / ((double)num + this.Mode2DelayDec));
				if (this.mNetDeltaTime < 0f)
				{
					this.mNetDeltaTime = 0f;
				}
				else if (this.mLocalTime + (double)this.mNetDeltaTime > this.mNetTime)
				{
					this.mNetDeltaTime = (float)((this.mNetTime - this.mLocalTime) * 0.99);
				}
				num2 = (float)(1.0 - (this.mNetTime - this.mLocalTime - (double)num) / this.Mode2DelayDec);
			}
			else if (this.mPredictNetTime - this.mLocalTime - (double)num > this.Mode2DelayAdd)
			{
				this.mNetDeltaTime = (float)((double)num + (this.mPredictNetTime - this.mLocalTime - (double)num - this.Mode2DelayAdd) / this.TestSpeedUpFact);
			}
			else
			{
				this.mNetDeltaTime = num;
				num2 = -0.1f;
			}
			this.mLocalTime += (double)this.mNetDeltaTime;
			this.ScaleTime = this.mNetDeltaTime / num;
			if (this.DynamicDelaySpeed > 0f)
			{
				this.DynamicDelayFact = Mathf.Clamp(this.DynamicDelayFact + num2 * num * this.DynamicDelaySpeed, 0f, 1f);
				this.Mode2DelayAdd = (double)(0.05f + this.DynamicDelayFact * 0.15f);
				this.Mode2DelayDec = (double)(0.01f + this.DynamicDelayFact * 0.03f);
			}
		}
		if (Time.timeScale < 1f)
		{
			this.mNetDeltaTime *= Time.timeScale;
		}
		while (FrameSyncManager.mMessageQueue.Count > 0)
		{
			MobaMessage msg = FrameSyncManager.mMessageQueue.Dequeue();
			MobaMessageManager.ExecuteMsg(msg);
		}
		double num3 = (double)(Stopwatch.GetTimestamp() - this.swStartTime) / (double)Stopwatch.Frequency;
		double num4 = (double)(DateTime.Now.Ticks - this.startTime) / 10000000.0;
	}
}
