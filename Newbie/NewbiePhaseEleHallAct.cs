using System;

namespace Newbie
{
	public class NewbiePhaseEleHallAct : NewbiePhaseBase
	{
		private NewbieStepEleHallActOpenAchievement _stepOpenAchievement;

		private NewbieStepEleHallActAchieveAward _stepAchieveAward;

		private NewbieStepEleHallActAchieveAwardEnd _stepAchieveAwardEnd;

		private NewbieStepEleHallActAchieveBack _stepAchieveBack;

		private NewbieStepEleHallActOpenDaily _stepOpenDaily;

		private NewbieStepEleHallActDailyAward _stepDailyAward;

		private NewbieStepEleHallActDailyAwardEnd _stepDailyAwardEnd;

		private NewbieStepEleHallActDoDailyHint _stepDoDailyHint;

		private NewbieStepEleHallActDailyBack _stepDailyBack;

		private NewbieStepEleHallActMagicBottle _stepMagicBottle;

		private NewbieStepEleHallActMagicBottleIntro _stepMagicBottleIntro;

		private NewbieStepEleHallActMagicBottleLvUp _stepMagicBottleLvUp;

		private NewbieStepEleHallActMagicBottleLvThree _stepMagicBottleLvThree;

		private NewbieStepEleHallActMagicBottleTale _stepMagicBottleTale;

		private NewbieStepEleHallActMagicBottleTaleEnd _stepMagicBottleTaleEnd;

		private NewbieStepEleHallActMagicBottleAwd _stepMagicBottleAwd;

		private NewbieStepEleHallActMagicTaleIntro _stepMagicTaleIntro;

		private NewbieStepEleHallActMagicClassicIntro _stepMagicClassicIntro;

		private NewbieStepEleHallActMagicBack _stepMagicBack;

		private NewbieStepEleHallActOpenActivity _stepOpenActivity;

		private NewbieStepEleHallActNewbieActivity _stepNewbieActivity;

		private NewbieStepEleHallActNewbieActAwd _stepNewbieActAwd;

		private NewbieStepEleHallActNewbieActAwdEnd _stepNewbieActAwdEnd;

		private NewbieStepEleHallActOpenLoginAwd _stepOpenLoginAwd;

		private NewbieStepEleHallActGetLoginAwd _stepGetLoginAwd;

		private NewbieStepEleHallActGetLoginAwdEnd _stepGetLoginAwdEnd;

		private NewbieStepEleHallActCloseActivity _stepCloseActivity;

		private NewbieStepEleHallActPlay _stepPlay;

		private NewbieStepEleHallActEnd _stepEnd;

		public NewbiePhaseEleHallAct()
		{
			this._phaseType = ENewbiePhaseType.EleHallAct;
			this.InitSteps();
		}

		public override void InitSteps()
		{
			this._stepOpenAchievement = new NewbieStepEleHallActOpenAchievement();
			this._stepAchieveAward = new NewbieStepEleHallActAchieveAward();
			this._stepAchieveAwardEnd = new NewbieStepEleHallActAchieveAwardEnd();
			this._stepAchieveBack = new NewbieStepEleHallActAchieveBack();
			this._stepOpenDaily = new NewbieStepEleHallActOpenDaily();
			this._stepDailyAward = new NewbieStepEleHallActDailyAward();
			this._stepDailyAwardEnd = new NewbieStepEleHallActDailyAwardEnd();
			this._stepDoDailyHint = new NewbieStepEleHallActDoDailyHint();
			this._stepDailyBack = new NewbieStepEleHallActDailyBack();
			this._stepMagicBottle = new NewbieStepEleHallActMagicBottle();
			this._stepMagicBottleIntro = new NewbieStepEleHallActMagicBottleIntro();
			this._stepMagicBottleLvUp = new NewbieStepEleHallActMagicBottleLvUp();
			this._stepMagicBottleLvThree = new NewbieStepEleHallActMagicBottleLvThree();
			this._stepMagicBottleTale = new NewbieStepEleHallActMagicBottleTale();
			this._stepMagicBottleTaleEnd = new NewbieStepEleHallActMagicBottleTaleEnd();
			this._stepMagicBottleAwd = new NewbieStepEleHallActMagicBottleAwd();
			this._stepMagicTaleIntro = new NewbieStepEleHallActMagicTaleIntro();
			this._stepMagicClassicIntro = new NewbieStepEleHallActMagicClassicIntro();
			this._stepMagicBack = new NewbieStepEleHallActMagicBack();
			this._stepOpenActivity = new NewbieStepEleHallActOpenActivity();
			this._stepNewbieActivity = new NewbieStepEleHallActNewbieActivity();
			this._stepNewbieActAwd = new NewbieStepEleHallActNewbieActAwd();
			this._stepNewbieActAwdEnd = new NewbieStepEleHallActNewbieActAwdEnd();
			this._stepOpenLoginAwd = new NewbieStepEleHallActOpenLoginAwd();
			this._stepGetLoginAwd = new NewbieStepEleHallActGetLoginAwd();
			this._stepGetLoginAwdEnd = new NewbieStepEleHallActGetLoginAwdEnd();
			this._stepCloseActivity = new NewbieStepEleHallActCloseActivity();
			this._stepPlay = new NewbieStepEleHallActPlay();
			this._stepEnd = new NewbieStepEleHallActEnd();
			this._stepDoDailyHint.BindNextStep(this._stepDailyBack);
			this._stepMagicBottleIntro.BindNextStep(this._stepMagicBottleLvUp);
			this._stepMagicBottleAwd.BindNextStep(this._stepMagicTaleIntro);
			this._stepMagicTaleIntro.BindNextStep(this._stepMagicClassicIntro);
			this._stepMagicClassicIntro.BindNextStep(this._stepMagicBack);
			this._allSteps.Add(this._stepOpenAchievement);
			this._allSteps.Add(this._stepAchieveAward);
			this._allSteps.Add(this._stepAchieveAwardEnd);
			this._allSteps.Add(this._stepAchieveBack);
			this._allSteps.Add(this._stepOpenDaily);
			this._allSteps.Add(this._stepDailyAward);
			this._allSteps.Add(this._stepDailyAwardEnd);
			this._allSteps.Add(this._stepDoDailyHint);
			this._allSteps.Add(this._stepDailyBack);
			this._allSteps.Add(this._stepMagicBottle);
			this._allSteps.Add(this._stepMagicBottleIntro);
			this._allSteps.Add(this._stepMagicBottleLvUp);
			this._allSteps.Add(this._stepMagicBottleLvThree);
			this._allSteps.Add(this._stepMagicBottleTale);
			this._allSteps.Add(this._stepMagicBottleTaleEnd);
			this._allSteps.Add(this._stepMagicBottleAwd);
			this._allSteps.Add(this._stepMagicTaleIntro);
			this._allSteps.Add(this._stepMagicClassicIntro);
			this._allSteps.Add(this._stepMagicBack);
			this._allSteps.Add(this._stepOpenActivity);
			this._allSteps.Add(this._stepNewbieActivity);
			this._allSteps.Add(this._stepNewbieActAwd);
			this._allSteps.Add(this._stepNewbieActAwdEnd);
			this._allSteps.Add(this._stepOpenLoginAwd);
			this._allSteps.Add(this._stepGetLoginAwd);
			this._allSteps.Add(this._stepGetLoginAwdEnd);
			this._allSteps.Add(this._stepCloseActivity);
			this._allSteps.Add(this._stepPlay);
			this._allSteps.Add(this._stepEnd);
		}

		public override void EnterPhase()
		{
			this.InitPhaseResources();
			this._curStep = this._stepOpenAchievement;
			this._curStep.HandleAction();
		}

		private void InitPhaseResources()
		{
			NewbieManager.Instance.InitEleHallActResource();
			this.LoadSoundBankEleHallAct();
		}

		private void LoadSoundBankEleHallAct()
		{
			AudioMgr.LoadBnk("Guide_D.bnk");
		}
	}
}
