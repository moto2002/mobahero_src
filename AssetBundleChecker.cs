using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class AssetBundleChecker
{
	public const string BUNDLE_INFOES = "BundleInfoes";

	public const string LOCALBUNDLE_INFOES = "LocalBundles";

	public const string BUNDLE_INFOES_XML = "BundleInfoes.xml";

	private static string m_assetroot = "Assets/";

	public static string m_targetFolderPath = "Bundles/";

	public static string m_androidFolderPath = "Android/";

	private static string m_iosFolderPath = "iPhone/";

	private static string m_standaloneFolderPath = "Standalone/";

	private static string m_expandedName = ".unity3d";

	private static string m_streamassetspath = "StreamingAssets/";

	private static string m_resourcepath = "Resources/";

	private static string m_outputbundlespath = "../../BundlesOutput/";

	private static string m_packagepath = AssetBundleChecker.m_assetroot + AssetBundleChecker.m_resourcepath;

	private static string m_testbindatafolder = "Test";

	public static string LOCAL_BUNDLE_INFO_XML_PSERSIST_FOLDER
	{
		get
		{
			return Application.persistentDataPath + "/";
		}
	}

	public static string BUNDLE_INFO_FILE_STREAMING_PATH
	{
		get
		{
			return Application.streamingAssetsPath + "/BundleInfoes.xml";
		}
	}

	public static string BUNDLE_INFO_XML_PERSIST_PATH
	{
		get
		{
			return AssetBundleChecker.LOCAL_BUNDLE_INFO_XML_PSERSIST_FOLDER + "BundleInfoes.xml";
		}
	}

	public static List<Bundle2Check> compareBundleInfoes(string text, string localPath)
	{
		List<Bundle2Check> list = new List<Bundle2Check>();
		Dictionary<string, Bundle2Check> dictionary = AssetBundleChecker.ReadLocalXML();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(text);
		XmlElement documentElement = xmlDocument.DocumentElement;
		string empty = string.Empty;
		foreach (XmlNode xmlNode in documentElement.ChildNodes)
		{
			if (xmlNode is XmlElement)
			{
				Bundle2Check info = AssetBundleChecker.GetInfo(xmlNode);
				if (dictionary.ContainsKey(info.bundleName))
				{
					string path = Application.persistentDataPath + "/" + info.bundleName + ".unity3d";
					if (!File.Exists(path) || !info.bundleMD5.Equals(dictionary[info.bundleName].bundleMD5))
					{
						Bundle2Check item = new Bundle2Check
						{
							assetType = info.assetType,
							bundleMD5 = info.bundleMD5,
							bundleVersion = info.bundleVersion,
							bundleName = info.bundleName,
							bundleSize = info.bundleSize
						};
						list.Add(item);
					}
				}
				else
				{
					Bundle2Check item2 = new Bundle2Check
					{
						assetType = info.assetType,
						bundleMD5 = info.bundleMD5,
						bundleVersion = info.bundleVersion,
						bundleName = info.bundleName,
						bundleSize = info.bundleSize
					};
					list.Add(item2);
				}
				AssetBundleInfo.Init(info.bundleName, info.assetType);
				AssetCache.Init(info.bundleName);
			}
		}
		return list;
	}

	private static Dictionary<string, Bundle2Check> ReadLocalXML()
	{
		Dictionary<string, Bundle2Check> dictionary = new Dictionary<string, Bundle2Check>();
		if (!File.Exists(AssetBundleChecker.BUNDLE_INFO_XML_PERSIST_PATH))
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement newChild = xmlDocument.CreateElement("BundleInfoes");
			xmlDocument.AppendChild(newChild);
			xmlDocument.Save(AssetBundleChecker.BUNDLE_INFO_XML_PERSIST_PATH);
		}
		XmlDocument xmlDocument2 = new XmlDocument();
		xmlDocument2.Load(AssetBundleChecker.BUNDLE_INFO_XML_PERSIST_PATH);
		XmlElement documentElement = xmlDocument2.DocumentElement;
		foreach (XmlNode xmlNode in documentElement.ChildNodes)
		{
			if (xmlNode is XmlElement)
			{
				Bundle2Check info = AssetBundleChecker.GetInfo(xmlNode);
				if (!dictionary.ContainsKey(info.bundleName))
				{
					dictionary.Add(info.bundleName, info);
				}
			}
		}
		return dictionary;
	}

	private static Bundle2Check GetInfo(XmlNode node)
	{
		Bundle2Check bundle2Check = new Bundle2Check();
		string attribute = (node as XmlElement).GetAttribute("bundleName");
		bundle2Check.bundleName = attribute;
		bundle2Check.assetType = int.Parse((node as XmlElement).GetAttribute("assetType"));
		bundle2Check.bundleVersion = int.Parse((node as XmlElement).GetAttribute("bundleVersion"));
		bundle2Check.bundleMD5 = (node as XmlElement).GetAttribute("bundleMD5");
		bundle2Check.bundleSize = long.Parse((node as XmlElement).GetAttribute("bundleSize"));
		return bundle2Check;
	}

	public static void OverrideLocalXML(string bundleName, int assetType, string md5Code, int newVersion, long nbundleSize)
	{
		Dictionary<string, Bundle2Check> dictionary = AssetBundleChecker.ReadLocalXML();
		if (dictionary.ContainsKey(bundleName))
		{
			if (!dictionary[bundleName].bundleMD5.Equals(md5Code))
			{
				dictionary[bundleName].bundleVersion = newVersion;
				dictionary[bundleName].bundleMD5 = md5Code;
				dictionary[bundleName].bundleSize = nbundleSize;
			}
		}
		else
		{
			dictionary.Add(bundleName, new Bundle2Check
			{
				bundleVersion = newVersion,
				bundleMD5 = md5Code,
				assetType = assetType,
				bundleSize = nbundleSize
			});
		}
		if (File.Exists(AssetBundleChecker.BUNDLE_INFO_XML_PERSIST_PATH))
		{
			File.Delete(AssetBundleChecker.BUNDLE_INFO_XML_PERSIST_PATH);
		}
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = xmlDocument.CreateElement("BundleInfoes");
		xmlDocument.AppendChild(xmlElement);
		foreach (KeyValuePair<string, Bundle2Check> current in dictionary)
		{
			XmlElement xmlElement2 = xmlDocument.CreateElement("Info");
			xmlElement.AppendChild(xmlElement2);
			xmlElement2.SetAttribute("bundleMD5", current.Value.bundleMD5.ToString());
			xmlElement2.SetAttribute("bundleVersion", current.Value.bundleVersion.ToString());
			xmlElement2.SetAttribute("assetType", current.Value.assetType.ToString());
			xmlElement2.SetAttribute("bundleName", current.Key);
			xmlElement2.SetAttribute("bundleSize", current.Value.bundleSize.ToString());
		}
		xmlDocument.Save(AssetBundleChecker.BUNDLE_INFO_XML_PERSIST_PATH);
		xmlDocument = null;
	}
}
