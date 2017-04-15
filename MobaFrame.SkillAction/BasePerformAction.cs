using Com.Game.Module;
using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public abstract class BasePerformAction : CompositeAction
	{
		public SkillDataKey skillKey;

		public string performId;

		protected int index;

		protected PerformData data;

		protected SkillData skillData;

		public Callback Callback_OnDestroy;

		public Callback<BaseAction, List<Units>> OnDamageCallback;

		public Callback<BaseAction> OnDamageEndCallback;

		protected new virtual bool useCollider
		{
			get
			{
				return this.skillData != null && this.skillData.config.hit_trigger_type != 0 && this.data.useCollider && this.IsMaster && base.IsC2P;
			}
		}

		protected bool IsUseColliderData
		{
			get
			{
				return this.data.useCollider;
			}
		}

		protected override void OnInit()
		{
			base.OnInit();
			if (GameManager.Instance == null)
			{
				return;
			}
			if (StringUtils.CheckValid(this.skillKey.SkillID))
			{
				this.skillData = GameManager.Instance.SkillData.GetData(this.skillKey);
			}
			this.data = Singleton<PerformDataManager>.Instance.GetVo(this.performId);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.OnDamageCallback = null;
			if (this.Callback_OnDestroy != null)
			{
				this.Callback_OnDestroy();
			}
		}

		protected virtual void OnDamage(BaseAction action, List<Units> targets)
		{
			if (this.OnDamageCallback != null)
			{
				this.OnDamageCallback(this, targets);
			}
		}

		protected virtual void OnDamageEnd(BaseAction action)
		{
			if (this.OnDamageEndCallback != null)
			{
				this.OnDamageEndCallback(this);
			}
		}

		protected override void OnActionEnd()
		{
			this.OnDamageEnd(this);
			base.OnActionEnd();
		}
	}
}
