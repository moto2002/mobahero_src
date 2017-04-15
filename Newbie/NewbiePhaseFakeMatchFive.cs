using System;

namespace Newbie
{
	public class NewbiePhaseFakeMatchFive : NewbiePhaseBase
	{
		private NewbieStepFakeMatchFiveHintPlay _stepHintPlay;

		private NewbieStepFakeMatchFiveHintSingleMatch _stepHintSingleMatch;

		private NewbieStepFakeMatchFiveHintSelMapFive _stepHintSelMapFive;

		private NewbieStepFakeMatchFiveHintMatch _stepHintMatch;

		private NewbieStepFakeMatchFiveHintSelHero _stepHintSelHero;

		private NewbieStepFakeMatchFiveHintSelHeroEnd _stepHintSelHeroEnd;

		private NewbieStepFakeMatchFiveHintHeroSkill _stepHintHeroSkill;

		private NewbieStepFakeMatchFiveHintHeroSkillEnd _stepHintHeroSkillEnd;

		private NewbieStepFakeMatchFiveOpenSummonerSkill _stepOpenSummonerSkill;

		private NewbieStepFakeMatchFiveOpenSummSkillEnd _stepOpenSummSkillEnd;

		private NewbieStepFakeMatchFiveSelHeroConfirm _stepSelHeroConfirm;

		private NewbieStepFakeMatchFiveSelHeroConfirmEnd _stepSelHeroConfirmEnd;

		private NewbieStepFakeMatchFiveBeginLoad _stepBeginLoad;

		private NewbieStepFakeMatchFiveTriggerChecker _stepTriggerChecker;

		private NewbieStepFakeMatchFiveFirHpLessNinty _stepFirHpLessNinty;

		private NewbieStepFakeMatchFiveFirHpLessNintyEnd _stepFirHpLessNintyEnd;

		private NewbieStepFakeMatchFiveFirHpLessThirty _stepFirHpLessThirty;

		private NewbieStepFakeMatchFiveFirHpLessThirtyEnd _stepFirHpLessThirtyEnd;

		private NewbieStepFakeMatchFiveFirSelUpWay _stepFirSelUpWay;

		private NewbieStepFakeMatchFiveFirSelUpWayEnd _stepFirSelUpWayEnd;

		private NewbieStepFakeMatchFiveFirSelDownWay _stepFirSelDownWay;

		private NewbieStepFakeMatchFiveFirSelDownWayEnd _stepFirSelDownWayEnd;

		private NewbieStepFakeMatchFiveSkillPanelSet _stepSkillPanelSet;

		private NewbieStepFakeMatchFiveSkillPanelSetEnd _stepSkillPanelSetEnd;

		private NewbieStepFakeMatchFiveFirHeroDead _stepFirHeroDead;

		private NewbieStepFakeMatchFiveFirHeroDeadEnd _stepFirHeroDeadEnd;

		private NewbieStepFakeMatchFiveFirNearFirTower _stepFirNearFirTower;

		private NewbieStepFakeMatchFiveFirNearFirTowerEnd _stepFirNearFirTowerEnd;

		private NewbieStepFakeMatchFiveFirFreeCam _stepFirFreeCam;

		private NewbieStepFakeMatchFiveFirFreeCamEnd _stepFirFreeCamEnd;

		private NewbieStepFakeMatchFiveSettlementFir _stepSettlementFir;

		private NewbieStepFakeMatchFiveSettlementSec _stepSettlementSec;

		private NewbieStepFakeMatchFiveSettlementThd _stepSettlementThd;

		private NewbieStepFakeMatchFiveBackHome _stepBackHome;

		private NewbieStepFakeMatchFiveEnd _stepEnd;

		public NewbiePhaseFakeMatchFive()
		{
			this._phaseType = ENewbiePhaseType.FakeMatchFive;
			this.InitSteps();
		}

		public override void InitSteps()
		{
			this._stepHintPlay = new NewbieStepFakeMatchFiveHintPlay();
			this._stepHintSingleMatch = new NewbieStepFakeMatchFiveHintSingleMatch();
			this._stepHintSelMapFive = new NewbieStepFakeMatchFiveHintSelMapFive();
			this._stepHintMatch = new NewbieStepFakeMatchFiveHintMatch();
			this._stepHintSelHero = new NewbieStepFakeMatchFiveHintSelHero();
			this._stepHintSelHeroEnd = new NewbieStepFakeMatchFiveHintSelHeroEnd();
			this._stepHintHeroSkill = new NewbieStepFakeMatchFiveHintHeroSkill();
			this._stepHintHeroSkillEnd = new NewbieStepFakeMatchFiveHintHeroSkillEnd();
			this._stepOpenSummonerSkill = new NewbieStepFakeMatchFiveOpenSummonerSkill();
			this._stepOpenSummSkillEnd = new NewbieStepFakeMatchFiveOpenSummSkillEnd();
			this._stepSelHeroConfirm = new NewbieStepFakeMatchFiveSelHeroConfirm();
			this._stepSelHeroConfirmEnd = new NewbieStepFakeMatchFiveSelHeroConfirmEnd();
			this._stepBeginLoad = new NewbieStepFakeMatchFiveBeginLoad();
			this._stepTriggerChecker = new NewbieStepFakeMatchFiveTriggerChecker();
			this._stepFirHpLessNinty = new NewbieStepFakeMatchFiveFirHpLessNinty();
			this._stepFirHpLessNintyEnd = new NewbieStepFakeMatchFiveFirHpLessNintyEnd();
			this._stepFirHpLessThirty = new NewbieStepFakeMatchFiveFirHpLessThirty();
			this._stepFirHpLessThirtyEnd = new NewbieStepFakeMatchFiveFirHpLessThirtyEnd();
			this._stepFirSelUpWay = new NewbieStepFakeMatchFiveFirSelUpWay();
			this._stepFirSelUpWayEnd = new NewbieStepFakeMatchFiveFirSelUpWayEnd();
			this._stepFirSelDownWay = new NewbieStepFakeMatchFiveFirSelDownWay();
			this._stepFirSelDownWayEnd = new NewbieStepFakeMatchFiveFirSelDownWayEnd();
			this._stepSkillPanelSet = new NewbieStepFakeMatchFiveSkillPanelSet();
			this._stepSkillPanelSetEnd = new NewbieStepFakeMatchFiveSkillPanelSetEnd();
			this._stepFirHeroDead = new NewbieStepFakeMatchFiveFirHeroDead();
			this._stepFirHeroDeadEnd = new NewbieStepFakeMatchFiveFirHeroDeadEnd();
			this._stepFirNearFirTower = new NewbieStepFakeMatchFiveFirNearFirTower();
			this._stepFirNearFirTowerEnd = new NewbieStepFakeMatchFiveFirNearFirTowerEnd();
			this._stepFirFreeCam = new NewbieStepFakeMatchFiveFirFreeCam();
			this._stepFirFreeCamEnd = new NewbieStepFakeMatchFiveFirFreeCamEnd();
			this._stepSettlementFir = new NewbieStepFakeMatchFiveSettlementFir();
			this._stepSettlementSec = new NewbieStepFakeMatchFiveSettlementSec();
			this._stepSettlementThd = new NewbieStepFakeMatchFiveSettlementThd();
			this._stepBackHome = new NewbieStepFakeMatchFiveBackHome();
			this._stepEnd = new NewbieStepFakeMatchFiveEnd();
			this._stepSettlementFir.BindNextStep(this._stepSettlementSec);
			this._stepSettlementSec.BindNextStep(this._stepSettlementThd);
			this._stepSettlementThd.BindNextStep(this._stepBackHome);
			this._stepBackHome.BindNextStep(this._stepEnd);
			this._allSteps.Add(this._stepHintPlay);
			this._allSteps.Add(this._stepHintSingleMatch);
			this._allSteps.Add(this._stepHintSelMapFive);
			this._allSteps.Add(this._stepHintMatch);
			this._allSteps.Add(this._stepHintSelHero);
			this._allSteps.Add(this._stepHintSelHeroEnd);
			this._allSteps.Add(this._stepHintHeroSkill);
			this._allSteps.Add(this._stepHintHeroSkillEnd);
			this._allSteps.Add(this._stepOpenSummonerSkill);
			this._allSteps.Add(this._stepOpenSummSkillEnd);
			this._allSteps.Add(this._stepSelHeroConfirm);
			this._allSteps.Add(this._stepSelHeroConfirmEnd);
			this._allSteps.Add(this._stepBeginLoad);
			this._allSteps.Add(this._stepTriggerChecker);
			this._allSteps.Add(this._stepFirHpLessNinty);
			this._allSteps.Add(this._stepFirHpLessNintyEnd);
			this._allSteps.Add(this._stepFirHpLessThirty);
			this._allSteps.Add(this._stepFirHpLessThirtyEnd);
			this._allSteps.Add(this._stepFirSelUpWay);
			this._allSteps.Add(this._stepFirSelUpWayEnd);
			this._allSteps.Add(this._stepFirSelDownWay);
			this._allSteps.Add(this._stepFirSelDownWayEnd);
			this._allSteps.Add(this._stepSkillPanelSet);
			this._allSteps.Add(this._stepSkillPanelSetEnd);
			this._allSteps.Add(this._stepFirHeroDead);
			this._allSteps.Add(this._stepFirHeroDeadEnd);
			this._allSteps.Add(this._stepFirNearFirTower);
			this._allSteps.Add(this._stepFirNearFirTowerEnd);
			this._allSteps.Add(this._stepFirFreeCam);
			this._allSteps.Add(this._stepFirFreeCamEnd);
			this._allSteps.Add(this._stepSettlementFir);
			this._allSteps.Add(this._stepSettlementSec);
			this._allSteps.Add(this._stepSettlementThd);
			this._allSteps.Add(this._stepBackHome);
			this._allSteps.Add(this._stepEnd);
		}

		public override void EnterPhase()
		{
			this.InitPhaseResources();
			this._curStep = this._stepHintPlay;
			this._curStep.HandleAction();
		}

		private void InitPhaseResources()
		{
			NewbieManager.Instance.InitFakeMatchFiveResource();
			this.LoadSoundBankFakeMatchFive();
		}

		private void LoadSoundBankFakeMatchFive()
		{
			AudioMgr.LoadBnk("Guide_C.bnk");
		}
	}
}
