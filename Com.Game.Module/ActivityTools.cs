using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using System;
using System.Collections.Generic;

namespace Com.Game.Module
{
	public static class ActivityTools
	{
		private static List<BannerInfo> listBannerActivity;

		public static BannerInfo GetBannerActivityByID(int id)
		{
			if (ActivityTools.listBannerActivity == null)
			{
				ActivityTools.RefreshBannerActivityList();
			}
			if (ActivityTools.listBannerActivity != null && id >= 0 && id < ActivityTools.listBannerActivity.Count)
			{
				return ActivityTools.listBannerActivity[id];
			}
			return null;
		}

		public static void RefreshBannerActivityList()
		{
			Dictionary<string, SysActivityVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysActivityVo>();
			ActivityTools.listBannerActivity = new List<BannerInfo>();
			foreach (KeyValuePair<string, SysActivityVo> current in typeDicByType)
			{
				if (current.Value.banner_type == 1)
				{
					if (!string.IsNullOrEmpty(current.Value.module))
					{
						DateTime serverCurrentTime = ToolsFacade.ServerCurrentTime;
						DateTime dateTime = ActivityTools.GetDateTime(current.Value.show_end_time, false);
						DateTime dateTime2 = ActivityTools.GetDateTime(current.Value.show_start_time, true);
						if (!(serverCurrentTime > dateTime) && !(serverCurrentTime < dateTime2))
						{
							string[] array = current.Value.module.Split(new char[]
							{
								','
							});
							SysActivityModuleVo sysActivityModuleVo = null;
							for (int i = 0; i < array.Length; i++)
							{
								SysActivityModuleVo dataById = BaseDataMgr.instance.GetDataById<SysActivityModuleVo>(array[i]);
								if (dataById != null)
								{
									if (dataById.type == 3)
									{
										sysActivityModuleVo = dataById;
										break;
									}
								}
							}
							if (sysActivityModuleVo != null)
							{
								BannerInfo item = new BannerInfo
								{
									activityConfig = current.Value,
									showEndTime = dateTime,
									id = current.Value.id,
									picModuleConfig = sysActivityModuleVo
								};
								ActivityTools.listBannerActivity.Add(item);
							}
						}
					}
				}
			}
			ActivityTools.listBannerActivity.Sort(new Comparison<BannerInfo>(ActivityTools.Comparision_SysActivityVo));
			if (ActivityTools.listBannerActivity.Count > 0 && ActivityTools.listBannerActivity.Count < 4)
			{
				int num = 0;
				do
				{
					ActivityTools.listBannerActivity.Add(ActivityTools.listBannerActivity[num++]);
				}
				while (ActivityTools.listBannerActivity.Count < 4);
			}
		}

		private static int Comparision_SysActivityVo(BannerInfo a, BannerInfo b)
		{
			int result;
			if (a == null)
			{
				result = 1;
			}
			else if (b == null)
			{
				result = -1;
			}
			else
			{
				if (a.id < b.id)
				{
					return -1;
				}
				if (a.id > b.id)
				{
					return 1;
				}
				return 0;
			}
			return result;
		}

		public static DateTime GetDateTime(string time, bool bMin = true)
		{
			DateTime result = new DateTime(2000, 1, 1);
			if (!string.IsNullOrEmpty(time))
			{
				if (time.Equals("[]"))
				{
					time = ((!bMin) ? "2016|12|23|0|0|0" : "2015|12|23|0|0|0");
				}
				string[] array = time.Split(new char[]
				{
					'|'
				});
				if (array.Length != 6)
				{
					ClientLogger.Error("活动配置结束时间错误：" + time);
				}
				else
				{
					int[] array2 = new int[6];
					int i;
					for (i = 0; i < array.Length; i++)
					{
						if (!int.TryParse(array[i], out array2[i]))
						{
							break;
						}
					}
					if (i >= array.Length)
					{
						result = new DateTime(array2[0], array2[1], array2[2], array2[3], array2[4], array2[5]);
					}
				}
			}
			return result;
		}
	}
}
