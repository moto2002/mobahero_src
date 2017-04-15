using Com.Game.Utils;
using System;
using UnityEngine;

namespace MobaTools
{
	public class GlobalSetting
	{
		public static readonly string CharacterPath = string.Empty;

		public static readonly string TexturePath = string.Empty;

		public static readonly string EffectPath = string.Empty;

		public static readonly string AssetExportPath = "/Export/AssetBundle/";

		public static readonly string AssetSavePath = "Assets" + GlobalSetting.AssetExportPath;

		public static string ConverToFtpPath(string bundlename)
		{
			return string.Concat(new string[]
			{
				GlobalSetting.GetApplicationDataPath(),
				GlobalSetting.GetPlatformName(),
				"/",
				GlobalSetting.AssetExportPath,
				bundlename,
				".assetbundle"
			});
		}

		public static string ConvertToAssetBundleName(string ResName)
		{
			string text = ResName.Replace('/', '.');
			ClientLogger.Info("==> ConvertToAssetBundleName : " + text);
			return text;
		}

		public static string ConvertToResPath(string bundleName)
		{
			string text = bundleName.Replace('.', '/');
			ClientLogger.Info("==> ConvertToResPath : " + text);
			return text;
		}

		public static string LoadConfigFile(string name)
		{
			string applicationDataPath = GlobalSetting.GetApplicationDataPath();
			string a = FileUtils.LoadFileByLine(applicationDataPath, name);
			string result = null;
			if (a != "error")
			{
				ClientLogger.Info("LoadConfigFile : 读取配置文件错误！");
			}
			else
			{
				FileUtils.CreateFile(applicationDataPath, name, "192.168.200.252");
				result = FileUtils.LoadFileByLine(applicationDataPath, name);
			}
			return result;
		}

		public static string GetApplicationDataPath()
		{
			string result = string.Empty;
			if (Application.platform == RuntimePlatform.Android)
			{
				result = Application.persistentDataPath;
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				result = Application.persistentDataPath;
			}
			else if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				result = Application.dataPath;
			}
			else
			{
				result = Application.dataPath;
			}
			return result;
		}

		public static string GetRuntimeStreamingAssetsPath()
		{
			string text;
			if (Application.platform == RuntimePlatform.Android)
			{
				text = "jar:file://" + Application.persistentDataPath + "!/assets/";
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				text = Application.persistentDataPath + "/Raw/";
			}
			else if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				text = "file://" + Application.dataPath + "/StreamingAssets/";
			}
			else
			{
				text = "file://" + Application.dataPath + "/StreamingAssets/";
			}
			ClientLogger.Info("==> StreamingAssetsPath : " + text);
			return text;
		}

		public static string GetResourcePath()
		{
			string text = GlobalSetting.AssetSavePath;
			switch (Application.platform)
			{
			case RuntimePlatform.OSXPlayer:
				text += "Mac/";
				break;
			case RuntimePlatform.WindowsPlayer:
				text += "Windows/";
				break;
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
				text += "WebPlayer/";
				break;
			case RuntimePlatform.IPhonePlayer:
				text += "IOS/";
				break;
			case RuntimePlatform.Android:
				text += "Android/";
				break;
			}
			ClientLogger.Info("==> GetResourcePath : " + text);
			return text;
		}

		public static string GetPlatformName()
		{
			string result = "Android";
			switch (Application.platform)
			{
			case RuntimePlatform.OSXPlayer:
				result = "Mac";
				break;
			case RuntimePlatform.WindowsPlayer:
				result = "Windows32";
				break;
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
				result = "WebPlayer";
				break;
			case RuntimePlatform.IPhonePlayer:
				result = "IOS";
				break;
			case RuntimePlatform.Android:
				result = "Android";
				break;
			}
			return result;
		}

		public static int GetDriverVersion()
		{
			string[] array = SystemInfo.operatingSystem.Split(new char[]
			{
				' '
			});
			string[] array2 = array[array.Length - 1].Split(new char[]
			{
				'.'
			});
			return int.Parse(array2[0]);
		}
	}
}
