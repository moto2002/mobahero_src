using Assets.Scripts.Model;
using System;

public static class Tools_IDTransform
{
	public static long GetUserIdBySummId(this ToolsFacade facade, long summonerid)
	{
		int num = ModelManager.Instance.Get_currSelectedAreaId();
		if (num == 0)
		{
			return (long)(num * 5000000) + summonerid + 100000L;
		}
		return (long)(num * 5000000) + summonerid;
	}

	public static long GetSummIdByUserid(this ToolsFacade facade, long userid)
	{
		int num = ModelManager.Instance.Get_currSelectedAreaId();
		long num2 = (long)(num * 5000000);
		if (num == 0)
		{
			return (userid - num2 - 100000L <= 0L) ? 0L : (userid - num2 - 100000L);
		}
		return (userid - num2 <= 0L) ? 0L : (userid - num2);
	}
}
