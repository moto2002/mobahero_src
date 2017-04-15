using System;
using UnityEngine;

public class HomeAtlasPreloader : MonoBehaviour
{
	private static GameObject _homeAtlasContainer;

	public static void Load()
	{
		if (!HomeAtlasPreloader._homeAtlasContainer)
		{
			GameObject original = Resources.Load<GameObject>("HomeAtlas");
			HomeAtlasPreloader._homeAtlasContainer = (GameObject)UnityEngine.Object.Instantiate(original);
			UnityEngine.Object.DontDestroyOnLoad(HomeAtlasPreloader._homeAtlasContainer);
		}
	}

	public static void Unload()
	{
		if (HomeAtlasPreloader._homeAtlasContainer)
		{
			UnityEngine.Object.Destroy(HomeAtlasPreloader._homeAtlasContainer);
			HomeAtlasPreloader._homeAtlasContainer = null;
		}
	}
}
