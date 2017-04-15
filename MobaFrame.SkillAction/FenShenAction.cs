using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class FenShenAction : BaseSkillAction
	{
		protected override bool doAction()
		{
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
			return true;
		}

		[DebuggerHidden]
		protected IEnumerator Coroutine()
		{
			FenShenAction.<Coroutine>c__Iterator63 <Coroutine>c__Iterator = new FenShenAction.<Coroutine>c__Iterator63();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}

		private void SpawnUnits()
		{
			if (!Singleton<PvpManager>.Instance.IsInPvp)
			{
				int num = 2;
				for (int i = 0; i < num; i++)
				{
					Hero hero = MapManager.Instance.CreateFenShen(base.unit, 12f, 1);
					hero.trans.position = MathUtils.RadomCirclePoint(base.unit.trans.position, 1f);
					hero.effect_id = "2|Perform_jsdead_fenshen";
				}
				base.unit.trans.position = MathUtils.RadomCirclePoint(base.unit.trans.position, 1f);
				base.unit.surface.ClearTarget();
			}
		}
	}
}
