using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaMessageData;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewCollection : MonoBehaviour
	{
		private Transform heroCollection;

		public UIScrollView verticalView;

		private UIGrid grid;

		private UISprite sideBG;

		private UILabel quality;

		private UILabel itemName;

		private PropViewCollectionUnit collectUnit;

		private EffectItem effectItem;

		private EffectItem currEffectItem;

		private Dictionary<string, SysGameItemsVo> sv = new Dictionary<string, SysGameItemsVo>();

		private Dictionary<int, GameObject> objs = new Dictionary<int, GameObject>();

		private List<SysGameItemsVo> sgivs = new List<SysGameItemsVo>();

		private List<int> typeList = new List<int>();

		private List<int> ItemListCurrHero = new List<int>();

		private List<int> ItemListAllHero = new List<int>();

		private GameObject hitchPoint;

		private GameObject targetModel;

		private GameObject perfabObject1;

		private GameObject perfabObject2;

		private GameObject perfabObject3;

		private GameObject showEffectObject1;

		private GameObject showEffectObject2;

		private GameObject showEffectObject3;

		private Coroutine corSetData;

		private object[] mgs;

		private int typeNum;

		private long heroID;

		private string heroNPC = string.Empty;

		public bool isSort;

		private bool isExchange;

		private static bool isFirstRefresh = true;

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientC2V.DemountCollection,
				ClientC2V.WearCollection,
				ClientV2C.sacriviewChangeHero,
				ClientV2C.propviewChangeToggle,
				ClientV2C.propviewCollectionAfterBuying,
				ClientV2C.propviewClickCollectionItem,
				ClientV2C.propviewCollectionWearEffect,
				ClientC2C.WaitServerResponseTimeOut,
				ClientC2C.GateConnected,
				ClientC2C.GateDisconnected
			};
			this.Initialize();
		}

		private void OnEnable()
		{
			this.Register();
			SacrificialCtrl.GetInstance().collectionState = CollectionState.Nothing;
		}

		private void OnDisable()
		{
			this.Unregister();
			base.StopAllCoroutines();
			this.corSetData = null;
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
			this.heroCollection = base.transform.Find("HeroCollection");
			this.effectItem = Resources.Load<EffectItem>("Prefab/UI/Sacrificial/EffectItem");
			this.collectUnit = Resources.Load<PropViewCollectionUnit>("Prefab/UI/Sacrificial/PropCollectionUnit");
			this.verticalView = this.heroCollection.Find("VerticalView").GetComponent<UIScrollView>();
			this.grid = this.verticalView.transform.Find("Grid").GetComponent<UIGrid>();
			this.sv = BaseDataMgr.instance.GetTypeDicByType<SysGameItemsVo>();
			this.sgivs = Tools_ParsePrice.ParseGameItemData(10, this.sv);
			this.typeList.Clear();
			for (int i = 0; i < this.sgivs.Count; i++)
			{
				this.typeList.Add(this.sgivs[i].hero_decorate_type);
			}
			this.typeList.Sort();
			this.typeList.Reverse();
			this.typeNum = this.typeList[0];
			this.verticalView.considerInactive = false;
			this.verticalView.mCalculatedBounds = false;
			PropViewCollection.isFirstRefresh = true;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void OnMsg_sacriviewChangeHero(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string modelID = string.Empty;
				modelID = (string)msg.Param;
				this.heroNPC = modelID;
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(modelID);
				if (heroInfoData != null)
				{
					this.heroID = heroInfoData.HeroId;
					this.GenerateUnit();
					if (null != this.currEffectItem)
					{
						MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewClickCollectionItem, this.currEffectItem, false);
					}
				}
			}
		}

		private void OnMsg_propviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				PropertyType propertyType = (PropertyType)((int)msg.Param);
				this.heroCollection.gameObject.SetActive(propertyType == PropertyType.Collection);
				if (propertyType == PropertyType.Collection)
				{
					if (this.corSetData != null)
					{
						base.StopCoroutine("AllocateUnitData");
					}
					this.corSetData = base.StartCoroutine("AllocateUnitData");
				}
			}
		}

		private void OnMsg_propviewClickCollectionItem(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				EffectItem tempItem = (EffectItem)msg.Param;
				this.currEffectItem = tempItem;
				GridHelper.FillGrid<PropViewCollectionUnit>(this.grid, this.collectUnit, this.typeNum, delegate(int idx, PropViewCollectionUnit comp)
				{
					comp.RefreshClickItemState(tempItem);
				});
			}
		}

		private void OnMsg_propviewCollectionAfterBuying(MobaMessage msg)
		{
			if (msg != null)
			{
				GridHelper.FillGrid<PropViewCollectionUnit>(this.grid, this.collectUnit, this.typeNum, delegate(int idx, PropViewCollectionUnit comp)
				{
					comp.RefreshData();
					comp.SetData(false);
				});
			}
		}

		private void OnMsg_propviewCollectionWearEffect(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				int num = (int)msg.Param;
				if (num != 0)
				{
					this.WearEffect(num);
				}
			}
		}

		private void OnMsg_WaitServerResponseTimeOut(MobaMessage msg)
		{
			if (msg != null)
			{
				MobaMessageType mobaMessageType = MobaMessageType.GameCode;
				int num = 230;
				MsgData_WaitServerResponsTimeout msgData_WaitServerResponsTimeout = (MsgData_WaitServerResponsTimeout)msg.Param;
				if (msgData_WaitServerResponsTimeout.MobaMsgType == mobaMessageType && msgData_WaitServerResponsTimeout.MsgID == num)
				{
					this.GenerateUnit();
					if (null != this.currEffectItem)
					{
						MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewClickCollectionItem, this.currEffectItem, false);
					}
					SacrificialCtrl.GetInstance().collectionState = CollectionState.Nothing;
				}
			}
		}

		private void OnMsg_GateConnected(MobaMessage msg)
		{
			this.GenerateUnit();
			if (null != this.currEffectItem)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewClickCollectionItem, this.currEffectItem, false);
			}
			SacrificialCtrl.GetInstance().collectionState = CollectionState.Nothing;
		}

		private void OnMsg_GateDisconnected(MobaMessage msg)
		{
			this.GenerateUnit();
			if (null != this.currEffectItem)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewClickCollectionItem, this.currEffectItem, false);
			}
			SacrificialCtrl.GetInstance().collectionState = CollectionState.Nothing;
		}

		private void OnMsg_WearCollection(MobaMessage msg)
		{
			if (msg != null)
			{
				EquipmentInfoData equipmentinfodata = (EquipmentInfoData)msg.Param;
				GridHelper.FillGrid<PropViewCollectionUnit>(this.grid, this.collectUnit, this.typeNum, delegate(int idx, PropViewCollectionUnit comp)
				{
					comp.RefreshData();
					comp.RefreshUnit(equipmentinfodata, 1);
				});
			}
			MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewClickCollectionItem, this.currEffectItem, false);
			SacrificialCtrl.GetInstance().collectionState = CollectionState.Nothing;
		}

		private void OnMsg_DemountCollection(MobaMessage msg)
		{
			if (msg != null)
			{
				if (!this.isExchange)
				{
					EquipmentInfoData equipmentinfodata = (EquipmentInfoData)msg.Param;
					GridHelper.FillGrid<PropViewCollectionUnit>(this.grid, this.collectUnit, this.typeNum, delegate(int idx, PropViewCollectionUnit comp)
					{
						comp.RefreshData();
						comp.RefreshUnit(equipmentinfodata, 2);
					});
					MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewClickCollectionItem, this.currEffectItem, false);
				}
				else
				{
					int[] param = new int[]
					{
						this.currEffectItem.ModelID,
						1
					};
					MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewUseEffectItem, param, false);
					this.isExchange = false;
				}
			}
			SacrificialCtrl.GetInstance().collectionState = CollectionState.Nothing;
		}

		private void WearEffect(int type)
		{
			int[] array = new int[2];
			if (type == 1)
			{
				int num = this.IsExchangeEffect();
				if (num != 0)
				{
					array[0] = num;
					array[1] = 2;
					MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewUseEffectItem, array, false);
					this.isExchange = true;
					return;
				}
			}
			array[0] = this.currEffectItem.ModelID;
			array[1] = type;
			MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewUseEffectItem, array, false);
		}

		private void GenerateUnit()
		{
			GridHelper.FillGrid<PropViewCollectionUnit>(this.grid, this.collectUnit, this.typeNum, delegate(int idx, PropViewCollectionUnit comp)
			{
				comp.Init(this.sgivs.FindAll((SysGameItemsVo sv) => sv.hero_decorate_type == idx + 1), idx + 1, this.verticalView, this.heroID);
				comp.name = (idx + EffectType.Pet).ToString();
			});
			this.grid.Reposition();
			this.verticalView.ResetPosition();
			this.verticalView.considerInactive = false;
			this.verticalView.mCalculatedBounds = false;
		}

		private int IsExchangeEffect()
		{
			int result = 0;
			HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byHeroID_X(this.heroID);
			if (heroInfoData == null)
			{
			}
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.currEffectItem.ModelID.ToString());
			switch (dataById.hero_decorate_type)
			{
			case 1:
				result = heroInfoData.petId;
				break;
			case 2:
				result = heroInfoData.tailEffectId;
				break;
			case 3:
				result = heroInfoData.levelEffectId;
				break;
			case 4:
				result = heroInfoData.backEffectId;
				break;
			case 5:
				result = heroInfoData.birthEffectId;
				break;
			case 6:
				result = heroInfoData.deathEffectId;
				break;
			case 7:
				result = heroInfoData.eyeUnitSkinId;
				break;
			}
			return result;
		}

		[DebuggerHidden]
		private IEnumerator AllocateUnitData()
		{
			PropViewCollection.<AllocateUnitData>c__Iterator17F <AllocateUnitData>c__Iterator17F = new PropViewCollection.<AllocateUnitData>c__Iterator17F();
			<AllocateUnitData>c__Iterator17F.<>f__this = this;
			return <AllocateUnitData>c__Iterator17F;
		}
	}
}
