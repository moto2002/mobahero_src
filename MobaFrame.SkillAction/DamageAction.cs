using Com.Game.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class DamageAction : BaseAction
	{
		public SkillDataKey skillKey;

		public string performId;

		public List<Units> skillTargets;

		protected SkillData skillData;

		protected PerformData performData;

		protected ColliderChecker mColliderChecker;

		private bool isCheckSingleTarget;

		private bool isUseCollider;

		private List<GameObject> hitColliders = new List<GameObject>();

		private List<Units> hitTargets = new List<Units>();

		private ColliderDamageType damageType;

		private Task _hitCheckTaskInst;

		public Callback<BaseAction, List<Units>> OnDamageCallback;

		public ColliderChecker ColliderChecker
		{
			get
			{
				return this.mColliderChecker;
			}
			set
			{
				this.mColliderChecker = value;
			}
		}

		protected override void OnInit()
		{
			base.OnInit();
			this.skillData = GameManager.Instance.SkillData.GetData(this.skillKey);
			if (StringUtils.CheckValid(this.performId))
			{
				this.performData = Singleton<PerformDataManager>.Instance.GetVo(this.performId);
			}
		}

		protected override bool doAction()
		{
			return false;
		}

		protected override void OnStop()
		{
			base.OnStop();
			this.isCheckSingleTarget = false;
			this.isUseCollider = false;
			if (this.mColliderChecker != null)
			{
				this.mColliderChecker.isActiveCollider = false;
				this.mColliderChecker.OnColliderEnterCallback = null;
				this.mColliderChecker.OnColliderExitCallback = null;
			}
			this.hitTargets.Clear();
			this.hitColliders.Clear();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.mColliderChecker = null;
			this.OnDamageCallback = null;
		}

		private void Destroy(float delayTime)
		{
			base.mCoroutineManager.StartCoroutine(this.Destroy_Coroutine(delayTime), true);
		}

		[DebuggerHidden]
		private IEnumerator Destroy_Coroutine(float delayTime)
		{
			DamageAction.<Destroy_Coroutine>c__Iterator6D <Destroy_Coroutine>c__Iterator6D = new DamageAction.<Destroy_Coroutine>c__Iterator6D();
			<Destroy_Coroutine>c__Iterator6D.delayTime = delayTime;
			<Destroy_Coroutine>c__Iterator6D.<$>delayTime = delayTime;
			<Destroy_Coroutine>c__Iterator6D.<>f__this = this;
			return <Destroy_Coroutine>c__Iterator6D;
		}

		[DebuggerHidden]
		private IEnumerator HitCheck()
		{
			DamageAction.<HitCheck>c__Iterator6E <HitCheck>c__Iterator6E = new DamageAction.<HitCheck>c__Iterator6E();
			<HitCheck>c__Iterator6E.<>f__this = this;
			return <HitCheck>c__Iterator6E;
		}

		public List<Units> GetTargets()
		{
			List<Units> list = new List<Units>();
			for (int i = 0; i < this.hitColliders.Count; i++)
			{
				if (this.hitColliders[i] != null)
				{
					if (list.Count >= this.skillData.config.max_num)
					{
						break;
					}
					if (UnitFeature.CheckTarget(base.unit.gameObject, this.hitColliders[i].gameObject, this.skillData.targetCamp, this.skillData.targetTag))
					{
						Units component = this.hitColliders[i].GetComponent<Units>();
						if (!(component == null) && !list.Contains(component) && component.isLive)
						{
							if (component != null && component.isLive)
							{
								list.Add(component);
							}
						}
					}
				}
			}
			return list;
		}

		private void doDamage()
		{
			if (this.skillData.targetCamp == SkillTargetCamp.Self)
			{
				this.hitTargets.Clear();
				this.hitTargets.Add(base.unit);
				this.OnDamage(this, this.hitTargets);
				return;
			}
			if (this.skillData.targetCamp != SkillTargetCamp.None)
			{
				if (this.skillData.config.hit_trigger_type == 2)
				{
					for (int i = 0; i < this.hitTargets.Count; i++)
					{
						if (this.hitTargets[i] != null && (this.hitTargets[i] == null || !this.hitTargets[i].isLive))
						{
							this.hitTargets.RemoveAt(i);
							i--;
						}
					}
					this.OnDamage(this, this.hitTargets);
				}
				else if (this.skillData.config.hit_trigger_type == 1 && this.skillTargets != null)
				{
					this.OnDamage(this, this.skillTargets);
				}
			}
		}

		protected void OnDamage(BaseAction action, List<Units> targets)
		{
			if (this.OnDamageCallback != null)
			{
				this.OnDamageCallback(action, targets);
			}
		}

		private void OnTriggerEnter(GameObject colliderObject)
		{
			if (!this.isUseCollider)
			{
				return;
			}
			if (this.isCheckSingleTarget)
			{
				if (this.skillTargets[0] != null && colliderObject == this.skillTargets[0].gameObject)
				{
					if (this.skillTargets[0].isLive)
					{
						this.hitTargets.Clear();
						this.hitTargets.Add(this.skillTargets[0]);
						this.OnDamage(this, this.hitTargets);
					}
					this._hitCheckTaskInst = null;
					this.Destroy();
				}
			}
			else
			{
				ColliderDamageType colliderDamageType = this.damageType;
				switch (colliderDamageType + 5)
				{
				case ColliderDamageType.Normal:
				case (ColliderDamageType)1:
				{
					if (this.hitColliders.Contains(colliderObject))
					{
						return;
					}
					if (!UnitFeature.CheckTarget(base.unit.gameObject, colliderObject, this.skillData.targetCamp, this.skillData.targetTag))
					{
						return;
					}
					Units component = colliderObject.GetComponent<Units>();
					if (component == null || !component.isLive)
					{
						return;
					}
					this.hitTargets.Add(component);
					this.hitColliders.Add(colliderObject);
					this.OnDamage(this, new List<Units>
					{
						component
					});
					return;
				}
				case (ColliderDamageType)2:
					return;
				case (ColliderDamageType)3:
				{
					if (this.hitTargets.Count >= this.skillData.config.max_num)
					{
						return;
					}
					if (!UnitFeature.CheckTarget(base.unit.gameObject, colliderObject, this.skillData.targetCamp, this.skillData.targetTag))
					{
						return;
					}
					Units component2 = colliderObject.GetComponent<Units>();
					if (component2 == null || !component2.isLive)
					{
						return;
					}
					if (this.hitTargets.Contains(component2))
					{
						return;
					}
					this.hitTargets.Add(component2);
					this.OnDamage(this, new List<Units>
					{
						component2
					});
					break;
				}
				case (ColliderDamageType)4:
				case (ColliderDamageType)5:
				{
					if (this.hitTargets.Count >= this.skillData.config.max_num)
					{
						return;
					}
					if (!UnitFeature.CheckTarget(base.unit.gameObject, colliderObject, this.skillData.targetCamp, this.skillData.targetTag))
					{
						return;
					}
					Units component3 = colliderObject.GetComponent<Units>();
					if (component3 == null || !component3.isLive)
					{
						return;
					}
					if (!this.hitTargets.Contains(component3))
					{
						this.hitTargets.Add(component3);
						this.hitColliders.Add(colliderObject);
					}
					break;
				}
				}
			}
		}

		private void OnTriggerExit(GameObject gameObject)
		{
			if (!this.isUseCollider)
			{
				return;
			}
			ColliderDamageType colliderDamageType = this.damageType;
			if (colliderDamageType != ColliderDamageType.ContinuesCollider)
			{
				if (this.hitColliders.Contains(gameObject))
				{
					Units component = gameObject.GetComponent<Units>();
					if (component != null)
					{
						this.hitTargets.Remove(component);
					}
					this.hitColliders.Remove(gameObject);
				}
			}
			else if (this.hitColliders.Contains(gameObject))
			{
				Units component2 = gameObject.GetComponent<Units>();
				this.OnDamage(this, new List<Units>
				{
					component2
				});
			}
		}

		public override void DoSpecialProcess()
		{
			this.TryStopHitCheckAndDoDamage();
		}

		private void TryStopHitCheckAndDoDamage()
		{
			if (!this.IsHaveBornPowerObj())
			{
				return;
			}
			if (this._hitCheckTaskInst != null)
			{
				base.mCoroutineManager.StopCoroutine(this._hitCheckTaskInst);
				this._hitCheckTaskInst = null;
				this.doDamage();
				this.Destroy(0.3f);
			}
		}

		private bool IsHaveBornPowerObj()
		{
			return this.skillData.config.skill_logic_type == 17;
		}

		private void TryShowBornPowerObjHint()
		{
			if (!this.IsHaveBornPowerObj())
			{
				return;
			}
			if (base.unit != null && base.unit.skillManager != null && base.unit.isPlayer)
			{
				base.unit.skillManager.TryShowBornPowerObjHint(this.skillData.config.skill_id, base.unit.isPlayer);
			}
		}
	}
}
