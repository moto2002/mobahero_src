using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleSettlement
{
	public class Settlement_PVE : MonoBehaviour
	{
		private bool initFlag;

		private Transform[] mStars;

		private Transform[] mConditions;

		private GameObject Fx_ui_winstar;

		private bool isUserNeverDied;

		private bool isUserGetFirstBlood;

		private CoroutineManager cMgr;

		private void Awake()
		{
			if (!this.initFlag)
			{
				this.Init();
			}
			MobaMessageManager.RegistMessage((ClientMsg)23031, new MobaMessageFunc(this.onMsg_showPve));
		}

		private void OnDestroy()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23031, new MobaMessageFunc(this.onMsg_showPve));
		}

		private void Init()
		{
			this.mStars = new Transform[]
			{
				base.transform.Find("Ribbon/starShadow0"),
				base.transform.Find("Ribbon/starShadow1"),
				base.transform.Find("Ribbon/starShadow2")
			};
			this.mConditions = new Transform[]
			{
				base.transform.Find("LevelCondition/condItem0"),
				base.transform.Find("LevelCondition/condItem1"),
				base.transform.Find("LevelCondition/condItem2")
			};
			this.cMgr = new CoroutineManager();
			this.Fx_ui_winstar = (Resources.Load("Prefab/Effects/UIEffect/Fx_ui_winstar") as GameObject);
			this.mConditions[0].FindChild("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_ThreeStarCondition1");
			this.mConditions[0].FindChild("Label_hl").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_ThreeStarCondition1");
			this.mConditions[1].FindChild("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_ThreeStarCondition2");
			this.mConditions[1].FindChild("Label_hl").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_ThreeStarCondition2");
			this.mConditions[2].FindChild("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_ThreeStarCondition3");
			this.mConditions[2].FindChild("Label_hl").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_ThreeStarCondition3");
			this.initFlag = true;
		}

		private void onMsg_showPve(MobaMessage msg)
		{
			this.isUserGetFirstBlood = GameManager.Instance.StarManager.isPlayerGetFirstBlood();
			this.isUserNeverDied = GameManager.Instance.StarManager.isPlayerNeverDied();
			this.cMgr.StartCoroutine(this.Play(), true);
		}

		private string GetSpriteName(bool isTrue)
		{
			return (!isTrue) ? "Settlement_icons_x" : "Settlement_icons_right";
		}

		[DebuggerHidden]
		private IEnumerator Play()
		{
			Settlement_PVE.<Play>c__Iterator10C <Play>c__Iterator10C = new Settlement_PVE.<Play>c__Iterator10C();
			<Play>c__Iterator10C.<>f__this = this;
			return <Play>c__Iterator10C;
		}
	}
}
