using System;

namespace Newbie
{
	public class NewbiePhaseElementaryBattleFiveFive : NewbiePhaseBase
	{
		private NewbieStepEleBatFiveHintEnterBatFive _stepHintEnterBatFive;

		private NewbieStepEleBatFiveHintToSelectMap _stepHintToSelectMap;

		private NewbieStepEleBatFiveSelectMap _stepSelectMap;

		private NewbieStepEleBatFiveLoading _stepLoading;

		private NewbieStepEleBatFiveHintPicFirst _stepHintPicFirst;

		private NewbieStepEleBatFiveHintPicFirstGoNext _stepHintPicFirstGoNext;

		private NewbieStepEleBatFiveHintPicSecond _stepHintPicSecond;

		private NewbieStepEleBatFiveHintPicSecondGoNext _stepHintPicSecondGoNext;

		private NewbieStepEleBatFiveBuyEquipment _stepBuyEquipment;

		private NewbieStepEleBatFivePlayerSelectOneWay _stepPlayerSelectOneWay;

		private NewbieStepEleBatFivePlayerSelectWayEnd _stepPlayerSelectWayEnd;

		private NewbieStepEleBatFiveFirAtkByTower _stepFirstAtkByTower;

		private NewbieStepEleBatFiveFirAtkByTowerEnd _stepFirstAtkByTowerEnd;

		private NewbieStepEleBatFiveFirSeeEnemySoldier _stepFirSeeEnemySoldier;

		private NewbieStepEleBatFiveFirSeeEnemySoldierEnd _stepFirSeeEnemySoldierEnd;

		private NewbieStepEleBatFiveFirSeeEnemyTower _stepFirSeeEnemyTower;

		private NewbieStepEleBatFiveFirSeeEnemyTowerEnd _stepFirSeeEnemyTowerEnd;

		private NewbieStepEleBatFiveFirInEnemyTowerRange _stepFirInEnemyTowerRange;

		private NewbieStepEleBatFiveFirInEnemyTowerRangeEnd _stepFirInEnemyTowerRangeEnd;

		private NewbieStepEleBatFiveFirHpLessEighty _stepFirHpLessEighty;

		private NewbieStepEleBatFiveFirHpLessEightyEnd _stepFirHpLessEightyEnd;

		private NewbieStepEleBatFiveFirHpLessThirty _stepFirHpLessThirty;

		private NewbieStepEleBatFiveFirHpLessThirtyEnd _stepFirHpLessThirtyEnd;

		private NewbieStepEleBatFiveFirNearOutShop _stepFirNearOutShop;

		private NewbieStepEleBatFiveFirNearOutShopEnd _stepFirNearOutShopEnd;

		private NewbieStepEleBatFiveFirBuyEquipHint _stepFirBuyEquipHint;

		private NewbieStepEleBatFiveFirBuyEquipHintEnd _stepFirBuyEquipHintEnd;

		private NewbieStepEleBatFiveFirOverMoney _stepFirOverMoney;

		private NewbieStepEleBatFiveFirOverMoneyEnd _stepFirOverMoneyEnd;

		private NewbieStepEleBatFiveFirAtkSecondTower _stepFirAtkSecondTower;

		private NewbieStepEleBatFiveFirAtkSecondTowerEnd _stepFirAtkSecondTowerEnd;

		private NewbieStepEleBatFiveFirInEnemySpring _stepFirInEnemySpring;

		private NewbieStepEleBatFiveFirInEnemySpringEnd _stepFirInEnemySpringEnd;

		private NewbieStepEleBatFiveFirAtkHome _stepFirAtkHome;

		private NewbieStepEleBatFiveFirAtkHomeEnd _stepFirAtkHomeEnd;

		private NewbieStepEleBatFiveLineTowersDead _stepLineTowersDead;

		private NewbieStepEleBatFiveLineTowersDeadEnd _stepLineTowersDeadEnd;

		private NewbieStepEleBatFiveFirNearGrass _stepFirNearGrass;

		private NewbieStepEleBatFiveFirNearGrassEnd _stepFirNearGrassEnd;

		private NewbieStepEleBatFiveSettlementFir _stepSettlementFir;

		private NewbieStepEleBatFiveSettlementSec _stepSettlementSec;

		private NewbieStepEleBatFiveSettlementThd _stepSettlementThd;

		private NewbieStepEleBatFiveBackHome _stepBackHome;

		private NewbieStepEleBatFiveEnd _stepEnd;

		public NewbiePhaseElementaryBattleFiveFive()
		{
			this._phaseType = ENewbiePhaseType.ElementaryBattleFiveFive;
			this.InitSteps();
		}

		public override void InitSteps()
		{
			this._stepHintEnterBatFive = new NewbieStepEleBatFiveHintEnterBatFive();
			this._stepHintToSelectMap = new NewbieStepEleBatFiveHintToSelectMap();
			this._stepSelectMap = new NewbieStepEleBatFiveSelectMap();
			this._stepLoading = new NewbieStepEleBatFiveLoading();
			this._stepHintPicFirst = new NewbieStepEleBatFiveHintPicFirst();
			this._stepHintPicFirstGoNext = new NewbieStepEleBatFiveHintPicFirstGoNext();
			this._stepHintPicSecond = new NewbieStepEleBatFiveHintPicSecond();
			this._stepHintPicSecondGoNext = new NewbieStepEleBatFiveHintPicSecondGoNext();
			this._stepBuyEquipment = new NewbieStepEleBatFiveBuyEquipment();
			this._stepPlayerSelectOneWay = new NewbieStepEleBatFivePlayerSelectOneWay();
			this._stepPlayerSelectWayEnd = new NewbieStepEleBatFivePlayerSelectWayEnd();
			this._stepFirstAtkByTower = new NewbieStepEleBatFiveFirAtkByTower();
			this._stepFirstAtkByTowerEnd = new NewbieStepEleBatFiveFirAtkByTowerEnd();
			this._stepFirSeeEnemySoldier = new NewbieStepEleBatFiveFirSeeEnemySoldier();
			this._stepFirSeeEnemySoldierEnd = new NewbieStepEleBatFiveFirSeeEnemySoldierEnd();
			this._stepFirSeeEnemyTower = new NewbieStepEleBatFiveFirSeeEnemyTower();
			this._stepFirSeeEnemyTowerEnd = new NewbieStepEleBatFiveFirSeeEnemyTowerEnd();
			this._stepFirInEnemyTowerRange = new NewbieStepEleBatFiveFirInEnemyTowerRange();
			this._stepFirInEnemyTowerRangeEnd = new NewbieStepEleBatFiveFirInEnemyTowerRangeEnd();
			this._stepFirHpLessEighty = new NewbieStepEleBatFiveFirHpLessEighty();
			this._stepFirHpLessEightyEnd = new NewbieStepEleBatFiveFirHpLessEightyEnd();
			this._stepFirHpLessThirty = new NewbieStepEleBatFiveFirHpLessThirty();
			this._stepFirHpLessThirtyEnd = new NewbieStepEleBatFiveFirHpLessThirtyEnd();
			this._stepFirNearOutShop = new NewbieStepEleBatFiveFirNearOutShop();
			this._stepFirNearOutShopEnd = new NewbieStepEleBatFiveFirNearOutShopEnd();
			this._stepFirBuyEquipHint = new NewbieStepEleBatFiveFirBuyEquipHint();
			this._stepFirBuyEquipHintEnd = new NewbieStepEleBatFiveFirBuyEquipHintEnd();
			this._stepFirOverMoney = new NewbieStepEleBatFiveFirOverMoney();
			this._stepFirOverMoneyEnd = new NewbieStepEleBatFiveFirOverMoneyEnd();
			this._stepFirAtkSecondTower = new NewbieStepEleBatFiveFirAtkSecondTower();
			this._stepFirAtkSecondTowerEnd = new NewbieStepEleBatFiveFirAtkSecondTowerEnd();
			this._stepFirInEnemySpring = new NewbieStepEleBatFiveFirInEnemySpring();
			this._stepFirInEnemySpringEnd = new NewbieStepEleBatFiveFirInEnemySpringEnd();
			this._stepFirAtkHome = new NewbieStepEleBatFiveFirAtkHome();
			this._stepFirAtkHomeEnd = new NewbieStepEleBatFiveFirAtkHomeEnd();
			this._stepLineTowersDead = new NewbieStepEleBatFiveLineTowersDead();
			this._stepLineTowersDeadEnd = new NewbieStepEleBatFiveLineTowersDeadEnd();
			this._stepFirNearGrass = new NewbieStepEleBatFiveFirNearGrass();
			this._stepFirNearGrassEnd = new NewbieStepEleBatFiveFirNearGrassEnd();
			this._stepSettlementFir = new NewbieStepEleBatFiveSettlementFir();
			this._stepSettlementSec = new NewbieStepEleBatFiveSettlementSec();
			this._stepSettlementThd = new NewbieStepEleBatFiveSettlementThd();
			this._stepBackHome = new NewbieStepEleBatFiveBackHome();
			this._stepEnd = new NewbieStepEleBatFiveEnd();
			this._stepHintEnterBatFive.BindNextStep(this._stepHintToSelectMap);
			this._stepHintToSelectMap.BindNextStep(this._stepSelectMap);
			this._stepSelectMap.BindNextStep(this._stepLoading);
			this._stepLoading.BindNextStep(this._stepHintPicFirst);
			this._stepHintPicFirst.BindNextStep(this._stepHintPicFirstGoNext);
			this._stepHintPicFirstGoNext.BindNextStep(this._stepHintPicSecond);
			this._stepHintPicSecond.BindNextStep(this._stepHintPicSecondGoNext);
			this._stepHintPicSecondGoNext.BindNextStep(this._stepBuyEquipment);
			this._stepBuyEquipment.BindNextStep(this._stepPlayerSelectOneWay);
			this._stepPlayerSelectOneWay.BindNextStep(this._stepPlayerSelectWayEnd);
			this._stepFirstAtkByTower.BindNextStep(this._stepFirstAtkByTowerEnd);
			this._stepFirSeeEnemySoldier.BindNextStep(this._stepFirSeeEnemySoldierEnd);
			this._stepFirSeeEnemyTower.BindNextStep(this._stepFirSeeEnemyTowerEnd);
			this._stepFirInEnemyTowerRange.BindNextStep(this._stepFirInEnemyTowerRangeEnd);
			this._stepFirHpLessEighty.BindNextStep(this._stepFirHpLessEightyEnd);
			this._stepFirHpLessThirty.BindNextStep(this._stepFirHpLessThirtyEnd);
			this._stepFirNearOutShop.BindNextStep(this._stepFirNearOutShopEnd);
			this._stepFirBuyEquipHint.BindNextStep(this._stepFirBuyEquipHintEnd);
			this._stepFirOverMoney.BindNextStep(this._stepFirOverMoneyEnd);
			this._stepFirAtkSecondTower.BindNextStep(this._stepFirAtkSecondTowerEnd);
			this._stepFirInEnemySpring.BindNextStep(this._stepFirInEnemySpringEnd);
			this._stepFirAtkHome.BindNextStep(this._stepFirAtkHomeEnd);
			this._stepLineTowersDead.BindNextStep(this._stepLineTowersDeadEnd);
			this._stepFirNearGrass.BindNextStep(this._stepFirNearGrassEnd);
			this._stepSettlementFir.BindNextStep(this._stepSettlementSec);
			this._stepSettlementSec.BindNextStep(this._stepSettlementThd);
			this._stepSettlementThd.BindNextStep(this._stepBackHome);
			this._stepBackHome.BindNextStep(this._stepEnd);
			this._allSteps.Add(this._stepHintEnterBatFive);
			this._allSteps.Add(this._stepHintToSelectMap);
			this._allSteps.Add(this._stepSelectMap);
			this._allSteps.Add(this._stepLoading);
			this._allSteps.Add(this._stepHintPicFirst);
			this._allSteps.Add(this._stepHintPicFirstGoNext);
			this._allSteps.Add(this._stepHintPicSecond);
			this._allSteps.Add(this._stepHintPicSecondGoNext);
			this._allSteps.Add(this._stepBuyEquipment);
			this._allSteps.Add(this._stepPlayerSelectOneWay);
			this._allSteps.Add(this._stepPlayerSelectWayEnd);
			this._allSteps.Add(this._stepFirstAtkByTower);
			this._allSteps.Add(this._stepFirstAtkByTowerEnd);
			this._allSteps.Add(this._stepFirSeeEnemySoldier);
			this._allSteps.Add(this._stepFirSeeEnemySoldierEnd);
			this._allSteps.Add(this._stepFirSeeEnemyTower);
			this._allSteps.Add(this._stepFirSeeEnemyTowerEnd);
			this._allSteps.Add(this._stepFirInEnemyTowerRange);
			this._allSteps.Add(this._stepFirInEnemyTowerRangeEnd);
			this._allSteps.Add(this._stepFirHpLessEighty);
			this._allSteps.Add(this._stepFirHpLessEightyEnd);
			this._allSteps.Add(this._stepFirHpLessThirty);
			this._allSteps.Add(this._stepFirHpLessThirtyEnd);
			this._allSteps.Add(this._stepFirNearOutShop);
			this._allSteps.Add(this._stepFirNearOutShopEnd);
			this._allSteps.Add(this._stepFirBuyEquipHint);
			this._allSteps.Add(this._stepFirBuyEquipHintEnd);
			this._allSteps.Add(this._stepFirOverMoney);
			this._allSteps.Add(this._stepFirOverMoneyEnd);
			this._allSteps.Add(this._stepFirAtkSecondTower);
			this._allSteps.Add(this._stepFirAtkSecondTowerEnd);
			this._allSteps.Add(this._stepFirInEnemySpring);
			this._allSteps.Add(this._stepFirInEnemySpringEnd);
			this._allSteps.Add(this._stepFirAtkHome);
			this._allSteps.Add(this._stepFirAtkHomeEnd);
			this._allSteps.Add(this._stepLineTowersDead);
			this._allSteps.Add(this._stepLineTowersDeadEnd);
			this._allSteps.Add(this._stepFirNearGrass);
			this._allSteps.Add(this._stepFirNearGrassEnd);
			this._allSteps.Add(this._stepSettlementFir);
			this._allSteps.Add(this._stepSettlementSec);
			this._allSteps.Add(this._stepSettlementThd);
			this._allSteps.Add(this._stepBackHome);
			this._allSteps.Add(this._stepEnd);
		}

		public override void EnterPhase()
		{
			this.InitPhaseResources();
			this._curStep = this._stepHintEnterBatFive;
			this._curStep.HandleAction();
		}

		private void InitPhaseResources()
		{
			NewbieManager.Instance.InitCommonResource();
			NewbieManager.Instance.InitEleBatFiveResource();
			this.LoadSoundBankEleBatFive();
		}

		private void LoadSoundBankEleBatFive()
		{
			AudioMgr.LoadBnk("Guide_B.bnk");
		}
	}
}
