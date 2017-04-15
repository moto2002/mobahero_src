using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CachedRes
{
	private const string TEMP_SPELL_NOT_FOUND = "spellprefab:{0,-15} is not found!";

	private static Dictionary<string, GameObject> _resUnits = new Dictionary<string, GameObject>();

	private static Dictionary<string, UnityEngine.Object> _textures = new Dictionary<string, UnityEngine.Object>();

	private static Dictionary<string, Texture[]> _fullImgs = new Dictionary<string, Texture[]>();

	private static Dictionary<string, GameObject> _uiTemps = new Dictionary<string, GameObject>();

	private static Dictionary<string, UnityEngine.Object> _audios = new Dictionary<string, UnityEngine.Object>();

	private static Dictionary<string, UnityEngine.Object> _spellsPrefabs = new Dictionary<string, UnityEngine.Object>();

	private static Dictionary<string, UnityEngine.Object> _res = null;

	public static GameObject getUnitAtResPath(string resPath)
	{
		return Resources.Load(resPath) as GameObject;
	}

	public static Texture getSkinTex(int skinId)
	{
		SysHeroSkinVo sysHeroSkinVo = BaseDataMgr.instance.GetDicByType<SysHeroSkinVo>()[skinId.ToString()] as SysHeroSkinVo;
		SysGameResVo sysGameResVo = BaseDataMgr.instance.GetDicByType<SysGameResVo>()[sysHeroSkinVo.Loading_icon] as SysGameResVo;
		string path = sysGameResVo.path;
		return CachedRes.getTex(path);
	}

	public static Texture getTex(string path)
	{
		return CachedRes.getFrom(path, ref CachedRes._textures, false) as Texture;
	}

	public static void ClearResources()
	{
		if (CachedRes._resUnits != null)
		{
			CachedRes._resUnits.Clear();
		}
		if (CachedRes._textures != null)
		{
			CachedRes._textures.Clear();
		}
		if (CachedRes._fullImgs != null)
		{
			CachedRes._fullImgs.Clear();
		}
		if (CachedRes._uiTemps != null)
		{
			CachedRes._uiTemps.Clear();
		}
		if (CachedRes._audios != null)
		{
			CachedRes._audios.Clear();
		}
		if (CachedRes._spellsPrefabs != null)
		{
			CachedRes._spellsPrefabs.Clear();
		}
		if (CachedRes._res != null)
		{
			CachedRes._res.Clear();
		}
	}

	public static GameObject getUITemplate(string uiName)
	{
		if (CachedRes._uiTemps.ContainsKey(uiName))
		{
			return CachedRes._uiTemps[uiName];
		}
		string path = "UITemplate/" + uiName;
		GameObject gameObject = Resources.Load(path) as GameObject;
		if (gameObject != null)
		{
			CachedRes._uiTemps.Add(uiName, gameObject);
			return CachedRes._uiTemps[uiName];
		}
		throw new NotFoundException(uiName);
	}

	public static AudioClip getAudio(string fileName)
	{
		string path = "Audio/" + fileName;
		return CachedRes.getFrom(path, ref CachedRes._audios, false) as AudioClip;
	}

	public static GameObject getClonedUnit(string resPath)
	{
		return UnityEngine.Object.Instantiate(CachedRes.getUnitAtResPath(resPath)) as GameObject;
	}

	public static GameObject getSpellPrefab(string resPath, bool ignoreError = false)
	{
		return CachedRes.getFrom(resPath, ref CachedRes._spellsPrefabs, ignoreError) as GameObject;
	}

	public static UnityEngine.Object loadRes(string path)
	{
		return CachedRes.getFrom(path, ref CachedRes._res, false);
	}

	private static UnityEngine.Object getFrom(string path, ref Dictionary<string, UnityEngine.Object> cache, bool ignoreError = false)
	{
		if (CachedRes._res == null)
		{
			CachedRes._res = new Dictionary<string, UnityEngine.Object>();
		}
		if (cache.ContainsKey(path))
		{
			return cache[path];
		}
		UnityEngine.Object @object = Resources.Load(path);
		if (!(@object == null))
		{
			cache.Add(path, @object);
			return @object;
		}
		if (!ignoreError)
		{
			Debug.LogError("-----------------XXXX---------------- " + string.Format("resource:{0} is not found!", path));
			return null;
		}
		return null;
	}

	public static Texture2D GetShinedFigure(string fig)
	{
		string text = null;
		if (!string.IsNullOrEmpty(fig))
		{
			text = "Texture/MagicBottle/Shined_figures_" + fig;
		}
		if (!string.IsNullOrEmpty(text))
		{
			return (Texture2D)CachedRes.getTex(text);
		}
		return null;
	}
}
