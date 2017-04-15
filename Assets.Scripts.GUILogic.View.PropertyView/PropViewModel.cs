using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewModel : MonoBehaviour
	{
		private ModelRotate modelRotate;

		[SerializeField]
		private Transform rotary;

		private object[] mgs;

		private Vector3 m_curFramePoint = Vector3.zero;

		private Vector3 m_priorFramePoint = Vector3.zero;

		private float m_fDizzinessValue;

		private bool m_bCanRotate;

		private Ray m_ray;

		private RaycastHit[] m_hits;

		private UITweener m_uiTweener;

		[SerializeField]
		private Camera m_uiCamera;

		[SerializeField]
		private Camera m_modelCamera;

		[SerializeField]
		private Transform m_modelRoot;

		[SerializeField]
		private float m_fRotateSpeed;

		private GameObject smodel;

		private string heroNPC = string.Empty;

		private string prevhero = string.Empty;

		private PropertyType tempType = PropertyType.Other;

		private int skinID;

		private Task task_loadModel;

		private Task task_generateModel;

		private Task task_showModel;

		private CoroutineManager coroutine;

		private int ID_HitStun;

		private Animator animator;

		private AnimatorOverrideController ac;

		private ResourceRequest rr;

		public string HeroNPC
		{
			get
			{
				return this.heroNPC;
			}
		}

		public int SkinID
		{
			get
			{
				return this.skinID;
			}
		}

		public GameObject Smodel
		{
			get
			{
				return this.smodel;
			}
		}

		private void Awake()
		{
			CamRatio.SetupCamera(this.m_modelCamera, 0f);
			this.modelRotate = base.GetComponent<ModelRotate>();
			this.mgs = new object[]
			{
				ClientV2C.sacriviewChangeHero,
				ClientV2C.propviewChangeSkin,
				ClientV2C.propviewChangeToggle
			};
			this.Initialize();
			this.coroutine = new CoroutineManager();
		}

		private void OnEnable()
		{
			this.m_uiCamera.gameObject.SetActive(false);
			this.m_uiCamera.gameObject.SetActive(true);
			this.Register();
			if (Singleton<MenuTopBarView>.Instance != null)
			{
				Singleton<MenuTopBarView>.Instance.SetActiveOrNot(false);
			}
			if (null != this.smodel && null != this.animator && this.animator.GetCurrentAnimatorStateInfo(0).length == 0f)
			{
				this.animator.Play("HeroBreath1");
			}
		}

		private void OnDisable()
		{
			this.Unregister();
			if (Singleton<MenuTopBarView>.Instance != null)
			{
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			this.CheckTouch();
			if (this.m_bCanRotate)
			{
				if (this.m_uiTweener != null && this.m_uiTweener.enabled)
				{
					this.m_uiTweener.enabled = false;
				}
				this.ProcessTouch();
				this.Rotate();
			}
			if (this.smodel != null)
			{
				this.rotary.localEulerAngles = new Vector3(-90f, this.smodel.transform.localEulerAngles.y, 0f);
			}
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Initialize()
		{
			if (null == this.rotary)
			{
				this.rotary = this.modelRotate.transform.Find("WJ_Zhuantai1");
			}
		}

		private void OnMsg_propviewLoadModel(MobaMessage msg)
		{
			this.coroutine.StopAllCoroutine();
			if (msg.Param != null)
			{
				GameObject gameObject = (GameObject)msg.Param;
			}
		}

		private void OnMsg_propviewChangeSkin(MobaMessage msg)
		{
			this.coroutine.StopAllCoroutine();
			if (msg.Param != null)
			{
				this.skinID = (int)msg.Param;
			}
			if (null != this.smodel)
			{
				UnityEngine.Object.Destroy(this.smodel);
			}
			if (null != this.animator)
			{
				UnityEngine.Object.Destroy(this.animator);
			}
			if (this.skinID == 0)
			{
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.heroNPC);
				if (heroMainData != null)
				{
					this.task_loadModel = this.coroutine.StartCoroutine(this.LoadModel(heroMainData.smodel_id), true);
				}
			}
			else
			{
				SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.skinID.ToString());
				if (dataById != null)
				{
					this.task_loadModel = this.coroutine.StartCoroutine(this.LoadModel(dataById.smodel_id), true);
				}
			}
		}

		private void OnMsg_sacriviewChangeHero(MobaMessage msg)
		{
			this.coroutine.StopAllCoroutine();
			if (msg.Param != null)
			{
				string modelID = string.Empty;
				modelID = (string)msg.Param;
				this.heroNPC = modelID;
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(modelID);
				this.skinID = ((heroInfoData != null) ? heroInfoData.CurrSkin : 0);
				if (null != this.smodel)
				{
					UnityEngine.Object.Destroy(this.smodel);
				}
				if (null != this.animator)
				{
					UnityEngine.Object.Destroy(this.animator);
				}
				if (this.skinID == 0)
				{
					SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.heroNPC);
					if (heroMainData != null)
					{
						this.task_loadModel = this.coroutine.StartCoroutine(this.LoadModel(heroMainData.smodel_id), true);
					}
				}
				else
				{
					SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(this.skinID.ToString());
					if (dataById != null)
					{
						this.task_loadModel = this.coroutine.StartCoroutine(this.LoadModel(dataById.smodel_id), true);
					}
				}
			}
		}

		private void OnMsg_propviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				PropertyType propertyType = (PropertyType)((int)msg.Param);
				this.tempType = propertyType;
			}
		}

		private string path(string name)
		{
			int length = name.IndexOf("_");
			return name.Substring(0, length);
		}

		private void HitError(GameObject modelPrefab)
		{
			string text = this.path(modelPrefab.name);
			Transform transform = modelPrefab.transform.FindChild("Model");
			Transform transform2 = transform.transform.FindChild(text);
			SkinnedMeshRenderer component = transform2.GetComponent<SkinnedMeshRenderer>();
			for (int i = 0; i < component.materials.Length; i++)
			{
				if (null == component.materials[i] || !component.materials[i].name.Contains(text))
				{
					UnityEngine.Debug.LogError(text + "Still missing materials(perfab),locate at (from 0)：" + i);
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator GenerateModel(GameObject prefab)
		{
			PropViewModel.<GenerateModel>c__Iterator181 <GenerateModel>c__Iterator = new PropViewModel.<GenerateModel>c__Iterator181();
			<GenerateModel>c__Iterator.prefab = prefab;
			<GenerateModel>c__Iterator.<$>prefab = prefab;
			<GenerateModel>c__Iterator.<>f__this = this;
			return <GenerateModel>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator ShowSmodel()
		{
			PropViewModel.<ShowSmodel>c__Iterator182 <ShowSmodel>c__Iterator = new PropViewModel.<ShowSmodel>c__Iterator182();
			<ShowSmodel>c__Iterator.<>f__this = this;
			return <ShowSmodel>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator LoadModel(string smodelname)
		{
			PropViewModel.<LoadModel>c__Iterator183 <LoadModel>c__Iterator = new PropViewModel.<LoadModel>c__Iterator183();
			<LoadModel>c__Iterator.smodelname = smodelname;
			<LoadModel>c__Iterator.<$>smodelname = smodelname;
			<LoadModel>c__Iterator.<>f__this = this;
			return <LoadModel>c__Iterator;
		}

		private void MoveModelPosition(PropertyType type, bool isActive = true)
		{
			if (null != this.rotary && null != this.smodel)
			{
				this.rotary.gameObject.SetActive(true);
				this.smodel.gameObject.SetActive(true);
				if (type == PropertyType.Rune)
				{
					this.rotary.localPosition = new Vector3(565f, this.rotary.localPosition.y, this.rotary.localPosition.z);
					this.smodel.transform.localPosition = new Vector3(565f, this.smodel.transform.localPosition.y, this.smodel.transform.localPosition.z);
					this.modelRotate.GetComponent<BoxCollider>().center = new Vector3(565f, this.modelRotate.GetComponent<BoxCollider>().center.y, this.modelRotate.GetComponent<BoxCollider>().center.z);
				}
				else
				{
					this.rotary.localPosition = new Vector3(200f, this.rotary.localPosition.y, this.rotary.localPosition.z);
					this.smodel.transform.localPosition = new Vector3(200f, this.smodel.transform.localPosition.y, this.smodel.transform.localPosition.z);
					this.modelRotate.GetComponent<BoxCollider>().center = new Vector3(200f, this.modelRotate.GetComponent<BoxCollider>().center.y, this.modelRotate.GetComponent<BoxCollider>().center.z);
				}
			}
		}

		private void SetModelActiveState(bool isActive)
		{
			this.smodel.SetActive(isActive);
		}

		public void CheckMaterial()
		{
			List<string> list = new List<string>();
			string text = this.path(this.smodel.name);
			Transform transform = this.smodel.transform.FindChild("Model");
			Transform transform2 = transform.transform.FindChild(text);
			if (null == transform2)
			{
				CtrlManager.ShowMsgBox("名称不匹配!!!", "Skinnedmeshrender组件的名字(我们不知道是什么) 不匹配Smodel的名字: \"" + text + "\" 也许有人可以修复一下", delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			SkinnedMeshRenderer component = transform2.GetComponent<SkinnedMeshRenderer>();
			Material[] materials = component.materials;
			if (materials.Length != 1)
			{
				for (int i = 0; i < materials.Length; i++)
				{
					if (null == materials[i] || !materials[i].name.Contains(text))
					{
						break;
					}
					if (i == materials.Length - 1)
					{
						break;
					}
				}
				for (int j = 0; j < materials.Length; j++)
				{
					list.Add((j != 0) ? (text + "_ST" + j) : text);
				}
				for (int k = 0; k < materials.Length; k++)
				{
					if (list.Contains(materials[k].name))
					{
						list.Remove(materials[k].name);
					}
				}
				for (int l = 0; l < materials.Length; l++)
				{
					if (null == materials[l] || !materials[l].name.Contains(text))
					{
						materials[l] = (Resources.Load("Material/showMat/Materials/" + list[0]) as Material);
						list.RemoveAt(0);
						if (list.Count == 0)
						{
							break;
						}
					}
				}
				component.materials = materials;
				for (int m = 0; m < component.materials.Length; m++)
				{
					if (null == component.materials[m] || !component.materials[m].name.Contains(text))
					{
						UnityEngine.Debug.LogError(text + "Still missing materials,locate at (from 0)：" + m);
					}
				}
				return;
			}
			if (null == materials[0] || !materials[0].name.Contains(text))
			{
				Material material = Resources.Load("Material/showMat/Materials/" + text) as Material;
				materials[0] = material;
				component.materials = materials;
				for (int n = 0; n < component.materials.Length; n++)
				{
					if (null == component.materials[n] || !component.materials[n].name.Contains(text))
					{
						UnityEngine.Debug.LogError(text + "Still missing materials,locate at (from 0)：" + n);
					}
				}
				return;
			}
		}

		private void CheckTouch()
		{
			if (this.m_uiCamera == null)
			{
				this.m_uiCamera = GameObject.Find("ViewRoot/Camera").GetComponent<Camera>();
				return;
			}
			if (Input.touchCount == 1)
			{
				if (Input.GetTouch(0).phase == TouchPhase.Began)
				{
					this.m_ray = this.m_uiCamera.ScreenPointToRay(Input.GetTouch(0).position);
					this.m_hits = Physics.RaycastAll(this.m_ray, 9999f);
					for (int i = 0; i < this.m_hits.Length; i++)
					{
						if (this.m_hits[i].collider.gameObject == base.gameObject)
						{
							this.m_bCanRotate = true;
							break;
						}
					}
				}
			}
			else if (this.m_bCanRotate)
			{
				this.CancelRotate();
			}
		}

		private void ProcessTouch()
		{
			this.m_curFramePoint = this.m_uiCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 3805f));
		}

		private void Rotate()
		{
			if (this.m_priorFramePoint != Vector3.zero)
			{
				float num = this.m_priorFramePoint.x - this.m_curFramePoint.x;
				this.m_fDizzinessValue += ((num <= 0f) ? (0f - num) : num);
				if (null != this.smodel)
				{
					this.smodel.transform.localEulerAngles += new Vector3(0f, num, 0f);
				}
			}
			this.m_priorFramePoint = this.m_curFramePoint;
		}

		private void CancelRotate()
		{
			if (this.m_fDizzinessValue > 5400f)
			{
				base.StartCoroutine(this.Dizziness_Coroutine());
			}
			Quaternion rot = default(Quaternion);
			rot.eulerAngles = new Vector3(0f, 180f, 0f);
			if (null != this.smodel)
			{
				this.m_uiTweener = TweenRotation.Begin(this.smodel, 0.8f, rot);
			}
			this.m_fDizzinessValue = 0f;
			this.m_priorFramePoint = Vector3.zero;
			this.m_bCanRotate = false;
		}

		[DebuggerHidden]
		private IEnumerator Dizziness_Coroutine()
		{
			PropViewModel.<Dizziness_Coroutine>c__Iterator184 <Dizziness_Coroutine>c__Iterator = new PropViewModel.<Dizziness_Coroutine>c__Iterator184();
			<Dizziness_Coroutine>c__Iterator.<>f__this = this;
			return <Dizziness_Coroutine>c__Iterator;
		}
	}
}
