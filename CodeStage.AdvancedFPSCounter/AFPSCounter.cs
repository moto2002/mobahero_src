using CodeStage.AdvancedFPSCounter.CountersData;
using CodeStage.AdvancedFPSCounter.Labels;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter
{
	[AddComponentMenu("")]
	public class AFPSCounter : MonoBehaviour
	{
		private const string CONTAINER_NAME = "Advanced FPS Counter";

		internal static string NEW_LINE = Environment.NewLine;

		private static AFPSCounter instance;

		public FPSCounterData fpsCounter = new FPSCounterData();

		public MemoryCounterData memoryCounter = new MemoryCounterData();

		public DeviceInfoCounterData deviceInfoCounter = new DeviceInfoCounterData();

		public KeyCode hotKey = KeyCode.BackQuote;

		public bool keepAlive = true;

		private bool obsoleteEnabled = true;

		[SerializeField]
		private AFPSCounterOperationMode operationMode = AFPSCounterOperationMode.Normal;

		[SerializeField]
		private bool forceFrameRate;

		[Range(-1f, 200f), SerializeField]
		private int forcedFrameRate = -1;

		[SerializeField]
		private Vector2 anchorsOffset = new Vector2(5f, 5f);

		[SerializeField]
		private Font labelsFont;

		[Range(0f, 100f), SerializeField]
		private int fontSize;

		[Range(0f, 10f), SerializeField]
		private float lineSpacing = 1f;

		internal DrawableLabel[] labels;

		private int anchorsCount;

		private int cachedVSync = -1;

		private int cachedFrameRate = -1;

		private int initialSceneIndex;

		private bool inited;

		public static AFPSCounter Instance
		{
			get
			{
				if (AFPSCounter.instance == null)
				{
					AFPSCounter aFPSCounter = (AFPSCounter)UnityEngine.Object.FindObjectOfType(typeof(AFPSCounter));
					if (aFPSCounter != null && aFPSCounter.IsPlacedCorrectly())
					{
						AFPSCounter.instance = aFPSCounter;
					}
					else
					{
						GameObject gameObject = new GameObject("Advanced FPS Counter");
						gameObject.AddComponent<AFPSCounter>();
					}
				}
				return AFPSCounter.instance;
			}
		}

		public AFPSCounterOperationMode OperationMode
		{
			get
			{
				return this.operationMode;
			}
			set
			{
				if (this.operationMode == value || !Application.isPlaying)
				{
					return;
				}
				this.operationMode = value;
				if (this.operationMode != AFPSCounterOperationMode.Disabled)
				{
					if (this.operationMode == AFPSCounterOperationMode.Background)
					{
						for (int i = 0; i < this.anchorsCount; i++)
						{
							this.labels[i].Clear();
						}
					}
					this.fpsCounter.UpdateValue();
					this.memoryCounter.UpdateValue();
					this.deviceInfoCounter.UpdateValue();
					this.OnEnable();
				}
				else
				{
					this.OnDisable();
				}
			}
		}

		[Obsolete("Use AFPSCounter.Instance.OperationMode instead of AFPSCounter.Instance.enabled!")]
		public new bool enabled
		{
			get
			{
				return this.obsoleteEnabled;
			}
			set
			{
				if (this.obsoleteEnabled == value || !Application.isPlaying)
				{
					return;
				}
				this.obsoleteEnabled = value;
				if (this.obsoleteEnabled)
				{
					this.operationMode = AFPSCounterOperationMode.Normal;
					this.OnEnable();
				}
				else
				{
					this.operationMode = AFPSCounterOperationMode.Disabled;
					this.OnDisable();
				}
			}
		}

		public bool ForceFrameRate
		{
			get
			{
				return this.forceFrameRate;
			}
			set
			{
				if (this.forceFrameRate == value || !Application.isPlaying)
				{
					return;
				}
				this.forceFrameRate = value;
				if (this.operationMode == AFPSCounterOperationMode.Disabled)
				{
					return;
				}
				this.RefreshForcedFrameRate();
			}
		}

		public int ForcedFrameRate
		{
			get
			{
				return this.forcedFrameRate;
			}
			set
			{
				if (this.forcedFrameRate == value || !Application.isPlaying)
				{
					return;
				}
				this.forcedFrameRate = value;
				if (this.operationMode == AFPSCounterOperationMode.Disabled)
				{
					return;
				}
				this.RefreshForcedFrameRate();
			}
		}

		public Vector2 AnchorsOffset
		{
			get
			{
				return this.anchorsOffset;
			}
			set
			{
				if (this.anchorsOffset == value || !Application.isPlaying)
				{
					return;
				}
				this.anchorsOffset = value;
				if (this.operationMode == AFPSCounterOperationMode.Disabled || this.labels == null)
				{
					return;
				}
				for (int i = 0; i < this.anchorsCount; i++)
				{
					this.labels[i].ChangeOffset(this.anchorsOffset);
				}
			}
		}

		public Font LabelsFont
		{
			get
			{
				return this.labelsFont;
			}
			set
			{
				if (this.labelsFont == value || !Application.isPlaying)
				{
					return;
				}
				this.labelsFont = value;
				if (this.operationMode == AFPSCounterOperationMode.Disabled || this.labels == null)
				{
					return;
				}
				for (int i = 0; i < this.anchorsCount; i++)
				{
					this.labels[i].ChangeFont(this.labelsFont);
				}
			}
		}

		public int FontSize
		{
			get
			{
				return this.fontSize;
			}
			set
			{
				if (this.fontSize == value || !Application.isPlaying)
				{
					return;
				}
				this.fontSize = value;
				if (this.operationMode == AFPSCounterOperationMode.Disabled || this.labels == null)
				{
					return;
				}
				for (int i = 0; i < this.anchorsCount; i++)
				{
					this.labels[i].ChangeFontSize(this.fontSize);
				}
			}
		}

		public float LineSpacing
		{
			get
			{
				return this.lineSpacing;
			}
			set
			{
				if (Math.Abs(this.lineSpacing - value) < 0.001f || !Application.isPlaying)
				{
					return;
				}
				this.lineSpacing = value;
				if (this.operationMode == AFPSCounterOperationMode.Disabled || this.labels == null)
				{
					return;
				}
				for (int i = 0; i < this.anchorsCount; i++)
				{
					this.labels[i].ChangeLineSpacing(this.lineSpacing);
				}
			}
		}

		private AFPSCounter()
		{
		}

		public void Dispose()
		{
			if (AFPSCounter.instance == this)
			{
				AFPSCounter.instance = null;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void Awake()
		{
			if (AFPSCounter.instance != null && AFPSCounter.instance.keepAlive)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (!this.IsPlacedCorrectly())
			{
				UnityEngine.Debug.LogWarning("Advanced FPS Counter is placed in scene incorrectly and will be auto-destroyed! Please, use \"GameObject->Create Other->Code Stage->Advanced FPS Counter\" menu to correct this!");
				UnityEngine.Object.Destroy(this);
				return;
			}
			this.fpsCounter.Init(this);
			this.memoryCounter.Init(this);
			this.deviceInfoCounter.Init(this);
			AFPSCounter.instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			this.anchorsCount = Enum.GetNames(typeof(LabelAnchor)).Length;
			this.labels = new DrawableLabel[this.anchorsCount];
			for (int i = 0; i < this.anchorsCount; i++)
			{
				this.labels[i] = new DrawableLabel((LabelAnchor)i, this.anchorsOffset, this.labelsFont, this.fontSize, this.lineSpacing);
			}
		}

		private void Start()
		{
			this.inited = true;
		}

		private void Update()
		{
			if (!this.inited)
			{
				return;
			}
			if (this.hotKey != KeyCode.None && Input.GetKeyDown(this.hotKey))
			{
				this.SwitchCounter();
			}
		}

		private void OnLevelWasLoaded(int index)
		{
			if (!this.inited)
			{
				return;
			}
			if (!this.keepAlive)
			{
				this.Dispose();
			}
			else if (this.fpsCounter.Enabled)
			{
				if (this.fpsCounter.ShowMinMax && this.fpsCounter.resetMinMaxOnNewScene)
				{
					this.fpsCounter.ResetMinMax();
				}
				if (this.fpsCounter.ShowAverage && this.fpsCounter.resetAverageOnNewScene)
				{
					this.fpsCounter.ResetAverage();
				}
			}
		}

		private void OnEnable()
		{
			if (this.operationMode == AFPSCounterOperationMode.Disabled)
			{
				return;
			}
			this.ActivateCounters();
			base.Invoke("RefreshForcedFrameRate", 0.5f);
		}

		private void OnDisable()
		{
			if (!this.inited)
			{
				return;
			}
			this.DeactivateCounters();
			if (base.IsInvoking("RefreshForcedFrameRate"))
			{
				base.CancelInvoke("RefreshForcedFrameRate");
			}
			this.RefreshForcedFrameRate(true);
			for (int i = 0; i < this.anchorsCount; i++)
			{
				this.labels[i].Clear();
			}
		}

		private void OnDestroy()
		{
			if (!this.inited)
			{
				return;
			}
			this.fpsCounter.Dispose();
			this.memoryCounter.Dispose();
			this.deviceInfoCounter.Dispose();
			if (this.labels != null)
			{
				for (int i = 0; i < this.anchorsCount; i++)
				{
					this.labels[i].Dispose();
				}
				Array.Clear(this.labels, 0, this.anchorsCount);
				this.labels = null;
			}
			this.inited = false;
		}

		private bool IsPlacedCorrectly()
		{
			return base.gameObject.GetComponentsInChildren<Component>().Length == 2 && base.transform.childCount == 0 && base.transform.parent == null;
		}

		internal void MakeDrawableLabelDirty(LabelAnchor anchor)
		{
			if (this.operationMode == AFPSCounterOperationMode.Normal)
			{
				this.labels[(int)anchor].dirty = true;
			}
		}

		internal void UpdateTexts()
		{
			if (this.operationMode != AFPSCounterOperationMode.Normal)
			{
				return;
			}
			bool flag = false;
			if (this.fpsCounter.Enabled)
			{
				DrawableLabel drawableLabel = this.labels[(int)this.fpsCounter.Anchor];
				if (drawableLabel.newText.Length > 0)
				{
					drawableLabel.newText.Append(AFPSCounter.NEW_LINE);
				}
				drawableLabel.newText.Append(this.fpsCounter.text);
				drawableLabel.dirty |= this.fpsCounter.dirty;
				this.fpsCounter.dirty = false;
				flag = true;
			}
			if (this.memoryCounter.Enabled)
			{
				DrawableLabel drawableLabel2 = this.labels[(int)this.memoryCounter.Anchor];
				if (drawableLabel2.newText.Length > 0)
				{
					drawableLabel2.newText.Append(AFPSCounter.NEW_LINE);
				}
				drawableLabel2.newText.Append(this.memoryCounter.text);
				drawableLabel2.dirty |= this.memoryCounter.dirty;
				this.memoryCounter.dirty = false;
				flag = true;
			}
			if (this.deviceInfoCounter.Enabled)
			{
				DrawableLabel drawableLabel3 = this.labels[(int)this.deviceInfoCounter.Anchor];
				if (drawableLabel3.newText.Length > 0)
				{
					drawableLabel3.newText.Append(AFPSCounter.NEW_LINE);
				}
				drawableLabel3.newText.Append(this.deviceInfoCounter.text);
				drawableLabel3.dirty |= this.deviceInfoCounter.dirty;
				this.deviceInfoCounter.dirty = false;
				flag = true;
			}
			if (flag)
			{
				for (int i = 0; i < this.anchorsCount; i++)
				{
					this.labels[i].CheckAndUpdate();
				}
			}
			else
			{
				for (int j = 0; j < this.anchorsCount; j++)
				{
					this.labels[j].Clear();
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator UpdateFPSCounter()
		{
			AFPSCounter.<UpdateFPSCounter>c__IteratorC <UpdateFPSCounter>c__IteratorC = new AFPSCounter.<UpdateFPSCounter>c__IteratorC();
			<UpdateFPSCounter>c__IteratorC.<>f__this = this;
			return <UpdateFPSCounter>c__IteratorC;
		}

		[DebuggerHidden]
		private IEnumerator UpdateMemoryCounter()
		{
			AFPSCounter.<UpdateMemoryCounter>c__IteratorD <UpdateMemoryCounter>c__IteratorD = new AFPSCounter.<UpdateMemoryCounter>c__IteratorD();
			<UpdateMemoryCounter>c__IteratorD.<>f__this = this;
			return <UpdateMemoryCounter>c__IteratorD;
		}

		private void SwitchCounter()
		{
			if (this.operationMode == AFPSCounterOperationMode.Disabled)
			{
				this.OperationMode = AFPSCounterOperationMode.Normal;
			}
			else if (this.operationMode == AFPSCounterOperationMode.Normal)
			{
				this.OperationMode = AFPSCounterOperationMode.Disabled;
			}
		}

		private void ActivateCounters()
		{
			this.fpsCounter.Activate();
			this.memoryCounter.Activate();
			this.deviceInfoCounter.Activate();
			if (this.fpsCounter.Enabled || this.memoryCounter.Enabled || this.deviceInfoCounter.Enabled)
			{
				this.UpdateTexts();
			}
		}

		private void DeactivateCounters()
		{
			if (AFPSCounter.instance == null)
			{
				return;
			}
			this.fpsCounter.Deactivate();
			this.memoryCounter.Deactivate();
			this.deviceInfoCounter.Deactivate();
		}

		private void RefreshForcedFrameRate()
		{
			this.RefreshForcedFrameRate(false);
		}

		private void RefreshForcedFrameRate(bool disabling)
		{
			if (this.forceFrameRate && !disabling)
			{
				if (this.cachedVSync == -1)
				{
					this.cachedVSync = QualitySettings.vSyncCount;
					this.cachedFrameRate = Application.targetFrameRate;
					QualitySettings.vSyncCount = 0;
				}
				Application.targetFrameRate = this.forcedFrameRate;
			}
			else if (this.cachedVSync != -1)
			{
				QualitySettings.vSyncCount = this.cachedVSync;
				Application.targetFrameRate = this.cachedFrameRate;
				this.cachedVSync = -1;
			}
		}

		internal static string Color32ToHex(Color32 color)
		{
			return color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2") + color.a.ToString("x2");
		}
	}
}
