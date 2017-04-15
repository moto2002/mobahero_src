using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class PlayEffectAction : BaseAction
	{
		public Units CastlerUnit;

		public string performId;

		public Vector3 effectPosition = Vector3.zero;

		public Vector3 effectRoation = Vector3.zero;

		private int skin;

		public string skillId = string.Empty;

		private PerformData data;

		private ResourceHandle _effectHandle;

		public EffectPlayer mEffectPlayer;

		private int meffectVis = -1;

		private Task fadeTask;

		private AudioSourceControl m_audioSourceControl;

		private Task playtask;

		private Task FollowTargetTask;

		public PerformData Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		public bool iseffvis(Units unit)
		{
			if (this.data != null && this.data.isForceDisplay)
			{
				return true;
			}
			bool flag = this.isEffectVisible();
			return !unit.IsHideEffect && flag;
		}

		public bool isEffectVisible()
		{
			if (this.meffectVis >= 0)
			{
				return this.meffectVis == 1;
			}
			if (base.unit != null && base.unit.currentSkillOrAttack != null && !base.unit.isMyTeam)
			{
				bool flag = base.unit.currentSkillOrAttack.isSkillVisible();
				this.meffectVis = ((!flag) ? 2 : 1);
				return flag;
			}
			this.meffectVis = 1;
			return true;
		}

		protected override void OnInit()
		{
			base.OnInit();
			this.data = Singleton<PerformDataManager>.Instance.GetVo(this.performId);
			this.TryAddBornPowerObjSkillData();
		}

		public override void Destroy()
		{
			if (base.IsDestroyed)
			{
				return;
			}
			if (base.gameObject != null)
			{
				AudioMgr.Stop(base.gameObject);
			}
			this.Fade();
			this.ShowDestroyEffect();
		}

		private void doRealRealDestroy()
		{
			if (this._effectHandle != null && this._effectHandle.Raw != null)
			{
				ResourceHandle.SafeRelease(ref this._effectHandle);
			}
			base.Destroy();
			if (base.unit != null)
			{
				base.unit.effectManager.RemoveEffect(this);
			}
		}

		private void Onfinish(bool manual)
		{
			this.doRealRealDestroy();
		}

		private void doRealDestroy()
		{
			if (this._effectHandle != null)
			{
				if (this._effectHandle.Raw)
				{
					this._effectHandle.Raw.gameObject.SetActive(false);
				}
			}
			this.PlayParticles(false);
			if (this.playtask != null)
			{
				base.mCoroutineManager.StopCoroutine(this.playtask);
			}
			base.mCoroutineManager.StopAllCoroutine();
			this.doRealRealDestroy();
		}

		[DebuggerHidden]
		private IEnumerator ParticleSystemStop(float time)
		{
			PlayEffectAction.<ParticleSystemStop>c__Iterator70 <ParticleSystemStop>c__Iterator = new PlayEffectAction.<ParticleSystemStop>c__Iterator70();
			<ParticleSystemStop>c__Iterator.time = time;
			<ParticleSystemStop>c__Iterator.<$>time = time;
			<ParticleSystemStop>c__Iterator.<>f__this = this;
			return <ParticleSystemStop>c__Iterator;
		}

		private void PlayParticles(bool b)
		{
			if (this.mEffectPlayer == null)
			{
				return;
			}
			if (this.mEffectPlayer.mParticleSystem != null)
			{
				for (int i = 0; i < this.mEffectPlayer.mParticleSystem.Length; i++)
				{
					if (this.mEffectPlayer.mParticleSystem[i] != null)
					{
						if (b)
						{
							this.mEffectPlayer.mParticleSystem[i].Play();
						}
						else
						{
							this.mEffectPlayer.mParticleSystem[i].Stop();
						}
					}
				}
			}
		}

		public void Fade()
		{
			if (this.data == null)
			{
				return;
			}
			if (base.mCoroutineManager == null)
			{
				return;
			}
			if (this.data.particleClose_time > 0f)
			{
				this.fadeTask = base.mCoroutineManager.StartCoroutine(this.ParticleSystemStop(this.data.particleClose_time), true);
			}
			else
			{
				this.doRealDestroy();
			}
		}

		protected virtual void ShowDestroyEffect()
		{
			if (this.data == null)
			{
				return;
			}
			if (StringUtils.CheckValid(this.data.endPerformId))
			{
				ActionManager.PlayEffect(this.data.endPerformId, base.unit, null, null, true, string.Empty, null);
			}
		}

		protected override bool doAction()
		{
			if (base.unit == null)
			{
				return false;
			}
			if (!this.CompareTag())
			{
				return false;
			}
			base.unit.effectManager.AddEffect(this);
			base.CreateNode(NodeType.EffectNode, this.performId);
			if (this.data == null)
			{
				ClientLogger.Error("特效没找到 Error performId = " + this.performId);
				return false;
			}
			this.UpdatePosition();
			if (StringUtils.CheckValid(this.data.config.effect_id))
			{
				this._effectHandle = this.SpawnEffect(this.data.config.effect_id, base.transform);
			}
			if (!this.iseffvis(base.unit))
			{
				if (this._effectHandle != null)
				{
					base.unit.effectManager.SetParticlesVisible(this._effectHandle.Raw.gameObject, false);
				}
			}
			else if (this._effectHandle != null)
			{
				base.unit.effectManager.SetParticlesVisible(this._effectHandle.Raw.gameObject, true);
			}
			base.gameObject.SetActive(false);
			base.mCoroutineManager.StartCoroutine(this.PlayEffect_Coroutinue(), true);
			return true;
		}

		private void OnDisable()
		{
			if (this.m_audioSourceControl != null)
			{
				this.m_audioSourceControl.stopPlaying();
				this.m_audioSourceControl = null;
			}
		}

		private void PlayEndSound(GameObject gm)
		{
			if (AudioMgr.Instance.isEffMute())
			{
				return;
			}
			if (AudioMgr.Instance.isUsingWWise() && base.gameObject != null)
			{
				if (AudioGameDataLoader.instance._spellSfx.ContainsKey(this.performId))
				{
					List<AudioGameDataLoader.audioBindstruct> list = AudioGameDataLoader.instance._spellSfx[this.performId];
					if (list != null)
					{
						if (list.Count > 0 && list[0].endevent)
						{
							AudioMgr.Play(list[0].eventstr, gm, false, false);
						}
						else if (list.Count > 1 && list[1].endevent)
						{
							AudioMgr.Play(list[1].eventstr, gm, false, false);
						}
						else if (list.Count > 2 && list[2].endevent)
						{
							AudioMgr.Play(list[2].eventstr, gm, false, false);
						}
					}
				}
				return;
			}
		}

		[DebuggerHidden]
		public IEnumerator PlaySound_Cor()
		{
			PlayEffectAction.<PlaySound_Cor>c__Iterator71 <PlaySound_Cor>c__Iterator = new PlayEffectAction.<PlaySound_Cor>c__Iterator71();
			<PlaySound_Cor>c__Iterator.<>f__this = this;
			return <PlaySound_Cor>c__Iterator;
		}

		private void PlaySound()
		{
			if (AudioMgr.Instance.isEffMute())
			{
				return;
			}
			if (!this.iseffvis(base.unit))
			{
				return;
			}
			if (this.playtask != null)
			{
				base.mCoroutineManager.StopCoroutine(this.playtask);
			}
			this.playtask = base.mCoroutineManager.StartCoroutine(this.PlaySound_Cor(), true);
		}

		[DebuggerHidden]
		private IEnumerator PlaySound_Coroutinue()
		{
			PlayEffectAction.<PlaySound_Coroutinue>c__Iterator72 <PlaySound_Coroutinue>c__Iterator = new PlayEffectAction.<PlaySound_Coroutinue>c__Iterator72();
			<PlaySound_Coroutinue>c__Iterator.<>f__this = this;
			return <PlaySound_Coroutinue>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator PlayEffect_Coroutinue()
		{
			PlayEffectAction.<PlayEffect_Coroutinue>c__Iterator73 <PlayEffect_Coroutinue>c__Iterator = new PlayEffectAction.<PlayEffect_Coroutinue>c__Iterator73();
			<PlayEffect_Coroutinue>c__Iterator.<>f__this = this;
			return <PlayEffect_Coroutinue>c__Iterator;
		}

		public override bool IsForceDisplay()
		{
			return this.data.isForceDisplay || base.IsForceDisplay();
		}

		private void TryHideSelf()
		{
			if (this.data.body_dispear && base.unit != null)
			{
				base.unit.HideSelf(0f);
			}
		}

		private void TryDissolve()
		{
			if (this.data.body_dissolve && base.unit != null && base.unit.isVisible)
			{
				base.unit.surface.UseOutLine();
			}
		}

		private void TryRemoveSelf()
		{
			if (this.data.body_destroy && base.unit != null && !base.unit.isBuilding)
			{
				base.unit.ForceNormalized(1f);
				if (!base.unit.isLive)
				{
					base.unit.RemoveSelf(0f);
				}
			}
		}

		protected virtual void UpdatePosition()
		{
			if (base.transform == null)
			{
				return;
			}
			switch (this.data.config.effect_pos_type)
			{
			case 1:
			case 5:
			{
				if (base.unit.mTransform == null)
				{
					return;
				}
				Transform transform = null;
				Vector3 zero = Vector3.zero;
				int effect_anchor = this.data.config.effect_anchor;
				base.unit.GetBone(effect_anchor, out transform, out zero);
				Vector3 point = new Vector3(this.data.offset_x, this.data.offset_y, this.data.offset_z);
				base.transform.position = transform.position + zero + base.unit.mTransform.rotation * point;
				base.transform.rotation = Quaternion.Euler(((!this.data.isUseCasterRot) ? Vector3.zero : base.unit.mTransform.localEulerAngles) + new Vector3(this.data.offset_rx, this.data.offset_ry, this.data.offset_rz));
				base.transform.localScale = Vector3.one;
				break;
			}
			case 2:
			{
				Transform parent = null;
				Vector3 zero2 = Vector3.zero;
				int effect_anchor2 = this.data.config.effect_anchor;
				base.unit.GetBone(effect_anchor2, out parent, out zero2);
				base.transform.parent = parent;
				base.transform.localPosition = new Vector3(this.data.offset_x + zero2.x, this.data.offset_y + zero2.y, this.data.offset_z + zero2.z);
				base.transform.localRotation = Quaternion.Euler(new Vector3(this.data.offset_rx, this.data.offset_ry, this.data.offset_rz));
				base.transform.localScale = Vector3.one;
				break;
			}
			case 3:
			{
				Vector3 position = this.effectPosition + new Vector3(this.data.offset_x, this.data.offset_y, this.data.offset_z);
				base.transform.position = position;
				base.transform.rotation = Quaternion.Euler(this.effectRoation + new Vector3(this.data.offset_rx, this.data.offset_ry, this.data.offset_rz));
				base.transform.localScale = Vector3.one;
				break;
			}
			case 6:
			{
				if (base.unit.mTransform == null)
				{
					return;
				}
				Transform transform2 = null;
				Vector3 zero3 = Vector3.zero;
				int effect_anchor3 = this.data.config.effect_anchor;
				base.unit.GetBone(effect_anchor3, out transform2, out zero3);
				Vector3 a = new Vector3(this.data.offset_x, this.data.offset_y, this.data.offset_z);
				float d = Vector3.Distance(a, Vector3.zero);
				Vector3 a2 = this.effectPosition - base.unit.mTransform.position;
				a2.Normalize();
				base.transform.position = transform2.position + zero3 + a2 * d;
				base.transform.LookAt(this.effectPosition);
				base.transform.localScale = Vector3.one;
				break;
			}
			}
		}

		protected ResourceHandle SpawnEffect(string effectId, Transform root)
		{
			if (!StringUtils.CheckValid(effectId))
			{
				return null;
			}
			bool isUsePool = this.data.isUsePool;
			if (this.CastlerUnit != null)
			{
				this.skin = HeroSkins.GetRealHeroSkin((TeamType)this.CastlerUnit.teamType, this.CastlerUnit.model_id);
			}
			else
			{
				Units units = (!(base.unit.ParentUnit == null)) ? base.unit.ParentUnit : base.unit;
				this.skin = HeroSkins.GetRealHeroSkin((TeamType)units.teamType, units.model_id);
			}
			ResourceHandle resourceHandle;
			if (base.transform != null)
			{
				resourceHandle = MapManager.Instance.SpawnResourceHandle(effectId, base.transform.position, base.transform.rotation, this.skin);
			}
			else
			{
				resourceHandle = MapManager.Instance.SpawnResourceHandle(effectId, this.effectPosition, Quaternion.Euler(this.effectRoation), this.skin);
			}
			if (resourceHandle == null)
			{
				return null;
			}
			Transform raw = resourceHandle.Raw;
			raw.gameObject.SetActive(false);
			if (root != null)
			{
				raw.parent = root;
				raw.localPosition = Vector3.zero;
				raw.localRotation = Quaternion.Euler(Vector3.zero);
				raw.localScale = Vector3.one;
			}
			raw.gameObject.SetActive(true);
			this.mEffectPlayer = raw.gameObject.GetComponent<EffectPlayer>();
			if (this.mEffectPlayer == null)
			{
				this.mEffectPlayer = raw.gameObject.AddComponent<EffectPlayer>();
				this.mEffectPlayer.enabled = true;
			}
			return resourceHandle;
		}

		[DebuggerHidden]
		private IEnumerator FollowTarget_Coroutine(Units target, Vector3 offset)
		{
			PlayEffectAction.<FollowTarget_Coroutine>c__Iterator74 <FollowTarget_Coroutine>c__Iterator = new PlayEffectAction.<FollowTarget_Coroutine>c__Iterator74();
			<FollowTarget_Coroutine>c__Iterator.target = target;
			<FollowTarget_Coroutine>c__Iterator.offset = offset;
			<FollowTarget_Coroutine>c__Iterator.<$>target = target;
			<FollowTarget_Coroutine>c__Iterator.<$>offset = offset;
			<FollowTarget_Coroutine>c__Iterator.<>f__this = this;
			return <FollowTarget_Coroutine>c__Iterator;
		}

		public bool IsHaveParticleCloseTime()
		{
			return this.data.particleClose_time > 0f;
		}

		private bool CompareTag()
		{
			if (base.unit == null)
			{
				return false;
			}
			if (this.data == null)
			{
				ClientLogger.Error("特效没找到 Error performId = " + this.performId);
				return false;
			}
			return TagManager.CheckTag(base.unit, this.data.performTagType);
		}

		protected override void OnDestroy()
		{
			if (this._effectHandle != null && this._effectHandle != null && this._effectHandle.Raw != null)
			{
				ResourceHandle.SafeRelease(ref this._effectHandle);
			}
			base.OnDestroy();
		}

		[DebuggerHidden]
		private IEnumerator DelayDestroyCoroutine(float delayTime)
		{
			PlayEffectAction.<DelayDestroyCoroutine>c__Iterator75 <DelayDestroyCoroutine>c__Iterator = new PlayEffectAction.<DelayDestroyCoroutine>c__Iterator75();
			<DelayDestroyCoroutine>c__Iterator.delayTime = delayTime;
			<DelayDestroyCoroutine>c__Iterator.<$>delayTime = delayTime;
			<DelayDestroyCoroutine>c__Iterator.<>f__this = this;
			return <DelayDestroyCoroutine>c__Iterator;
		}

		public override void DoSpecialProcess()
		{
			this.EndPlayEffect();
		}

		private void EndPlayEffect()
		{
			if (base.gameObject == null)
			{
				return;
			}
			if (this.playtask != null)
			{
				base.mCoroutineManager.StopCoroutine(this.playtask);
			}
			if (base.gameObject != null)
			{
				AudioMgr.Stop(base.gameObject);
				this.PlayEndSound(base.gameObject);
			}
			float num = 0f;
			ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				float num2 = componentsInChildren[i].startDelay;
				if (componentsInChildren[i].emissionRate > 1.01f)
				{
					num2 += componentsInChildren[i].duration;
				}
				if (componentsInChildren[i].emissionRate <= 1.01f || componentsInChildren[i].maxParticles > 1)
				{
					num2 += componentsInChildren[i].startLifetime;
				}
				if (num2 > num)
				{
					num = num2;
				}
			}
			float num3 = 1f;
			float num4 = num - num3;
			if (num4 > 0f)
			{
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].Simulate(num4, false, true);
					if (componentsInChildren[j].IsAlive(false))
					{
						componentsInChildren[j].Play();
					}
				}
			}
			base.mCoroutineManager.StopAllCoroutine();
			base.mCoroutineManager.StartCoroutine(this.DelayDestroyCoroutine(num3 + 0.1f), true);
		}

		private void TryAddBornPowerObjSkillData()
		{
			if (base.unit != null && base.unit.skillManager != null)
			{
				base.unit.skillManager.TryAddBornPowerObjSkillData(this.skillId, this.actionId);
			}
		}

		public void Show(bool isShow)
		{
			ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (isShow)
				{
					componentsInChildren[i].Play();
				}
				else
				{
					componentsInChildren[i].Stop();
				}
			}
		}
	}
}
