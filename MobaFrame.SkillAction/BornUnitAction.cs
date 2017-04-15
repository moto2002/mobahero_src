using Com.Game.Module;
using Com.Game.Utils;
using Holoville.HOTween;
using MobaHeros;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class BornUnitAction : BaseHighEffAction
	{
		private enum BornPointType
		{
			NormalPoint,
			FrontPoint,
			JiTuiPoint,
			SkillPoint,
			TopPoint,
			LocalPoint,
			SetByData,
			TargetsPoint
		}

		protected override void doStartHighEffect_Special()
		{
			Dictionary<DataType, object> dictionary = new Dictionary<DataType, object>();
			string unit_id = this.data.unit_id;
			dictionary.Add(DataType.NameId, unit_id);
			dictionary.Add(DataType.TeamType, base.unit.teamType);
			UnitControlType unitControlType = UnitControlType.None;
			if (base.unit != null)
			{
				unitControlType = base.unit.unitControlType;
			}
			SkillUnit skillUnit = null;
			if (!Singleton<PvpManager>.Instance.IsInPvp)
			{
				skillUnit = (MapManager.Instance.SpawnSkillUnit(dictionary, null, base.unit.mTransform.position, Quaternion.identity, unitControlType, base.unit, this.skillId) as SkillUnit);
			}
			if (skillUnit != null)
			{
				skillUnit.unitControlType = base.unit.unitControlType;
				skillUnit.SetParentUnit(base.unit);
				skillUnit.UnitInit(false);
				string[] skillIDs = skillUnit.skillManager.GetSkillIDs();
				string[] array = skillIDs;
				for (int i = 0; i < array.Length; i++)
				{
					string skillID = array[i];
					skillUnit.skillManager.SetSkillLevel(skillID, base.unit.skillManager.GetSkillLevel(this.skillId));
				}
				skillUnit.UnitStart();
				Vector3? skillPosition = this.skillPosition;
				Vector3 arg_159_0;
				if (skillPosition.HasValue)
				{
					Vector3? skillPosition2 = this.skillPosition;
					arg_159_0 = skillPosition2.Value;
				}
				else
				{
					arg_159_0 = base.unit.mTransform.position;
				}
				Vector3 position = arg_159_0;
				BornUnitAction.SetPosition(skillUnit, this.targetUnits, base.unit, this.data, position);
			}
			this.Destroy();
		}

		public static void SetPosition(Units tempUnit, List<Units> targets, Units parentUnit, HighEffectData data, Vector3 position)
		{
			bool flag = data.param2 == 1f;
			bool flag2 = data.param2 == 2f;
			switch ((int)data.param3)
			{
			case 0:
				tempUnit.SetPosition(parentUnit.mTransform.position, false);
				tempUnit.mTransform.rotation = parentUnit.mTransform.rotation;
				if (flag)
				{
					EffectFollowBehavior effectFollowBehavior = tempUnit.gameObject.GetComponent<EffectFollowBehavior>();
					if (effectFollowBehavior == null)
					{
						effectFollowBehavior = tempUnit.gameObject.AddComponent<EffectFollowBehavior>();
					}
					effectFollowBehavior.SetFollowObj(parentUnit.mTransform, Vector3.zero);
				}
				else if (flag2)
				{
					RotationBehavior rotationBehavior = tempUnit.gameObject.GetComponent<RotationBehavior>();
					if (rotationBehavior == null)
					{
						rotationBehavior = tempUnit.gameObject.AddComponent<RotationBehavior>();
					}
					rotationBehavior.SetFollowObj(parentUnit.mTransform, Vector3.zero, 0f);
				}
				break;
			case 1:
				tempUnit.transform.position = position + Vector3.up * 10f;
				HOTween.To(tempUnit.transform, data.param4, new TweenParms().Prop("localPosition", position));
				if (data.param5 == 1f)
				{
					tempUnit.transform.localRotation = parentUnit.transform.rotation;
				}
				break;
			case 2:
				if (tempUnit.gameObject.GetComponent<JiTui>() == null)
				{
					tempUnit.gameObject.AddComponent<JiTui>().Init(tempUnit, position, data.param4, true, 70f);
				}
				parentUnit.TurnToTarget(new Vector3?(position), true, false, 0f);
				break;
			case 3:
				tempUnit.transform.position = position;
				break;
			case 4:
				tempUnit.SetPosition(new Vector3(parentUnit.mTransform.position.x, parentUnit.mTransform.position.y + parentUnit.GetHeight(), parentUnit.mTransform.position.z), false);
				tempUnit.mTransform.rotation = parentUnit.mTransform.rotation;
				if (flag)
				{
					EffectFollowBehavior effectFollowBehavior2 = tempUnit.gameObject.GetComponent<EffectFollowBehavior>();
					if (effectFollowBehavior2 == null)
					{
						effectFollowBehavior2 = tempUnit.gameObject.AddComponent<EffectFollowBehavior>();
					}
					effectFollowBehavior2.SetFollowObj(parentUnit.mTransform, new Vector3(0f, parentUnit.GetHeight(), 0f));
				}
				else if (flag2)
				{
					RotationBehavior rotationBehavior2 = tempUnit.gameObject.GetComponent<RotationBehavior>();
					if (rotationBehavior2 == null)
					{
						rotationBehavior2 = tempUnit.gameObject.AddComponent<RotationBehavior>();
					}
					rotationBehavior2.SetFollowObj(parentUnit.mTransform, new Vector3(0f, parentUnit.GetHeight(), 0f), 0f);
				}
				break;
			case 5:
			{
				Vector3 vector;
				Vector3 b;
				BornUnitAction.GetModelPoint(data, out vector, out b);
				tempUnit.SetPosition(parentUnit.mTransform.position + vector, false);
				tempUnit.mTransform.rotation = Quaternion.Euler(parentUnit.mTransform.eulerAngles + b);
				if (flag)
				{
					EffectFollowBehavior effectFollowBehavior3 = tempUnit.gameObject.GetComponent<EffectFollowBehavior>();
					if (effectFollowBehavior3 == null)
					{
						effectFollowBehavior3 = tempUnit.gameObject.AddComponent<EffectFollowBehavior>();
					}
					effectFollowBehavior3.SetFollowObj(parentUnit.mTransform, vector);
				}
				else if (flag2)
				{
					RotationBehavior rotationBehavior3 = tempUnit.gameObject.GetComponent<RotationBehavior>();
					if (rotationBehavior3 == null)
					{
						rotationBehavior3 = tempUnit.gameObject.AddComponent<RotationBehavior>();
					}
					rotationBehavior3.SetFollowObj(parentUnit.mTransform, vector, 0f);
				}
				break;
			}
			case 6:
			{
				float param = data.param4;
				float num = 0.0174532924f * data.param5;
				float param2 = data.param6;
				Vector3 zero = Vector3.zero;
				Vector3 vector2;
				BornUnitAction.GetModelPoint(data, out vector2, out zero);
				float x = Mathf.Sin(num) * param;
				float z = Mathf.Cos(num) * param;
				vector2 = new Vector3(x, vector2.y, z);
				zero = new Vector3(0f, num, 0f);
				tempUnit.SetPosition(parentUnit.mTransform.position + vector2, false);
				tempUnit.mTransform.rotation = Quaternion.Euler(parentUnit.mTransform.eulerAngles + zero);
				if (flag)
				{
					EffectFollowBehavior effectFollowBehavior4 = tempUnit.gameObject.GetComponent<EffectFollowBehavior>();
					if (effectFollowBehavior4 == null)
					{
						effectFollowBehavior4 = tempUnit.gameObject.AddComponent<EffectFollowBehavior>();
					}
					effectFollowBehavior4.SetFollowObj(parentUnit.mTransform, vector2);
				}
				else if (flag2)
				{
					RotationBehavior rotationBehavior4 = tempUnit.gameObject.GetComponent<RotationBehavior>();
					if (rotationBehavior4 == null)
					{
						rotationBehavior4 = tempUnit.gameObject.AddComponent<RotationBehavior>();
					}
					rotationBehavior4.speed = param2;
					rotationBehavior4.SetFollowObj(parentUnit.mTransform, vector2, 0f);
				}
				break;
			}
			case 7:
				if (targets == null)
				{
					return;
				}
				if (targets.Count == 0)
				{
					return;
				}
				if (targets[0].mTransform == null)
				{
					ClientLogger.Error("targets[0].mTransform == null  targets[0]=" + targets[0].name);
					return;
				}
				if (tempUnit == null)
				{
					return;
				}
				tempUnit.SetPosition(targets[0].mTransform.position, false);
				tempUnit.mTransform.rotation = targets[0].mTransform.rotation;
				if (flag)
				{
					EffectFollowBehavior effectFollowBehavior5 = tempUnit.gameObject.GetComponent<EffectFollowBehavior>();
					if (effectFollowBehavior5 == null)
					{
						effectFollowBehavior5 = tempUnit.gameObject.AddComponent<EffectFollowBehavior>();
					}
					effectFollowBehavior5.SetFollowObj(targets[0].mTransform, Vector3.zero);
				}
				else if (flag2)
				{
					RotationBehavior rotationBehavior5 = tempUnit.gameObject.GetComponent<RotationBehavior>();
					if (rotationBehavior5 == null)
					{
						rotationBehavior5 = tempUnit.gameObject.AddComponent<RotationBehavior>();
					}
					rotationBehavior5.SetFollowObj(targets[0].mTransform, Vector3.zero, 0f);
				}
				break;
			}
		}

		public static void GetModelPoint(HighEffectData data, out Vector3 localPosition, out Vector3 localRotation)
		{
			localPosition = Vector3.zero;
			localRotation = Vector3.zero;
			SkillUnitData vo = Singleton<SkillUnitDataMgr>.Instance.GetVo(data.unit_id);
			string model_id = vo.config.model_id;
			GameObject gameObject = ResourceManager.Load<GameObject>(model_id, true, true, null, 0, false);
			if (gameObject == null)
			{
				Debug.LogError(" GetModelPoint Error : " + model_id);
			}
			localPosition = gameObject.transform.localPosition;
			localRotation = gameObject.transform.localEulerAngles;
		}
	}
}
