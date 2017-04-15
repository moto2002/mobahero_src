using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetController : UnitComponent
{
	private enum InterPState
	{
		Predicting,
		FittingCheck,
		FittingDoing
	}

	public static float sReportIntervalTime = 0.2f;

	public static int sReportStaticSnapTimeMax = 5;

	public static float sPredictMaxTime = 1f;

	public static float sFittingDistanceScope1 = 0.01f;

	public static float sFittingDistanceScope2 = 0.25f;

	public static float sRotateSpeed = 360f;

	public static float sAccelerationSpeed = 10f;

	public static bool sDebugOutput;

	private bool mIsReportMode;

	private int mReportIndex;

	private float mReportElapsedTime;

	private int mStaticSnapAccumNum;

	private NetController.InterPState mInterPCurrState;

	private UnitSnapInfo mInterPCatchSnap;

	private List<UnitSnapInfo> mInterPSnapList;

	private long mInterPLastTicks;

	private bool mInterPLastIsMoving;

	private Vector3 mInterPLastSpeed;

	private float mInterPLastRotateY;

	private float mPredictElapsedTime;

	private float mFittingElapsedTime;

	private float mFittingSmoothTime;

	private Vector3 mFittingStartPos;

	private Vector3 mFittingStartSpeed;

	private Vector3 mFittingEndPos;

	private Vector3 mFittingEndSpeed;

	private Vector3 mLastPosition = Vector3.zero;

	private float mLastRotateY;

	private float[] mDR_xParam;

	private float[] mDR_yParam;

	private float[] mDR_zParam;

	private Vector3 mDR_LastPos;

	private int mLN_SpeedState;

	private float mLN_AccSpeed;

	private float mLN_AccTime;

	private float mLN_VecTime;

	private float mLN_StartSpeed;

	private float mLN_EndSpeed;

	private float mTickForLastFrame;

	private float mDeltaForLastFrame;

	private long lastTick;

	public NetController()
	{
		this.mInterPSnapList = new List<UnitSnapInfo>();
		this.mDR_xParam = new float[4];
		this.mDR_yParam = new float[4];
		this.mDR_zParam = new float[4];
	}

	public NetController(Units self) : base(self)
	{
	}

	public override void OnInit()
	{
		bool reportMode = this.self.unitControlType == UnitControlType.PvpAIControl || this.self.unitControlType == UnitControlType.PvpMyControl;
		this.SetReportMode(reportMode);
		this.mInterPCurrState = NetController.InterPState.Predicting;
		this.mInterPLastTicks = 0L;
		this.mInterPLastIsMoving = false;
		this.mInterPLastSpeed = Vector3.zero;
		this.mInterPLastRotateY = 0f;
		this.mInterPCatchSnap = null;
		this.mInterPSnapList.Clear();
		this.mPredictElapsedTime = 0f;
	}

	public override void OnStart()
	{
	}

	public override void OnUpdate(float deltaTime)
	{
		long ticks = DateTime.Now.Ticks;
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			this.mDeltaForLastFrame = FrameSyncManager.Instance.mNetDeltaTime;
			return;
		}
		if (this.lastTick > 0L)
		{
			this.mDeltaForLastFrame = (float)(ticks - this.lastTick) / 1E+07f;
		}
		else
		{
			this.mDeltaForLastFrame = deltaTime;
		}
		if (this.self.isHero || this.self.isPlayer)
		{
			return;
		}
		this.mTickForLastFrame = (float)ticks;
	}

	public override void OnStop()
	{
	}

	public override void OnExit()
	{
	}

	public void SetReportMode(bool report)
	{
		this.mIsReportMode = report;
	}

	private void UnitSnapPredicting(ref float targetRotate)
	{
		if (this.mInterPLastIsMoving)
		{
			this.mPredictElapsedTime += this.mDeltaForLastFrame;
			if (this.mPredictElapsedTime > NetController.sPredictMaxTime)
			{
				this.mPredictElapsedTime = 0f;
				if (this.self.animController.IsInMove())
				{
					if (this.self.unique_id == 106)
					{
						Debug.LogError(Time.frameCount + ", Do Move Stop_Predict_Over_Time");
					}
					this.self.animController.PlayAnim(AnimationType.Move, false, 0, true, false);
				}
				this.mInterPLastIsMoving = false;
				if (this.self.unique_id == 106)
				{
					Debug.LogError(string.Concat(new object[]
					{
						Time.frameCount,
						"Predict over time , elapsed time : [",
						this.mPredictElapsedTime,
						"], max time : [",
						NetController.sPredictMaxTime,
						"], deltaTime : [",
						this.mDeltaForLastFrame,
						"]"
					}));
				}
			}
			else
			{
				base.transform.position += this.mInterPLastSpeed * this.mDeltaForLastFrame;
				if (this.self.animController.IsInIdle())
				{
					this.self.animController.PlayAnim(AnimationType.Move, true, 0, true, false);
				}
			}
		}
		else
		{
			this.mPredictElapsedTime = 0f;
			if (this.self.animController.IsInMove())
			{
				this.self.animController.PlayAnim(AnimationType.Move, false, 0, true, false);
			}
		}
		targetRotate = this.mInterPLastRotateY;
	}

	private void ChangeInterPCurrState(NetController.InterPState state)
	{
		this.mInterPCurrState = state;
	}

	private void Rotate(float from, float to, float speed)
	{
		bool flag = Mathf.Abs(from - to) > 0.001f;
		float num = this.mDeltaForLastFrame * speed;
		float num2 = to - from;
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		else if (num2 < -180f)
		{
			num2 += 360f;
		}
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		if (Mathf.Abs(num2) < num)
		{
			base.transform.Rotate(0f, num2, 0f);
		}
		else if (num2 > 0f)
		{
			base.transform.Rotate(0f, num, 0f);
		}
		else
		{
			base.transform.Rotate(0f, -num, 0f);
			base.transform.rotation.SetEulerRotation(eulerAngles.x, eulerAngles.y - num, eulerAngles.z);
		}
	}

	public void DeadReckoingParamInit(Vector3 startPos, Vector3 startSpeed, Vector3 endPos, Vector3 endSpeed, float smoothTime)
	{
		Vector3[] array = new Vector3[4];
		float[] array2 = new float[4];
		float[] array3 = new float[4];
		float[] array4 = new float[4];
		array[0] = startPos;
		array[1] = startPos + startSpeed * smoothTime;
		array[2] = endPos - endSpeed * smoothTime;
		array[3] = endPos;
		for (int i = 0; i < 4; i++)
		{
			array2[i] = array[i].x;
			array3[i] = array[i].y;
			array4[i] = array[i].z;
		}
		this.mDR_xParam[0] = array2[3] - 3f * array2[2] + 3f * array2[1] - array2[0];
		this.mDR_xParam[1] = 3f * array2[2] - 6f * array2[1] + 3f * array2[0];
		this.mDR_xParam[2] = 3f * array2[1] - 3f * array2[0];
		this.mDR_xParam[3] = array2[0];
		this.mDR_yParam[0] = array3[3] - 3f * array3[2] + 3f * array3[1] - array3[0];
		this.mDR_yParam[1] = 3f * array3[2] - 6f * array3[1] + 3f * array3[0];
		this.mDR_yParam[2] = 3f * array3[1] - 3f * array3[0];
		this.mDR_yParam[3] = array3[0];
		this.mDR_zParam[0] = array4[3] - 3f * array4[2] + 3f * array4[1] - array4[0];
		this.mDR_zParam[1] = 3f * array4[2] - 6f * array4[1] + 3f * array4[0];
		this.mDR_zParam[2] = 3f * array4[1] - 3f * array4[0];
		this.mDR_zParam[3] = array4[0];
		this.mDR_LastPos = startPos;
	}

	private void DeadReckoningSmoothPos(ref Vector3 fittingPos, ref float fittingRotateY)
	{
		float num = 1f;
		if (this.mFittingSmoothTime > 0f && this.mFittingElapsedTime >= 0f)
		{
			num = this.mFittingElapsedTime / this.mFittingSmoothTime;
		}
		if (num > 1f)
		{
			num = 1f;
		}
		fittingPos.x = this.mDR_xParam[0] * num * num * num + this.mDR_xParam[1] * num * num + this.mDR_xParam[2] * num + this.mDR_xParam[3];
		fittingPos.y = this.mDR_yParam[0] * num * num * num + this.mDR_yParam[1] * num * num + this.mDR_yParam[2] * num + this.mDR_yParam[3];
		fittingPos.z = this.mDR_zParam[0] * num * num * num + this.mDR_zParam[1] * num * num + this.mDR_zParam[2] * num + this.mDR_zParam[3];
		Vector3 vector = fittingPos - this.mDR_LastPos;
		Vector2 vector2 = new Vector2(vector.x, vector.z);
		if (vector2.magnitude > 0.001f)
		{
			vector2.Normalize();
			float x = vector2.x;
			float y = vector2.y;
			float num2 = 0f;
			if (Mathf.Abs(x) < 1E-05f)
			{
				if (y > 0f)
				{
					num2 = 0f;
				}
				else
				{
					num2 = 3.14159274f;
				}
			}
			else if (Mathf.Abs(y) < 1E-05f)
			{
				if (x > 0f)
				{
					num2 = 1.57079637f;
				}
				else
				{
					num2 = 4.712389f;
				}
			}
			else if (x > 0f && y > 0f)
			{
				num2 = Mathf.Acos(y);
			}
			else if (x > 0f && y < 0f)
			{
				num2 = 1.57079637f + Mathf.Acos(x);
			}
			else if (x < 0f && y < 0f)
			{
				num2 = 3.14159274f + Mathf.Acos(-y);
			}
			else if (x < 0f && y > 0f)
			{
				num2 = 4.712389f + Mathf.Acos(-x);
			}
			fittingRotateY = num2 / 3.14159274f * 180f;
		}
	}

	public void LinearParamInit(Vector3 startPos, Vector3 startSpeed, Vector3 endPos, Vector3 endSpeed, float smoothTime)
	{
		string text = string.Empty;
		this.mLN_StartSpeed = startSpeed.magnitude;
		this.mLN_EndSpeed = endSpeed.magnitude;
		string text2 = text;
		text = string.Concat(new object[]
		{
			text2,
			"Start Speed : [",
			this.mLN_StartSpeed,
			"], End Speed : [",
			this.mLN_EndSpeed,
			"], SmoothTime ：[",
			smoothTime,
			"], "
		});
		if (Mathf.Abs(this.mLN_StartSpeed - this.mLN_EndSpeed) < 0.001f)
		{
			this.mLN_SpeedState = 0;
			this.mLN_AccSpeed = 0f;
			this.mLN_AccTime = 0f;
			this.mLN_VecTime = smoothTime;
		}
		else if (this.mLN_EndSpeed > this.mLN_StartSpeed)
		{
			this.mLN_SpeedState = 1;
			this.mLN_AccSpeed = NetController.sAccelerationSpeed;
			float num = (this.mLN_EndSpeed - this.mLN_StartSpeed) / this.mLN_AccSpeed;
			if (num > smoothTime)
			{
				this.mLN_AccTime = smoothTime;
				this.mLN_AccSpeed = (this.mLN_EndSpeed - this.mLN_StartSpeed) / smoothTime;
			}
			else
			{
				this.mLN_AccTime = num;
			}
			this.mLN_VecTime = smoothTime - this.mLN_AccTime;
		}
		else
		{
			this.mLN_SpeedState = 2;
			this.mLN_AccSpeed = NetController.sAccelerationSpeed * -1f;
			float num2 = (this.mLN_EndSpeed - this.mLN_StartSpeed) / this.mLN_AccSpeed;
			if (num2 > smoothTime)
			{
				this.mLN_AccTime = smoothTime;
				this.mLN_AccSpeed = (this.mLN_EndSpeed - this.mLN_StartSpeed) / smoothTime;
			}
			else
			{
				this.mLN_AccTime = num2;
			}
			this.mLN_VecTime = smoothTime - this.mLN_AccTime;
		}
		text2 = text;
		text = string.Concat(new object[]
		{
			text2,
			"Speed State : [",
			this.mLN_SpeedState,
			"], Acc Speed : [",
			this.mLN_AccSpeed,
			"], Acc Time : [",
			this.mLN_AccTime,
			"], Vec Time : [",
			this.mLN_VecTime,
			"]"
		});
	}

	private void LinearSmoothPos(ref Vector3 fittingPos, ref float fittingRotateY)
	{
		float d = 0f;
		switch (this.mLN_SpeedState)
		{
		case 0:
			d = this.mLN_StartSpeed * this.mFittingElapsedTime;
			break;
		case 1:
			if (this.mFittingElapsedTime <= this.mLN_AccTime)
			{
				float num = this.mFittingElapsedTime;
				d = this.mLN_StartSpeed * num + 0.5f * this.mLN_AccSpeed * num * num;
			}
			else
			{
				float num2 = this.mLN_AccTime;
				float num3 = this.mFittingElapsedTime - this.mLN_AccTime;
				d = this.mLN_StartSpeed * num2 + 0.5f * this.mLN_AccSpeed * num2 * num2 + this.mLN_EndSpeed * num3;
			}
			break;
		case 2:
			if (this.mFittingElapsedTime <= this.mLN_VecTime)
			{
				float num4 = this.mFittingElapsedTime;
				d = this.mLN_StartSpeed * num4;
			}
			else
			{
				float num5 = this.mLN_VecTime;
				float num6 = this.mFittingElapsedTime - this.mLN_VecTime;
				d = this.mLN_StartSpeed * num6 + 0.5f * this.mLN_AccSpeed * num6 * num6 + this.mLN_StartSpeed * num5;
			}
			break;
		}
		fittingPos = this.mFittingStartPos + d * (this.mFittingEndPos - this.mFittingStartPos).normalized;
		Vector3 vector = this.mFittingEndPos - this.mFittingStartPos;
		Vector2 vector2 = new Vector2(vector.x, vector.z);
		if (vector2.magnitude > 0.001f)
		{
			vector2.Normalize();
			float x = vector2.x;
			float y = vector2.y;
			float num7 = 0f;
			if (Mathf.Abs(x) < 1E-05f)
			{
				if (y > 0f)
				{
					num7 = 0f;
				}
				else
				{
					num7 = 3.14159274f;
				}
			}
			else if (Mathf.Abs(y) < 1E-05f)
			{
				if (x > 0f)
				{
					num7 = 1.57079637f;
				}
				else
				{
					num7 = 4.712389f;
				}
			}
			else if (x > 0f && y > 0f)
			{
				num7 = Mathf.Acos(y);
			}
			else if (x > 0f && y < 0f)
			{
				num7 = 1.57079637f + Mathf.Acos(x);
			}
			else if (x < 0f && y < 0f)
			{
				num7 = 3.14159274f + Mathf.Acos(-y);
			}
			else if (x < 0f && y > 0f)
			{
				num7 = 4.712389f + Mathf.Acos(-x);
			}
			fittingRotateY = num7 / 3.14159274f * 180f;
		}
	}
}
