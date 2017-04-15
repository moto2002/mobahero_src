using System;

namespace Assets.Scripts.Model
{
	internal class Model_Net : ModelBase<NetInfo>
	{
		public Model_Net()
		{
			base.Init(EModelType.Model_Net);
			base.Data = new NetInfo();
			base.Valid = true;
		}

		public override void RegisterMsgHandler()
		{
		}

		public override void UnRegisterMsgHandler()
		{
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
		}
	}
}
