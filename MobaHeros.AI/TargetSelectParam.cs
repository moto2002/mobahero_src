using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaHeros.AI
{
	internal class TargetSelectParam
	{
		private static TargetSelectParam _instance;

		private static Dictionary<int, object> _allParms = new Dictionary<int, object>();

		private static Dictionary<int, Dictionary<int, List<float>>> _allRates = new Dictionary<int, Dictionary<int, List<float>>>();

		public static TargetSelectParam Instance
		{
			get
			{
				if (TargetSelectParam._instance == null)
				{
					TargetSelectParam._instance = new TargetSelectParam();
				}
				return TargetSelectParam._instance;
			}
		}

		private TargetSelectParam()
		{
			this.InitParm();
			this.InitRate();
		}

		private void InitParm()
		{
			TargetSelectParam._allParms.Add(1, 0.05f);
			TargetSelectParam._allParms.Add(2, 0.2f);
			TargetSelectParam._allParms.Add(3, 0.2f);
			TargetSelectParam._allParms.Add(4, null);
			TargetSelectParam._allParms.Add(5, 0);
			TargetSelectParam._allParms.Add(6, null);
			TargetSelectParam._allParms.Add(7, null);
			TargetSelectParam._allParms.Add(8, null);
			TargetSelectParam._allParms.Add(9, null);
			TargetSelectParam._allParms.Add(10, null);
		}

		private void InitRate()
		{
			TargetSelectParam._allRates.Add(1, this.GetData(new float[]
			{
				90f
			}, new float[]
			{
				60f
			}, new float[]
			{
				30f
			}));
			TargetSelectParam._allRates.Add(2, this.GetData(new float[]
			{
				20f,
				2f
			}, new float[]
			{
				35f,
				2.3f
			}, new float[]
			{
				40f,
				3f
			}));
			TargetSelectParam._allRates.Add(3, this.GetData(new float[]
			{
				25f,
				1f
			}, new float[]
			{
				35f,
				1.3f
			}, new float[]
			{
				45f,
				2f
			}));
			TargetSelectParam._allRates.Add(4, this.GetData(new float[]
			{
				30f,
				1f
			}, new float[]
			{
				40f,
				1.3f
			}, new float[]
			{
				50f,
				2f
			}));
			TargetSelectParam._allRates.Add(5, this.GetData(new float[]
			{
				30f,
				1f
			}, new float[]
			{
				40f,
				1.3f
			}, new float[]
			{
				60f,
				2f
			}));
			TargetSelectParam._allRates.Add(6, this.GetData(new float[]
			{
				10f,
				1f
			}, new float[]
			{
				50f,
				3f
			}, new float[]
			{
				50f,
				3.8f
			}));
			TargetSelectParam._allRates.Add(7, this.GetData(new float[]
			{
				10f,
				0.5f,
				1f
			}, new float[]
			{
				20f,
				1f,
				1.8f
			}, new float[]
			{
				50f,
				1f,
				2.8f
			}));
		}

		private Dictionary<int, List<float>> GetData(float[] arr1, float[] arr2, float[] arr3)
		{
			return new Dictionary<int, List<float>>
			{
				{
					1,
					this.GetList(arr1)
				},
				{
					2,
					this.GetList(arr2)
				},
				{
					3,
					this.GetList(arr3)
				}
			};
		}

		private List<float> GetList(float[] arr)
		{
			List<float> list = new List<float>();
			for (int i = 0; i < arr.Length; i++)
			{
				float item = arr[i];
				list.Add(item);
			}
			return list;
		}

		public object GetParam(int priority)
		{
			if (TargetSelectParam._allParms.ContainsKey(priority))
			{
				return TargetSelectParam._allParms[priority];
			}
			return null;
		}

		public List<float> GetRateList(int priority, int stage)
		{
			if (TargetSelectParam._allRates.ContainsKey(priority) && TargetSelectParam._allRates[priority].ContainsKey(stage))
			{
				return TargetSelectParam._allRates[priority][stage];
			}
			return null;
		}

		public float GetPossibility(TargetPriority priority, TeamType team)
		{
			float result = 0f;
			Strategy strategy = this.GetStrategy(team);
			float strategyFactor = strategy.StrategyFactor;
			int curStage = StrategyHelper.GetCurStage(strategyFactor);
			switch (priority)
			{
			case TargetPriority.First_Monster_Low_Hp:
				result = this.GetFirstRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			case TargetPriority.Second_Hero_Low_Hp:
				result = this.GetSecondRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			case TargetPriority.Third_Hero_Assist_League:
				result = this.GetThirdRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			case TargetPriority.Fourth_State_Stuck_Hero:
				result = this.GetFourthRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			case TargetPriority.Fifth_State_Speed_Hero:
				result = this.GetFifthRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			case TargetPriority.Sixth_Attack_Self_Hero:
				result = this.GetSixthRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			case TargetPriority.Seventh_Attack_League_hero:
				result = this.GetSeventhRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			case TargetPriority.Eighth_Attack_League_Monster:
				result = this.GetEighthRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			case TargetPriority.Ninth_First_Insight:
				result = this.GetNinthRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			case TargetPriority.Last_Outsight:
				result = this.GetLastRate(strategy, this.GetRateList((int)priority, curStage));
				break;
			}
			return result;
		}

		private Strategy GetStrategy(TeamType team)
		{
			return StrategyManager.Instance.GetStrategy(team);
		}

		private float GetFirstRate(Strategy strategy, List<float> args)
		{
			return args[0] - Mathf.Abs(strategy.StrategyFactor);
		}

		private float GetSecondRate(Strategy stra, List<float> args)
		{
			return args[0] + stra.StrategyFactor + (stra.LeagueLeftManaFactor + stra.LeagueSkillCoolFactor) * args[1];
		}

		private float GetThirdRate(Strategy stra, List<float> args)
		{
			return args[0] + stra.StrategyFactor + (stra.LeagueLeftManaFactor + stra.LeagueSkillCoolFactor) * args[1];
		}

		private float GetFourthRate(Strategy stra, List<float> args)
		{
			return args[0] + stra.StrategyFactor + (stra.LeagueLeftManaFactor + stra.LeagueSkillCoolFactor) * args[1];
		}

		private float GetFifthRate(Strategy stra, List<float> args)
		{
			return args[0] + stra.StrategyFactor + (stra.LeagueLeftManaFactor + stra.LeagueSkillCoolFactor) * args[1];
		}

		private float GetSixthRate(Strategy stra, List<float> args)
		{
			return args[0] + stra.StrategyFactor + (stra.LeagueAverageHpFactor - 5f) * args[1];
		}

		private float GetSeventhRate(Strategy stra, List<float> args)
		{
			return args[0] + stra.StrategyFactor * args[1] + (stra.LeagueAverageHpFactor - 5f) * args[2];
		}

		private float GetEighthRate(Strategy stra, List<float> args)
		{
			return 100f;
		}

		private float GetNinthRate(Strategy stra, List<float> args)
		{
			return 100f;
		}

		private float GetLastRate(Strategy stra, List<float> args)
		{
			return 100f;
		}
	}
}
