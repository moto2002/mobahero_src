using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.Runes
{
	public class RunesBottom : MonoBehaviour
	{
		private Transform transOverview;

		private Transform transConfirm;

		private Transform btnCoalesceAll;

		private Transform btnConfirm;

		private Transform closeBG;

		private Transform confirmBG;

		private Transform coalesceBG;

		private UILabel countCompound;

		private UILabel countCost;

		private UIScrollView scrollview;

		private UIGrid gridInfo;

		private Color lackC = new Color(0.996078432f, 0.6784314f, 0f, 1f);

		private RunesCoalesceInfo runescoalesceinfo;

		private List<EquipmentInfoData> originList = new List<EquipmentInfoData>();

		private List<EquipmentInfoData> primaryList = new List<EquipmentInfoData>();

		private List<EquipmentInfoData> middleList = new List<EquipmentInfoData>();

		private List<EquipmentInfoData> finalmiddleList = new List<EquipmentInfoData>();

		private List<EquipmentInfoData> finalhighList = new List<EquipmentInfoData>();

		private Coroutine coroutine;

		private object[] mgs;

		private int cost1to2;

		private int cost2to3;

		private int costMoney;

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.runesviewChangeToggle,
				ClientV2C.runesviewOnCompoundAll,
				ClientV2C.runesviewCheckRune,
				ClientV2C.coalesceviewAfterCoalesce,
				ClientV2C.coalesceviewAfterPurchase
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
			this.transOverview = base.transform.Find("BeforeCoalesce");
			this.transConfirm = base.transform.Find("ConfirmCoalesce");
			this.closeBG = this.transConfirm.Find("CloseBG");
			this.coalesceBG = this.transOverview.Find("BG");
			this.confirmBG = this.transOverview.Find("TitleBG");
			this.btnCoalesceAll = this.transOverview.Find("CoalesceAllBtn/Sprite");
			this.btnConfirm = this.transOverview.Find("ConfirmBtn/Sprite");
			this.countCompound = this.transOverview.Find("Coalesce/Count").GetComponent<UILabel>();
			this.countCost = this.transOverview.Find("Cost/Count").GetComponent<UILabel>();
			this.scrollview = this.transConfirm.Find("CheckView").GetComponent<UIScrollView>();
			this.gridInfo = this.transConfirm.Find("CheckView/Grid").GetComponent<UIGrid>();
			this.runescoalesceinfo = Resources.Load<RunesCoalesceInfo>("Prefab/UI/Sacrificial/RunesAchievementHint");
			UIEventListener.Get(this.btnCoalesceAll.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickCoalesce);
			UIEventListener.Get(this.btnConfirm.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickConfirm);
			UIEventListener.Get(this.closeBG.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickCloseBG);
		}

		private void OnMsg_runesviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				RunesFunctionType runesFunctionType = (RunesFunctionType)((int)msg.Param);
				this.btnCoalesceAll.parent.gameObject.SetActive(true);
				this.transConfirm.gameObject.SetActive(false);
				this.btnConfirm.parent.gameObject.SetActive(false);
				this.confirmBG.gameObject.SetActive(false);
				this.coalesceBG.gameObject.SetActive(true);
				if (runesFunctionType == RunesFunctionType.Storage)
				{
					this.SetBtnState();
					if (this.coroutine != null)
					{
						base.StopCoroutine("SyncGetData");
					}
					this.coroutine = base.StartCoroutine("SyncGetData");
				}
			}
		}

		private void OnMsg_runesviewCheckRune(MobaMessage msg)
		{
			if (msg != null)
			{
				this.GetData();
			}
		}

		private void OnMsg_runesviewOnCompoundAll(MobaMessage msg)
		{
			if (msg != null)
			{
				CtrlManager.OpenWindow(WindowID.GetItemsView, null);
				for (int i = 0; i < this.finalmiddleList.Count; i++)
				{
					this.PlayAnime(this.finalmiddleList[i].ModelId);
				}
				for (int j = 0; j < this.finalhighList.Count; j++)
				{
					this.PlayAnime(this.finalhighList[j].ModelId);
				}
				GetItemsView expr_79 = Singleton<GetItemsView>.Instance;
				expr_79.onFinish = (Callback)Delegate.Combine(expr_79.onFinish, new Callback(this.RefreshDataUI));
				Singleton<GetItemsView>.Instance.Play();
			}
		}

		private void OnMsg_coalesceviewAfterCoalesce(MobaMessage msg)
		{
			if (msg != null)
			{
				this.GetData();
			}
		}

		private void OnMsg_coalesceviewAfterPurchase(MobaMessage msg)
		{
			if (msg != null)
			{
				this.GetData();
			}
		}

		private void PlayAnime(int modelid)
		{
			MobaMessageManagerTools.GetItems_Rune(modelid);
		}

		private void RefreshDataUI()
		{
			this.finalmiddleList.Clear();
			this.finalhighList.Clear();
			this.SetBtnState();
			this.confirmBG.gameObject.SetActive(false);
			this.coalesceBG.gameObject.SetActive(true);
			Singleton<MenuTopBarView>.Instance.RefreshUI();
			RunesCtrl.GetInstance().runestate = RunesInlayState.Nothing;
		}

		private void SetBtnState()
		{
			this.transConfirm.gameObject.SetActive(false);
			this.btnCoalesceAll.parent.gameObject.SetActive(true);
			this.btnConfirm.parent.gameObject.SetActive(false);
			this.countCompound.text = "x0";
			this.countCost.text = "0";
			this.btnCoalesceAll.GetComponent<UISprite>().alpha = 0.4862745f;
			this.btnCoalesceAll.GetComponent<UIButton>().defaultColor = new Color(255f, 255f, 255f, 0.4862745f);
			this.btnCoalesceAll.GetComponent<UIButton>().hover = new Color(255f, 255f, 255f, 0.4862745f);
			this.btnCoalesceAll.GetComponent<UIButton>().pressed = new Color(255f, 255f, 255f, 0.4862745f);
		}

		private void GetData()
		{
			this.originList.Clear();
			this.finalmiddleList.Clear();
			this.finalhighList.Clear();
			this.primaryList.Clear();
			this.middleList.Clear();
			List<EquipmentInfoData> list = ModelManager.Instance.Get_equipmentList_X();
			Dictionary<string, SysGameItemsVo> gameItems = BaseDataMgr.instance.GetTypeDicByType<SysGameItemsVo>();
			if (gameItems == null)
			{
				return;
			}
			for (int num = 0; num != list.Count; num++)
			{
				if (list[num] != null)
				{
					string key = list[num].ModelId.ToString();
					if (gameItems.ContainsKey(key) && gameItems[key].type == 4)
					{
						this.originList.Add(list[num]);
					}
				}
			}
			EquipmentInfoData equipmentInfoData = this.originList.Find((EquipmentInfoData obj) => 1 == gameItems[obj.ModelId.ToString()].quality);
			if (this.cost1to2 == 0 && equipmentInfoData != null)
			{
				this.cost1to2 = gameItems[this.originList.Find((EquipmentInfoData obj) => 1 == gameItems[obj.ModelId.ToString()].quality).ModelId.ToString()].consumption_money;
			}
			if (this.cost2to3 == 0)
			{
				equipmentInfoData = this.originList.Find((EquipmentInfoData obj) => 2 == gameItems[obj.ModelId.ToString()].quality);
				if (equipmentInfoData != null)
				{
					this.cost2to3 = gameItems[this.originList.Find((EquipmentInfoData obj) => 2 == gameItems[obj.ModelId.ToString()].quality).ModelId.ToString()].consumption_money;
				}
			}
			int num2 = 0;
			bool flag = this.originList != null && this.originList.Count != 0 && int.TryParse(gameItems[this.originList[0].ModelId.ToString()].consumption, out num2);
			this.primaryList.AddRange(this.originList.FindAll((EquipmentInfoData obj) => obj.Count >= 3 && 1 == gameItems[obj.ModelId.ToString()].quality));
			this.middleList.AddRange(this.originList.FindAll((EquipmentInfoData obj) => 2 == gameItems[obj.ModelId.ToString()].quality));
			this.SetData((!flag) ? 3 : num2);
		}

		private void SetData(int consumption)
		{
			this.costMoney = 0;
			if (this.primaryList != null && this.primaryList.Count != 0)
			{
				for (int l = 0; l < this.primaryList.Count; l++)
				{
					this.ParseData(this.primaryList[l], consumption);
				}
			}
			List<EquipmentInfoData> list = new List<EquipmentInfoData>();
			int m;
			for (m = 0; m < this.finalmiddleList.Count; m++)
			{
				EquipmentInfoData equipmentInfoData = this.middleList.Find((EquipmentInfoData obj) => obj.ModelId == this.finalmiddleList[m].ModelId);
				if (equipmentInfoData != null)
				{
					this.finalmiddleList[m].Count += equipmentInfoData.Count;
					list.Add(equipmentInfoData);
				}
			}
			List<EquipmentInfoData> list2 = new List<EquipmentInfoData>();
			list2 = this.middleList.Except(list).ToList<EquipmentInfoData>();
			list2 = this.middleList.FindAll((EquipmentInfoData obj) => obj.Count >= 3);
			for (int j = 0; j < list2.Count; j++)
			{
				EquipmentInfoData item = new EquipmentInfoData
				{
					Count = list2[j].Count,
					EquipmentId = 0L,
					ModelId = list2[j].ModelId
				};
				this.finalmiddleList.Add(item);
			}
			if (this.finalmiddleList != null && this.finalmiddleList.Count != 0)
			{
				for (int k = 0; k < this.finalmiddleList.Count; k++)
				{
					this.ParseData(this.finalmiddleList[k], consumption, true);
				}
			}
			this.finalmiddleList = this.finalmiddleList.FindAll((EquipmentInfoData obj) => obj.Count > 0);
			int i;
			for (i = 0; i < this.middleList.Count; i++)
			{
				EquipmentInfoData equipmentInfoData2 = this.finalmiddleList.Find((EquipmentInfoData obj) => obj.ModelId == this.middleList[i].ModelId);
				if (equipmentInfoData2 != null && equipmentInfoData2.Count <= this.middleList[i].Count)
				{
					this.finalmiddleList.Remove(equipmentInfoData2);
				}
				else if (equipmentInfoData2 != null && equipmentInfoData2.Count > this.middleList[i].Count)
				{
					equipmentInfoData2.Count -= this.middleList[i].Count;
				}
			}
			this.SetDisplay();
		}

		private void SetDisplay()
		{
			List<EquipmentInfoData> finalList = new List<EquipmentInfoData>();
			finalList.AddRange(this.finalmiddleList);
			finalList.AddRange(this.finalhighList);
			if (finalList.Count > 0)
			{
				this.btnCoalesceAll.GetComponent<UISprite>().alpha = 1f;
				this.btnCoalesceAll.GetComponent<UIButton>().defaultColor = new Color(255f, 255f, 255f, 1f);
				this.btnCoalesceAll.GetComponent<UIButton>().hover = new Color(255f, 255f, 255f, 1f);
				this.btnCoalesceAll.GetComponent<UIButton>().pressed = new Color(255f, 255f, 255f, 1f);
				if (finalList.Count < this.gridInfo.transform.childCount)
				{
					int num = this.gridInfo.transform.childCount - 1;
					while (finalList.Count != this.gridInfo.transform.childCount)
					{
						UnityEngine.Object.DestroyImmediate(this.gridInfo.transform.GetChild(num).gameObject);
						num--;
					}
				}
				GridHelper.FillGrid<RunesCoalesceInfo>(this.gridInfo, this.runescoalesceinfo, finalList.Count, delegate(int idx, RunesCoalesceInfo comp)
				{
					comp.Init(finalList[idx].ModelId, finalList[idx].Count);
					comp.name = finalList[idx].ModelId.ToString();
				});
				List<RunesCoalesceInfo> list = new List<RunesCoalesceInfo>();
				for (int num2 = 0; num2 != this.gridInfo.transform.childCount; num2++)
				{
					list.Add(this.gridInfo.transform.GetChild(num2).GetComponent<RunesCoalesceInfo>());
				}
				list.Sort(new Comparison<RunesCoalesceInfo>(this.SortByCustom));
				if (this.gridInfo.transform.childCount == list.Count)
				{
					for (int num3 = 0; num3 != list.Count; num3++)
					{
						RunesCoalesceInfo runesCoalesceInfo = list[num3];
						for (int num4 = 0; num4 != this.gridInfo.transform.childCount; num4++)
						{
							if (this.gridInfo.transform.GetChild(num4).name.CompareTo(runesCoalesceInfo.name) == 0)
							{
								this.gridInfo.transform.GetChild(num4).name = string.Format("{0:D3}", num3);
								break;
							}
						}
					}
				}
				this.gridInfo.Reposition();
				int num5 = 0;
				for (int i = 0; i < this.finalhighList.Count; i++)
				{
					num5 += this.finalhighList[i].Count;
				}
				this.countCompound.text = "x" + num5.ToString();
				this.countCost.text = this.costMoney.ToString();
				this.countCost.color = (((long)this.costMoney <= ModelManager.Instance.Get_userData_X().Money) ? this.lackC : Color.red);
			}
			else
			{
				this.SetBtnState();
			}
		}

		private void SetAnime()
		{
			List<EquipmentInfoData> list = new List<EquipmentInfoData>();
			list.AddRange(this.finalmiddleList);
			list.AddRange(this.finalhighList);
			if (list.Count > 0)
			{
				CtrlManager.OpenWindow(WindowID.GetItemsView, null);
				for (int i = 0; i < list.Count; i++)
				{
					MobaMessageManagerTools.GetItems_Rune(list[i].ModelId);
				}
				Singleton<GetItemsView>.Instance.Play();
			}
		}

		private void ParseData(EquipmentInfoData eid, int consumption)
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(eid.ModelId.ToString());
			SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(dataById.synthetic_id);
			int count = eid.Count;
			int num = count / consumption;
			int num2 = count % consumption;
			if (num > 0)
			{
				int modelId = int.Parse(dataById.synthetic_id);
				EquipmentInfoData item = new EquipmentInfoData
				{
					Count = num,
					EquipmentId = 0L,
					ModelId = modelId
				};
				this.finalmiddleList.Add(item);
				int num3 = this.cost1to2 * num;
				this.costMoney += num3;
			}
		}

		private void ParseData(EquipmentInfoData eid, int consumption, bool is2to3)
		{
			if (!is2to3)
			{
				return;
			}
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(eid.ModelId.ToString());
			int count = eid.Count;
			int num = count / consumption;
			int count2 = count % consumption;
			if (num > 0)
			{
				int num2 = int.Parse(dataById.synthetic_id);
				if (this.finalhighList != null && this.finalhighList.Count > 0)
				{
					for (int i = 0; i < this.finalhighList.Count; i++)
					{
						if (this.finalhighList[i].ModelId == num2)
						{
							this.finalhighList[i].Count += num;
							break;
						}
					}
				}
				EquipmentInfoData item = new EquipmentInfoData
				{
					Count = num,
					EquipmentId = 0L,
					ModelId = num2
				};
				this.finalhighList.Add(item);
				int num3 = this.cost2to3 * num;
				this.costMoney += num3;
				eid.Count = count2;
			}
		}

		private void ClickCoalesce(GameObject obj)
		{
			if (null != obj)
			{
				if (this.finalmiddleList.Count == 0 && this.finalhighList.Count == 0)
				{
					Singleton<TipView>.Instance.ShowViewSetText("没有可以合成的符文", 1f);
					return;
				}
				UserData userData = ModelManager.Instance.Get_userData_X();
				if ((long)this.costMoney > userData.Money)
				{
					Singleton<TipView>.Instance.ShowViewSetText("金钱不足", 1f);
					return;
				}
				this.transConfirm.gameObject.SetActive(true);
				this.btnConfirm.parent.gameObject.SetActive(true);
				this.btnCoalesceAll.parent.gameObject.SetActive(false);
				this.confirmBG.gameObject.SetActive(true);
				this.coalesceBG.gameObject.SetActive(false);
				this.scrollview.ResetPosition();
				this.gridInfo.Reposition();
			}
		}

		private void ClickConfirm(GameObject obj)
		{
			if (null != obj)
			{
				if (RunesCtrl.GetInstance().runestate == RunesInlayState.Doing)
				{
					return;
				}
				RunesCtrl.GetInstance().runestate = RunesInlayState.Doing;
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在合成...", false, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.OneKeyCompose, param, new object[0]);
			}
		}

		private void ClickCloseBG(GameObject obj)
		{
			if (null != obj)
			{
				this.transConfirm.gameObject.SetActive(false);
				this.btnCoalesceAll.parent.gameObject.SetActive(true);
				this.btnConfirm.parent.gameObject.SetActive(false);
				this.confirmBG.gameObject.SetActive(false);
				this.coalesceBG.gameObject.SetActive(true);
			}
		}

		private int SortByCustom(RunesCoalesceInfo a, RunesCoalesceInfo b)
		{
			if (!(null != a) || !(null != b))
			{
				return 0;
			}
			if (a.Quality > b.Quality)
			{
				return -1;
			}
			if (a.Quality < b.Quality)
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

		[DebuggerHidden]
		private IEnumerator SyncGetData()
		{
			RunesBottom.<SyncGetData>c__Iterator185 <SyncGetData>c__Iterator = new RunesBottom.<SyncGetData>c__Iterator185();
			<SyncGetData>c__Iterator.<>f__this = this;
			return <SyncGetData>c__Iterator;
		}
	}
}
