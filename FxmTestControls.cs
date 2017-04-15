using System;
using UnityEngine;

public class FxmTestControls : MonoBehaviour
{
	public enum AXIS
	{
		X,
		Y,
		Z
	}

	protected const int m_nRepeatIndex = 3;

	public bool m_bMinimize;

	protected int m_nTriangles;

	protected int m_nVertices;

	protected int m_nMeshCount;

	protected int m_nParticleCount;

	protected int m_nPlayIndex;

	protected int m_nTransIndex;

	protected float[] m_fPlayToolbarTimes = new float[]
	{
		1f,
		1f,
		1f,
		0.3f,
		0.6f,
		1f,
		2f,
		3f
	};

	protected FxmTestControls.AXIS m_nTransAxis = FxmTestControls.AXIS.Z;

	protected float m_fDelayCreateTime = 0.2f;

	protected bool m_bCalledDelayCreate;

	protected bool m_bStartAliveAnimation;

	protected float m_fDistPerTime = 10f;

	protected int m_nRotateIndex;

	protected int m_nMultiShotCount = 1;

	protected float m_fTransRate = 1f;

	protected float m_fStartPosition;

	public float m_fTimeScale = 1f;

	protected float m_fPlayStartTime;

	protected float m_fOldTimeScale = 1f;

	protected float m_fCreateTime;

	public float GetTimeScale()
	{
		return this.m_fTimeScale;
	}

	public bool IsRepeat()
	{
		return 3 <= this.m_nPlayIndex;
	}

	public bool IsAutoRepeat()
	{
		return this.m_nPlayIndex == 0;
	}

	public float GetRepeatTime()
	{
		return this.m_fPlayToolbarTimes[this.m_nPlayIndex];
	}

	public void SetStartTime()
	{
		this.m_fPlayStartTime = Time.time;
	}

	private void LoadPrefs()
	{
		this.m_nPlayIndex = PlayerPrefs.GetInt("FxmTestControls.m_nPlayIndex", this.m_nPlayIndex);
		this.m_nTransIndex = PlayerPrefs.GetInt("FxmTestControls.m_nTransIndex", this.m_nTransIndex);
		this.m_fTimeScale = PlayerPrefs.GetFloat("FxmTestControls.m_fTimeScale", this.m_fTimeScale);
		this.m_fDistPerTime = PlayerPrefs.GetFloat("FxmTestControls.m_fDistPerTime", this.m_fDistPerTime);
		this.m_nRotateIndex = PlayerPrefs.GetInt("FxmTestControls.m_nRotateIndex", this.m_nRotateIndex);
		this.m_nTransAxis = (FxmTestControls.AXIS)PlayerPrefs.GetInt("FxmTestControls.m_nTransAxis", (int)this.m_nTransAxis);
		this.m_bMinimize = (PlayerPrefs.GetInt("FxmTestControls.m_bMinimize", (!this.m_bMinimize) ? 0 : 1) == 1);
		this.SetTimeScale(this.m_fTimeScale);
	}

	private void SavePrefs()
	{
		PlayerPrefs.SetInt("FxmTestControls.m_nPlayIndex", this.m_nPlayIndex);
		PlayerPrefs.SetInt("FxmTestControls.m_nTransIndex", this.m_nTransIndex);
		PlayerPrefs.SetFloat("FxmTestControls.m_fTimeScale", this.m_fTimeScale);
		PlayerPrefs.SetFloat("FxmTestControls.m_fDistPerTime", this.m_fDistPerTime);
		PlayerPrefs.SetInt("FxmTestControls.m_nRotateIndex", this.m_nRotateIndex);
		PlayerPrefs.SetInt("FxmTestControls.m_nTransAxis", (int)this.m_nTransAxis);
	}

	public void SetDefaultSetting()
	{
		this.m_nPlayIndex = 0;
		this.m_nTransIndex = 0;
		this.m_nTransAxis = FxmTestControls.AXIS.Z;
		this.m_fDistPerTime = 10f;
		this.m_nRotateIndex = 0;
		this.m_nMultiShotCount = 1;
		this.m_fTransRate = 1f;
		this.m_fStartPosition = 0f;
		this.SavePrefs();
	}

	public void AutoSetting(int nPlayIndex, int nTransIndex, FxmTestControls.AXIS nTransAxis, float fDistPerTime, int nRotateIndex, int nMultiShotCount, float fTransRate, float fStartAdjustRate)
	{
		this.m_nPlayIndex = nPlayIndex;
		this.m_nTransIndex = nTransIndex;
		this.m_nTransAxis = nTransAxis;
		this.m_fDistPerTime = fDistPerTime;
		this.m_nRotateIndex = nRotateIndex;
		this.m_nMultiShotCount = nMultiShotCount;
		this.m_fTransRate = fTransRate;
		this.m_fStartPosition = fStartAdjustRate;
	}

	private void Awake()
	{
		NgUtil.LogDevelop("Awake - m_FXMakerControls");
		this.LoadPrefs();
	}

	private void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - m_FXMakerControls");
		this.LoadPrefs();
	}

	private void Start()
	{
	}

	private void Update()
	{
		this.m_fTimeScale = Time.timeScale;
		if (FxmTestMain.inst.GetInstanceEffectObject() == null && !this.IsAutoRepeat())
		{
			this.DelayCreateInstanceEffect(false);
		}
		else
		{
			NgObject.GetMeshInfo(NcEffectBehaviour.GetRootInstanceEffect(), true, out this.m_nVertices, out this.m_nTriangles, out this.m_nMeshCount);
			this.m_nParticleCount = 0;
			ParticleSystem[] componentsInChildren = NcEffectBehaviour.GetRootInstanceEffect().GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem particleSystem = array[i];
				this.m_nParticleCount += particleSystem.particleCount;
			}
			ParticleEmitter[] componentsInChildren2 = NcEffectBehaviour.GetRootInstanceEffect().GetComponentsInChildren<ParticleEmitter>();
			ParticleEmitter[] array2 = componentsInChildren2;
			for (int j = 0; j < array2.Length; j++)
			{
				ParticleEmitter particleEmitter = array2[j];
				this.m_nParticleCount += particleEmitter.particleCount;
			}
			if (this.m_fDelayCreateTime < Time.time - this.m_fPlayStartTime)
			{
				if (this.IsRepeat() && this.m_fCreateTime + this.GetRepeatTime() < Time.time)
				{
					this.DelayCreateInstanceEffect(false);
				}
				if (this.m_nTransIndex == 0 && this.IsAutoRepeat() && !this.m_bCalledDelayCreate && !this.IsAliveAnimation())
				{
					this.DelayCreateInstanceEffect(false);
				}
			}
		}
	}

	private bool IsAliveAnimation()
	{
		GameObject rootInstanceEffect = NcEffectBehaviour.GetRootInstanceEffect();
		Transform[] componentsInChildren = rootInstanceEffect.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			int num = -1;
			int num2 = -1;
			bool flag = false;
			NcEffectBehaviour[] components = transform.GetComponents<NcEffectBehaviour>();
			NcEffectBehaviour[] array2 = components;
			for (int j = 0; j < array2.Length; j++)
			{
				NcEffectBehaviour ncEffectBehaviour = array2[j];
				int animationState = ncEffectBehaviour.GetAnimationState();
				if (animationState != 0)
				{
					if (animationState == 1)
					{
						num = 1;
					}
				}
				else
				{
					num = 0;
				}
			}
			if (transform.particleSystem != null)
			{
				num2 = 0;
				if (NgObject.IsActive(transform.gameObject) && ((transform.particleSystem.enableEmission && transform.particleSystem.IsAlive()) || 0 < transform.particleSystem.particleCount))
				{
					num2 = 1;
				}
			}
			if (num2 < 1 && transform.particleEmitter != null)
			{
				num2 = 0;
				if (NgObject.IsActive(transform.gameObject) && (transform.particleEmitter.emit || 0 < transform.particleEmitter.particleCount))
				{
					num2 = 1;
				}
			}
			if (transform.renderer != null && transform.renderer.enabled && NgObject.IsActive(transform.gameObject))
			{
				flag = true;
			}
			if (0 < num)
			{
				return true;
			}
			if (num2 == 1)
			{
				return true;
			}
			if (flag && (transform.GetComponent<MeshFilter>() != null || transform.GetComponent<TrailRenderer>() != null || transform.GetComponent<LineRenderer>() != null))
			{
				return true;
			}
		}
		return false;
	}

	public void OnGUIControl()
	{
		GUI.Window(10, this.GetActionToolbarRect(), new GUI.WindowFunction(this.winActionToolbar), "PrefabSimulate - " + ((!FxmTestMain.inst.IsCurrentEffectObject()) ? "Not Selected" : FxmTestMain.inst.GetOriginalEffectObject().name));
	}

	public Rect GetActionToolbarRect()
	{
		float num = (float)Screen.height * ((!this.m_bMinimize) ? 0.35f : 0.1f);
		return new Rect(0f, (float)Screen.height - num, (float)Screen.width, num);
	}

	private string NgTooltipTooltip(string str)
	{
		return str;
	}

	public static GUIContent[] GetHcEffectControls_Play(float fAutoRet, float timeScale, float timeOneShot1, float timeRepeat1, float timeRepeat2, float timeRepeat3, float timeRepeat4, float timeRepeat5)
	{
		return new GUIContent[]
		{
			new GUIContent("AutoRet", string.Empty),
			new GUIContent(timeScale.ToString("0.00") + "x S", string.Empty),
			new GUIContent(timeOneShot1.ToString("0.0") + "x S", string.Empty),
			new GUIContent(timeRepeat1.ToString("0.0") + "s R", string.Empty),
			new GUIContent(timeRepeat2.ToString("0.0") + "s R", string.Empty),
			new GUIContent(timeRepeat3.ToString("0.0") + "s R", string.Empty),
			new GUIContent(timeRepeat4.ToString("0.0") + "s R", string.Empty),
			new GUIContent(timeRepeat5.ToString("0.0") + "s R", string.Empty)
		};
	}

	public static GUIContent[] GetHcEffectControls_Trans(FxmTestControls.AXIS nTransAxis)
	{
		return new GUIContent[]
		{
			new GUIContent("Stop", string.Empty),
			new GUIContent(nTransAxis.ToString() + " Move", string.Empty),
			new GUIContent(nTransAxis.ToString() + " Scale", string.Empty),
			new GUIContent("Arc", string.Empty),
			new GUIContent("Fall", string.Empty),
			new GUIContent("Raise", string.Empty),
			new GUIContent("Circle", string.Empty),
			new GUIContent("Tornado", string.Empty)
		};
	}

	public static GUIContent[] GetHcEffectControls_Rotate()
	{
		return new GUIContent[]
		{
			new GUIContent("Rot", string.Empty),
			new GUIContent("Fix", string.Empty)
		};
	}

	private void winActionToolbar(int id)
	{
		Rect actionToolbarRect = this.GetActionToolbarRect();
		string text = string.Empty;
		string str = string.Empty;
		int num = 10;
		int count = 5;
		this.m_bMinimize = GUI.Toggle(new Rect(3f, 1f, FXMakerLayout.m_fMinimizeClickWidth, FXMakerLayout.m_fMinimizeClickHeight), this.m_bMinimize, "Mini");
		if (GUI.changed)
		{
			PlayerPrefs.SetInt("FxmTestControls.m_bMinimize", (!this.m_bMinimize) ? 0 : 1);
		}
		GUI.changed = false;
		Rect childVerticalRect;
		Rect innerHorizontalRect;
		if (FXMakerLayout.m_bMinimizeAll || this.m_bMinimize)
		{
			count = 1;
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 1);
			if (FxmTestMain.inst.IsCurrentEffectObject())
			{
				text = string.Format("P={0} M={1} T={2}", this.m_nParticleCount, this.m_nMeshCount, this.m_nTriangles);
				str = string.Format("ParticleCount = {0} MeshCount = {1}\n Mesh: Triangles = {2} Vertices = {3}", new object[]
				{
					this.m_nParticleCount,
					this.m_nMeshCount,
					this.m_nTriangles,
					this.m_nVertices
				});
			}
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 2), text);
			if (FxmTestMain.inst.IsCurrentEffectObject())
			{
				float rightValue = (3 > this.m_nPlayIndex) ? 10f : this.m_fPlayToolbarTimes[this.m_nPlayIndex];
				childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 1);
				GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 2), "ElapsedTime " + (Time.time - this.m_fPlayStartTime).ToString("0.000"));
				innerHorizontalRect = FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 4, 4);
				innerHorizontalRect.y += 5f;
				GUI.HorizontalSlider(innerHorizontalRect, Time.time - this.m_fPlayStartTime, 0f, rightValue);
				childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 1);
				if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 8, 2), "Restart"))
				{
					this.CreateInstanceEffect();
				}
			}
			return;
		}
		childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 2);
		if (NcEffectBehaviour.GetRootInstanceEffect())
		{
			text = string.Format("P = {0}\nM = {1}\nT = {2}", this.m_nParticleCount, this.m_nMeshCount, this.m_nTriangles);
			str = string.Format("ParticleCount = {0} MeshCount = {1}\n Mesh: Triangles = {2} Vertices = {3}", new object[]
			{
				this.m_nParticleCount,
				this.m_nMeshCount,
				this.m_nTriangles,
				this.m_nVertices
			});
		}
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 1), new GUIContent(text, this.NgTooltipTooltip(str)));
		if (FxmTestMain.inst.IsCurrentEffectObject())
		{
			bool flag = false;
			GUIContent[] hcEffectControls_Play = FxmTestControls.GetHcEffectControls_Play(0f, this.m_fTimeScale, this.m_fPlayToolbarTimes[1], this.m_fPlayToolbarTimes[3], this.m_fPlayToolbarTimes[4], this.m_fPlayToolbarTimes[5], this.m_fPlayToolbarTimes[6], this.m_fPlayToolbarTimes[7]);
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 0, 1);
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 1, 1), new GUIContent("Play", string.Empty));
			int nPlayIndex = FXMakerLayout.TooltipSelectionGrid(actionToolbarRect, FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 8), this.m_nPlayIndex, hcEffectControls_Play, hcEffectControls_Play.Length);
			if (GUI.changed)
			{
				flag = true;
			}
			GUIContent[] hcEffectControls_Trans = FxmTestControls.GetHcEffectControls_Trans(this.m_nTransAxis);
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 1, 1);
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 1, 1), new GUIContent("Trans", string.Empty));
			int num2 = FXMakerLayout.TooltipSelectionGrid(actionToolbarRect, FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 8), this.m_nTransIndex, hcEffectControls_Trans, hcEffectControls_Trans.Length);
			if (GUI.changed)
			{
				flag = true;
				if ((num2 == 1 || num2 == 2) && Input.GetMouseButtonUp(1))
				{
					if (this.m_nTransAxis == FxmTestControls.AXIS.Z)
					{
						this.m_nTransAxis = FxmTestControls.AXIS.X;
					}
					else
					{
						this.m_nTransAxis++;
					}
					PlayerPrefs.SetInt("FxmTestControls.m_nTransAxis", (int)this.m_nTransAxis);
				}
			}
			if (flag)
			{
				FxmTestMain.inst.CreateCurrentInstanceEffect(false);
				this.RunActionControl(nPlayIndex, num2);
				PlayerPrefs.SetInt("FxmTestControls.m_nPlayIndex", this.m_nPlayIndex);
				PlayerPrefs.SetInt("FxmTestControls.m_nTransIndex", this.m_nTransIndex);
			}
		}
		float num3 = this.m_fDistPerTime;
		childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 2, 1);
		GUIContent gUIContent = new GUIContent("DistPerTime", string.Empty);
		GUIContent expr_473 = gUIContent;
		expr_473.text = expr_473.text + " " + this.m_fDistPerTime.ToString("00.00");
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 2), gUIContent);
		innerHorizontalRect = FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 5);
		innerHorizontalRect.y += 5f;
		num3 = GUI.HorizontalSlider(innerHorizontalRect, num3, 0.1f, 40f);
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 14, 1), new GUIContent("<", string.Empty)))
		{
			num3 = (float)((int)(num3 - 1f));
		}
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 15, 1), new GUIContent(">", string.Empty)))
		{
			num3 = (float)((int)(num3 + 1f));
		}
		if (num3 != this.m_fDistPerTime)
		{
			this.m_fDistPerTime = ((num3 != 0f) ? num3 : 0.1f);
			PlayerPrefs.SetFloat("FxmTestControls.m_fDistPerTime", this.m_fDistPerTime);
			if (0 < this.m_nTransIndex)
			{
				this.CreateInstanceEffect();
			}
		}
		if (NgLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 9, 1), new GUIContent("Multi", this.m_nMultiShotCount.ToString()), true))
		{
			if (Input.GetMouseButtonUp(0))
			{
				this.m_nMultiShotCount++;
				if (4 < this.m_nMultiShotCount)
				{
					this.m_nMultiShotCount = 1;
				}
			}
			else
			{
				this.m_nMultiShotCount = 1;
			}
			this.CreateInstanceEffect();
		}
		GUIContent[] hcEffectControls_Rotate = FxmTestControls.GetHcEffectControls_Rotate();
		childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 2, 1);
		int num4 = FXMakerLayout.TooltipSelectionGrid(actionToolbarRect, FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 8, 1), this.m_nRotateIndex, hcEffectControls_Rotate, hcEffectControls_Rotate.Length);
		if (num4 != this.m_nRotateIndex)
		{
			this.m_nRotateIndex = num4;
			PlayerPrefs.SetInt("FxmTestControls.m_nRotateIndex", this.m_nRotateIndex);
			if (0 < this.m_nTransIndex)
			{
				this.CreateInstanceEffect();
			}
		}
		float num5 = this.m_fTimeScale;
		childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 3, 1);
		gUIContent = new GUIContent("TimeScale", string.Empty);
		GUIContent expr_684 = gUIContent;
		expr_684.text = expr_684.text + " " + this.m_fTimeScale.ToString("0.00");
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 2), gUIContent);
		innerHorizontalRect = FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 5);
		innerHorizontalRect.y += 5f;
		num5 = GUI.HorizontalSlider(innerHorizontalRect, num5, 0f, 3f);
		if (num5 == 0f)
		{
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 7, 1), new GUIContent("Resume", string.Empty)))
			{
				num5 = this.m_fOldTimeScale;
			}
		}
		else if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 7, 1), new GUIContent("Pause", string.Empty)))
		{
			num5 = 0f;
		}
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 8, 1), new GUIContent("Reset", string.Empty)))
		{
			num5 = 1f;
		}
		this.SetTimeScale(num5);
		if (FxmTestMain.inst.IsCurrentEffectObject())
		{
			float rightValue2 = (3 > this.m_nPlayIndex) ? 10f : this.m_fPlayToolbarTimes[this.m_nPlayIndex];
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 4, 1);
			gUIContent = new GUIContent("ElapsedTime", string.Empty);
			GUIContent expr_7D7 = gUIContent;
			expr_7D7.text = expr_7D7.text + " " + (Time.time - this.m_fPlayStartTime).ToString("0.000");
			GUI.Box(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 0, 2), gUIContent);
			innerHorizontalRect = FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 2, 5);
			innerHorizontalRect.y += 5f;
			GUI.HorizontalSlider(innerHorizontalRect, Time.time - this.m_fPlayStartTime, 0f, rightValue2);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 14, 1), new GUIContent("+.5", string.Empty)))
			{
				this.SetTimeScale(1f);
				base.Invoke("invokeStopTimer", 0.5f);
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 15, 1), new GUIContent("+.1", string.Empty)))
			{
				this.SetTimeScale(0.4f);
				base.Invoke("invokeStopTimer", 0.1f);
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 16, 1), new GUIContent("+.05", string.Empty)))
			{
				this.SetTimeScale(0.2f);
				base.Invoke("invokeStopTimer", 0.05f);
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num * 2, 17, 1), new GUIContent("+.01", string.Empty)))
			{
				this.SetTimeScale(0.04f);
				base.Invoke("invokeStopTimer", 0.01f);
			}
			childVerticalRect = FXMakerLayout.GetChildVerticalRect(actionToolbarRect, 0, count, 3, 2);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(childVerticalRect, num, 9, 1), new GUIContent("Restart", string.Empty)))
			{
				this.CreateInstanceEffect();
			}
		}
	}

	private void invokeStopTimer()
	{
		this.SetTimeScale(0f);
	}

	public void RunActionControl()
	{
		this.RunActionControl(this.m_nPlayIndex, this.m_nTransIndex);
	}

	protected void RunActionControl(int nPlayIndex, int nTransIndex)
	{
		NgUtil.LogDevelop("RunActionControl() - nPlayIndex " + nPlayIndex);
		base.CancelInvoke();
		this.m_bCalledDelayCreate = false;
		this.ResumeTimeScale();
		this.m_bStartAliveAnimation = false;
		switch (nPlayIndex)
		{
		case 2:
			this.SetTimeScale(this.m_fPlayToolbarTimes[nPlayIndex]);
			break;
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
			if (nPlayIndex != this.m_nPlayIndex)
			{
				nTransIndex = 0;
			}
			break;
		}
		if (0 < nTransIndex)
		{
			float num = ((!(Camera.main != null)) ? 1f : (Vector3.Magnitude(Camera.main.transform.position) * 0.8f)) * this.m_fTransRate;
			GameObject instanceEffectObject = FxmTestMain.inst.GetInstanceEffectObject();
			GameObject gameObject = NgObject.CreateGameObject(instanceEffectObject.transform.parent.gameObject, "simulate");
			FxmTestSimulate fxmTestSimulate = gameObject.AddComponent<FxmTestSimulate>();
			instanceEffectObject.transform.parent = gameObject.transform;
			FxmTestMain.inst.ChangeRoot_InstanceEffectObject(gameObject);
			fxmTestSimulate.Init(this, this.m_nMultiShotCount);
			switch (nTransIndex)
			{
			case 1:
				fxmTestSimulate.SimulateMove(this.m_nTransAxis, num, this.m_fDistPerTime, this.m_nRotateIndex == 0);
				break;
			case 2:
				fxmTestSimulate.SimulateScale(this.m_nTransAxis, num * 0.3f, this.m_fStartPosition, this.m_fDistPerTime, this.m_nRotateIndex == 0);
				break;
			case 3:
				fxmTestSimulate.SimulateArc(num * 0.7f, this.m_fDistPerTime, this.m_nRotateIndex == 0);
				break;
			case 4:
				fxmTestSimulate.SimulateFall(num * 0.7f, this.m_fDistPerTime, this.m_nRotateIndex == 0);
				break;
			case 5:
				fxmTestSimulate.SimulateRaise(num * 0.7f, this.m_fDistPerTime, this.m_nRotateIndex == 0);
				break;
			case 6:
				fxmTestSimulate.SimulateCircle(num * 0.5f, this.m_fDistPerTime, this.m_nRotateIndex == 0);
				break;
			case 7:
				fxmTestSimulate.SimulateTornado(num * 0.3f, num * 0.7f, this.m_fDistPerTime, this.m_nRotateIndex == 0);
				break;
			}
		}
		if (0 < nTransIndex && 3 <= nPlayIndex)
		{
			nPlayIndex = 0;
		}
		this.m_nPlayIndex = nPlayIndex;
		this.m_nTransIndex = nTransIndex;
		if (this.IsRepeat())
		{
			this.m_fCreateTime = Time.time;
		}
	}

	public void OnActionTransEnd()
	{
		this.DelayCreateInstanceEffect(true);
	}

	private void RotateFront(Transform target)
	{
		Quaternion localRotation = FxmTestMain.inst.GetOriginalEffectObject().transform.localRotation;
		Vector3 eulerAngles = localRotation.eulerAngles;
		switch (this.m_nRotateIndex)
		{
		case 1:
			eulerAngles.y += 90f;
			break;
		case 2:
			eulerAngles.y -= 90f;
			break;
		case 3:
			eulerAngles.z -= 90f;
			break;
		}
		localRotation.eulerAngles = eulerAngles;
		target.localRotation = localRotation;
	}

	private void DelayCreateInstanceEffect(bool bEndMove)
	{
		this.m_bCalledDelayCreate = true;
		base.Invoke("NextInstanceEffect", (float)((!bEndMove) ? 1 : 3) * this.m_fDelayCreateTime);
	}

	private void NextInstanceEffect()
	{
		if (FxmTestMain.inst.m_bAutoChange)
		{
			FxmTestMain.inst.ChangeEffect(true);
		}
		else
		{
			this.CreateInstanceEffect();
		}
	}

	private void CreateInstanceEffect()
	{
		if (FxmTestMain.inst.IsCurrentEffectObject())
		{
			FxmTestMain.inst.CreateCurrentInstanceEffect(true);
		}
	}

	private void SetTimeScale(float timeScale)
	{
		if (this.m_fTimeScale != timeScale || this.m_fTimeScale != Time.timeScale)
		{
			if (timeScale == 0f && this.m_fTimeScale != 0f)
			{
				this.m_fOldTimeScale = this.m_fTimeScale;
			}
			this.m_fTimeScale = timeScale;
			if (0.01f <= this.m_fTimeScale)
			{
				PlayerPrefs.SetFloat("FxmTestControls.m_fTimeScale", this.m_fTimeScale);
			}
			Time.timeScale = this.m_fTimeScale;
		}
	}

	public void ResumeTimeScale()
	{
		if (this.m_fTimeScale == 0f)
		{
			this.SetTimeScale(this.m_fOldTimeScale);
		}
	}
}
