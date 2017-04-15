using MobaFrame.SkillAction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HomeGCManager : MonoBehaviour
{
	private string[] _homeAtlasNames = new string[]
	{
		"SelectHero Atlas",
		"Common",
		"CommonbgAtlas",
		"Banner",
		"Friend",
		"HIKL",
		"Home",
		"Icon",
		"Sacrificial",
		"PlayerInformation",
		"AllButton",
		"BattleEntrance",
		"Hero_rune",
		"ArenaModeAtlas",
		"SummonerRegister",
		"Rank",
		"CheckIn",
		"GetItems",
		"RuneIconsAtlas",
		"Achievement",
		"DataStatistics",
		"Store",
		"HomePay",
		"HomeDoubleCard",
		"HomeChat",
		"HomeActivity",
		"Passport",
		"HeadportraitAtlas",
		"BottleSystem",
		"NewYearGift"
	};

	private string[] _newbieGuideAtlasNames = new string[]
	{
		"newbie_atlas1",
		"NewbieLoadAtlas"
	};

	private string[] _loadLevelLoading = new string[]
	{
		"PublicOther"
	};

	private string[] _loginAtlasNames = new string[]
	{
		"Login",
		"SelectServer"
	};

	private string[] _pvploadingAtlasNames = new string[]
	{
		"PvpLoadingAtlas",
		"LoadingCardboardAtlas"
	};

	private string[] _battlingAtlasNames = new string[]
	{
		"HUD_small_shop",
		"HUD_MapBig",
		"HUD_Map",
		"HUD_instructions",
		"HUD_Extra",
		"HUD_Blood",
		"HUD_big",
		"HUD_BattleShop",
		"HUD",
		"HUD_get_goldFont",
		"BattleUI",
		"SmallHeroIcon",
		"MapHeroIcon",
		"SummerSkill",
		"Unlock",
		"BuffIcons",
		"HUDNumber"
	};

	private string[] _battleSettlementAtlasNames = new string[]
	{
		"Settlement_Ring",
		"Settlement",
		"GoldenFont",
		"GetItems",
		"Equipment"
	};

	private string _battlingHeroSkillIconAtlasName = string.Empty;

	public static HomeGCManager Instance;

	private List<Texture> textrues = new List<Texture>();

	private List<string> _recordedBattleTextureNames = new List<string>();

	private void Awake()
	{
		HomeGCManager.Instance = this;
	}

	private void Start()
	{
		this.DoInit();
	}

	public void DoClearToBattleLoading()
	{
		HomeAtlasPreloader.Unload();
		CachedRes.ClearResources();
		ResourceManager.ClearResources();
		UIAtlas[] loadedUIAtlas = this.GetLoadedUIAtlas();
		this.ClearUIAtlasTexture(this._homeAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._loadLevelLoading, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._loginAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._battlingAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._battleSettlementAtlasNames, loadedUIAtlas);
		this.DoClearTextures();
		Resources.UnloadUnusedAssets();
	}

	private UIAtlas[] GetLoadedUIAtlas()
	{
		return Resources.FindObjectsOfTypeAll<UIAtlas>();
	}

	private Font[] GetLoadedFont()
	{
		return Resources.FindObjectsOfTypeAll<Font>();
	}

	private Texture[] GetLoadedTexture()
	{
		return Resources.FindObjectsOfTypeAll<Texture>();
	}

	private Mesh[] GetLoadedMesh()
	{
		return Resources.FindObjectsOfTypeAll<Mesh>();
	}

	public void DoClearToHome()
	{
		CachedRes.ClearResources();
		ResourceManager.ClearResources();
		UIAtlas[] loadedUIAtlas = this.GetLoadedUIAtlas();
		this.ClearUIAtlasTexture(this._newbieGuideAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._loginAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._pvploadingAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._battlingAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._battleSettlementAtlasNames, loadedUIAtlas);
		this.DoClearTextures();
		Resources.UnloadUnusedAssets();
	}

	public void DoClearTextures()
	{
		for (int i = 0; i < this.textrues.Count; i++)
		{
			Resources.UnloadAsset(this.textrues[i]);
		}
		this.textrues.Clear();
	}

	public void AddTexture(Texture tex)
	{
		if (this.textrues == null)
		{
			this.textrues = new List<Texture>();
		}
		this.textrues.Add(tex);
	}

	public void UnloadAsset(UnityEngine.Object obj)
	{
		Resources.UnloadAsset(obj);
	}

	public void UnloadUITextureAsset(GameObject obj)
	{
		if (obj != null && obj.GetComponent<UITexture>() != null)
		{
			Resources.UnloadAsset(obj.GetComponent<UITexture>().mainTexture);
		}
	}

	public void UnloadUISpriteAsset(GameObject obj)
	{
		if (obj != null && obj.GetComponent<UISprite>() != null)
		{
			Resources.UnloadAsset(obj.GetComponent<UISprite>().atlas.texture);
		}
	}

	public void DoInit()
	{
		UIAtlas[] loadedUIAtlas = this.GetLoadedUIAtlas();
		this.ClearUIAtlasTexture(this._newbieGuideAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._loadLevelLoading, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._loginAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._battlingAtlasNames, loadedUIAtlas);
		this.ClearUIAtlasTexture(this._battleSettlementAtlasNames, loadedUIAtlas);
		this.DoClearTextures();
		Resources.UnloadUnusedAssets();
	}

	private void ClearUIAtlasTexture(string[] inAtlasNames, UIAtlas[] inCurLoadedUIAtlases)
	{
		if (inAtlasNames == null || inAtlasNames.Length < 1)
		{
			return;
		}
		if (inCurLoadedUIAtlases == null || inCurLoadedUIAtlases.Length < 1)
		{
			return;
		}
		List<UIAtlas> list = new List<UIAtlas>(inCurLoadedUIAtlases);
		for (int i = 0; i < inAtlasNames.Length; i++)
		{
			UIAtlas uIAtlasByName = this.GetUIAtlasByName(inAtlasNames[i], list);
			if (uIAtlasByName != null)
			{
				Texture texture = uIAtlasByName.texture;
				if (texture != null)
				{
					Resources.UnloadAsset(texture);
				}
				if (uIAtlasByName.spriteMaterial != null && uIAtlasByName.spriteMaterial.HasProperty("_MainTex"))
				{
					texture = uIAtlasByName.spriteMaterial.GetTexture("_MainTex");
					if (texture != null)
					{
						Resources.UnloadAsset(texture);
					}
				}
				if (uIAtlasByName.spriteMaterial != null && uIAtlasByName.spriteMaterial.HasProperty("_DetailTex"))
				{
					texture = uIAtlasByName.spriteMaterial.GetTexture("_DetailTex");
					if (texture != null)
					{
						Resources.UnloadAsset(texture);
					}
				}
				list.Remove(uIAtlasByName);
			}
		}
	}

	public void ClearUIAtlasResourceImmediately(UIAtlas inAtlas)
	{
		if (inAtlas != null)
		{
			Texture texture = inAtlas.texture;
			if (texture != null)
			{
				Resources.UnloadAsset(texture);
			}
			if (inAtlas.spriteMaterial != null)
			{
				texture = inAtlas.spriteMaterial.GetTexture("_MainTex");
				if (texture != null)
				{
					Resources.UnloadAsset(texture);
				}
				texture = inAtlas.spriteMaterial.GetTexture("_DetailTex");
				if (texture != null)
				{
					Resources.UnloadAsset(texture);
				}
			}
		}
	}

	private UIAtlas GetUIAtlasByName(string inName, List<UIAtlas> inUIAtlases)
	{
		if (string.IsNullOrEmpty(inName))
		{
			return null;
		}
		if (inUIAtlases == null || inUIAtlases.Count < 1)
		{
			return null;
		}
		for (int i = 0; i < inUIAtlases.Count; i++)
		{
			if (inUIAtlases[i] != null && inUIAtlases[i].name != null && inUIAtlases[i].name.Equals(inName))
			{
				return inUIAtlases[i];
			}
		}
		return null;
	}

	public void ClearBattleSceneResources(bool inIsUnloadUnusedAsset)
	{
		UIAtlas[] loadedUIAtlas = this.GetLoadedUIAtlas();
		this.ClearUIAtlasTexture(this._battlingAtlasNames, loadedUIAtlas);
		if (!string.IsNullOrEmpty(this._battlingHeroSkillIconAtlasName))
		{
			this.ClearUIAtlasTexture(new string[]
			{
				this._battlingHeroSkillIconAtlasName
			}, loadedUIAtlas);
			this._battlingHeroSkillIconAtlasName = string.Empty;
		}
		GameObject gameObject = GameObject.Find("GlobalObject/recycler");
		if (gameObject != null)
		{
			Transform transform = gameObject.transform;
			if (transform != null)
			{
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if (child != null)
					{
						UnityEngine.Object.Destroy(child.gameObject);
					}
				}
			}
		}
		string curLevelMapName = LevelManager.GetCurLevelMapName();
		if (StringUtils.CheckValid(curLevelMapName))
		{
			gameObject = GameObject.Find(curLevelMapName);
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		if (inIsUnloadUnusedAsset)
		{
			Resources.UnloadUnusedAssets();
		}
	}

	public void ClearBattleResouces()
	{
		ClickIcon.Clear();
		SurfaceManager.ClearResources();
		ModelAnimation.ClearData();
		BaseAction.ClearResources();
		this.ClearDynamicDrawerResources();
		ResourceManager.ClearResources();
		GameObject gameObject = GameObject.Find("SpawnerPool(Clone)");
		if (gameObject != null)
		{
			PoolRoot component = gameObject.GetComponent<PoolRoot>();
			if (component != null)
			{
				component.ClearResources();
			}
		}
		this.ClearBattleSceneResources(false);
		Resources.UnloadUnusedAssets();
	}

	private void ClearDynamicDrawerResources()
	{
	}

	public void SetBattlingHeroSkillIconAtlasName(string inName)
	{
		if (!string.IsNullOrEmpty(inName) && string.IsNullOrEmpty(this._battlingHeroSkillIconAtlasName) && this._battlingHeroSkillIconAtlasName != null)
		{
			this._battlingHeroSkillIconAtlasName = inName;
		}
	}

	public void ClearUiTextureResource(UITexture inUiTexture)
	{
		if (inUiTexture != null && inUiTexture.mainTexture != null)
		{
			inUiTexture.mainTexture = null;
		}
	}

	public void ClearUiTextureResImmediate(UITexture inUiTexture)
	{
		if (inUiTexture != null && inUiTexture.mainTexture != null)
		{
			Resources.UnloadAsset(inUiTexture.mainTexture);
		}
	}

	public void ClearChildUiTextureResImmediate(GameObject inGo)
	{
		if (inGo == null)
		{
			return;
		}
		UITexture[] componentsInChildren = inGo.GetComponentsInChildren<UITexture>(true);
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.ClearUiTextureResImmediate(componentsInChildren[i]);
			}
		}
	}
}
