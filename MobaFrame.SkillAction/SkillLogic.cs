using System;

namespace MobaFrame.SkillAction
{
	public class SkillLogic : BaseLogic<Units>
	{
		protected new Units owner;

		protected Task mActionTask;

		public new object data;

		public SkillLogic(Units m_owner)
		{
			this.owner = m_owner;
		}

		public SkillLogic(Units m_owner, object data)
		{
			this.owner = m_owner;
			this.data = data;
		}

		public override void OnInit()
		{
		}

		public override void OnAction()
		{
		}

		public override void OnInterrupt()
		{
		}

		public override void OnEnd()
		{
		}

		public override void Destroy()
		{
		}
	}
}
