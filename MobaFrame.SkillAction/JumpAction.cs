using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class JumpAction : BaseHighEffAction
	{
		public new Vector3? skillPosition;

		private JiTui JiTuiComponent;

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.JiTuiComponent != null)
			{
				UnityEngine.Object.Destroy(this.JiTuiComponent);
			}
			this.JiTuiComponent = null;
		}

		protected override void doStartHighEffect_Special()
		{
			base.unit.InterruptAction(SkillInterruptType.Passive);
			Vector3? vector = this.skillPosition;
			Vector3 vector2 = vector.Value;
			Vector3 position = base.unit.mTransform.position;
			if (Vector3.Distance(position, vector2) < 1f)
			{
				vector2 += base.unit.mTransform.forward;
			}
			base.unit.TurnToTarget(new Vector3?(vector2), true, false, 0f);
			this.JiTuiComponent = base.unit.gameObject.GetComponent<JiTui>();
			if (this.JiTuiComponent == null)
			{
				this.JiTuiComponent = base.unit.gameObject.AddComponent<JiTui>();
			}
			this.JiTuiComponent.Init(base.unit, vector2, this.data.param1, false, 70f);
			this.JiTuiComponent.OnDestroyCallback = new Callback(this.JITuiEnd);
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				base.unit.moveController.PauseMoveInPvp = true;
			}
			base.mCoroutineManager.StartCoroutine(this.JiTuiComponent.Repel_Coroutine(), true);
		}

		private void JITuiEnd()
		{
			if (Singleton<PvpManager>.Instance.IsInPvp && base.unit.isPlayer)
			{
				PvpEvent.SendFlashTo(base.unit.unique_id, base.unit.trans.position);
			}
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitUnderGroud, base.unit, null, null);
			this.Destroy();
		}
	}
}
