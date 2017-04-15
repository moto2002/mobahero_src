using System;

namespace Assets.Scripts.Model
{
	internal class Model_setting : ModelBase<SettingModelData>
	{
		private SettingModelData data;

		public Model_setting()
		{
			base.Init(EModelType.Model_setting);
			this.data = new SettingModelData();
			base.Data = this.data;
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
