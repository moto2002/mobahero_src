using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaHeros.AI
{
	public static class TargetSelectHelper
	{
		public static Units GetNearestEnemyTowerOfTarget(Units target)
		{
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits((TeamType)target.teamType, TargetTag.Tower);
			return TargetSelectHelper.GetNearestTowerOfCustom(target, mapUnits);
		}

		public static Transform GetNearestGuidePoint(Units target)
		{
			Dictionary<string, Transform> guidePoints = MapManager.Instance.GetGuidePoints((TeamType)target.teamType);
			if (guidePoints.Count == 0)
			{
				return null;
			}
			int nearestTransOfTarget = TargetSelectHelper.GetNearestTransOfTarget(target, guidePoints);
			if (nearestTransOfTarget == -1 || nearestTransOfTarget == guidePoints.Count)
			{
				return null;
			}
			return MapManager.Instance.GetGuidePoint((TeamType)target.teamType, 0, nearestTransOfTarget);
		}

		public static int GetNearestGuidePointIdx(Units target)
		{
			Dictionary<string, Transform> guidePoints = MapManager.Instance.GetGuidePoints((TeamType)target.teamType);
			if (guidePoints.Count == 0)
			{
				return -1;
			}
			return TargetSelectHelper.GetNearestTransOfTarget(target, guidePoints);
		}

		public static Units GetNearestEnemyTowerOfSelf(Units self)
		{
			TeamType teamType = (self.teamType != 0) ? TeamType.LM : TeamType.BL;
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits(teamType, TargetTag.Building);
			return TargetSelectHelper.GetNearestTowerOfCustom(self, mapUnits);
		}

		public static bool AnyEnemyInTowerRange(Units tower, Units ruleOut = null)
		{
			TeamType teamType = (tower.teamType != 0) ? TeamType.LM : TeamType.BL;
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits(teamType, TargetTag.HeroAndMonster);
			if (mapUnits == null)
			{
				return false;
			}
			foreach (Units current in mapUnits)
			{
				if (UnitFeature.DistanceToTarget(tower.mTransform, current.mTransform) <= tower.GetAttackRange(current))
				{
					if (!(ruleOut != null) || !(current == ruleOut))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static Units GetRecentAttackTower(Units target)
		{
			AIManager aiManager = target.aiManager;
			if (aiManager == null)
			{
				return null;
			}
			foreach (Units current in aiManager.GetRecentAttacker(3f))
			{
				if (current != null && current.isLive && current.isTower && TeamManager.CanAttack(current, target))
				{
					return current;
				}
			}
			return null;
		}

		public static List<Units> GetRecentAttackers(Units target)
		{
			List<Units> list = new List<Units>();
			foreach (Units current in target.aiManager.GetRecentAttacker(3f))
			{
				if (current != null && current.isLive)
				{
					list.Add(current);
				}
			}
			return list;
		}

		public static List<Units> GetAllians(Units target, float distance, TargetTag tag, bool isAllians = true)
		{
			TeamType teamType = TeamType.None;
			if (isAllians)
			{
				teamType = (TeamType)target.teamType;
			}
			else if (target.teamType == 0)
			{
				teamType = TeamType.BL;
			}
			else if (target.teamType == 1)
			{
				teamType = TeamType.LM;
			}
			List<Units> list = new List<Units>();
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits(teamType, tag);
			if (mapUnits == null)
			{
				return list;
			}
			foreach (Units current in mapUnits)
			{
				if (!(current == null))
				{
					if (UnitFeature.DistanceToPoint(target.transform.position, current.transform.position) <= distance)
					{
						list.Add(current);
					}
				}
			}
			return list;
		}

		public static Units GetNearestTowerOfCustom(Units compareTarget, IList<Units> towers)
		{
			float num = 999999f;
			Units result = null;
			if (towers != null)
			{
				foreach (Units current in towers)
				{
					if (current.isLive)
					{
						float num2 = UnitFeature.DistanceToTarget(current.mTransform, compareTarget.mTransform);
						if (num2 < num)
						{
							num = num2;
							result = current;
						}
					}
				}
			}
			return result;
		}

		public static int GetNearestTransOfTarget(Units target, Dictionary<string, Transform> points)
		{
			float num = 999999f;
			int result = -1;
			if (points != null)
			{
				int num2 = -1;
				foreach (string current in points.Keys)
				{
					num2++;
					Transform owner = points[current];
					float num3 = UnitFeature.DistanceToTarget(owner, target.mTransform);
					if (num3 < num)
					{
						num = num3;
						result = num2;
					}
				}
			}
			return result;
		}
	}
}
