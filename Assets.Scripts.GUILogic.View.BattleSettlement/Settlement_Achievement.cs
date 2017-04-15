using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleSettlement
{
	public class Settlement_Achievement : MonoBehaviour
	{
		public enum EAchievementMedalType
		{
			MVP = 1001,
			Legendray = 2001,
			DoubleKill = 3001,
			TripleKill,
			QuadraKill,
			PentaKill
		}

		protected GameObject mSliderTipLabel;

		protected UIPanel mPanel;

		protected UIScrollView mScrollView;

		protected UIGrid mGrid;

		protected UICenterOnChild mCenterOnChild;

		protected UIWrapContent mWrapContent;

		protected UILabel mMedalName;

		protected UILabel mMedalDesc;

		protected UITexture mShine;

		private Transform mObjectCache;

		private List<Transform> transList = new List<Transform>();

		private CoroutineManager cMgr = new CoroutineManager();

		private List<UITexture> loadedTextures = new List<UITexture>();

		private HomeKDAData homeKdaData;

		private void Awake()
		{
			this.mSliderTipLabel = base.transform.FindChild("SlideTip").gameObject;
			this.mPanel = base.transform.FindChild("Scroll View").GetComponent<UIPanel>();
			this.mScrollView = this.mPanel.GetComponent<UIScrollView>();
			this.mGrid = base.transform.FindChild("Scroll View/Grid").GetComponent<UIGrid>();
			this.mCenterOnChild = this.mGrid.GetComponent<UICenterOnChild>();
			this.mWrapContent = this.mGrid.GetComponent<UIWrapContent>();
			this.mMedalName = base.transform.FindChild("AchiName").GetComponent<UILabel>();
			this.mMedalDesc = base.transform.FindChild("AchiDesc").GetComponent<UILabel>();
			this.mShine = base.transform.FindChild("shine").GetComponent<UITexture>();
			this.mPanel.alpha = 0.01f;
			this.mObjectCache = base.transform.Find("Container");
			this.mCenterOnChild.onFinished = new SpringPanel.OnFinished(this.ResetTrans);
			MobaMessageManager.RegistMessage((ClientMsg)23037, new MobaMessageFunc(this.onMsg_showAchievement));
		}

		private void OnDestroy()
		{
			this.cMgr.StopAllCoroutine();
			for (int i = 0; i < this.loadedTextures.Count; i++)
			{
				HomeGCManager.Instance.ClearUiTextureResImmediate(this.loadedTextures[i]);
			}
			MobaMessageManager.UnRegistMessage((ClientMsg)23037, new MobaMessageFunc(this.onMsg_showAchievement));
		}

		private void onMsg_showAchievement(MobaMessage msg)
		{
			List<Settlement_Achievement.EAchievementMedalType> list = (List<Settlement_Achievement.EAchievementMedalType>)msg.Param;
			if (list.Count == 0)
			{
				this.cMgr.StartCoroutine(this.ForceContinue(), true);
			}
			else
			{
				base.transform.GetComponent<UIWidget>().alpha = 1f;
				this.mPanel.depth = Singleton<BattleSettlementView>.Instance.transform.GetComponent<UIPanel>().depth + 1;
				this.homeKdaData = ModelManager.Instance.Get_HomeKDAData();
				this.FilterAndInitialize(list);
			}
		}

		private int AchiVo_SortByPriority(SysSummonersAchievementVo a, SysSummonersAchievementVo b)
		{
			return b.priority.CompareTo(a.priority);
		}

		private bool FilterAndInitialize(List<Settlement_Achievement.EAchievementMedalType> list)
		{
			Dictionary<int, SysSummonersAchievementVo> dictionary = new Dictionary<int, SysSummonersAchievementVo>();
			foreach (Settlement_Achievement.EAchievementMedalType current in list)
			{
				int num = (int)current;
				SysSummonersAchievementVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersAchievementVo>(num.ToString());
				if (dataById != null)
				{
					if (dictionary.ContainsKey(dataById.type))
					{
						if (dataById.priority > dictionary[dataById.type].priority)
						{
							dictionary[dataById.type] = dataById;
						}
					}
					else
					{
						dictionary.Add(dataById.type, dataById);
					}
				}
			}
			if (dictionary.Count == 0)
			{
				return false;
			}
			List<SysSummonersAchievementVo> list2 = dictionary.Values.ToList<SysSummonersAchievementVo>();
			if (list2.Count > 1)
			{
				list2.Sort(new Comparison<SysSummonersAchievementVo>(this.AchiVo_SortByPriority));
			}
			if (list2.Count >= 3)
			{
				SysSummonersAchievementVo[] array = list2.ToArray();
				SysSummonersAchievementVo sysSummonersAchievementVo = array[1];
				array[1] = array[0];
				array[0] = sysSummonersAchievementVo;
				list2 = array.ToList<SysSummonersAchievementVo>();
			}
			this.AddMedals(list2);
			return true;
		}

		private void ResetTrans()
		{
			this.SetDescribe();
		}

		private void SetDescribe()
		{
			string name = this.mCenterOnChild.centeredObject.name;
			SysSummonersAchievementVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersAchievementVo>(name);
			int num = 0;
			this.mMedalName.text = LanguageManager.Instance.GetStringById(dataById.name);
			PvpTeamInfo pvpTeamInfo = ModelManager.Instance.Get_Settle_PvpTeamInfo();
			string userId = ModelManager.Instance.Get_userData_X().UserId;
			PlayerCounter playerCounter = null;
			pvpTeamInfo.unitCounters.TryGetValue(userId, out playerCounter);
			int id = dataById.id;
			switch (id)
			{
			case 3001:
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_default", true, true, null, 0, false);
				if (this.homeKdaData != null)
				{
					num = ((playerCounter == null) ? 1 : playerCounter.extKillCount[2]);
					num += this.homeKdaData.doublekill;
				}
				break;
			case 3002:
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_purple", true, true, null, 0, false);
				if (this.homeKdaData != null)
				{
					num = ((playerCounter == null) ? 1 : playerCounter.extKillCount[3]);
					num += this.homeKdaData.triplekill;
				}
				break;
			case 3003:
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_gold", true, true, null, 0, false);
				if (this.homeKdaData != null)
				{
					num = ((playerCounter == null) ? 1 : playerCounter.extKillCount[4]);
					num += this.homeKdaData.quadrakill;
				}
				break;
			case 3004:
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_gold", true, true, null, 0, false);
				if (this.homeKdaData != null)
				{
					num = ((playerCounter == null) ? 1 : playerCounter.extKillCount[5]);
					num += this.homeKdaData.pentakill;
				}
				break;
			default:
				if (id != 1001)
				{
					if (id != 2001)
					{
						this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_default", true, true, null, 0, false);
					}
					else
					{
						this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_purple", true, true, null, 0, false);
						if (this.homeKdaData != null)
						{
							num = ((playerCounter == null) ? 1 : playerCounter.extKillCount[104]);
							num += this.homeKdaData.godlike;
						}
					}
				}
				else
				{
					this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_red", true, true, null, 0, false);
					if (this.homeKdaData != null)
					{
						num = 1 + this.homeKdaData.mvp;
					}
				}
				break;
			}
			this.mMedalDesc.text = ((num > 0) ? LanguageManager.Instance.GetStringById(dataById.describe).Replace("*", num.ToString()) : LanguageManager.Instance.GetStringById(dataById.describe));
		}

		[DebuggerHidden]
		private IEnumerator Reposition()
		{
			Settlement_Achievement.<Reposition>c__Iterator102 <Reposition>c__Iterator = new Settlement_Achievement.<Reposition>c__Iterator102();
			<Reposition>c__Iterator.<>f__this = this;
			return <Reposition>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator ForceContinue()
		{
			return new Settlement_Achievement.<ForceContinue>c__Iterator103();
		}

		private void AddMedals(List<SysSummonersAchievementVo> medalList)
		{
			if (medalList != null && medalList.Count > 0)
			{
				for (int i = 0; i < medalList.Count; i++)
				{
					GameObject gameObject = NGUITools.AddChild(this.mGrid.gameObject, this.mObjectCache.gameObject);
					gameObject.SetActive(true);
					NGUITools.AddChild(gameObject, ResourceManager.Load<GameObject>(medalList[i].prefab, true, true, null, 0, false));
					this.loadedTextures.Add(gameObject.GetComponentInChildren<UITexture>());
					gameObject.name = medalList[i].id.ToString();
					this.transList.Add(gameObject.transform);
				}
				this.cMgr.StartCoroutine(this.Reposition(), true);
				this.mSliderTipLabel.SetActive(medalList.Count > 1);
			}
		}
	}
}
