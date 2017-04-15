using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;

namespace MobaFrame.SkillAction
{
	public class RemoveBuffAction : BaseAction
	{
		public Units targetUnit;

		public string buffId;

		public short reduce_layers = -1;

		protected override void OnRecordStart()
		{
		}

		protected override void OnSendStart()
		{
			PvpEvent.SendRemoveBuffEvent(new BuffInfo
			{
				unitId = this.targetUnit.unique_id,
				casterUnitId = (!(base.unit != null)) ? 0 : base.unit.unique_id,
				buffId = this.buffId,
				reduce_layers = this.reduce_layers
			});
		}

		protected override bool doAction()
		{
			this.targetUnit.buffManager.RemoveBuff(this.buffId, (int)this.reduce_layers);
			return true;
		}
	}
}
