using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class AddGoldAction : BaseHighEffAction
	{
		private Task updateGoldTask;

		protected override void OnDestroy()
		{
			if (base.mCoroutineManager != null && this.updateGoldTask != null)
			{
				base.mCoroutineManager.StopCoroutine(this.updateGoldTask);
			}
			base.OnDestroy();
		}

		protected override void StartHighEff()
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return;
			}
			if (UtilManager.Instance == null)
			{
				return;
			}
			if ((int)this.data.param1 == 0)
			{
				this.AddGold(true);
			}
			else if ((int)this.data.param1 == 1)
			{
				this.updateGoldTask = base.mCoroutineManager.StartCoroutine(this.UpdateGold(), true);
			}
		}

		[DebuggerHidden]
		private IEnumerator UpdateGold()
		{
			AddGoldAction.<UpdateGold>c__Iterator78 <UpdateGold>c__Iterator = new AddGoldAction.<UpdateGold>c__Iterator78();
			<UpdateGold>c__Iterator.<>f__this = this;
			return <UpdateGold>c__Iterator;
		}

		private void AddGold(bool isJumpFont = false)
		{
			if (this.targetUnits == null)
			{
				return;
			}
			for (int i = 0; i < this.targetUnits.Count; i++)
			{
				UtilManager.Instance.ChangeGoldById(this.targetUnits[i].unique_id, (int)this.data.param2);
				if (isJumpFont && this.targetUnits[i].isVisible)
				{
					this.targetUnits[i].JumpFont("+" + this.data.param2 + "g", HUDText.goldColor);
				}
			}
		}
	}
}
