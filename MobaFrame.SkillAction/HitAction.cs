using Com.Game.Utils;
using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class HitAction : BasePerformAction
	{
		public Units casterUnit;

		protected override void OnRecordStart()
		{
		}

		protected override bool doAction()
		{
			if (this.casterUnit == null)
			{
				return false;
			}
			if (this.data == null)
			{
				ClientLogger.Error("Ч��û�ҵ�:" + this.performId);
				return false;
			}
			if (base.unit == null)
			{
				return false;
			}
			if ((int)this.data.effectParam1 == 1)
			{
				if (this.casterUnit != null)
				{
					List<Units> list = new List<Units>();
					list.Add(this.casterUnit);
					this.AddAction(ActionManager.Link(this.skillKey, this.performId, base.unit, list, null, this.casterUnit));
				}
			}
			else if ((int)this.data.effectParam1 == 0)
			{
				if (this.data.effectParam2 == 0f)
				{
					this.AddAction(ActionManager.PlayEffect(this.performId, base.unit, null, null, true, string.Empty, this.casterUnit));
				}
				else
				{
					this.AddAction(ActionManager.PlayLookAtEffect(this.performId, base.unit, this.casterUnit.trans.forward, true, this.casterUnit));
				}
			}
			else if ((int)this.data.effectParam1 == 2)
			{
				this.AddAction(ActionManager.AbsorbMissile(this.skillKey, this.performId, base.unit, this.casterUnit));
			}
			return true;
		}
	}
}
