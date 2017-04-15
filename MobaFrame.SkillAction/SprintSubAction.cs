using Pathfinding;
using System;

namespace MobaFrame.SkillAction
{
	public class SprintSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.Sprint.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.ChangeLayer("ChongZhuang");
			this.targetUnit.Sprint.Add();
			TagMask tagMask = new TagMask(-1, -1);
			this.targetUnit.SetTagsChange(tagMask.tagsChange);
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.RevertLayer();
			this.targetUnit.Sprint.Remove();
			this.targetUnit.ResetTagsChange();
		}
	}
}
