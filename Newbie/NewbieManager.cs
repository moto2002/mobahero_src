using anysdk;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros;
using MobaHeros.Pvp;
using MobaHeros.Pvp.State;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Newbie
{
	internal class NewbieManager
	{
		private const int CObstacleCount = 5;

		private static NewbieManager _inst;

		private bool _isSpecialEnterBattleSuc;

		private NewbieCheckSpecialEnterBattleStat _checkSpecialEnterBattleStat;

		private NewbiePhaseElementaryBattleOneOne _phaseElementaryBattleOneOne = new NewbiePhaseElementaryBattleOneOne();

		private NewbiePhaseElementaryBattleFiveFive _phaseElementaryBattleFiveFive = new NewbiePhaseElementaryBattleFiveFive();

		private NewbiePhaseFakeMatchFive _phaseFakeMatchFive = new NewbiePhaseFakeMatchFive();

		private NewbiePhaseEleHallAct _phaseEleHallAct = new NewbiePhaseEleHallAct();

		private NewbiePhaseNormalCastSkill _phaseNormalCastSkill = new NewbiePhaseNormalCastSkill();

		private NewbiePhaseBase _curPhaseInst;

		private ResourceHandle[] _obstacleEffects = new ResourceHandle[5];

		private Vector3[] _obstaclePoses = new Vector3[]
		{
			new Vector3(-22.22f, 0f, 0f),
			new Vector3(-12.65f, 0f, 0f),
			new Vector3(1.71f, 0f, 0f),
			new Vector3(6.21f, 0f, 0f),
			new Vector3(16.19f, 0f, 0f)
		};

		private Vector3 _hidePos = new Vector3(0f, -999999f, 0f);

		private string _obstacleEffectResId = "Fx_newwall_02";

		private string _moveGuideLineArrowResId = "Fx_introduction_arrowline";

		private GameObject _moveGuideLineArrowResHandle;

		private GameObject _heroInInst;

		private string _selSoldierHintResId = string.Empty;

		private GameObject _selSoldierHintResObj;

		private GameObject _selSoldierHintInst;

		private NewbieCheckMoveNextStep _autoMoveNextStepCtrlObj;

		private NewbieLoopVoiceCtrl _loopVoiceCtrlObj;

		private GameObject _onceVoiceCtrlObj;

		private NewbieCheckInTowerAtkRange _towerAtkRangeCheckObj;

		private NewbieSettlementContinueCtrl _settlementContinueCtrlObj;

		private NewbieMoveGuideLine _newbieMoveGuideLineCtrlObj;

		private bool _isCheckSelEnemyHero;

		private bool _isForbidSelectTower;

		private NewbieEleBatFiveCheckSelWay _newbieEleBatFiveCheckSelWayInst;

		private NewbieEleBatFiveTriggerChecker _newbieEleBatFiveTriggerChecker;

		private NewbieEleBatFiveCheckDelayHpLessThirtyEnd _newbieEleBatFiveCheckDelayHpLessThirtyEnd;

		private List<GameObject> _eleBatFiveLineArrowObjs = new List<GameObject>();

		private GameObject _eleBatFiveFirSeeTowerHintObj;

		private bool _isTriggerFirAtkedByTower;

		private bool _isTriggerFirAtkSecondTower;

		private Vector3 _enemyUpSecondTowerPos = new Vector3(2.34f, 0f, 46.57f);

		private Vector3 _enemyMidSecondTowerPos = new Vector3(19.96f, 0f, 22.67f);

		private Vector3 _enemyDownSecondTowerPos = new Vector3(46.65f, 0f, 2.35f);

		private bool _isTriggerFirAtkHome;

		private bool _isTriggerFirBuyEquipHint;

		private bool _isShowEleBatFiveLearnSkillHint;

		private bool _isEnableEleBatFiveCheckTrigger = true;

		private bool _isFinFakeMatchFiveSelHero;

		private bool _isFinFakeMatchFiveHeroSkill;

		private bool _isFinFakeMatchFiveSummSkill;

		private bool _isFinFakeMatchFiveConfirmSel;

		private bool _isFakeMatchFiveAutoSelHero = true;

		private bool _isFakeMatchFiveHandleNearBattle = true;

		private bool _isEnableFakeMatchFiveTrigger;

		private bool _isFakeMatchFiveTriggerFreeCam;

		private NewbieFakeMatchFiveTriggerChecker _fakeMatchFiveTriggerChecker;

		private NewbieFakeMatchFiveDelayStepEnd _fakeMatchFiveDelayStepEnd;

		private NewbieEleHallActCheckMagicLvThree _eleHallActCheckMagicLvThree;

		private NewbieSubtitleCtrl _newbieSubtitleCtrlInst;

		private bool _isDoPhaseEleHallAct = true;

		private NewbieNormCastCheckLearnSkillFir _normCastCheckLearnSkillFir;

		private bool _isFinNormCastOpenSysSetting;

		private bool _isDoPhaseNormCastSkill;

		public static NewbieManager Instance
		{
			get
			{
				if (NewbieManager._inst == null)
				{
					NewbieManager._inst = new NewbieManager();
				}
				return NewbieManager._inst;
			}
		}

		private NewbieManager()
		{
		}

		public void InitCommonResource()
		{
			PvpStateManager.Instance.ChangeState(new PvpStateNewbieBegin());
		}

		public void InitEleBatOneResource()
		{
			this._heroInInst = null;
			this._moveGuideLineArrowResHandle = ResourceManager.Load<GameObject>(this._moveGuideLineArrowResId, true, true, null, 0, false);
			this._moveGuideLineArrowResHandle.transform.position = this._hidePos;
			this._selSoldierHintResObj = ResourceManager.LoadPath<GameObject>("Prefab/Effects/UIEffect/Fx_introduction_littlearrow", null, 0);
			this._selSoldierHintResObj.transform.position = this._hidePos;
			this.EnableEffectCam();
		}

		public void InitEleBatFiveResource()
		{
			this._isEnableEleBatFiveCheckTrigger = true;
			this._moveGuideLineArrowResHandle = ResourceManager.Load<GameObject>(this._moveGuideLineArrowResId, true, true, null, 0, false);
			this._moveGuideLineArrowResHandle.transform.position = this._hidePos;
			this._selSoldierHintResObj = ResourceManager.LoadPath<GameObject>("Prefab/Effects/UIEffect/Fx_introduction_littlearrow", null, 0);
			this._selSoldierHintResObj.transform.position = this._hidePos;
			this.EnableEffectCam();
		}

		public void InitFakeMatchFiveResource()
		{
			this._isFinFakeMatchFiveSelHero = false;
			this._isFinFakeMatchFiveHeroSkill = false;
			this._isFinFakeMatchFiveSummSkill = false;
			this._isFinFakeMatchFiveConfirmSel = false;
			this._isFakeMatchFiveAutoSelHero = true;
			this._isFakeMatchFiveHandleNearBattle = true;
			this._isEnableFakeMatchFiveTrigger = false;
			this._isFakeMatchFiveTriggerFreeCam = false;
			this.EnableEffectCam();
		}

		public void InitEleHallActResource()
		{
			this._eleHallActCheckMagicLvThree = null;
			this._newbieSubtitleCtrlInst = null;
			this.EnableEffectCam();
		}

		public void InitNormalCastSkillResource()
		{
			this._normCastCheckLearnSkillFir = null;
			this._isFinNormCastOpenSysSetting = false;
			this.EnableEffectCam();
		}

		private void OnEnd()
		{
		}

		public bool IsOpenNewbieGuide()
		{
			return true;
		}

		private bool IsNewbieGuideFinish(int inPhaseType)
		{
			return inPhaseType == 11;
		}

		public bool IsNeedDoNewbieGuide(out ENewbieGuideType outNewbieGuideType, out ENewbiePhaseType outNewbiePhaseType)
		{
			int guide_type = ModelManager.Instance.Get_userData_X().Guide_type;
			int guide_stage = ModelManager.Instance.Get_userData_X().Guide_stage;
			outNewbieGuideType = (ENewbieGuideType)guide_type;
			outNewbiePhaseType = (ENewbiePhaseType)guide_stage;
			return !this.IsNewbieGuideFinish(guide_stage);
		}

		private bool IsNotSelNewbieGuideType(ENewbieGuideType inGuideType)
		{
			return inGuideType == ENewbieGuideType.None;
		}

		public void InitSetSpecialEnterBattleInfo()
		{
			this._isSpecialEnterBattleSuc = false;
		}

		public void SetSpecialEnterBattleSuc()
		{
			this._isSpecialEnterBattleSuc = true;
		}

		public bool IsSpecialEnterBattleSuc()
		{
			return this._isSpecialEnterBattleSuc;
		}

		public void OnSpecialEnterBattleFailed()
		{
			CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("ServerResponse_Title_GameServerDisconnect", "游戏服务器断开"), LanguageManager.Instance.GetStringById("ServerResponse_Content_GameServerDisconnect", "网络出故障了，请重试"), new Action(this.NewbieRestartGame), PopViewType.PopOneButton, "重启游戏", "取消", null);
		}

		private void NewbieRestartGame()
		{
			if (GlobalSettings.isLoginByAnySDK)
			{
				InitSDK.instance.SetAnySDKExtData("4");
				AnySDK.getInstance().release();
			}
			GlobalObject.ReStartGame();
		}

		public void OnSelNewbieGuideType(ENewbieGuideType inGuideType)
		{
		}

		public void NotifyServerNewbieGuidePhase(ENewbiePhaseType inPhaseType)
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.TeachingGuide, null, new object[]
			{
				1,
				(int)inPhaseType
			});
			ModelManager.Instance.Get_userData_X().Guide_stage = (int)inPhaseType;
		}

		public void NotifyServerGuideNormalCastSkill()
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.TeachingGuide, null, new object[]
			{
				2,
				1
			});
			ModelManager.Instance.Get_userData_X().Guide_normalCastSkill = 1;
		}

		public void ProcessNewbieGuide(ENewbieGuideType inGuideType, ENewbiePhaseType inFinishPhaseType)
		{
			this._curPhaseInst = this.GetCurNewbiePhaseByFinishPhaseType(inFinishPhaseType);
			this._curPhaseInst.EnterPhase();
		}

		private NewbiePhaseBase GetCurNewbiePhaseByFinishPhaseType(ENewbiePhaseType inFinishPhaseType)
		{
			if (inFinishPhaseType == ENewbiePhaseType.None)
			{
				return this._phaseElementaryBattleOneOne;
			}
			if (inFinishPhaseType == ENewbiePhaseType.ElementaryBattleOneOne)
			{
				return this._phaseFakeMatchFive;
			}
			if (inFinishPhaseType == ENewbiePhaseType.FakeMatchFive)
			{
				return this._phaseEleHallAct;
			}
			return this._phaseElementaryBattleOneOne;
		}

		public bool IsInNewbieGuide()
		{
			int guide_stage = ModelManager.Instance.Get_userData_X().Guide_stage;
			return !this.IsNewbieGuideFinish(guide_stage);
		}

		public bool IsInNewbiePhase(ENewbiePhaseType inPhaseType)
		{
			return this.IsInNewbieGuide() && this.IsCurPhase(inPhaseType);
		}

		public bool IsInNewbiePhaseEleHallAct()
		{
			return this._isDoPhaseEleHallAct && this.IsCurPhase(ENewbiePhaseType.EleHallAct);
		}

		private bool IsFinNewbieNormalCastSkill()
		{
			int guide_normalCastSkill = ModelManager.Instance.Get_userData_X().Guide_normalCastSkill;
			return guide_normalCastSkill > 0;
		}

		private bool IsNewbieNormalCastSkillHero(string inHeroId)
		{
			return !string.IsNullOrEmpty(inHeroId) && (inHeroId == "Yinjue" || inHeroId == "Xiaohei" || inHeroId == "Shenniu" || inHeroId == "Jiuwei" || inHeroId == "Huonv" || inHeroId == "Bingnv" || inHeroId == "Emowushi");
		}

		public bool IsGuideNewbieNormalCastSkill()
		{
			return !this.IsInNewbieGuide() && this._isDoPhaseNormCastSkill;
		}

		private bool IsInNewbieNormalCastSkill()
		{
			return !this.IsInNewbieGuide() && this._isDoPhaseNormCastSkill && this.IsCurPhase(ENewbiePhaseType.NormalCastSkill);
		}

		public bool IsDoNewbieSpecialProcess()
		{
			return this.IsInNewbieGuide() && (this.IsCurPhase(ENewbiePhaseType.ElementaryBattleOneOne) || this.IsCurPhase(ENewbiePhaseType.ElementaryBattleFiveFive));
		}

		public bool IsDoNewbieSpawnProcess()
		{
			return this.IsInNewbieGuide() && this._curPhaseInst != null && this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleOneOne);
		}

		public bool IsHandleNewbieServerMsg()
		{
			return this.IsInNewbieGuide() && this._curPhaseInst != null && (this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleOneOne) || this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleFiveFive));
		}

		public bool IsForbidShowSkillLevelUp()
		{
			return this.IsInNewbieGuide() && this._curPhaseInst != null && this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleOneOne);
		}

		public bool IsNewbieSysSetting()
		{
			return this.IsInNewbieGuide() && this._curPhaseInst != null && (this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleOneOne) || this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleFiveFive));
		}

		public bool IsShowNewbieSettlementEleBatOne()
		{
			return this.IsInNewbieGuide() && this._curPhaseInst != null && this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleOneOne);
		}

		public bool IsShowNewbieSettlementEleBatFive()
		{
			return this.IsInNewbieGuide() && this._curPhaseInst != null && this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleFiveFive);
		}

		public bool IsShowNewbieSettlementFakeMatchFive()
		{
			return this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive);
		}

		public void TryProcessNewbieGuide()
		{
			ENewbieGuideType inGuideType = ENewbieGuideType.None;
			ENewbiePhaseType inFinishPhaseType = ENewbiePhaseType.None;
			if (this.IsOpenNewbieGuide() && this.IsNeedDoNewbieGuide(out inGuideType, out inFinishPhaseType))
			{
				this.ProcessNewbieGuide(inGuideType, inFinishPhaseType);
			}
		}

		public void EnterGuideNewbieNormalCastSkill()
		{
			this._curPhaseInst = this._phaseNormalCastSkill;
			this._curPhaseInst.EnterPhase();
		}

		public void StartCheckSpecialEnterBattleSuc(float inDelayedTime, ENewbieStepType inMoveStep)
		{
			GameObject gameObject = new GameObject();
			this._checkSpecialEnterBattleStat = gameObject.AddComponent<NewbieCheckSpecialEnterBattleStat>();
			this._checkSpecialEnterBattleStat.StartCheckSpecialEnterBattle(inDelayedTime, inMoveStep);
		}

		public void StopCheckSpecialEnterBattleSuc()
		{
			if (this._checkSpecialEnterBattleStat != null)
			{
				this._checkSpecialEnterBattleStat.StopCheckSpecialEnterBattle();
				UnityEngine.Object.Destroy(this._checkSpecialEnterBattleStat.gameObject);
				this._checkSpecialEnterBattleStat = null;
			}
		}

		public void MoveNextStep()
		{
			if (this._curPhaseInst == null)
			{
				return;
			}
			this._curPhaseInst.DescribeSelf();
			this._curPhaseInst.MoveNextStep();
		}

		public bool MoveCertainStep(ENewbieStepType inStepType, bool inIsCheckCurStep, ENewbieStepType inCheckStepType)
		{
			if (this._curPhaseInst == null)
			{
				return false;
			}
			this._curPhaseInst.DescribeSelf();
			bool flag = this._curPhaseInst.MoveCertainStep(inStepType, inIsCheckCurStep, inCheckStepType);
			if (flag)
			{
			}
			return flag;
		}

		public void AutoMoveNextStepWithDelay(float inDelayTime)
		{
			if (this._autoMoveNextStepCtrlObj != null)
			{
				UnityEngine.Object.Destroy(this._autoMoveNextStepCtrlObj.gameObject);
			}
			GameObject gameObject = new GameObject();
			this._autoMoveNextStepCtrlObj = gameObject.AddComponent<NewbieCheckMoveNextStep>();
			this._autoMoveNextStepCtrlObj.StartAutoMoveNextStepDelay(inDelayTime);
		}

		public void NotifyServerNewbieStep(ENewbieStepType inStepType)
		{
			byte[] msgBody = SerializeHelper.Serialize<NewbieMoveToStepData>(new NewbieMoveToStepData
			{
				stepType = (int)inStepType
			});
			byte[] args = SerializeHelper.Serialize<NewbieInBattleData>(new NewbieInBattleData
			{
				msgType = 5,
				msgBody = msgBody
			});
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_NewbieInBattle, args);
		}

		public void DisplayAllEffectObstacles()
		{
			for (int i = 0; i < 5; i++)
			{
				ResourceHandle resourceHandle = MapManager.Instance.SpawnResourceHandle(this._obstacleEffectResId, null, 0);
				Transform raw = resourceHandle.Raw;
				if (raw != null)
				{
					raw.position = this._obstaclePoses[i];
				}
				this._obstacleEffects[i] = resourceHandle;
			}
		}

		public void DestroyEffectObstacleByIdx(int inIdx)
		{
			if (this._obstacleEffects[inIdx] != null)
			{
				this._obstacleEffects[inIdx].Release();
				this._obstacleEffects[inIdx] = null;
			}
		}

		public GameObject GetMoveGuideLineArrowResHandle()
		{
			return this._moveGuideLineArrowResHandle;
		}

		public void StartCheckSelEnemyHero()
		{
			this._isCheckSelEnemyHero = true;
		}

		public void TryStopCheckSelEnemyHero(Units inTarget)
		{
			if (!this._isCheckSelEnemyHero)
			{
				return;
			}
			if (inTarget != null && TeamManager.CheckTeamType(inTarget.teamType, 1) && TagManager.CheckTag(inTarget, global::TargetTag.Hero))
			{
				this._isCheckSelEnemyHero = false;
				this.MoveNextStep();
			}
		}

		public bool IsForbidSelectTower(Units inTarget)
		{
			return this._isForbidSelectTower && inTarget != null && TagManager.CheckTag(inTarget, global::TargetTag.Tower);
		}

		public void StartCheckForbidSelectTower()
		{
			this._isForbidSelectTower = true;
		}

		public void StopCheckForbidSelectTower()
		{
			this._isForbidSelectTower = false;
		}

		public void StartCheckMoveEnemyTower()
		{
			IList<Units> tower = MapManager.Instance.GetTower(TeamType.BL);
			Units units = tower[0];
			GameObject gameObject = new GameObject();
			this._towerAtkRangeCheckObj = gameObject.AddComponent<NewbieCheckInTowerAtkRange>();
			this._towerAtkRangeCheckObj.StartCheckInTowerAtkRange(PlayerControlMgr.Instance.GetPlayer(), units.mTransform.position, units.attack_range - 0.2f);
		}

		public void StopCheckMoveEnemyTower()
		{
			if (this._towerAtkRangeCheckObj != null)
			{
				this._towerAtkRangeCheckObj.StopCheckInTowerAtkRange();
				UnityEngine.Object.Destroy(this._towerAtkRangeCheckObj.gameObject);
				this._towerAtkRangeCheckObj = null;
			}
		}

		public void PlayVoiceHintOnce(string inAudioResId)
		{
			if (string.IsNullOrEmpty(inAudioResId))
			{
				return;
			}
			this._onceVoiceCtrlObj = new GameObject();
			SysPvpPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPvpPromptVo>(inAudioResId);
			this.NewbiePlayHint(dataById.sound, this._onceVoiceCtrlObj);
		}

		public void StopVoiceHintOnce()
		{
			if (this._onceVoiceCtrlObj != null)
			{
				this.ForceStopVoiceHint(this._onceVoiceCtrlObj);
				UnityEngine.Object.Destroy(this._onceVoiceCtrlObj);
				this._onceVoiceCtrlObj = null;
			}
		}

		public void NewbiePlayHint(string voiceId, GameObject inObj)
		{
			if (AudioMgr.Instance.isVoiceMute())
			{
				return;
			}
			if (string.IsNullOrEmpty(voiceId) || voiceId.Equals("[]"))
			{
				return;
			}
			AudioMgr.Play(voiceId, inObj, false, false);
		}

		public void PlayVoiceHintLoop(string inResId, float inLoopTime)
		{
			if (string.IsNullOrEmpty(inResId))
			{
				return;
			}
			SysPvpPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPvpPromptVo>(inResId);
			GameObject gameObject = new GameObject();
			this._loopVoiceCtrlObj = gameObject.AddComponent<NewbieLoopVoiceCtrl>();
			this._loopVoiceCtrlObj.StartLoopVoice(dataById.sound, inLoopTime);
		}

		public void StopVoiceHintLoop()
		{
			if (this._loopVoiceCtrlObj != null && this._loopVoiceCtrlObj.gameObject != null)
			{
				this.ForceStopVoiceHint(this._loopVoiceCtrlObj.gameObject);
				this._loopVoiceCtrlObj.StopLoopVoice();
				UnityEngine.Object.Destroy(this._loopVoiceCtrlObj.gameObject);
				this._loopVoiceCtrlObj = null;
			}
		}

		public void ForceStopVoiceHint(GameObject inObj)
		{
			AudioMgr.Stop(inObj);
		}

		public void StopAllVoiceHint()
		{
			this.StopVoiceHintOnce();
			this.StopVoiceHintLoop();
		}

		public void StartDelayShowSettlementContinue(float inDelayTime)
		{
			GameObject gameObject = new GameObject();
			this._settlementContinueCtrlObj = gameObject.AddComponent<NewbieSettlementContinueCtrl>();
			this._settlementContinueCtrlObj.StartDelayShowSettlementContinue(inDelayTime);
		}

		public void StopDelayShowSettlementContinue()
		{
			if (this._settlementContinueCtrlObj != null)
			{
				this._settlementContinueCtrlObj.StopDelayShowSettlementContinue();
				UnityEngine.Object.Destroy(this._settlementContinueCtrlObj.gameObject);
				this._settlementContinueCtrlObj = null;
			}
		}

		public void ShowMoveGuideLine(Vector3 inTargetPos)
		{
			GameObject gameObject = new GameObject();
			this._newbieMoveGuideLineCtrlObj = gameObject.AddComponent<NewbieMoveGuideLine>();
			this._newbieMoveGuideLineCtrlObj.ShowMoveGuideLine(inTargetPos);
		}

		public void HideMoveGuideLine()
		{
			if (this._newbieMoveGuideLineCtrlObj != null)
			{
				this._newbieMoveGuideLineCtrlObj.HideMoveGuideLine();
				UnityEngine.Object.Destroy(this._newbieMoveGuideLineCtrlObj.gameObject);
				this._newbieMoveGuideLineCtrlObj = null;
			}
		}

		public void ShowHudView()
		{
			if (Singleton<HUDModuleManager>.Instance.gameObject)
			{
				Singleton<HUDModuleManager>.Instance.FlyIn();
			}
			this.HideSettingBtn();
			this.HideViewLockBtn();
		}

		private void HideSettingBtn()
		{
			if (Singleton<HUDModuleManager>.Instance != null && Singleton<HUDModuleManager>.Instance.gameObject != null && Singleton<HUDModuleManager>.Instance.gameObject.transform != null)
			{
				Transform transform = Singleton<HUDModuleManager>.Instance.gameObject.transform.Find("LowFrequencyRefreshPanel/FunctionBtnsModule(Clone)/Setting");
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
			}
		}

		private void HideViewLockBtn()
		{
			if (Singleton<HUDModuleManager>.Instance != null && Singleton<HUDModuleManager>.Instance.gameObject != null && Singleton<HUDModuleManager>.Instance.gameObject.transform != null)
			{
				Transform transform = Singleton<HUDModuleManager>.Instance.gameObject.transform.Find("LowFrequencyRefreshPanel/FunctionBtnsModule(Clone)/DragLock");
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
			}
		}

		private void HideVoiceBtn()
		{
			if (Singleton<BarrageEmitterView>.Instance != null && Singleton<BarrageEmitterView>.Instance.talkToggle != null)
			{
				Singleton<BarrageEmitterView>.Instance.talkToggle.gameObject.SetActive(false);
			}
		}

		public void HideHudView()
		{
			if (Singleton<HUDModuleManager>.Instance.gameObject)
			{
				Singleton<HUDModuleManager>.Instance.FlyOut();
			}
		}

		public void EleBatFiveHideHudViewComponents()
		{
			this.HideSettingBtn();
			this.HideViewLockBtn();
			this.HideVoiceBtn();
		}

		public void ShowEleBatFiveLearnSkillHint(int inSkillIdx)
		{
			if (Singleton<SkillView>.Instance == null)
			{
				return;
			}
			if (!Singleton<SkillView>.Instance.NewbieCheckIsSkillCanLevelUp(inSkillIdx))
			{
				return;
			}
			this._isShowEleBatFiveLearnSkillHint = true;
			this.DisableEleBatFiveTriggerCheck();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowEleBatFiveLearnSkillHint(inSkillIdx);
		}

		public void TryHideEleBatFiveLearnSkillHint()
		{
			if (this.IsInNewbieGuide() && this._curPhaseInst != null && this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleFiveFive) && this._isShowEleBatFiveLearnSkillHint)
			{
				this._isShowEleBatFiveLearnSkillHint = false;
				this.EnableEleBatFiveTriggerCheck();
				Singleton<NewbieView>.Instance.HideEleBatFiveLearnSkillHint();
				CtrlManager.CloseWindow(WindowID.NewbieView);
			}
		}

		public void SetEleBatFiveFirSeeTowerHintObj(GameObject inObj)
		{
			this._eleBatFiveFirSeeTowerHintObj = inObj;
		}

		public void DestroyEleBatFiveFirSeeTowerHintObj()
		{
			if (this._eleBatFiveFirSeeTowerHintObj != null)
			{
				UnityEngine.Object.Destroy(this._eleBatFiveFirSeeTowerHintObj);
				this._eleBatFiveFirSeeTowerHintObj = null;
			}
		}

		public void ShowHeroIn()
		{
		}

		public void HideHeroIn()
		{
			if (this._heroInInst != null)
			{
				this._heroInInst.transform.position = new Vector3(999f, 999f, 999f);
				Camera componentInChildren = this._heroInInst.GetComponentInChildren<Camera>();
				if (componentInChildren != null)
				{
					componentInChildren.enabled = false;
				}
				Animator[] componentsInChildren = this._heroInInst.GetComponentsInChildren<Animator>(true);
				if (componentsInChildren != null)
				{
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						if (componentsInChildren[i] != null)
						{
							componentsInChildren[i].enabled = false;
						}
					}
				}
			}
		}

		public void ShowSelSoldierHint()
		{
			Vector3 position = new Vector3(-1.54f, 0f, -0.44f);
			this._selSoldierHintInst = (UnityEngine.Object.Instantiate(this._selSoldierHintResObj, position, Quaternion.identity) as GameObject);
		}

		public void HideSelSoldierHint()
		{
			if (this._selSoldierHintInst != null)
			{
				UnityEngine.Object.Destroy(this._selSoldierHintInst);
				this._selSoldierHintInst = null;
			}
		}

		public void EleBatFiveCreateForbidDoorEffect()
		{
			Vector3[] array = new Vector3[]
			{
				new Vector3(-36.95f, -21.44f, -71.12f),
				new Vector3(-20.98f, -37.17f, -13f)
			};
			for (int i = 0; i < array.Length; i++)
			{
				ResourceHandle resourceHandle = MapManager.Instance.SpawnResourceHandle(this._obstacleEffectResId, null, 0);
				Transform raw = resourceHandle.Raw;
				if (raw != null)
				{
					raw.position = new Vector3(array[i].x, 0f, array[i].y);
					raw.rotation = Quaternion.Euler(0f, array[i].z, 0f);
				}
			}
		}

		public void EleBatFiveStartCheckSelOneWay()
		{
			GameObject gameObject = new GameObject();
			this._newbieEleBatFiveCheckSelWayInst = gameObject.AddComponent<NewbieEleBatFiveCheckSelWay>();
			this._newbieEleBatFiveCheckSelWayInst.StartCheckSelWay();
		}

		public void EleBatFiveStopCheckSelOneWay()
		{
			if (this._newbieEleBatFiveCheckSelWayInst != null)
			{
				this._newbieEleBatFiveCheckSelWayInst.StopCheckSelWay();
				UnityEngine.Object.Destroy(this._newbieEleBatFiveCheckSelWayInst.gameObject);
				this._newbieEleBatFiveCheckSelWayInst = null;
			}
		}

		public void EleBatFiveStartTriggerChecker()
		{
			GameObject gameObject = new GameObject();
			this._newbieEleBatFiveTriggerChecker = gameObject.AddComponent<NewbieEleBatFiveTriggerChecker>();
			this._newbieEleBatFiveTriggerChecker.StartCheckTrigger();
		}

		public void EleBatFiveStopTriggerChecker()
		{
			if (this._newbieEleBatFiveTriggerChecker != null)
			{
				this._newbieEleBatFiveTriggerChecker.StopCheckTrigger();
				UnityEngine.Object.Destroy(this._newbieEleBatFiveTriggerChecker.gameObject);
				this._newbieEleBatFiveTriggerChecker = null;
			}
		}

		public void StartEleBatFiveDelayHpLessThirtyEnd(float inDelayTime)
		{
			GameObject gameObject = new GameObject();
			this._newbieEleBatFiveCheckDelayHpLessThirtyEnd = gameObject.AddComponent<NewbieEleBatFiveCheckDelayHpLessThirtyEnd>();
			this._newbieEleBatFiveCheckDelayHpLessThirtyEnd.StartDelayMoveHpLessThirtyEnd(inDelayTime);
		}

		public void StopEleBatFiveDelayHpLessThirtyEnd()
		{
			if (this._newbieEleBatFiveCheckDelayHpLessThirtyEnd != null)
			{
				this._newbieEleBatFiveCheckDelayHpLessThirtyEnd.StopDelayMoveHpLessThirtyEnd();
			}
		}

		public void DestroyEleBatFiveDelayHpLessThirtyEnd()
		{
			if (this._newbieEleBatFiveCheckDelayHpLessThirtyEnd != null)
			{
				this._newbieEleBatFiveCheckDelayHpLessThirtyEnd.StopDelayMoveHpLessThirtyEnd();
				UnityEngine.Object.Destroy(this._newbieEleBatFiveCheckDelayHpLessThirtyEnd.gameObject);
				this._newbieEleBatFiveCheckDelayHpLessThirtyEnd = null;
			}
		}

		public void EnableEleBatFiveTriggerCheck()
		{
			this._isEnableEleBatFiveCheckTrigger = true;
		}

		public void DisableEleBatFiveTriggerCheck()
		{
			this._isEnableEleBatFiveCheckTrigger = false;
		}

		public bool IsEnableEleBatFiveCheckTrigger()
		{
			return this._isEnableEleBatFiveCheckTrigger;
		}

		public void ShowEleBatFiveLineHint()
		{
			Vector3[] array = new Vector3[]
			{
				new Vector3(-48.32f, 0.5f, -41.94f),
				new Vector3(-48.32f, 0.5f, 30.2f),
				new Vector3(-28.9f, 0.5f, 48.7f),
				new Vector3(42f, 0.5f, 48.7f)
			};
			Vector3[] array2 = new Vector3[]
			{
				new Vector3(-42.73f, 0.5f, -42.86f),
				new Vector3(42.73f, 0.5f, 42.86f)
			};
			Vector3[] array3 = new Vector3[]
			{
				new Vector3(-41.94f, 0.5f, -48.32f),
				new Vector3(30.2f, 0.5f, -48.32f),
				new Vector3(48.7f, 0.5f, -28.9f),
				new Vector3(48.7f, 0.5f, 42f)
			};
			for (int i = 0; i < array.Length - 1; i++)
			{
				this.GenLineHintBetweenPoints(array[i], array[i + 1], this._eleBatFiveLineArrowObjs);
			}
			for (int j = 0; j < array2.Length - 1; j++)
			{
				this.GenLineHintBetweenPoints(array2[j], array2[j + 1], this._eleBatFiveLineArrowObjs);
			}
			for (int k = 0; k < array3.Length - 1; k++)
			{
				this.GenLineHintBetweenPoints(array3[k], array3[k + 1], this._eleBatFiveLineArrowObjs);
			}
		}

		private void GenLineHintBetweenPoints(Vector3 inStartPos, Vector3 inEndPos, List<GameObject> outArrowObjs)
		{
			if (this._moveGuideLineArrowResHandle == null)
			{
				return;
			}
			float num = 3.5f;
			float magnitude = (inEndPos - inStartPos).magnitude;
			int num2 = Mathf.FloorToInt(magnitude / num) + 1;
			Vector3 normalized = (inEndPos - inStartPos).normalized;
			for (int i = 0; i < num2; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this._moveGuideLineArrowResHandle) as GameObject;
				if (gameObject != null)
				{
					gameObject.transform.position = inStartPos + normalized * (float)i * num;
					gameObject.transform.LookAt(inEndPos);
					outArrowObjs.Add(gameObject);
				}
			}
		}

		public void ClearEleBatFiveLineResources()
		{
			for (int i = 0; i < this._eleBatFiveLineArrowObjs.Count; i++)
			{
				GameObject gameObject = this._eleBatFiveLineArrowObjs[i];
				if (gameObject != null)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			this._eleBatFiveLineArrowObjs.Clear();
		}

		public void OnUnitWounded(Units inTarget, Units inAttacker)
		{
			if (this.IsInNewbieGuide() && this._curPhaseInst != null && this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleFiveFive))
			{
				Units inPlayerHero = null;
				if (PlayerControlMgr.Instance != null)
				{
					inPlayerHero = PlayerControlMgr.Instance.GetPlayer();
				}
				if (this.IsEnableEleBatFiveCheckTrigger())
				{
					this.TryTriggerFirstAttackedByTower(inTarget, inAttacker, inPlayerHero);
					this.TryTriggerFirAttackHome(inTarget, inAttacker, inPlayerHero);
				}
			}
		}

		private void TryTriggerFirstAttackedByTower(Units inTarget, Units inAttacker, Units inPlayerHero)
		{
			if (this._isTriggerFirAtkedByTower)
			{
				return;
			}
			if (inPlayerHero == null || inTarget == null || inAttacker == null)
			{
				return;
			}
			if (inPlayerHero.unique_id != inTarget.unique_id)
			{
				return;
			}
			if (!TagManager.CheckTag(inAttacker, global::TargetTag.Tower))
			{
				return;
			}
			this.MoveCertainStep(ENewbieStepType.EleBatFive_FirAtkByTower, false, ENewbieStepType.None);
			this._isTriggerFirAtkedByTower = true;
		}

		private void TryTriggerFirAttackSecondTower(Units inTarget, Units inAttacker, Units inPlayerHero)
		{
			if (this._isTriggerFirAtkSecondTower)
			{
				return;
			}
			if (inTarget == null || inAttacker == null || inPlayerHero == null)
			{
				return;
			}
			if (inAttacker.unique_id != inPlayerHero.unique_id)
			{
				return;
			}
			if (!TagManager.CheckTag(inTarget, global::TargetTag.Tower))
			{
				return;
			}
			if (inTarget.mTransform == null)
			{
				return;
			}
			if ((inTarget.mTransform.position - this._enemyUpSecondTowerPos).sqrMagnitude < 1f || (inTarget.mTransform.position - this._enemyMidSecondTowerPos).sqrMagnitude < 1f || (inTarget.mTransform.position - this._enemyDownSecondTowerPos).sqrMagnitude < 1f)
			{
				this.MoveCertainStep(ENewbieStepType.EleBatFive_FirAtkSecondTower, false, ENewbieStepType.None);
				this._isTriggerFirAtkSecondTower = true;
			}
		}

		private void TryTriggerFirAttackHome(Units inTarget, Units inAttacker, Units inPlayerHero)
		{
			if (this._isTriggerFirAtkHome)
			{
				return;
			}
			if (inTarget == null || inAttacker == null || inPlayerHero == null)
			{
				return;
			}
			if (inAttacker.unique_id != inPlayerHero.unique_id)
			{
				return;
			}
			if (!TagManager.CheckTag(inTarget, global::TargetTag.Home))
			{
				return;
			}
			this.MoveCertainStep(ENewbieStepType.EleBatFive_FirAtkHome, false, ENewbieStepType.None);
			this._isTriggerFirAtkHome = true;
		}

		public void TryTriggerBuyEquipHint()
		{
			if (this.IsInNewbieGuide() && this._curPhaseInst != null && this._curPhaseInst.IsPhase(ENewbiePhaseType.ElementaryBattleFiveFive))
			{
				if (!this.IsEnableEleBatFiveCheckTrigger())
				{
					return;
				}
				if (!this._isTriggerFirBuyEquipHint)
				{
					this.MoveCertainStep(ENewbieStepType.EleBatFive_FirBuyEquipHint, false, ENewbieStepType.None);
					this._isTriggerFirBuyEquipHint = true;
				}
			}
		}

		public void EnableFakeMatchFiveTrigger()
		{
			this._isEnableFakeMatchFiveTrigger = true;
		}

		public void DisableFakeMatchFiveTrigger()
		{
			this._isEnableFakeMatchFiveTrigger = false;
		}

		public bool IsEnableFakeMatchFiveTrigger()
		{
			return this._isEnableFakeMatchFiveTrigger;
		}

		public void StartFakeMatchFiveCheckFirHpLessNintyEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StartCheckFirHpLessNintyEnd();
			}
		}

		public void StopFakeMatchFiveCheckFirHpLessNintyEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StopCheckFirHpLessNintyEnd();
			}
		}

		public void StartFakeMatchFiveCheckFirHpLessThirtyEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StartCheckFirHpLessThirtyEnd();
			}
		}

		public void StopFakeMatchFiveCheckFirHpLessThirtyEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StopCheckFirHpLessThirtyEnd();
			}
		}

		public void StartFakeMatchFiveCheckFirSelUpWayEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StartCheckFirSelUpWayEnd();
			}
		}

		public void StopFakeMatchFiveCheckFirSelUpWayEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StopCheckFirSelUpWayEnd();
			}
		}

		public void StartFakeMatchFiveCheckFirSelDownWayEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StartCheckFirSelDownWayEnd();
			}
		}

		public void StopFakeMatchFiveCheckFirSelDownWayEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StopCheckFirSelDownWayEnd();
			}
		}

		public void StartFakeMatchFiveCheckFirHeroDeadEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StartCheckFirHeroDeadEnd();
			}
		}

		public void StopFakeMatchFiveCheckFirHeroDeadEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StopCheckFirHeroDeadEnd();
			}
		}

		public void StartFakeMatchFiveCheckFirNearFirTowerEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StartCheckFirNearFirTowerEnd();
			}
		}

		public void StopFakeMatchFiveCheckFirNearFirTowerEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StopCheckFirNearFirTowerEnd();
			}
		}

		public void StartFakeMatchFiveCheckFirFreeCamEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StartCheckFirFreeCamEnd();
			}
		}

		public void StopFakeMatchFiveCheckFirFreeCamEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				this._fakeMatchFiveDelayStepEnd.StopCheckFirFreeCamEnd();
			}
		}

		public void FakeMatchFiveStartCheckTrigger()
		{
			GameObject gameObject = new GameObject();
			this._fakeMatchFiveTriggerChecker = gameObject.AddComponent<NewbieFakeMatchFiveTriggerChecker>();
		}

		public void FakeMatchFiveStopCheckTrigger()
		{
			if (this._fakeMatchFiveTriggerChecker != null)
			{
				UnityEngine.Object.Destroy(this._fakeMatchFiveTriggerChecker.gameObject);
				this._fakeMatchFiveTriggerChecker = null;
			}
		}

		public void FakeMatchFiveCreateDelayStepEnd()
		{
			GameObject gameObject = new GameObject();
			this._fakeMatchFiveDelayStepEnd = gameObject.AddComponent<NewbieFakeMatchFiveDelayStepEnd>();
		}

		public void FakeMatchFiveDestroyDelayStepEnd()
		{
			if (this._fakeMatchFiveDelayStepEnd != null)
			{
				UnityEngine.Object.Destroy(this._fakeMatchFiveDelayStepEnd.gameObject);
				this._fakeMatchFiveDelayStepEnd = null;
			}
		}

		public void EleHallActStartCheckMagicLvThree()
		{
			GameObject gameObject = new GameObject();
			this._eleHallActCheckMagicLvThree = gameObject.AddComponent<NewbieEleHallActCheckMagicLvThree>();
		}

		public void EleHallActStopCheckMagicLvThree()
		{
			if (this._eleHallActCheckMagicLvThree != null)
			{
				this._eleHallActCheckMagicLvThree.StopCheckMagicBottleLvThree();
			}
		}

		public void EleHallActDestroyCheckMagicLvThree()
		{
			if (this._eleHallActCheckMagicLvThree != null)
			{
				this._eleHallActCheckMagicLvThree.StopCheckMagicBottleLvThree();
				UnityEngine.Object.Destroy(this._eleHallActCheckMagicLvThree.gameObject);
				this._eleHallActCheckMagicLvThree = null;
			}
		}

		public void CreateSubtitleCtrl()
		{
			GameObject gameObject = new GameObject();
			this._newbieSubtitleCtrlInst = gameObject.AddComponent<NewbieSubtitleCtrl>();
		}

		public void DestroySubtitleCtrl()
		{
			if (this._newbieSubtitleCtrlInst != null)
			{
				UnityEngine.Object.Destroy(this._newbieSubtitleCtrlInst.gameObject);
				this._newbieSubtitleCtrlInst = null;
			}
		}

		public void StartDisplaySubtitle(NewbieSubtitleData inSubtitleData)
		{
			if (this._newbieSubtitleCtrlInst != null)
			{
				this._newbieSubtitleCtrlInst.StartUpdateSubtitle(inSubtitleData);
			}
		}

		public void StopDisplaySubtitle()
		{
			if (this._newbieSubtitleCtrlInst != null)
			{
				this._newbieSubtitleCtrlInst.StopUpdateSubtitle();
			}
		}

		public void DoDisplaySubtitle(string inLanguageId, int inSubtitleProcessIdx, int inSubtitleProcessLength)
		{
			if (string.IsNullOrEmpty(inLanguageId))
			{
				return;
			}
			SysLanguageVo languageData = BaseDataMgr.instance.GetLanguageData(inLanguageId);
			if (languageData == null)
			{
				return;
			}
			string inProcess = string.Concat(new string[]
			{
				"(",
				inSubtitleProcessIdx.ToString(),
				"/",
				inSubtitleProcessLength.ToString(),
				")"
			});
			if (Singleton<NewbieView>.Instance != null)
			{
				Singleton<NewbieView>.Instance.ShowScreenSubtitle(languageData.content, inProcess);
			}
		}

		public void HideSubtitle()
		{
			if (Singleton<NewbieView>.Instance != null)
			{
				Singleton<NewbieView>.Instance.HideScreenSubtitle();
			}
		}

		public void NormCastStartCheckLearnSkillFir()
		{
			GameObject gameObject = new GameObject();
			this._normCastCheckLearnSkillFir = gameObject.AddComponent<NewbieNormCastCheckLearnSkillFir>();
			this._normCastCheckLearnSkillFir.StartCheckLearnSkillFir();
		}

		public void NormCastStopCheckLearnSkillFir()
		{
			if (this._normCastCheckLearnSkillFir != null)
			{
				this._normCastCheckLearnSkillFir.StopCheckLearnSkillFir();
				UnityEngine.Object.Destroy(this._normCastCheckLearnSkillFir.gameObject);
				this._normCastCheckLearnSkillFir = null;
			}
		}

		public void ClearPhaseEleBatOneOneResources()
		{
			this.UnloadSoundBankEleBatOne();
			this._moveGuideLineArrowResHandle = null;
			this._heroInInst = null;
			this._selSoldierHintResObj = null;
			this._selSoldierHintInst = null;
			this._autoMoveNextStepCtrlObj = null;
			this._loopVoiceCtrlObj = null;
			this._towerAtkRangeCheckObj = null;
			this._settlementContinueCtrlObj = null;
			this._newbieMoveGuideLineCtrlObj = null;
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.HideMask();
			Singleton<NewbieView>.Instance.ClearResources();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public void ClearPhaseEleBatFiveResources()
		{
			this.UnloadSoundBankEleBatFive();
			this._eleBatFiveLineArrowObjs.Clear();
		}

		public void ClearPhaseFakeMatchFiveResources()
		{
			this.UnloadSoundBankFakeMatchFive();
		}

		public void ClearPhaseEleHallActResources()
		{
			this.UnloadSoundBankEleHallAct();
		}

		public void ClearPhaseNormCastResources()
		{
		}

		private void UnloadSoundBankEleBatOne()
		{
			AudioMgr.UnLoadBnk("Guide_A.bnk", false);
		}

		private void UnloadSoundBankEleBatFive()
		{
			AudioMgr.UnLoadBnk("Guide_B.bnk", false);
		}

		private void UnloadSoundBankFakeMatchFive()
		{
			AudioMgr.UnLoadBnk("Guide_C.bnk", false);
		}

		private void UnloadSoundBankEleHallAct()
		{
			AudioMgr.UnLoadBnk("Guide_D.bnk", false);
		}

		public void TryForceHidePartNewbieView()
		{
			if (CtrlManager.IsWindowOpen(WindowID.NewbieView) && Singleton<NewbieView>.Instance != null)
			{
				Singleton<NewbieView>.Instance.TryForceHideEleBatOneSkillHint();
				Singleton<NewbieView>.Instance.TryForceHideEleBatFiveSkillHint();
				Singleton<NewbieView>.Instance.TryForceHideFakeMatchFiveHint();
				Singleton<NewbieView>.Instance.TryForceHideNormCastSkillHint();
			}
		}

		public void TryHandleSelectHero()
		{
			if (!this.IsInNewbieGuide())
			{
				return;
			}
			if (!this.IsCurPhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			if (this._isFinFakeMatchFiveSelHero)
			{
				return;
			}
			this._isFinFakeMatchFiveSelHero = true;
			this._isFakeMatchFiveAutoSelHero = false;
			if (this.IsCurStep(ENewbieStepType.FakeMatchFive_HintSelHero))
			{
				if (!this._isFinFakeMatchFiveHeroSkill)
				{
					this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintHeroSkill, false, ENewbieStepType.None);
				}
				else if (!this._isFinFakeMatchFiveSummSkill)
				{
					this.MoveCertainStep(ENewbieStepType.FakeMatchFive_OpenSummonerSkill, false, ENewbieStepType.None);
				}
				else if (!this._isFinFakeMatchFiveConfirmSel)
				{
					this.MoveCertainStep(ENewbieStepType.FakeMatchFive_SelHeroConfirm, false, ENewbieStepType.None);
				}
				else
				{
					this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintSelHeroEnd, false, ENewbieStepType.None);
				}
			}
		}

		public void TryHandleHeroSkillIntroduction()
		{
			if (!this.IsInNewbieGuide())
			{
				return;
			}
			if (!this.IsCurPhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			if (this._isFinFakeMatchFiveHeroSkill)
			{
				return;
			}
			this._isFinFakeMatchFiveHeroSkill = true;
			if (this.IsCurStep(ENewbieStepType.FakeMatchFive_HintHeroSkill))
			{
				if (!this._isFinFakeMatchFiveSummSkill)
				{
					this.MoveCertainStep(ENewbieStepType.FakeMatchFive_OpenSummonerSkill, false, ENewbieStepType.None);
				}
				else if (!this._isFinFakeMatchFiveConfirmSel)
				{
					this.MoveCertainStep(ENewbieStepType.FakeMatchFive_SelHeroConfirm, false, ENewbieStepType.None);
				}
				else
				{
					this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintHeroSkillEnd, false, ENewbieStepType.None);
				}
			}
		}

		public void TryHandleOpenSummonerSkill()
		{
			if (!this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			this._isFinFakeMatchFiveSummSkill = true;
			if (this.IsCurStep(ENewbieStepType.FakeMatchFive_OpenSummonerSkill))
			{
				this.MoveCertainStep(ENewbieStepType.FakeMatchFive_OpenSummSkillEnd, false, ENewbieStepType.None);
			}
			else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_HintSelHero))
			{
				this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintSelHeroEnd, false, ENewbieStepType.None);
			}
			else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_HintHeroSkill))
			{
				this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintHeroSkillEnd, false, ENewbieStepType.None);
			}
		}

		public void TryHandleSelSummonerSkill()
		{
			if (!this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			if (this.IsCurStep(ENewbieStepType.FakeMatchFive_OpenSummSkillEnd))
			{
				if (!this._isFinFakeMatchFiveConfirmSel)
				{
					this.MoveCertainStep(ENewbieStepType.FakeMatchFive_SelHeroConfirm, false, ENewbieStepType.None);
				}
			}
			else if (!this._isFinFakeMatchFiveSelHero)
			{
				this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintSelHero, false, ENewbieStepType.None);
			}
			else if (!this._isFinFakeMatchFiveHeroSkill)
			{
				this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintHeroSkill, false, ENewbieStepType.None);
			}
		}

		public void TryHandleSelHeroNearOverTime()
		{
			if (!this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			if (!this._isFakeMatchFiveAutoSelHero)
			{
				return;
			}
			this._isFakeMatchFiveAutoSelHero = false;
			ReadyPlayerSampleInfo readyPlayerSampleInfo = Singleton<PvpManager>.Instance.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == Singleton<PvpManager>.Instance.MyLobbyUserId);
			if (readyPlayerSampleInfo == null)
			{
				return;
			}
			string value = string.Empty;
			if (readyPlayerSampleInfo.heroInfo != null)
			{
				value = readyPlayerSampleInfo.heroInfo.heroId;
			}
			if (!string.IsNullOrEmpty(value))
			{
				return;
			}
			List<ReadyPlayerSampleInfo> ourPlayers = Singleton<PvpManager>.Instance.OurPlayers;
			if (ourPlayers == null)
			{
				return;
			}
			List<string> list = new List<string>();
			for (int i = 0; i < ourPlayers.Count; i++)
			{
				ReadyPlayerSampleInfo readyPlayerSampleInfo2 = ourPlayers[i];
				if (readyPlayerSampleInfo2 != null && readyPlayerSampleInfo2.heroInfo != null && !string.IsNullOrEmpty(readyPlayerSampleInfo2.heroInfo.heroId))
				{
					list.Add(readyPlayerSampleInfo2.heroInfo.heroId);
				}
			}
			List<string> list2 = new List<string>();
			if (CharacterDataMgr.instance.OwenHeros != null)
			{
				list2.AddRange(CharacterDataMgr.instance.OwenHeros);
			}
			string text = string.Empty;
			if (Singleton<PvpManager>.Instance.freeHeros != null)
			{
				for (int j = 0; j < Singleton<PvpManager>.Instance.freeHeros.Count; j++)
				{
					text = Singleton<PvpManager>.Instance.freeHeros[j].Split(new char[]
					{
						','
					})[0];
					if (!string.IsNullOrEmpty(text) && !list2.Contains(text))
					{
						list2.Add(text);
					}
				}
			}
			bool flag = false;
			string text2 = string.Empty;
			for (int k = 0; k < list2.Count; k++)
			{
				if (!string.IsNullOrEmpty(list2[k]) && !list.Contains(list2[k]))
				{
					flag = true;
					text2 = list2[k];
					break;
				}
			}
			if (flag && Singleton<PvpSelectHeroView>.Instance != null)
			{
				Singleton<PvpSelectHeroView>.Instance.NewbieAutoSelHero(text2);
				Singleton<PvpSelectHeroView>.Instance.ShowHeroInfo(text2);
			}
		}

		public void TryHandleNearEnterBattle()
		{
			if (!this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			if (!this._isFakeMatchFiveHandleNearBattle)
			{
				return;
			}
			this._isFakeMatchFiveHandleNearBattle = false;
			this.NotifyServerNewbieGuidePhase(ENewbiePhaseType.FakeMatchFive);
		}

		public void TryHandleConfirmSelHero()
		{
			if (!this.IsInNewbieGuide())
			{
				return;
			}
			if (!this.IsCurPhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			if (this._isFinFakeMatchFiveConfirmSel)
			{
				return;
			}
			this._isFinFakeMatchFiveConfirmSel = true;
			this.MoveCertainStep(ENewbieStepType.FakeMatchFive_SelHeroConfirmEnd, true, ENewbieStepType.FakeMatchFive_SelHeroConfirm);
		}

		public void TryHandleCamDragLock()
		{
			if (!this.IsInNewbieGuide())
			{
				return;
			}
			if (!this.IsCurPhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			if (!this._isFakeMatchFiveTriggerFreeCam)
			{
				this._isFakeMatchFiveTriggerFreeCam = true;
				this.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirFreeCam, false, ENewbieStepType.None);
			}
		}

		public void TryHandleMatchedSuc()
		{
			if (!this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintMatch, false, ENewbieStepType.None);
		}

		public void TryHandleOpenSelHeroView()
		{
			if (!this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				return;
			}
			this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintSelHero, false, ENewbieStepType.None);
		}

		public void TryHandleOnOpenBottleSystem()
		{
			if (!this.IsInNewbiePhaseEleHallAct())
			{
				return;
			}
			this.MoveCertainStep(ENewbieStepType.EleHallAct_MagicBottleIntro, false, ENewbieStepType.None);
		}

		public void TryHandleGetItemsFinished()
		{
			if (this.IsInNewbiePhaseEleHallAct())
			{
				this.HandlePhaseEleHallActFinGetItems();
			}
		}

		private void HandlePhaseEleHallActFinGetItems()
		{
			if (this.IsCurStep(ENewbieStepType.EleHallAct_AchieveAwardEnd))
			{
				this.MoveCertainStep(ENewbieStepType.EleHallAct_AchieveBack, false, ENewbieStepType.None);
			}
			else if (this.IsCurStep(ENewbieStepType.EleHallAct_DailyAwardEnd))
			{
				this.MoveCertainStep(ENewbieStepType.EleHallAct_DoDailyHint, false, ENewbieStepType.None);
			}
			else if (this.IsCurStep(ENewbieStepType.EleHallAct_MagicBottleTaleEnd))
			{
				this.MoveCertainStep(ENewbieStepType.EleHallAct_MagicBottleAwd, false, ENewbieStepType.None);
			}
			else if (this.IsCurStep(ENewbieStepType.EleHallAct_NewbieActAwdEnd))
			{
				this.MoveCertainStep(ENewbieStepType.EleHallAct_OpenLoginAwd, false, ENewbieStepType.None);
			}
			else if (this.IsCurStep(ENewbieStepType.EleHallAct_GetLoginAwdEnd))
			{
				this.MoveCertainStep(ENewbieStepType.EleHallAct_CloseActivity, false, ENewbieStepType.None);
			}
		}

		public void TryHandleOpenSysSetting()
		{
			if (this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHpLessNinty))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHpLessThirty))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHeroDead))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirNearFirTower))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirFreeCam))
				{
					this.DisableEffectCam();
				}
			}
			else if (this.IsInNewbieNormalCastSkill())
			{
				if (this.IsCurStep(ENewbieStepType.NormCast_OpenSysSetting))
				{
					if (!this._isFinNormCastOpenSysSetting)
					{
						this._isFinNormCastOpenSysSetting = true;
						this.MoveCertainStep(ENewbieStepType.NormCast_SetNormalCast, false, ENewbieStepType.None);
					}
				}
				else if (this.IsCurStep(ENewbieStepType.NormCast_UseSkillFir))
				{
					this.DisableEffectCam();
				}
			}
		}

		public void TryHandleSetCastSkillMode(bool inIsCrazyMode)
		{
			if (this.IsInNewbieNormalCastSkill())
			{
				this.MoveCertainStep(ENewbieStepType.NormCast_CloseSysSetting, true, ENewbieStepType.NormCast_SetNormalCast);
			}
		}

		public void TryHandleSkillPanelSet()
		{
			if (this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				this.MoveCertainStep(ENewbieStepType.FakeMatchFive_SkillPanelSetEnd, true, ENewbieStepType.FakeMatchFive_SkillPanelSet);
			}
		}

		public void TryHandleCloseSysSetting()
		{
			if (this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				this.EnableEffectCam();
				if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHpLessNinty) && Singleton<NewbieView>.Instance != null)
				{
					Singleton<NewbieView>.Instance.AjustFakeMatchFiveUseRecoveryHintPos();
				}
				this.MoveCertainStep(ENewbieStepType.FakeMatchFive_SkillPanelSetEnd, true, ENewbieStepType.FakeMatchFive_SkillPanelSet);
			}
			else if (this.IsInNewbieNormalCastSkill())
			{
				if (this.IsCurStep(ENewbieStepType.NormCast_SetNormalCast))
				{
					this.MoveCertainStep(ENewbieStepType.NormCast_CloseSysSettingEnd, false, ENewbieStepType.None);
				}
				else if (this.IsCurStep(ENewbieStepType.NormCast_CloseSysSetting))
				{
					if (ModelManager.Instance.Get_SettingData().crazyCastingSkill)
					{
						this.MoveCertainStep(ENewbieStepType.NormCast_CloseSysSettingEnd, false, ENewbieStepType.None);
					}
					else
					{
						this.MoveCertainStep(ENewbieStepType.NormCast_UseSkillFir, false, ENewbieStepType.None);
					}
				}
				else if (this.IsCurStep(ENewbieStepType.NormCast_ClickGround) && ModelManager.Instance.Get_SettingData().crazyCastingSkill)
				{
					this.MoveCertainStep(ENewbieStepType.NormCast_End, false, ENewbieStepType.None);
				}
				this.EnableEffectCam();
				if (this.IsCurStep(ENewbieStepType.NormCast_UseSkillFir) && Singleton<NewbieView>.Instance != null)
				{
					Singleton<NewbieView>.Instance.AdjustNormCastUseSkillFirPos();
				}
			}
		}

		public void TryHandleUseSkill(int inIndex)
		{
			if (this.IsInNewbieNormalCastSkill() && inIndex == 0 && this.IsCurStep(ENewbieStepType.NormCast_UseSkillFir))
			{
				if (!ModelManager.Instance.Get_SettingData().crazyCastingSkill)
				{
					this.MoveCertainStep(ENewbieStepType.NormCast_ClickGround, false, ENewbieStepType.None);
				}
				else
				{
					this.MoveCertainStep(ENewbieStepType.NormCast_End, false, ENewbieStepType.None);
				}
			}
		}

		public void TryHandleNotifyServerUseSkill()
		{
			if (this.IsInNewbieNormalCastSkill() && this.IsCurStep(ENewbieStepType.NormCast_ClickGround))
			{
				this.MoveCertainStep(ENewbieStepType.NormCast_DoubleClick, false, ENewbieStepType.None);
			}
		}

		public void TryHandleOpenShop()
		{
			if (this.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleOneOne))
			{
				if (this.IsCurStep(ENewbieStepType.ElementaryBatOneOne_UseSkillFourth))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.ElementaryBatOneOne_UseSkillFirst))
				{
					this.DisableEffectCam();
				}
			}
			else if (this.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleFiveFive))
			{
				if (!this.IsCurStep(ENewbieStepType.EleBatFive_HintEnterBatFive) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintToSelectMap) && !this.IsCurStep(ENewbieStepType.EleBatFive_SelectMap) && !this.IsCurStep(ENewbieStepType.EleBatFive_Loading) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintPicFirst) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintPicFirstGoNext) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintPicSecond) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintPicSecondGoNext) && !this.IsCurStep(ENewbieStepType.EleBatFive_BuyEquipment))
				{
					this.DisableEffectCam();
				}
			}
			else if (this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHpLessNinty))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHpLessThirty))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirSelUpWay))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirSelDownWay))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHeroDead))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirNearFirTower))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirFreeCam))
				{
					this.DisableEffectCam();
				}
			}
			else if (this.IsInNewbieNormalCastSkill())
			{
				if (this.IsCurStep(ENewbieStepType.NormCast_OpenSysSetting))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.NormCast_UseSkillFir))
				{
					this.DisableEffectCam();
				}
			}
		}

		public void TryHandleCloseShop()
		{
			if (this.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleOneOne))
			{
				this.EnableEffectCam();
			}
			if (this.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleFiveFive))
			{
				this.EnableEffectCam();
			}
			else if (this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				this.EnableEffectCam();
			}
			else if (this.IsInNewbieNormalCastSkill())
			{
				this.EnableEffectCam();
			}
		}

		public void TryHandleOpenStatistic()
		{
			if (this.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleOneOne))
			{
				if (this.IsCurStep(ENewbieStepType.ElementaryBatOneOne_UseSkillFourth))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.ElementaryBatOneOne_UseSkillFirst))
				{
					this.DisableEffectCam();
				}
			}
			else if (this.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleFiveFive))
			{
				if (!this.IsCurStep(ENewbieStepType.EleBatFive_HintEnterBatFive) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintToSelectMap) && !this.IsCurStep(ENewbieStepType.EleBatFive_SelectMap) && !this.IsCurStep(ENewbieStepType.EleBatFive_Loading) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintPicFirst) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintPicFirstGoNext) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintPicSecond) && !this.IsCurStep(ENewbieStepType.EleBatFive_HintPicSecondGoNext) && !this.IsCurStep(ENewbieStepType.EleBatFive_BuyEquipment))
				{
					this.DisableEffectCam();
				}
			}
			else if (this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHpLessNinty))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHpLessThirty))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirSelUpWay))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirSelDownWay))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirHeroDead))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirNearFirTower))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.FakeMatchFive_FirFreeCam))
				{
					this.DisableEffectCam();
				}
			}
			else if (this.IsInNewbieNormalCastSkill())
			{
				if (this.IsCurStep(ENewbieStepType.NormCast_OpenSysSetting))
				{
					this.DisableEffectCam();
				}
				else if (this.IsCurStep(ENewbieStepType.NormCast_UseSkillFir))
				{
					this.DisableEffectCam();
				}
			}
		}

		public void TryHandleCloseStatistic()
		{
			if (this.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleOneOne))
			{
				this.EnableEffectCam();
			}
			if (this.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleFiveFive))
			{
				this.EnableEffectCam();
			}
			else if (this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				this.EnableEffectCam();
			}
			else if (this.IsInNewbieNormalCastSkill())
			{
				this.EnableEffectCam();
			}
		}

		private void EnableEffectCam()
		{
			if (CameraRoot.Instance != null && CameraRoot.Instance.m_uiEffectCamera != null)
			{
				CameraRoot.Instance.m_uiEffectCamera.enabled = true;
			}
		}

		private void DisableEffectCam()
		{
			if (CameraRoot.Instance != null && CameraRoot.Instance.m_uiEffectCamera != null)
			{
				CameraRoot.Instance.m_uiEffectCamera.enabled = false;
			}
		}

		public void DisablePhaseEleHallAct()
		{
			this._isDoPhaseEleHallAct = false;
		}

		public void TryHandleGuideNormCastSkill(string inSelHeroId)
		{
			this._isDoPhaseNormCastSkill = false;
			if (string.IsNullOrEmpty(inSelHeroId))
			{
				return;
			}
			if (Singleton<PvpManager>.Instance == null || Singleton<PvpManager>.Instance.IsObserver)
			{
				return;
			}
			if (!this.IsInNewbieGuide() && !this.IsFinNewbieNormalCastSkill() && this.IsNewbieNormalCastSkillHero(inSelHeroId))
			{
				this.NotifyServerGuideNormalCastSkill();
				this._isDoPhaseNormCastSkill = true;
			}
			else
			{
				this._isDoPhaseNormCastSkill = false;
			}
		}

		public void TryHandleOpenHomeBottomView()
		{
			if (this.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive) && !this.IsCurStep(ENewbieStepType.FakeMatchFive_HintPlay))
			{
				this.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintPlay, false, ENewbieStepType.None);
			}
		}

		private bool IsCurPhase(ENewbiePhaseType inPhaseType)
		{
			return this._curPhaseInst != null && this._curPhaseInst.IsPhase(inPhaseType);
		}

		private bool IsCurStep(ENewbieStepType inStepType)
		{
			return this._curPhaseInst != null && this._curPhaseInst.IsCurStep(inStepType);
		}

		public void HandleJoinQueue(int inBattleId, byte inJoinType, byte inResult)
		{
		}

		public void HandleRoomReady()
		{
		}

		public void HandleReadyCheckAllOk()
		{
		}

		public void HandleNewbieRespSpawnHero(NewbieRespSpawnHeroData inData)
		{
			Units unit = MapManager.Instance.GetUnit(inData.heroUniqueId);
			if (inData.groupType == 0)
			{
				UnitVisibilityManager.NewbieBecomeVisible(unit);
			}
			else
			{
				UnitVisibilityManager.NewbieBecomeVisible(unit);
				this.MoveCertainStep(ENewbieStepType.ElementaryBatOneOne_SelEnemyHero, false, ENewbieStepType.None);
			}
		}

		public void HandleNewbieMoveToStep(NewbieMoveToStepData inData)
		{
			ENewbieStepType stepType = (ENewbieStepType)inData.stepType;
			bool flag = this.MoveCertainStep(stepType, false, ENewbieStepType.None);
		}
	}
}
