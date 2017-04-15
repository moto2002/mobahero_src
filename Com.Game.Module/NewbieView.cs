using Assets.Scripts.GUILogic.View.BattleEquipment;
using Assets.Scripts.Model;
using GUIFramework;
using HUD.Module;
using MobaHeros.Pvp;
using Newbie;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class NewbieView : BaseView<NewbieView>
	{
		private Collider _maskCollider;

		private UISprite _maskPic;

		private Transform _titleRoot;

		private UILabel _titleMainText;

		private UILabel _titleSubText;

		private UISprite _titleMultiLineBgp;

		private UISprite _titleOneLineBgp;

		private Transform _playNiceEffectTrans;

		private GameObject _eleBatOneGestureObj;

		private Transform _eleBatOneStartGameBtn;

		private Transform _openStoreBtnTrans;

		private Transform _fastBuyEquipTrans;

		private Transform _closeStorePicTrans;

		private Transform _selEnemySoldierCheckTrans;

		private Transform _eleBatOneLearnSkillFirTrans;

		private Transform _eleBatOneLearnSkillSecTrans;

		private Transform _eleBatOneLearnSkillThirdTrans;

		private Transform _eleBatOneLearnSkillFourthTrans;

		private Transform _eleBatOneDispSkillInfoTrans;

		private Transform _eleBatOneDispSKillInfoHideTrans;

		private Transform _eleBatOneDispSkillInfoHintTrans;

		private Transform _useSkillFirTrans;

		private Transform _useSkillSecTrans;

		private Transform _useSkillThirdTrans;

		private Transform _useSkillFourthTrans;

		private Transform _settlementContinueBtnTrans;

		private Transform _settlementBackHomeBtnTrans;

		private Transform _eleBatFiveHintEnterBatFiveTrans;

		private Transform _eleBatFiveHintToSelectMapTrans;

		private Transform _eleBatFiveHintPicFirstTrans;

		private Transform _eleBatFiveHintPicSecondTrans;

		private Transform _eleBatFiveHintMoveNextBtnTrans;

		private Transform _eleBatFiveLearnSkillFirTrans;

		private Transform _eleBatFiveLearnSkillSecTrans;

		private Transform _eleBatFiveLearnSkillThdTrans;

		private Transform _eleBatFiveLearnSkillFourthTrans;

		private Transform _eleBatFiveUseRecoveryTrans;

		private Transform _eleBatFiveUseBackHomeTrans;

		private Transform _eleBatFiveAttackHintTrans;

		private Transform _eleBatFiveUseEyeTrans;

		private Transform _eleBatFiveMiniMapShopTrans;

		private Transform _fakeMatchFiveHintPlayTrans;

		private Transform _fakeMatchFiveHintSingleMatch;

		private Transform _fakeMatchFiveSelMapFiveTrans;

		private Transform _fakeMatchFiveSelMapFiveHintTrans;

		private Transform _fakeMatchFiveHintSelHeroTrans;

		private Transform _fakeMatchFiveHeroSkillIntro;

		private Transform _fakeMatchFiveOpenSummSkillTrans;

		private Transform _fakeMatchFiveSelHeroConfirm;

		private Transform _fakeMatchFiveUseRecoveryTrans;

		private Transform _fakeMatchFiveUseBackHomeTrans;

		private Transform _fakeMatchFiveSettingUpWayTrans;

		private Transform _fakeMatchFiveSettingDownWayTrans;

		private Transform _fakeMatchFiveHintAttackTrans;

		private Transform _fakeMatchFiveBarrageTrans;

		private Transform _fakeMatchFiveFreeCamTrans;

		private Transform _fakeMatchFiveSkillPanelSetTrans;

		private bool _isSelEnemySoldierInfoInited;

		private bool _isEnableSelUnit;

		private Vector2 _selAreaLeftTop = Vector2.one;

		private Vector2 _selAreaLeftBottom = Vector2.one;

		private Vector2 _selAreaRightBottom = Vector2.one;

		private Vector2 _selAreaRightTop = Vector2.one;

		private int _enemySoldierUniqueId;

		private Transform _eleHallActOpenAchieveTrans;

		private Transform _eleHallActAchieveAwdTrans;

		private Transform _eleHallActAchieveBackTrans;

		private Transform _eleHallActOpenDailyTrans;

		private Transform _eleHallActDailyAwdTrans;

		private Transform _eleHallActDailyBackTrans;

		private Transform _eleHallActOpenMagicBottleTrans;

		private Transform _eleHallActMagicLvUpTrans;

		private Transform _eleHallActMagicLvThreeTrans;

		private Transform _eleHallActMagicTaleTrans;

		private Transform _eleHallActMagicBackTrans;

		private Transform _eleHallActOpenActivityTrans;

		private Transform _eleHallActNewbieActivityTrans;

		private Transform _eleHallActNewbieActAwdTrans;

		private Transform _eleHallActOpenLoginAwdTrans;

		private Transform _eleHallActGetLoginAwdTrans;

		private Transform _eleHallActCloseActivityTrans;

		private Transform _eleHallActPlayTrans;

		private Transform _screenSubtitleRoot;

		private UILabel _screenSubtitleContent;

		private UILabel _screenSubtitleProcess;

		private Transform _normCastOpenSysSettingTrans;

		private Transform _normCastSetNormCastTrans;

		private Transform _normCastCloseSysSettingTrans;

		private Transform _normCastUseSkillFirTrans;

		public NewbieView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/NewbieView");
		}

		public void SetMaskInfo(bool inIsColliderActive, Color inMaskPicColor)
		{
			this._maskCollider.enabled = inIsColliderActive;
			this._maskPic.color = inMaskPicColor;
			this._maskPic.enabled = true;
		}

		public void HideMask()
		{
			this._maskCollider.enabled = false;
			this._maskPic.enabled = false;
		}

		public void ShowSelSoldierChecker()
		{
			this._isEnableSelUnit = true;
			this._selEnemySoldierCheckTrans.gameObject.SetActive(true);
			UIEventListener.Get(this._selEnemySoldierCheckTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.CheckSelEnemySoldier);
		}

		public void HideSelSoldierChecker()
		{
			UIEventListener.Get(this._selEnemySoldierCheckTrans.gameObject).onClick = null;
			this._selEnemySoldierCheckTrans.gameObject.SetActive(false);
		}

		private void CheckSelEnemySoldier(GameObject obj = null)
		{
			if (!this._isEnableSelUnit)
			{
				return;
			}
			if (!this._isSelEnemySoldierInfoInited)
			{
				if (!this.TryInitSelEnemySoldierInfo())
				{
					return;
				}
				this._isSelEnemySoldierInfoInited = true;
			}
			UICamera.MouseOrTouch currentTouch = UICamera.currentTouch;
			if (currentTouch == null)
			{
				return;
			}
			if (this.IsInSelArea(currentTouch.pos))
			{
				this._isEnableSelUnit = false;
				this.OnSelEnemySoldier(this._enemySoldierUniqueId);
				NewbieManager.Instance.MoveNextStep();
			}
		}

		private void OnSelEnemySoldier(int inUniqueId)
		{
			Units unit = MapManager.Instance.GetUnit(inUniqueId);
			if (unit == null)
			{
				return;
			}
			Units player = PlayerControlMgr.Instance.GetPlayer();
			if (player == null)
			{
				return;
			}
			player.mCmdCacheController.EnqueueSkillCmd(player.attacks[0], unit.mTransform.position, unit, true, true);
		}

		private bool TryInitSelEnemySoldierInfo()
		{
			List<Units> mapUnits = MapManager.Instance.GetMapUnits(TargetTag.Minions);
			if (mapUnits == null || mapUnits.Count < 1)
			{
				return false;
			}
			Units units = null;
			for (int i = 0; i < mapUnits.Count; i++)
			{
				if (mapUnits[i] != null && TeamManager.CheckTeamType(mapUnits[i].teamType, 1))
				{
					units = mapUnits[i];
					break;
				}
			}
			if (units == null)
			{
				return false;
			}
			this._enemySoldierUniqueId = units.unique_id;
			float num = units.m_ColliderHeight;
			if (num < 1.1f)
			{
				num = 1.1f;
			}
			float num2 = units.m_Radius;
			if (num2 < 0.51f)
			{
				num2 = 0.51f;
			}
			Vector3 position = units.mTransform.position;
			Vector3 position2 = new Vector3(position.x, position.y + num * 0.5f, position.z);
			Vector2 vector = Camera.main.WorldToScreenPoint(position2);
			Vector3 position3 = new Vector3(position2.x + 1f, position2.y, position2.z);
			float num3 = (Camera.main.WorldToScreenPoint(position3).x - vector.x) * num2 * 2f;
			if (num3 < 0f)
			{
				num3 = -num3;
			}
			num3 += 20f;
			this._selAreaLeftTop = new Vector2(vector.x - num3, vector.y + num3);
			this._selAreaLeftBottom = new Vector2(vector.x - num3, vector.y - num3);
			this._selAreaRightTop = new Vector2(vector.x + num3, vector.y + num3);
			this._selAreaRightBottom = new Vector2(vector.x + num3, vector.y - num3);
			return true;
		}

		private bool IsInSelArea(Vector2 inPos)
		{
			return inPos.x > this._selAreaLeftTop.x && inPos.x < this._selAreaRightTop.x && inPos.y > this._selAreaLeftBottom.y && inPos.y < this._selAreaLeftTop.y;
		}

		public void ShowGestureAnimation(bool active)
		{
			if (this._eleBatOneGestureObj != null)
			{
				HomeGCManager.Instance.ClearChildUiTextureResImmediate(this._eleBatOneGestureObj);
				UnityEngine.Object.Destroy(this._eleBatOneGestureObj);
			}
			GameObject gameObject = ResourceManager.LoadPath<GameObject>("Prefab/UI/NewbieEleBatOneGesture", null, 0);
			Transform transform = this.transform.Find("Animation");
			if (gameObject != null && transform != null)
			{
				this._eleBatOneGestureObj = (UnityEngine.Object.Instantiate(gameObject) as GameObject);
				if (this._eleBatOneGestureObj != null)
				{
					this._eleBatOneGestureObj.transform.parent = transform;
					this._eleBatOneGestureObj.transform.localPosition = Vector3.zero;
					this._eleBatOneGestureObj.transform.localRotation = Quaternion.identity;
					this._eleBatOneGestureObj.transform.localScale = Vector3.one;
				}
			}
		}

		private void HideGestureAnimation()
		{
			if (this._eleBatOneGestureObj != null)
			{
				HomeGCManager.Instance.ClearChildUiTextureResImmediate(this._eleBatOneGestureObj);
				UnityEngine.Object.Destroy(this._eleBatOneGestureObj);
			}
		}

		public void ShowStartGameBtn()
		{
			if (this._eleBatOneStartGameBtn != null)
			{
				this._eleBatOneStartGameBtn.gameObject.SetActive(true);
			}
		}

		private void HideStartGameBtn()
		{
			if (this._eleBatOneStartGameBtn != null)
			{
				this._eleBatOneStartGameBtn.gameObject.SetActive(false);
			}
		}

		public void HideLearnHoldDevice()
		{
			this.HideGestureAnimation();
			this.HideStartGameBtn();
		}

		public void ShowLearnSkillHint(int inIdx)
		{
			GameObject learnSkillObjByIndex = this.GetLearnSkillObjByIndex(inIdx);
			learnSkillObjByIndex.SetActive(true);
		}

		public void HideLearnSkillHint(int inIdx)
		{
			GameObject learnSkillObjByIndex = this.GetLearnSkillObjByIndex(inIdx);
			learnSkillObjByIndex.SetActive(false);
		}

		private GameObject GetLearnSkillObjByIndex(int inIdx)
		{
			if (inIdx == 0)
			{
				return this._eleBatOneLearnSkillFirTrans.gameObject;
			}
			if (inIdx == 1)
			{
				return this._eleBatOneLearnSkillSecTrans.gameObject;
			}
			if (inIdx == 2)
			{
				return this._eleBatOneLearnSkillThirdTrans.gameObject;
			}
			return this._eleBatOneLearnSkillFourthTrans.gameObject;
		}

		public void ShowDispSkillInfoHint()
		{
			if (this._eleBatOneDispSkillInfoTrans != null)
			{
				this._eleBatOneDispSkillInfoTrans.gameObject.SetActive(true);
			}
		}

		public void HideDispSkillInfoHint()
		{
			if (this._eleBatOneDispSKillInfoHideTrans != null)
			{
				this._eleBatOneDispSKillInfoHideTrans.gameObject.SetActive(false);
			}
			if (this._eleBatOneDispSkillInfoTrans != null)
			{
				this._eleBatOneDispSkillInfoTrans.gameObject.SetActive(false);
			}
		}

		public void HideDispSkillInfoHintEffect()
		{
			if (this._eleBatOneDispSkillInfoHintTrans != null)
			{
				this._eleBatOneDispSkillInfoHintTrans.gameObject.SetActive(false);
			}
		}

		public void ShowUseSkillHint(int inIdx)
		{
			GameObject useSkillObjByIndex = this.GetUseSkillObjByIndex(inIdx);
			useSkillObjByIndex.SetActive(true);
		}

		public void HideUseSkillHint(int inIdx)
		{
			GameObject useSkillObjByIndex = this.GetUseSkillObjByIndex(inIdx);
			useSkillObjByIndex.SetActive(false);
		}

		public void HideAllUseSkillHint()
		{
			this._useSkillFirTrans.gameObject.SetActive(false);
			this._useSkillSecTrans.gameObject.SetActive(false);
			this._useSkillThirdTrans.gameObject.SetActive(false);
			this._useSkillFourthTrans.gameObject.SetActive(false);
		}

		private GameObject GetUseSkillObjByIndex(int inIdx)
		{
			if (inIdx == 0)
			{
				return this._useSkillFirTrans.gameObject;
			}
			if (inIdx == 1)
			{
				return this._useSkillSecTrans.gameObject;
			}
			if (inIdx == 2)
			{
				return this._useSkillThirdTrans.gameObject;
			}
			return this._useSkillFourthTrans.gameObject;
		}

		public void ShowOpenStoreHint()
		{
			this._openStoreBtnTrans.gameObject.SetActive(true);
		}

		public void HideOpenStoreHint()
		{
			this._openStoreBtnTrans.gameObject.SetActive(false);
		}

		public void ShowCloseStoreHint()
		{
			this._closeStorePicTrans.gameObject.SetActive(true);
		}

		public void HideCloseStoreHint()
		{
			this._closeStorePicTrans.gameObject.SetActive(false);
		}

		public void ShowFastBuyEquipHint()
		{
			UIEventListener.Get(this._fastBuyEquipTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.NewbieFastBuyEquip);
			this._fastBuyEquipTrans.gameObject.SetActive(true);
		}

		public void HideFastBuyEquipHint()
		{
			UIEventListener.Get(this._fastBuyEquipTrans.gameObject).onClick = null;
			this._fastBuyEquipTrans.gameObject.SetActive(false);
		}

		public void ShowPlayNiceEffect()
		{
			this._playNiceEffectTrans.gameObject.SetActive(true);
		}

		public void HidePlayNiceEffect()
		{
			this._playNiceEffectTrans.gameObject.SetActive(false);
		}

		public void ShowSettlementContinue()
		{
			this._settlementContinueBtnTrans.gameObject.SetActive(true);
		}

		public void HideSettlementContinue()
		{
			this._settlementContinueBtnTrans.gameObject.SetActive(false);
		}

		public void ShowSettlementBackHome()
		{
			this._settlementBackHomeBtnTrans.gameObject.SetActive(true);
		}

		public void HideSettlementBackHome()
		{
			this._settlementBackHomeBtnTrans.gameObject.SetActive(false);
		}

		public void ShowHintEnterBatFive()
		{
			this._eleBatFiveHintEnterBatFiveTrans.gameObject.SetActive(true);
		}

		public void HideHintEnterBatFive()
		{
			this._eleBatFiveHintEnterBatFiveTrans.gameObject.SetActive(false);
		}

		public void ShowHintEleBatFiveToSelectMap()
		{
			this._eleBatFiveHintToSelectMapTrans.gameObject.SetActive(true);
		}

		public void HideHintEleBatFiveToSelectMap()
		{
			this._eleBatFiveHintToSelectMapTrans.gameObject.SetActive(false);
		}

		public void ShowEleBatFiveHintPicFirst()
		{
			this._eleBatFiveHintPicFirstTrans.gameObject.SetActive(true);
		}

		public void HideEleBatFiveHintPicFirst()
		{
			this._eleBatFiveHintPicFirstTrans.gameObject.SetActive(false);
		}

		public void ShowEleBatFiveHintPicSecond()
		{
			this._eleBatFiveHintPicSecondTrans.gameObject.SetActive(true);
		}

		public void HideEleBatFiveHintPicSecond()
		{
			this._eleBatFiveHintPicSecondTrans.gameObject.SetActive(false);
		}

		public void ShowEleBatFiveHintMoveNextBtn()
		{
			this._eleBatFiveHintMoveNextBtnTrans.gameObject.SetActive(true);
		}

		public void HideEleBatFiveHintMoveNextBtn()
		{
			this._eleBatFiveHintMoveNextBtnTrans.gameObject.SetActive(false);
		}

		public void ShowEleBatFiveLearnSkillHint(int inSkillIndex)
		{
			Transform eleBatFiveLearnSkillHintTrans = this.GetEleBatFiveLearnSkillHintTrans(inSkillIndex);
			if (eleBatFiveLearnSkillHintTrans != null)
			{
				eleBatFiveLearnSkillHintTrans.gameObject.SetActive(true);
			}
		}

		public Transform GetEleBatFiveLearnSkillHintTrans(int inSkillIndex)
		{
			if (inSkillIndex == 0)
			{
				return this._eleBatFiveLearnSkillFirTrans;
			}
			if (inSkillIndex == 1)
			{
				return this._eleBatFiveLearnSkillSecTrans;
			}
			if (inSkillIndex == 2)
			{
				return this._eleBatFiveLearnSkillThdTrans;
			}
			if (inSkillIndex == 3)
			{
				return this._eleBatFiveLearnSkillFourthTrans;
			}
			return null;
		}

		public void HideEleBatFiveLearnSkillHint()
		{
			if (this._eleBatFiveLearnSkillFirTrans != null)
			{
				this._eleBatFiveLearnSkillFirTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveLearnSkillSecTrans != null)
			{
				this._eleBatFiveLearnSkillSecTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveLearnSkillThdTrans != null)
			{
				this._eleBatFiveLearnSkillThdTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveLearnSkillFourthTrans != null)
			{
				this._eleBatFiveLearnSkillFourthTrans.gameObject.SetActive(false);
			}
		}

		public void ShowEleBatFiveUseRecoveryHint()
		{
			this._eleBatFiveUseRecoveryTrans.gameObject.SetActive(true);
		}

		public void HideEleBatFiveUseRecoveryHint()
		{
			this._eleBatFiveUseRecoveryTrans.gameObject.SetActive(false);
		}

		public void ShowEleBatFiveUseBackHome()
		{
			this._eleBatFiveUseBackHomeTrans.gameObject.SetActive(true);
		}

		public void HideEleBatFiveUseBackHome()
		{
			this._eleBatFiveUseBackHomeTrans.gameObject.SetActive(false);
		}

		public void ShowEleBatFiveAttackHint()
		{
			this._eleBatFiveAttackHintTrans.gameObject.SetActive(true);
		}

		public void HideEleBatFiveAttackHint()
		{
			this._eleBatFiveAttackHintTrans.gameObject.SetActive(false);
		}

		public void ShowEleBatFiveUseEye()
		{
			this._eleBatFiveUseEyeTrans.gameObject.SetActive(true);
		}

		public void HideEleBatFiveUseEye()
		{
			this._eleBatFiveUseEyeTrans.gameObject.SetActive(false);
		}

		public void ShowEleBatFiveMiniMapShop()
		{
			this._eleBatFiveMiniMapShopTrans.gameObject.SetActive(true);
		}

		public void HideEleBatFiveMiniMapShop()
		{
			this._eleBatFiveMiniMapShopTrans.gameObject.SetActive(false);
		}

		public void ShowFakeMatchFiveHintPlay()
		{
			if (this._fakeMatchFiveHintPlayTrans != null && this._fakeMatchFiveHintPlayTrans.gameObject != null)
			{
				this._fakeMatchFiveHintPlayTrans.gameObject.SetActive(true);
			}
		}

		public void HideFakeMatchFiveHintPlay()
		{
			if (this._fakeMatchFiveHintPlayTrans != null && this._fakeMatchFiveHintPlayTrans.gameObject != null)
			{
				this._fakeMatchFiveHintPlayTrans.gameObject.SetActive(false);
			}
		}

		public void ShowFakeMatchFiveHintSingleMatch()
		{
			if (this._fakeMatchFiveHintSingleMatch != null && this._fakeMatchFiveHintSingleMatch.gameObject != null)
			{
				this._fakeMatchFiveHintSingleMatch.gameObject.SetActive(true);
			}
		}

		public void HideFakeMatchFiveHintSingleMatch()
		{
			if (this._fakeMatchFiveHintSingleMatch != null && this._fakeMatchFiveHintSingleMatch.gameObject != null)
			{
				this._fakeMatchFiveHintSingleMatch.gameObject.SetActive(false);
			}
		}

		public void ShowFakeMatchFiveSelMapFive()
		{
			if (this._fakeMatchFiveSelMapFiveHintTrans != null && this._fakeMatchFiveSelMapFiveHintTrans.gameObject != null)
			{
				this._fakeMatchFiveSelMapFiveHintTrans.gameObject.SetActive(true);
			}
			if (this._fakeMatchFiveSelMapFiveTrans != null && this._fakeMatchFiveSelMapFiveTrans.gameObject != null)
			{
				this._fakeMatchFiveSelMapFiveTrans.gameObject.SetActive(true);
			}
		}

		public void HideFakeMatchFiveSelMapFive()
		{
			if (this._fakeMatchFiveSelMapFiveTrans != null && this._fakeMatchFiveSelMapFiveTrans.gameObject != null)
			{
				this._fakeMatchFiveSelMapFiveTrans.gameObject.SetActive(false);
			}
		}

		private void HideFakeMatchFiveSelMapFiveHint()
		{
			if (this._fakeMatchFiveSelMapFiveHintTrans != null && this._fakeMatchFiveSelMapFiveHintTrans.gameObject != null)
			{
				this._fakeMatchFiveSelMapFiveHintTrans.gameObject.SetActive(false);
			}
		}

		public void ShowFakeMatchFiveHintSelHero()
		{
			if (this._fakeMatchFiveHintSelHeroTrans != null && this._fakeMatchFiveHintSelHeroTrans.gameObject != null)
			{
				this._fakeMatchFiveHintSelHeroTrans.gameObject.SetActive(true);
			}
		}

		public void HideFakeMatchFiveHintSelHero()
		{
			if (this._fakeMatchFiveHintSelHeroTrans != null && this._fakeMatchFiveHintSelHeroTrans.gameObject != null)
			{
				this._fakeMatchFiveHintSelHeroTrans.gameObject.SetActive(false);
			}
		}

		public void ShowFakeMatchFiveHeroSkillIntro()
		{
			if (this._fakeMatchFiveHeroSkillIntro != null && this._fakeMatchFiveHeroSkillIntro.gameObject != null)
			{
				this._fakeMatchFiveHeroSkillIntro.gameObject.SetActive(true);
			}
		}

		public void HideFakeMatchFiveHeroSkillIntro()
		{
			if (this._fakeMatchFiveHeroSkillIntro != null && this._fakeMatchFiveHeroSkillIntro.gameObject != null)
			{
				this._fakeMatchFiveHeroSkillIntro.gameObject.SetActive(false);
			}
		}

		public void ShowFakeMatchFiveOpenSummSkill()
		{
			if (this._fakeMatchFiveOpenSummSkillTrans != null && this._fakeMatchFiveOpenSummSkillTrans.gameObject != null)
			{
				this._fakeMatchFiveOpenSummSkillTrans.gameObject.SetActive(true);
			}
		}

		public void HideFakeMatchFiveOpenSummSkill()
		{
			if (this._fakeMatchFiveOpenSummSkillTrans != null && this._fakeMatchFiveOpenSummSkillTrans.gameObject != null)
			{
				this._fakeMatchFiveOpenSummSkillTrans.gameObject.SetActive(false);
			}
		}

		public void ShowFakeMatchFiveSelHeroConfirm()
		{
			if (this._fakeMatchFiveSelHeroConfirm != null && this._fakeMatchFiveSelHeroConfirm.gameObject != null)
			{
				this._fakeMatchFiveSelHeroConfirm.gameObject.SetActive(true);
			}
		}

		public void HideFakeMatchFiveSelHeroConfirm()
		{
			if (this._fakeMatchFiveSelHeroConfirm != null && this._fakeMatchFiveSelHeroConfirm.gameObject != null)
			{
				this._fakeMatchFiveSelHeroConfirm.gameObject.SetActive(false);
			}
		}

		public void ShowFakeMatchFiveUseRecoveryHint()
		{
			this.AjustFakeMatchFiveUseRecoveryHintPos();
			if (this._fakeMatchFiveUseRecoveryTrans != null && this._fakeMatchFiveUseRecoveryTrans.gameObject != null)
			{
				this._fakeMatchFiveUseRecoveryTrans.gameObject.SetActive(true);
			}
		}

		public void HideFakeMatchFiveUseRecoveryHint()
		{
			if (this._fakeMatchFiveUseRecoveryTrans != null && this._fakeMatchFiveUseRecoveryTrans.gameObject != null)
			{
				this._fakeMatchFiveUseRecoveryTrans.gameObject.SetActive(false);
			}
		}

		public void AjustFakeMatchFiveUseRecoveryHintPos()
		{
			SettingModelData settingModelData = ModelManager.Instance.Get_SettingData();
			if (this._fakeMatchFiveUseRecoveryTrans != null && settingModelData != null)
			{
				if (settingModelData.skillPanelPivot == 1 || settingModelData.skillPanelPivot == 2)
				{
					this._fakeMatchFiveUseRecoveryTrans.localPosition = new Vector3(320f, 70f, 0f);
				}
				else
				{
					this._fakeMatchFiveUseRecoveryTrans.localPosition = new Vector3(-385f, 70f, 0f);
				}
			}
		}

		public void ShowFakeMatchFiveUseBackHomeHint()
		{
			this._fakeMatchFiveUseBackHomeTrans.gameObject.SetActive(true);
		}

		public void HideFakeMatchFiveUseBackHomeHint()
		{
			this._fakeMatchFiveUseBackHomeTrans.gameObject.SetActive(false);
		}

		public void ShowFakeMatchFiveSettingUpWay()
		{
			this._fakeMatchFiveSettingUpWayTrans.gameObject.SetActive(true);
		}

		public void HideFakeMatchFiveSettingUpWay()
		{
			this._fakeMatchFiveSettingUpWayTrans.gameObject.SetActive(false);
		}

		public void ShowFakeMatchFiveSettingDownWay()
		{
			this._fakeMatchFiveSettingDownWayTrans.gameObject.SetActive(true);
		}

		public void HideFakeMatchFiveSettingDownWay()
		{
			this._fakeMatchFiveSettingDownWayTrans.gameObject.SetActive(false);
		}

		public void ShowFakeMatchFiveHintAttack()
		{
			this._fakeMatchFiveHintAttackTrans.gameObject.SetActive(true);
		}

		public void HideFakeMatchFiveHintAttack()
		{
			this._fakeMatchFiveHintAttackTrans.gameObject.SetActive(false);
		}

		public void ShowFakeMatchFiveBarrage()
		{
			this._fakeMatchFiveBarrageTrans.gameObject.SetActive(true);
		}

		public void HideFakeMatchFiveBarrage()
		{
			this._fakeMatchFiveBarrageTrans.gameObject.SetActive(false);
		}

		public void ShowFakeMatchFiveFreeCamHint()
		{
			this._fakeMatchFiveFreeCamTrans.gameObject.SetActive(true);
		}

		public void HideFakeMatchFiveFreeCamHint()
		{
			this._fakeMatchFiveFreeCamTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActOpenAchieve()
		{
			this._eleHallActOpenAchieveTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActOpenAchieve()
		{
			this._eleHallActOpenAchieveTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActAchieveAwd()
		{
			this._eleHallActAchieveAwdTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActAchieveAwd()
		{
			this._eleHallActAchieveAwdTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActAchieveBack()
		{
			this._eleHallActAchieveBackTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActAchieveBack()
		{
			this._eleHallActAchieveBackTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActOpenDaily()
		{
			this._eleHallActOpenDailyTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActOpenDaily()
		{
			this._eleHallActOpenDailyTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActDailyAwd()
		{
			this._eleHallActDailyAwdTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActDailyAwd()
		{
			this._eleHallActDailyAwdTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActDailyBack()
		{
			this._eleHallActDailyBackTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActDailyBack()
		{
			this._eleHallActDailyBackTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActOpenMagicBottle()
		{
			this._eleHallActOpenMagicBottleTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActOpenMagicBottle()
		{
			this._eleHallActOpenMagicBottleTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActMagicLvUp()
		{
			this._eleHallActMagicLvUpTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActMagicLvUp()
		{
			this._eleHallActMagicLvUpTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActMagicLvThree()
		{
			this._eleHallActMagicLvThreeTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActMagicLvThree()
		{
			this._eleHallActMagicLvThreeTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActMagicTale()
		{
			this._eleHallActMagicTaleTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActMagicTale()
		{
			this._eleHallActMagicTaleTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActMagicBack()
		{
			this._eleHallActMagicBackTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActMagicBack()
		{
			this._eleHallActMagicBackTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActOpenActivity()
		{
			this._eleHallActOpenActivityTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActOpenActivity()
		{
			this._eleHallActOpenActivityTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActNewbieActivity()
		{
			this._eleHallActNewbieActivityTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActNewbieActivity()
		{
			this._eleHallActNewbieActivityTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActNewbieActAwd()
		{
			this._eleHallActNewbieActAwdTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActNewbieActAwd()
		{
			this._eleHallActNewbieActAwdTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActOpenLoginAwd()
		{
			this._eleHallActOpenLoginAwdTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActOpenLoginAwd()
		{
			this._eleHallActOpenLoginAwdTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActGetLoginAwd()
		{
			this._eleHallActGetLoginAwdTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActGetLoginAwd()
		{
			this._eleHallActGetLoginAwdTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActCloseActivity()
		{
			this._eleHallActCloseActivityTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActCloseActivity()
		{
			this._eleHallActCloseActivityTrans.gameObject.SetActive(false);
		}

		public void ShowEleHallActPlay()
		{
			this._eleHallActPlayTrans.gameObject.SetActive(true);
		}

		public void HideEleHallActPlay()
		{
			this._eleHallActPlayTrans.gameObject.SetActive(false);
		}

		public void ShowScreenSubtitle(string inContent, string inProcess)
		{
			if (!string.IsNullOrEmpty(inContent) && this._screenSubtitleContent != null)
			{
				this._screenSubtitleContent.text = inContent;
			}
			if (!string.IsNullOrEmpty(inProcess) && this._screenSubtitleProcess != null)
			{
				this._screenSubtitleProcess.text = inProcess;
			}
			if (this._screenSubtitleRoot != null && this._screenSubtitleRoot.gameObject != null)
			{
				this._screenSubtitleRoot.gameObject.SetActive(true);
			}
		}

		public void HideScreenSubtitle()
		{
			if (this._screenSubtitleRoot != null && this._screenSubtitleRoot.gameObject != null)
			{
				this._screenSubtitleRoot.gameObject.SetActive(false);
			}
		}

		public void ShowNormCastOpenSysSetting()
		{
			this._normCastOpenSysSettingTrans.gameObject.SetActive(true);
		}

		public void HideNormCastOpenSysSetting()
		{
			this._normCastOpenSysSettingTrans.gameObject.SetActive(false);
		}

		public void ShowNormCastSetNormCast()
		{
			this._normCastSetNormCastTrans.gameObject.SetActive(true);
		}

		public void HideNormCastSetNormCast()
		{
			this._normCastSetNormCastTrans.gameObject.SetActive(false);
		}

		public void ShowNormCastCloseSysSetting()
		{
			this._normCastCloseSysSettingTrans.gameObject.SetActive(true);
		}

		public void HideNormCastCloseSysSetting()
		{
			this._normCastCloseSysSettingTrans.gameObject.SetActive(false);
		}

		public void ShowNormCastUseSkillFir()
		{
			this.AdjustNormCastUseSkillFirPos();
			this._normCastUseSkillFirTrans.gameObject.SetActive(true);
		}

		public void HideNormCastUseSkillFir()
		{
			this._normCastUseSkillFirTrans.gameObject.SetActive(false);
		}

		public void AdjustNormCastUseSkillFirPos()
		{
			SettingModelData settingModelData = ModelManager.Instance.Get_SettingData();
			if (this._normCastUseSkillFirTrans != null && settingModelData != null)
			{
				if (settingModelData.skillPanelPivot == 1)
				{
					this._normCastUseSkillFirTrans.localPosition = new Vector3(-850f, 745f, 0f);
					this._normCastUseSkillFirTrans.localRotation = Quaternion.Euler(Vector3.zero);
				}
				else if (settingModelData.skillPanelPivot == 2)
				{
					this._normCastUseSkillFirTrans.localPosition = new Vector3(850f, 665f, 0f);
					this._normCastUseSkillFirTrans.localRotation = Quaternion.Euler(0f, 0f, 90f);
				}
				else
				{
					this._normCastUseSkillFirTrans.localPosition = new Vector3(-217.0671f, 132.1487f, 0f);
					this._normCastUseSkillFirTrans.localRotation = Quaternion.Euler(Vector3.one);
				}
			}
		}

		public void TryForceHideEleBatOneSkillHint()
		{
			if (this._eleBatOneLearnSkillFourthTrans != null && this._eleBatOneLearnSkillFourthTrans.gameObject != null)
			{
				this._eleBatOneLearnSkillFourthTrans.gameObject.SetActive(false);
			}
			if (this._useSkillFourthTrans != null && this._useSkillFourthTrans.gameObject != null)
			{
				this._useSkillFourthTrans.gameObject.SetActive(false);
			}
		}

		public void TryForceHideEleBatFiveSkillHint()
		{
			if (this._eleBatFiveLearnSkillFirTrans != null && this._eleBatFiveLearnSkillFirTrans.gameObject != null)
			{
				this._eleBatFiveLearnSkillFirTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveLearnSkillSecTrans != null && this._eleBatFiveLearnSkillSecTrans.gameObject != null)
			{
				this._eleBatFiveLearnSkillSecTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveLearnSkillThdTrans != null && this._eleBatFiveLearnSkillThdTrans.gameObject != null)
			{
				this._eleBatFiveLearnSkillThdTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveLearnSkillFourthTrans != null && this._eleBatFiveLearnSkillFourthTrans.gameObject != null)
			{
				this._eleBatFiveLearnSkillFourthTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveUseRecoveryTrans != null && this._eleBatFiveUseRecoveryTrans.gameObject != null)
			{
				this._eleBatFiveUseRecoveryTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveUseBackHomeTrans != null && this._eleBatFiveUseBackHomeTrans.gameObject != null)
			{
				this._eleBatFiveUseBackHomeTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveAttackHintTrans != null && this._eleBatFiveAttackHintTrans.gameObject != null)
			{
				this._eleBatFiveAttackHintTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveUseEyeTrans != null && this._eleBatFiveUseEyeTrans.gameObject != null)
			{
				this._eleBatFiveUseEyeTrans.gameObject.SetActive(false);
			}
			if (this._eleBatFiveMiniMapShopTrans != null && this._eleBatFiveMiniMapShopTrans.gameObject != null)
			{
				this._eleBatFiveMiniMapShopTrans.gameObject.SetActive(false);
			}
		}

		public void TryForceHideFakeMatchFiveHint()
		{
			if (this._fakeMatchFiveUseRecoveryTrans != null && this._fakeMatchFiveUseRecoveryTrans.gameObject != null)
			{
				this._fakeMatchFiveUseRecoveryTrans.gameObject.SetActive(false);
			}
			if (this._fakeMatchFiveUseBackHomeTrans != null && this._fakeMatchFiveUseBackHomeTrans.gameObject != null)
			{
				this._fakeMatchFiveUseBackHomeTrans.gameObject.SetActive(false);
			}
			if (this._fakeMatchFiveSettingUpWayTrans != null && this._fakeMatchFiveSettingUpWayTrans.gameObject != null)
			{
				this._fakeMatchFiveSettingUpWayTrans.gameObject.SetActive(false);
			}
			if (this._fakeMatchFiveSettingDownWayTrans != null && this._fakeMatchFiveSettingDownWayTrans.gameObject != null)
			{
				this._fakeMatchFiveSettingDownWayTrans.gameObject.SetActive(false);
			}
			if (this._fakeMatchFiveSkillPanelSetTrans != null && this._fakeMatchFiveSkillPanelSetTrans.gameObject != null)
			{
				this._fakeMatchFiveSkillPanelSetTrans.gameObject.SetActive(false);
			}
			if (this._fakeMatchFiveHintAttackTrans != null && this._fakeMatchFiveHintAttackTrans.gameObject != null)
			{
				this._fakeMatchFiveHintAttackTrans.gameObject.SetActive(false);
			}
			if (this._fakeMatchFiveBarrageTrans != null && this._fakeMatchFiveBarrageTrans.gameObject != null)
			{
				this._fakeMatchFiveBarrageTrans.gameObject.SetActive(false);
			}
			if (this._fakeMatchFiveFreeCamTrans != null && this._fakeMatchFiveFreeCamTrans.gameObject != null)
			{
				this._fakeMatchFiveFreeCamTrans.gameObject.SetActive(false);
			}
		}

		public void TryForceHideNormCastSkillHint()
		{
			if (this._normCastOpenSysSettingTrans != null && this._normCastOpenSysSettingTrans.gameObject != null)
			{
				this._normCastOpenSysSettingTrans.gameObject.SetActive(false);
			}
			if (this._normCastSetNormCastTrans != null && this._normCastSetNormCastTrans.gameObject != null)
			{
				this._normCastSetNormCastTrans.gameObject.SetActive(false);
			}
			if (this._normCastCloseSysSettingTrans != null && this._normCastCloseSysSettingTrans.gameObject != null)
			{
				this._normCastCloseSysSettingTrans.gameObject.SetActive(false);
			}
			if (this._normCastUseSkillFirTrans != null && this._normCastUseSkillFirTrans.gameObject != null)
			{
				this._normCastUseSkillFirTrans.gameObject.SetActive(false);
			}
			this.HideTitle();
		}

		public override void Init()
		{
			base.Init();
			this.InitUIObj();
		}

		private void InitUIObj()
		{
			this.InitTitle();
			this.InitScreeSubtitle();
			this.InitPlayNiceEffect();
			this.InitMask();
			this.InitEleBatOneLearnHoldDev();
			this.InitStoreHint();
			this.InitEleBatOneLearnSkill();
			this.InitEleBatOneDispSkillInfo();
			this.InitUseSkillHint();
			this.InitSettlement();
			this._eleBatFiveHintEnterBatFiveTrans = null;
			this._eleBatFiveHintToSelectMapTrans = null;
			this._eleBatFiveHintPicFirstTrans = null;
			this._eleBatFiveHintPicSecondTrans = null;
			this._eleBatFiveHintMoveNextBtnTrans = null;
			this._eleBatFiveLearnSkillFirTrans = null;
			this._eleBatFiveLearnSkillSecTrans = null;
			this._eleBatFiveLearnSkillThdTrans = null;
			this._eleBatFiveLearnSkillFourthTrans = null;
			this._eleBatFiveUseRecoveryTrans = null;
			this._eleBatFiveUseBackHomeTrans = null;
			this._eleBatFiveAttackHintTrans = null;
			this._eleBatFiveUseEyeTrans = null;
			this._eleBatFiveMiniMapShopTrans = null;
			if (NewbieManager.Instance.IsInNewbiePhase(ENewbiePhaseType.ElementaryBattleFiveFive))
			{
				this.InitEleBatFiveHintEnterBatFive();
				this.InitEleBatFiveHintToSelectMap();
				this.InitEleBatFiveHintPic();
				this.InitEleBatFiveLearnSkill();
				this.InitEleBatFiveUseRecovery();
				this.InitEleBatFiveUseBackHome();
				this.InitEleBatFiveAttackHint();
				this.InitEleBatFiveUseEye();
				this.InitEleBatFiveMiniMapShop();
			}
			this._fakeMatchFiveHintPlayTrans = null;
			this._fakeMatchFiveHintSingleMatch = null;
			this._fakeMatchFiveSelMapFiveTrans = this.transform.Find("FakeMatchFiveSelMapFive");
			this._fakeMatchFiveSelMapFiveHintTrans = this.transform.Find("FakeMatchFiveSelMapFive/Fx_introduction_clickhere");
			this._fakeMatchFiveHintSelHeroTrans = null;
			this._fakeMatchFiveHeroSkillIntro = null;
			this._fakeMatchFiveOpenSummSkillTrans = null;
			this._fakeMatchFiveSelHeroConfirm = null;
			this._fakeMatchFiveUseRecoveryTrans = null;
			this._fakeMatchFiveUseBackHomeTrans = null;
			this._fakeMatchFiveSettingUpWayTrans = null;
			this._fakeMatchFiveSettingDownWayTrans = null;
			this._fakeMatchFiveSkillPanelSetTrans = null;
			this._fakeMatchFiveHintAttackTrans = null;
			this._fakeMatchFiveBarrageTrans = null;
			this._fakeMatchFiveFreeCamTrans = null;
			if (NewbieManager.Instance.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
			{
				this.InitFakeMatchFiveHintPlay();
				this.InitFakeMatchFiveHintSingleMatch();
				this.InitFakeMatchFiveSelMapFive();
				this.InitFakeMatchFiveHintSelHero();
				this.InitFakeMatchFiveHeroSkillIntro();
				this.InitFakeMatchFiveOpenSummSkill();
				this.InitFakeMatchFiveSelHeroConfirm();
				this.InitFakeMatchFiveUseRecovery();
				this.InitFakeMatchFiveUseBackHome();
				this.InitFakeMatchFiveSettingUpWay();
				this.InitFakeMatchFiveSettingDownWay();
				this.InitFakeMatchFiveSkillPanelSet();
				this.InitFakeMatchFiveHintAttack();
				this.InitFakeMatchFiveBarrage();
				this.InitFakeMatchFiveFreeCam();
			}
			this.InitEleHallActOpenAchieve();
			this.InitEleHallActAchieveAwd();
			this.InitEleHallActAchieveBack();
			this.InitEleHallActOpenDaily();
			this.InitEleHallActDailyAwd();
			this.InitEleHallActDailyBack();
			this.InitEleHallActOpenMagicBottle();
			this.InitEleHallActMagicLvUp();
			this.InitEleHallActMagicLvThree();
			this.InitEleHallActMagicTale();
			this.InitEleHallActMagicBack();
			this.InitEleHallActActivity();
			this.InitEleHallActPlay();
			this.InitNormCastOpenSysSetting();
			this.InitNormCastSetNormCast();
			this.InitNormCastCloseSysSetting();
			this.InitNormCastUseSkillFir();
		}

		private void InitSelNewbieGuideType()
		{
		}

		private void InitTitle()
		{
			this._titleRoot = this.transform.Find("TopAnchor/TitleRoot");
			this._titleMainText = this._titleRoot.Find("MainText").GetComponent<UILabel>();
			this._titleSubText = this._titleRoot.Find("SubText").GetComponent<UILabel>();
			this._titleMultiLineBgp = this._titleRoot.Find("BgPic").GetComponent<UISprite>();
			this._titleOneLineBgp = this._titleRoot.Find("BgPicOneLine").GetComponent<UISprite>();
		}

		private void InitScreeSubtitle()
		{
			this._screenSubtitleRoot = this.transform.Find("BottomAnchor/ScreenSubtitleRoot");
			this._screenSubtitleContent = this._screenSubtitleRoot.Find("MainText").GetComponent<UILabel>();
			this._screenSubtitleProcess = this._screenSubtitleRoot.Find("SubText").GetComponent<UILabel>();
		}

		private void InitPlayNiceEffect()
		{
			this._playNiceEffectTrans = this.transform.Find("CenterAnchor/Fx_introduction_nice");
		}

		private void InitMask()
		{
			GameObject gameObject = this.transform.Find("mask_collider").gameObject;
			this._maskCollider = gameObject.GetComponent<Collider>();
			this._maskPic = gameObject.GetComponent<UISprite>();
		}

		private void InitEleBatOneLearnHoldDev()
		{
			this._eleBatOneStartGameBtn = this.transform.Find("StartGameBtn");
			UIEventListener.Get(this._eleBatOneStartGameBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.EleBatOneClickStartGame);
		}

		private void InitStoreHint()
		{
			this._openStoreBtnTrans = this.transform.Find("EleBatOneOneShopRoot/Anchor/SAnchor/Btn");
			this._fastBuyEquipTrans = this.transform.Find("EleBatOneOneShopRoot/Anchor/RAnchor/RecommendGrid/10001");
			this._closeStorePicTrans = this.transform.Find("EleBatOneOneShopRoot/TopAnchor/BackBtn");
			this._selEnemySoldierCheckTrans = this.transform.Find("SelEnemySoldierCheck");
			UIEventListener.Get(this._openStoreBtnTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.NewbieOpenStore);
			UIEventListener.Get(this._closeStorePicTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.NewbieCloseStore);
		}

		private void InitEleBatOneLearnSkill()
		{
			this._eleBatOneLearnSkillFirTrans = this.transform.Find("EleBatOneOneLearnSkill/Anchor/FirstLevelUpBtn");
			this._eleBatOneLearnSkillSecTrans = this.transform.Find("EleBatOneOneLearnSkill/Anchor/SecondLevelUpBtn");
			this._eleBatOneLearnSkillThirdTrans = this.transform.Find("EleBatOneOneLearnSkill/Anchor/ThirdLevelUpBtn");
			this._eleBatOneLearnSkillFourthTrans = this.transform.Find("EleBatOneOneLearnSkill/Anchor/FourthLevelUpBtn");
			UIEventListener.Get(this._eleBatOneLearnSkillFirTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickLearnSkillFirstBtn);
			UIEventListener.Get(this._eleBatOneLearnSkillSecTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickLearnSkillSecondBtn);
			UIEventListener.Get(this._eleBatOneLearnSkillThirdTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickLearnSkillThirdBtn);
			UIEventListener.Get(this._eleBatOneLearnSkillFourthTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickLearnSkillFourthBtn);
		}

		private void InitEleBatOneDispSkillInfo()
		{
			this._eleBatOneDispSkillInfoTrans = this.transform.Find("EleBatOneDispSkillInfoRoot");
			this._eleBatOneDispSKillInfoHideTrans = this.transform.Find("EleBatOneDispSkillInfoRoot/HideColliderObj");
			this._eleBatOneDispSkillInfoHintTrans = this.transform.Find("EleBatOneDispSkillInfoRoot/Fx_introduction_clickhere");
			UIEventListener.Get(this._eleBatOneDispSkillInfoTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickDispSkillInfoBtn);
			UIEventListener.Get(this._eleBatOneDispSKillInfoHideTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickDispSkillInfoHideCollider);
		}

		private void InitUseSkillHint()
		{
			this._useSkillFirTrans = this.transform.Find("EleBatOneOneLearnSkill/Anchor/FirstUseBtn");
			this._useSkillSecTrans = this.transform.Find("EleBatOneOneLearnSkill/Anchor/SecondUseBtn");
			this._useSkillThirdTrans = this.transform.Find("EleBatOneOneLearnSkill/Anchor/ThirdUseBtn");
			this._useSkillFourthTrans = this.transform.Find("EleBatOneOneLearnSkill/Anchor/FourthUseBtn");
			UIEventListener.Get(this._useSkillFirTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickUseSkillBtnFirst);
			UIEventListener.Get(this._useSkillSecTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickUseSkillBtnSecond);
			UIEventListener.Get(this._useSkillThirdTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickUseSkillBtnThird);
			UIEventListener.Get(this._useSkillFourthTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickUseSkillBtnFourth);
		}

		private void InitSettlement()
		{
			this._settlementContinueBtnTrans = this.transform.Find("BackHomeRoot/continueBtn");
			this._settlementBackHomeBtnTrans = this.transform.Find("BackHomeRoot/backBtn");
			UIEventListener.Get(this._settlementContinueBtnTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickContinueBtn);
			UIEventListener.Get(this._settlementBackHomeBtnTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickBackHomeBtn);
		}

		private void InitEleBatFiveHintEnterBatFive()
		{
			this._eleBatFiveHintEnterBatFiveTrans = this.transform.Find("CenterAnchor/EleBatFiveHintEnterBatFiveRoot");
		}

		private void InitEleBatFiveHintToSelectMap()
		{
			this._eleBatFiveHintToSelectMapTrans = this.transform.Find("CenterAnchor/EleBatFiveHintToSelMapRoot");
			UIEventListener.Get(this._eleBatFiveHintToSelectMapTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleBatFiveSelectMap);
		}

		private void InitEleBatFiveHintPic()
		{
			this._eleBatFiveHintPicFirstTrans = this.transform.Find("EleBatFiveHintPicFirstRoot");
			this._eleBatFiveHintPicSecondTrans = this.transform.Find("EleBatFiveHintPicSecondRoot");
			this._eleBatFiveHintMoveNextBtnTrans = this.transform.Find("EleBatFiveHintMoveNextBtnRoot");
			UIEventListener.Get(this._eleBatFiveHintMoveNextBtnTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleBatFiveHintPicMoveNext);
		}

		private void InitEleBatFiveLearnSkill()
		{
			this._eleBatFiveLearnSkillFirTrans = this.transform.Find("EleBatFiveHintLearnSkillRoot/Anchor/FirstLevelUpBtn");
			this._eleBatFiveLearnSkillSecTrans = this.transform.Find("EleBatFiveHintLearnSkillRoot/Anchor/SecondLevelUpBtn");
			this._eleBatFiveLearnSkillThdTrans = this.transform.Find("EleBatFiveHintLearnSkillRoot/Anchor/ThirdLevelUpBtn");
			this._eleBatFiveLearnSkillFourthTrans = this.transform.Find("EleBatFiveHintLearnSkillRoot/Anchor/FourthLevelUpBtn");
		}

		private void InitEleBatFiveUseRecovery()
		{
			this._eleBatFiveUseRecoveryTrans = this.transform.Find("EleBatFiveHintLearnSkillRoot/Anchor/UseRecoveryBtn");
			UIEventListener.Get(this._eleBatFiveUseRecoveryTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleBatFiveUseRecovery);
		}

		private void InitEleBatFiveUseBackHome()
		{
			this._eleBatFiveUseBackHomeTrans = this.transform.Find("EleBatFiveHintLearnSkillRoot/Anchor/UseBackHomeBtn");
			UIEventListener.Get(this._eleBatFiveUseBackHomeTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleBatFiveUseBackHome);
		}

		private void InitEleBatFiveAttackHint()
		{
			this._eleBatFiveAttackHintTrans = this.transform.Find("EleBatFiveHintLearnSkillRoot/Anchor/AttackHintBtn");
			UIEventListener.Get(this._eleBatFiveAttackHintTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleBatFiveAttackHint);
		}

		private void InitEleBatFiveUseEye()
		{
			this._eleBatFiveUseEyeTrans = this.transform.Find("EleBatFiveHintLearnSkillRoot/Anchor/UseEyeBtn");
			UIEventListener.Get(this._eleBatFiveUseEyeTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleBatFiveUseEye);
		}

		private void InitEleBatFiveMiniMapShop()
		{
			this._eleBatFiveMiniMapShopTrans = this.transform.Find("EleBatFiveMiniMapShopRoot");
		}

		private void InitFakeMatchFiveHintPlay()
		{
			this._fakeMatchFiveHintPlayTrans = this.transform.Find("BottomAnchor/FakeMatchFiveHintPlay");
			UIEventListener.Get(this._fakeMatchFiveHintPlayTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveHintPlay);
		}

		private void InitFakeMatchFiveHintSingleMatch()
		{
			this._fakeMatchFiveHintSingleMatch = this.transform.Find("CenterAnchor/FakeMatchFiveHintSingleMatch");
			UIEventListener.Get(this._fakeMatchFiveHintSingleMatch.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveHintSingleMatch);
		}

		private void InitFakeMatchFiveSelMapFive()
		{
			this._fakeMatchFiveSelMapFiveTrans = this.transform.Find("FakeMatchFiveSelMapFive");
			this._fakeMatchFiveSelMapFiveHintTrans = this.transform.Find("FakeMatchFiveSelMapFive/Fx_introduction_clickhere");
			UIEventListener.Get(this._fakeMatchFiveSelMapFiveTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveSelMapFive);
		}

		private void InitFakeMatchFiveHintSelHero()
		{
			this._fakeMatchFiveHintSelHeroTrans = this.transform.Find("CenterAnchor/FakeMatchFiveHintSelHero");
		}

		private void InitFakeMatchFiveHeroSkillIntro()
		{
			this._fakeMatchFiveHeroSkillIntro = this.transform.Find("BottomAnchor/FakeMatchFiveHeroSkillIntro");
		}

		private void InitFakeMatchFiveOpenSummSkill()
		{
			this._fakeMatchFiveOpenSummSkillTrans = this.transform.Find("BottomAnchor/FakeMatchFiveOpenSummSkill");
		}

		private void InitFakeMatchFiveSelHeroConfirm()
		{
			this._fakeMatchFiveSelHeroConfirm = this.transform.Find("BottomAnchor/FakeMatchFiveSelHeroConfirm");
		}

		private void InitFakeMatchFiveUseRecovery()
		{
			this._fakeMatchFiveUseRecoveryTrans = this.transform.Find("FakeMatchFiveHintUseSkillRoot/Anchor/UseRecoveryBtn");
			UIEventListener.Get(this._fakeMatchFiveUseRecoveryTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveUseRecovery);
		}

		private void InitFakeMatchFiveUseBackHome()
		{
			this._fakeMatchFiveUseBackHomeTrans = this.transform.Find("FakeMatchFiveHintUseSkillRoot/Anchor/UseBackHomeBtn");
			UIEventListener.Get(this._fakeMatchFiveUseBackHomeTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveUseBackHome);
		}

		private void InitFakeMatchFiveSettingUpWay()
		{
			this._fakeMatchFiveSettingUpWayTrans = this.transform.Find("FakeMatchFiveSettingUpWay");
			UIEventListener.Get(this._fakeMatchFiveSettingUpWayTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveSettingUpWay);
		}

		private void InitFakeMatchFiveSettingDownWay()
		{
			this._fakeMatchFiveSettingDownWayTrans = this.transform.Find("FakeMatchFiveSettingDownWay");
			UIEventListener.Get(this._fakeMatchFiveSettingDownWayTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveSettingDownWay);
		}

		private void InitFakeMatchFiveSkillPanelSet()
		{
			this._fakeMatchFiveSkillPanelSetTrans = this.transform.Find("CenterAnchor/FakeMatchFiveSkillPanelSet");
		}

		private void InitFakeMatchFiveHintAttack()
		{
			this._fakeMatchFiveHintAttackTrans = this.transform.Find("FakeMatchFiveHintAttack");
			UIEventListener.Get(this._fakeMatchFiveHintAttackTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveHintAttack);
		}

		private void InitFakeMatchFiveBarrage()
		{
			this._fakeMatchFiveBarrageTrans = this.transform.Find("FakeMatchFiveBarrage");
			UIEventListener.Get(this._fakeMatchFiveBarrageTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveBarrage);
		}

		private void InitFakeMatchFiveFreeCam()
		{
			this._fakeMatchFiveFreeCamTrans = this.transform.Find("FakeMatchFiveFreeCam");
			UIEventListener.Get(this._fakeMatchFiveFreeCamTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFakeMatchFiveFreeCam);
		}

		private void InitEleHallActOpenAchieve()
		{
			this._eleHallActOpenAchieveTrans = this.transform.Find("BottomAnchor/EleHallActOpenAchieve");
			UIEventListener.Get(this._eleHallActOpenAchieveTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActOpenAchieve);
		}

		private void InitEleHallActAchieveAwd()
		{
			this._eleHallActAchieveAwdTrans = this.transform.Find("CenterAnchor/EleHallActAchieveAwd");
			UIEventListener.Get(this._eleHallActAchieveAwdTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActAchieveAwd);
		}

		private void InitEleHallActAchieveBack()
		{
			this._eleHallActAchieveBackTrans = this.transform.Find("TopLeftAnchor/EleHallActAchieveBack");
			UIEventListener.Get(this._eleHallActAchieveBackTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActAchieveBack);
		}

		private void InitEleHallActOpenDaily()
		{
			this._eleHallActOpenDailyTrans = this.transform.Find("BottomAnchor/EleHallActOpenDaily");
			UIEventListener.Get(this._eleHallActOpenDailyTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActOpenDaily);
		}

		private void InitEleHallActDailyAwd()
		{
			this._eleHallActDailyAwdTrans = this.transform.Find("CenterAnchor/EleHallActDailyAwd");
			UIEventListener.Get(this._eleHallActDailyAwdTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActDailyAwd);
		}

		private void InitEleHallActDailyBack()
		{
			this._eleHallActDailyBackTrans = this.transform.Find("TopLeftAnchor/EleHallActDailyBack");
			UIEventListener.Get(this._eleHallActDailyBackTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActDailyBack);
		}

		private void InitEleHallActOpenMagicBottle()
		{
			this._eleHallActOpenMagicBottleTrans = this.transform.Find("CenterAnchor/EleHallActOpenMagicBottle");
			UIEventListener.Get(this._eleHallActOpenMagicBottleTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActOpenMagicBottle);
		}

		private void InitEleHallActMagicLvUp()
		{
			this._eleHallActMagicLvUpTrans = this.transform.Find("CenterAnchor/EleHallActMagicLvUp");
			UIEventListener.Get(this._eleHallActMagicLvUpTrans.gameObject).onPress = new UIEventListener.BoolDelegate(this.PressEleHallActMagicLvUp);
		}

		private void InitEleHallActMagicLvThree()
		{
			this._eleHallActMagicLvThreeTrans = this.transform.Find("CenterAnchor/EleHallActMagicLvThree");
			UIEventListener.Get(this._eleHallActMagicLvThreeTrans.gameObject).onPress = new UIEventListener.BoolDelegate(this.PressEleHallActMagicLvThree);
		}

		private void InitEleHallActMagicTale()
		{
			this._eleHallActMagicTaleTrans = this.transform.Find("CenterAnchor/EleHallActMagicTale");
			UIEventListener.Get(this._eleHallActMagicTaleTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActMagicTale);
		}

		private void InitEleHallActMagicBack()
		{
			this._eleHallActMagicBackTrans = this.transform.Find("TopLeftAnchor/EleHallActMagicBack");
			UIEventListener.Get(this._eleHallActMagicBackTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActMagicBack);
		}

		private void InitEleHallActActivity()
		{
			this._eleHallActOpenActivityTrans = this.transform.Find("CenterAnchor/EleHallActOpenActivity");
			this._eleHallActNewbieActivityTrans = this.transform.Find("EleHallActNewbieActivity");
			this._eleHallActNewbieActAwdTrans = this.transform.Find("EleHallActNewbieActAwd");
			this._eleHallActOpenLoginAwdTrans = this.transform.Find("EleHallActOpenLoginAwd");
			this._eleHallActGetLoginAwdTrans = this.transform.Find("EleHallActGetLoginAwd");
			this._eleHallActCloseActivityTrans = this.transform.Find("EleHallActCloseActivity");
			UIEventListener.Get(this._eleHallActOpenActivityTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActOpenActivity);
			UIEventListener.Get(this._eleHallActNewbieActivityTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActNewbieActivity);
			UIEventListener.Get(this._eleHallActNewbieActAwdTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActNewbieActAwd);
			UIEventListener.Get(this._eleHallActOpenLoginAwdTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActOpenLoginAwd);
			UIEventListener.Get(this._eleHallActGetLoginAwdTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActGetLoginAwd);
			UIEventListener.Get(this._eleHallActCloseActivityTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActCloseActivity);
		}

		private void InitEleHallActPlay()
		{
			this._eleHallActPlayTrans = this.transform.Find("BottomAnchor/EleHallActPlay");
			UIEventListener.Get(this._eleHallActPlayTrans.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickEleHallActPlay);
		}

		private void InitNormCastOpenSysSetting()
		{
			this._normCastOpenSysSettingTrans = this.transform.Find("NormCastOpenSetting");
		}

		private void InitNormCastSetNormCast()
		{
			this._normCastSetNormCastTrans = this.transform.Find("CenterAnchor/NormCastSetNormCast");
		}

		private void InitNormCastCloseSysSetting()
		{
			this._normCastCloseSysSettingTrans = this.transform.Find("CenterAnchor/NormCastCloseSetting");
		}

		private void InitNormCastUseSkillFir()
		{
			this._normCastUseSkillFirTrans = this.transform.Find("NormCastUseSkill/Anchor/FirstUseBtn");
		}

		private void ClickEleBatFiveSelectMap(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_SelectMap, false, ENewbieStepType.None);
		}

		private void ClickEleBatFiveHintPicMoveNext(GameObject obj = null)
		{
			NewbieManager.Instance.MoveNextStep();
		}

		private void ClickEleBatFiveUseRecovery(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirHpLessEightyEnd, true, ENewbieStepType.EleBatFive_FirHpLessEighty);
			if (Singleton<SkillView>.Instance != null)
			{
				Singleton<SkillView>.Instance.NewbieEleBatFiveUseExtraSkill("Permanent_TreatmentSkill");
			}
		}

		private void ClickEleBatFiveUseBackHome(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirHpLessThirtyEnd, true, ENewbieStepType.EleBatFive_FirHpLessThirty);
			if (Singleton<SkillView>.Instance != null)
			{
				Singleton<SkillView>.Instance.NewbieEleBatFiveUseExtraSkill("Skill_GoTown");
			}
		}

		private void ClickEleBatFiveAttackHint(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirAtkSecondTowerEnd, true, ENewbieStepType.EleBatFive_FirAtkSecondTower);
			if (PlayerControlMgr.Instance != null && PlayerControlMgr.Instance.GetPlayer() != null)
			{
				TeamSignalManager.TrySendTeamPosNotify(TeamSignalType.Fire, PlayerControlMgr.Instance.GetPlayer().transform.position);
			}
		}

		private void ClickEleBatFiveUseEye(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleBatFive_FirNearGrassEnd, true, ENewbieStepType.EleBatFive_FirNearGrass);
			if (Singleton<SkillView>.Instance != null)
			{
				Singleton<SkillView>.Instance.NewbieEleBatFiveUseExtraSkill("Permanent_VisionWard");
			}
		}

		private void ClickFakeMatchFiveHintPlay(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintSingleMatch, false, ENewbieStepType.None);
			Singleton<MenuBottomBarView>.Instance.NewbieClickPlayBtn();
		}

		private void ClickFakeMatchFiveHintSingleMatch(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_HintSelMapFive, false, ENewbieStepType.None);
			Singleton<UIPvpEntranceCtrl>.Instance.matchType = MatchType.DP;
			CtrlManager.OpenWindow(WindowID.PvpEntranceView, null);
		}

		private void ClickFakeMatchFiveSelMapFive(GameObject obj = null)
		{
			Singleton<UIPvpEntranceCtrl>.Instance.NewbieFakeMatchFiveSelMap();
			this.HideFakeMatchFiveSelMapFiveHint();
		}

		private void ClickFakeMatchFiveUseRecovery(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHpLessNintyEnd, true, ENewbieStepType.FakeMatchFive_FirHpLessNinty);
			if (Singleton<SkillView>.Instance != null)
			{
				Singleton<SkillView>.Instance.NewbieEleBatFiveUseExtraSkill("Permanent_TreatmentSkill");
			}
		}

		private void ClickFakeMatchFiveUseBackHome(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHpLessThirtyEnd, true, ENewbieStepType.FakeMatchFive_FirHpLessThirty);
			if (Singleton<SkillView>.Instance != null)
			{
				Singleton<SkillView>.Instance.NewbieEleBatFiveUseExtraSkill("Skill_GoTown");
			}
		}

		private void ClickFakeMatchFiveSettingUpWay(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirSelUpWayEnd, true, ENewbieStepType.FakeMatchFive_FirSelUpWay);
			CtrlManager.OpenWindow(WindowID.ReturnView, null);
		}

		private void ClickFakeMatchFiveSettingDownWay(GameObject obj = null)
		{
			CtrlManager.OpenWindow(WindowID.ReturnView, null);
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_SkillPanelSet, false, ENewbieStepType.None);
		}

		private void ClickFakeMatchFiveHintAttack(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHeroDeadEnd, true, ENewbieStepType.FakeMatchFive_FirHeroDead);
			if (Singleton<HUDModuleManager>.Instance != null)
			{
				ActionIndicator module = Singleton<HUDModuleManager>.Instance.GetModule<ActionIndicator>(EHUDModule.ActionIndicator);
				if (module != null)
				{
					module.NewbieClickAttack();
				}
			}
		}

		private void ClickFakeMatchFiveBarrage(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirNearFirTowerEnd, true, ENewbieStepType.FakeMatchFive_FirNearFirTower);
			if (Singleton<BarrageEmitterView>.Instance != null)
			{
				Singleton<BarrageEmitterView>.Instance.NewbieClickPanelBtn();
			}
		}

		private void ClickFakeMatchFiveFreeCam(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirFreeCamEnd, true, ENewbieStepType.FakeMatchFive_FirFreeCam);
			GlobalSettings.Instance.isLockView = true;
			BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.Center);
		}

		private void ClickEleHallActOpenAchieve(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_AchieveAward, false, ENewbieStepType.None);
			if (Singleton<MenuBottomBarView>.Instance != null)
			{
				Singleton<MenuBottomBarView>.Instance.NewbieClickTask();
				if (Singleton<TaskView>.Instance != null)
				{
					Singleton<TaskView>.Instance.NewbieAutoSelBattleItem();
				}
			}
		}

		private void ClickEleHallActAchieveAwd(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_AchieveAwardEnd, false, ENewbieStepType.None);
			if (Singleton<TaskView>.Instance != null)
			{
				Singleton<TaskView>.Instance.NewbieGetAchieveAwd();
			}
		}

		private void ClickEleHallActAchieveBack(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_OpenDaily, false, ENewbieStepType.None);
			if (Singleton<MenuTopBarView>.Instance != null)
			{
				Singleton<MenuTopBarView>.Instance.NewbieReturnWindow();
			}
		}

		private void ClickEleHallActOpenDaily(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_DailyAward, false, ENewbieStepType.None);
			if (Singleton<MenuBottomBarView>.Instance != null)
			{
				Singleton<MenuBottomBarView>.Instance.NewbieClickDaily();
			}
		}

		private void ClickEleHallActDailyAwd(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_DailyAwardEnd, false, ENewbieStepType.None);
			if (Singleton<DailyView>.Instance != null)
			{
				Singleton<DailyView>.Instance.NewbieGetDailyAwd();
			}
		}

		private void ClickEleHallActDailyBack(GameObject obj = null)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_MagicBottle, false, ENewbieStepType.None);
			if (Singleton<MenuTopBarView>.Instance != null)
			{
				Singleton<MenuTopBarView>.Instance.NewbieReturnWindow();
			}
		}

		private void ClickEleHallActOpenMagicBottle(GameObject obj = null)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
		}

		private void PressEleHallActMagicLvUp(GameObject obj, bool isPress)
		{
			if (Singleton<BottleSystemView>.Instance != null)
			{
				Singleton<BottleSystemView>.Instance.NewbieEleHallActMagicUseExp(isPress);
			}
			if (!isPress)
			{
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_MagicBottleLvThree, false, ENewbieStepType.None);
			}
		}

		private void PressEleHallActMagicLvThree(GameObject obj, bool isPress)
		{
			if (Singleton<BottleSystemView>.Instance != null)
			{
				Singleton<BottleSystemView>.Instance.NewbieEleHallActMagicUseExp(isPress);
			}
		}

		private void ClickEleHallActMagicTale(GameObject obj)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_MagicBottleTaleEnd, false, ENewbieStepType.None);
			if (Singleton<BottleSystemView>.Instance != null)
			{
				Singleton<BottleSystemView>.Instance.NewbieEleHallActMagicTaleAwd();
			}
		}

		private void ClickEleHallActMagicBack(GameObject obj)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_OpenActivity, false, ENewbieStepType.None);
			if (Singleton<MenuTopBarView>.Instance != null)
			{
				Singleton<MenuTopBarView>.Instance.NewbieReturnWindow();
			}
		}

		private void ClickEleHallActOpenActivity(GameObject obj)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_NewbieActivity, false, ENewbieStepType.None);
			CtrlManager.OpenWindow(WindowID.ActivityView, null);
			MobaMessageManagerTools.SendClientMsg(ClientC2V.ShowActivityNotice, null, false);
		}

		private void ClickEleHallActNewbieActivity(GameObject obj)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_NewbieActAwd, false, ENewbieStepType.None);
			if (Singleton<ActivityView>.Instance != null)
			{
				Singleton<ActivityView>.Instance.NewbieSelActivityByType(110);
			}
		}

		private void ClickEleHallActNewbieActAwd(GameObject obj)
		{
			if (Singleton<ActivityView>.Instance != null)
			{
				bool flag = Singleton<ActivityView>.Instance.NewbieGetNewbieActAwd();
				if (flag)
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_NewbieActAwdEnd, false, ENewbieStepType.None);
				}
			}
		}

		private void ClickEleHallActOpenLoginAwd(GameObject obj)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_GetLoginAwd, false, ENewbieStepType.None);
			if (Singleton<ActivityView>.Instance != null)
			{
				Singleton<ActivityView>.Instance.NewbieSelActByActivityId(9110);
			}
		}

		private void ClickEleHallActGetLoginAwd(GameObject obj)
		{
			if (Singleton<ActivityView>.Instance != null)
			{
				bool flag = Singleton<ActivityView>.Instance.NewbieGetLoginAwd();
				if (flag)
				{
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_GetLoginAwdEnd, false, ENewbieStepType.None);
				}
			}
		}

		private void ClickEleHallActCloseActivity(GameObject obj)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_Play, false, ENewbieStepType.None);
			CtrlManager.CloseWindow(WindowID.ActivityView);
		}

		private void ClickEleHallActPlay(GameObject obj)
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.EleHallAct_End, false, ENewbieStepType.None);
			Singleton<MenuBottomBarView>.Instance.NewbieClickPlayBtn();
		}

		private void EleBatOneClickStartGame(GameObject obj = null)
		{
			NewbieManager.Instance.MoveNextStep();
		}

		private void NewbieOpenStore(GameObject obj = null)
		{
			EBattleShopType nearestShopType = BattleEquipTools_op.GetNearestShopType();
			MobaMessageManagerTools.BattleShop_openBattleShop(nearestShopType, EBattleShopOpenType.eFromButton);
			NewbieManager.Instance.MoveNextStep();
		}

		private void NewbieFastBuyEquip(GameObject obj = null)
		{
			if (Singleton<ShowEquipmentPanelView>.Instance != null)
			{
				Singleton<ShowEquipmentPanelView>.Instance.NewbieBuyRecommendItem();
			}
			NewbieManager.Instance.MoveNextStep();
		}

		private void NewbieCloseStore(GameObject obj = null)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickBack, null, false);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickLearnSkillFirstBtn(GameObject obj = null)
		{
			Singleton<SkillView>.Instance.NewbieEleBatOneOneLearnSkill(0);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickLearnSkillSecondBtn(GameObject obj = null)
		{
			Singleton<SkillView>.Instance.NewbieEleBatOneOneLearnSkill(1);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickLearnSkillThirdBtn(GameObject obj = null)
		{
			Singleton<SkillView>.Instance.NewbieEleBatOneOneLearnSkill(2);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickLearnSkillFourthBtn(GameObject obj = null)
		{
			Singleton<SkillView>.Instance.NewbieEleBatOneOneLearnSkill(3);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickDispSkillInfoBtn(GameObject obj = null)
		{
			this.HideDispSkillInfoHintEffect();
			if (Singleton<HUDModuleManager>.Instance != null)
			{
				FunctionBtnsModule module = Singleton<HUDModuleManager>.Instance.GetModule<FunctionBtnsModule>(EHUDModule.FunctionBtns);
				if (module != null)
				{
					module.NewbieOnClickSkillInfo(null);
				}
			}
		}

		private void OnClickDispSkillInfoHideCollider(GameObject obj = null)
		{
			if (Singleton<HUDModuleManager>.Instance != null)
			{
				FunctionBtnsModule module = Singleton<HUDModuleManager>.Instance.GetModule<FunctionBtnsModule>(EHUDModule.FunctionBtns);
				if (module != null)
				{
					module.NewbieForceHideSkillInfo();
				}
			}
		}

		private void OnClickUseSkillBtnFirst(GameObject obj = null)
		{
			Singleton<SkillView>.Instance.NewbieEleBatOneOneUseSkill(0);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickUseSkillBtnSecond(GameObject obj = null)
		{
			Singleton<SkillView>.Instance.NewbieEleBatOneOneUseSkill(1);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickUseSkillBtnThird(GameObject obj = null)
		{
			Singleton<SkillView>.Instance.NewbieEleBatOneOneUseSkill(2);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickUseSkillBtnFourth(GameObject obj = null)
		{
			Singleton<SkillView>.Instance.NewbieEleBatOneOneUseSkill(3);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickContinueBtn(GameObject obj = null)
		{
			if (Singleton<BattleSettlementView>.Instance != null)
			{
				Singleton<BattleSettlementView>.Instance.NewbieDestroyResultEffect();
			}
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21025, null, 0f);
			MobaMessageManager.ExecuteMsg(message);
			NewbieManager.Instance.MoveNextStep();
		}

		private void OnClickBackHomeBtn(GameObject obj = null)
		{
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21026, null, 0f);
			MobaMessageManager.ExecuteMsg(message);
			NewbieManager.Instance.MoveNextStep();
		}

		public void ShowTitleWithText(string inMainText, string inSubText)
		{
			this._titleMultiLineBgp.enabled = true;
			this._titleOneLineBgp.enabled = false;
			this._titleSubText.enabled = true;
			this._titleSubText.fontSize = 24;
			this._titleMainText.text = inMainText;
			this._titleSubText.text = inSubText;
			this._titleRoot.gameObject.SetActive(true);
		}

		public void ShowTitleWithText(string inMainText)
		{
			this._titleOneLineBgp.enabled = true;
			this._titleMultiLineBgp.enabled = false;
			this._titleSubText.enabled = false;
			this._titleMainText.text = inMainText;
			this._titleRoot.gameObject.SetActive(true);
		}

		public void ClearResources()
		{
			if (this.gameObject == null)
			{
				return;
			}
			UITexture[] componentsInChildren = this.gameObject.GetComponentsInChildren<UITexture>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					HomeGCManager.Instance.ClearUiTextureResource(componentsInChildren[i]);
				}
			}
		}

		public void HideTitle()
		{
			if (this._titleRoot != null && this._titleRoot.gameObject != null)
			{
				this._titleRoot.gameObject.SetActive(false);
			}
		}

		public override void HandleAfterOpenView()
		{
			this.HideMask();
			this.HideAllUseSkillHint();
			this.HideScreenSubtitle();
		}

		public override void Destroy()
		{
			base.Destroy();
		}
	}
}
