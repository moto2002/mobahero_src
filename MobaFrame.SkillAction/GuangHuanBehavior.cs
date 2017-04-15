using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class GuangHuanBehavior : MobaMono
	{
		protected string skillId;

		protected Units parent;

		protected Units self;

		[SerializeField]
		protected bool isActive;

		protected HighEffectData data;

		public Callback OnGuangHuanEnd;

		private GameObject cacheGameObject;

		protected float curTime;

		[SerializeField]
		protected List<Units> active_targets;

		protected Dictionary<int, PlayEffectAction> mPlayEffectActions;

		protected CoroutineManager mCoroutineManager = new CoroutineManager();

		protected bool isDestroying;

		protected float removeMainUnitDelayTime;

		protected bool keep_effect
		{
			get
			{
				return this.data != null && this.data.param2 == 1f;
			}
		}

		protected bool destroy_flag
		{
			get
			{
				return this.data != null && this.data.param3 == 1f;
			}
		}

		protected float exist_time
		{
			get
			{
				return this.data.param1;
			}
		}

		protected float guangHuanType
		{
			get
			{
				return this.data.param4;
			}
		}

		protected float extraParam1
		{
			get
			{
				return this.data.param5;
			}
		}

		protected float extraParam2
		{
			get
			{
				return this.data.param6;
			}
		}

		public virtual void Init(string higheffId, string skillId, Units self)
		{
			this.skillId = skillId;
			this.self = self;
			this.parent = ((!(self is SkillUnit)) ? null : ((SkillUnit)self).ParentUnit);
			if (!StringUtils.CheckValid(higheffId))
			{
				this.DestroyBehaviour();
				return;
			}
			this.data = Singleton<HighEffectDataManager>.Instance.GetVo(higheffId);
			this.active_targets = new List<Units>();
			this.active_targets.Clear();
			this.mPlayEffectActions = new Dictionary<int, PlayEffectAction>();
			this.AssianGuangHuanShape();
			this.isActive = true;
			this.cacheGameObject = base.gameObject;
			this.removeMainUnitDelayTime = ((this.data == null) ? 0f : this.data.param7);
			if (this.removeMainUnitDelayTime < 0.001f)
			{
				this.removeMainUnitDelayTime = 0f;
			}
		}

		protected virtual void AssianGuangHuanShape()
		{
		}

		protected virtual void Update()
		{
			if (this.self == null)
			{
				return;
			}
			if (this.isActive && this.exist_time > 0f)
			{
				this.curTime += Time.deltaTime;
				if (this.curTime > this.exist_time)
				{
					this.isActive = false;
					this.DestroyBehaviour();
				}
			}
			if (!this.self.isLive)
			{
				this.isActive = false;
				this.DestroyBehaviour();
			}
		}

		private void DestroyBehaviour()
		{
			UnityEngine.Object.Destroy(this);
		}

		protected virtual void hitIn(GameObject go)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return;
			}
			if (!this.isActive)
			{
				return;
			}
			if (!TagManager.IsCharacterTarget(go))
			{
				return;
			}
			if (!TagManager.CheckTag(go, this.data.targetTag))
			{
				return;
			}
			if (!TeamManager.CheckTeam(this.self.gameObject, go, this.data.targetCamp, this.parent))
			{
				return;
			}
			Units component = go.GetComponent<Units>();
			if (component == null || !component.isLive || this.active_targets.Contains(component))
			{
				return;
			}
			if (this.data.maxNum != 0 && this.active_targets.Count > this.data.maxNum)
			{
				return;
			}
			this.AddEffects(component);
		}

		protected virtual void hitOut(GameObject go)
		{
			if (!this.isActive)
			{
				return;
			}
			if (!TagManager.IsCharacterTarget(go))
			{
				return;
			}
			if (!TagManager.CheckTag(go, this.data.targetTag))
			{
				return;
			}
			if (!TeamManager.CheckTeam(this.self.gameObject, go, this.data.targetCamp, this.parent))
			{
				return;
			}
			Units component = go.GetComponent<Units>();
			if (component == null || !this.active_targets.Contains(component))
			{
				return;
			}
			this.RemoveEffects(component, true);
		}

		protected void OnDestroy()
		{
			this.isActive = false;
			this.RemoveAllEffects();
		}

		protected void OnDisable()
		{
			this.mCoroutineManager.StopAllCoroutine();
			this.isActive = false;
			this.RemoveAllEffects();
		}

		public void AddEffects(Units target)
		{
			if (target == null || this.active_targets.Contains(target))
			{
				return;
			}
			if (!this.doWithItemType(target))
			{
				return;
			}
			this.active_targets.Add(target);
			this.AddDamage(target);
			this.AddPerform(target);
			this.AddAttachEffect(target);
			this.AddSelfAttachEffect();
			if (this.destroy_flag)
			{
				this.RemoveAllEffects();
				this.self.RemoveSelf(this.removeMainUnitDelayTime);
			}
		}

		[DebuggerHidden]
		protected IEnumerator Destroy_Coroutine()
		{
			GuangHuanBehavior.<Destroy_Coroutine>c__Iterator76 <Destroy_Coroutine>c__Iterator = new GuangHuanBehavior.<Destroy_Coroutine>c__Iterator76();
			<Destroy_Coroutine>c__Iterator.<>f__this = this;
			return <Destroy_Coroutine>c__Iterator;
		}

		public void RemoveEffects(Units target, bool removeFromList)
		{
			if (target != null && this.active_targets.Contains(target) && removeFromList)
			{
				this.active_targets.Remove(target);
			}
			if (this.keep_effect)
			{
				return;
			}
			this.RevertAttachEffect(target);
			this.RevertPerform(target);
			this.RevertDamage(target);
		}

		public void RemoveAllEffects()
		{
			if (this.data == null)
			{
				return;
			}
			bool flag = false;
			string str = string.Empty;
			if (this.data.higheffId == "HighEff_149")
			{
				flag = true;
				str += "XiaoHei Guanghuan Remove!!\n";
			}
			if (this.active_targets != null)
			{
				for (int i = 0; i < this.active_targets.Count; i++)
				{
					this.RemoveEffects(this.active_targets[i], false);
					if (flag)
					{
						str = str + "Remove Units : " + this.active_targets[i].gameObject.name + "\n";
					}
				}
				this.active_targets.Clear();
			}
			if (this.OnGuangHuanEnd != null)
			{
				this.OnGuangHuanEnd();
			}
			if (this.cacheGameObject != null)
			{
				this.DestroyBehaviour();
			}
			if (flag)
			{
			}
		}

		protected virtual bool doWithItemType(Units target)
		{
			return !(target == null);
		}

		protected void AddDamage(Units target)
		{
			if (!this.self.IsMaster)
			{
				return;
			}
			if (this.data.damage_ids != null && target.dataChange != null)
			{
				target.dataChange.doSkillWoundAction(this.data.damage_ids, this.self, true, new float[0]);
			}
		}

		protected void AddAttachEffect(Units target)
		{
			if (!this.self.IsMaster)
			{
				return;
			}
			if (this.data.attachHighEffs != null)
			{
				for (int i = 0; i < this.data.attachHighEffs.Length; i++)
				{
					string text = this.data.attachHighEffs[i];
					if (StringUtils.CheckValid(text) && target != null && target.isLive)
					{
						ActionManager.AddHighEffect(text, this.skillId, target, this.self, null, true);
					}
				}
			}
			if (this.data.attachBuffs != null)
			{
				for (int j = 0; j < this.data.attachBuffs.Length; j++)
				{
					string text2 = this.data.attachBuffs[j];
					if (StringUtils.CheckValid(text2) && target != null && target.isLive)
					{
						ActionManager.AddBuff(text2, target, this.self, true, string.Empty);
					}
				}
			}
		}

		protected void AddPerform(Units target)
		{
			if (StringUtils.CheckValid(this.data.config.perform_id))
			{
				string[] stringValue = StringUtils.GetStringValue(this.data.config.perform_id, ',');
				if (stringValue != null)
				{
					for (int i = 0; i < stringValue.Length; i++)
					{
						if (StringUtils.CheckValid(stringValue[i]))
						{
							PlayEffectAction playEffectAction = ActionManager.PlayEffect(stringValue[i], target, null, null, true, string.Empty, null);
							if (playEffectAction != null && !this.mPlayEffectActions.ContainsKey(target.unique_id))
							{
								this.mPlayEffectActions.Add(target.unique_id, playEffectAction);
							}
						}
					}
				}
			}
		}

		protected void AddSelfAttachEffect()
		{
			if (!this.self.IsMaster)
			{
				return;
			}
			if (this.data.attachSelfHighEffs != null)
			{
				for (int i = 0; i < this.data.attachSelfHighEffs.Length; i++)
				{
					string text = this.data.attachSelfHighEffs[i];
					if (StringUtils.CheckValid(text) && this.self != null && this.self.isLive)
					{
						ActionManager.AddHighEffect(text, this.skillId, this.self, this.self, null, true);
					}
				}
			}
		}

		protected void RevertDamage(Units target)
		{
			if (!this.self.IsMaster)
			{
				return;
			}
			if (this.data.damage_ids != null && target.dataChange != null)
			{
				target.dataChange.doSkillWoundAction(this.data.damage_ids, this.self, true, new float[0]);
			}
		}

		protected void RevertAttachEffect(Units target)
		{
			if (!this.self.IsMaster)
			{
				return;
			}
			if (this.data == null)
			{
				return;
			}
			if (this.data.attachHighEffs != null)
			{
				for (int i = 0; i < this.data.attachHighEffs.Length; i++)
				{
					ActionManager.RemoveHighEffect(this.data.attachHighEffs[i], target, true);
				}
			}
			if (this.data.attachBuffs != null)
			{
				for (int j = 0; j < this.data.attachBuffs.Length; j++)
				{
					string buff_id = this.data.attachBuffs[j];
					ActionManager.RemoveBuff(buff_id, target, this.self, -1, true);
				}
			}
		}

		protected void RevertSelfAttachEffect()
		{
			if (!this.self.IsMaster)
			{
				return;
			}
			if (this.data == null)
			{
				return;
			}
			if (this.data.attachSelfHighEffs != null)
			{
				for (int i = 0; i < this.data.attachSelfHighEffs.Length; i++)
				{
					ActionManager.RemoveHighEffect(this.data.attachSelfHighEffs[i], this.self, true);
				}
			}
		}

		protected void RevertPerform(Units target)
		{
			int unique_id = target.unique_id;
			if (this.mPlayEffectActions != null && this.mPlayEffectActions.ContainsKey(unique_id))
			{
				if (this.mPlayEffectActions[unique_id] != null)
				{
					this.mPlayEffectActions[unique_id].Destroy();
				}
				this.mPlayEffectActions.Remove(unique_id);
			}
		}
	}
}
