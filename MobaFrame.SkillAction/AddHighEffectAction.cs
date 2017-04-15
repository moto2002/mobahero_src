using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class AddHighEffectAction : BaseHighEffAction
	{
		public new Vector3 skillPosition = Vector3.zero;

		protected override void OnPlay()
		{
			base.RecordStart();
			base.SendStart();
			if (!this.doAction())
			{
				this.Destroy();
			}
		}

		protected override void OnSendStart()
		{
			HighEffInfo highEffInfo = new HighEffInfo();
			if (this.targetUnits != null)
			{
				highEffInfo.unitIds = new List<short>();
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						highEffInfo.unitIds.Add((short)this.targetUnits[i].unique_id);
					}
				}
			}
			highEffInfo.casterUnitId = ((!(base.unit != null)) ? 0 : base.unit.unique_id);
			highEffInfo.highEffId = this.higheffId;
			highEffInfo.skillId = this.skillId;
			highEffInfo.skillPosition = MoveController.Vector3ToSVector3(this.skillPosition);
			PvpEvent.SendAddHighEffEvent(highEffInfo);
		}

		protected override void StartHighEff()
		{
			if (this.targetUnits != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null && this.targetUnits[i].highEffManager != null)
					{
						this.targetUnits[i].highEffManager.AddHighEffect(this.higheffId, this.skillId, base.unit, new Vector3?(this.skillPosition), base.IsC2P);
					}
				}
			}
		}

		protected override void StopHighEff()
		{
		}
	}
}
