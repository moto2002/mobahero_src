using System;

namespace Newbie
{
	public class NewbiePhaseElementaryBattleOneOne : NewbiePhaseBase
	{
		private NewbieStepEleBatOneOneSelMap _stepSelMap;

		private NewbieStepEleBatOneOneLoading _stepLoading;

		private NewbieStepEleBatOneOneHeroIn _stepHeroIn;

		private NewbieStepEleBatOneOneNormalView _stepNormalView;

		private NewbieStepEleBatOneOneLearnHoldDev _stepLearnHoldDev;

		private NewbieStepEleBatOneOneWaitStartPlay _stepWaitStartPlay;

		private NewbieStepEleBatOneOneHeroAppear _stepHeroAppear;

		private NewbieStepEleBatOneOneOpenStore _stepOpenStore;

		private NewbieStepEleBatOneOneStoreHint _stepStoreHint;

		private NewbieStepEleBatOneOneCloseStore _stepCloseStore;

		private NewbieStepEleBatOneOneBuyEquip _stepBuyEquip;

		private NewbieStepEleBatOneOneBuyEquipAward _stepBuyEquipAward;

		private NewbieStepEleBatOneOneMoveToHome _stepMoveToHome;

		private NewbieStepEleBatOneOneMoveHomeAward _stepMoveHomeAward;

		private NewbieStepEleBatOneOneMoveToTower _stepMoveToTower;

		private NewbieStepEleBatOneOneMoveTowerAward _stepMoveTowerAward;

		private NewbieStepEleBatOneOneKillSoldier _stepKillSoldier;

		private NewbieStepEleBatOneOneSelSoldier _stepSelSoldier;

		private NewbieStepEleBatOneOneSelSoldierEnd _stepSelSoldierEnd;

		private NewbieStepEleBatOneOneKillSoldierAward _stepKillSoldierAward;

		private NewbieStepEleBatOneOneLearnSkill _stepLearnSkill;

		private NewbieStepEleBatOneOneLearnSkillThdAwd _stepLearnSkillThdAwd;

		private NewbieStepEleBatOneOneSpawnFourSoldier _stepSpawnFourSoldier;

		private NewbieStepEleBatOneOneUseSkill _stepUseSkill;

		private NewbieStepEleBatOneOneUseSkillThdEnd _stepUseSkillThdEnd;

		private NewbieStepEleBatOneOneFourSoldierAwd _stepFourSoldierAwd;

		private NewbieStepEleBatOneOneLearnSkillOne _stepLearnSkillOne;

		private NewbieStepEleBatOneOneLearnSkillOneAwd _stepLearnSkillOneAwd;

		private NewbieStepEleBatOneOneSpawnEnemyHero _stepSpawnEnemyHero;

		private NewbieStepEleBatOneOneSelEnemyHero _stepSelEnemyHero;

		private NewbieStepEleBatOneOneUseSkillFirst _stepUseSkillFirst;

		private NewbieStepEleBatOneOneUseSkillFirEnd _stepUseSkillFirEnd;

		private NewbieStepEleBatOneOneKillHeroAwd _stepKillHeroAwd;

		private NewbieStepEleBatOneOneLevelUpFour _stepLevelUpFour;

		private NewbieStepEleBatOneOneLearnSkillSecond _stepLearnSkillSecond;

		private NewbieStepEleBatOneOneLearnSkillSndAwd _stepLearnSkillSndAwd;

		private NewbieStepEleBatOneOneCamMoveEnemyHome _stepCamMoveEnemyHome;

		private NewbieStepBatOneOneEnemyHomeHint _stepEnemyHomeHint;

		private NewbieStepBatOneOneCamMoveEnemyTower _stepCamMoveEnemyTower;

		private NewbieStepBatOneOneEnemyTowerHint _stepEnemyTowerHint;

		private NewbieStepBatOneOneCamMoveSelf _stepCamMoveSelf;

		private NewbieStepBatOneOneMoveEnemyTower _stepMoveEnemyTower;

		private NewbieStepBatOneOneSoldierBearHurt _stepSoldierBearHurt;

		private NewbieStepBatOneOneDestroyTower _stepDestroyTower;

		private NewbieStepBatOneOneDestroyTowerAwd _stepDestroyTowerAwd;

		private NewbieStepBatOneOneLevelUpSix _stepLevelUpSix;

		private NewbieStepBatOneOneLearnSkillFourth _stepLearnSkillFourth;

		private NewbieStepBatOneOneReliveHero _stepReliveHero;

		private NewbieStepBatOneOneSelReliveHero _stepSelReliveHero;

		private NewbieStepBatOneOneUseSkillFourth _stepUseSkillFourth;

		private NewbieStepBatOneOneUseSkillFourEnd _stepUseSkillFourEnd;

		private NewbieStepBatOneOneRelivedHeroAwd _stepRelivedHeroAwd;

		private NewbieStepBatOneOneKillHome _stepKillHome;

		private NewbieStepBatOneOneSettlementFir _stepSettlementFir;

		private NewbieStepBatOneOneSettlementSec _stepSettlementSec;

		private NewbieStepBatOneOneSettlementThd _stepSettlementThd;

		private NewbieStepBatOneOneBackHome _stepBackHome;

		private NewbieStepBatOneOneEnd _stepEnd;

		public NewbiePhaseElementaryBattleOneOne()
		{
			this._phaseType = ENewbiePhaseType.ElementaryBattleOneOne;
			this.InitSteps();
		}

		public override void InitSteps()
		{
			this._stepSelMap = new NewbieStepEleBatOneOneSelMap();
			this._stepLoading = new NewbieStepEleBatOneOneLoading();
			this._stepHeroIn = new NewbieStepEleBatOneOneHeroIn();
			this._stepNormalView = new NewbieStepEleBatOneOneNormalView();
			this._stepLearnHoldDev = new NewbieStepEleBatOneOneLearnHoldDev();
			this._stepWaitStartPlay = new NewbieStepEleBatOneOneWaitStartPlay();
			this._stepHeroAppear = new NewbieStepEleBatOneOneHeroAppear();
			this._stepOpenStore = new NewbieStepEleBatOneOneOpenStore();
			this._stepStoreHint = new NewbieStepEleBatOneOneStoreHint();
			this._stepCloseStore = new NewbieStepEleBatOneOneCloseStore();
			this._stepBuyEquip = new NewbieStepEleBatOneOneBuyEquip();
			this._stepBuyEquipAward = new NewbieStepEleBatOneOneBuyEquipAward();
			this._stepMoveToHome = new NewbieStepEleBatOneOneMoveToHome();
			this._stepMoveHomeAward = new NewbieStepEleBatOneOneMoveHomeAward();
			this._stepMoveToTower = new NewbieStepEleBatOneOneMoveToTower();
			this._stepMoveTowerAward = new NewbieStepEleBatOneOneMoveTowerAward();
			this._stepKillSoldier = new NewbieStepEleBatOneOneKillSoldier();
			this._stepSelSoldier = new NewbieStepEleBatOneOneSelSoldier();
			this._stepSelSoldierEnd = new NewbieStepEleBatOneOneSelSoldierEnd();
			this._stepKillSoldierAward = new NewbieStepEleBatOneOneKillSoldierAward();
			this._stepLearnSkill = new NewbieStepEleBatOneOneLearnSkill();
			this._stepLearnSkillThdAwd = new NewbieStepEleBatOneOneLearnSkillThdAwd();
			this._stepSpawnFourSoldier = new NewbieStepEleBatOneOneSpawnFourSoldier();
			this._stepUseSkill = new NewbieStepEleBatOneOneUseSkill();
			this._stepUseSkillThdEnd = new NewbieStepEleBatOneOneUseSkillThdEnd();
			this._stepFourSoldierAwd = new NewbieStepEleBatOneOneFourSoldierAwd();
			this._stepLearnSkillOne = new NewbieStepEleBatOneOneLearnSkillOne();
			this._stepLearnSkillOneAwd = new NewbieStepEleBatOneOneLearnSkillOneAwd();
			this._stepSpawnEnemyHero = new NewbieStepEleBatOneOneSpawnEnemyHero();
			this._stepSelEnemyHero = new NewbieStepEleBatOneOneSelEnemyHero();
			this._stepUseSkillFirst = new NewbieStepEleBatOneOneUseSkillFirst();
			this._stepUseSkillFirEnd = new NewbieStepEleBatOneOneUseSkillFirEnd();
			this._stepKillHeroAwd = new NewbieStepEleBatOneOneKillHeroAwd();
			this._stepLevelUpFour = new NewbieStepEleBatOneOneLevelUpFour();
			this._stepLearnSkillSecond = new NewbieStepEleBatOneOneLearnSkillSecond();
			this._stepLearnSkillSndAwd = new NewbieStepEleBatOneOneLearnSkillSndAwd();
			this._stepCamMoveEnemyHome = new NewbieStepEleBatOneOneCamMoveEnemyHome();
			this._stepEnemyHomeHint = new NewbieStepBatOneOneEnemyHomeHint();
			this._stepCamMoveEnemyTower = new NewbieStepBatOneOneCamMoveEnemyTower();
			this._stepEnemyTowerHint = new NewbieStepBatOneOneEnemyTowerHint();
			this._stepCamMoveSelf = new NewbieStepBatOneOneCamMoveSelf();
			this._stepMoveEnemyTower = new NewbieStepBatOneOneMoveEnemyTower();
			this._stepSoldierBearHurt = new NewbieStepBatOneOneSoldierBearHurt();
			this._stepDestroyTower = new NewbieStepBatOneOneDestroyTower();
			this._stepDestroyTowerAwd = new NewbieStepBatOneOneDestroyTowerAwd();
			this._stepLevelUpSix = new NewbieStepBatOneOneLevelUpSix();
			this._stepLearnSkillFourth = new NewbieStepBatOneOneLearnSkillFourth();
			this._stepReliveHero = new NewbieStepBatOneOneReliveHero();
			this._stepSelReliveHero = new NewbieStepBatOneOneSelReliveHero();
			this._stepUseSkillFourth = new NewbieStepBatOneOneUseSkillFourth();
			this._stepUseSkillFourEnd = new NewbieStepBatOneOneUseSkillFourEnd();
			this._stepRelivedHeroAwd = new NewbieStepBatOneOneRelivedHeroAwd();
			this._stepKillHome = new NewbieStepBatOneOneKillHome();
			this._stepSettlementFir = new NewbieStepBatOneOneSettlementFir();
			this._stepSettlementSec = new NewbieStepBatOneOneSettlementSec();
			this._stepSettlementThd = new NewbieStepBatOneOneSettlementThd();
			this._stepBackHome = new NewbieStepBatOneOneBackHome();
			this._stepEnd = new NewbieStepBatOneOneEnd();
			this._stepSelMap.BindNextStep(this._stepLoading);
			this._stepLoading.BindNextStep(this._stepNormalView);
			this._stepNormalView.BindNextStep(this._stepLearnHoldDev);
			this._stepLearnHoldDev.BindNextStep(this._stepWaitStartPlay);
			this._stepWaitStartPlay.BindNextStep(this._stepHeroAppear);
			this._stepHeroAppear.BindNextStep(this._stepOpenStore);
			this._stepOpenStore.BindNextStep(this._stepStoreHint);
			this._stepStoreHint.BindNextStep(this._stepCloseStore);
			this._stepCloseStore.BindNextStep(this._stepBuyEquip);
			this._stepBuyEquip.BindNextStep(this._stepBuyEquipAward);
			this._stepBuyEquipAward.BindNextStep(this._stepMoveToHome);
			this._stepMoveToHome.BindNextStep(this._stepMoveHomeAward);
			this._stepMoveHomeAward.BindNextStep(this._stepMoveToTower);
			this._stepMoveToTower.BindNextStep(this._stepMoveTowerAward);
			this._stepMoveTowerAward.BindNextStep(this._stepKillSoldier);
			this._stepKillSoldier.BindNextStep(this._stepSelSoldier);
			this._stepSelSoldier.BindNextStep(this._stepSelSoldierEnd);
			this._stepKillSoldierAward.BindNextStep(this._stepLearnSkill);
			this._stepLearnSkill.BindNextStep(this._stepLearnSkillThdAwd);
			this._stepLearnSkillThdAwd.BindNextStep(this._stepSpawnFourSoldier);
			this._stepSpawnFourSoldier.BindNextStep(this._stepUseSkill);
			this._stepUseSkill.BindNextStep(this._stepUseSkillThdEnd);
			this._stepFourSoldierAwd.BindNextStep(this._stepLearnSkillOne);
			this._stepLearnSkillOne.BindNextStep(this._stepLearnSkillOneAwd);
			this._stepLearnSkillOneAwd.BindNextStep(this._stepSpawnEnemyHero);
			this._stepSpawnEnemyHero.BindNextStep(this._stepSelEnemyHero);
			this._stepSelEnemyHero.BindNextStep(this._stepUseSkillFirst);
			this._stepUseSkillFirst.BindNextStep(this._stepUseSkillFirEnd);
			this._stepKillHeroAwd.BindNextStep(this._stepLevelUpFour);
			this._stepLevelUpFour.BindNextStep(this._stepLearnSkillSecond);
			this._stepLearnSkillSecond.BindNextStep(this._stepLearnSkillSndAwd);
			this._stepLearnSkillSndAwd.BindNextStep(this._stepCamMoveEnemyHome);
			this._stepCamMoveEnemyHome.BindNextStep(this._stepEnemyHomeHint);
			this._stepEnemyHomeHint.BindNextStep(this._stepCamMoveEnemyTower);
			this._stepCamMoveEnemyTower.BindNextStep(this._stepEnemyTowerHint);
			this._stepEnemyTowerHint.BindNextStep(this._stepCamMoveSelf);
			this._stepCamMoveSelf.BindNextStep(this._stepMoveEnemyTower);
			this._stepMoveEnemyTower.BindNextStep(this._stepSoldierBearHurt);
			this._stepSoldierBearHurt.BindNextStep(this._stepDestroyTower);
			this._stepDestroyTowerAwd.BindNextStep(this._stepLevelUpSix);
			this._stepLevelUpSix.BindNextStep(this._stepLearnSkillFourth);
			this._stepLearnSkillFourth.BindNextStep(this._stepReliveHero);
			this._stepReliveHero.BindNextStep(this._stepSelReliveHero);
			this._stepSelReliveHero.BindNextStep(this._stepUseSkillFourth);
			this._stepUseSkillFourth.BindNextStep(this._stepUseSkillFourEnd);
			this._stepRelivedHeroAwd.BindNextStep(this._stepKillHome);
			this._stepSettlementFir.BindNextStep(this._stepSettlementSec);
			this._stepSettlementSec.BindNextStep(this._stepSettlementThd);
			this._stepSettlementThd.BindNextStep(this._stepBackHome);
			this._stepBackHome.BindNextStep(this._stepEnd);
			this._allSteps.Add(this._stepSelMap);
			this._allSteps.Add(this._stepLoading);
			this._allSteps.Add(this._stepHeroIn);
			this._allSteps.Add(this._stepNormalView);
			this._allSteps.Add(this._stepLearnHoldDev);
			this._allSteps.Add(this._stepWaitStartPlay);
			this._allSteps.Add(this._stepHeroAppear);
			this._allSteps.Add(this._stepOpenStore);
			this._allSteps.Add(this._stepStoreHint);
			this._allSteps.Add(this._stepCloseStore);
			this._allSteps.Add(this._stepBuyEquip);
			this._allSteps.Add(this._stepBuyEquipAward);
			this._allSteps.Add(this._stepMoveToHome);
			this._allSteps.Add(this._stepMoveHomeAward);
			this._allSteps.Add(this._stepMoveToTower);
			this._allSteps.Add(this._stepMoveTowerAward);
			this._allSteps.Add(this._stepKillSoldier);
			this._allSteps.Add(this._stepSelSoldier);
			this._allSteps.Add(this._stepSelSoldierEnd);
			this._allSteps.Add(this._stepKillSoldierAward);
			this._allSteps.Add(this._stepLearnSkill);
			this._allSteps.Add(this._stepLearnSkillThdAwd);
			this._allSteps.Add(this._stepSpawnFourSoldier);
			this._allSteps.Add(this._stepUseSkill);
			this._allSteps.Add(this._stepUseSkillThdEnd);
			this._allSteps.Add(this._stepFourSoldierAwd);
			this._allSteps.Add(this._stepLearnSkillOne);
			this._allSteps.Add(this._stepLearnSkillOneAwd);
			this._allSteps.Add(this._stepSpawnEnemyHero);
			this._allSteps.Add(this._stepSelEnemyHero);
			this._allSteps.Add(this._stepUseSkillFirst);
			this._allSteps.Add(this._stepUseSkillFirEnd);
			this._allSteps.Add(this._stepKillHeroAwd);
			this._allSteps.Add(this._stepLevelUpFour);
			this._allSteps.Add(this._stepLearnSkillSecond);
			this._allSteps.Add(this._stepLearnSkillSndAwd);
			this._allSteps.Add(this._stepCamMoveEnemyHome);
			this._allSteps.Add(this._stepEnemyHomeHint);
			this._allSteps.Add(this._stepCamMoveEnemyTower);
			this._allSteps.Add(this._stepEnemyTowerHint);
			this._allSteps.Add(this._stepCamMoveSelf);
			this._allSteps.Add(this._stepMoveEnemyTower);
			this._allSteps.Add(this._stepSoldierBearHurt);
			this._allSteps.Add(this._stepDestroyTower);
			this._allSteps.Add(this._stepDestroyTowerAwd);
			this._allSteps.Add(this._stepLevelUpSix);
			this._allSteps.Add(this._stepLearnSkillFourth);
			this._allSteps.Add(this._stepReliveHero);
			this._allSteps.Add(this._stepSelReliveHero);
			this._allSteps.Add(this._stepUseSkillFourth);
			this._allSteps.Add(this._stepUseSkillFourEnd);
			this._allSteps.Add(this._stepRelivedHeroAwd);
			this._allSteps.Add(this._stepKillHome);
			this._allSteps.Add(this._stepSettlementFir);
			this._allSteps.Add(this._stepSettlementSec);
			this._allSteps.Add(this._stepSettlementThd);
			this._allSteps.Add(this._stepBackHome);
			this._allSteps.Add(this._stepEnd);
		}

		public override void EnterPhase()
		{
			this.InitPhaseResources();
			this._curStep = this._stepSelMap;
			this._curStep.HandleAction();
		}

		private void InitPhaseResources()
		{
			NewbieManager.Instance.InitCommonResource();
			NewbieManager.Instance.InitEleBatOneResource();
			this.LoadSoundBankEleBatOne();
		}

		private void LoadSoundBankEleBatOne()
		{
			AudioMgr.LoadBnk("Guide_A.bnk");
		}
	}
}
