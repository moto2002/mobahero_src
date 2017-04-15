using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class FOWSystem : MonoBehaviour
{
	public enum LOSChecks
	{
		None,
		Static,
		OnlyOnce,
		EveryUpdate
	}

	public class Revealer
	{
		public bool isActive;

		public FOWSystem.LOSChecks los;

		public Vector3 pos = Vector3.zero;

		public Vector3 prvpos = Vector3.zero;

		public float inner;

		public float outer;

		public bool[] cachedBuffer;

		public int cachedSize;

		public int cachedX;

		public int cachedY;

		public float time;

		public bool moved = true;

		public bool immidate;

		public float prevval = 1f;
	}

	public enum State
	{
		Blending,
		NeedUpdate,
		UpdateTexture1
	}

	public static FOWSystem instance;

	protected byte[] mHeights;

	protected Transform mTrans;

	protected Vector3 mOrigin = Vector3.zero;

	protected Vector3 mSize = Vector3.one;

	private static BetterList<FOWSystem.Revealer> mRevealers = new BetterList<FOWSystem.Revealer>();

	private static BetterList<FOWSystem.Revealer> staticRevealers = new BetterList<FOWSystem.Revealer>();

	private static BetterList<FOWSystem.Revealer> mAdded = new BetterList<FOWSystem.Revealer>();

	private static BetterList<FOWSystem.Revealer> mRemoved = new BetterList<FOWSystem.Revealer>();

	protected Color[] mBuffer0;

	protected Color[] mBuffer1;

	protected Color[] mBuffer2;

	protected Texture2D mTexture1;

	protected float mBlendFactor;

	protected float mNextUpdate;

	protected int mScreenHeight;

	protected FOWSystem.State mState;

	private Thread mThread;

	public int worldSize = 160;

	public int textureSize = 256;

	public float updateFrequency = 0.1f;

	public float textureBlendTime = 0.5f;

	public int blurIterations = 2;

	public Vector2 heightRange = new Vector2(0f, 255f);

	public LayerMask raycastMask = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("FOW") | 1 << LayerMask.NameToLayer("Grass");

	public float raycastRadius = 0.5f;

	public float margin = 1.5f;

	public FOWEffect m_FowEff;

	private float mElapsed;

	private Vector3[] debugworldpos;

	private float[] debugworldvisible;

	private Color _white;

	private string heightmapname = "map17";

	public static bool effectVisible = true;

	private bool enablefog = true;

	private bool inited;

	private bool updatating = true;

	private Vector3 basePos = new Vector3(-80f, 0f, -80f);

	public Texture2D texture1
	{
		get
		{
			return this.mTexture1;
		}
	}

	public float blendFactor
	{
		get
		{
			return this.mBlendFactor;
		}
	}

	public static FOWSystem Instance
	{
		get
		{
			if (FOWSystem.instance == null)
			{
				GameObject gameObject = GameObject.Find("FowSys");
				if (gameObject != null)
				{
					UnityEngine.Object.DestroyImmediate(gameObject);
				}
				GameObject gameObject2 = new GameObject("FowSys");
				FOWSystem fOWSystem = gameObject2.AddComponent<FOWSystem>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject2);
				GameObject gameObject3 = UnityEngine.Object.Instantiate(Resources.Load("meshDrawer")) as GameObject;
				gameObject3.transform.parent = gameObject2.transform;
				FOWSystem.instance = fOWSystem;
			}
			return FOWSystem.instance;
		}
	}

	public bool getenabelfog()
	{
		return this.enablefog;
	}

	public static FOWSystem.Revealer CreateRevealer(FOWSystem.LOSChecks ls = FOWSystem.LOSChecks.EveryUpdate)
	{
		FOWSystem.Revealer revealer = new FOWSystem.Revealer();
		revealer.isActive = false;
		revealer.los = ls;
		if (ls != FOWSystem.LOSChecks.Static)
		{
			BetterList<FOWSystem.Revealer> obj = FOWSystem.mAdded;
			lock (obj)
			{
				FOWSystem.mAdded.Add(revealer);
			}
		}
		else
		{
			FOWSystem.staticRevealers.Add(revealer);
		}
		return revealer;
	}

	public void RemoveRevealer(FOWSystem.Revealer rev)
	{
		BetterList<FOWSystem.Revealer> obj = FOWSystem.staticRevealers;
		lock (obj)
		{
			FOWSystem.staticRevealers.Remove(rev);
		}
		this.RefreshStaticRevealer();
	}

	public void RefreshStaticRevealer()
	{
		float num = 3.2f;
		Vector3 b = this.mOrigin;
		b.y = 0f;
		float num2 = (float)this.worldSize / (float)this.textureSize;
		float num3 = (float)this.textureSize / (float)this.worldSize;
		for (int i = 0; i < this.textureSize; i++)
		{
			b.z = this.mOrigin.z + (float)i * num2;
			for (int j = 0; j < this.textureSize; j++)
			{
				b.x = this.mOrigin.x + (float)j * num2;
				for (int k = 0; k < FOWSystem.staticRevealers.size; k++)
				{
					FOWSystem.Revealer revealer = FOWSystem.staticRevealers[k];
					Vector3 pos = revealer.pos;
					pos.y = 0f;
					Vector3 vector = revealer.pos - this.mOrigin;
					float num4 = Vector3.Distance(pos, b);
					if (num4 <= revealer.outer)
					{
						int num5 = Mathf.RoundToInt(vector.x * num3);
						int num6 = Mathf.RoundToInt(vector.z * num3);
						num5 = Mathf.Clamp(num5, 0, this.textureSize - 1);
						num6 = Mathf.Clamp(num6, 0, this.textureSize - 1);
						int num7 = j - num5;
						int num8 = i - num6;
						int num9 = num7 * num7 + num8 * num8;
						if ((float)num9 < num)
						{
							this.mBuffer1[j + i * this.textureSize].b = 1f;
							this.mBuffer1[j + i * this.textureSize].g = 0f;
							this.mBuffer2[j + i * this.textureSize].b = 1f;
							this.mBuffer2[j + i * this.textureSize].g = 0f;
						}
						else
						{
							Vector2 a = new Vector2((float)num7, (float)num8);
							a.Normalize();
							a *= num;
							int sx = num5 + Mathf.RoundToInt(a.x);
							int sy = num6 + Mathf.RoundToInt(a.y);
							int sightHeight = (int)this.WorldToGridHeight(vector.y);
							if (this.mBuffer1[j + i * this.textureSize].b == 1f)
							{
								this.mBuffer1[j + i * this.textureSize].g = 0f;
								this.mBuffer2[j + i * this.textureSize].g = 0f;
							}
							else if (this.IsVisible(sx, sy, j, i, sightHeight, 0, 0))
							{
								this.mBuffer1[j + i * this.textureSize].b = 1f;
								this.mBuffer1[j + i * this.textureSize].g = 0f;
								this.mBuffer2[j + i * this.textureSize].b = 1f;
								this.mBuffer2[j + i * this.textureSize].g = 0f;
							}
						}
					}
				}
			}
		}
	}

	public static void CreateStaticTimeRevealer(Vector3 pos, float rad, float tim)
	{
		FOWSystem.Revealer revealer = new FOWSystem.Revealer();
		revealer.los = FOWSystem.LOSChecks.OnlyOnce;
		revealer.isActive = true;
		FOWSystem.Revealer arg_1F_0 = revealer;
		revealer.outer = rad;
		arg_1F_0.inner = rad;
		revealer.time = tim;
		revealer.pos = pos;
		BetterList<FOWSystem.Revealer> obj = FOWSystem.mAdded;
		lock (obj)
		{
			FOWSystem.mAdded.Add(revealer);
		}
	}

	public static void DeleteRevealer(FOWSystem.Revealer rev)
	{
		BetterList<FOWSystem.Revealer> obj = FOWSystem.mRemoved;
		lock (obj)
		{
			FOWSystem.mRemoved.Add(rev);
		}
	}

	public void Awake()
	{
	}

	public void CreateInstance()
	{
		FOWSystem.instance = this;
	}

	public void enableFog(bool enable)
	{
		this.enablefog = enable;
		if (this.m_FowEff != null)
		{
			this.m_FowEff.enabelFog(this.enablefog);
		}
	}

	public void refreshBound()
	{
		this.mOrigin = this.mTrans.position;
		this.mOrigin.x = this.mOrigin.x - (float)this.worldSize * 0.5f;
		this.mOrigin.z = this.mOrigin.z - (float)this.worldSize * 0.5f;
		this.CreateGrid();
	}

	public FOWEffect BindCam(Camera cam)
	{
		return null;
	}

	public void DoStart(string name = "")
	{
		this.heightmapname = name;
		string path = "GraphCaches/" + this.heightmapname + "_h";
		TextAsset textAsset = Resources.Load(path, typeof(TextAsset)) as TextAsset;
		if (textAsset != null && textAsset.bytes != null)
		{
			this.mHeights = textAsset.bytes;
		}
		this.mNextUpdate = Time.time + this.updateFrequency;
	}

	public void Init()
	{
	}

	public void OnDestroy()
	{
	}

	public void DoUpdate()
	{
	}

	private void MainThreadUpdate()
	{
		if (this.mState == FOWSystem.State.NeedUpdate)
		{
			this.UpdateBuffer();
			this.mElapsed = Time.deltaTime;
			this.mState = FOWSystem.State.UpdateTexture1;
		}
	}

	private void ThreadUpdate()
	{
		Stopwatch stopwatch = new Stopwatch();
		while (true)
		{
			Thread.Sleep(1);
		}
	}

	private bool IsVisible(int sx, int sy, int fx, int fy, int sightHeight, int variance, int ingrass)
	{
		int num = Mathf.Abs(fx - sx);
		int num2 = Mathf.Abs(fy - sy);
		int num3 = (sx >= fx) ? -1 : 1;
		int num4 = (sy >= fy) ? -1 : 1;
		int num5 = num - num2;
		byte b = (byte)sightHeight;
		float num6 = (float)this.mHeights[fx + fy * this.textureSize];
		byte b2 = (byte)variance;
		while (sx != fx || sy != fy)
		{
			int num7 = fx - sx;
			int num8 = fy - sy;
			int num9 = (int)this.mHeights[sx + sy * this.textureSize];
			int num10 = (num9 < 200 || num9 >= 255) ? num9 : 0;
			num9 = ingrass * num10 + (1 - ingrass) * num9;
			if (num9 > (int)(b + b2))
			{
				return false;
			}
			int num11 = num5 << 1;
			if (num11 > -num2)
			{
				num5 -= num2;
				sx += num3;
			}
			if (num11 < num)
			{
				num5 += num;
				sy += num4;
			}
		}
		return true;
	}

	public byte WorldToGridHeight(float height)
	{
		int value = Mathf.RoundToInt(height / this.mSize.y * 255f);
		return (byte)Mathf.Clamp(value, 0, 255);
	}

	public static Color32[] CreateGridOffLine(Vector3 zeropos, int wsize, int txsize, float castrad, int laymask)
	{
		Vector3 vector = new Vector3((float)wsize, 255f, (float)wsize);
		zeropos.x -= (float)wsize * 0.5f;
		zeropos.z -= (float)wsize * 0.5f;
		Vector3 origin = zeropos;
		origin.y += vector.y;
		float num = (float)wsize / (float)txsize;
		bool flag = castrad > 0f;
		int num2 = 0;
		int num3 = txsize * txsize;
		Color32[] array = new Color32[num3];
		Dictionary<GameObject, int> dictionary = new Dictionary<GameObject, int>();
		for (int i = 0; i < txsize; i++)
		{
			origin.z = zeropos.z + (float)i * num;
			for (int j = 0; j < txsize; j++)
			{
				origin.x = zeropos.x + (float)j * num;
				array[j + i * txsize] = Color.black;
				if (flag)
				{
					RaycastHit raycastHit;
					if (Physics.SphereCast(new Ray(origin, Vector3.down), castrad, out raycastHit, vector.y, laymask))
					{
						float num4 = origin.y - raycastHit.distance - castrad;
						int value = Mathf.RoundToInt(num4 / vector.y * 255f);
						byte b = (byte)Mathf.Clamp(value, 0, 255);
						if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Grass"))
						{
							if (dictionary.ContainsKey(raycastHit.collider.gameObject))
							{
								int num5 = dictionary[raycastHit.collider.gameObject];
								byte b2 = (byte)(200 + num5);
								array[j + i * txsize] = new Color32(b2, b2, b2, b2);
							}
							else
							{
								dictionary.Add(raycastHit.collider.gameObject, num2);
								byte b3 = (byte)(200 + num2);
								array[j + i * txsize] = new Color32(b3, b3, b3, b3);
								num2++;
							}
						}
						else
						{
							array[j + i * txsize] = new Color32(b, b, b, b);
						}
					}
					else
					{
						array[j + i * txsize] = Color.white * 255f;
					}
				}
			}
		}
		dictionary.Clear();
		return array;
	}

	protected virtual void CreateGrid0()
	{
		this.mSize = new Vector3((float)this.worldSize, this.heightRange.y - this.heightRange.x, (float)this.worldSize);
		float num = (float)this.worldSize / (float)this.textureSize;
		for (int i = 0; i < this.textureSize; i++)
		{
			for (int j = 0; j < this.textureSize; j++)
			{
				this.mBuffer1[j + i * this.textureSize].b = 0f;
				this.mBuffer1[j + i * this.textureSize].g = 0f;
				this.mBuffer2[j + i * this.textureSize].b = 0f;
				this.mBuffer2[j + i * this.textureSize].g = 0f;
				if (this.mHeights[j + i * this.textureSize] < 255)
				{
					this.mBuffer1[j + i * this.textureSize].b = 0f;
					this.mBuffer2[j + i * this.textureSize].b = 0f;
				}
				else
				{
					this.mBuffer1[j + i * this.textureSize].b = 1f;
					this.mBuffer2[j + i * this.textureSize].b = 1f;
				}
			}
		}
		this.RefreshStaticRevealer();
	}

	protected virtual void CreateGrid()
	{
		this.mSize = new Vector3((float)this.worldSize, this.heightRange.y - this.heightRange.x, (float)this.worldSize);
		Vector3 origin = this.mOrigin;
		origin.y += this.mSize.y;
		float num = (float)this.worldSize / (float)this.textureSize;
		bool flag = this.raycastRadius > 0f;
		int num2 = 0;
		Dictionary<GameObject, int> dictionary = new Dictionary<GameObject, int>();
		for (int i = 0; i < this.textureSize; i++)
		{
			origin.z = this.mOrigin.z + (float)i * num;
			for (int j = 0; j < this.textureSize; j++)
			{
				origin.x = this.mOrigin.x + (float)j * num;
				this.mBuffer1[j + i * this.textureSize].b = 0f;
				this.mBuffer1[j + i * this.textureSize].g = 0f;
				this.mBuffer2[j + i * this.textureSize].b = 0f;
				this.mBuffer2[j + i * this.textureSize].g = 0f;
				RaycastHit raycastHit;
				if (flag)
				{
					if (Physics.SphereCast(new Ray(origin, Vector3.down), this.raycastRadius, out raycastHit, this.mSize.y, this.raycastMask))
					{
						float height = origin.y - raycastHit.distance - this.raycastRadius;
						if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Grass"))
						{
							if (dictionary.ContainsKey(raycastHit.collider.gameObject))
							{
								int num3 = dictionary[raycastHit.collider.gameObject];
								this.mHeights[j + i * this.textureSize] = (byte)(200 + num3);
							}
							else
							{
								dictionary.Add(raycastHit.collider.gameObject, num2);
								this.mHeights[j + i * this.textureSize] = (byte)(200 + num2);
								num2++;
							}
						}
						else
						{
							this.mHeights[j + i * this.textureSize] = this.WorldToGridHeight(height);
						}
						this.mBuffer1[j + i * this.textureSize].b = 0f;
						this.mBuffer2[j + i * this.textureSize].b = 0f;
					}
					else
					{
						this.mBuffer1[j + i * this.textureSize].b = 1f;
						this.mBuffer2[j + i * this.textureSize].b = 1f;
						this.mHeights[j + i * this.textureSize] = 255;
					}
				}
				else if (Physics.Raycast(new Ray(origin, Vector3.down), out raycastHit, this.mSize.y, this.raycastMask))
				{
					float height2 = origin.y - raycastHit.distance;
					this.mHeights[j + i * this.textureSize] = this.WorldToGridHeight(height2);
					this.mBuffer1[j + i * this.textureSize].b = 0f;
					this.mBuffer2[j + i * this.textureSize].b = 0f;
				}
			}
		}
		this.RefreshStaticRevealer();
		dictionary.Clear();
	}

	private void UpdateBuffer()
	{
		this.updatating = true;
		bool flag = false;
		if (FOWSystem.mAdded.size > 0)
		{
			BetterList<FOWSystem.Revealer> obj = FOWSystem.mAdded;
			lock (obj)
			{
				while (FOWSystem.mAdded.size > 0)
				{
					int num = FOWSystem.mAdded.size - 1;
					FOWSystem.mRevealers.Add(FOWSystem.mAdded.buffer[num]);
					FOWSystem.mAdded.RemoveAt(num);
				}
			}
		}
		Color[] array = this.mBuffer2;
		this.mBuffer2 = this.mBuffer1;
		this.mBuffer1 = array;
		if (FOWSystem.mRemoved.size > 0)
		{
			BetterList<FOWSystem.Revealer> obj2 = FOWSystem.mRemoved;
			lock (obj2)
			{
				while (FOWSystem.mRemoved.size > 0)
				{
					int num2 = FOWSystem.mRemoved.size - 1;
					FOWSystem.Revealer revealer = FOWSystem.mRemoved.buffer[num2];
					if (revealer.los != FOWSystem.LOSChecks.Static)
					{
						FOWSystem.mRevealers.Remove(revealer);
						FOWSystem.mRemoved.RemoveAt(num2);
					}
					else if (FOWSystem.staticRevealers.Contains(revealer))
					{
						flag = true;
						FOWSystem.staticRevealers.Remove(revealer);
						FOWSystem.mRemoved.RemoveAt(num2);
					}
				}
			}
			if (flag)
			{
				this.RefreshStaticRevealer();
			}
		}
		float num3 = this.mElapsed * 100f;
		if (FOWSystem.effectVisible)
		{
			int i = 0;
			int num4 = this.mBuffer1.Length;
			while (i < num4)
			{
				Color color = this.mBuffer0[i];
				Color color2 = this.mBuffer1[i];
				if (color.r < color2.r)
				{
					this.mBuffer0[i].r = Mathf.Lerp(this.mBuffer0[i].r, color2.r, num3 * 1f);
				}
				else
				{
					this.mBuffer0[i].r = Mathf.Lerp(this.mBuffer0[i].r, color2.r, num3 * (0.2f + color2.g));
				}
				color2.g = Mathf.Lerp(color2.g, 0f, num3);
				this.mBuffer1[i].r = color2.b;
				this.mBuffer1[i].g = color2.g;
				this.mBuffer2[i].g = color2.g;
				i++;
			}
		}
		else
		{
			int j = 0;
			int num5 = this.mBuffer1.Length;
			while (j < num5)
			{
				Color color3 = this.mBuffer0[j];
				Color color4 = this.mBuffer1[j];
				this.mBuffer0[j] = color4;
				color4.g = Mathf.Lerp(color4.g, 0f, num3);
				this.mBuffer1[j].r = color4.b;
				this.mBuffer1[j].g = color4.g;
				this.mBuffer2[j].g = color4.g;
				j++;
			}
		}
		float worldToTex = (float)this.textureSize / (float)this.worldSize;
		for (int k = 0; k < FOWSystem.mRevealers.size; k++)
		{
			FOWSystem.Revealer revealer2 = FOWSystem.mRevealers[k];
			if (revealer2.isActive)
			{
				if (revealer2.los == FOWSystem.LOSChecks.None || GlobalSettings.FogMode == 4)
				{
					this.RevealUsingRadius(revealer2, worldToTex, revealer2.immidate);
				}
				else if (revealer2.los == FOWSystem.LOSChecks.OnlyOnce)
				{
					revealer2.time -= this.mElapsed * 100f;
					if (revealer2.time <= 0f)
					{
						revealer2.time = 0f;
						BetterList<FOWSystem.Revealer> obj3 = FOWSystem.mRemoved;
						lock (obj3)
						{
							FOWSystem.mRemoved.Add(revealer2);
						}
						goto IL_49C;
					}
					this.RevealUsingCache(revealer2, worldToTex, revealer2.immidate);
				}
				else if (revealer2.moved)
				{
					this.RevealUsingLOS(revealer2, worldToTex, revealer2.immidate);
					revealer2.moved = false;
				}
				else
				{
					this.RevealUsingBCache(revealer2, worldToTex, revealer2.immidate);
				}
				revealer2.immidate = false;
			}
			IL_49C:;
		}
		if (FOWSystem.effectVisible)
		{
			this.BlurVisibility1();
		}
		this.updatating = false;
	}

	private void RevealUsingRadius(FOWSystem.Revealer r, float worldToTex, bool imm = false)
	{
		Vector3 vector = r.pos - this.mOrigin;
		int num = Mathf.RoundToInt((vector.x - r.outer) * worldToTex);
		int num2 = Mathf.RoundToInt((vector.z - r.outer) * worldToTex);
		int num3 = Mathf.RoundToInt((vector.x + r.outer) * worldToTex);
		int num4 = Mathf.RoundToInt((vector.z + r.outer) * worldToTex);
		int num5 = Mathf.RoundToInt(vector.x * worldToTex);
		int num6 = Mathf.RoundToInt(vector.z * worldToTex);
		num5 = Mathf.Clamp(num5, 0, this.textureSize - 1);
		num6 = Mathf.Clamp(num6, 0, this.textureSize - 1);
		int num7 = Mathf.RoundToInt(r.outer * r.outer * worldToTex * worldToTex);
		for (int i = num2; i < num4; i++)
		{
			if (i > -1 && i < this.textureSize)
			{
				int num8 = i * this.textureSize;
				for (int j = num; j < num3; j++)
				{
					if (j > -1 && j < this.textureSize)
					{
						int num9 = j - num5;
						int num10 = i - num6;
						int num11 = num9 * num9 + num10 * num10;
						if (num11 < num7)
						{
							this.mBuffer1[j + num8].r = 1f;
							if (imm)
							{
								this.mBuffer0[j + num8].r = 1f;
							}
						}
					}
				}
			}
		}
	}

	private void RevealUsingLOS(FOWSystem.Revealer r, float worldToTex, bool imm = false)
	{
		Vector3 vector = r.pos - this.mOrigin;
		int num = Mathf.RoundToInt((vector.x - r.outer) * worldToTex);
		int num2 = Mathf.RoundToInt((vector.z - r.outer) * worldToTex);
		int num3 = Mathf.RoundToInt((vector.x + r.outer) * worldToTex);
		int num4 = Mathf.RoundToInt((vector.z + r.outer) * worldToTex);
		int num5 = Mathf.RoundToInt(vector.x * worldToTex);
		int num6 = Mathf.RoundToInt(vector.z * worldToTex);
		num5 = Mathf.Clamp(num5, 0, this.textureSize - 1);
		num6 = Mathf.Clamp(num6, 0, this.textureSize - 1);
		int num7 = Mathf.RoundToInt(r.inner * r.inner * worldToTex * worldToTex);
		int num8 = Mathf.RoundToInt(r.outer * r.outer * worldToTex * worldToTex);
		int sightHeight = (int)this.WorldToGridHeight(r.pos.y);
		int ingrass = (this.mHeights[num5 + num6 * this.textureSize] < 200) ? 0 : 1;
		float num9 = (float)this.worldSize / (float)this.textureSize;
		for (int i = num2; i < num4; i++)
		{
			if (i > -1 && i < this.textureSize)
			{
				for (int j = num; j < num3; j++)
				{
					if (j > -1 && j < this.textureSize)
					{
						int num10 = j - num5;
						int num11 = i - num6;
						int num12 = num10 * num10 + num11 * num11;
						int num13 = j + i * this.textureSize;
						if (this.mBuffer1[num13].b == 1f)
						{
							this.mBuffer1[num13].r = 1f;
							this.mBuffer1[num13].g = 0f;
							this.mBuffer2[num13].r = 1f;
							this.mBuffer2[num13].g = 0f;
							if (imm)
							{
								this.mBuffer0[num13].r = 1f;
								this.mBuffer0[num13].g = 0f;
							}
						}
						else if (num12 < num7)
						{
							this.mBuffer1[num13].r = 1f;
							this.mBuffer1[num13].g = 0f;
							this.mBuffer2[num13].r = 1f;
							this.mBuffer2[num13].g = 0f;
							if (imm)
							{
								this.mBuffer0[num13].r = 1f;
								this.mBuffer0[num13].g = 0f;
							}
						}
						else if (num12 < num8)
						{
							Vector2 a = new Vector2((float)num10, (float)num11);
							a.Normalize();
							a *= r.inner;
							int num14 = num5 + Mathf.RoundToInt(a.x);
							int num15 = num6 + Mathf.RoundToInt(a.y);
							if (num14 > -1 && num14 < this.textureSize && num15 > -1 && num15 < this.textureSize && this.IsVisible(num14, num15, j, i, sightHeight, (int)this.margin, ingrass))
							{
								this.mBuffer1[num13].r = 1f;
								this.mBuffer1[num13].g = 0f;
								this.mBuffer2[num13].r = 1f;
								this.mBuffer2[num13].g = 0f;
								if (imm)
								{
									this.mBuffer0[num13].r = 1f;
									this.mBuffer0[num13].g = 0f;
								}
							}
							else
							{
								this.mBuffer1[num13].g = 1f;
								this.mBuffer2[num13].g = 1f;
							}
						}
					}
				}
			}
		}
	}

	private void RevealUsingCache(FOWSystem.Revealer r, float worldToTex, bool imm = false)
	{
		if (r.cachedBuffer == null)
		{
			this.RevealIntoCache(r, worldToTex);
		}
		int i = r.cachedY;
		int num = r.cachedY + r.cachedSize;
		while (i < num)
		{
			if (i > -1 && i < this.textureSize)
			{
				int num2 = i * this.textureSize;
				int num3 = (i - r.cachedY) * r.cachedSize;
				int j = r.cachedX;
				int num4 = r.cachedX + r.cachedSize;
				while (j < num4)
				{
					if (j > -1 && j < this.textureSize)
					{
						int num5 = j - r.cachedX + num3;
						if (r.cachedBuffer[num5])
						{
							this.mBuffer1[j + num2].r = 1f;
							this.mBuffer2[j + num2].r = 1f;
							if (imm)
							{
								this.mBuffer0[j + num2].r = 1f;
							}
						}
					}
					j++;
				}
			}
			i++;
		}
	}

	private void RevealUsingBCache(FOWSystem.Revealer r, float worldToTex, bool imm = false)
	{
		Vector3 vector = r.pos - this.mOrigin;
		int num = Mathf.RoundToInt((vector.x - r.outer) * worldToTex);
		int num2 = Mathf.RoundToInt((vector.z - r.outer) * worldToTex);
		int num3 = Mathf.RoundToInt((vector.x + r.outer) * worldToTex);
		int num4 = Mathf.RoundToInt((vector.z + r.outer) * worldToTex);
		int num5 = Mathf.RoundToInt(vector.x * worldToTex);
		int num6 = Mathf.RoundToInt(vector.z * worldToTex);
		num5 = Mathf.Clamp(num5, 0, this.textureSize - 1);
		num6 = Mathf.Clamp(num6, 0, this.textureSize - 1);
		int num7 = Mathf.RoundToInt(r.outer * r.outer * worldToTex * worldToTex);
		float num8 = (float)this.worldSize / (float)this.textureSize;
		for (int i = num2; i < num4; i++)
		{
			if (i > -1 && i < this.textureSize)
			{
				for (int j = num; j < num3; j++)
				{
					if (j > -1 && j < this.textureSize)
					{
						int num9 = j - num5;
						int num10 = i - num6;
						int num11 = num9 * num9 + num10 * num10;
						int num12 = j + i * this.textureSize;
						if (num11 < num7)
						{
							Color color = this.mBuffer2[num12];
							this.mBuffer1[num12] = color;
							if (imm)
							{
								this.mBuffer0[num12] = color;
							}
						}
					}
				}
			}
		}
	}

	private void RevealIntoCache(FOWSystem.Revealer r, float worldToTex)
	{
		Vector3 vector = r.pos - this.mOrigin;
		int num = Mathf.RoundToInt((vector.x - r.outer) * worldToTex);
		int num2 = Mathf.RoundToInt((vector.z - r.outer) * worldToTex);
		int num3 = Mathf.RoundToInt((vector.x + r.outer) * worldToTex);
		int num4 = Mathf.RoundToInt((vector.z + r.outer) * worldToTex);
		int num5 = Mathf.RoundToInt(vector.x * worldToTex);
		int num6 = Mathf.RoundToInt(vector.z * worldToTex);
		num5 = Mathf.Clamp(num5, 0, this.textureSize - 1);
		num6 = Mathf.Clamp(num6, 0, this.textureSize - 1);
		int num7 = Mathf.RoundToInt((float)(num3 - num));
		r.cachedBuffer = new bool[num7 * num7];
		r.cachedSize = num7;
		r.cachedX = num;
		r.cachedY = num2;
		int i = 0;
		int num8 = num7 * num7;
		while (i < num8)
		{
			r.cachedBuffer[i] = false;
			i++;
		}
		int num9 = Mathf.RoundToInt(r.inner * r.inner * worldToTex * worldToTex);
		int num10 = Mathf.RoundToInt(r.outer * r.outer * worldToTex * worldToTex);
		int sightHeight = (int)this.WorldToGridHeight(r.pos.y);
		int ingrass = (this.mHeights[num5 + num6 * this.textureSize] < 200) ? 0 : 1;
		for (int j = num2; j < num4; j++)
		{
			if (j > -1 && j < this.textureSize)
			{
				for (int k = num; k < num3; k++)
				{
					if (k > -1 && k < this.textureSize)
					{
						int num11 = k - num5;
						int num12 = j - num6;
						int num13 = num11 * num11 + num12 * num12;
						int num14 = k - num + (j - num2) * num7;
						int num15 = k + j * this.textureSize;
						if (this.mBuffer1[num15].b == 1f)
						{
							if (num14 < r.cachedBuffer.Length && num14 >= 0)
							{
								r.cachedBuffer[num14] = true;
							}
						}
						else if (num13 < num9)
						{
							if (num14 < r.cachedBuffer.Length && num14 >= 0)
							{
								r.cachedBuffer[num14] = true;
							}
						}
						else if (num13 < num10)
						{
							Vector2 a = new Vector2((float)num11, (float)num12);
							a.Normalize();
							a *= r.inner;
							int num16 = num5 + Mathf.RoundToInt(a.x);
							int num17 = num6 + Mathf.RoundToInt(a.y);
							if (num16 > -1 && num16 < this.textureSize && num17 > -1 && num17 < this.textureSize && this.IsVisible(num16, num17, k, j, sightHeight, (int)this.margin, ingrass) && num14 < r.cachedBuffer.Length && num14 >= 0)
							{
								r.cachedBuffer[num14] = true;
							}
						}
					}
				}
			}
		}
	}

	private void BlurVisibility()
	{
		for (int i = 0; i < this.textureSize; i++)
		{
			int num = i * this.textureSize;
			int num2 = i - 1;
			if (num2 < 0)
			{
				num2 = 0;
			}
			int num3 = i + 1;
			if (num3 >= this.textureSize)
			{
				num3 = i;
			}
			num2 *= this.textureSize;
			num3 *= this.textureSize;
			for (int j = 0; j < this.textureSize; j++)
			{
				int num4 = j - 1;
				if (num4 < 0)
				{
					num4 = 0;
				}
				int num5 = j + 1;
				if (num5 >= this.textureSize)
				{
					num5 = j;
				}
				int num6 = j + num;
				float num7 = this.mBuffer1[num6].r;
				num7 += this.mBuffer1[num4 + num].r;
				num7 += this.mBuffer1[num5 + num].r;
				num7 += this.mBuffer1[j + num2].r;
				num7 += this.mBuffer1[j + num3].r;
				num7 += this.mBuffer1[num4 + num2].r;
				num7 += this.mBuffer1[num5 + num2].r;
				num7 += this.mBuffer1[num4 + num3].r;
				num7 += this.mBuffer1[num5 + num3].r;
				Color color = this.mBuffer2[num6];
				color.r = (float)((byte)(num7 / 9f));
				color.r *= 2f;
				this.mBuffer2[num6] = color;
			}
		}
		Color[] array = this.mBuffer1;
		this.mBuffer1 = this.mBuffer2;
		this.mBuffer2 = array;
	}

	private void BlurVisibility1()
	{
		float num = (float)(this.textureSize * this.textureSize);
		int num2 = 0;
		while ((float)num2 < num)
		{
			int num3 = num2 / this.textureSize;
			int num4 = num2 % this.textureSize;
			int num5 = num3 * this.textureSize;
			int num6 = num3 - 1;
			if (num6 < 0)
			{
				num6 = 0;
			}
			int num7 = num3 + 1;
			if (num7 == this.textureSize)
			{
				num7 = num3;
			}
			num6 *= this.textureSize;
			num7 *= this.textureSize;
			int num8 = num4 - 1;
			if (num8 < 0)
			{
				num8 = 0;
			}
			int num9 = num4 + 1;
			if (num9 == this.textureSize)
			{
				num9 = num4;
			}
			float num10 = this.mBuffer1[num2].r;
			num10 += this.mBuffer1[num8 + num5].r;
			num10 += this.mBuffer1[num9 + num5].r;
			num10 += this.mBuffer1[num4 + num6].r;
			num10 += this.mBuffer1[num4 + num7].r;
			num10 += this.mBuffer1[num8 + num6].r;
			num10 += this.mBuffer1[num9 + num6].r;
			num10 += this.mBuffer1[num8 + num7].r;
			num10 += this.mBuffer1[num9 + num7].r;
			Color color = this.mBuffer2[num2];
			color.r = num10 / 9f;
			color.r *= 2f;
			color.g = this.mBuffer1[num2].g;
			color.b = this.mBuffer1[num2].b;
			this.mBuffer2[num2] = color;
			num2++;
		}
	}

	private void UpdateTexture()
	{
		if (!FOWSystem.effectVisible)
		{
			if (this.mState == FOWSystem.State.UpdateTexture1)
			{
				this.mState = FOWSystem.State.Blending;
			}
			return;
		}
	}

	public Color getBuff(Vector3 pos1)
	{
		pos1 -= this.mOrigin;
		float num = (float)this.textureSize / (float)this.worldSize;
		int num2 = Mathf.RoundToInt(pos1.x * num);
		int num3 = Mathf.RoundToInt(pos1.z * num);
		num2 = Mathf.Clamp(num2, 0, this.textureSize - 1);
		num3 = Mathf.Clamp(num3, 0, this.textureSize - 1);
		int num4 = num2 + num3 * this.textureSize;
		return this.mBuffer1[num4];
	}

	public bool IsVisibleReal(Vector3 pos)
	{
		return true;
	}

	public bool IsVisible_Circle(Vector3 pos, Vector3 forward, Vector3 side, float rad, float offset = 0f)
	{
		Vector3 pos2 = pos + forward * (rad + offset) + side * rad;
		Vector3 pos3 = pos + forward * (rad + offset) + side * -rad;
		Vector3 pos4 = pos + forward * (-rad + offset) + side * rad;
		Vector3 pos5 = pos + forward * (-rad + offset) + side * -rad;
		return this.IsVisible(pos2) || this.IsVisible(pos3) || this.IsVisible(pos4) || this.IsVisible(pos5);
	}

	public bool IsVisible_Rect(Vector3 pos, Vector3 forward, Vector3 side, float len, float width, bool longline = false)
	{
		Vector3 pos2 = pos + forward * len + side * width * 0.5f;
		Vector3 pos3 = pos + forward * len + side * -width * 0.5f;
		Vector3 pos4 = pos + side * width * 0.5f;
		Vector3 pos5 = pos + side * -width * 0.5f;
		if (width == 0f)
		{
			if (this.IsVisible(pos2))
			{
				return true;
			}
			if (this.IsVisible(pos4))
			{
				return true;
			}
			for (float num = 1f; num < len; num += 1f)
			{
				pos2 = pos + forward * num + side * width * 0.5f;
				pos3 = pos + forward * num + side * -width * 0.5f;
				if (this.IsVisible(pos2))
				{
					return true;
				}
				if (this.IsVisible(pos3))
				{
					return true;
				}
			}
			return false;
		}
		else
		{
			if (this.IsVisible(pos2))
			{
				return true;
			}
			if (this.IsVisible(pos3))
			{
				return true;
			}
			if (this.IsVisible(pos4))
			{
				return true;
			}
			if (this.IsVisible(pos5))
			{
				return true;
			}
			for (float num2 = 1f; num2 < len; num2 += 1f)
			{
				pos2 = pos + forward * num2 + side * width * 0.5f;
				pos3 = pos + forward * num2 + side * -width * 0.5f;
				if (this.IsVisible(pos2))
				{
					return true;
				}
				if (this.IsVisible(pos3))
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsVisible_Fan(Vector3 pos, Vector3 forward, Vector3 side, float rad, float deg)
	{
		float num = rad * Mathf.Tan(deg * 0.5f);
		Vector3 pos2 = pos + forward * rad + side * num;
		Vector3 pos3 = pos + forward * rad + side * -num;
		return this.IsVisible(pos2) || this.IsVisible(pos3) || this.IsVisible(pos);
	}

	public int GetHeight(Vector3 pos)
	{
		return -1;
	}

	public int IsInGrass(Vector3 pos)
	{
		if (Singleton<PvpManager>.Instance.IsGlobalObserver)
		{
			pos -= this.basePos;
			float num = (float)this.textureSize / (float)this.worldSize;
			int num2 = Mathf.RoundToInt(pos.x * num);
			int num3 = Mathf.RoundToInt(pos.z * num);
			num2 = Mathf.Clamp(num2, 0, this.textureSize - 1);
			num3 = Mathf.Clamp(num3, 0, this.textureSize - 1);
			int num4 = (int)this.mHeights[num2 + num3 * this.textureSize];
			return (num4 < 200 || num4 >= 255) ? -1 : num4;
		}
		return -1;
	}

	public float getVisible(Vector3 pos)
	{
		return 0f;
	}

	public bool IsVisible(Vector3 pos)
	{
		return true;
	}

	public Color GetVisibleC0(Vector3 pos)
	{
		pos -= this.mOrigin;
		float num = (float)this.textureSize / (float)this.worldSize;
		int num2 = Mathf.RoundToInt(pos.x * num);
		int num3 = Mathf.RoundToInt(pos.z * num);
		num2 = Mathf.Clamp(num2, 0, this.textureSize - 1);
		num3 = Mathf.Clamp(num3, 0, this.textureSize - 1);
		int num4 = num2 + num3 * this.textureSize;
		return this.mBuffer0[num4];
	}

	public Color GetVisibleC1(Vector3 pos)
	{
		pos -= this.mOrigin;
		float num = (float)this.textureSize / (float)this.worldSize;
		int num2 = Mathf.RoundToInt(pos.x * num);
		int num3 = Mathf.RoundToInt(pos.z * num);
		num2 = Mathf.Clamp(num2, 0, this.textureSize - 1);
		num3 = Mathf.Clamp(num3, 0, this.textureSize - 1);
		int num4 = num2 + num3 * this.textureSize;
		return this.mBuffer1[num4];
	}

	public bool IsVisible(FOWSystem.Revealer rev)
	{
		return true;
	}

	public bool IsExplored(Vector3 pos)
	{
		pos -= this.mOrigin;
		float num = (float)this.textureSize / (float)this.worldSize;
		int num2 = Mathf.RoundToInt(pos.x * num);
		int num3 = Mathf.RoundToInt(pos.z * num);
		num2 = Mathf.Clamp(num2, 0, this.textureSize - 1);
		num3 = Mathf.Clamp(num3, 0, this.textureSize - 1);
		return this.mBuffer1[num2 + num3 * this.textureSize].g > 0f;
	}
}
