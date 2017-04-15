using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class RunesOperationItem : MonoBehaviour
	{
		[SerializeField]
		private UILabel rune_name;

		[SerializeField]
		private UILabel rune_attribute;

		[SerializeField]
		private UILabel rune_number;

		[SerializeField]
		private UISprite rune_bg;

		[SerializeField]
		private UILabel rune_state;

		[SerializeField]
		private Transform rune_mount;

		[SerializeField]
		private Transform rune_demount;

		[SerializeField]
		private UISprite rune_inlay;

		[SerializeField]
		private UISprite rune_compound;

		[SerializeField]
		private UISprite rune_purchase;

		private string modelID;

		private string equipID;

		public Callback<RunesOperationItem, GameObject> ClickCallBack;

		public Callback<RunesOperationItem, GameObject> Click_2_CallBack;

		private RunesClassification runesclassification;

		private RunesVariety runesvariety;

		private RunesState runesstate;

		private int classification;

		private int variety;

		private int count;

		private Color fullColor = new Color(0f, 1f, 0.141176477f, 1f);

		private Color lackColor = new Color(0.623529434f, 0f, 0.101960786f, 1f);

		public string ModelID
		{
			get
			{
				return this.modelID;
			}
		}

		public string EquipID
		{
			get
			{
				return this.equipID;
			}
		}

		public RunesClassification Runesclassification
		{
			get
			{
				return this.runesclassification;
			}
		}

		public RunesVariety Runesvariety
		{
			get
			{
				return this.runesvariety;
			}
		}

		public RunesState Runesstate
		{
			get
			{
				return this.runesstate;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public void Init(EquipmentInfoData _equipmentinfodata, int modelid)
		{
			this.modelID = _equipmentinfodata.ModelId.ToString();
			this.equipID = _equipmentinfodata.EquipmentId.ToString();
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.ModelID);
			try
			{
				this.variety = int.Parse(dataById.sub_type.Split(new char[]
				{
					'|'
				})[1]);
			}
			catch (Exception var_1_62)
			{
			}
			this.classification = dataById.quality;
			this.runesclassification = (RunesClassification)this.classification;
			this.runesvariety = (RunesVariety)this.variety;
			UIEventListener.Get(this.rune_inlay.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickItem);
			UIEventListener.Get(this.rune_demount.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickItem);
			this.count = _equipmentinfodata.Count;
			this.rune_number.text = this.count.ToString();
			if (_equipmentinfodata.Count < 0)
			{
				this.rune_number.text = "0";
			}
			this.rune_compound.gameObject.SetActive(false);
			if (modelid != 0)
			{
				bool flag = modelid == int.Parse(this.modelID);
				this.runesstate = ((!flag) ? RunesState.UnEquiped : RunesState.Equipted);
				this.rune_inlay.gameObject.SetActive(!flag);
				this.rune_demount.gameObject.SetActive(flag);
			}
			else
			{
				this.runesstate = RunesState.UnEquiped;
				this.rune_number.gameObject.SetActive(true);
				this.rune_inlay.gameObject.SetActive(true);
				this.rune_demount.gameObject.SetActive(false);
			}
			this.Details(dataById);
		}

		public void Initialize(EquipmentInfoData _equipmentinfodata, int modelid)
		{
			this.modelID = _equipmentinfodata.ModelId.ToString();
			this.equipID = _equipmentinfodata.EquipmentId.ToString();
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.ModelID);
			try
			{
				this.variety = int.Parse(dataById.sub_type.Split(new char[]
				{
					'|'
				})[1]);
			}
			catch (Exception var_1_61)
			{
			}
			this.classification = dataById.quality;
			this.runesclassification = (RunesClassification)this.classification;
			this.runesvariety = (RunesVariety)this.variety;
			UIEventListener.Get(this.rune_compound.gameObject).onClick = new UIEventListener.VoidDelegate(this.Click_2_Item);
			UIEventListener.Get(this.rune_purchase.gameObject).onClick = new UIEventListener.VoidDelegate(this.Click_2_Item);
			this.count = _equipmentinfodata.Count;
			this.rune_number.text = this.count.ToString();
			if (_equipmentinfodata.Count < 0)
			{
				this.rune_number.text = "0";
			}
			this.SetBtnState(this.count, dataById);
		}

		private void ClickItem(GameObject obj)
		{
			if (this.ClickCallBack != null)
			{
				this.ClickCallBack(this, obj);
			}
		}

		private void Click_2_Item(GameObject obj)
		{
			if (this.Click_2_CallBack != null)
			{
				this.Click_2_CallBack(this, obj);
			}
		}

		private void SetBtnState(int count, SysGameItemsVo gameitems)
		{
			if (count < 0)
			{
				return;
			}
			int num = 0;
			this.rune_number.gameObject.SetActive(true);
			this.rune_inlay.gameObject.SetActive(false);
			this.rune_demount.gameObject.SetActive(false);
			bool flag = int.TryParse(gameitems.consumption, out num);
			if (flag)
			{
				if (this.runesclassification == RunesClassification.Primary)
				{
					this.rune_compound.gameObject.SetActive(count >= num);
					this.rune_purchase.gameObject.SetActive(count < num);
				}
				else if (this.runesclassification == RunesClassification.Middle)
				{
					this.rune_compound.gameObject.SetActive(count >= num);
					this.rune_purchase.gameObject.SetActive(false);
				}
			}
			else
			{
				this.rune_compound.gameObject.SetActive(false);
				this.rune_purchase.gameObject.SetActive(false);
			}
			this.rune_number.color = ((count >= 3) ? this.fullColor : this.lackColor);
			this.Details(gameitems);
		}

		private void Details(SysGameItemsVo gameitems)
		{
			this.rune_name.text = LanguageManager.Instance.GetStringById(gameitems.name);
			RunesState runesState = this.runesstate;
			if (runesState != RunesState.UnEquiped)
			{
				if (runesState == RunesState.Equipted)
				{
					this.rune_state.text = "剩余:";
				}
			}
			else
			{
				this.rune_state.text = "剩余:";
			}
			switch (gameitems.quality)
			{
			case 2:
				this.rune_name.color = new Color(0.003921569f, 0.509803951f, 0.8745098f);
				break;
			case 3:
				this.rune_name.color = new Color(0.6509804f, 0f, 0.7882353f);
				break;
			case 4:
				this.rune_name.color = new Color(0.7411765f, 0.419607848f, 0f);
				break;
			case 5:
				this.rune_name.color = new Color(0.7490196f, 0.5803922f, 0f);
				break;
			}
			string[] array = gameitems.attribute.Split(new char[]
			{
				'|'
			});
			this.rune_attribute.text = ((!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? ("+" + float.Parse(array[1]).ToString(array[2]) + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)) : ("+" + array[1] + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)));
			this.rune_bg.spriteName = gameitems.icon;
		}
	}
}
