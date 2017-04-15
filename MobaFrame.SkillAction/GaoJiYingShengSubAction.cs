using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class GaoJiYingShengSubAction : BaseStateAction
	{
		private bool isInvisible = true;

		private readonly int tagType = 2;

		private PlayEffectAction gthEffect;

		protected override bool IsInState
		{
			get
			{
				return false;
			}
		}

		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			GaoJiYingShengSubAction.<Coroutine>c__Iterator7D <Coroutine>c__Iterator7D = new GaoJiYingShengSubAction.<Coroutine>c__Iterator7D();
			<Coroutine>c__Iterator7D.<>f__this = this;
			return <Coroutine>c__Iterator7D;
		}

		[DebuggerHidden]
		private IEnumerator Update()
		{
			GaoJiYingShengSubAction.<Update>c__Iterator7E <Update>c__Iterator7E = new GaoJiYingShengSubAction.<Update>c__Iterator7E();
			<Update>c__Iterator7E.<>f__this = this;
			return <Update>c__Iterator7E;
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.SetLockCharaEffect(true);
			this.targetUnit.Invisible.Add();
			this.targetUnit.ChangeLayer("Invisible");
			base.mCoroutineManager.StartCoroutine(this.Update(), true);
			TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitAttack, null, new TriggerAction(this.RemoveSelf), this.targetUnit.unique_id);
			TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitConjure, null, new TriggerAction(this.RemoveSelf), this.targetUnit.unique_id);
		}

		protected override void RevertState()
		{
			base.RevertState();
			base.StopActions();
			this.targetUnit.RevertLayer();
			this.targetUnit.SetLockCharaEffect(false);
			if (this.isInvisible)
			{
				this.targetUnit.Invisible.Remove();
			}
		}

		private void RemoveSelf()
		{
			this.Destroy();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.gthEffect != null)
			{
				this.gthEffect.Destroy();
			}
		}

		private void UpdateInvisibleByDistance()
		{
			bool flag = true;
			IList<Units> allHeroes = MapManager.Instance.GetAllHeroes();
			for (int i = 0; i < allHeroes.Count; i++)
			{
				Units units = allHeroes[i];
				if (units != null && units.isLive && units.TeamType != this.targetUnit.TeamType && Vector3.Distance(units.trans.position, this.targetUnit.trans.position) < 4f)
				{
					flag = false;
				}
			}
			if (this.isInvisible != flag)
			{
				this.isInvisible = flag;
				this.ShowGTH(!this.isInvisible);
			}
		}

		private void ShowGTH(bool isShow)
		{
			if (isShow)
			{
				if (this.gthEffect == null)
				{
					if (this.performIds.Length > 0)
					{
						this.gthEffect = ActionManager.PlayEffect(this.performIds[0], this.targetUnit, null, null, true, string.Empty, null);
					}
				}
				else
				{
					this.gthEffect.Show(true);
				}
			}
			else if (this.gthEffect != null)
			{
				this.gthEffect.Show(false);
			}
		}
	}
}
