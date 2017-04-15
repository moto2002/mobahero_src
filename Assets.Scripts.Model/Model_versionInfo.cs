using System;

namespace Assets.Scripts.Model
{
	internal class Model_versionInfo : ModelBase<ModelVersionInfo>
	{
		public Model_versionInfo()
		{
			base.Init(EModelType.Model_versionInfo);
			base.Data = new ModelVersionInfo();
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

		private void OnMsg_VersionInfo(MobaMessage msg)
		{
			ModelVersionInfo modelVersionInfo = base.Data as ModelVersionInfo;
			modelVersionInfo.versionStr = (msg.Param as string);
		}
	}
}
