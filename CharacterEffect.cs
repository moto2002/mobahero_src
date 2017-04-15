using Com.Game.Data;
using Com.Game.Manager;
using Holoville.HOTween;
using MobaFrame.SkillAction;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CharacterEffect : MonoBehaviour
{
	private enum eShaderType
	{
		Outline,
		Rimlight,
		Petrifaction,
		Grow
	}

	public static bool NoEditMat;

	private GameObject cacheGameObject;

	[NonSerialized]
	public Units self;

	private static Shader m_outlineShader;

	private static Shader m_outlineShadersd;

	private static Shader m_rimlightShader;

	private static Shader m_highlightShader;

	private static Shader m_petrifaction;

	private static Shader transparentShder;

	private static Shader m_backHomeShader;

	private static Shader m_growShader;

	private Color m_outlineColor;

	private Color m_rimlightColor;

	private Color m_highlightColor;

	private Renderer[] m_renderers;

	private CharacterEffect.eShaderType m_curShaderType;

	private float m_outlineWidth;

	private float m_rimlightWidth;

	private float m_highlightWidth;

	private float m_timeRimlight;

	private bool m_bIsHero = true;

	[NonSerialized]
	public float m_rimAlpha = 1f;

	[NonSerialized]
	public bool isActive;

	public ParticleSystem[] ExtraParticleSystem;

	public Renderer[] ExtraRender;

	private float shadowHeight;

	private CoroutineManager alphaCoroutineManager = new CoroutineManager();

	private Task alphaTask;

	private Tweener tweenerAlpha;

	private static bool enabeloutline = true;

	private static int enabeloutlineLevel = -1;

	public bool IsLockEffect
	{
		get
		{
			return this.self && this.self.isLive && this.self.IsLockCharaEffect;
		}
	}

	public void Awake()
	{
		if (CharacterEffect.m_outlineShader == null)
		{
			CharacterEffect.m_outlineShader = Shader.Find("MyShader/RimLight_Outline");
		}
		if (CharacterEffect.m_outlineShadersd == null)
		{
			CharacterEffect.m_outlineShadersd = Shader.Find("MyShader/RimLight_Outline_SD");
		}
		if (CharacterEffect.m_rimlightShader == null)
		{
			CharacterEffect.m_rimlightShader = Shader.Find("MyShader/RimLight");
		}
		if (CharacterEffect.m_highlightShader == null)
		{
			CharacterEffect.m_highlightShader = Shader.Find("MyShader/HighLight");
		}
		if (CharacterEffect.m_petrifaction == null)
		{
			CharacterEffect.m_petrifaction = Shader.Find("Self-Illumin/Diffuse");
		}
		if (CharacterEffect.m_backHomeShader == null)
		{
			CharacterEffect.m_backHomeShader = Shader.Find("MyShader/RimLight_Outline_Alpha");
		}
		if (CharacterEffect.transparentShder == null)
		{
			CharacterEffect.transparentShder = Shader.Find("Transparent/VertexLit");
		}
		if (CharacterEffect.m_growShader == null)
		{
			CharacterEffect.m_growShader = Shader.Find("Tut/HeroGlow");
		}
		this.m_outlineWidth = 0.035f;
		this.m_rimlightColor = MyColor.White;
		this.m_rimlightWidth = 1f;
		this.m_rimAlpha = 1f;
		this.cacheGameObject = base.gameObject;
	}

	public Shader GetOutLineShader()
	{
		return CharacterEffect.m_outlineShadersd;
	}

	public void SetRenderer(Renderer[] renders)
	{
		this.m_renderers = renders;
	}

	public void InitRenderer(Units selfUnit, Renderer[] renders)
	{
		this.self = selfUnit;
		this.m_renderers = renders;
		this.m_curShaderType = CharacterEffect.eShaderType.Outline;
		this.m_rimAlpha = 1f;
		this.CastShadows(true);
		this.alphaCoroutineManager.StopAllCoroutine();
	}

	public int getScenetype()
	{
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
		if (dataById != null)
		{
			return dataById.hero1_number_cap;
		}
		return -1;
	}

	public void SetIsHero(bool isHero)
	{
		if (CharacterEffect.NoEditMat)
		{
			return;
		}
		if (this.m_renderers == null)
		{
			return;
		}
		this.m_bIsHero = isHero;
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null)
			{
				if (this.m_renderers[i] is ParticleSystemRenderer)
				{
					if (this.m_bIsHero)
					{
						this.m_renderers[i].gameObject.layer = Layer.UnitLayer;
					}
					else
					{
						this.m_renderers[i].gameObject.layer = Layer.MonsterLayer;
					}
				}
				else
				{
					this.m_renderers[i].material.shader = CharacterEffect.m_outlineShadersd;
					this.m_renderers[i].material.SetFloat("_OutlineWidth", this.m_outlineWidth);
					this.m_renderers[i].material.SetFloat("_Cutoff", 0.5f);
					this.m_renderers[i].material.SetColor("_SDColor", new Color(0.1176f, 0.1176f, 0.1176f, 1f));
					if (this.m_bIsHero)
					{
						this.m_renderers[i].gameObject.layer = Layer.UnitLayer;
					}
					else
					{
						this.m_renderers[i].gameObject.layer = Layer.MonsterLayer;
					}
				}
			}
		}
	}

	public void SetColor_Outline(Color color)
	{
		if (this.m_renderers == null)
		{
			return;
		}
		if (this.m_outlineColor == color)
		{
			return;
		}
		this.m_outlineColor = color;
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null && this.m_renderers[i].GetType() != typeof(ParticleSystemRenderer))
			{
				Material material = this.m_renderers[i].material;
				material.SetColor("_Color", MyColor.White);
				material.SetColor("_OutlineColor", this.m_outlineColor);
				material.SetFloat("NBlendV", 0f);
			}
		}
	}

	public void SetOutlineWidth(float width)
	{
		if (this.m_renderers == null)
		{
			return;
		}
		float num;
		if (GlobalSettings.Instance.ChaOutlineLevel == 0)
		{
			num = 0f;
		}
		else if (GlobalSettings.Instance.ChaOutlineLevel == 1)
		{
			num = 0.5f * width;
		}
		else
		{
			num = width;
		}
		if (this.m_outlineWidth == num)
		{
			return;
		}
		this.m_outlineWidth = num;
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null && this.m_renderers[i].GetType() != typeof(ParticleSystemRenderer))
			{
				this.m_renderers[i].material.SetFloat("_OutlineWidth", this.m_outlineWidth);
			}
		}
	}

	public void SetShadowHeight(float h)
	{
		if (this.m_renderers == null)
		{
			return;
		}
		if (Math.Abs(this.shadowHeight - h) < 0.01f)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null && this.m_renderers[i].GetType() != typeof(ParticleSystemRenderer))
			{
				this.m_renderers[i].material.SetFloat("_Height", h);
				this.shadowHeight = h;
			}
		}
	}

	public void UseOutline()
	{
		if (this.IsLockEffect)
		{
			return;
		}
		if (this.m_renderers == null)
		{
			return;
		}
		this.m_curShaderType = CharacterEffect.eShaderType.Outline;
		this.m_timeRimlight = 0f;
		bool flag = this.self.isHero || this.self.isMonster;
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null && this.m_renderers[i].GetType() != typeof(ParticleSystemRenderer))
			{
				this.m_renderers[i].material.shader = ((!flag) ? CharacterEffect.m_outlineShader : CharacterEffect.m_outlineShadersd);
				this.m_renderers[i].material.SetFloat("_OutlineWidth", this.m_outlineWidth);
				if (flag)
				{
					this.m_renderers[i].material.SetColor("_SDColor", new Color(0.1176f, 0.1176f, 0.1176f, 1f));
				}
			}
		}
	}

	public void UseRimlight()
	{
		if (!this.isActive)
		{
			return;
		}
		if (this.IsLockEffect)
		{
			return;
		}
		if (this.m_renderers == null)
		{
			return;
		}
		if (this.m_bIsHero)
		{
			this.m_curShaderType = CharacterEffect.eShaderType.Rimlight;
			for (int i = 0; i < this.m_renderers.Length; i++)
			{
				if (this.m_renderers[i] != null && this.m_renderers[i].GetType() != typeof(ParticleSystemRenderer))
				{
					this.m_renderers[i].material.shader = CharacterEffect.m_rimlightShader;
					this.m_renderers[i].material.SetFloat("_OutlineWidth", this.m_outlineWidth);
					this.m_renderers[i].material.SetColor("_RimColor", this.m_rimlightColor);
					this.m_renderers[i].material.SetFloat("_RimWidth", 1f);
				}
			}
			this.m_timeRimlight = this.m_rimlightWidth;
		}
		else
		{
			this.m_curShaderType = CharacterEffect.eShaderType.Rimlight;
			for (int j = 0; j < this.m_renderers.Length; j++)
			{
				if (this.m_renderers[j] != null && this.m_renderers[j].GetType() != typeof(ParticleSystemRenderer))
				{
					this.m_renderers[j].material.shader = CharacterEffect.m_rimlightShader;
					this.m_renderers[j].material.SetFloat("_OutlineWidth", this.m_outlineWidth);
					this.m_renderers[j].material.SetColor("_RimColor", this.m_rimlightColor);
					this.m_renderers[j].material.SetFloat("_RimWidth", 1f);
				}
			}
			this.m_timeRimlight = this.m_rimlightWidth;
		}
	}

	public void UsePetrifaction()
	{
		if (this.IsLockEffect)
		{
			return;
		}
		if (this.m_renderers == null)
		{
			return;
		}
		this.m_curShaderType = CharacterEffect.eShaderType.Petrifaction;
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null && this.m_renderers[i].GetType() != typeof(ParticleSystemRenderer))
			{
				this.m_renderers[i].material.shader = CharacterEffect.m_petrifaction;
				this.m_renderers[i].material.SetColor("_Color", Color.gray);
			}
		}
	}

	public void ShowAlpha(bool playOrStopPS, float endAlpha = 1f, float duration = 0.02f, float delay = 0f)
	{
		if (CharacterEffect.NoEditMat)
		{
			return;
		}
		this.StopAlphaTween();
		this.alphaCoroutineManager.StopAllCoroutine();
		this.alphaTask = this.alphaCoroutineManager.StartCoroutine(this.TweenAlpha(playOrStopPS, endAlpha, duration, delay), true);
	}

	public void StopAlphaTween()
	{
		if (this.tweenerAlpha != null)
		{
			HOTween.Kill(this.tweenerAlpha);
		}
		if (this.alphaTask != null)
		{
			this.alphaTask.Stop();
		}
	}

	public void StopAlphaCoroutine()
	{
		if (this.alphaCoroutineManager != null)
		{
			this.alphaCoroutineManager.StopAllCoroutine();
		}
	}

	public void SetAlpha(float alpha)
	{
		if (CharacterEffect.NoEditMat)
		{
			return;
		}
		this.m_rimAlpha = alpha;
		if (alpha < 1f)
		{
			this.OnAlphaStart(false);
		}
		else
		{
			this.OnAlphaStart(true);
		}
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null)
			{
				if (alpha > 0f)
				{
					this.m_renderers[i].enabled = true;
				}
				this.m_renderers[i].material.SetFloat("amt", alpha);
			}
		}
		this.updataherovisible(this.self, alpha);
	}

	private void updataherovisible(Units unts, float alp)
	{
	}

	private void OnAlphaUpdate()
	{
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null)
			{
				this.m_renderers[i].material.SetFloat("amt", this.m_rimAlpha);
			}
		}
		this.updataherovisible(this.self, this.m_rimAlpha);
	}

	private void OnAlphaStart(bool playOrStopPS)
	{
		if (this.m_renderers == null)
		{
			return;
		}
		bool flag = this.self.isHero || this.self.isMonster;
		if (!playOrStopPS)
		{
			for (int i = 0; i < this.m_renderers.Length; i++)
			{
				if (this.m_renderers[i] != null && this.m_renderers[i].GetType() != typeof(ParticleSystemRenderer))
				{
					this.m_renderers[i].material.shader = CharacterEffect.m_backHomeShader;
				}
			}
			if (this.ExtraParticleSystem != null)
			{
				for (int j = 0; j < this.ExtraParticleSystem.Length; j++)
				{
					if (!(this.ExtraParticleSystem[j] == null))
					{
						this.ExtraParticleSystem[j].Stop();
						this.ExtraParticleSystem[j].gameObject.SetActive(false);
					}
				}
			}
			if (this.ExtraRender != null)
			{
				for (int k = 0; k < this.ExtraRender.Length; k++)
				{
					if (this.ExtraRender[k] != null)
					{
						this.ExtraRender[k].enabled = false;
					}
				}
			}
		}
		else
		{
			if (this.self.m_nVisibleState < 2)
			{
				for (int l = 0; l < this.m_renderers.Length; l++)
				{
					if (this.m_renderers[l] != null && this.m_renderers[l].GetType() != typeof(ParticleSystemRenderer))
					{
						this.m_renderers[l].material.shader = ((!flag) ? CharacterEffect.m_outlineShader : CharacterEffect.m_outlineShadersd);
						if (flag)
						{
							this.m_renderers[l].material.SetColor("_SDColor", new Color(0.1176f, 0.1176f, 0.1176f, 1f));
						}
					}
				}
			}
			if (this.ExtraParticleSystem != null)
			{
				for (int m = 0; m < this.ExtraParticleSystem.Length; m++)
				{
					if (!(this.ExtraParticleSystem[m] == null))
					{
						this.ExtraParticleSystem[m].gameObject.SetActive(true);
						this.ExtraParticleSystem[m].Play();
					}
				}
			}
			if (this.ExtraRender != null)
			{
				for (int n = 0; n < this.ExtraRender.Length; n++)
				{
					if (this.ExtraRender[n] != null)
					{
						this.ExtraRender[n].enabled = true;
					}
				}
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator TweenAlpha(bool playOrStopPS, float endAlpha = 1f, float duration = 0.02f, float delay = 0f)
	{
		CharacterEffect.<TweenAlpha>c__Iterator39 <TweenAlpha>c__Iterator = new CharacterEffect.<TweenAlpha>c__Iterator39();
		<TweenAlpha>c__Iterator.delay = delay;
		<TweenAlpha>c__Iterator.playOrStopPS = playOrStopPS;
		<TweenAlpha>c__Iterator.duration = duration;
		<TweenAlpha>c__Iterator.endAlpha = endAlpha;
		<TweenAlpha>c__Iterator.<$>delay = delay;
		<TweenAlpha>c__Iterator.<$>playOrStopPS = playOrStopPS;
		<TweenAlpha>c__Iterator.<$>duration = duration;
		<TweenAlpha>c__Iterator.<$>endAlpha = endAlpha;
		<TweenAlpha>c__Iterator.<>f__this = this;
		return <TweenAlpha>c__Iterator;
	}

	public void ShowParticle(bool playOrStopPS)
	{
		this.OnAlphaStart(playOrStopPS);
		ParticleSystem[] componentsInChildren = this.cacheGameObject.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (playOrStopPS)
			{
				componentsInChildren[i].Play();
			}
			else
			{
				componentsInChildren[i].Stop();
			}
		}
		this.CastShadows(playOrStopPS);
	}

	public void CastShadows(bool b)
	{
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null)
			{
				if (b)
				{
					this.m_renderers[i].castShadows = true;
				}
				else
				{
					this.m_renderers[i].castShadows = false;
				}
			}
		}
	}

	public void SetParticlesVisible(bool isShow)
	{
		if (this.cacheGameObject == null)
		{
			return;
		}
		ParticleAdapter[] componentsInChildren = this.cacheGameObject.GetComponentsInChildren<ParticleAdapter>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ShowRenders(isShow);
			}
		}
		EffectPlayer[] componentsInChildren2 = this.cacheGameObject.GetComponentsInChildren<EffectPlayer>();
		if (componentsInChildren2 != null)
		{
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].ShowRenders(isShow);
			}
		}
		if (componentsInChildren == null && componentsInChildren2 == null)
		{
			Renderer[] componentsInChildren3 = this.cacheGameObject.GetComponentsInChildren<Renderer>();
			if (componentsInChildren3 != null)
			{
				for (int k = 0; k < componentsInChildren3.Length; k++)
				{
					if (componentsInChildren3[k] != null)
					{
						componentsInChildren3[k].enabled = isShow;
						if (this.self.isHero)
						{
						}
					}
				}
			}
		}
	}

	private void Update()
	{
		if (this.self == null)
		{
			return;
		}
		if (!this.self.isLive)
		{
			return;
		}
		if (this.IsLockEffect)
		{
			return;
		}
		if (CharacterEffect.enabeloutlineLevel != GlobalSettings.Instance.ChaOutlineLevel)
		{
			CharacterEffect.enabeloutlineLevel = GlobalSettings.Instance.ChaOutlineLevel;
			this.SetOutlineWidth(0.035f);
		}
		if (this.m_bIsHero)
		{
			if (this.m_timeRimlight > 0f)
			{
				this.UpdateRimlightWidth();
				this.m_timeRimlight -= Time.deltaTime * 4f;
				if (this.m_curShaderType != CharacterEffect.eShaderType.Rimlight)
				{
					this.m_curShaderType = CharacterEffect.eShaderType.Rimlight;
				}
			}
			else if (this.m_curShaderType != CharacterEffect.eShaderType.Outline)
			{
				this.UseOutline();
				this.m_curShaderType = CharacterEffect.eShaderType.Outline;
			}
		}
		else if (this.m_timeRimlight > 0f)
		{
			this.UpdateRimlightWidth();
			this.m_timeRimlight -= Time.deltaTime * 4f;
			if (this.m_curShaderType != CharacterEffect.eShaderType.Rimlight)
			{
				this.m_curShaderType = CharacterEffect.eShaderType.Rimlight;
			}
		}
		else if (this.m_curShaderType != CharacterEffect.eShaderType.Outline && this.self.m_nVisibleState != 2)
		{
			this.UseOutline();
			this.m_curShaderType = CharacterEffect.eShaderType.Outline;
		}
	}

	private void UpdateRimlightWidth()
	{
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null && this.m_renderers[i].GetType() != typeof(ParticleSystemRenderer))
			{
				this.m_renderers[i].material.SetFloat("_RimWidth", this.m_timeRimlight);
				this.m_renderers[i].material.SetFloat("_OutlineWidth", this.m_outlineWidth);
			}
		}
	}

	public void UseGrow()
	{
		if (this.IsLockEffect)
		{
			return;
		}
		if (this.m_renderers == null)
		{
			return;
		}
		this.m_curShaderType = CharacterEffect.eShaderType.Grow;
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null)
			{
				this.m_renderers[i].material.shader = CharacterEffect.m_growShader;
			}
		}
	}

	public void SelectHero(Color selectColor)
	{
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (!this.CheckRenderer(i))
			{
				if (this.m_renderers[i] != null)
				{
					this.m_renderers[i].material.SetColor("_Color", selectColor);
					this.m_renderers[i].material.SetColor("_OutlineColor", selectColor);
				}
			}
		}
	}

	public void SelectMonster(Color selectColor)
	{
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (!this.CheckRenderer(i))
			{
				if (this.m_renderers[i] != null && this.m_renderers[i].GetType() != typeof(ParticleSystemRenderer))
				{
					this.m_renderers[i].material.shader = CharacterEffect.m_outlineShader;
					this.m_renderers[i].material.SetColor("_Color", selectColor);
					this.m_renderers[i].material.SetColor("_OutlineColor", selectColor);
				}
			}
		}
	}

	public void DeselectHero(Color baseColor, Color outlineColor)
	{
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (!this.CheckRenderer(i))
			{
				if (this.m_renderers[i] != null)
				{
					this.m_renderers[i].material.SetColor("_Color", baseColor);
					this.m_renderers[i].material.SetColor("_OutlineColor", outlineColor);
				}
			}
		}
	}

	public void DeselectMonster(Shader baseShader, Color baseColor)
	{
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (!this.CheckRenderer(i))
			{
				if (this.m_renderers[i] != null)
				{
					this.m_renderers[i].material.shader = baseShader;
					this.m_renderers[i].material.SetColor("_Color", baseColor);
				}
			}
		}
	}

	private bool CheckRenderer(int index)
	{
		return this.m_renderers != null && (this.m_renderers[index] == null || this.m_renderers[index].gameObject.name == "Shadow" || this.m_renderers[index].gameObject.name == "MonsterSelect");
	}

	public void HideModel()
	{
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null)
			{
				this.m_renderers[i].gameObject.SetActive(false);
			}
		}
	}

	public void RevertModel()
	{
		if (this.m_renderers == null)
		{
			return;
		}
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			if (this.m_renderers[i] != null)
			{
				this.m_renderers[i].gameObject.SetActive(true);
			}
		}
	}

	public void SetOutFlash()
	{
		if (this.IsLockEffect)
		{
			return;
		}
		if (this.m_renderers == null)
		{
			return;
		}
		if (this.m_bIsHero)
		{
			this.m_curShaderType = CharacterEffect.eShaderType.Rimlight;
			for (int i = 0; i < this.m_renderers.Length; i++)
			{
				if (this.m_renderers[i] != null)
				{
					this.m_renderers[i].material.shader = CharacterEffect.transparentShder;
					this.m_renderers[i].material.SetColor("_Emission", MyColor.White * 0.7f);
				}
			}
			this.m_timeRimlight = this.m_rimlightWidth * 1.2f;
		}
		else
		{
			this.m_curShaderType = CharacterEffect.eShaderType.Rimlight;
			for (int j = 0; j < this.m_renderers.Length; j++)
			{
				if (this.m_renderers[j] != null)
				{
					this.m_renderers[j].material.shader = CharacterEffect.transparentShder;
					this.m_renderers[j].material.SetColor("_Emission", MyColor.White * 0.7f);
				}
			}
			this.m_timeRimlight = this.m_rimlightWidth * 1.2f;
		}
	}
}
