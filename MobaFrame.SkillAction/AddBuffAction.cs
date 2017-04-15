using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;

namespace MobaFrame.SkillAction
{
	public class AddBuffAction : CompositeAction
	{
		public string buffId;

		public Units targetUnit;

		public string skillId;

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
			PvpEvent.SendAddBuffEvent(new BuffInfo
			{
				unitId = this.targetUnit.unique_id,
				casterUnitId = (!(base.unit != null)) ? 0 : base.unit.unique_id,
				buffId = this.buffId
			});
		}

		protected override void OnRecordStart()
		{
		}

		protected override bool doAction()
		{
			BuffData vo = Singleton<BuffDataManager>.Instance.GetVo(this.buffId);
			if (vo == null)
			{
				return false;
			}
			if (base.IsC2P)
			{
				if (!MathUtils.Rand(vo.config.probability))
				{
					return false;
				}
				this.targetUnit.buffManager.AddBuff(this.buffId, base.unit);
			}
			else
			{
				this.targetUnit.buffManager.AddBuff(this.buffId, base.unit);
			}
			return true;
		}
	}
}
