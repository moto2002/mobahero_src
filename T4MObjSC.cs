using System;
using UnityEngine;

[ExecuteInEditMode]
public class T4MObjSC : MonoBehaviour
{
	[HideInInspector]
	public string ConvertType = string.Empty;

	[HideInInspector]
	public bool EnabledLODSystem = true;

	[HideInInspector]
	public Vector3[] ObjPosition;

	[HideInInspector]
	public T4MLodObjSC[] ObjLodScript;

	[HideInInspector]
	public int[] ObjLodStatus;

	[HideInInspector]
	public float MaxViewDistance = 60f;

	[HideInInspector]
	public float LOD2Start = 20f;

	[HideInInspector]
	public float LOD3Start = 40f;

	[HideInInspector]
	public float Interval = 0.5f;

	[HideInInspector]
	public Transform PlayerCamera;

	private Vector3 OldPlayerPos;

	[HideInInspector]
	public int Mode = 1;

	[HideInInspector]
	public int Master;

	[HideInInspector]
	public bool enabledBillboard = true;

	[HideInInspector]
	public Vector3[] BillboardPosition;

	[HideInInspector]
	public float BillInterval = 0.05f;

	[HideInInspector]
	public int[] BillStatus;

	[HideInInspector]
	public float BillMaxViewDistance = 30f;

	[HideInInspector]
	public T4MBillBObjSC[] BillScript;

	[HideInInspector]
	public bool enabledLayerCul = true;

	[HideInInspector]
	public float BackGroundView = 1000f;

	[HideInInspector]
	public float FarView = 200f;

	[HideInInspector]
	public float NormalView = 60f;

	[HideInInspector]
	public float CloseView = 30f;

	private float[] distances = new float[32];

	[HideInInspector]
	public int Axis;

	[HideInInspector]
	public bool LODbasedOnScript = true;

	[HideInInspector]
	public bool BilBbasedOnScript = true;

	public Material T4MMaterial;

	public MeshFilter T4MMesh;

	[HideInInspector]
	public Color TranslucencyColor = new Color(0.73f, 0.85f, 0.4f, 1f);

	[HideInInspector]
	public Vector4 Wind = new Vector4(0.85f, 0.075f, 0.4f, 0.5f);

	[HideInInspector]
	public float WindFrequency = 0.75f;

	[HideInInspector]
	public float GrassWindFrequency = 1.5f;

	[HideInInspector]
	public bool ActiveWind;

	public bool LayerCullPreview;

	public bool LODPreview;

	public bool BillboardPreview;

	private bool isActive;

	public Texture2D T4MMaskTex2d;

	public Texture2D T4MMaskTexd;

	private void OnEnable()
	{
		this.isActive = true;
	}

	private void OnDisable()
	{
		this.isActive = false;
	}

	public void Awake()
	{
		if (this.Master == 1)
		{
			if (this.PlayerCamera == null && Camera.main)
			{
				this.PlayerCamera = Camera.main.transform;
			}
			else if (this.PlayerCamera == null && !Camera.main)
			{
				Camera[] array = UnityEngine.Object.FindObjectsOfType(typeof(Camera)) as Camera[];
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].GetComponent<AudioListener>())
					{
						this.PlayerCamera = array[i].transform;
					}
				}
			}
			if (this.enabledLayerCul)
			{
				this.distances[26] = this.CloseView;
				this.distances[27] = this.NormalView;
				this.distances[28] = this.FarView;
				this.distances[29] = this.BackGroundView;
				if (this.PlayerCamera && this.PlayerCamera.camera)
				{
					this.PlayerCamera.camera.layerCullDistances = this.distances;
				}
			}
			if (this.EnabledLODSystem && this.ObjPosition.Length > 0 && this.Mode == 1)
			{
				if (this.ObjLodScript[0].gameObject != null)
				{
					if (this.LODbasedOnScript)
					{
						base.InvokeRepeating("LODScript", UnityEngine.Random.Range(0f, this.Interval), this.Interval);
					}
					else
					{
						base.InvokeRepeating("LODLay", UnityEngine.Random.Range(0f, this.Interval), this.Interval);
					}
				}
			}
			else if (this.EnabledLODSystem && this.ObjPosition.Length > 0 && this.Mode == 2 && this.ObjLodScript[0] != null)
			{
				for (int j = 0; j < this.ObjPosition.Length; j++)
				{
					if (this.ObjLodScript[j] != null)
					{
						if (this.LODbasedOnScript)
						{
							this.ObjLodScript[j].ActivateLODScrpt();
						}
						else
						{
							this.ObjLodScript[j].ActivateLODLay();
						}
					}
				}
			}
			if (this.enabledBillboard && this.BillboardPosition.Length > 0 && this.BillScript[0] != null)
			{
				if (this.BilBbasedOnScript)
				{
					base.InvokeRepeating("BillScrpt", UnityEngine.Random.Range(0f, this.BillInterval), this.BillInterval);
				}
				else
				{
					base.InvokeRepeating("BillLay", UnityEngine.Random.Range(0f, this.BillInterval), this.BillInterval);
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (this.isActive)
		{
			if (this.ActiveWind)
			{
				Color color = this.Wind * Mathf.Sin(Time.realtimeSinceStartup * this.WindFrequency);
				color.a = this.Wind.w;
				Color color2 = this.Wind * Mathf.Sin(Time.realtimeSinceStartup * this.GrassWindFrequency);
				color2.a = this.Wind.w;
				Shader.SetGlobalColor("_Wind", color);
				Shader.SetGlobalColor("_GrassWind", color2);
				Shader.SetGlobalColor("_TranslucencyColor", this.TranslucencyColor);
				Shader.SetGlobalFloat("_TranslucencyViewDependency;", 0.65f);
			}
			if (this.PlayerCamera && !Application.isPlaying && this.Master == 1)
			{
				if (this.LayerCullPreview && this.enabledLayerCul)
				{
					this.distances[26] = this.CloseView;
					this.distances[27] = this.NormalView;
					this.distances[28] = this.FarView;
					this.distances[29] = this.BackGroundView;
					this.PlayerCamera.camera.layerCullDistances = this.distances;
				}
				else
				{
					this.distances[26] = this.PlayerCamera.camera.farClipPlane;
					this.distances[27] = this.PlayerCamera.camera.farClipPlane;
					this.distances[28] = this.PlayerCamera.camera.farClipPlane;
					this.distances[29] = this.PlayerCamera.camera.farClipPlane;
					this.PlayerCamera.camera.layerCullDistances = this.distances;
				}
				if (this.LODPreview)
				{
					if (this.EnabledLODSystem && this.ObjPosition.Length > 0 && this.Mode == 1)
					{
						if (this.ObjLodScript[0].gameObject != null)
						{
							if (this.LODbasedOnScript)
							{
								this.LODScript();
							}
							else
							{
								this.LODLay();
							}
						}
					}
					else if (this.EnabledLODSystem && this.ObjPosition.Length > 0 && this.Mode == 2 && this.ObjLodScript[0] != null)
					{
						for (int i = 0; i < this.ObjPosition.Length; i++)
						{
							if (this.ObjLodScript[i] != null)
							{
								if (this.LODbasedOnScript)
								{
									this.ObjLodScript[i].AFLODScrpt();
								}
								else
								{
									this.ObjLodScript[i].AFLODLay();
								}
							}
						}
					}
				}
				if (this.BillboardPreview && this.enabledBillboard && this.BillboardPosition.Length > 0 && this.BillScript[0] != null)
				{
					if (this.BilBbasedOnScript)
					{
						this.BillScrpt();
					}
					else
					{
						this.BillLay();
					}
				}
			}
		}
	}

	private void BillScrpt()
	{
		for (int i = 0; i < this.BillboardPosition.Length; i++)
		{
			if (Vector3.Distance(this.BillboardPosition[i], this.PlayerCamera.position) <= this.BillMaxViewDistance)
			{
				if (this.BillStatus[i] != 1)
				{
					this.BillScript[i].Render.enabled = true;
					this.BillStatus[i] = 1;
				}
				if (this.Axis == 0)
				{
					this.BillScript[i].Transf.LookAt(new Vector3(this.PlayerCamera.position.x, this.BillScript[i].Transf.position.y, this.PlayerCamera.position.z), Vector3.up);
				}
				else
				{
					this.BillScript[i].Transf.LookAt(this.PlayerCamera.position, Vector3.up);
				}
			}
			else if (this.BillStatus[i] != 0 && !this.BillScript[i].Render.enabled)
			{
				this.BillScript[i].Render.enabled = false;
				this.BillStatus[i] = 0;
			}
		}
	}

	private void BillLay()
	{
		for (int i = 0; i < this.BillboardPosition.Length; i++)
		{
			int layer = this.BillScript[i].gameObject.layer;
			if (Vector3.Distance(this.BillboardPosition[i], this.PlayerCamera.position) <= this.distances[layer])
			{
				if (this.Axis == 0)
				{
					this.BillScript[i].Transf.LookAt(new Vector3(this.PlayerCamera.position.x, this.BillScript[i].Transf.position.y, this.PlayerCamera.position.z), Vector3.up);
				}
				else
				{
					this.BillScript[i].Transf.LookAt(this.PlayerCamera.position, Vector3.up);
				}
			}
		}
	}

	private void LODScript()
	{
		if (this.OldPlayerPos == this.PlayerCamera.position)
		{
			return;
		}
		this.OldPlayerPos = this.PlayerCamera.position;
		for (int i = 0; i < this.ObjPosition.Length; i++)
		{
			float num = Vector3.Distance(new Vector3(this.ObjPosition[i].x, this.PlayerCamera.position.y, this.ObjPosition[i].z), this.PlayerCamera.position);
			if (num <= this.MaxViewDistance)
			{
				if (num < this.LOD2Start && this.ObjLodStatus[i] != 1)
				{
					Renderer arg_C8_0 = this.ObjLodScript[i].LOD2;
					bool flag = false;
					this.ObjLodScript[i].LOD3.enabled = flag;
					arg_C8_0.enabled = flag;
					this.ObjLodScript[i].LOD1.enabled = true;
					this.ObjLodStatus[i] = 1;
				}
				else if (num >= this.LOD2Start && num < this.LOD3Start && this.ObjLodStatus[i] != 2)
				{
					Renderer arg_137_0 = this.ObjLodScript[i].LOD1;
					bool flag = false;
					this.ObjLodScript[i].LOD3.enabled = flag;
					arg_137_0.enabled = flag;
					this.ObjLodScript[i].LOD2.enabled = true;
					this.ObjLodStatus[i] = 2;
				}
				else if (num >= this.LOD3Start && this.ObjLodStatus[i] != 3)
				{
					Renderer arg_19A_0 = this.ObjLodScript[i].LOD2;
					bool flag = false;
					this.ObjLodScript[i].LOD1.enabled = flag;
					arg_19A_0.enabled = flag;
					this.ObjLodScript[i].LOD3.enabled = true;
					this.ObjLodStatus[i] = 3;
				}
			}
			else if (this.ObjLodStatus[i] != 0)
			{
				Renderer arg_205_0 = this.ObjLodScript[i].LOD1;
				bool flag = false;
				this.ObjLodScript[i].LOD3.enabled = flag;
				flag = flag;
				this.ObjLodScript[i].LOD2.enabled = flag;
				arg_205_0.enabled = flag;
				this.ObjLodStatus[i] = 0;
			}
		}
	}

	private void LODLay()
	{
		if (this.OldPlayerPos == this.PlayerCamera.position)
		{
			return;
		}
		this.OldPlayerPos = this.PlayerCamera.position;
		for (int i = 0; i < this.ObjPosition.Length; i++)
		{
			float num = Vector3.Distance(new Vector3(this.ObjPosition[i].x, this.PlayerCamera.position.y, this.ObjPosition[i].z), this.PlayerCamera.position);
			int layer = this.ObjLodScript[i].gameObject.layer;
			if (num <= this.distances[layer] + 5f)
			{
				if (num < this.LOD2Start && this.ObjLodStatus[i] != 1)
				{
					Renderer arg_E6_0 = this.ObjLodScript[i].LOD2;
					bool enabled = false;
					this.ObjLodScript[i].LOD3.enabled = enabled;
					arg_E6_0.enabled = enabled;
					this.ObjLodScript[i].LOD1.enabled = true;
					this.ObjLodStatus[i] = 1;
				}
				else if (num >= this.LOD2Start && num < this.LOD3Start && this.ObjLodStatus[i] != 2)
				{
					Renderer arg_158_0 = this.ObjLodScript[i].LOD1;
					bool enabled = false;
					this.ObjLodScript[i].LOD3.enabled = enabled;
					arg_158_0.enabled = enabled;
					this.ObjLodScript[i].LOD2.enabled = true;
					this.ObjLodStatus[i] = 2;
				}
				else if (num >= this.LOD3Start && this.ObjLodStatus[i] != 3)
				{
					Renderer arg_1BE_0 = this.ObjLodScript[i].LOD2;
					bool enabled = false;
					this.ObjLodScript[i].LOD1.enabled = enabled;
					arg_1BE_0.enabled = enabled;
					this.ObjLodScript[i].LOD3.enabled = true;
					this.ObjLodStatus[i] = 3;
				}
			}
		}
	}
}
