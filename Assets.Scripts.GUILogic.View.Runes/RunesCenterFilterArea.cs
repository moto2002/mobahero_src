using Assets.Scripts.GUILogic.View.PropertyView;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.Runes
{
	public class RunesCenterFilterArea : MonoBehaviour
	{
		private Transform transFilterArea;

		private Transform transProcessBar;

		private Transform transBG;

		private UIScrollView runesFilterView;

		private UIGrid gridFilterArea;

		private RunesOperationItem runesoperationitem;

		private List<EquipmentInfoData> eidList = new List<EquipmentInfoData>();

		private List<RunesOperationItem> allRunesList = new List<RunesOperationItem>();

		private List<RunesOperationItem> claRunesList = new List<RunesOperationItem>();

		private List<RunesOperationItem> varRunesList = new List<RunesOperationItem>();

		private object[] mgs;

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.runesviewChangeToggle,
				ClientV2C.runesviewFilterClassification,
				ClientV2C.runesviewFilterVariety,
				ClientV2C.runesviewOnCompoundAll,
				ClientV2C.coalesceviewCloseView,
				ClientC2V.RunesErrorRefreshEquipment
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
			this.transFilterArea = base.transform.Find("FilterArea");
			this.transProcessBar = base.transform.Find("Progress Bar");
			this.transBG = base.transform.Find("BG");
			this.runesFilterView = this.transFilterArea.GetComponent<UIScrollView>();
			this.gridFilterArea = this.transFilterArea.Find("Grid").GetComponent<UIGrid>();
			this.runesoperationitem = Resources.Load<RunesOperationItem>("Prefab/UI/Sacrificial/RunesOperationItem");
		}

		private void OnMsg_runesviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				RunesFunctionType runesFunctionType = (RunesFunctionType)((int)msg.Param);
				this.transFilterArea.gameObject.SetActive(runesFunctionType == RunesFunctionType.Storage);
				this.transProcessBar.gameObject.SetActive(runesFunctionType == RunesFunctionType.Storage);
				this.transBG.gameObject.SetActive(runesFunctionType == RunesFunctionType.Storage);
				this.InitArea(runesFunctionType == RunesFunctionType.Storage);
			}
		}

		private void OnMsg_runesviewOnCompoundAll(MobaMessage msg)
		{
			if (msg != null)
			{
				this.CollectRunes();
				this.GenerateSort(0);
			}
		}

		private void OnMsg_coalesceviewCloseView(MobaMessage msg)
		{
			if (msg != null)
			{
				this.CollectRunes();
				this.GenerateSort(0);
			}
		}

		private void OnMsg_RunesErrorRefreshEquipment(MobaMessage msg)
		{
			if (msg != null)
			{
				this.CollectRunes();
				this.GenerateSort(0);
			}
		}

		private void InitArea(bool isTrue)
		{
			if (isTrue)
			{
				this.CollectRunes();
				this.GenerateSort(0);
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
			if (this.eidList.Count < this.gridFilterArea.transform.childCount)
			{
				int num = this.gridFilterArea.transform.childCount - this.eidList.Count;
				int num2 = this.gridFilterArea.transform.childCount - 1;
				while (this.eidList.Count != this.gridFilterArea.transform.childCount)
				{
					UnityEngine.Object.DestroyImmediate(this.gridFilterArea.transform.GetChild(num2).gameObject);
					num2--;
				}
			}
			GridHelper.FillGrid<RunesOperationItem>(this.gridFilterArea, this.runesoperationitem, this.eidList.Count, delegate(int idx, RunesOperationItem comp)
			{
				comp.Initialize(this.eidList[idx], modelid);
				comp.name = this.eidList[idx].ModelId.ToString();
				comp.Click_2_CallBack = new Callback<RunesOperationItem, GameObject>(this.Operation);
			});
			List<RunesOperationItem> list = new List<RunesOperationItem>();
			for (int num3 = 0; num3 != this.gridFilterArea.transform.childCount; num3++)
			{
				list.Add(this.gridFilterArea.transform.GetChild(num3).GetComponent<RunesOperationItem>());
			}
			list.Sort(new Comparison<RunesOperationItem>(this.SortByCustom));
			if (this.gridFilterArea.transform.childCount == list.Count)
			{
				for (int num4 = 0; num4 != list.Count; num4++)
				{
					RunesOperationItem runesOperationItem = list[num4];
					for (int num5 = 0; num5 != this.gridFilterArea.transform.childCount; num5++)
					{
						if (this.gridFilterArea.transform.GetChild(num5).name.CompareTo(runesOperationItem.name) == 0)
						{
							this.gridFilterArea.transform.GetChild(num5).name = string.Format("{0:D3}", num4);
							break;
						}
					}
				}
			}
			this.RePosition();
			this.allRunesList.Clear();
			for (int num6 = 0; num6 != this.gridFilterArea.transform.childCount; num6++)
			{
				this.allRunesList.Add(this.gridFilterArea.transform.GetChild(num6).GetComponent<RunesOperationItem>());
			}
			this.claRunesList.Clear();
			this.varRunesList.Clear();
		}

		private void Operation(RunesOperationItem roi, GameObject obj)
		{
			string name = obj.name;
			string text = name;
			if (text != null)
			{
				if (RunesCenterFilterArea.<>f__switch$map27 == null)
				{
					RunesCenterFilterArea.<>f__switch$map27 = new Dictionary<string, int>(2)
					{
						{
							"Purchase",
							0
						},
						{
							"Compound",
							1
						}
					};
				}
				int num;
				if (RunesCenterFilterArea.<>f__switch$map27.TryGetValue(text, out num))
				{
					if (num != 0)
					{
						if (num == 1)
						{
							CtrlManager.OpenWindow(WindowID.SynthesisPopupView, null);
							MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewOpenOverView, roi.ModelID, false);
						}
					}
					else
					{
						CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
						Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.AfterBuying));
						Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.Props, roi.ModelID, 1, false);
					}
				}
			}
		}

		private void AfterBuying()
		{
			this.CollectRunes();
			this.GenerateSort(0);
			MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewCheckRune, null, false);
		}

		private void OnMsg_runesviewFilterClassification(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				RunesClassification rcf = (RunesClassification)((int)msg.Param);
				this.FilterClassification(rcf);
				this.RePosition();
			}
		}

		private void OnMsg_runesviewFilterVariety(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				RunesVariety rv = (RunesVariety)((int)msg.Param);
				this.FilterVariety(rv);
				this.RePosition();
			}
		}

		private void FilterClassification(RunesClassification rcf)
		{
			this.claRunesList.Clear();
			if (this.allRunesList.Count != this.gridFilterArea.transform.childCount)
			{
				this.allRunesList.Clear();
				for (int num = 0; num != this.gridFilterArea.transform.childCount; num++)
				{
					this.allRunesList.Add(this.gridFilterArea.transform.GetChild(num).GetComponent<RunesOperationItem>());
				}
			}
			UIToggle activeToggle = UIToggle.GetActiveToggle(5);
			bool flag = null != activeToggle && this.varRunesList.Count > 0;
			for (int num2 = 0; num2 != this.allRunesList.Count; num2++)
			{
				if (this.allRunesList[num2].Runesclassification == rcf)
				{
					this.claRunesList.Add(this.allRunesList[num2]);
				}
			}
			if (rcf != RunesClassification.All)
			{
				if (flag)
				{
					for (int num3 = 0; num3 != this.varRunesList.Count; num3++)
					{
						this.varRunesList[num3].gameObject.SetActive(this.varRunesList[num3].Runesclassification == rcf);
					}
				}
				else
				{
					for (int num4 = 0; num4 != this.allRunesList.Count; num4++)
					{
						this.allRunesList[num4].gameObject.SetActive(this.allRunesList[num4].Runesclassification == rcf);
					}
				}
			}
			else
			{
				this.varRunesList.Clear();
				for (int num5 = 0; num5 != this.allRunesList.Count; num5++)
				{
					this.allRunesList[num5].gameObject.SetActive(true);
				}
			}
		}

		private void FilterVariety(RunesVariety rv)
		{
			this.varRunesList.Clear();
			if (this.allRunesList.Count != this.gridFilterArea.transform.childCount)
			{
				this.allRunesList.Clear();
				for (int num = 0; num != this.gridFilterArea.transform.childCount; num++)
				{
					this.allRunesList.Add(this.gridFilterArea.transform.GetChild(num).GetComponent<RunesOperationItem>());
				}
			}
			UIToggle activeToggle = UIToggle.GetActiveToggle(4);
			bool flag = null != activeToggle && this.claRunesList.Count > 0;
			for (int num2 = 0; num2 != this.allRunesList.Count; num2++)
			{
				if (this.allRunesList[num2].Runesvariety == rv)
				{
					this.varRunesList.Add(this.allRunesList[num2]);
				}
			}
			if (flag)
			{
				for (int num3 = 0; num3 != this.claRunesList.Count; num3++)
				{
					this.claRunesList[num3].gameObject.SetActive(this.claRunesList[num3].Runesvariety == rv);
				}
			}
			else
			{
				for (int num4 = 0; num4 != this.allRunesList.Count; num4++)
				{
					this.allRunesList[num4].gameObject.SetActive(this.allRunesList[num4].Runesvariety == rv);
				}
			}
		}

		private void RePosition()
		{
			this.gridFilterArea.Reposition();
			this.runesFilterView.ResetPosition();
			this.runesFilterView.considerInactive = false;
			this.runesFilterView.mCalculatedBounds = false;
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
	}
}
