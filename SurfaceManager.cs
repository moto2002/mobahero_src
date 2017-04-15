using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SurfaceManager : UnitComponent
{
	public HUDText mText;

	public UIBloodBar mHpBar;

	public ResourceHandleWrapper<SkillPointer> mSkillPointer;

	private ResourceHandle _mTargetIcon;

	private ResourceHandle _mainPlayerIcon;

	public GameObject cacheGameObj;

	private Shader m_baseShader;

	private Shader m_outlineShader;

	private Shader m_rimlightShader;

	private Shader m_highlightShader;

	private Color m_baseColor;

	private Color m_selectColor;

	private Color m_outlineColor;

	private BoxCollider m_boxCollider;

	public Material material;

	public Shader sourceShader;

	public bool isChangePlayer;

	private AnimationCurve outlineAnimCurve;

	private Keyframe[] outlineAnimCurveKeys;

	private float animTime;

	private List<Material> mats = new List<Material>(4);

	private static Material deathMat;

	public Transform ShadowTexture;

	protected CharacterEffect mCharacterEffect;

	private bool _hasHud;

	private Camera _mainCamera;

	private Camera _uiCamera;

	public bool NeedUpdateHud;

	private ParticleSystem[] m_particles;

	private int marginX = 30;

	private int marginY = 30;

	private Vector3 _screenPosition;

	private float timeSum;

	private Task TriggerEnter_Task;

	private Callback DissolveCallback;

	private float m_fTime;

	private Task DissolveEffect_Task;

	private static Material _deathMat;

	public Transform mTargetIcon
	{
		get
		{
			return (this._mTargetIcon == null) ? null : this._mTargetIcon.Raw;
		}
	}

	public Transform mainPlayerIcon
	{
		get
		{
			return (this._mainPlayerIcon == null) ? null : this._mainPlayerIcon.Raw;
		}
	}

	public Camera mainCamera
	{
		get
		{
			if (!this._mainCamera)
			{
				this._mainCamera = BattleCameraMgr.Instance.BattleCamera;
			}
			return this._mainCamera;
		}
	}

	public Camera uiCamera
	{
		get
		{
			if (!this._uiCamera)
			{
				this._uiCamera = CameraRoot.Instance.UICamera;
			}
			return this._uiCamera;
		}
	}

	public Renderer[] mRenders
	{
		get
		{
			if (this.self != null)
			{
				return this.self.mRenders;
			}
			return null;
		}
	}

	public ParticleSystem[] mParticles
	{
		get
		{
			return this.m_particles;
		}
		set
		{
			this.m_particles = value;
		}
	}

	public SkinnedMeshRenderer mSkinnedRenderer
	{
		get
		{
			if (this.self != null)
			{
				return this.self.mRender;
			}
			return null;
		}
	}

	public Material mMaterial
	{
		get
		{
			if (this.mSkinnedRenderer != null)
			{
				return this.mSkinnedRenderer.material;
			}
			return null;
		}
		set
		{
			this.mSkinnedRenderer.material = value;
		}
	}

	public Shader mShader
	{
		get
		{
			if (this.mMaterial != null)
			{
				return this.mMaterial.shader;
			}
			return null;
		}
		set
		{
			if (this.mMaterial != null)
			{
				this.mMaterial.shader = value;
			}
		}
	}

	public Vector3 HudbarLocalScale
	{
		get;
		private set;
	}

	public float HudbarHeight
	{
		get;
		private set;
	}

	public Vector3 HudbarWorldPos
	{
		get;
		private set;
	}

	public static Material DeathMat
	{
		get
		{
			if (SurfaceManager._deathMat == null)
			{
				SurfaceManager._deathMat = (Resources.Load("Material/Death") as Material);
			}
			return UnityEngine.Object.Instantiate(SurfaceManager._deathMat) as Material;
		}
		set
		{
			SurfaceManager._deathMat = value;
		}
	}

	public SurfaceManager()
	{
	}

	public SurfaceManager(Units self) : base(self)
	{
	}

	public override void OnCreate()
	{
		base.OnCreate();
		this.cacheGameObj = base.gameObject;
		this.sourceShader = this.mShader;
		if (SurfaceManager.deathMat == null)
		{
			SurfaceManager.deathMat = ResourceManager.LoadPath<Material>("Material/Death", null, 0);
		}
	}

	public override void OnInit()
	{
		this.m_CoroutineManager.StopAllCoroutine();
		if (this.self != null)
		{
			this.mCharacterEffect = this.self.mCharacterEffect;
		}
		if (this.self.isHero || this.self.isMonster || this.self.isHome || this.self.isTower || this.self.isItem)
		{
			if (this.mCharacterEffect != null)
			{
				if (this.self.isHome)
				{
					Renderer[] componentsInChildren = this.self.transform.GetComponentsInChildren<Renderer>();
					this.mCharacterEffect.SetRenderer(componentsInChildren);
					this.mCharacterEffect.SetOutlineWidth(0.035f);
				}
				this.mCharacterEffect.InitRenderer(this.self, this.mRenders);
				this.mCharacterEffect.isActive = true;
			}
			if (this.self.isPlayer && this.mCharacterEffect != null)
			{
				this.mCharacterEffect.SetColor_Outline(Color.black);
				this.mCharacterEffect.SetIsHero(true);
			}
			else if (this.self.isHero)
			{
				if (this.self.isMyTeam && this.mCharacterEffect != null)
				{
					this.mCharacterEffect.SetColor_Outline(Color.black);
				}
				else if (this.self.isEnemy && this.mCharacterEffect != null)
				{
					this.mCharacterEffect.SetColor_Outline(Color.black);
				}
				if (this.mCharacterEffect != null)
				{
					this.mCharacterEffect.SetIsHero(true);
				}
			}
			else if (this.self.isMonster && this.mCharacterEffect != null)
			{
				this.mCharacterEffect.SetIsHero(false);
			}
			else if (this.self.isTower && this.mCharacterEffect != null)
			{
				this.mCharacterEffect.SetOutlineWidth(0.035f);
			}
			this.RevertModel();
		}
		this._hasHud = this.IsHaveHudBarAndHudText();
	}

	public void SetBloodBar(bool inIsMyTeam)
	{
		if (this.mHpBar != null)
		{
			this.mHpBar.SetBloodBar(inIsMyTeam);
		}
	}

	public void UpdateHud(bool visible)
	{
		if (!this._hasHud)
		{
			if (this.mHpBar)
			{
				this.mHpBar.gameObject.SetActive(false);
			}
			return;
		}
		this.CheckNeedHudUpdate();
		if (!this.NeedUpdateHud)
		{
			if (this.mHpBar)
			{
				this.mHpBar.SkipUpdate();
			}
			return;
		}
		if (!this.mHpBar)
		{
			this.mHpBar = Singleton<CharacterView>.Instance.CreateHudBar(this.self);
			if (this.mHpBar == null)
			{
				return;
			}
			this.mHpBar.On_Spawn();
			this.mText = Singleton<CharacterView>.Instance.CreateHudText(this.self);
		}
		if (this.mHpBar != null)
		{
			this.mHpBar.OnUpdate(visible, false);
		}
	}

	public override void OnStart()
	{
		Units self = this.self;
		string tag = self.tag;
		switch (tag)
		{
		case "Hero":
		case "Player":
			this.HudbarLocalScale = Vector3.one * 1f;
			this.HudbarHeight = self.GetHeight() + 1f;
			break;
		case "Building":
		case "Home":
			this.HudbarLocalScale = Vector3.one * 1.5f;
			this.HudbarHeight = self.GetHeight();
			break;
		case "Monster":
			if (self.isCreep)
			{
				this.HudbarLocalScale = Vector3.one * 1f;
				this.HudbarHeight = self.GetHeight() + 0.5f;
			}
			else
			{
				this.HudbarLocalScale = new Vector3(0.8f, 1f, 0.8f);
				this.HudbarHeight = self.GetHeight() + 0.5f;
			}
			break;
		}
	}

	private bool IsBarVisibleInScreen()
	{
		return this._screenPosition.x >= (float)(-(float)this.marginX) && this._screenPosition.x <= (float)(Screen.width + this.marginX) && this._screenPosition.y >= (float)(-(float)this.marginY) && this._screenPosition.y <= (float)(Screen.height + this.marginY);
	}

	private void CheckNeedHudUpdate()
	{
		this._screenPosition = this.mainCamera.WorldToScreenPoint(this.self.transform.position + new Vector3(0f, this.HudbarHeight, 0f));
		this.HudbarWorldPos = this.uiCamera.ScreenToWorldPoint(this._screenPosition);
		this.NeedUpdateHud = this.IsBarVisibleInScreen();
		if (!this.NeedUpdateHud && this.mHpBar)
		{
			this.mHpBar.isChangeVisible = true;
		}
	}

	public override void OnUpdate(float deltaTime)
	{
		this.timeSum += deltaTime;
		if (this.self.isPlayer)
		{
			if ((double)this.self.hp < (double)this.self.hp_max * 0.1 && this.self.isLive)
			{
				if (!Singleton<HeartView>.Instance.IsOpened)
				{
					CtrlManager.OpenWindow(WindowID.HeartView, null);
				}
			}
			else if (Singleton<HeartView>.Instance.IsOpened)
			{
				CtrlManager.CloseWindow(WindowID.HeartView);
			}
		}
		if ((this.self.isHero || this.self.IsSummonedCreature) && this.self.isMyTeam && this.self.isLive)
		{
			this.SetShadowHeight(this.self.mTransform.position.y);
			if (this.self.isPlayer && Singleton<SkillView>.Instance.transform && this.timeSum >= 0.8f)
			{
				Singleton<SkillView>.Instance.UpdateSelfValue(this.self, false, false);
				this.timeSum = 0f;
			}
		}
		if (this.isChangePlayer)
		{
			if (!this.self.isPlayer)
			{
				if (this.mSkillPointer.IsValid<SkillPointer>())
				{
					this.mSkillPointer.Component.DestroySelf();
				}
			}
			else if (this.mHpBar != null)
			{
				this.mHpBar.UpdateView();
			}
			this.isChangePlayer = false;
		}
		if (this.self.isSelected && this.outlineAnimCurve != null)
		{
			this.animTime += Time.deltaTime;
			if (this.animTime > this.outlineAnimCurveKeys[2].time)
			{
				this.animTime -= this.outlineAnimCurveKeys[2].time;
			}
			float num = this.outlineAnimCurve.Evaluate(this.animTime);
			float num2 = num * 0.8f;
			num2 = Mathf.Clamp(num2, 0.65f, 1f);
			this.SetOutline(new Color(num2, 0f, 0f), 0.03f * num);
		}
		else
		{
			this.SetOutline(Color.black, 0.035f);
		}
	}

	public override void OnStop()
	{
		this.m_CoroutineManager.StopAllCoroutine();
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.isActive = false;
		}
		this.ClearSurface(true);
		this.DestroyHUDText();
	}

	public override void OnExit()
	{
		this.m_CoroutineManager.StopAllCoroutine();
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.isActive = false;
		}
		if (this.material != null)
		{
			this.Add2Mats(this.material);
		}
		if (this.mMaterial != null)
		{
			this.Add2Mats(this.mMaterial);
		}
		if (this.mats != null)
		{
			Material[] array = this.mats.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
		}
		this.HideModel();
	}

	public override void OnTarget(Units attacker)
	{
	}

	public override void OnWound(Units attacker, float damage)
	{
		if (this.self == null || !this.self.isLive || !this.self.isVisibleInCamera)
		{
			return;
		}
		if (this.self.isVisible && damage > 0f)
		{
			this.ShowRimLight();
		}
	}

	public override void OnDeath(Units attacker)
	{
		this.RevertShader();
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.isActive = false;
		}
		if (this.self.isPlayer)
		{
			CtrlManager.CloseWindow(WindowID.HeartView);
		}
		if (this.self.isPlayer)
		{
			Singleton<SkillView>.Instance.ShowGuideBar(false, 1f, "回城");
		}
		this.ClearSurface(false);
	}

	private void Add2Mats(Material mat)
	{
		if (this.mats == null)
		{
			this.mats = new List<Material>(5);
		}
		if (!this.mats.Contains(mat))
		{
			this.mats.Add(mat);
		}
	}

	public void SetShader(string shader)
	{
		if (this.material != null)
		{
			this.material.shader = Shader.Find(shader);
		}
	}

	public void RevertShader()
	{
		if (this.material != null)
		{
			this.mShader = this.sourceShader;
			this.mMaterial.SetColor("_Color", Color.white);
			this.mMaterial.SetFloat("amt", 1f);
		}
	}

	public void SetOutline(Color color, float width = 0.035f)
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.SetColor_Outline(color);
			this.mCharacterEffect.SetOutlineWidth(width);
		}
	}

	public void SetShadowHeight(float h)
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.SetShadowHeight(h);
		}
	}

	public void ClearOutline()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.SetOutlineWidth(0.035f);
		}
	}

	public void HideModel()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.HideModel();
		}
	}

	public void RevertModel()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.RevertModel();
		}
	}

	public void ClearEffects(bool isRevertMaterial = true)
	{
		this.m_CoroutineManager.StopAllCoroutine();
		this.RevertShader();
		this.RevertModel();
	}

	public bool isShowSkillPointer()
	{
		return this.mSkillPointer.IsValid<SkillPointer>() && this.mSkillPointer.Component.isShow;
	}

	public void ShowSkillPointer()
	{
		if (this.mSkillPointer.IsValid<SkillPointer>())
		{
			this.mSkillPointer.Component.Show();
		}
	}

	public void HideSkillPointer()
	{
		if (!this.mSkillPointer.IsValid<SkillPointer>())
		{
			return;
		}
		if (this.mSkillPointer.Component.isShow)
		{
			this.mSkillPointer.Component.Hide();
		}
	}

	public void DestroySkillPointer()
	{
		if (this.mSkillPointer.IsValid<SkillPointer>())
		{
			this.mSkillPointer.Release();
		}
		this.mSkillPointer = null;
	}

	private void InitSkillPointerTrigger()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "TriggerObj";
		gameObject.transform.parent = base.transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.layer = LayerMask.NameToLayer("SkillPointer");
		CheckTrigger checkTrigger = gameObject.AddComponent<CheckTrigger>();
		checkTrigger.OnTrigger += new CallbackDelegateBool(this.CallWhenTrigger);
		this.m_boxCollider = gameObject.GetComponent<BoxCollider>();
		if (this.m_boxCollider == null)
		{
			this.m_boxCollider = gameObject.AddComponent<BoxCollider>();
			this.m_boxCollider.size = new Vector3(1f, 5f, 1f);
		}
		this.m_boxCollider.isTrigger = true;
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
	}

	private void CallWhenTrigger(bool bIsTrigger)
	{
		if (bIsTrigger && this.self.isLive)
		{
			string tag = this.self.tag;
			if (tag != null)
			{
				if (SurfaceManager.<>f__switch$map7 == null)
				{
					SurfaceManager.<>f__switch$map7 = new Dictionary<string, int>(2)
					{
						{
							"Hero",
							0
						},
						{
							"Monster",
							1
						}
					};
				}
				int num;
				if (SurfaceManager.<>f__switch$map7.TryGetValue(tag, out num))
				{
					if (num != 0)
					{
						if (num == 1)
						{
							this.SelectMonster();
						}
					}
					else
					{
						this.SelectHero();
					}
				}
			}
			this.TriggerEnter_Task = this.m_CoroutineManager.StartCoroutine(this.TriggerEnter_Coroutine(), true);
		}
		else
		{
			this.TriggerExit();
		}
	}

	[DebuggerHidden]
	private IEnumerator TriggerEnter_Coroutine()
	{
		SurfaceManager.<TriggerEnter_Coroutine>c__Iterator3D <TriggerEnter_Coroutine>c__Iterator3D = new SurfaceManager.<TriggerEnter_Coroutine>c__Iterator3D();
		<TriggerEnter_Coroutine>c__Iterator3D.<>f__this = this;
		return <TriggerEnter_Coroutine>c__Iterator3D;
	}

	private void SelectHero()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.SelectHero(this.m_selectColor);
		}
	}

	private void SelectMonster()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.SelectMonster(this.m_selectColor);
		}
	}

	private void TriggerExit()
	{
		string tag = this.self.tag;
		if (tag != null)
		{
			if (SurfaceManager.<>f__switch$map8 == null)
			{
				SurfaceManager.<>f__switch$map8 = new Dictionary<string, int>(2)
				{
					{
						"Hero",
						0
					},
					{
						"Monster",
						1
					}
				};
			}
			int num;
			if (SurfaceManager.<>f__switch$map8.TryGetValue(tag, out num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						this.DeselectMonster();
					}
				}
				else
				{
					this.DeselectHero();
				}
			}
		}
		this.m_CoroutineManager.StopCoroutine(this.TriggerEnter_Task);
	}

	private void DeselectHero()
	{
		if (this.self.team.ID == 1)
		{
			return;
		}
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.DeselectHero(this.m_baseColor, this.m_outlineColor);
		}
	}

	[DebuggerHidden]
	private IEnumerator DeselectHero_Coroutine(float time)
	{
		SurfaceManager.<DeselectHero_Coroutine>c__Iterator3E <DeselectHero_Coroutine>c__Iterator3E = new SurfaceManager.<DeselectHero_Coroutine>c__Iterator3E();
		<DeselectHero_Coroutine>c__Iterator3E.time = time;
		<DeselectHero_Coroutine>c__Iterator3E.<$>time = time;
		<DeselectHero_Coroutine>c__Iterator3E.<>f__this = this;
		return <DeselectHero_Coroutine>c__Iterator3E;
	}

	private void DeselectMonster()
	{
		if (this.self.team.ID == 1)
		{
			return;
		}
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.DeselectMonster(this.m_baseShader, this.m_outlineColor);
		}
	}

	[DebuggerHidden]
	private IEnumerator DeselectMonster_Coroutine(float time)
	{
		SurfaceManager.<DeselectMonster_Coroutine>c__Iterator3F <DeselectMonster_Coroutine>c__Iterator3F = new SurfaceManager.<DeselectMonster_Coroutine>c__Iterator3F();
		<DeselectMonster_Coroutine>c__Iterator3F.time = time;
		<DeselectMonster_Coroutine>c__Iterator3F.<$>time = time;
		<DeselectMonster_Coroutine>c__Iterator3F.<>f__this = this;
		return <DeselectMonster_Coroutine>c__Iterator3F;
	}

	private void Deselect()
	{
		string tag = this.self.tag;
		if (tag != null)
		{
			if (SurfaceManager.<>f__switch$map9 == null)
			{
				SurfaceManager.<>f__switch$map9 = new Dictionary<string, int>(2)
				{
					{
						"Hero",
						0
					},
					{
						"Monster",
						1
					}
				};
			}
			int num;
			if (SurfaceManager.<>f__switch$map9.TryGetValue(tag, out num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						this.m_CoroutineManager.StartCoroutine(this.DeselectMonster_Coroutine(0.1f), true);
					}
				}
				else
				{
					this.m_CoroutineManager.StartCoroutine(this.DeselectHero_Coroutine(0.1f), true);
				}
			}
		}
		this.m_CoroutineManager.StopCoroutine(this.TriggerEnter_Task);
	}

	public void jumpFont(string text, Color color)
	{
		if (this.self != null && (this.self.IsHideEffect || this.self.m_nVisibleState >= 2))
		{
			return;
		}
		this.jumpFont(text, color, 0f);
	}

	private void jumpFont(string text, Color color, float time)
	{
		if (this.mText != null)
		{
			this.mText.Add(text, color, time);
		}
	}

	public Transform MarkAsTarget()
	{
		if (this._mTargetIcon == null)
		{
			if (this.self.tag == "Building" || this.self.tag == "Home")
			{
				this._mTargetIcon = this.CreateTargetIcon("TargetIcon2");
			}
			else
			{
				this._mTargetIcon = this.CreateTargetIcon("TargetIcon");
			}
		}
		else
		{
			ClickIcon.Get(this.mTargetIcon.gameObject).Init(ClickIconType.TargetIcon);
		}
		if (this.self.tag == "Hero" || this.self.tag == "Monster")
		{
			if (this.self.isMonster)
			{
				this.SetOutline(MyColor.Red, 0.035f);
			}
			else
			{
				this.SetOutline(MyColor.Red, 0.035f);
			}
			this.CreateCurve(0.8f, 2f, 0.5f);
		}
		this.animTime = 0f;
		this.self.isSelected = true;
		return this.mTargetIcon;
	}

	private void CreateCurve(float bottomValue, float topValue, float duration)
	{
		this.outlineAnimCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, bottomValue),
			new Keyframe(duration / 2f, topValue),
			new Keyframe(duration, bottomValue)
		});
		this.outlineAnimCurveKeys = this.outlineAnimCurve.keys;
	}

	private ResourceHandle CreateTargetIcon(string iconName)
	{
		ResourceHandle resourceHandle;
		if (iconName == "TargetIcon2")
		{
			if (ClickIcon.iconHandle2 == null)
			{
				ClickIcon.iconHandle2 = MapManager.Instance.SpawnResourceHandle(iconName, null, 0);
				if (ClickIcon.iconHandle2 == null)
				{
					ClickIcon.iconHandle2 = MapManager.Instance.SpawnResourceHandle("TargetIcon", null, 0);
				}
			}
			resourceHandle = ClickIcon.iconHandle2;
		}
		else
		{
			if (ClickIcon.iconHandle1 == null)
			{
				ClickIcon.iconHandle1 = MapManager.Instance.SpawnResourceHandle(iconName, null, 0);
				if (ClickIcon.iconHandle1 == null)
				{
					ClickIcon.iconHandle1 = MapManager.Instance.SpawnResourceHandle("TargetIcon", null, 0);
				}
			}
			resourceHandle = ClickIcon.iconHandle1;
		}
		Transform raw = resourceHandle.Raw;
		raw.parent = this.self.transform;
		raw.position = new Vector3(this.self.transform.position.x, this.self.transform.position.y + 0.1f, this.self.transform.position.z);
		raw.localEulerAngles = Vector3.zero;
		ClickIcon clickIcon = ClickIcon.Get(raw.gameObject);
		if (clickIcon == null)
		{
			return resourceHandle;
		}
		clickIcon.Init(ClickIconType.TargetIcon);
		if (this.self.tag == "Home" || this.self.tag == "Building")
		{
			clickIcon.rotateSpeedFactor = 0.6f;
		}
		if (this.self.gameObject == null)
		{
			return resourceHandle;
		}
		float d = this.self.m_SelectRadius / this.self.gameObject.transform.localScale.x * 2.4f;
		raw.localScale = Vector3.one * d;
		return resourceHandle;
	}

	public void ClearTarget()
	{
		this.ClearTargetIcon();
		this.ClearOutline();
		this.self.isSelected = false;
	}

	private void ClearTargetIcon()
	{
		if (this._mTargetIcon != null)
		{
			this._mTargetIcon.Raw.position = new Vector3(this._mTargetIcon.Raw.position.x + 9999f, this._mTargetIcon.Raw.position.y + 9999f, this._mTargetIcon.Raw.position.z + 9999f);
			this._mTargetIcon = null;
		}
	}

	public void MarkAsMainPlayer()
	{
		if (this.mainPlayerIcon == null)
		{
			this.CreateMainPlayerIcon();
		}
		else
		{
			ClickIcon.Get(this.mainPlayerIcon.gameObject).Init(ClickIconType.MainPlayerIcon);
		}
		this.SetOutline(Color.black, 0.035f);
	}

	public void CreateMainPlayerIcon()
	{
		this._mainPlayerIcon = MapManager.Instance.SpawnResourceHandle("MainPlayerIcon", null, 0);
		Transform mainPlayerIcon = this.mainPlayerIcon;
		mainPlayerIcon.parent = this.self.transform;
		mainPlayerIcon.localPosition = Vector3.zero;
		mainPlayerIcon.localEulerAngles = Vector3.zero;
		ClickIcon.Get(mainPlayerIcon.gameObject).Init(ClickIconType.MainPlayerIcon);
		if (this.self.isMyTeam && this.self.isHero)
		{
			mainPlayerIcon.gameObject.SetActive(true);
		}
		else
		{
			mainPlayerIcon.gameObject.SetActive(false);
		}
	}

	public void ClearMainPlayer()
	{
		if (this._mainPlayerIcon != null)
		{
			ClickIcon.Get(this.mainPlayerIcon.gameObject).Clean();
			this._mainPlayerIcon.DelayRelease(0.2f);
			this._mainPlayerIcon = null;
		}
		this.ClearOutline();
	}

	public void ClearTargetAndMainPool()
	{
		this.ClearTargetIcon();
		if (this._mainPlayerIcon != null)
		{
			this._mainPlayerIcon.Release();
			this._mainPlayerIcon = null;
		}
	}

	public void MarkAsPlayer(bool isplayer)
	{
		if (isplayer)
		{
			if (this.mCharacterEffect != null)
			{
				this.mCharacterEffect.SetColor_Outline(Color.black);
			}
			if (this.mHpBar != null)
			{
				this.mHpBar.UpdateHudBarType(this.self);
			}
		}
		else
		{
			if (this.mCharacterEffect != null)
			{
				this.mCharacterEffect.SetColor_Outline(MyColor.Blue);
			}
			if (this.mHpBar != null)
			{
				this.mHpBar.UpdateHudBarType(this.self);
			}
		}
	}

	public void MarkAsAttacker(Units attacker)
	{
		if (attacker != null && attacker.isLive && base.gameObject.activeInHierarchy)
		{
			attacker.SetOutline(MyColor.Blue, 0.08f);
		}
	}

	public void ClearSurface(bool isRevertMaterial = true)
	{
		this.ClearTargetAndMainPool();
		this.ClearOutline();
		this.ClearEffects(isRevertMaterial);
		this.HideShadow();
		this.DestroySkillPointer();
		this.DestroyHUDBarCallBack();
		this.DestroyHUDBar();
	}

	private void DestroyHUDBar()
	{
		if (this.mHpBar != null)
		{
			Singleton<CharacterView>.Instance.DestroyHudBar(this.self);
		}
		this.mHpBar = null;
	}

	private void DestroyHUDBarCallBack()
	{
		if (this.mHpBar != null)
		{
			this.mHpBar.UnRegisterEvent();
		}
	}

	private void DestroyHUDText()
	{
		if (this.mText != null)
		{
			this.mText.ShowHudText(false);
			Singleton<CharacterView>.Instance.DestroyHudText(this.self);
		}
		this.mText = null;
	}

	public void ShowShadow()
	{
	}

	public void HideShadow()
	{
	}

	public void Dissolve(float time)
	{
		this.Add2Mats(this.mMaterial);
		if (this.mShader != SurfaceManager.deathMat.shader)
		{
			this.mShader = SurfaceManager.deathMat.shader;
			this.mMaterial.SetTexture("_DissolveSrc", SurfaceManager.deathMat.GetTexture("_DissolveSrc"));
			this.mMaterial.SetColor("_Color", SurfaceManager.deathMat.GetColor("_Color"));
			this.mMaterial.SetColor("_SpecColor", SurfaceManager.deathMat.GetColor("_SpecColor"));
			this.mMaterial.SetColor("_DissColor", SurfaceManager.deathMat.GetColor("_DissColor"));
			this.mMaterial.SetFloat("_Illuminate", SurfaceManager.deathMat.GetFloat("_Illuminate"));
			this.mMaterial.SetVector("_ColorAnimate", SurfaceManager.deathMat.GetVector("_ColorAnimate"));
		}
		this.mMaterial.SetFloat("_Amount", 0f);
		this.DissolveEffect_Task = this.m_CoroutineManager.StartCoroutine(this.ExcuteDissolve_Coroutine(), true);
		this.m_CoroutineManager.StartCoroutine(this.DissolveCallback_Invoke(time), true);
	}

	[DebuggerHidden]
	private IEnumerator DissolveCallback_Invoke(float time)
	{
		SurfaceManager.<DissolveCallback_Invoke>c__Iterator40 <DissolveCallback_Invoke>c__Iterator = new SurfaceManager.<DissolveCallback_Invoke>c__Iterator40();
		<DissolveCallback_Invoke>c__Iterator.time = time;
		<DissolveCallback_Invoke>c__Iterator.<$>time = time;
		<DissolveCallback_Invoke>c__Iterator.<>f__this = this;
		return <DissolveCallback_Invoke>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator ExcuteDissolve_Coroutine()
	{
		SurfaceManager.<ExcuteDissolve_Coroutine>c__Iterator41 <ExcuteDissolve_Coroutine>c__Iterator = new SurfaceManager.<ExcuteDissolve_Coroutine>c__Iterator41();
		<ExcuteDissolve_Coroutine>c__Iterator.<>f__this = this;
		return <ExcuteDissolve_Coroutine>c__Iterator;
	}

	public void ShowRimLight()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.UseRimlight();
		}
	}

	public void UseOutLine()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.UseOutline();
		}
	}

	public void ShowOutFlash()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.SetOutFlash();
		}
	}

	public void EnableAllRenders(bool b)
	{
		bool isHero = this.self.isHero;
		if (this.cacheGameObj == null)
		{
			return;
		}
		if (this.mRenders != null)
		{
			for (int i = 0; i < this.mRenders.Length; i++)
			{
				if (this.mRenders[i] != null)
				{
					if (isHero)
					{
						this.mRenders[i].enabled = b;
					}
					else
					{
						this.mRenders[i].enabled = b;
					}
				}
			}
		}
		this.mParticles = this.cacheGameObj.GetComponentsInChildren<ParticleSystem>();
		if (this.mParticles != null)
		{
			for (int j = 0; j < this.mParticles.Length; j++)
			{
				if (this.mParticles[j] != null)
				{
					if (b)
					{
						this.mParticles[j].Play();
					}
					else
					{
						this.mParticles[j].Stop();
					}
				}
			}
		}
	}

	public void ShowPetrifaction()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.UsePetrifaction();
		}
	}

	public void ShowAlpha(bool playOrStopPS, float endAlpha = 1f, float duration = 0.02f, float delay = 0f)
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.ShowAlpha(playOrStopPS, endAlpha, duration, delay);
		}
	}

	public void ShowParticle(bool playOrStopPS)
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.ShowParticle(playOrStopPS);
		}
	}

	public void StopAlphaCoroutine()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.StopAlphaCoroutine();
		}
	}

	public float GetAlpha()
	{
		if (this.mCharacterEffect != null)
		{
			return this.mCharacterEffect.m_rimAlpha;
		}
		return -1f;
	}

	public void SetAlpha(float alpha)
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.SetAlpha(alpha);
		}
	}

	public void SetParticlesVisible(bool isShow)
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.SetParticlesVisible(isShow);
		}
	}

	public void StopAlphaTween()
	{
		if (this.mCharacterEffect != null)
		{
			this.mCharacterEffect.StopAlphaTween();
		}
	}

	public void UpdateHUDBar()
	{
		if (this.mHpBar != null)
		{
			this.mHpBar.UpdateView();
		}
	}

	public void SetHUDBarActive(bool act)
	{
		if (this.mHpBar != null)
		{
			this.mHpBar.setActive(act);
		}
	}

	private bool IsHaveHudBarAndHudText()
	{
		if (this.self.isItem || this.self.isBuffItem)
		{
			return false;
		}
		if (!this.self.isMonster && !this.self.isTower)
		{
			return true;
		}
		SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(this.self.npc_id);
		return monsterMainData == null || monsterMainData.is_display_head_ui > 0;
	}

	public static void ClearResources()
	{
		SurfaceManager.deathMat = null;
		SurfaceManager._deathMat = null;
	}
}
