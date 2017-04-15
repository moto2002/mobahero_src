using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;

namespace MobaFrame.SkillAction
{
	public class StartBuffAction : CompositeAction
	{
		public string buffId;

		private BuffData data;

		private bool isPlayEffect;

		protected override void OnInit()
		{
			base.OnInit();
			this.data = Singleton<BuffDataManager>.Instance.GetVo(this.buffId);
		}

		protected override void OnDestroy()
		{
			if (base.unit != null)
			{
			}
			this.isPlayEffect = false;
			base.OnDestroy();
		}

		protected override void OnSendStart()
		{
			PvpEvent.SendDoBuffEvent(new BuffInfo
			{
				unitId = base.unit.unique_id,
				buffId = this.buffId
			});
		}

		protected override void OnRecordStart()
		{
		}

		protected override bool doAction()
		{
			if (base.unit != null)
			{
			}
			base.unit.buffManager.DoBuff(this.buffId, this);
			return true;
		}
	}
}
