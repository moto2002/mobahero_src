using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;

namespace MobaFrame.SkillAction
{
	public class RemoveHighEffectAction : BaseAction
	{
		public string higheffId;

		public HighEffectData data;

		protected override void OnInit()
		{
			base.OnInit();
			this.data = Singleton<HighEffectDataManager>.Instance.GetVo(this.higheffId);
		}

		protected override void OnSendStart()
		{
			PvpEvent.SendRemoveHightEvent(new RemoveHighEffInfo
			{
				unitId = base.unit.unique_id,
				highEffId = this.higheffId
			});
		}

		protected override bool doAction()
		{
			base.unit.highEffManager.RemoveHighEffect(this.higheffId);
			return true;
		}

		protected override void OnRecordStart()
		{
		}
	}
}
