using System;

namespace Newbie
{
	public class NewbiePhaseNormalCastSkill : NewbiePhaseBase
	{
		private NewbieStepNormCastCheckNormalCast _stepCheckNormalCast;

		private NewbieStepNormCastOpenSysSetting _stepOpenSysSetting;

		private NewbieStepNormCastSetNormalCast _stepSetNormalCast;

		private NewbieStepNormCastCloseSysSetting _stepCloseSysSetting;

		private NewbieStepNormCastCloseSysSettingEnd _stepCloseSysSettingEnd;

		private NewbieStepNormCastUseSkillFir _stepUseSkillFir;

		private NewbieStepNormCastClickGround _stepClickGround;

		private NewbieStepNormCastDoubleClick _stepDoubleClick;

		private NewbieStepNormCastEnd _stepEnd;

		public NewbiePhaseNormalCastSkill()
		{
			this._phaseType = ENewbiePhaseType.NormalCastSkill;
			this.InitSteps();
		}

		public override void InitSteps()
		{
			this._stepCheckNormalCast = new NewbieStepNormCastCheckNormalCast();
			this._stepOpenSysSetting = new NewbieStepNormCastOpenSysSetting();
			this._stepSetNormalCast = new NewbieStepNormCastSetNormalCast();
			this._stepCloseSysSetting = new NewbieStepNormCastCloseSysSetting();
			this._stepCloseSysSettingEnd = new NewbieStepNormCastCloseSysSettingEnd();
			this._stepUseSkillFir = new NewbieStepNormCastUseSkillFir();
			this._stepClickGround = new NewbieStepNormCastClickGround();
			this._stepDoubleClick = new NewbieStepNormCastDoubleClick();
			this._stepEnd = new NewbieStepNormCastEnd();
			this._stepDoubleClick.BindNextStep(this._stepEnd);
			this._allSteps.Add(this._stepCheckNormalCast);
			this._allSteps.Add(this._stepOpenSysSetting);
			this._allSteps.Add(this._stepSetNormalCast);
			this._allSteps.Add(this._stepCloseSysSetting);
			this._allSteps.Add(this._stepCloseSysSettingEnd);
			this._allSteps.Add(this._stepUseSkillFir);
			this._allSteps.Add(this._stepClickGround);
			this._allSteps.Add(this._stepDoubleClick);
			this._allSteps.Add(this._stepEnd);
		}

		public override void EnterPhase()
		{
			this.InitPhaseResources();
			this._curStep = this._stepCheckNormalCast;
			this._curStep.HandleAction();
		}

		private void InitPhaseResources()
		{
			NewbieManager.Instance.InitNormalCastSkillResource();
		}
	}
}
