using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PostMist : MonoBehaviour
{
	public Texture2D tex;

	public Texture2D footMask;

	public bool doMist = true;

	public Material mat;

	public RenderTexture bg;

	public GameObject bgCam;

	public Camera cam;

	public static int texW = 65;

	public static int texH = 37;

	private static float sight = 20f;

	private static float enemySight = 20f;

	private static float fade = 13f;

	private static float enemyFade = 13f;

	private int p;

	private int k;

	private int k2;

	private Color white = new Color(1f, 1f, 1f, 1f);

	private List<TexSight> sightTexs = new List<TexSight>(5);

	private bool inited;

	private bool dead;

	private TeamType initTeam;

	private int uid;

	private TeamType team;

	public static PostMist Instance;

	public static bool mistOn;

	private void Start()
	{
		this.tex = new Texture2D(PostMist.texW, PostMist.texH, TextureFormat.Alpha8, false);
		this.tex.name = "PostMist_" + Time.time.ToString();
		this.footMask = new Texture2D(PostMist.texW, PostMist.texH, TextureFormat.Alpha8, false);
		this.footMask.name = "PostMist_" + Time.time.ToString();
		this.tex.wrapMode = TextureWrapMode.Clamp;
		this.footMask.wrapMode = TextureWrapMode.Clamp;
		this.mat.SetTexture("_MistTex", this.tex);
		this.mat.SetTexture("Bg", this.bg);
		this.mat.SetTexture("Mask", this.footMask);
	}

	private void Update()
	{
		if (!this.doMist)
		{
			return;
		}
		this.p++;
		if (this.p % 15 == 0)
		{
			this.p = 0;
			this.ReadUnits();
		}
	}

	private void LateUpdate()
	{
		this.k++;
		this.k2++;
		if (this.k2 % 17 == 0)
		{
			this.k2 = 0;
			if (MapRuler.map != null)
			{
				MapRuler.map.FlushPixel(this.bgCam.transform.position, this.tex);
			}
		}
		if (this.k % 10 == 0)
		{
			this.k = 0;
			this.footMask.Apply();
			this.tex.Apply();
		}
	}

	public void ResetMist()
	{
		if (this.sightTexs != null)
		{
			this.sightTexs.Clear();
		}
		if (this.tex != null)
		{
			for (int i = 0; i < this.tex.width; i++)
			{
				for (int j = 0; j < this.tex.height; j++)
				{
					this.tex.SetPixel(i, j, this.white * 0f);
				}
			}
		}
	}

	public void EnableMist()
	{
		this.doMist = true;
		this.inited = false;
		this.bgCam.SetActive(true);
		base.gameObject.SetActive(true);
		this.ResetMist();
	}

	public void DisableMist()
	{
		this.doMist = false;
		this.inited = false;
		if (this.bgCam)
		{
			this.bgCam.SetActive(false);
		}
		if (base.gameObject)
		{
			base.gameObject.SetActive(false);
		}
	}

	public bool MaskBy(int sx, int sy, float vault = 0.5f)
	{
		int indexX = this.tex.width * sx / (int)this.cam.pixelWidth;
		int indexY = this.tex.height * sy / (int)this.cam.pixelHeight;
		float mistAmt = TexSight.GetMistAmt(indexX, indexY, this.sightTexs.ToArray());
		return mistAmt < vault;
	}

	private void InitTeam()
	{
		if (this.inited)
		{
			return;
		}
		this.uid = Singleton<PvpManager>.Instance.MyHeroUniqueId;
		Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
		if (!allMapUnits.ContainsKey(this.uid))
		{
			return;
		}
		this.initTeam = (TeamType)allMapUnits[this.uid].teamType;
		this.inited = true;
	}

	private void ReadUnits()
	{
		List<Units> pVPUnits = this.GetPVPUnits0(this.team);
		if (pVPUnits != null && pVPUnits.Count > 0)
		{
			List<UnitSight> list = new List<UnitSight>(pVPUnits.Count);
			foreach (Units current in pVPUnits)
			{
				UnitSight item = new UnitSight(current.mTransform.position, PostMist.sight, PostMist.fade);
				list.Add(item);
			}
			if (list.Count > 0)
			{
				this.Pass_MyUnitSight(list.ToArray());
			}
		}
		this.ReadUnits2();
		this.SetTexel();
		this.SetFootPrint();
	}

	private void ReadUnits2()
	{
		if (!this.inited)
		{
			return;
		}
		TeamType teamType;
		if (this.team == TeamType.LM)
		{
			teamType = TeamType.BL;
		}
		else
		{
			teamType = TeamType.LM;
		}
		List<Units> pVPUnits = this.GetPVPUnits2(teamType);
		if (pVPUnits != null && pVPUnits.Count > 0)
		{
			List<UnitSight> list = new List<UnitSight>(pVPUnits.Count);
			foreach (Units current in pVPUnits)
			{
				Vector3 vector = this.cam.WorldToScreenPoint(current.mTransform.position);
				if (!this.MaskBy((int)vector.x, (int)vector.y, 0.02f))
				{
					UnitSight item = new UnitSight(current.mTransform.position, PostMist.enemySight, PostMist.enemyFade);
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				this.Pass_EnemyUnitSight(list.ToArray());
			}
		}
	}

	private List<Units> GetPVPUnits0(TeamType team)
	{
		this.InitTeam();
		team = TeamType.LM;
		Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
		if (!allMapUnits.ContainsKey(this.uid))
		{
			if (!this.inited)
			{
				return null;
			}
			this.dead = true;
			team = this.initTeam;
		}
		else
		{
			this.dead = false;
			team = (TeamType)allMapUnits[this.uid].teamType;
		}
		List<Units> list = new List<Units>(12);
		IList<Units> mapUnits = MapManager.Instance.GetMapUnits(team, TargetTag.HeroAndMonster);
		IList<Units> mapUnits2 = MapManager.Instance.GetMapUnits(team, TargetTag.Building);
		if (mapUnits != null)
		{
			list.AddRange(mapUnits);
		}
		if (mapUnits2 != null)
		{
			list.AddRange(mapUnits2);
		}
		return list;
	}

	private List<Units> GetPVPUnits2(TeamType team)
	{
		List<Units> list = new List<Units>(12);
		IList<Units> mapUnits = MapManager.Instance.GetMapUnits(team, TargetTag.HeroAndMonster);
		IList<Units> mapUnits2 = MapManager.Instance.GetMapUnits(team, TargetTag.Building);
		if (mapUnits != null)
		{
			list.AddRange(mapUnits);
		}
		if (mapUnits2 != null)
		{
			list.AddRange(mapUnits2);
		}
		return list;
	}

	public void Pass_MyUnitSight(UnitSight[] sights)
	{
		if (sights == null || sights.Length < 1)
		{
			return;
		}
		this.sightTexs.Clear();
		for (int i = 0; i < sights.Length; i++)
		{
			UnitSight u = sights[i];
			TexSight item = TexSight.ConvertUnitSight(u, this.cam);
			this.sightTexs.Add(item);
		}
	}

	public void Pass_EnemyUnitSight(UnitSight[] sights)
	{
		if (sights == null || sights.Length < 1)
		{
			return;
		}
		for (int i = 0; i < sights.Length; i++)
		{
			UnitSight u = sights[i];
			TexSight item = TexSight.ConvertUnitSight(u, this.cam);
			this.sightTexs.Add(item);
		}
	}

	private void SetFootPrint()
	{
		int num = 0;
		int num2 = 0;
		float num3 = 0f;
		if (MapRuler.map != null)
		{
			MapRuler.map.GetCamFootXY(this.bgCam.transform.position, out num, out num2);
			num3 = MapRuler.map.screen2Map;
		}
		for (int i = 0; i < this.footMask.width; i++)
		{
			for (int j = 0; j < this.footMask.height; j++)
			{
				if (MapRuler.map != null)
				{
					int num4 = Mathf.Max(0, num + (int)(num3 * (float)(i - this.footMask.width / 2)));
					num4 = Mathf.Min(num4, MapRuler.map.width - 1);
					int num5 = Mathf.Max(0, num2 + (int)(num3 * (float)(j - this.footMask.height / 2)));
					num5 = Mathf.Min(num5, MapRuler.map.height - 1);
					float b = MapRuler.map.ReadMask(num4, num5);
					this.footMask.SetPixel(i, j, this.white * b);
				}
			}
		}
	}

	private void SetTexel()
	{
		for (int i = 0; i < this.tex.width; i++)
		{
			for (int j = 0; j < this.tex.height; j++)
			{
				float mistAmt = TexSight.GetMistAmt(i, j, this.sightTexs.ToArray());
				float a = (this.white * (1f - mistAmt)).a;
				this.tex.SetPixel(i, j, this.white * a);
			}
		}
	}

	public static void SetMistPerspect(Camera cam)
	{
		if (PostMist.Instance != null)
		{
			return;
		}
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
		gameObject.layer = Layer.UnitLayer;
		MeshCollider component = gameObject.GetComponent<MeshCollider>();
		UnityEngine.Object.Destroy(component);
		Shader shader = Shader.Find("MyShader/PostEffects/MistPanelBg");
		Renderer component2 = gameObject.GetComponent<MeshRenderer>();
		component2.material = new Material(shader);
		component2.material.hideFlags = HideFlags.DontSave;
		component2.castShadows = false;
		component2.receiveShadows = false;
		Transform transform = gameObject.transform;
		transform.parent = cam.transform;
		transform.localRotation = Quaternion.identity;
		transform.localPosition = new Vector3(0f, 0f, cam.nearClipPlane + 0.01f);
		float num = 0.0174532924f * cam.fieldOfView;
		float num2 = Mathf.Tan(num / 2f) * (cam.nearClipPlane + 0.01f) * 2f;
		transform.localScale = new Vector3(num2 * cam.aspect, num2, 1f);
		PostMist postMist = gameObject.AddComponent<PostMist>();
		postMist.mat = component2.sharedMaterial;
		postMist.bg = PostMist.CreateCompanyCam(cam, postMist);
		PostMist.Instance = postMist;
	}

	private static RenderTexture CreateCompanyCam(Camera camBat, PostMist p3)
	{
		GameObject gameObject = new GameObject("CompanyCamera");
		gameObject.transform.parent = camBat.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		Camera camera = gameObject.AddComponent<Camera>();
		camera.CopyFrom(camBat);
		RenderTexture renderTexture = new RenderTexture((int)camera.pixelWidth / 4, (int)camera.pixelHeight / 4, 16);
		camera.targetTexture = renderTexture;
		int mask = LayerMask.GetMask(new string[]
		{
			"Map",
			"Ground"
		});
		camera.cullingMask = mask;
		camera.nearClipPlane = camBat.nearClipPlane + 0.5f;
		camera.clearFlags = CameraClearFlags.Color;
		camera.depth = 12f;
		p3.bgCam = gameObject;
		p3.cam = camBat;
		return renderTexture;
	}
}
