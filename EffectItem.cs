using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectItem : MonoBehaviour
{
	[SerializeField]
	private UISprite frame;

	[SerializeField]
	private UITexture contentTexture;

	[SerializeField]
	private Transform exclamation;

	[SerializeField]
	private Transform newmark;

	[SerializeField]
	private UILabel countLabel;

	[SerializeField]
	private UISprite lockPic;

	private EffectType effectType;

	private SysGameItemsVo gameitem;

	private int source;

	private int quality;

	private int modelID;

	private int count;

	private int type;

	private bool isFirstTime = true;

	private bool isNew;

	private bool isOwned;

	private bool hasWore;

	private bool isUsing;

	private bool ischosen;

	private bool isCurrentHero;

	private new string name = string.Empty;

	private string icon = string.Empty;

	public Callback<GameObject> ClickCallback;

	private UIDragScrollView ug1;

	private UIDragScrollView ug2;

	public int Source
	{
		get
		{
			return this.source;
		}
	}

	public int Quality
	{
		get
		{
			return this.quality;
		}
	}

	public int ModelID
	{
		get
		{
			return this.modelID;
		}
	}

	public int Count
	{
		get
		{
			return this.count;
		}
	}

	public int Type
	{
		get
		{
			return this.type;
		}
	}

	public bool IsNew
	{
		get
		{
			return this.isNew;
		}
	}

	public bool IsOwned
	{
		get
		{
			return this.isOwned;
		}
	}

	public bool HasWore
	{
		get
		{
			return this.hasWore;
		}
	}

	public bool IsUsing
	{
		get
		{
			return this.isUsing;
		}
	}

	public bool IsChosen
	{
		get
		{
			return this.ischosen;
		}
	}

	public bool IsCurrentHero
	{
		get
		{
			return this.isCurrentHero;
		}
	}

	public string Name
	{
		get
		{
			return this.name;
		}
	}

	public UIDragScrollView UG1
	{
		get
		{
			return this.ug1;
		}
	}

	public UIDragScrollView UG2
	{
		get
		{
			return this.ug2;
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		this.frame.spriteName = "Hero_personal_frame_0" + this.quality;
		this.frame.SetDimensions(176, 176);
	}

	public void GenerateScrollView(UIScrollView hor, UIScrollView ver)
	{
		if (null == hor || null == ver)
		{
			return;
		}
		DIYDrag component = this.frame.GetComponent<DIYDrag>();
		UIDragScrollView[] components = this.frame.GetComponents<UIDragScrollView>();
		if (components.Length != 2)
		{
			for (int num = components.Length - 1; num != -1; num--)
			{
				UnityEngine.Object.Destroy(components[num]);
			}
			this.ug1 = this.frame.gameObject.AddComponent<UIDragScrollView>();
			this.ug2 = this.frame.gameObject.AddComponent<UIDragScrollView>();
			this.ug1.scrollView = hor;
			this.ug2.scrollView = ver;
		}
		else
		{
			this.ug1 = components[0];
			this.ug2 = components[1];
			this.ug1.scrollView = hor;
			this.ug2.scrollView = ver;
		}
		if (null == component)
		{
			this.frame.gameObject.AddComponent<DIYDrag>();
		}
	}

	public void Init(EquipmentInfoData eid, SysGameItemsVo sgi, EffectType effType, int idx, long heroid)
	{
		this.effectType = effType;
		this.modelID = int.Parse(sgi.items_id);
		this.gameitem = sgi;
		this.quality = this.gameitem.quality;
		this.type = this.gameitem.hero_decorate_type;
		this.icon = this.gameitem.icon;
		this.name = LanguageManager.Instance.GetStringById(this.gameitem.name);
		this.frame.spriteName = "Hero_personal_frame_0" + this.quality;
		this.frame.SetDimensions(176, 176);
		this.CollectionIsUsing(heroid);
		this.CountShow(eid);
		this.NewShow(eid);
		UIEventListener.Get(this.frame.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickItem);
	}

	private void Update()
	{
		if (this.contentTexture.isVisible && this.icon != string.Empty && this.contentTexture.mainTexture == null)
		{
			this.contentTexture.mainTexture = ResourceManager.Load<Texture>(this.icon, true, true, null, 0, false);
		}
	}

	private void InitEquipCount(EquipmentInfoData eid)
	{
		this.count = ((eid != null) ? eid.Count : 0);
		this.isOwned = (null != eid);
		this.countLabel.text = "x" + this.count;
	}

	private void InitEquipNewMark(EquipmentInfoData eid)
	{
		SysCustomizationVo dataById = BaseDataMgr.instance.GetDataById<SysCustomizationVo>(this.modelID.ToString());
		this.newmark.gameObject.SetActive(dataById.newproduct == 1);
		this.isNew = (dataById.newproduct == 1);
		this.source = int.Parse(dataById.customization_source);
		if (dataById.newproduct != 1)
		{
			if (eid != null)
			{
				this.hasWore = (1 == eid.isweared);
				this.exclamation.gameObject.SetActive(!this.hasWore);
			}
			else
			{
				this.exclamation.gameObject.SetActive(false);
			}
		}
		else
		{
			this.exclamation.gameObject.SetActive(false);
		}
	}

	private void CountShow(List<EquipmentInfoData> eid)
	{
		EquipmentInfoData equipmentInfoData = eid.Find((EquipmentInfoData obj) => obj.ModelId == this.modelID);
		this.count = ((equipmentInfoData != null) ? equipmentInfoData.Count : 0);
		this.isOwned = (null != equipmentInfoData);
		this.countLabel.text = "x" + this.count;
	}

	private void CountShow(EquipmentInfoData eid)
	{
		this.count = ((eid != null) ? eid.Count : 0);
		this.isOwned = (null != eid);
		this.countLabel.text = "x" + this.count;
	}

	private void NewShow(List<EquipmentInfoData> eid)
	{
		SysCustomizationVo dataById = BaseDataMgr.instance.GetDataById<SysCustomizationVo>(this.modelID.ToString());
		this.newmark.gameObject.SetActive(dataById.newproduct == 1);
		this.isNew = (dataById.newproduct == 1);
		this.source = int.Parse(dataById.customization_source);
		EquipmentInfoData equipmentInfoData = eid.Find((EquipmentInfoData obj) => obj.ModelId == this.modelID);
		if (dataById.newproduct != 1)
		{
			if (equipmentInfoData != null)
			{
				this.hasWore = (1 == equipmentInfoData.isweared);
				this.exclamation.gameObject.SetActive(!this.hasWore);
			}
			else
			{
				this.exclamation.gameObject.SetActive(false);
			}
		}
		else
		{
			this.exclamation.gameObject.SetActive(false);
		}
	}

	private void NewShow(EquipmentInfoData eid)
	{
		SysCustomizationVo dataById = BaseDataMgr.instance.GetDataById<SysCustomizationVo>(this.modelID.ToString());
		this.newmark.gameObject.SetActive(dataById.newproduct == 1);
		this.isNew = (dataById.newproduct == 1);
		this.source = int.Parse(dataById.customization_source);
		if (dataById.newproduct != 1)
		{
			if (eid != null)
			{
				this.hasWore = (1 == eid.isweared);
				this.exclamation.gameObject.SetActive(!this.hasWore);
			}
			else
			{
				this.exclamation.gameObject.SetActive(false);
			}
		}
		else
		{
			this.exclamation.gameObject.SetActive(false);
		}
	}

	private void FrameShow()
	{
		if (this.ischosen)
		{
			this.frame.spriteName = "Hero_personal_frame_select";
			this.frame.SetDimensions(202, 202);
		}
		else
		{
			this.frame.spriteName = "Hero_personal_frame_0" + this.quality;
			this.frame.SetDimensions(176, 176);
		}
	}

	public void CheckClickState()
	{
		this.FrameShow();
	}

	private void ClickItem(GameObject obj)
	{
		if (this.ClickCallback != null)
		{
			this.ClickCallback(base.gameObject);
		}
	}

	public void Choose(bool isTrue)
	{
		this.ischosen = isTrue;
	}

	public void Refresh(EquipmentInfoData eid, int eModelID, int type)
	{
		if (eModelID == this.modelID)
		{
			this.isUsing = (1 == type);
		}
		else
		{
			this.isUsing = false;
		}
		this.CountShow(eid);
		this.NewShow(eid);
	}

	public void CollectionIsUsing(long heroid)
	{
		HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byHeroID_X(heroid);
		if (heroInfoData != null)
		{
			if (heroInfoData.petId == this.modelID)
			{
				this.isUsing = true;
			}
			else if (heroInfoData.tailEffectId == this.modelID)
			{
				this.isUsing = true;
			}
			else if (heroInfoData.levelEffectId == this.modelID)
			{
				this.isUsing = true;
			}
			else if (heroInfoData.backEffectId == this.modelID)
			{
				this.isUsing = true;
			}
			else if (heroInfoData.birthEffectId == this.modelID)
			{
				this.isUsing = true;
			}
			else if (heroInfoData.deathEffectId == this.modelID)
			{
				this.isUsing = true;
			}
			else if (heroInfoData.eyeUnitSkinId == this.modelID)
			{
				this.isUsing = true;
			}
		}
	}

	public void CheckIsCurrentHero(List<int> tempItemCurrHero, List<int> tempItemAllHero)
	{
		this.isCurrentHero = tempItemCurrHero.Contains(this.modelID);
		SysCustomizationVo dataById = BaseDataMgr.instance.GetDataById<SysCustomizationVo>(this.modelID.ToString());
		if (!this.isOwned)
		{
			this.isOwned = tempItemAllHero.Contains(this.modelID);
		}
		if (!this.hasWore)
		{
			this.hasWore = tempItemAllHero.Contains(this.modelID);
		}
		if (dataById.newproduct != 1)
		{
			this.exclamation.gameObject.SetActive(!this.hasWore);
		}
		else
		{
			this.exclamation.gameObject.SetActive(false);
		}
		this.countLabel.gameObject.SetActive(this.isOwned);
		this.lockPic.gameObject.SetActive(!this.isOwned);
	}
}
