using Assets.Scripts.GUILogic.View.PropertyView;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.Runes
{
	public class RunesRightInlay : MonoBehaviour
	{
		private RunesOperationItem runesoperationitem;

		public RunesOperationItem operatingRuneItem;

		private Transform transInlay;

		private Transform transfilterOption;

		private UIScrollView scrollview;

		private UIGrid gridRunesArea;

		private int runesPosition;

		private RunesPara thisRP = new RunesPara
		{
			isNeedRefresh = true
		};

		private List<UIToggle> btnFilter;

		private List<EquipmentInfoData> eidList = new List<EquipmentInfoData>();

		private List<RunesOperationItem> allRunesList = new List<RunesOperationItem>();

		private object[] mgs;

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.runesviewChangeToggle,
				ClientV2C.runesviewClickRune,
				ClientC2V.EquipInlay,
				ClientC2V.EquipDemount,
				ClientC2V.RunesAllDemountEquip
			};
			this.Initialize();
		}

		private void OnEnable()
		{
			this.Register();
		}

		private void OnDisable()
		{
			this.Unregister();
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
			this.btnFilter = new List<UIToggle>();
			this.transInlay = base.transform.Find("Inlay");
			this.transfilterOption = this.transInlay.Find("FilterOption");
			this.scrollview = this.transInlay.Find("HeroRunesArea").GetComponent<UIScrollView>();
			this.gridRunesArea = this.scrollview.transform.Find("Grid").GetComponent<UIGrid>();
			this.runesoperationitem = Resources.Load<RunesOperationItem>("Prefab/UI/Sacrificial/RunesOperationItem");
			for (int num = 0; num != this.transfilterOption.transform.childCount; num++)
			{
				this.btnFilter.Add(this.transfilterOption.transform.GetChild(num).GetComponent<UIToggle>());
				UIEventListener expr_C5 = UIEventListener.Get(this.transfilterOption.transform.GetChild(num).gameObject);
				expr_C5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_C5.onClick, new UIEventListener.VoidDelegate(this.ClickFilter));
			}
			for (int num2 = 0; num2 != this.btnFilter.Count; num2++)
			{
				this.btnFilter[num2].optionCanBeNone = true;
				this.btnFilter[num2].value = false;
			}
		}

		private void OnMsg_EquipInlay(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				int modelid = (int)msg.Param;
				this.CollectRunes();
				this.GenerateSort(modelid);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewSkipAnotherRune, null, false);
			}
		}

		private void OnMsg_EquipDemount(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				if (null != this.operatingRuneItem && !string.IsNullOrEmpty(this.operatingRuneItem.EquipID) && this.operatingRuneItem.EquipID.CompareTo("0") != 0)
				{
					this.InlayOrDemount(this.operatingRuneItem.EquipID, this.thisRP.runesItem.ModelID.ToString(), this.runesPosition);
					this.operatingRuneItem = null;
					return;
				}
				this.CollectRunes();
				this.GenerateSort(0);
			}
		}

		private void OnMsg_RunesAllDemountEquip(MobaMessage msg)
		{
			if (msg != null)
			{
				this.CollectRunes();
				this.GenerateSort(0);
			}
		}

		private void OnMsg_runesviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				RunesFunctionType runesFunctionType = (RunesFunctionType)((int)msg.Param);
				this.transInlay.gameObject.SetActive(false);
				if (runesFunctionType == RunesFunctionType.Inlay)
				{
					this.transInlay.gameObject.SetActive(true);
					this.thisRP.isNeedRefresh = true;
					this.thisRP.runesItem = null;
					this.CollectRunes();
					this.GenerateSort(0);
				}
			}
		}

		private void OnMsg_runesviewClickRune(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				RunesPara runesPara = default(RunesPara);
				try
				{
					runesPara = (RunesPara)msg.Param;
				}
				catch
				{
					return;
				}
				this.thisRP = runesPara;
				this.runesPosition = (int)runesPara.runesItem.RunePosition;
				if (runesPara.isNeedRefresh)
				{
					this.CollectRunes();
					this.GenerateSort((!(null == runesPara.runesItem)) ? runesPara.runesItem.ModelID : 0);
				}
			}
		}

		private void ClickFilter(GameObject obj)
		{
			if (null != obj)
			{
				UIToggle activeToggle = UIToggle.GetActiveToggle(3);
				if (null == activeToggle)
				{
					for (int num = 0; num != this.allRunesList.Count; num++)
					{
						this.allRunesList[num].gameObject.SetActive(true);
					}
					this.RePosition();
					return;
				}
				for (int num2 = 0; num2 != this.allRunesList.Count; num2++)
				{
					this.allRunesList[num2].gameObject.SetActive(false);
					if (this.allRunesList[num2].Runesvariety == (RunesVariety)((int)Enum.Parse(typeof(RunesVariety), obj.name)))
					{
						this.allRunesList[num2].gameObject.SetActive(true);
					}
				}
				this.RePosition();
			}
		}

		private void CollectRunes()
		{
			this.eidList.Clear();
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			Dictionary<string, SysGameItemsVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysGameItemsVo>();
			if (typeDicByType == null)
			{
				return;
			}
			for (int num = 0; num != list.Count; num++)
			{
				if (list[num] != null)
				{
					string key = list[num].ModelId.ToString();
					if (typeDicByType.ContainsKey(key) && typeDicByType[key].type == 4)
					{
						this.eidList.Add(list[num]);
					}
				}
			}
		}

		private void GenerateSort(int modelid)
		{
			if (modelid != 0 && !this.eidList.Any((EquipmentInfoData obj) => obj.ModelId == modelid))
			{
				EquipmentInfoData item = new EquipmentInfoData
				{
					ModelId = modelid,
					EquipmentId = 0L,
					Count = 0
				};
				this.eidList.Add(item);
			}
			if (this.eidList.Count < this.gridRunesArea.transform.childCount)
			{
				int num = this.gridRunesArea.transform.childCount - this.eidList.Count;
				int num2 = this.gridRunesArea.transform.childCount - 1;
				while (this.eidList.Count != this.gridRunesArea.transform.childCount)
				{
					UnityEngine.Object.DestroyImmediate(this.gridRunesArea.transform.GetChild(num2).gameObject);
					num2--;
				}
			}
			GridHelper.FillGrid<RunesOperationItem>(this.gridRunesArea, this.runesoperationitem, this.eidList.Count, delegate(int idx, RunesOperationItem comp)
			{
				comp.Init(this.eidList[idx], modelid);
				comp.name = this.eidList[idx].ModelId.ToString();
				if (modelid != 0 && this.eidList[idx].ModelId == modelid)
				{
					comp.name = "0";
				}
				comp.ClickCallBack = new Callback<RunesOperationItem, GameObject>(this.Operation);
			});
			List<RunesOperationItem> list = new List<RunesOperationItem>();
			for (int num3 = 0; num3 != this.gridRunesArea.transform.childCount; num3++)
			{
				list.Add(this.gridRunesArea.transform.GetChild(num3).GetComponent<RunesOperationItem>());
			}
			list.Sort(new Comparison<RunesOperationItem>(this.SortByCustom));
			if (this.gridRunesArea.transform.childCount == list.Count)
			{
				for (int num4 = 0; num4 != list.Count; num4++)
				{
					RunesOperationItem runesOperationItem = list[num4];
					for (int num5 = 0; num5 != this.gridRunesArea.transform.childCount; num5++)
					{
						if (this.gridRunesArea.transform.GetChild(num5).name.CompareTo(runesOperationItem.name) == 0)
						{
							this.gridRunesArea.transform.GetChild(num5).name = string.Format("{0:D3}", num4);
							break;
						}
					}
				}
			}
			this.RePosition();
			this.allRunesList.Clear();
			for (int num6 = 0; num6 != this.gridRunesArea.transform.childCount; num6++)
			{
				this.allRunesList.Add(this.gridRunesArea.transform.GetChild(num6).GetComponent<RunesOperationItem>());
			}
			this.ReCheckToggle();
		}

		private void Operation(RunesOperationItem roi, GameObject obj)
		{
			AudioMgr.Play("Play_Menu_click", null, false, false);
			string name = obj.name;
			string text = name;
			switch (text)
			{
			case "Inlay":
			{
				if (null == this.thisRP.runesItem)
				{
					Singleton<TipView>.Instance.ShowViewSetText("请先选择一个符文槽！！！", 1f);
					return;
				}
				if (this.thisRP.runesItem.ModelID != 0)
				{
					this.operatingRuneItem = roi;
					this.InlayOrDemount("0", this.thisRP.runesItem.ModelID.ToString(), this.runesPosition);
					return;
				}
				string text2 = string.Empty;
				text2 = roi.EquipID;
				RunesOperaInfo runesOperaInfo = default(RunesOperaInfo);
				if (!string.IsNullOrEmpty(text2))
				{
					runesOperaInfo.equipID = text2;
					runesOperaInfo.modelID = roi.ModelID;
					runesOperaInfo.runesPosition = this.runesPosition;
				}
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInlayRunes, runesOperaInfo, false);
				break;
			}
			case "Demount":
			{
				string text2 = roi.EquipID;
				RunesOperaInfo runesOperaInfo = default(RunesOperaInfo);
				if (!string.IsNullOrEmpty(text2))
				{
					runesOperaInfo.equipID = text2;
					runesOperaInfo.modelID = roi.ModelID;
					runesOperaInfo.runesPosition = this.runesPosition;
				}
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewDemountRunes, runesOperaInfo, false);
				break;
			}
			}
		}

		private void InlayOrDemount(string equipid, string modelid, int position)
		{
			RunesOperaInfo runesOperaInfo = default(RunesOperaInfo);
			runesOperaInfo.equipID = equipid;
			runesOperaInfo.modelID = modelid;
			runesOperaInfo.runesPosition = position;
			if (equipid.CompareTo("0") == 0)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewDemountRunes, runesOperaInfo, false);
			}
			else
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInlayRunes, runesOperaInfo, false);
			}
		}

		private int SortByCustom(RunesOperationItem a, RunesOperationItem b)
		{
			if (!(null != a) || !(null != b))
			{
				return 0;
			}
			if (a.Runesstate > b.Runesstate)
			{
				return -1;
			}
			if (a.Runesstate < b.Runesstate)
			{
				return 1;
			}
			if (a.Runesclassification > b.Runesclassification)
			{
				return -1;
			}
			if (a.Runesclassification < b.Runesclassification)
			{
				return 1;
			}
			if (a.Count > b.Count)
			{
				return -1;
			}
			if (a.Count < b.Count)
			{
				return 1;
			}
			return a.ModelID.CompareTo(b.ModelID);
		}

		private void RePosition()
		{
			this.gridRunesArea.Reposition();
			this.scrollview.ResetPosition();
			this.scrollview.considerInactive = false;
			this.scrollview.mCalculatedBounds = false;
		}

		private void ReCheckToggle()
		{
			UIToggle activeToggle = UIToggle.GetActiveToggle(3);
			GameObject gameObject = new GameObject();
			if (null != activeToggle)
			{
				gameObject.name = activeToggle.name;
				this.ClickFilter(gameObject);
			}
			UnityEngine.Object.Destroy(gameObject);
		}
	}
}
