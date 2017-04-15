using Com.Game.Module;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class RepleSubAction : BaseStateAction
	{
		private float distance;

		private bool usePosition;

		private Vector3 targetPos = Vector3.zero;

		private float speed = 20f;

		private float angle = 70f;

		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.JiTui.IsInState;
			}
		}

		protected override void StartHighEff()
		{
			base.CreateNode(NodeType.SkillNode, this.higheffId);
			this.distance = this.data.param1;
			base.unit.transform.Rotate(0f, this.rotateY, 0f);
			this.targetPos = this.targetUnit.transform.position + base.unit.transform.forward * this.distance;
			this.targetPos.y = 0f;
			this.isActive = true;
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.JiTui.Add();
			base.EnableAction(this.targetUnit, false, 0f);
			this.targetUnit.ShowDebuffIcon(true, 116);
		}

		protected override void RevertState()
		{
			base.RevertState();
			if (base.gameObject != null)
			{
				base.gameObject.transform.rotation = Quaternion.identity;
			}
			this.targetUnit.transform.position = new Vector3(this.targetUnit.transform.position.x, Mathf.Clamp(this.targetPos.y, 0.1f, 1f), this.targetUnit.mTransform.position.z);
			base.EnableAction(this.targetUnit, true, 0f);
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitUnderGroud, this.targetUnit, null, null);
			this.targetUnit.JiTui.Remove();
			this.targetUnit.ShowDebuffIcon(false, 116);
		}

		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			RepleSubAction.<Coroutine>c__Iterator84 <Coroutine>c__Iterator = new RepleSubAction.<Coroutine>c__Iterator84();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}
	}
}
