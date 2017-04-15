using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class EffectPlayer : BasePlayer
	{
		public int skillNodeId;

		public string performId;

		public float exitTime;

		[SerializeField]
		private ParticleSystem[] m_particleSystems;

		[SerializeField]
		private LineRenderer[] m_lineRender;

		[SerializeField]
		private Renderer[] m_Render;

		private Animator[] m_Animator;

		public ParticleSystem[] mParticleSystem
		{
			get
			{
				return this.m_particleSystems;
			}
		}

		public LineRenderer[] mLineRender
		{
			get
			{
				return this.m_lineRender;
			}
		}

		public Animator[] mAnimator
		{
			get
			{
				return this.m_Animator;
			}
		}

		private void Awake()
		{
			if (this.m_particleSystems == null || this.m_particleSystems.Length == 0)
			{
				this.m_particleSystems = base.GetComponentsInChildren<ParticleSystem>();
			}
			if (this.m_lineRender == null || this.m_lineRender.Length == 0)
			{
				this.m_lineRender = base.GetComponentsInChildren<LineRenderer>();
			}
			this.m_Render = base.GetComponentsInChildren<Renderer>();
			if (this.mAnimator == null || this.mAnimator.Length == 0)
			{
				this.m_Animator = base.GetComponentsInChildren<Animator>();
			}
		}

		private void OnDestroy()
		{
		}

		public override void Play()
		{
			if (this.OnPlayCallback != null)
			{
				this.OnPlayCallback(this.skillNodeId);
			}
		}

		public override void Stop()
		{
			if (this.OnStopCallback != null)
			{
				this.OnStopCallback(this.skillNodeId);
			}
		}

		public void AddEffect(GameObject effect)
		{
			if (effect != null)
			{
				effect.transform.parent = base.transform;
			}
		}

		public void ShowRenders(bool bIsShow)
		{
			if (this.m_Render != null)
			{
				for (int i = 0; i < this.m_Render.Length; i++)
				{
					if (this.m_Render[i] != null)
					{
						this.m_Render[i].enabled = bIsShow;
					}
				}
			}
		}
	}
}
