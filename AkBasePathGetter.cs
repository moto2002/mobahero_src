using System;
using System.IO;
using System.Reflection;

public class AkBasePathGetter
{
	public static string GetPlatformName()
	{
		try
		{
			Type type = Type.GetType("AkCustomPlatformNameGetter");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("GetPlatformName");
				if (method != null)
				{
					string text = (string)method.Invoke(null, null);
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
			}
		}
		catch
		{
		}
		return "Android";
	}

	public static string GetPlatformBasePath()
	{
		string result = string.Empty;
		result = Path.Combine(AkBasePathGetter.GetFullSoundBankPath(), AkBasePathGetter.GetPlatformName());
		AkBasePathGetter.FixSlashes(ref result);
		return result;
	}

	public static string GetFullSoundBankPath()
	{
		string basePath = AkInitializer.GetBasePath();
		AkBasePathGetter.FixSlashes(ref basePath);
		return basePath;
	}

	public static void FixSlashes(ref string path)
	{
		string text = Path.DirectorySeparatorChar.ToString();
		string oldValue = string.Empty;
		if (Path.DirectorySeparatorChar == '/')
		{
			oldValue = "\\";
		}
		else
		{
			oldValue = "/";
		}
		path.Trim();
		path = path.Replace(oldValue, text);
		path = path.TrimStart(new char[]
		{
			'\\'
		});
		if (!path.EndsWith(text))
		{
			path += text;
		}
	}

	public static string GetValidBasePath()
	{
		return AkBasePathGetter.GetPlatformBasePath();
	}
}
