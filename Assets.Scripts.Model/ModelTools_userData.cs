using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Assets.Scripts.Model
{
	public static class ModelTools_userData
	{
		public static UserData Get_userData_X(this ModelManager mmng)
		{
			UserData userData = mmng.Get_userData();
			if (userData == null)
			{
				userData = mmng.GetData<UserData>(EModelType.Model_userData);
			}
			return userData;
		}

		public static T Get_userData_filed_X<T>(this ModelManager mmng, string propertyName)
		{
			T result = default(T);
			UserData userData = mmng.Get_userData_X();
			if (userData != null)
			{
				Type typeFromHandle = typeof(UserData);
				PropertyInfo property = typeFromHandle.GetProperty(propertyName);
				if (property != null)
				{
					result = (T)((object)property.GetGetMethod().Invoke(userData, new object[0]));
				}
			}
			return result;
		}

		public static void Set_userData_field_X<T>(this ModelManager mmng, string propertyName, T value)
		{
			UserData userData = mmng.Get_userData_X();
			if (userData != null)
			{
				Type typeFromHandle = typeof(UserData);
				PropertyInfo property = typeFromHandle.GetProperty(propertyName);
				if (property != null)
				{
					property.GetSetMethod().Invoke(userData, new object[]
					{
						value
					});
				}
			}
		}

		private static UserData Get_userData(this ModelManager mmng)
		{
			UserData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_userData))
			{
				result = mmng.GetData<UserData>(EModelType.Model_userData);
			}
			return result;
		}

		public static SysRankStageVo Get_LadderLevel(this ModelManager mmng)
		{
			double ladderScore = mmng.Get_userData_filed_X("LadderScore");
			int rankNum = mmng.Get_userData_filed_X("LastDayLadderRank");
			return mmng.Get_LadderLevel(ladderScore, rankNum);
		}

		public static SysRankStageVo Get_LadderLevel(this ModelManager mmng, double ladderScore, int rankNum)
		{
			List<object> list = (from obj in BaseDataMgr.instance.GetDicByType<SysRankStageVo>().Values.ToList<object>()
			where (double)((SysRankStageVo)obj).StageScore <= ladderScore
			select obj).ToList<object>();
			if (list == null)
			{
				return null;
			}
			list = (from obj in list
			where ((SysRankStageVo)obj).StageRanking == 0 || (((SysRankStageVo)obj).StageRanking != 0 && ((SysRankStageVo)obj).StageRanking > rankNum)
			select obj).ToList<object>();
			return (SysRankStageVo)list.LastOrDefault<object>();
		}

		public static SysRankStageVo Get_NextLadderLevel(this ModelManager mmng)
		{
			SysRankStageVo _rankNow = mmng.Get_LadderLevel();
			object obj2 = BaseDataMgr.instance.GetDicByType<SysRankStageVo>().Values.ToList<object>().Find((object obj) => ((SysRankStageVo)obj).RankStageId == _rankNow.RankStageId + 1);
			if (obj2 == null)
			{
				return null;
			}
			return (SysRankStageVo)obj2;
		}

		public static bool IsRepeatAvatar(this ModelManager mmng, string type, string _id)
		{
			if (type == "3")
			{
				string text = mmng.Get_userData_filed_X("OwnIconStr");
				return !string.IsNullOrEmpty(text.Trim()) && text.Contains(_id + "_");
			}
			string text2 = mmng.Get_userData_filed_X("OwnPictureFrame");
			return !string.IsNullOrEmpty(text2.Trim()) && text2.Contains(_id + "_");
		}

		public static void GetNewAvatar(this ModelManager mmng, string type, string _id)
		{
			if (type == "3")
			{
				UserData expr_16 = mmng.Get_userData_X();
				expr_16.OwnIconStr += string.Format("{0}|", _id + "_1");
			}
			else
			{
				UserData expr_46 = mmng.Get_userData_X();
				expr_46.OwnPictureFrame += string.Format("{0}|", _id + "_1");
			}
		}
	}
}
