using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleSettlement
{
	public class Settlement_Rank : MonoBehaviour
	{
		private bool initFlag;

		private GameObject Fx_Ranking_icon;

		private UILabel mPoint;

		private UILabel mLevelName;

		private UILabel mNextLevelName;

		private UISprite mBar;

		private UILabel mDeltaScore;

		private UITexture mShine;

		private double preLadderScore;

		private double curLadderScore;

		private CoroutineManager cMgr = new CoroutineManager();

		private void Awake()
		{
			if (!this.initFlag)
			{
				this.Init();
			}
			if (ModelManager.Instance.Get_SettlementModelData().pvp_teamInfo != null)
			{
				this.preLadderScore = ModelManager.Instance.Get_SettlementModelData().pvp_teamInfo.CurrLadderScore;
			}
			else
			{
				this.preLadderScore = ModelManager.Instance.Get_userData_X().LadderScore;
			}
			if (this.preLadderScore == 0.0)
			{
				this.preLadderScore = Singleton<PvpManager>.Instance.BattleResult.CurrLadderScore;
			}
			MobaMessageManager.RegistMessage((ClientMsg)23035, new MobaMessageFunc(this.onMsg_show));
		}

		private void OnDestroy()
		{
			this.cMgr.StopAllCoroutine();
			MobaMessageManager.UnRegistMessage((ClientMsg)23035, new MobaMessageFunc(this.onMsg_show));
			HomeGCManager.Instance.ClearUiTextureResImmediate(base.transform.FindChild("Fx_Ranking_icon/Ranking/GoldenFrame").GetComponent<UITexture>());
		}

		private void Init()
		{
			this.initFlag = true;
			this.Fx_Ranking_icon = base.transform.FindChild("Fx_Ranking_icon").gameObject;
			this.mPoint = base.transform.FindChild("RankCenter/Point").GetComponent<UILabel>();
			this.mLevelName = base.transform.FindChild("RankCenter/Level").GetComponent<UILabel>();
			this.mNextLevelName = base.transform.FindChild("NextStageLabel").GetComponent<UILabel>();
			this.mBar = base.transform.FindChild("RankCenter/ProgBarBg/ProgBarFore").GetComponent<UISprite>();
			this.mDeltaScore = base.transform.FindChild("Delta/Label").GetComponent<UILabel>();
			this.mShine = this.Fx_Ranking_icon.transform.FindChild("Ranking/shine").GetComponent<UITexture>();
			base.transform.FindChild("NextStageTipLabel").GetComponent<UILabel>().text = "- " + LanguageManager.Instance.GetStringById("RankUI_Settlement_NextStage") + " -";
		}

		private void onMsg_show(MobaMessage msg)
		{
			this.preLadderScore = Singleton<PvpManager>.Instance.RoomInfo.OldLadderScore;
			this.curLadderScore = ModelManager.Instance.Get_userData_X().LadderScore;
			if (LevelManager.m_CurLevel.IsRank())
			{
				this.mPoint.text = this.curLadderScore.ToString();
			}
			double num = this.curLadderScore - this.preLadderScore;
			if (num >= 0.0)
			{
				this.mDeltaScore.text = "+" + num.ToString("F0");
			}
			else
			{
				this.mDeltaScore.text = num.ToString("F0");
			}
			this.SetLadderData();
			this.Fx_Ranking_icon.GetComponent<EffectPlayTool>().Play();
		}

		private void SetLadderData()
		{
			SysRankStageVo sysRankStageVo = ModelManager.Instance.Get_LadderLevel();
			UITexture component = this.Fx_Ranking_icon.transform.FindChild("Ranking/GoldenFrame").GetComponent<UITexture>();
			component.mainTexture = ResourceManager.Load<Texture>(sysRankStageVo.StageImg, true, true, null, 0, false);
			this.mLevelName.text = LanguageManager.Instance.GetStringById(sysRankStageVo.StageName);
			this.mLevelName.gradientTop = ModelManager.Instance.Get_ColorByString_X(sysRankStageVo.GradientTop);
			this.mLevelName.gradientBottom = ModelManager.Instance.Get_ColorByString_X(sysRankStageVo.GradientBottom);
			RankIconEffectPlayerTools component2 = component.transform.GetComponent<RankIconEffectPlayerTools>();
			component2.RankLevel = sysRankStageVo.RankStage;
			component2.SortPanel = Singleton<BattleSettlementView>.Instance.transform.GetComponent<UIPanel>();
			component2.SortWidget = component;
			component2.SetEffectActive(true, 0f);
			component2.SetScale(component.width);
			int num = Convert.ToInt32(this.curLadderScore) - sysRankStageVo.StageScore;
			switch (sysRankStageVo.RankStage)
			{
			case 1:
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_default", true, true, null, 0, false);
				break;
			case 2:
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_purple", true, true, null, 0, false);
				break;
			case 3:
			case 4:
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_gold", true, true, null, 0, false);
				break;
			case 5:
			case 6:
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_red", true, true, null, 0, false);
				break;
			default:
				this.mShine.mainTexture = ResourceManager.Load<Texture>("Light_default", true, true, null, 0, false);
				break;
			}
			SysRankStageVo sysRankStageVo2 = ModelManager.Instance.Get_NextLadderLevel();
			if (sysRankStageVo2 == null)
			{
				base.transform.FindChild("NextStageTipLabel").gameObject.SetActive(false);
				this.mNextLevelName.gameObject.SetActive(false);
				this.cMgr.StartCoroutine(this.TweenPlay(this.mBar, 1f, this.mPoint, Convert.ToInt32(this.curLadderScore)), true);
				return;
			}
			if (sysRankStageVo2.StageRanking > 0)
			{
				this.mNextLevelName.text = string.Format("{0}(积分达到{1}分且排位排名在{2}名以内)", LanguageManager.Instance.GetStringById(sysRankStageVo2.StageName), sysRankStageVo2.StageScore.ToString(), sysRankStageVo2.StageRanking.ToString());
			}
			else
			{
				this.mNextLevelName.text = string.Format("{0}({1}')", LanguageManager.Instance.GetStringById(sysRankStageVo2.StageName), sysRankStageVo2.StageScore.ToString());
			}
			int num2 = sysRankStageVo2.StageScore - sysRankStageVo.StageScore;
			if (num2 == 0)
			{
				num2 = 1;
			}
			this.cMgr.StartCoroutine(this.TweenPlay(this.mBar, (float)num / (float)num2, this.mPoint, Convert.ToInt32(this.curLadderScore)), true);
		}

		[DebuggerHidden]
		private IEnumerator TweenPlay(UISprite _bar, float _finalPercent, UILabel _num, int _finalNum)
		{
			Settlement_Rank.<TweenPlay>c__Iterator10D <TweenPlay>c__Iterator10D = new Settlement_Rank.<TweenPlay>c__Iterator10D();
			<TweenPlay>c__Iterator10D._finalPercent = _finalPercent;
			<TweenPlay>c__Iterator10D._bar = _bar;
			<TweenPlay>c__Iterator10D._finalNum = _finalNum;
			<TweenPlay>c__Iterator10D._num = _num;
			<TweenPlay>c__Iterator10D.<$>_finalPercent = _finalPercent;
			<TweenPlay>c__Iterator10D.<$>_bar = _bar;
			<TweenPlay>c__Iterator10D.<$>_finalNum = _finalNum;
			<TweenPlay>c__Iterator10D.<$>_num = _num;
			return <TweenPlay>c__Iterator10D;
		}
	}
}
