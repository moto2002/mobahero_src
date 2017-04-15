using System;
using System.Collections.Generic;
using UnityEngine;

public class NcTrailTexture : NcEffectBehaviour
{
	public enum AXIS_TYPE
	{
		AXIS_FORWARD,
		AXIS_BACK,
		AXIS_RIGHT,
		AXIS_LEFT,
		AXIS_UP,
		AXIS_DOWN
	}

	public class Point
	{
		public float timeCreated;

		public Vector3 basePosition;

		public Vector3 tipPosition;

		public bool lineBreak;
	}

	public float m_fDelayTime;

	public float m_fEmitTime;

	public bool m_bSmoothHide = true;

	protected bool m_bEmit = true;

	protected float m_fStartTime;

	protected float m_fStopTime;

	public float m_fLifeTime = 0.7f;

	public NcTrailTexture.AXIS_TYPE m_TipAxis = NcTrailTexture.AXIS_TYPE.AXIS_BACK;

	public float m_fTipSize = 1f;

	public bool m_bCenterAlign;

	public bool m_UvFlipHorizontal;

	public bool m_UvFlipVirtical;

	public int m_nFadeHeadCount = 2;

	public int m_nFadeTailCount = 2;

	public Color[] m_Colors;

	public float[] m_SizeRates;

	public bool m_bInterpolation;

	public int m_nMaxSmoothCount = 10;

	public int m_nSubdivisions = 4;

	protected List<NcTrailTexture.Point> m_SmoothedPoints = new List<NcTrailTexture.Point>();

	public float m_fMinVertexDistance = 0.2f;

	public float m_fMaxVertexDistance = 10f;

	public float m_fMaxAngle = 3f;

	public bool m_bAutoDestruct;

	protected List<NcTrailTexture.Point> m_Points = new List<NcTrailTexture.Point>();

	protected Transform m_base;

	protected GameObject m_TrialObject;

	protected Mesh m_TrailMesh;

	protected Vector3 m_LastPosition;

	protected Vector3 m_LastCameraPosition1;

	protected Vector3 m_LastCameraPosition2;

	protected bool m_bLastFrameEmit = true;

	public void SetEmit(bool bEmit)
	{
		this.m_bEmit = bEmit;
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		this.m_fStopTime = 0f;
	}

	public override int GetAnimationState()
	{
		if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (NcEffectBehaviour.GetEngineTime() < this.m_fStartTime + this.m_fDelayTime + 0.1f)
		{
			return 1;
		}
		return -1;
	}

	private void OnDisable()
	{
		if (this.m_TrialObject != null)
		{
			NcAutoDestruct.CreateAutoDestruct(this.m_TrialObject, 0f, this.m_fLifeTime / 2f, true, true);
		}
	}

	private void Start()
	{
		if (base.renderer == null || base.renderer.sharedMaterial == null)
		{
			base.enabled = false;
			return;
		}
		if (0f < this.m_fDelayTime)
		{
			this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		}
		else
		{
			this.InitTrailObject();
		}
	}

	private void InitTrailObject()
	{
		this.m_base = base.transform;
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		this.m_LastPosition = base.transform.position;
		this.m_TrialObject = new GameObject("Trail");
		this.m_TrialObject.transform.position = Vector3.zero;
		this.m_TrialObject.transform.rotation = Quaternion.identity;
		this.m_TrialObject.transform.localScale = base.transform.localScale;
		this.m_TrialObject.AddComponent(typeof(MeshFilter));
		this.m_TrialObject.AddComponent(typeof(MeshRenderer));
		this.m_TrialObject.renderer.sharedMaterial = base.renderer.sharedMaterial;
		this.m_TrailMesh = this.m_TrialObject.GetComponent<MeshFilter>().mesh;
		base.CreateEditorGameObject(this.m_TrialObject);
	}

	private Vector3 GetTipPoint()
	{
		switch (this.m_TipAxis)
		{
		case NcTrailTexture.AXIS_TYPE.AXIS_FORWARD:
			return this.m_base.position + this.m_base.forward;
		case NcTrailTexture.AXIS_TYPE.AXIS_BACK:
			return this.m_base.position + this.m_base.forward * -1f;
		case NcTrailTexture.AXIS_TYPE.AXIS_RIGHT:
			return this.m_base.position + this.m_base.right;
		case NcTrailTexture.AXIS_TYPE.AXIS_LEFT:
			return this.m_base.position + this.m_base.right * -1f;
		case NcTrailTexture.AXIS_TYPE.AXIS_UP:
			return this.m_base.position + this.m_base.up;
		case NcTrailTexture.AXIS_TYPE.AXIS_DOWN:
			return this.m_base.position + this.m_base.up * -1f;
		default:
			return this.m_base.position + this.m_base.forward;
		}
	}

	private void Update()
	{
		if (base.renderer == null || base.renderer.sharedMaterial == null)
		{
			base.enabled = false;
			return;
		}
		if (0f < this.m_fDelayTime)
		{
			if (NcEffectBehaviour.GetEngineTime() < this.m_fStartTime + this.m_fDelayTime)
			{
				return;
			}
			this.m_fDelayTime = 0f;
			this.m_fStartTime = 0f;
			this.InitTrailObject();
		}
		if (this.m_bEmit && 0f < this.m_fEmitTime && this.m_fStopTime == 0f && this.m_fStartTime + this.m_fEmitTime < NcEffectBehaviour.GetEngineTime())
		{
			if (this.m_bSmoothHide)
			{
				this.m_fStopTime = NcEffectBehaviour.GetEngineTime();
			}
			else
			{
				this.m_bEmit = false;
			}
		}
		if (0f < this.m_fStopTime && this.m_fLifeTime < NcEffectBehaviour.GetEngineTime() - this.m_fStopTime)
		{
			this.m_bEmit = false;
		}
		if (!this.m_bEmit && this.m_Points.Count == 0 && this.m_bAutoDestruct)
		{
			UnityEngine.Object.Destroy(this.m_TrialObject);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		float magnitude = (this.m_LastPosition - base.transform.position).magnitude;
		if (this.m_bEmit)
		{
			if (magnitude > this.m_fMinVertexDistance)
			{
				bool flag = false;
				if (this.m_Points.Count < 3)
				{
					flag = true;
				}
				else
				{
					Vector3 from = this.m_Points[this.m_Points.Count - 2].basePosition - this.m_Points[this.m_Points.Count - 3].basePosition;
					Vector3 to = this.m_Points[this.m_Points.Count - 1].basePosition - this.m_Points[this.m_Points.Count - 2].basePosition;
					if (Vector3.Angle(from, to) > this.m_fMaxAngle || magnitude > this.m_fMaxVertexDistance)
					{
						flag = true;
					}
				}
				if (flag)
				{
					NcTrailTexture.Point point = new NcTrailTexture.Point();
					point.basePosition = this.m_base.position;
					point.tipPosition = this.GetTipPoint();
					if (0f < this.m_fStopTime)
					{
						point.timeCreated = NcEffectBehaviour.GetEngineTime() - (NcEffectBehaviour.GetEngineTime() - this.m_fStopTime);
					}
					else
					{
						point.timeCreated = NcEffectBehaviour.GetEngineTime();
					}
					this.m_Points.Add(point);
					this.m_LastPosition = base.transform.position;
					if (this.m_bInterpolation)
					{
						if (this.m_Points.Count == 1)
						{
							this.m_SmoothedPoints.Add(point);
						}
						else if (1 < this.m_Points.Count)
						{
							for (int i = 0; i < 1 + this.m_nSubdivisions; i++)
							{
								this.m_SmoothedPoints.Add(point);
							}
						}
						int num = 2;
						if (num <= this.m_Points.Count)
						{
							int num2 = Mathf.Min(this.m_nMaxSmoothCount, this.m_Points.Count);
							Vector3[] array = new Vector3[num2];
							for (int j = 0; j < num2; j++)
							{
								array[j] = this.m_Points[this.m_Points.Count - (num2 - j)].basePosition;
							}
							IEnumerable<Vector3> collection = NgInterpolate.NewCatmullRom(array, this.m_nSubdivisions, false);
							Vector3[] array2 = new Vector3[num2];
							for (int k = 0; k < num2; k++)
							{
								array2[k] = this.m_Points[this.m_Points.Count - (num2 - k)].tipPosition;
							}
							IEnumerable<Vector3> collection2 = NgInterpolate.NewCatmullRom(array2, this.m_nSubdivisions, false);
							List<Vector3> list = new List<Vector3>(collection);
							List<Vector3> list2 = new List<Vector3>(collection2);
							float timeCreated = this.m_Points[this.m_Points.Count - num2].timeCreated;
							float timeCreated2 = this.m_Points[this.m_Points.Count - 1].timeCreated;
							for (int l = 0; l < list.Count; l++)
							{
								int num3 = this.m_SmoothedPoints.Count - (list.Count - l);
								if (-1 < num3 && num3 < this.m_SmoothedPoints.Count)
								{
									NcTrailTexture.Point point2 = new NcTrailTexture.Point();
									point2.tipPosition = list2[l];
									point2.basePosition = list[l];
									point2.timeCreated = Mathf.Lerp(timeCreated, timeCreated2, (float)l / (float)list.Count);
									this.m_SmoothedPoints[num3] = point2;
								}
							}
						}
					}
				}
				else
				{
					this.m_Points[this.m_Points.Count - 1].tipPosition = this.GetTipPoint();
					this.m_Points[this.m_Points.Count - 1].basePosition = this.m_base.position;
					if (this.m_bInterpolation)
					{
						this.m_SmoothedPoints[this.m_SmoothedPoints.Count - 1].tipPosition = this.GetTipPoint();
						this.m_SmoothedPoints[this.m_SmoothedPoints.Count - 1].basePosition = this.m_base.position;
					}
				}
			}
			else
			{
				if (this.m_Points.Count > 0)
				{
					this.m_Points[this.m_Points.Count - 1].tipPosition = this.GetTipPoint();
					this.m_Points[this.m_Points.Count - 1].basePosition = this.m_base.position;
				}
				if (this.m_bInterpolation && this.m_SmoothedPoints.Count > 0)
				{
					this.m_SmoothedPoints[this.m_SmoothedPoints.Count - 1].tipPosition = this.GetTipPoint();
					this.m_SmoothedPoints[this.m_SmoothedPoints.Count - 1].basePosition = this.m_base.position;
				}
			}
		}
		if (!this.m_bEmit && this.m_bLastFrameEmit && this.m_Points.Count > 0)
		{
			this.m_Points[this.m_Points.Count - 1].lineBreak = true;
		}
		this.m_bLastFrameEmit = this.m_bEmit;
		List<NcTrailTexture.Point> list3 = new List<NcTrailTexture.Point>();
		foreach (NcTrailTexture.Point current in this.m_Points)
		{
			if (NcEffectBehaviour.GetEngineTime() - current.timeCreated > this.m_fLifeTime)
			{
				list3.Add(current);
			}
		}
		foreach (NcTrailTexture.Point current2 in list3)
		{
			this.m_Points.Remove(current2);
		}
		if (this.m_bInterpolation)
		{
			list3 = new List<NcTrailTexture.Point>();
			foreach (NcTrailTexture.Point current3 in this.m_SmoothedPoints)
			{
				if (NcEffectBehaviour.GetEngineTime() - current3.timeCreated > this.m_fLifeTime)
				{
					list3.Add(current3);
				}
			}
			foreach (NcTrailTexture.Point current4 in list3)
			{
				this.m_SmoothedPoints.Remove(current4);
			}
		}
		List<NcTrailTexture.Point> list4;
		if (this.m_bInterpolation)
		{
			list4 = this.m_SmoothedPoints;
		}
		else
		{
			list4 = this.m_Points;
		}
		if (list4.Count > 1)
		{
			Vector3[] array3 = new Vector3[list4.Count * 2];
			Vector2[] array4 = new Vector2[list4.Count * 2];
			int[] array5 = new int[(list4.Count - 1) * 6];
			Color[] array6 = new Color[list4.Count * 2];
			for (int m = 0; m < list4.Count; m++)
			{
				NcTrailTexture.Point point3 = list4[m];
				float num4 = (NcEffectBehaviour.GetEngineTime() - point3.timeCreated) / this.m_fLifeTime;
				Color color = Color.Lerp(Color.white, Color.clear, num4);
				if (this.m_Colors != null && this.m_Colors.Length > 0)
				{
					float num5 = num4 * (float)(this.m_Colors.Length - 1);
					float num6 = Mathf.Floor(num5);
					float num7 = Mathf.Clamp(Mathf.Ceil(num5), 1f, (float)(this.m_Colors.Length - 1));
					float t = Mathf.InverseLerp(num6, num7, num5);
					if (num6 >= (float)this.m_Colors.Length)
					{
						num6 = (float)(this.m_Colors.Length - 1);
					}
					if (num6 < 0f)
					{
						num6 = 0f;
					}
					if (num7 >= (float)this.m_Colors.Length)
					{
						num7 = (float)(this.m_Colors.Length - 1);
					}
					if (num7 < 0f)
					{
						num7 = 0f;
					}
					color = Color.Lerp(this.m_Colors[(int)num6], this.m_Colors[(int)num7], t);
				}
				Vector3 a = point3.basePosition - point3.tipPosition;
				float num8 = this.m_fTipSize;
				if (this.m_SizeRates != null && this.m_SizeRates.Length > 0)
				{
					float num9 = num4 * (float)(this.m_SizeRates.Length - 1);
					float num10 = Mathf.Floor(num9);
					float num11 = Mathf.Clamp(Mathf.Ceil(num9), 1f, (float)(this.m_SizeRates.Length - 1));
					float t2 = Mathf.InverseLerp(num10, num11, num9);
					if (num10 >= (float)this.m_SizeRates.Length)
					{
						num10 = (float)(this.m_SizeRates.Length - 1);
					}
					if (num10 < 0f)
					{
						num10 = 0f;
					}
					if (num11 >= (float)this.m_SizeRates.Length)
					{
						num11 = (float)(this.m_SizeRates.Length - 1);
					}
					if (num11 < 0f)
					{
						num11 = 0f;
					}
					num8 *= Mathf.Lerp(this.m_SizeRates[(int)num10], this.m_SizeRates[(int)num11], t2);
				}
				if (this.m_bCenterAlign)
				{
					array3[m * 2] = point3.basePosition - a * (num8 * 0.5f);
					array3[m * 2 + 1] = point3.basePosition + a * (num8 * 0.5f);
				}
				else
				{
					array3[m * 2] = point3.basePosition - a * num8;
					array3[m * 2 + 1] = point3.basePosition;
				}
				int num12 = (!this.m_bInterpolation) ? this.m_nFadeTailCount : (this.m_nFadeTailCount * this.m_nSubdivisions);
				int num13 = (!this.m_bInterpolation) ? this.m_nFadeHeadCount : (this.m_nFadeHeadCount * this.m_nSubdivisions);
				if (0 < num12 && m <= num12)
				{
					color.a = color.a * (float)m / (float)num12;
				}
				if (0 < num13 && list4.Count - (m + 1) <= num13)
				{
					color.a = color.a * (float)(list4.Count - (m + 1)) / (float)num13;
				}
				array6[m * 2] = (array6[m * 2 + 1] = color);
				float num14 = (float)m / (float)list4.Count;
				array4[m * 2] = new Vector2((!this.m_UvFlipHorizontal) ? num14 : (1f - num14), (float)((!this.m_UvFlipVirtical) ? 0 : 1));
				array4[m * 2 + 1] = new Vector2((!this.m_UvFlipHorizontal) ? num14 : (1f - num14), (float)((!this.m_UvFlipVirtical) ? 1 : 0));
				if (m > 0)
				{
					array5[(m - 1) * 6] = m * 2 - 2;
					array5[(m - 1) * 6 + 1] = m * 2 - 1;
					array5[(m - 1) * 6 + 2] = m * 2;
					array5[(m - 1) * 6 + 3] = m * 2 + 1;
					array5[(m - 1) * 6 + 4] = m * 2;
					array5[(m - 1) * 6 + 5] = m * 2 - 1;
				}
			}
			this.m_TrailMesh.Clear();
			this.m_TrailMesh.vertices = array3;
			this.m_TrailMesh.colors = array6;
			this.m_TrailMesh.uv = array4;
			this.m_TrailMesh.triangles = array5;
		}
		else
		{
			this.m_TrailMesh.Clear();
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime /= fSpeedRate;
		this.m_fEmitTime /= fSpeedRate;
		this.m_fLifeTime /= fSpeedRate;
	}
}
