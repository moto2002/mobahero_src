using Com.Game.Module;
using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class BackHomeAction : BaseHighEffAction
	{
		private Task backTask;

		private int vis;

		protected override void StartHighEff()
		{
			this.vis = base.unit.m_nVisibleState;
			this.backTask = base.mCoroutineManager.StartCoroutine(this.BackHomeCoroutine(), true);
		}

		protected override bool doAction()
		{
			base.AddActionToSkill(this);
			return base.doAction();
		}

		protected override void OnDestroy()
		{
			base.RemoveAction(this);
			base.OnDestroy();
		}

		[DebuggerHidden]
		protected IEnumerator BackHomeCoroutine()
		{
			BackHomeAction.<BackHomeCoroutine>c__Iterator79 <BackHomeCoroutine>c__Iterator = new BackHomeAction.<BackHomeCoroutine>c__Iterator79();
			<BackHomeCoroutine>c__Iterator.<>f__this = this;
			return <BackHomeCoroutine>c__Iterator;
		}

		protected override void StopHighEff()
		{
			base.mCoroutineManager.StopCoroutine(this.backTask);
			base.unit.surface.StopAlphaTween();
			base.mCoroutineManager.StopAllCoroutine();
			if (base.unit.m_nVisibleState == 0)
			{
				base.unit.SetAlpha(1f);
			}
			else if (base.unit.m_nVisibleState == 1)
			{
				base.unit.SetAlpha(0.5f);
			}
			base.unit.m_nVisibleState = -1;
			base.unit.surface.ShowParticle(true);
			base.unit.PlayAnim(AnimationType.BackHome, false, 0, true, false);
			if (base.unit.isPlayer)
			{
				Singleton<SkillView>.Instance.ShowGuideBar(false, 1f, "回城");
			}
		}
	}
}
