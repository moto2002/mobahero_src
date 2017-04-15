using System;
using UnityEngine;

public class UnitFeature
{
	public static float Health(Units pBot)
	{
		return pBot.hp / pBot.hp_max;
	}

	public static float IndividualWeaponStrength(Units pBot, int WeaponType)
	{
		return 0f;
	}

	public static float TotalWeaponStrength(Units pBot)
	{
		return 0f;
	}

	public static float DistanceToItem(Transform pBot, int ItemType)
	{
		return 0f;
	}

	public static float DistanceToTarget(Units owner, Units target)
	{
		Vector3 vector = target.mTransform.position - owner.mTransform.position;
		vector.y = 0f;
		return vector.magnitude;
	}

	public static float DistanceToTarget(Transform owner, Transform target)
	{
		if (owner == null || target == null)
		{
			return 0f;
		}
		Vector3 vector = target.position - owner.position;
		vector.y = 0f;
		return vector.magnitude;
	}

	public static float DistanceToTargetSqr(Transform owner, Transform target)
	{
		if (owner == null)
		{
			return 0f;
		}
		if (target == null)
		{
			return 0f;
		}
		Vector3 vector = target.position - owner.position;
		vector.y = 0f;
		return vector.sqrMagnitude;
	}

	public static float DistanceToPoint(Vector3 current, Vector3 target)
	{
		Vector3 vector = target - current;
		vector.y = 0f;
		return vector.magnitude;
	}

	public static float DistanceToPointSqr(Vector3 current, Vector3 target)
	{
		Vector3 vector = target - current;
		vector.y = 0f;
		return vector.sqrMagnitude;
	}

	public static bool TargetInDistance(Transform owner, Transform target, float distance)
	{
		return UnitFeature.DistanceToTarget(owner, target) <= distance;
	}

	public static bool TargetInRange(Vector3 targetPos, EffectiveRangeType rangeType, Vector3 position, float direction = -1f, float param1 = 0f, float param2 = 0f)
	{
		if (direction == -1f)
		{
			return UnitFeature.TargetInRange(targetPos, rangeType, position, Vector3.zero, param1, param2);
		}
		return UnitFeature.TargetInRange(targetPos, rangeType, position, new Vector3(0f, 0f, direction), param1, param2);
	}

	public static bool TargetInRange(Vector3 targetPos, EffectiveRangeType rangeType, Vector3 position, Vector3 rotation, float param1 = 0f, float param2 = 0f)
	{
		bool result = false;
		switch (rangeType)
		{
		case EffectiveRangeType.JuXing:
		{
			Vector3 vector = targetPos - position;
			if (vector == Vector3.zero)
			{
				result = true;
			}
			else
			{
				Quaternion a = Quaternion.LookRotation(vector, Vector3.up);
				if (Quaternion.Angle(a, Quaternion.Euler(rotation)) <= 90f && vector.z <= param1 && vector.x <= param2)
				{
					result = true;
				}
			}
			break;
		}
		case EffectiveRangeType.YuanXing:
		case EffectiveRangeType.Single:
			result = true;
			break;
		case EffectiveRangeType.ShanXing:
		{
			Vector3 vector2 = targetPos - position;
			if (vector2 == Vector3.zero)
			{
				result = true;
			}
			else
			{
				Quaternion a2 = Quaternion.LookRotation(vector2, Vector3.up);
				if (Quaternion.Angle(a2, Quaternion.Euler(rotation)) <= param2 / 2f)
				{
					result = true;
				}
			}
			break;
		}
		case EffectiveRangeType.AllMap:
			result = true;
			break;
		}
		return result;
	}

	public static bool CheckTarget(GameObject self, GameObject target, SkillTargetCamp targetCampType, TargetTag targetTag)
	{
		return TagManager.CheckTag(target, targetTag) && TeamManager.CheckTeam(self, target, targetCampType, null);
	}
}
