using Com.Game.Module;
using System;

namespace Newbie
{
	public class NewbieStepNormCastUseSkillFir : NewbieStepBase
	{
		public NewbieStepNormCastUseSkillFir()
		{
			this._stepType = ENewbieStepType.NormCast_UseSkillFir;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideNormCastUseSkillFir();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowNormCastUseSkillFir();
		}
	}
}
