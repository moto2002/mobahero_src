using System;
using UnityEngine;

public class FxmTestSimulate : MonoBehaviour
{
	public enum MODE_TYPE
	{
		NONE,
		MOVE,
		ARC,
		ROTATE,
		TORNADO,
		SCALE
	}

	public FxmTestSimulate.MODE_TYPE m_Mode;

	public FxmTestControls.AXIS m_nAxis;

	public float m_fStartTime;

	public Vector3 m_StartPos;

	public Vector3 m_EndPos;

	public float m_fSpeed;

	public bool m_bRotFront;

	public float m_fDist;

	public float m_fRadius;

	public float m_fArcLenRate;

	public AnimationCurve m_Curve;

	public Component m_FXMakerControls;

	public int m_nMultiShotIndex;

	public int m_nMultiShotCount;

	public int m_nCircleCount;

	public Vector3 m_PrevPosition = Vector3.zero;

	protected static int m_nMultiShotCreate;

	public void Init(Component fxmEffectControls, int nMultiShotCount)
	{
		this.m_FXMakerControls = fxmEffectControls;
		this.m_nMultiShotCount = nMultiShotCount;
	}

	public void SimulateMove(FxmTestControls.AXIS nTransAxis, float fHalfDist, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		this.m_nAxis = nTransAxis;
		this.m_StartPos = position;
		this.m_EndPos = position;
		int nAxis;
		int expr_2F = nAxis = (int)this.m_nAxis;
		float num = this.m_StartPos[nAxis];
		this.m_StartPos[expr_2F] = num - fHalfDist;
		int expr_50 = nAxis = (int)this.m_nAxis;
		num = this.m_EndPos[nAxis];
		this.m_EndPos[expr_50] = num + fHalfDist;
		this.m_fDist = Vector3.Distance(this.m_StartPos, this.m_EndPos);
		this.m_Mode = FxmTestSimulate.MODE_TYPE.MOVE;
		this.SimulateStart(this.m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateArc(float fHalfDist, float fSpeed, bool bRotFront)
	{
		this.m_Curve = FxmTestMain.inst.m_SimulateArcCurve;
		if (this.m_Curve == null)
		{
			Debug.LogError("FXMakerOption.m_SimulateArcCurve is null !!!!");
			return;
		}
		Vector3 position = base.transform.position;
		this.m_StartPos = new Vector3(position.x - fHalfDist, position.y, position.z);
		this.m_EndPos = new Vector3(position.x + fHalfDist, position.y, position.z);
		this.m_fDist = Vector3.Distance(this.m_StartPos, this.m_EndPos);
		this.m_Mode = FxmTestSimulate.MODE_TYPE.ARC;
		this.SimulateStart(this.m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateFall(float fHeight, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		this.m_StartPos = new Vector3(position.x, position.y + fHeight, position.z);
		this.m_EndPos = new Vector3(position.x, position.y, position.z);
		this.m_fDist = Vector3.Distance(this.m_StartPos, this.m_EndPos);
		this.m_Mode = FxmTestSimulate.MODE_TYPE.MOVE;
		this.SimulateStart(this.m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateRaise(float fHeight, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		this.m_StartPos = new Vector3(position.x, position.y, position.z);
		this.m_EndPos = new Vector3(position.x, position.y + fHeight, position.z);
		this.m_fDist = Vector3.Distance(this.m_StartPos, this.m_EndPos);
		this.m_Mode = FxmTestSimulate.MODE_TYPE.MOVE;
		this.SimulateStart(this.m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateCircle(float fRadius, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		this.m_fRadius = fRadius;
		this.m_Mode = FxmTestSimulate.MODE_TYPE.ROTATE;
		this.m_fDist = 1f;
		this.SimulateStart(new Vector3(position.x - fRadius, position.y, position.z), fSpeed, bRotFront);
	}

	public void SimulateTornado(float fRadius, float fHeight, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		this.m_fRadius = fRadius;
		this.m_Mode = FxmTestSimulate.MODE_TYPE.TORNADO;
		this.m_StartPos = new Vector3(position.x - fRadius, position.y, position.z);
		this.m_EndPos = new Vector3(position.x - fRadius, position.y + fHeight, position.z);
		this.m_fDist = Vector3.Distance(this.m_StartPos, this.m_EndPos);
		this.SimulateStart(this.m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateScale(FxmTestControls.AXIS nTransAxis, float fHalfDist, float fStartPosition, float fSpeed, bool bRotFront)
	{
		Vector3 position = base.transform.position;
		this.m_nAxis = nTransAxis;
		this.m_StartPos = position;
		this.m_EndPos = position;
		int nAxis;
		int expr_2F = nAxis = (int)this.m_nAxis;
		float num = this.m_StartPos[nAxis];
		this.m_StartPos[expr_2F] = num + fHalfDist * fStartPosition;
		int expr_52 = nAxis = (int)this.m_nAxis;
		num = this.m_EndPos[nAxis];
		this.m_EndPos[expr_52] = num + (fHalfDist * 2f + fHalfDist * fStartPosition);
		this.m_fDist = Vector3.Distance(this.m_StartPos, this.m_EndPos);
		this.m_Mode = FxmTestSimulate.MODE_TYPE.SCALE;
		this.SimulateStart(this.m_StartPos, fSpeed, bRotFront);
	}

	public void Stop()
	{
		this.m_fSpeed = 0f;
	}

	private void SimulateStart(Vector3 startPos, float fSpeed, bool bRotFront)
	{
		base.transform.position = startPos;
		this.m_fSpeed = fSpeed;
		this.m_bRotFront = bRotFront;
		this.m_nCircleCount = 0;
		this.m_PrevPosition = Vector3.zero;
		if (bRotFront && this.m_Mode == FxmTestSimulate.MODE_TYPE.MOVE)
		{
			base.transform.LookAt(this.m_EndPos);
		}
		if (this.m_Mode != FxmTestSimulate.MODE_TYPE.SCALE && 1 < this.m_nMultiShotCount)
		{
			NcDuplicator ncDuplicator = base.gameObject.AddComponent<NcDuplicator>();
			ncDuplicator.m_fDuplicateTime = 0.2f;
			ncDuplicator.m_nDuplicateCount = this.m_nMultiShotCount;
			ncDuplicator.m_fDuplicateLifeTime = 0f;
			FxmTestSimulate.m_nMultiShotCreate = 0;
			this.m_nMultiShotIndex = 0;
		}
		this.m_fStartTime = Time.time;
		this.Update();
	}

	private Vector3 GetArcPos(float fTimeRate)
	{
		Vector3 vector = Vector3.Lerp(this.m_StartPos, this.m_EndPos, fTimeRate);
		return new Vector3(vector.x, this.m_Curve.Evaluate(fTimeRate) * this.m_fDist, vector.z);
	}

	private void Awake()
	{
		this.m_nMultiShotIndex = FxmTestSimulate.m_nMultiShotCreate;
		FxmTestSimulate.m_nMultiShotCreate++;
	}

	private void Start()
	{
		this.m_fStartTime = Time.time;
	}

	private void Update()
	{
		if (0f < this.m_fDist && 0f < this.m_fSpeed)
		{
			switch (this.m_Mode)
			{
			case FxmTestSimulate.MODE_TYPE.MOVE:
			{
				float num = this.m_fDist / this.m_fSpeed;
				float num2 = Time.time - this.m_fStartTime;
				base.transform.position = Vector3.Lerp(this.m_StartPos, this.m_EndPos, num2 / num);
				if (1f < num2 / num)
				{
					this.OnMoveEnd();
				}
				break;
			}
			case FxmTestSimulate.MODE_TYPE.ARC:
			{
				float num3 = this.m_fDist / this.m_fSpeed;
				float num4 = Time.time - this.m_fStartTime;
				Vector3 arcPos = this.GetArcPos(num4 / num3 + num4 / num3 * 0.01f);
				base.transform.position = this.GetArcPos(num4 / num3);
				if (this.m_bRotFront)
				{
					base.transform.LookAt(arcPos);
				}
				if (1f < num4 / num3)
				{
					this.OnMoveEnd();
				}
				break;
			}
			case FxmTestSimulate.MODE_TYPE.ROTATE:
			{
				float num5 = this.m_fSpeed / 3.14f * 360f;
				base.transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * ((this.m_fRadius != 0f) ? (num5 / (this.m_fRadius * 2f)) : 0f));
				if (this.m_PrevPosition.z < 0f && 0f < base.transform.position.z)
				{
					if (1 <= this.m_nCircleCount)
					{
						this.OnMoveEnd();
					}
					this.m_nCircleCount++;
				}
				break;
			}
			case FxmTestSimulate.MODE_TYPE.TORNADO:
			{
				float num6 = this.m_fDist / (this.m_fSpeed / 20f);
				float num7 = Time.time - this.m_fStartTime;
				Vector3 vector = Vector3.Lerp(this.m_StartPos, this.m_EndPos, num7 / num6);
				base.transform.position = new Vector3(base.transform.position.x, vector.y, base.transform.position.z);
				float num8 = this.m_fSpeed / 3.14f * 360f;
				base.transform.RotateAround(new Vector3(0f, vector.y, 0f), Vector3.up, Time.deltaTime * ((this.m_fRadius != 0f) ? (num8 / (this.m_fRadius * 2f)) : 0f));
				if (1f < num7 / num6)
				{
					this.OnMoveEnd();
				}
				break;
			}
			case FxmTestSimulate.MODE_TYPE.SCALE:
			{
				float num9 = this.m_fDist / this.m_fSpeed;
				float num10 = Time.time - this.m_fStartTime;
				Vector3 localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
				localScale[(int)this.m_nAxis] = this.m_fDist * num10 / num9;
				if (localScale[(int)this.m_nAxis] == 0f)
				{
					localScale[(int)this.m_nAxis] = 0.001f;
				}
				base.transform.localScale = localScale;
				if (1f < num10 / num9)
				{
					this.OnMoveEnd();
				}
				break;
			}
			}
		}
		this.m_PrevPosition = base.transform.position;
	}

	private void FixedUpdate()
	{
	}

	public void LateUpdate()
	{
	}

	private void OnMoveEnd()
	{
		this.m_fSpeed = 0f;
		NgObject.SetActiveRecursively(base.gameObject, false);
		if (1 < FxmTestSimulate.m_nMultiShotCreate && this.m_nMultiShotIndex < FxmTestSimulate.m_nMultiShotCreate - 1)
		{
			return;
		}
		if (this.m_FXMakerControls != null)
		{
			this.m_FXMakerControls.SendMessage("OnActionTransEnd");
		}
	}
}
