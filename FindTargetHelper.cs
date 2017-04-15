using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindTargetHelper
{
	public static void SortTargets(Units self, SortType sortType, ref List<Units> target_units)
	{
		if (target_units != null && target_units.Count > 1)
		{
			switch (sortType)
			{
			case SortType.Hatred:
				target_units.Sort(delegate(Units a, Units b)
				{
					if (a.unique_id == b.unique_id)
					{
						return 0;
					}
					int targetHatredValue = self.GetTargetHatredValue(a);
					int targetHatredValue2 = self.GetTargetHatredValue(b);
					int result = -1;
					if (targetHatredValue > targetHatredValue2)
					{
						result = -1;
					}
					else if (targetHatredValue < targetHatredValue2)
					{
						result = 1;
					}
					return result;
				});
				break;
			case SortType.Distance:
				target_units.Sort(delegate(Units a, Units b)
				{
					if (a.unique_id == b.unique_id)
					{
						return 0;
					}
					float num = 0f;
					if (a != null)
					{
						num = UnitFeature.DistanceToTargetSqr(self.mTransform, a.mTransform);
					}
					float num2 = 0f;
					if (b != null)
					{
						num2 = UnitFeature.DistanceToTargetSqr(self.mTransform, b.mTransform);
					}
					int result = -1;
					if (num > num2)
					{
						result = 1;
					}
					else if (num < num2)
					{
						result = -1;
					}
					return result;
				});
				break;
			case SortType.Blood:
				target_units.Sort(delegate(Units a, Units b)
				{
					if (a.unique_id == b.unique_id)
					{
						return 0;
					}
					float hp = a.hp;
					float hp2 = b.hp;
					int result = -1;
					if (hp > hp2)
					{
						result = 1;
					}
					else if (hp < hp2)
					{
						result = -1;
					}
					return result;
				});
				break;
			case SortType.Magic:
				target_units.Sort(delegate(Units a, Units b)
				{
					if (a.unique_id == b.unique_id)
					{
						return 0;
					}
					float mp = a.mp;
					float mp2 = b.mp;
					int result = -1;
					if (mp > mp2)
					{
						result = 1;
					}
					else if (mp < mp2)
					{
						result = -1;
					}
					return result;
				});
				break;
			case SortType.Prority:
				target_units.Sort(delegate(Units a, Units b)
				{
					if (a.unique_id == b.unique_id)
					{
						return 0;
					}
					int targetPorityValue = self.GetTargetPorityValue(a);
					int targetPorityValue2 = self.GetTargetPorityValue(b);
					int result = -1;
					if (targetPorityValue > targetPorityValue2)
					{
						result = -1;
					}
					else if (targetPorityValue < targetPorityValue2)
					{
						result = 1;
					}
					return result;
				});
				break;
			}
		}
	}

	public static List<Units> FilterTargets(Units self, List<Units> targets, FindType findType, object param)
	{
		if (targets == null)
		{
			return null;
		}
		List<Units> result = new List<Units>(targets);
		FindTargetHelper.FilterTargetsRef(self, ref result, findType, param);
		return result;
	}

	public static void FilterTargetsRef(Units self, ref List<Units> targets, FindType findType, object param)
	{
		if (targets != null && targets.Count > 0)
		{
			switch (findType)
			{
			case FindType.FirstIndex:
			{
				int num = (int)param;
				for (int i = 0; i < targets.Count; i++)
				{
					if (i > num)
					{
						targets.RemoveAt(i);
						i--;
					}
				}
				break;
			}
			case FindType.Distance:
			{
				float num2 = (float)param * (float)param;
				for (int j = 0; j < targets.Count; j++)
				{
					float num3 = UnitFeature.DistanceToTargetSqr(self.mTransform, targets[j].mTransform);
					if (num3 > num2)
					{
						targets.RemoveAt(j);
						j--;
					}
				}
				break;
			}
			case FindType.Blood:
			{
				float num4 = (float)param;
				for (int k = 0; k < targets.Count; k++)
				{
					if (targets[k].hp > targets[k].hp_max * num4)
					{
						targets.RemoveAt(k);
						k--;
					}
				}
				break;
			}
			}
		}
	}

	public static Units findClosestTarget(Vector3 cur_pos, List<Units> targets)
	{
		if (targets != null)
		{
			float num = 1000f;
			int num2 = -1;
			for (int i = 0; i < targets.Count; i++)
			{
				Units units = targets[i];
				if (units != null)
				{
					float num3 = UnitFeature.DistanceToPointSqr(cur_pos, units.transform.position);
					if (num3 < num)
					{
						num = num3;
						num2 = i;
					}
				}
			}
			if (num2 != -1)
			{
				Units units2 = targets[num2];
				if (units2 != null)
				{
					return units2;
				}
			}
		}
		return null;
	}

	public static Units findDyingTarget(Vector3 cur_pos, List<Units> targets)
	{
		if (targets != null)
		{
			float num = 1000f;
			float num2 = -1f;
			int num3 = -1;
			for (int i = 0; i < targets.Count; i++)
			{
				Units units = targets[i];
				if (units != null && units.isLive)
				{
					float num4 = UnitFeature.DistanceToPointSqr(cur_pos, units.transform.position);
					if (num4 < num)
					{
						float num5 = units.hp;
						if (num5 < num2 || num2 == -1f)
						{
							num = num4;
							num3 = i;
							num2 = num5;
						}
					}
				}
			}
			if (num3 != -1)
			{
				Units units2 = targets[num3];
				if (units2 != null)
				{
					return units2;
				}
			}
		}
		return null;
	}

	public static Units findClosestTarget(Units self, Vector3 pos, SkillTargetCamp targetType, global::TargetTag targetTag, EffectiveRangeType rangeType, float param1 = 0f, float param2 = 0f)
	{
		List<Units> list = FindTargetHelper.findTargets(self, pos, targetType, targetTag, rangeType, param1, param2, -1);
		Units result = null;
		if (list != null)
		{
			if (list.Count > 1)
			{
				result = FindTargetHelper.findClosestTarget(pos, list);
			}
			else if (list.Count == 1)
			{
				result = list[0];
			}
		}
		return result;
	}

	public static Units findDyingTarget(Units self, Vector3 pos, SkillTargetCamp targetType, global::TargetTag targetTag, EffectiveRangeType rangeType, float param1 = 0f, float param2 = 0f)
	{
		Units result = null;
		List<Units> list = FindTargetHelper.findTargets(self, pos, targetType, targetTag, rangeType, param1, param2, -1);
		if (list != null)
		{
			if (list.Count > 1)
			{
				result = FindTargetHelper.findDyingTarget(self.transform.position, list);
			}
			else
			{
				result = list[0];
			}
		}
		return result;
	}

	public static List<Units> findTargets(Units self, Vector3 pos, SkillTargetCamp targetType, global::TargetTag targetTag, EffectiveRangeType rangeType, float param1 = 0f, float param2 = 0f, int max_num = -1)
	{
		List<Units> list = new List<Units>();
		List<int> targets = null;
		switch (targetType)
		{
		case SkillTargetCamp.Self:
			list.Add(self);
			break;
		case SkillTargetCamp.Enemy:
			targets = TeamManager.GetOtherTeam(self.team, Relation.Hostility, true);
			break;
		case SkillTargetCamp.Partener:
			targets = TeamManager.GetOtherTeam(self.team, Relation.Friendly, true);
			break;
		case SkillTargetCamp.AttackYouTarget:
		{
			Units attackedYouTarget = self.GetAttackedYouTarget();
			if (attackedYouTarget != null)
			{
				list.Add(attackedYouTarget);
			}
			targets = TeamManager.GetOtherTeam(self.team, Relation.Hostility, true);
			break;
		}
		case SkillTargetCamp.SkillHitTarget:
		{
			Units skillHitedTarget = self.GetSkillHitedTarget();
			if (skillHitedTarget != null)
			{
				list.Add(skillHitedTarget);
			}
			return list;
		}
		case SkillTargetCamp.AttackTarget:
		{
			Units attackTarget = self.GetAttackTarget();
			if (attackTarget != null)
			{
				list.Add(attackTarget);
			}
			targets = TeamManager.GetOtherTeam(self.team, Relation.Hostility, true);
			break;
		}
		case SkillTargetCamp.SkillHitYouTarget:
		{
			Units skillHitedYouTarget = self.GetSkillHitedYouTarget();
			if (skillHitedYouTarget != null)
			{
				list.Add(skillHitedYouTarget);
			}
			return list;
		}
		case SkillTargetCamp.SelectTarget:
			if (self.currentSkillOrAttack != null)
			{
				return self.currentSkillOrAttack.attackTargets;
			}
			return list;
		}
		if (list.Count <= 0)
		{
			switch (rangeType)
			{
			case EffectiveRangeType.JuXing:
			case EffectiveRangeType.YuanXing:
			case EffectiveRangeType.ShanXing:
			case EffectiveRangeType.Single:
			{
				float radius = (param1 <= param2) ? param2 : param1;
				int layerMask = 1 << LayerMask.NameToLayer("Vehicle");
				Collider[] array = Physics.OverlapSphere(pos, radius, layerMask);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						Collider collider = array[i];
						GameObject gameObject = collider.transform.parent.gameObject;
						Units component = gameObject.GetComponent<Units>();
						if (max_num == -1 || list.Count < max_num)
						{
							if (TagManager.CheckTag(component, targetTag))
							{
								if (UnitFeature.TargetInRange(collider.transform.position, rangeType, pos, self.transform.eulerAngles, param1, param2))
								{
									if (TeamManager.CheckTeam(self.gameObject, gameObject, targetType, null))
									{
										if (!(component == null) && component.isLive && component.CanSkillSelected)
										{
											if (!list.Contains(component))
											{
												list.Add(component);
											}
										}
									}
								}
							}
						}
					}
				}
				break;
			}
			case EffectiveRangeType.AllMap:
			{
				Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
				Dictionary<int, Units>.Enumerator enumerator = allMapUnits.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, Units> current = enumerator.Current;
					Units value = current.Value;
					if (!(value == null) && value.isLive && value.CanSkillSelected)
					{
						if (max_num == -1 || list.Count < max_num)
						{
							if (TagManager.CheckTag(value, targetTag))
							{
								if (TeamManager.CheckTeamType(value.teamType, targets))
								{
									if (!list.Contains(value))
									{
										list.Add(value);
									}
								}
							}
						}
					}
				}
				break;
			}
			case EffectiveRangeType.Link:
			{
				float num = (param1 <= param2) ? param2 : param1;
				int layerMask2 = 1 << LayerMask.NameToLayer("Vehicle");
				if (max_num > 0)
				{
					if (!(self == null))
					{
						Units attackTarget2 = self.GetAttackTarget();
						if (!(attackTarget2 == null))
						{
							list.Add(attackTarget2);
							pos = attackTarget2.transform.position;
							while (list.Count < max_num)
							{
								bool flag = false;
								Units units = null;
								float num2 = -1f;
								Collider[] array2 = Physics.OverlapSphere(pos, num, layerMask2);
								for (int j = 0; j < array2.Length; j++)
								{
									if (array2[j] != null)
									{
										Collider collider2 = array2[j];
										GameObject gameObject2 = collider2.transform.parent.gameObject;
										Units component2 = gameObject2.GetComponent<Units>();
										if (max_num != -1 && list.Count >= max_num)
										{
											break;
										}
										if (TagManager.CheckTag(component2, targetTag))
										{
											if (TeamManager.CheckTeam(self.gameObject, gameObject2, targetType, null))
											{
												if (!(component2 == null) && component2.isLive && component2.CanSkillSelected)
												{
													if (!list.Contains(component2))
													{
														if ((units == null || num2 > (pos - component2.transform.position).sqrMagnitude) && (pos - component2.transform.position).sqrMagnitude < num * num)
														{
															units = component2;
															num2 = (pos - component2.transform.position).sqrMagnitude;
														}
														if (units != null)
														{
															flag = true;
														}
													}
												}
											}
										}
									}
								}
								if (!flag)
								{
									break;
								}
								list.Add(units);
								pos = units.transform.position;
							}
						}
					}
				}
				break;
			}
			}
		}
		return list;
	}

	public static List<Units> findTargets(Units self, Vector3 pos, SkillTargetCamp targetType, global::TargetTag targetTag, float radius)
	{
		List<Units> list = new List<Units>();
		int layerMask = 1 << LayerMask.NameToLayer("UnitSelectObj");
		Collider[] array = Physics.OverlapSphere(pos, radius, layerMask);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				Collider collider = array[i];
				GameObject gameObject = collider.transform.parent.gameObject;
				Units component = gameObject.GetComponent<Units>();
				if (!component.IsMonsterCreep() || !(self.GetAttackedYouTarget() != component))
				{
					if (TagManager.CheckTag(component, targetTag))
					{
						if (TeamManager.CheckTeam(self.gameObject, gameObject, targetType, null))
						{
							if (!(component == null) && component.isLive && component.CanSkillSelected)
							{
								if (!list.Contains(component))
								{
									list.Add(component);
								}
							}
						}
					}
				}
			}
		}
		return list;
	}

	public static Units FindAutoSkillTarget(Units self, Vector3 pos, global::TargetTag targetTag, float radius, SkillTargetCamp skillTargetCamp = SkillTargetCamp.None, Skill skill = null)
	{
		List<Units> target_units = new List<Units>();
		Units target = null;
		if (targetTag == global::TargetTag.Hero)
		{
			target = PlayerControlMgr.Instance.GetSelectedTarget();
			if (target != null && Vector3.Distance(pos, target.mTransform.position) < radius)
			{
				return target;
			}
			target = null;
		}
		else
		{
			if (skill == null)
			{
				target = FindTargetHelper.FindNearst(self, pos, targetTag);
			}
			else
			{
				target = FindTargetHelper.findClosestTarget(self, pos, skillTargetCamp, targetTag, skill.data.selectRangeType, skill.data.selectRange1, skill.data.selectRange2);
			}
			if (target != null && Vector3.Distance(pos, target.mTransform.position) < radius)
			{
				return target;
			}
			target = null;
		}
		IList<Units> allHeroes = MapManager.Instance.GetAllHeroes();
		for (int i = 0; i < allHeroes.Count; i++)
		{
			target = allHeroes[i];
			if (Vector3.Distance(self.mTransform.position, target.mTransform.position) <= radius)
			{
				if (!(target == null) && target.isLive && target.CanSkillSelected && target.IsManualSelectable())
				{
					if ((TeamManager.CanAttack(self, target) || (self != target && target.isHero && skillTargetCamp == SkillTargetCamp.AllWhitOutSelf) || skillTargetCamp == SkillTargetCamp.Partener || skillTargetCamp == SkillTargetCamp.All) && !target_units.Contains(target))
					{
						target_units.Add(target);
					}
				}
			}
		}
		target = null;
		if (target_units.Count == 0)
		{
			return target;
		}
		IList<Units> mapUnits = MapManager.Instance.GetMapUnits(self.TeamType, global::TargetTag.Hero);
		Units x = mapUnits.FirstOrDefault(delegate(Units unit)
		{
			target = unit.GetAttackTarget();
			if (target == null)
			{
				target = unit.GetSkillHitedTarget();
			}
			if (target == null)
			{
				target = unit.GetAttackedYouTarget();
			}
			if (target == null)
			{
				target = unit.GetSkillHitedYouTarget();
			}
			return target_units.Contains(target);
		});
		if (x == null)
		{
			FindTargetHelper.SortTargets(self, GlobalSettings.Instance.AttackSortType, ref target_units);
			target = target_units[0];
		}
		return target;
	}

	public static Units FindNearstLabisi(Units self, Vector3 pos)
	{
		Units result = null;
		float num = 999999f;
		Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
		foreach (KeyValuePair<int, Units> current in allMapUnits)
		{
			if (current.Value.TeamType == self.TeamType && current.Value.UnitType == UnitType.LabisiUnit)
			{
				float num2 = Vector3.Distance(self.mTransform.position, current.Value.mTransform.position);
				if (num2 < num)
				{
					num = num2;
					result = current.Value;
				}
			}
		}
		return result;
	}

	public static Units FindNearst(Units self, Vector3 pos, global::TargetTag targetTag)
	{
		Units result = null;
		float num = 999999f;
		List<Units> mapUnits = MapManager.Instance.GetMapUnits(targetTag);
		for (int i = 0; i < mapUnits.Count; i++)
		{
			if (self != mapUnits[i])
			{
				float num2 = Vector3.Distance(self.mTransform.position, mapUnits[i].mTransform.position);
				if (num2 < num)
				{
					num = num2;
					result = mapUnits[i];
				}
			}
		}
		return result;
	}

	public static List<Units> TargetsFromNearToFar(List<Units> enemys, Units units = null)
	{
		for (int i = 0; i <= enemys.Count - 1; i++)
		{
			for (int j = 0; j < enemys.Count - i - 1; j++)
			{
				if (UnitFeature.DistanceToPointSqr(units.transform.position, enemys[j].transform.position) > UnitFeature.DistanceToPointSqr(units.transform.position, enemys[j + 1].transform.position))
				{
					Units value = enemys[j];
					enemys[j] = enemys[j + 1];
					enemys[j + 1] = value;
				}
			}
		}
		return enemys;
	}

	public static IList<Units> TargetsFromNearToFar(IList<Units> enemys, Units units = null)
	{
		for (int i = 0; i <= enemys.Count - 1; i++)
		{
			for (int j = 0; j < enemys.Count - i - 1; j++)
			{
				if (UnitFeature.DistanceToPointSqr(units.transform.position, enemys[j].transform.position) > UnitFeature.DistanceToPointSqr(units.transform.position, enemys[j + 1].transform.position))
				{
					Units value = enemys[j];
					enemys[j] = enemys[j + 1];
					enemys[j + 1] = value;
				}
			}
		}
		return enemys;
	}
}
