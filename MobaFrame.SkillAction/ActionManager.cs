using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class ActionManager
	{
		public static PlayAnimAction PlayAnim(string perform_id, Units actionUnit, bool isRecord = true)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			PlayAnimAction playAnimAction = new PlayAnimAction
			{
				unit = actionUnit,
				performId = perform_id,
				IsRecordAction = isRecord
			};
			playAnimAction.Play();
			return playAnimAction;
		}

		public static PlayEffectAction PlayEffect(string perform_id, Units actionUnit, Vector3? effectPosition = null, Vector3? effectRotation = null, bool isRecord = true, string inSkillId = "", Units casterUnit = null)
		{
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			PlayEffectAction playEffectAction = new PlayEffectAction
			{
				skillId = inSkillId,
				performId = perform_id,
				unit = actionUnit,
				CastlerUnit = casterUnit,
				effectPosition = (!effectPosition.HasValue) ? Vector3.zero : effectPosition.Value,
				effectRoation = (!effectRotation.HasValue) ? Vector3.zero : effectRotation.Value,
				IsRecordAction = isRecord
			};
			playEffectAction.Play();
			return playEffectAction;
		}

		public static PlayLookAtEffectAction PlayLookAtEffect(string perform_id, Units actionUnit, Vector3 forward, bool isRecord = true, Units casterUnit = null)
		{
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			PlayLookAtEffectAction playLookAtEffectAction = new PlayLookAtEffectAction
			{
				performId = perform_id,
				unit = actionUnit,
				effectPosition = Vector3.zero,
				effectRoation = Vector3.zero,
				CastlerUnit = casterUnit,
				forward = forward,
				IsRecordAction = isRecord
			};
			playLookAtEffectAction.Play();
			return playLookAtEffectAction;
		}

		public static CharacterEffectAction PlayCharacterEffect(string effectIds, Units actionUnit, int triggerType)
		{
			if (actionUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(effectIds))
			{
				return null;
			}
			CharacterEffectAction characterEffectAction = new CharacterEffectAction
			{
				effectIds = effectIds,
				unit = actionUnit,
				triggerType = triggerType
			};
			characterEffectAction.Play();
			return characterEffectAction;
		}

		public static PerformAction PlayPerform(SkillDataKey skillKey, string perform_id, Units actionUnit, List<Units> targetUnits = null, Vector3? targetPosition = null, bool isRecord = true, Units casterUnit = null)
		{
			if (actionUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			PerformAction performAction = new PerformAction
			{
				skillKey = skillKey,
				performId = perform_id,
				unit = actionUnit,
				CasterUnit = casterUnit,
				targetUnits = targetUnits,
				targetPosition = targetPosition,
				IsRecordAction = isRecord
			};
			performAction.Play();
			return performAction;
		}

		public static PerformAction PlayPerformMustntLive(SkillDataKey skill_key, string perform_id, Units actionUnit, List<Units> targetUnits = null, Vector3? targetPosition = null, bool isRecord = true, BaseHighEffAction highEffAction = null, Units casterUnit = null)
		{
			if (actionUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			PerformAction performAction = new PerformAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnits = targetUnits,
				targetPosition = targetPosition,
				IsRecordAction = isRecord,
				highEffAction = highEffAction,
				CasterUnit = casterUnit
			};
			performAction.Play();
			return performAction;
		}

		public static MissileAction Missile(SkillDataKey skill_key, string perform_id, Units actionUnit, Units targetUnit = null, Vector3? targetPosition = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			MissileAction missileAction = new MissileAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit,
				targetPosition = targetPosition
			};
			missileAction.Play();
			return missileAction;
		}

		public static AbsorbAction AbsorbMissile(SkillDataKey skill_key, string perform_id, Units actionUnit, Units targetUnit = null)
		{
			if (actionUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			AbsorbAction absorbAction = new AbsorbAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit
			};
			absorbAction.Play();
			return absorbAction;
		}

		public static LinkMissileAction LinkMissile(SkillDataKey skill_key, string perform_id, Units actionUnit, Units targetUnit = null, Vector3? targetPosition = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			LinkMissileAction linkMissileAction = new LinkMissileAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit,
				targetPosition = targetPosition
			};
			linkMissileAction.Play();
			return linkMissileAction;
		}

		public static ParabolaMissileAction ParabolaMissile(SkillDataKey skill_key, string perform_id, Units actionUnit, float distanceToTarget, Units targetUnit = null, Vector3? targetPosition = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			ParabolaMissileAction parabolaMissileAction = new ParabolaMissileAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit,
				targetPosition = targetPosition,
				startDistanceToTarget = distanceToTarget
			};
			parabolaMissileAction.Play();
			return parabolaMissileAction;
		}

		public static CurveMissileAction CurveMissile(SkillDataKey skill_key, string perform_id, Units actionUnit, float distanceToTarget, Units targetUnit = null, Vector3? targetPosition = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			CurveMissileAction curveMissileAction = new CurveMissileAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit,
				targetPosition = targetPosition,
				startDistanceToTarget = distanceToTarget
			};
			curveMissileAction.Play();
			return curveMissileAction;
		}

		public static BombAction Bomb(SkillDataKey skill_key, string perform_id, Units actionUnit, Units targetUnit = null, Vector3? targetPosition = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			BombAction bombAction = new BombAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit,
				targetPosition = targetPosition
			};
			bombAction.Play();
			return bombAction;
		}

		public static MissileBombAction MissileBomb(SkillDataKey skill_key, string perform_id, Units actionUnit, float distanceToTarget, Units targetUnit = null, Vector3? targetPosition = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			MissileBombAction missileBombAction = new MissileBombAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit,
				targetPosition = targetPosition,
				startDistanceToTarget = distanceToTarget
			};
			missileBombAction.Play();
			return missileBombAction;
		}

		public static LinkAction Link(SkillDataKey skill_key, string perform_id, Units actionUnit, List<Units> targetUnits, Vector3? targetPosition = null, Units casterUnit = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			LinkAction linkAction = new LinkAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				CasterUnit = casterUnit,
				targetUnits = targetUnits,
				targetPosition = targetPosition.HasValue ? targetPosition : new Vector3?(Vector3.zero)
			};
			linkAction.Play();
			return linkAction;
		}

		public static BulletAction Bullet(SkillDataKey skill_key, string perform_id, Units actionUnit, Units targetUnit = null, Vector3? targetPositon = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (targetUnit == null && !targetPositon.HasValue)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			BulletAction bulletAction = new BulletAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit,
				targetPosition = targetPositon
			};
			bulletAction.Play();
			return bulletAction;
		}

		public static Hook Hook(SkillDataKey skill_key, string perform_id, Units actionUnit, Units targetUnit, Vector3? targetPositon = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!targetPositon.HasValue)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			Hook hook = new Hook
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetPosition = targetPositon,
				CasterUnit = actionUnit,
				targetUnits = targetUnit
			};
			hook.Play();
			return hook;
		}

		public static BulletJinkesiAction BulletJinkesi(SkillDataKey skill_key, string perform_id, Units actionUnit, Units targetUnit = null, Vector3? targetPositon = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (targetUnit == null && !targetPositon.HasValue)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			BulletJinkesiAction bulletJinkesiAction = new BulletJinkesiAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit,
				targetPosition = targetPositon
			};
			bulletJinkesiAction.Play();
			return bulletJinkesiAction;
		}

		public static BecomeDeadAction BecomeDead(Units actionUnit, Units deadUnit)
		{
			if (deadUnit == null || !deadUnit.isLive)
			{
				return null;
			}
			BecomeDeadAction becomeDeadAction = new BecomeDeadAction
			{
				unit = actionUnit,
				targetUnit = deadUnit
			};
			becomeDeadAction.Play();
			return becomeDeadAction;
		}

		public static BecomeFakeDeadAction BecomeFakeDead(Units actionUnit, Units deadUnit)
		{
			if (deadUnit == null)
			{
				return null;
			}
			BecomeFakeDeadAction becomeFakeDeadAction = new BecomeFakeDeadAction
			{
				unit = actionUnit,
				targetUnit = deadUnit
			};
			becomeFakeDeadAction.Play();
			return becomeFakeDeadAction;
		}

		public static SimpleAction Simple(SkillDataKey skill_key, string perform_id, Units actionUnit, Units casterUnit = null, Vector3? targetPosition = null)
		{
			if (actionUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			SimpleAction simpleAction = new SimpleAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				CasterUnit = casterUnit,
				targetPosition = targetPosition
			};
			simpleAction.Play();
			return simpleAction;
		}

		public static FenLieJianAction FenLieJian(SkillDataKey skill_key, string perform_id, Units actionUnit, List<Units> targetUnits = null, Vector3? targetPosition = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			FenLieJianAction fenLieJianAction = new FenLieJianAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnits = targetUnits,
				targetPosition = targetPosition
			};
			fenLieJianAction.Play();
			return fenLieJianAction;
		}

		public static DragonMissileAction DragonMissile(SkillDataKey skill_key, string perform_id, Units actionUnit, List<Units> targetUnits)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			DragonMissileAction dragonMissileAction = new DragonMissileAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnits = targetUnits
			};
			dragonMissileAction.Play();
			return dragonMissileAction;
		}

		public static SputteringMissileAction SputteringMissile(SkillDataKey skill_key, string perform_id, Units actionUnit, List<Units> targetUnits, BaseHighEffAction hieffAction, Units casterUnit)
		{
			if (actionUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			SputteringMissileAction sputteringMissileAction = new SputteringMissileAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnits = targetUnits,
				highEffAction = hieffAction,
				CasterUnit = casterUnit
			};
			sputteringMissileAction.Play();
			return sputteringMissileAction;
		}

		public static SputteringLinkAction SputteringLink(SkillDataKey skill_key, string perform_id, Units actionUnit, List<Units> targetUnits, BaseHighEffAction hieffAction, Units casterUnit)
		{
			if (actionUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			SputteringLinkAction sputteringLinkAction = new SputteringLinkAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnits = targetUnits,
				highEffAction = hieffAction,
				CasterUnit = casterUnit
			};
			sputteringLinkAction.Play();
			return sputteringLinkAction;
		}

		public static PassiveLinkAction PassiveLink(SkillDataKey skill_key, string perform_id, Units actionUnit, List<Units> targetUnits, BaseHighEffAction hieffAction)
		{
			if (actionUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			PassiveLinkAction passiveLinkAction = new PassiveLinkAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnits = targetUnits,
				highEffAction = hieffAction
			};
			passiveLinkAction.Play();
			return passiveLinkAction;
		}

		public static ChongZhuangAction ChongZhuangAction(SkillDataKey skill_key, string perform_id, Units actionUnit, Units targetUnit = null, Vector3? targetPosition = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			ChongZhuangAction chongZhuangAction = new ChongZhuangAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetUnit = targetUnit,
				targetPosition = targetPosition
			};
			chongZhuangAction.Play();
			return chongZhuangAction;
		}

		public static DartsAction Darts(SkillDataKey skill_key, string perform_id, Units actionUnit, Vector3? targetPositon = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!targetPositon.HasValue)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			DartsAction dartsAction = new DartsAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetPosition = targetPositon
			};
			dartsAction.Play();
			return dartsAction;
		}

		public static PointMissile PointMissile(SkillDataKey skill_key, string perform_id, Units actionUnit, Vector3? targetPositon = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!targetPositon.HasValue)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			PointMissile pointMissile = new PointMissile
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = actionUnit,
				targetPosition = targetPositon
			};
			pointMissile.Play();
			return pointMissile;
		}

		public static DelayDisplayEyeItemAction DelayDisplayEyeItem(string inPreEffectRes, Vector3 inOriginalPos, Vector3 inTargetPos, float inLifeTime, GameObject inEyeItem)
		{
			if (!StringUtils.CheckValid(inPreEffectRes))
			{
				return null;
			}
			if (inLifeTime < 0.001f)
			{
				return null;
			}
			if (inEyeItem == null)
			{
				return null;
			}
			DelayDisplayEyeItemAction delayDisplayEyeItemAction = new DelayDisplayEyeItemAction
			{
				preEffectResource = inPreEffectRes,
				originalPos = inOriginalPos,
				targetPos = inTargetPos,
				lifeTime = inLifeTime,
				eyeItemObj = inEyeItem
			};
			delayDisplayEyeItemAction.Play();
			return delayDisplayEyeItemAction;
		}

		public static TweenRotateAction TweenRotate(Units inActionUnit, float inFromAngleDegree, float inToAngleDegree, float inTweenTime)
		{
			TweenRotateAction tweenRotateAction = new TweenRotateAction
			{
				unit = inActionUnit,
				fromAngleDegree = inFromAngleDegree,
				toAngleDegree = inToAngleDegree,
				tweenTime = inTweenTime
			};
			tweenRotateAction.Play();
			return tweenRotateAction;
		}

		public static ReadySkillAction ReadySkill(SkillDataKey skill_key, Units actionUnit, List<Units> targets, Vector3? skillPosition, Skill skill, bool IsC2P = true)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdReady, actionUnit, skill);
			ReadySkillAction readySkillAction = new ReadySkillAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targets,
				targetPosition = skillPosition,
				IsC2P = IsC2P,
				castBeforeTime = (skill != null) ? skill.GetCastBefore() : 0f
			};
			readySkillAction.Play();
			return readySkillAction;
		}

		public static StartSkillAction StartSkill(SkillDataKey skill_key, Units actionUnit, List<Units> targetUnits = null, Vector3? targetPosition = null, bool IsC2P = true, Skill skill = null)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			StartSkillAction startSkillAction = new StartSkillAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUnits,
				targetPosition = targetPosition,
				IsC2P = IsC2P
			};
			startSkillAction.Play();
			return startSkillAction;
		}

		public static StopSkillAction StopSkill(SkillDataKey skill_key, Units unit, SkillInterruptType interruptType = SkillInterruptType.Force, bool IsC2P = true)
		{
			if (skill_key.Level <= 0)
			{
				return null;
			}
			if (unit == null || !unit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			StopSkillAction stopSkillAction = new StopSkillAction
			{
				skillKey = skill_key,
				unit = unit,
				interruptType = interruptType,
				IsC2P = IsC2P
			};
			stopSkillAction.Play();
			return stopSkillAction;
		}

		public static PVP_StopSkillAction PVP_StopSkill(SkillDataKey skill_key, Units unit, SkillInterruptType interruptType = SkillInterruptType.Force, SkillCastPhase phase = SkillCastPhase.Cast_None, bool IsC2P = true)
		{
			if (unit == null || !unit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			PVP_StopSkillAction pVP_StopSkillAction = new PVP_StopSkillAction
			{
				skillKey = skill_key,
				unit = unit,
				interruptType = interruptType,
				skillCastPhase = phase,
				IsC2P = IsC2P
			};
			pVP_StopSkillAction.Play();
			return pVP_StopSkillAction;
		}

		public static DoSkillAction DoSkill(Units inUnit, string inSkillId)
		{
			if (inUnit == null || !inUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(inSkillId))
			{
				return null;
			}
			DoSkillAction doSkillAction = new DoSkillAction
			{
				unit = inUnit,
				skillId = inSkillId,
				IsC2P = false
			};
			doSkillAction.Play();
			return doSkillAction;
		}

		public static HitSkillAction HitSkill(SkillDataKey skill_key, Units actionUnit, List<Units> targetUnits, bool IsC2P = true)
		{
			if (!Singleton<PvpManager>.Instance.IsFromServer(IsC2P) && !PvpManager.audiotoolediting)
			{
				return null;
			}
			if (actionUnit == null)
			{
				return null;
			}
			if (targetUnits == null || targetUnits.Count < 0)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			HitSkillAction hitSkillAction = new HitSkillAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUnits,
				IsC2P = IsC2P
			};
			hitSkillAction.Play();
			return hitSkillAction;
		}

		public static HitAction Hit(SkillDataKey skill_key, string perform_id, Units targetUnit, Units casterUnit)
		{
			if (targetUnit == null)
			{
				return null;
			}
			if (casterUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(perform_id))
			{
				return null;
			}
			HitAction hitAction = new HitAction
			{
				skillKey = skill_key,
				performId = perform_id,
				unit = targetUnit,
				casterUnit = casterUnit
			};
			hitAction.Play();
			return hitAction;
		}

		public static EndSkillAction EndSkill(SkillDataKey skillKey, Units actionUnit, bool IsC2P = true)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skillKey.SkillID))
			{
				return null;
			}
			EndSkillAction endSkillAction = new EndSkillAction
			{
				skillKey = skillKey,
				unit = actionUnit,
				IsC2P = IsC2P
			};
			endSkillAction.Play();
			return endSkillAction;
		}

		public static ShanShuoAction ShanShuo(SkillDataKey skill_key, Units actionUnit, Vector3 skillPos)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			ShanShuoAction shanShuoAction = new ShanShuoAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetPosition = new Vector3?(skillPos)
			};
			shanShuoAction.Play();
			return shanShuoAction;
		}

		public static ShanShuoAction ShanShuoWeiYi(SkillDataKey skill_key, Units actionUnit, Vector3 skillPos)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				ShanShuoWeiYiActionClient shanShuoWeiYiActionClient = new ShanShuoWeiYiActionClient
				{
					skillKey = skill_key,
					unit = actionUnit,
					targetPosition = new Vector3?(skillPos)
				};
				shanShuoWeiYiActionClient.Play();
				return shanShuoWeiYiActionClient;
			}
			ShanShuoWeiYiAction shanShuoWeiYiAction = new ShanShuoWeiYiAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetPosition = new Vector3?(skillPos)
			};
			shanShuoWeiYiAction.Play();
			return shanShuoWeiYiAction;
		}

		public static TuxiAction TuXi(SkillDataKey skill_key, Units actionUnit, List<Units> targets)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			TuxiAction tuxiAction = new TuxiAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targets
			};
			tuxiAction.Play();
			return tuxiAction;
		}

		public static WaltzAction Waltz(SkillDataKey skill_key, Units actionUnit, List<Units> targets)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			WaltzAction waltzAction = new WaltzAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targets
			};
			waltzAction.Play();
			return waltzAction;
		}

		public static JianRengFengBaoAction JianRengFengBao(SkillDataKey skill_key, Units actionUnit)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			JianRengFengBaoAction jianRengFengBaoAction = new JianRengFengBaoAction
			{
				skillKey = skill_key,
				unit = actionUnit
			};
			jianRengFengBaoAction.Play();
			return jianRengFengBaoAction;
		}

		public static MultiSkillAction MultiSkill(SkillDataKey skill_key, Units actionUnit, List<Units> targetUntis, Vector3? targetPosition)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			MultiSkillAction multiSkillAction = new MultiSkillAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUntis,
				targetPosition = targetPosition
			};
			multiSkillAction.Play();
			return multiSkillAction;
		}

		public static RenWuAction RenWu(SkillDataKey skill_key, Units actionUnit, List<Units> targetUntis, Vector3? targetPosition)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			RenWuAction renWuAction = new RenWuAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUntis,
				targetPosition = targetPosition
			};
			renWuAction.Play();
			return renWuAction;
		}

		public static FaLisheQuAction FaLiSheQu(SkillDataKey skill_key, Units actionUnit, List<Units> targetUnits)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			FaLisheQuAction faLisheQuAction = new FaLisheQuAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUnits
			};
			faLisheQuAction.Play();
			return faLisheQuAction;
		}

		public static JinDianLianJieAction JinDianLianJie(SkillDataKey skill_key, Units actionUnit, List<Units> targetUnits)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			JinDianLianJieAction jinDianLianJieAction = new JinDianLianJieAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUnits
			};
			jinDianLianJieAction.Play();
			return jinDianLianJieAction;
		}

		public static SimpleSkillAction SimpleSkill(SkillDataKey skill_key, Units actionUnit, List<Units> targetUntis, Vector3? targetPosition)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			SimpleSkillAction simpleSkillAction = new SimpleSkillAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUntis,
				targetPosition = targetPosition
			};
			simpleSkillAction.Play();
			return simpleSkillAction;
		}

		public static SiLingMaiChongAction SiLingMaiChong(SkillDataKey skill_key, Units actionUnit, List<Units> targetUnits)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			SiLingMaiChongAction siLingMaiChongAction = new SiLingMaiChongAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUnits
			};
			siLingMaiChongAction.Play();
			return siLingMaiChongAction;
		}

		public static HuoYanBaoHongAction HuoYanBaoHong(SkillDataKey skill_key, Units actionUnit, List<Units> targetUnits)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			HuoYanBaoHongAction huoYanBaoHongAction = new HuoYanBaoHongAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUnits
			};
			huoYanBaoHongAction.Play();
			return huoYanBaoHongAction;
		}

		public static SiWangLiHuanAction SiWangLiHuan(SkillDataKey skill_key, Units actionUnit, List<Units> targetUnits)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			SiWangLiHuanAction siWangLiHuanAction = new SiWangLiHuanAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUnits
			};
			siWangLiHuanAction.Play();
			return siWangLiHuanAction;
		}

		public static FenShenAction FenShen(SkillDataKey skill_key, Units actionUnit)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			FenShenAction fenShenAction = new FenShenAction
			{
				skillKey = skill_key,
				unit = actionUnit
			};
			fenShenAction.Play();
			return fenShenAction;
		}

		public static SimpleReadySkillAction SimpleReadySkill(SkillDataKey skill_key, Units actionUnit, List<Units> targetUntis, Vector3? targetPosition)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			SimpleReadySkillAction simpleReadySkillAction = new SimpleReadySkillAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUntis,
				targetPosition = targetPosition
			};
			simpleReadySkillAction.Play();
			return simpleReadySkillAction;
		}

		public static AnAhaReadyAction AnShaReadySkill(SkillDataKey skill_key, Units actionUnit, List<Units> targetUntis, Vector3? targetPosition)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			AnAhaReadyAction anAhaReadyAction = new AnAhaReadyAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUntis,
				targetPosition = targetPosition
			};
			anAhaReadyAction.Play();
			return anAhaReadyAction;
		}

		public static RenWuReadyAction RenWuReadySkill(SkillDataKey skill_key, Units actionUnit, List<Units> targetUntis, Vector3? targetPosition)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(skill_key.SkillID))
			{
				return null;
			}
			RenWuReadyAction renWuReadyAction = new RenWuReadyAction
			{
				skillKey = skill_key,
				unit = actionUnit,
				targetUnits = targetUntis,
				targetPosition = targetPosition
			};
			renWuReadyAction.Play();
			return renWuReadyAction;
		}

		public static SputteringAction Sputtering(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			SputteringAction sputteringAction = new SputteringAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			sputteringAction.Play();
			return sputteringAction;
		}

		public static SimpleHighEffectAction SimpleHighEffect(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			SimpleHighEffectAction simpleHighEffectAction = new SimpleHighEffectAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			simpleHighEffectAction.Play();
			return simpleHighEffectAction;
		}

		public static YunXuanAction YunXuan(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			YunXuanAction yunXuanAction = new YunXuanAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			yunXuanAction.Play();
			return yunXuanAction;
		}

		public static JiFeiAction JiFei(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			JiFeiAction jiFeiAction = new JiFeiAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			jiFeiAction.Play();
			return jiFeiAction;
		}

		public static RepleAction Reple(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit, float rotateY)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			RepleAction repleAction = new RepleAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId,
				rotateY = rotateY
			};
			repleAction.Play();
			return repleAction;
		}

		public static ParabolaAction Parabola(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit, Vector3? skillPosition)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			if (!skillPosition.HasValue)
			{
				return null;
			}
			ParabolaAction parabolaAction = new ParabolaAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId,
				skillPosition = skillPosition
			};
			parabolaAction.Play();
			return parabolaAction;
		}

		public static WeiyiAction Weiyi(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit, float rotateY)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			WeiyiAction weiyiAction = new WeiyiAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId,
				rotateY = rotateY
			};
			weiyiAction.Play();
			return weiyiAction;
		}

		public static WuDiAction WuDi(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			WuDiAction wuDiAction = new WuDiAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			wuDiAction.Play();
			return wuDiAction;
		}

		public static MoFaMianYiAction MoFaMianYi(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			MoFaMianYiAction moFaMianYiAction = new MoFaMianYiAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			moFaMianYiAction.Play();
			return moFaMianYiAction;
		}

		public static DingShenAction DingShen(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			DingShenAction dingShenAction = new DingShenAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			dingShenAction.Play();
			return dingShenAction;
		}

		public static ChengMoAction ChengMo(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ChengMoAction chengMoAction = new ChengMoAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			chengMoAction.Play();
			return chengMoAction;
		}

		public static PetrifactionAction Petrifaction(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			PetrifactionAction petrifactionAction = new PetrifactionAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			petrifactionAction.Play();
			return petrifactionAction;
		}

		public static BaoZhaAction BaoZha(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			BaoZhaAction baoZhaAction = new BaoZhaAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			baoZhaAction.Play();
			return baoZhaAction;
		}

		public static AttachBuffAction AttachBuff(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			AttachBuffAction attachBuffAction = new AttachBuffAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			attachBuffAction.Play();
			return attachBuffAction;
		}

		public static ClearAllBuffAction ClearAllBuff(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ClearAllBuffAction clearAllBuffAction = new ClearAllBuffAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			clearAllBuffAction.Play();
			return clearAllBuffAction;
		}

		public static ClearAllDebuffAction ClearAllDeBuff(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ClearAllDebuffAction clearAllDebuffAction = new ClearAllDebuffAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			clearAllDebuffAction.Play();
			return clearAllDebuffAction;
		}

		public static ClearHalfBuffAction ClearHalfBuff(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ClearHalfBuffAction clearHalfBuffAction = new ClearHalfBuffAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			clearHalfBuffAction.Play();
			return clearHalfBuffAction;
		}

		public static MoFahuDunAction MoFaHuDun(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			MoFahuDunAction moFahuDunAction = new MoFahuDunAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			moFahuDunAction.Play();
			return moFahuDunAction;
		}

		public static TowAction Tow(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			TowAction towAction = new TowAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			towAction.Play();
			return towAction;
		}

		public static GuangHuanAction Guanghuan(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return null;
			}
			GuangHuanAction guangHuanAction = new GuangHuanAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			guangHuanAction.Play();
			return guangHuanAction;
		}

		public static BloodBallGuangHuanAction BloodBallGuanghuan(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			BloodBallGuangHuanAction bloodBallGuangHuanAction = new BloodBallGuangHuanAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			bloodBallGuangHuanAction.Play();
			return bloodBallGuangHuanAction;
		}

		public static RebirthAction Rebirth(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return null;
			}
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			RebirthAction rebirthAction = new RebirthAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			rebirthAction.Play();
			return rebirthAction;
		}

		public static PVP_RebirthAction PVP_Rebirth(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			PVP_RebirthAction pVP_RebirthAction = new PVP_RebirthAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			pVP_RebirthAction.Play();
			return pVP_RebirthAction;
		}

		public static OccoecatioAction Occoecatio(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			OccoecatioAction occoecatioAction = new OccoecatioAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			occoecatioAction.Play();
			return occoecatioAction;
		}

		public static AttackExtraDamageAction AttackExtraDamage(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			AttackExtraDamageAction attackExtraDamageAction = new AttackExtraDamageAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			attackExtraDamageAction.Play();
			return attackExtraDamageAction;
		}

		public static FrozenAction Frozen(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			FrozenAction frozenAction = new FrozenAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			frozenAction.Play();
			return frozenAction;
		}

		public static CharmAction Charm(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			CharmAction charmAction = new CharmAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			charmAction.Play();
			return charmAction;
		}

		public static TauntAction Taunt(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			TauntAction tauntAction = new TauntAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			tauntAction.Play();
			return tauntAction;
		}

		public static GrowthAction Growth(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			GrowthAction growthAction = new GrowthAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			growthAction.Play();
			return growthAction;
		}

		public static AttackForTargeBuffAction AttackForTargetBuff(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			AttackForTargeBuffAction attackForTargeBuffAction = new AttackForTargeBuffAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			attackForTargeBuffAction.Play();
			return attackForTargeBuffAction;
		}

		public static MorphAction Morph(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			MorphAction morphAction = new MorphAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			morphAction.Play();
			return morphAction;
		}

		public static ChaosAction Chaos(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit, Vector3? skillPosition = null)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ChaosAction chaosAction = new ChaosAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId,
				skillPosition = skillPosition
			};
			chaosAction.Play();
			return chaosAction;
		}

		public static HookBackAction HookBack(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			HookBackAction hookBackAction = new HookBackAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			hookBackAction.Play();
			return hookBackAction;
		}

		public static PlayEffectPerformAction PlayEffectPerform(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			PlayEffectPerformAction playEffectPerformAction = new PlayEffectPerformAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			playEffectPerformAction.Play();
			return playEffectPerformAction;
		}

		public static InvisibleAction Invisible(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			InvisibleAction invisibleAction = new InvisibleAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			invisibleAction.Play();
			return invisibleAction;
		}

		public static HuiGuangFanZhaoAction HuiGuangFanZhao(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			HuiGuangFanZhaoAction huiGuangFanZhaoAction = new HuiGuangFanZhaoAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			huiGuangFanZhaoAction.Play();
			return huiGuangFanZhaoAction;
		}

		public static FuneralAction Funeral(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			FuneralAction funeralAction = new FuneralAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			funeralAction.Play();
			return funeralAction;
		}

		public static FearAction Fear(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			FearAction fearAction = new FearAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			fearAction.Play();
			return fearAction;
		}

		public static TemptationAction Temptation(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			TemptationAction temptationAction = new TemptationAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			temptationAction.Play();
			return temptationAction;
		}

		public static ReplaceMaterialAction ReplaceMaterial(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ReplaceMaterialAction replaceMaterialAction = new ReplaceMaterialAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			replaceMaterialAction.Play();
			return replaceMaterialAction;
		}

		public static FengBaoZhiYanAction FengBaoZhiYan(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			FengBaoZhiYanAction fengBaoZhiYanAction = new FengBaoZhiYanAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			fengBaoZhiYanAction.Play();
			return fengBaoZhiYanAction;
		}

		public static ConjureSkillAction ConjureSkill(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ConjureSkillAction conjureSkillAction = new ConjureSkillAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			conjureSkillAction.Play();
			return conjureSkillAction;
		}

		public static ImprisonmentAction Imprisonment(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ImprisonmentAction imprisonmentAction = new ImprisonmentAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			imprisonmentAction.Play();
			return imprisonmentAction;
		}

		public static DestroyHighEffAction DestroyHighEff(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			DestroyHighEffAction destroyHighEffAction = new DestroyHighEffAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			destroyHighEffAction.Play();
			return destroyHighEffAction;
		}

		public static BetweenSputterAction BetweenSputter(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			BetweenSputterAction betweenSputterAction = new BetweenSputterAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			betweenSputterAction.Play();
			return betweenSputterAction;
		}

		public static SprintAction Sprint(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			SprintAction sprintAction = new SprintAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			sprintAction.Play();
			return sprintAction;
		}

		public static GaoJiYingShengAction GaoJiYingSheng(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			GaoJiYingShengAction gaoJiYingShengAction = new GaoJiYingShengAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			gaoJiYingShengAction.Play();
			return gaoJiYingShengAction;
		}

		public static RemoveBuffHigheffAction RemoveBuffHigheff(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			RemoveBuffHigheffAction removeBuffHigheffAction = new RemoveBuffHigheffAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			removeBuffHigheffAction.Play();
			return removeBuffHigheffAction;
		}

		public static JumpAction Jump(string higheff_id, string skillId, Units actionUnit, Vector3? skillPosition)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			if (!skillPosition.HasValue)
			{
				return null;
			}
			JumpAction jumpAction = new JumpAction
			{
				unit = actionUnit,
				skillPosition = skillPosition,
				higheffId = higheff_id,
				skillId = skillId
			};
			jumpAction.Play();
			return jumpAction;
		}

		public static DoSkillEffAgainAction DoSkillEffAgain(string higheff_id, string skillId, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			DoSkillEffAgainAction doSkillEffAgainAction = new DoSkillEffAgainAction
			{
				unit = actionUnit,
				higheffId = higheff_id,
				skillId = skillId
			};
			doSkillEffAgainAction.Play();
			return doSkillEffAgainAction;
		}

		public static AddStateEffectAction AddStateEffect(string higheff_id, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			AddStateEffectAction addStateEffectAction = new AddStateEffectAction
			{
				unit = actionUnit,
				higheffId = higheff_id
			};
			addStateEffectAction.Play();
			return addStateEffectAction;
		}

		public static BornUnitAction BornUnit(string higheff_id, string skillId, Units actionUnit, Vector3? skillPosition)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			BornUnitAction bornUnitAction = new BornUnitAction
			{
				unit = actionUnit,
				higheffId = higheff_id,
				skillPosition = skillPosition,
				skillId = skillId
			};
			bornUnitAction.Play();
			return bornUnitAction;
		}

		public static SpawnMonsterAction SpawnMonster(string higheff_id, string skillId, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			SpawnMonsterAction spawnMonsterAction = new SpawnMonsterAction
			{
				unit = actionUnit,
				higheffId = higheff_id,
				skillId = skillId
			};
			spawnMonsterAction.Play();
			return spawnMonsterAction;
		}

		public static ResidentPropAction ResidentProp(string higheff_id, string skillId, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ResidentPropAction residentPropAction = new ResidentPropAction
			{
				unit = actionUnit,
				higheffId = higheff_id,
				skillId = skillId
			};
			residentPropAction.Play();
			return residentPropAction;
		}

		public static SwitchAction Switch(string higheff_id, string skillId, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			SwitchAction switchAction = new SwitchAction
			{
				unit = actionUnit,
				higheffId = higheff_id,
				skillId = skillId
			};
			switchAction.Play();
			return switchAction;
		}

		public static PerformReplaceAction PerformReplace(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			PerformReplaceAction performReplaceAction = new PerformReplaceAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			performReplaceAction.Play();
			return performReplaceAction;
		}

		public static BuffLayerEffect BuffLayerEffect(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			BuffLayerEffect buffLayerEffect = new BuffLayerEffect
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			buffLayerEffect.Play();
			return buffLayerEffect;
		}

		public static BackHomeAction BackHome(string higheff_id, string skillId, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			BackHomeAction backHomeAction = new BackHomeAction
			{
				unit = actionUnit,
				higheffId = higheff_id,
				skillId = skillId
			};
			backHomeAction.Play();
			return backHomeAction;
		}

		public static FlyingkickAction Flyingkick(string higheff_id, string skillId, Units actionUnit, List<Units> targetUnits, Vector3 position)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			FlyingkickAction flyingkickAction = new FlyingkickAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				skillPosition = new Vector3?(position),
				higheffId = higheff_id,
				skillId = skillId
			};
			flyingkickAction.Play();
			return flyingkickAction;
		}

		public static TengYunTuJiAction TengYunTuJi(string higheff_id, string skillId, Units actionUnit, List<Units> targetUnits, Vector3 position)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			TengYunTuJiAction tengYunTuJiAction = new TengYunTuJiAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				skillPosition = new Vector3?(position),
				higheffId = higheff_id,
				skillId = skillId
			};
			tengYunTuJiAction.Play();
			return tengYunTuJiAction;
		}

		public static AddDataBagAction AddDataBag(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			AddDataBagAction addDataBagAction = new AddDataBagAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			addDataBagAction.Play();
			return addDataBagAction;
		}

		public static AddGoldAction AddGold(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			AddGoldAction addGoldAction = new AddGoldAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			addGoldAction.Play();
			return addGoldAction;
		}

		public static AssistAddGoldAction AssistAddGold(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			AssistAddGoldAction assistAddGoldAction = new AssistAddGoldAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			assistAddGoldAction.Play();
			return assistAddGoldAction;
		}

		public static PhysicCritPropAction AddPhysicCritPropAction(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit)
		{
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			PhysicCritPropAction physicCritPropAction = new PhysicCritPropAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId
			};
			physicCritPropAction.Play();
			return physicCritPropAction;
		}

		public static ScreenEffectAction ScreenEffect(string higheff_id, string skillId)
		{
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ScreenEffectAction screenEffectAction = new ScreenEffectAction
			{
				higheffId = higheff_id,
				skillId = skillId
			};
			screenEffectAction.Play();
			return screenEffectAction;
		}

		public static ShakeCameraAction ShakeCamera(string higheff_id, string skillId, Units unit)
		{
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			ShakeCameraAction shakeCameraAction = new ShakeCameraAction
			{
				higheffId = higheff_id,
				skillId = skillId
			};
			shakeCameraAction.Play();
			return shakeCameraAction;
		}

		public static WoundAction Wound(Units targetUnit, Units actionUnit, List<short> dataKeys, List<float> dataValues, bool isCrit = false, bool IsC2P = true, int damageType = 0)
		{
			if (Singleton<PvpManager>.Instance.IsPvpSkill(IsC2P))
			{
				return null;
			}
			if (targetUnit == null || !targetUnit.isLive)
			{
				return null;
			}
			if (dataKeys == null || dataValues == null)
			{
				return null;
			}
			WoundAction woundAction = new WoundAction
			{
				targetUnit = targetUnit,
				dataKeys = dataKeys,
				dataValues = dataValues,
				IsC2P = IsC2P,
				isCrit = isCrit,
				damageType = damageType,
				unit = (!(actionUnit != null) || !(actionUnit.ParentUnit != null)) ? actionUnit : actionUnit.ParentUnit
			};
			woundAction.Play();
			return woundAction;
		}

		public static DataUpdateAction DataUpdate(Units actionUnit, DataUpdateInfo dataUpdateInfo, bool IsC2P = true)
		{
			if (Singleton<PvpManager>.Instance.IsPvpSkill(IsC2P))
			{
				return null;
			}
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			DataUpdateAction dataUpdateAction = new DataUpdateAction
			{
				unit = actionUnit,
				dataUpdateInfo = dataUpdateInfo,
				IsC2P = IsC2P
			};
			dataUpdateAction.Play();
			return dataUpdateAction;
		}

		public static DataChangeAction BuffChange(Units targetUnit, Units actionUnit, string buffId, bool isReverse, bool IsC2P = true)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return null;
			}
			if (targetUnit == null || !targetUnit.isLive)
			{
				return null;
			}
			if (!StringUtils.CheckValid(buffId))
			{
				return null;
			}
			DataChangeAction dataChangeAction = new DataChangeAction
			{
				unit = (!(actionUnit != null) || !(actionUnit.ParentUnit != null)) ? actionUnit : actionUnit.ParentUnit,
				targetUnit = targetUnit,
				buffId = buffId,
				isReverse = isReverse,
				IsC2P = IsC2P
			};
			dataChangeAction.Play();
			return dataChangeAction;
		}

		public static PVE_DataRevertAction DataRevert(Units actionUnit)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				return null;
			}
			PVE_DataRevertAction pVE_DataRevertAction = new PVE_DataRevertAction
			{
				unit = actionUnit
			};
			pVE_DataRevertAction.Play();
			return pVE_DataRevertAction;
		}

		public static PVP_DataRevertAction DataRevert(Units actionUnit, float addHp, float addMp)
		{
			PVP_DataRevertAction pVP_DataRevertAction = new PVP_DataRevertAction
			{
				unit = actionUnit,
				addHp = addHp,
				addMp = addMp
			};
			pVP_DataRevertAction.Play();
			return pVP_DataRevertAction;
		}

		public static JumpFontAction JumpFontSingle(JumpFontInfo info, Units actionUnit, bool isMustShow, bool isC2P = true)
		{
			if (actionUnit == null || !actionUnit.isLive)
			{
				return null;
			}
			if (info == null)
			{
				return null;
			}
			JumpFontAction jumpFontAction = new JumpFontAction
			{
				unit = actionUnit,
				jumpFontInfo = info,
				IsC2P = isC2P
			};
			jumpFontAction.Play();
			return jumpFontAction;
		}

		public static AddBuffAction AddBuff(string buff_id, Units targetUnit, Units casterUnit, bool IsC2P = true, string skillID = "")
		{
			if (Singleton<PvpManager>.Instance.IsPvpSkill(IsC2P))
			{
				return null;
			}
			if (!StringUtils.CheckValid(buff_id))
			{
				return null;
			}
			if (targetUnit == null)
			{
				return null;
			}
			BuffData vo = Singleton<BuffDataManager>.Instance.GetVo(buff_id);
			if (vo == null)
			{
				return null;
			}
			if (!targetUnit.isLive && vo.isClearWhenDeath)
			{
				return null;
			}
			AddBuffAction addBuffAction = new AddBuffAction
			{
				unit = casterUnit,
				targetUnit = targetUnit,
				buffId = buff_id,
				skillId = skillID,
				IsC2P = IsC2P
			};
			addBuffAction.Play();
			return addBuffAction;
		}

		public static StartBuffAction StartBuff(string buff_id, Units actionUnit, bool IsC2P = true)
		{
			if (Singleton<PvpManager>.Instance.IsPvpSkill(IsC2P))
			{
				return null;
			}
			if (!ActionManager.IsTargetUnitValid(actionUnit))
			{
				return null;
			}
			if (!StringUtils.CheckValid(buff_id))
			{
				return null;
			}
			StartBuffAction startBuffAction = new StartBuffAction
			{
				unit = actionUnit,
				buffId = buff_id,
				IsC2P = IsC2P
			};
			startBuffAction.Play();
			return startBuffAction;
		}

		public static RemoveBuffAction RemoveBuff(string buff_id, Units targetUnit, Units casterUnit, short reduce_layers, bool IsC2P = true)
		{
			if (Singleton<PvpManager>.Instance.IsPvpSkill(IsC2P))
			{
				return null;
			}
			if (targetUnit == null)
			{
				return null;
			}
			if (!StringUtils.CheckValid(buff_id))
			{
				return null;
			}
			RemoveBuffAction removeBuffAction = new RemoveBuffAction
			{
				unit = casterUnit,
				targetUnit = targetUnit,
				buffId = buff_id,
				reduce_layers = reduce_layers,
				IsC2P = IsC2P
			};
			removeBuffAction.Play();
			return removeBuffAction;
		}

		public static AddHighEffectAction AddHighEffect(string higheff_id, string skillId, List<Units> targetUnits, Units casterUnit, Vector3? skillPosition = null, bool IsC2P = true)
		{
			if (Singleton<PvpManager>.Instance.IsPvpSkill(IsC2P))
			{
				return null;
			}
			if (!ActionManager.IsTargetUnitValid(targetUnits))
			{
				return null;
			}
			AddHighEffectAction addHighEffectAction = new AddHighEffectAction
			{
				unit = casterUnit,
				targetUnits = targetUnits,
				higheffId = higheff_id,
				skillId = skillId,
				skillPosition = skillPosition.HasValue ? skillPosition.Value : Vector3.zero,
				IsC2P = IsC2P
			};
			addHighEffectAction.Play();
			return addHighEffectAction;
		}

		public static AddHighEffectAction AddHighEffect(string higheff_id, string skillId, Units targetUnit, Units casterUnit, Vector3? skillPosition = null, bool IsC2P = true)
		{
			if (Singleton<PvpManager>.Instance.IsPvpSkill(IsC2P))
			{
				return null;
			}
			if (!ActionManager.IsTargetUnitValid(targetUnit))
			{
				return null;
			}
			AddHighEffectAction addHighEffectAction = new AddHighEffectAction
			{
				unit = casterUnit,
				targetUnits = new List<Units>
				{
					targetUnit
				},
				higheffId = higheff_id,
				skillId = skillId,
				skillPosition = skillPosition.HasValue ? skillPosition.Value : Vector3.zero,
				IsC2P = IsC2P
			};
			addHighEffectAction.Play();
			return addHighEffectAction;
		}

		public static StartHighEffectAction StartHighEffect(string higheff_id, string skillId, List<Units> targetUnits, Units actionUnit, float rotateY, Vector3? skillPosition = null, Units owner = null, bool IsC2P = true)
		{
			if (Singleton<PvpManager>.Instance.IsPvpSkill(IsC2P))
			{
				return null;
			}
			if (!StringUtils.CheckValid(higheff_id))
			{
				return null;
			}
			if (owner != null)
			{
				owner.highEffManager.DestroySameAction(higheff_id);
			}
			StartHighEffectAction startHighEffectAction = new StartHighEffectAction
			{
				unit = actionUnit,
				targetUnits = targetUnits,
				skillPosition = new Vector3?(skillPosition.HasValue ? skillPosition.Value : Vector3.zero),
				higheffId = higheff_id,
				owner = owner,
				IsC2P = IsC2P,
				skillId = skillId,
				rotateY = rotateY
			};
			startHighEffectAction.Play();
			return startHighEffectAction;
		}

		public static RemoveHighEffectAction RemoveHighEffect(string higheff_id, Units actionUnit, bool IsC2P = true)
		{
			if (Singleton<PvpManager>.Instance.IsPvpSkill(IsC2P))
			{
				return null;
			}
			if (actionUnit == null)
			{
				return null;
			}
			RemoveHighEffectAction removeHighEffectAction = new RemoveHighEffectAction
			{
				unit = actionUnit,
				higheffId = higheff_id,
				IsC2P = IsC2P
			};
			removeHighEffectAction.Play();
			return removeHighEffectAction;
		}

		protected static bool IsTargetUnitValid(Units targetUnit)
		{
			return !(targetUnit == null) && targetUnit.isLive;
		}

		protected static bool IsTargetUnitValid(List<Units> targetUnits)
		{
			return targetUnits != null && targetUnits.Count > 0;
		}
	}
}
