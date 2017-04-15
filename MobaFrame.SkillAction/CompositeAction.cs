using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public abstract class CompositeAction : BaseAction
	{
		protected List<BaseAction> mActions = new List<BaseAction>();

		protected bool isActionsDestroyed
		{
			get
			{
				if (this.mActions.Count > 0)
				{
					bool flag = false;
					for (int i = 0; i < this.mActions.Count; i++)
					{
						if (this.mActions[i] != null && !this.mActions[i].IsDestroyed)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						return false;
					}
				}
				return true;
			}
		}

		public CompositeAction()
		{
		}

		protected override void OnStop()
		{
			this.StopActions();
			base.OnStop();
		}

		protected override void OnDestroy()
		{
			this.DestroyActions();
			base.OnDestroy();
		}

		public virtual void AddAction(BaseAction action)
		{
			if (action != null && !action.isDestroyed && !this.mActions.Contains(action))
			{
				this.mActions.Add(action);
				action.OnActionEndCallback = (Callback<BaseAction>)Delegate.Combine(action.OnActionEndCallback, new Callback<BaseAction>(this.OnSubActionEnd));
			}
		}

		protected void RemoveAction(BaseAction action)
		{
			if (action != null && !action.isDestroyed && this.mActions.Contains(action))
			{
				action.OnActionEndCallback = (Callback<BaseAction>)Delegate.Remove(action.OnActionEndCallback, new Callback<BaseAction>(this.OnSubActionEnd));
				this.mActions.Remove(action);
			}
		}

		protected void StopActions()
		{
			for (int i = 0; i < this.mActions.Count; i++)
			{
				if (this.mActions[i] != null)
				{
					this.mActions[i].Stop();
				}
			}
		}

		protected void DestroyActions()
		{
			for (int i = 0; i < this.mActions.Count; i++)
			{
				if (this.mActions[i] != null)
				{
					this.mActions[i].DestroyNode();
					if (!this.mActions[i].IsAutoDestroy)
					{
						this.mActions[i].Destroy();
					}
				}
			}
			this.mActions.Clear();
		}

		protected void AttachSubAction(BaseAction action)
		{
			if (base.gameObject == null)
			{
				return;
			}
			if (action == null || action.gameObject == null)
			{
				return;
			}
			action.transform.parent = base.transform;
			action.transform.localPosition = Vector3.zero;
			action.transform.localRotation = Quaternion.Euler(Vector3.zero);
			this.AddAction(action);
		}

		protected void OnSubActionEnd(BaseAction action)
		{
			if (this.isActionsDestroyed)
			{
				this.Destroy();
			}
		}

		public override bool GetActionById(int inActionId, out BaseAction outActionInst)
		{
			outActionInst = null;
			if (this.actionId == inActionId)
			{
				outActionInst = this;
				return true;
			}
			if (this.mActions == null)
			{
				return false;
			}
			if (this.mActions.Count <= 0)
			{
				return false;
			}
			for (int i = 0; i < this.mActions.Count; i++)
			{
				if (this.mActions[i] != null)
				{
					if (this.mActions[i].GetActionById(inActionId, out outActionInst))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
