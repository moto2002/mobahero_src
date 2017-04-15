using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ModelRotate : MonoBehaviour
{
	[SerializeField]
	private Camera m_uiCamera;

	[SerializeField]
	private Transform m_modelRoot;

	[SerializeField]
	private float m_fRotateSpeed;

	[SerializeField]
	private GameObject m_EvolutionEff;

	[SerializeField]
	private GameObject m_QualityUpEff;

	private GameObject m_modelObj;

	private Transform m_rotary;

	private UITweener m_uiTweener;

	private Ray m_ray;

	private RaycastHit[] m_hits;

	private Vector3 m_curFramePoint = Vector3.zero;

	private Vector3 m_priorFramePoint = Vector3.zero;

	private Renderer[] m_renderers;

	private Shader m_shaderFadeInOut;

	private Animator m_animatorControllerHeroBase;

	private float m_fDizzinessValue;

	private bool m_bCanRotate;

	public bool ShowOrHideModel;

	private Animator m_animatorControllerHeroObj;

	private int ID_HitStun;

	private int ID_HitStunEnd;

	private int m_nLayerUnit;

	private int skinid;

	public string hero_id = string.Empty;

	private string prevhero = string.Empty;

	public GameObject ModelObj
	{
		get
		{
			return this.m_modelObj;
		}
		set
		{
			this.m_modelObj = value;
		}
	}

	public int SkinID
	{
		get
		{
			return this.skinid;
		}
		set
		{
			this.skinid = value;
		}
	}

	private void Awake()
	{
		this.m_shaderFadeInOut = Shader.Find("Custom/FadeInOut");
		this.m_nLayerUnit = LayerMask.NameToLayer("Unit");
		this.m_rotary = base.transform.Find("WJ_Zhuantai");
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.m_modelObj);
	}

	public void ChangeModel(string heroName, string heroId, string npcid)
	{
		this.Stop();
		this.InitModel(heroName, heroId, npcid);
		this.ShowModel();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		this.Stop();
	}

	private void Stop()
	{
		base.CancelInvoke();
		base.StopAllCoroutines();
	}

	private string GetHeroModelName(string hero_id)
	{
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(hero_id);
		return heroMainData.model_id;
	}

	public void InitModel(string material, string heroID, string npcid)
	{
		this.hero_id = npcid;
		GameObject gameObject = ResourceManager.Load<GameObject>(heroID, true, true, null, 0, false);
		string text = this.path(gameObject.name);
		Transform transform = gameObject.transform.FindChild("Model");
		Transform transform2 = transform.transform.FindChild(text);
		if (null == transform2)
		{
			CtrlManager.ShowMsgBox("名称不匹配!!!", "Skinnedmeshrender组件的名字(我们不知道是什么) 不匹配Smodel的名字: \"" + text + "\" 也许有人可以修复一下", delegate
			{
			}, PopViewType.PopOneButton, "确定", "取消", null);
			return;
		}
		SkinnedMeshRenderer component = transform2.GetComponent<SkinnedMeshRenderer>();
		for (int i = 0; i < component.materials.Length; i++)
		{
			if (null == component.materials[i] || !component.materials[i].name.Contains(text))
			{
				UnityEngine.Debug.LogError(text + "Still missing materials(perfab),locate at (from 0)：" + i);
			}
		}
		if (gameObject != null)
		{
			this.InitSModel(gameObject);
			return;
		}
	}

	private void InitSModel(GameObject md)
	{
		if (this.m_modelObj != null)
		{
			UnityEngine.Object.Destroy(this.m_modelObj);
			this.m_modelObj = null;
			UnityEngine.Object.Destroy(this.m_animatorControllerHeroObj);
			this.m_animatorControllerHeroObj = null;
			UnityEngine.Object.Destroy(this.m_animatorControllerHeroBase);
			this.m_animatorControllerHeroBase = null;
		}
		this.m_modelObj = (UnityEngine.Object.Instantiate(md) as GameObject);
		if (this.m_modelObj != null)
		{
			this.m_animatorControllerHeroObj = this.m_modelObj.GetComponentInChildren<Animator>();
		}
		this.ID_HitStun = Animator.StringToHash("IsHitStun");
		SkinnedMeshRenderer[] componentsInChildren = this.m_modelObj.GetComponentsInChildren<SkinnedMeshRenderer>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!(componentsInChildren[i] != null) || componentsInChildren[i].sharedMesh != null)
				{
				}
			}
		}
		this.m_modelObj.transform.parent = this.m_modelRoot;
		this.m_modelObj.transform.localScale = Vector3.one * 200f;
		this.m_modelObj.transform.localPosition = new Vector3(200f, 165f, -40f);
		this.m_modelObj.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		this.m_modelObj.SetActive(false);
	}

	public void ShowModel()
	{
		if (this.m_modelObj != null)
		{
			this.m_modelObj.SetActive(true);
			if (this.prevhero != string.Empty)
			{
				AudioMgr.unloadLanguageSoundBank(this.prevhero, 0);
			}
			Hero component = this.m_modelObj.GetComponent<Hero>();
			string text = string.Empty;
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.hero_id);
			if (heroMainData != null)
			{
				text = heroMainData.music_id;
			}
			AudioMgr.SetLisenerPos_NoDir(AudioMgr.Instance.transform, Vector3.zero, 0);
			AudioMgr.loadLanguageSoundBank(text, 0);
			HeroVoicePlayer.playHeroVoice(text, true, AudioMgr.Instance.exSoundObj);
			this.prevhero = component.musicid;
			if (this.m_animatorControllerHeroObj != null)
			{
				this.m_animatorControllerHeroObj.Play("HeroBreath1");
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator ShowModel_Coroutine()
	{
		ModelRotate.<ShowModel_Coroutine>c__Iterator1D7 <ShowModel_Coroutine>c__Iterator1D = new ModelRotate.<ShowModel_Coroutine>c__Iterator1D7();
		<ShowModel_Coroutine>c__Iterator1D.<>f__this = this;
		return <ShowModel_Coroutine>c__Iterator1D;
	}

	public void HideModel()
	{
		if (base.gameObject != null && this.m_animatorControllerHeroBase != null)
		{
			base.StartCoroutine("HideModel_Coroutine");
		}
	}

	[DebuggerHidden]
	public IEnumerator HideModel_Clone()
	{
		ModelRotate.<HideModel_Clone>c__Iterator1D8 <HideModel_Clone>c__Iterator1D = new ModelRotate.<HideModel_Clone>c__Iterator1D8();
		<HideModel_Clone>c__Iterator1D.<>f__this = this;
		return <HideModel_Clone>c__Iterator1D;
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
		if (this.m_modelObj != null)
		{
			this.m_rotary.localEulerAngles = new Vector3(-90f, this.m_modelObj.transform.localEulerAngles.y, 0f);
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
			this.m_modelObj.transform.localEulerAngles += new Vector3(0f, num, 0f);
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
		if (null != this.m_modelObj)
		{
			this.m_uiTweener = TweenRotation.Begin(this.m_modelObj, 0.8f, rot);
		}
		this.m_fDizzinessValue = 0f;
		this.m_priorFramePoint = Vector3.zero;
		this.m_bCanRotate = false;
	}

	[DebuggerHidden]
	private IEnumerator Dizziness_Coroutine()
	{
		ModelRotate.<Dizziness_Coroutine>c__Iterator1D9 <Dizziness_Coroutine>c__Iterator1D = new ModelRotate.<Dizziness_Coroutine>c__Iterator1D9();
		<Dizziness_Coroutine>c__Iterator1D.<>f__this = this;
		return <Dizziness_Coroutine>c__Iterator1D;
	}

	[DebuggerHidden]
	private IEnumerator ReboundState_Coroutine(int ID, float time, bool deathFlag = false)
	{
		ModelRotate.<ReboundState_Coroutine>c__Iterator1DA <ReboundState_Coroutine>c__Iterator1DA = new ModelRotate.<ReboundState_Coroutine>c__Iterator1DA();
		<ReboundState_Coroutine>c__Iterator1DA.time = time;
		<ReboundState_Coroutine>c__Iterator1DA.ID = ID;
		<ReboundState_Coroutine>c__Iterator1DA.<$>time = time;
		<ReboundState_Coroutine>c__Iterator1DA.<$>ID = ID;
		<ReboundState_Coroutine>c__Iterator1DA.<>f__this = this;
		return <ReboundState_Coroutine>c__Iterator1DA;
	}

	public void ShowEvolutionEffect()
	{
		this.m_EvolutionEff.SetActive(false);
		this.m_EvolutionEff.SetActive(true);
	}

	public void ShowQualityUpEffect()
	{
		this.m_QualityUpEff.SetActive(false);
		this.m_QualityUpEff.SetActive(true);
	}

	public void CheckMaterial()
	{
		List<string> list = new List<string>();
		string text = this.path(this.m_modelObj.name);
		Transform transform = this.m_modelObj.transform.FindChild("Model");
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

	private string path(string name)
	{
		int length = name.IndexOf("_");
		return name.Substring(0, length);
	}
}
