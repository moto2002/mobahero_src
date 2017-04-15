using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleInfo
{
	public const int NUM_ACTOR_BUNDLES = 1;

	public const int NUM_TEXTURE_BUNDLES = 1;

	public const int NumSharedDependencyBundles = 3;

	public const string SharedBundleName = "Shared";

	public static Dictionary<string, stringBundleInfo> FamilyInfo;

	public static readonly bool UseSharedDependencyBundle;

	static AssetBundleInfo()
	{
		AssetBundleInfo.FamilyInfo = new Dictionary<string, stringBundleInfo>();
	}

	public static void Init(string bundleName, int assetType)
	{
		Type typeFromHandle;
		switch (assetType)
		{
		case 1:
			typeFromHandle = typeof(GameObject);
			goto IL_97;
		case 2:
			typeFromHandle = typeof(Texture);
			goto IL_97;
		case 3:
			typeFromHandle = typeof(AudioClip);
			goto IL_97;
		case 4:
			typeFromHandle = typeof(TextAsset);
			goto IL_97;
		case 10:
			typeFromHandle = typeof(Material);
			goto IL_97;
		}
		typeFromHandle = typeof(UnityEngine.Object);
		IL_97:
		stringBundleInfo value = new stringBundleInfo
		{
			TypeOf = typeFromHandle,
			BundleName = bundleName + ".unity3d"
		};
		if (!AssetBundleInfo.FamilyInfo.ContainsKey(bundleName))
		{
			AssetBundleInfo.FamilyInfo.Add(bundleName, value);
		}
	}
}
