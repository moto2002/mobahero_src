using System;

public static class Tools_NumberCheck
{
	public static int ToInt32(this ToolsFacade facade, float src)
	{
		string s = src.ToString("F0");
		int result = 0;
		int.TryParse(s, out result);
		return result;
	}

	public static bool TryParseToInt32(this ToolsFacade facade, float src, out int solution)
	{
		string s = src.ToString("F0");
		return int.TryParse(s, out solution);
	}

	public static int ToInt32(this ToolsFacade facade, double src)
	{
		string s = src.ToString("F0");
		int result = 0;
		int.TryParse(s, out result);
		return result;
	}

	public static bool TryParseToInt32(this ToolsFacade facade, double src, out int solution)
	{
		string s = src.ToString("F0");
		return int.TryParse(s, out solution);
	}
}
