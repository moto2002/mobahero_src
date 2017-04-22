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
    /// <summary>
    /// shader效果类型
    /// </summary>
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
    /// <summary>
    /// 是否激活
    /// </summary>
	[NonSerialized]
	public bool isActive;
    /// <summary>
    /// 额外粒子效果列表
    /// </summary>
	public ParticleSystem[] ExtraParticleSystem;
    /// <summary>
    /// 额外渲染器列表
    /// </summary>
	public Renderer[] ExtraRender;
    /// <summary>
    /// 阴影高度
    /// </summary>
	private float shadowHeight;

	private CoroutineManager alphaCoroutineManager = new CoroutineManager();
    /// <summary>
    /// alpha变化task
    /// </summary>
	private Task alphaTask;

	private Tweener tweenerAlpha;
    /// <summary>
    /// 是否启用轮廓线效果
    /// </summary>
	private static bool enabeloutline = true;
    /// <summary>
    /// 启用轮廓线级别
    /// </summary>
	private static int enabeloutlineLevel = -1;
    /// <summary>
    /// 是否锁定效果
    /// </summary>
	public bool IsLockEffect
	{
		get
		{
			return this.self && this.self.isLive && this.self.IsLockCharaEffect;
		}
	}
    /// <summary>
    /// 初始化，并获取资源引用
    /// </summary>
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
    /// <summary>
    /// 获取轮廓线shader
    /// </summary>
    /// <returns></returns>
	public Shader GetOutLineShader()
	{
		return CharacterEffect.m_outlineShadersd;
	}
    /// <summary>
    /// 设置渲染器列表
    /// </summary>
    /// <param name="renders"></param>
	public void SetRenderer(Renderer[] renders)
	{
		this.m_renderers = renders;
	}
    /// <summary>
    /// 初始化渲染器等属性
    /// </summary>
    /// <param name="selfUnit"></param>
    /// <param name="renders"></param>
	public void InitRenderer(Units selfUnit, Renderer[] renders)
	{
		this.self = selfUnit;
		this.m_renderers = renders;
		this.m_curShaderType = CharacterEffect.eShaderType.Outline;
		this.m_rimAlpha = 1f;
		this.CastShadows(true);
		this.alphaCoroutineManager.StopAllCoroutine();
	}
    /// <summary>
    /// 获取场景关卡类型
    /// </summary>
    /// <returns></returns>
	public int getScenetype()
	{
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
		if (dataById != null)
		{
			return dataById.hero1_number_cap;
		}
		return -1;
	}
    /// <summary>
    /// 设置是否英雄,并据此设置渲染器相关属性
    /// </summary>
    /// <param name="isHero"></param>
	public void SetIsHero(bool isHero)
	{
		if (CharacterEffect.NoEditMat)//没有编辑材质
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
					if (this.m_bIsHero) //英雄同monster位于不同Layer
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
    /// <summary>
    /// 设置轮廓线效果颜色
    /// </summary>
    /// <param name="color"></param>
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
    /// <summary>
    /// 设置轮廓线效果width
    /// </summary>
    /// <param name="width"></param>
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
    /// <summary>
    /// 设置阴影高度
    /// </summary>
    /// <param name="h"></param>
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
    /// <summary>
    /// 使用轮廓线效果
    /// </summary>
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
    /// <summary>
    /// 使用边缘高亮效果
    /// </summary>
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
    /// <summary>
    /// 使用石化效果
    /// </summary>
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
    /// <summary>
    /// 显示alpha效果
    /// </summary>
    /// <param name="playOrStopPS"></param>
    /// <param name="endAlpha"></param>
    /// <param name="duration"></param>
    /// <param name="delay"></param>
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
    /// <summary>
    /// 停止所有alpha效果
    /// </summary>
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
    /// <summary>
    /// 停止所有alpha效果协程
    /// </summary>
	public void StopAlphaCoroutine()
	{
		if (this.alphaCoroutineManager != null)
		{
			this.alphaCoroutineManager.StopAllCoroutine();
		}
	}
    /// <summary>
    /// 设置整体alpha值
    /// </summary>
    /// <param name="alpha"></param>
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
    /// <summary>
    /// 更新unit的透明度
    /// </summary>
    /// <param name="unts"></param>
    /// <param name="alp"></param>
	private void updataherovisible(Units unts, float alp)
	{
	}
    /// <summary>
    /// 透明度更新
    /// </summary>
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
    /// <summary>
    /// 开始或停止的初始alpha
    /// </summary>
    /// <param name="playOrStopPS"></param>
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
    /// <summary>
    /// 显示或关闭粒子效果
    /// </summary>
    /// <param name="playOrStopPS"></param>
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
    /// <summary>
    /// 启用或关闭投影
    /// </summary>
    /// <param name="b"></param>
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
    /// <summary>
    /// 设置粒子效果是否启用显示
    /// </summary>
    /// <param name="isShow"></param>
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
    /// <summary>
    /// 更新角色效果类型
    /// </summary>
	private void Update()
	{
		if (this.self == null)
		{
			return;
		}
		if (!this.self.isLive)//角色死亡，不更新
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
    /// <summary>
    /// 更新边缘光效果宽度
    /// </summary>
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
    /// <summary>
    /// 使用渐变效果
    /// </summary>
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
    /// <summary>
    /// 选中英雄效果，指定特定选中颜色
    /// </summary>
    /// <param name="selectColor"></param>
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
    /// <summary>
    /// 选中怪物效果，指定怪物选中的颜色
    /// </summary>
    /// <param name="selectColor"></param>
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
    /// <summary>
    /// 取消英雄选中
    /// </summary>
    /// <param name="baseColor"></param>
    /// <param name="outlineColor"></param>
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
    /// <summary>
    /// 取消选中monster效果
    /// </summary>
    /// <param name="baseShader"></param>
    /// <param name="baseColor"></param>
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
    /// <summary>
    /// 检查是否是有效的可选择渲染器游戏对象
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
	private bool CheckRenderer(int index)
	{
		return this.m_renderers != null && (this.m_renderers[index] == null || this.m_renderers[index].gameObject.name == "Shadow" || this.m_renderers[index].gameObject.name == "MonsterSelect");
	}
    /// <summary>
    /// 隐藏模型显示
    /// </summary>
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
    /// <summary>
    /// 恢复模型显示
    /// </summary>
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
    /// <summary>
    /// 设置外部闪光效果
    /// </summary>
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
