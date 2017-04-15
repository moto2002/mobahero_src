using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleSettlement
{
	public class Settlement_Summoner : MonoBehaviour
	{
		private bool initFlag;

		private MeshRenderer mSummonerIcon;

		private UICircleProgressBar expBar;

		private UILabel mCurLevel;

		private UILabel mCurLevelTemp;

		private UILabel mGainExpLabel;

		private Transform mAboveEffPanel;

		private CoroutineManager cMgr = new CoroutineManager();

		private int gainExp;

		private GameObject Fx_shuzi;

		private GameObject Fx_playerlvuptx;

		private int currentExp;

		private int requiredExp;

		private int level;

		private static int levelUpCount;

		private void Awake()
		{
			if (!this.initFlag)
			{
				this.Init();
			}
			Settlement_Summoner.levelUpCount = 0;
			this.Fx_shuzi = (Resources.Load("Prefab/Effects/UIEffect/Fx_shuzi") as GameObject);
			this.Fx_playerlvuptx = (Resources.Load("Prefab/Effects/UIEffect/Fx_playerlvuptx") as GameObject);
			MobaMessageManager.RegistMessage((ClientMsg)23030, new MobaMessageFunc(this.onMsg_show));
		}

		private void OnDestroy()
		{
			this.cMgr.StopAllCoroutine();
			this.Fx_shuzi = null;
			this.Fx_playerlvuptx = null;
			MobaMessageManager.UnRegistMessage((ClientMsg)23030, new MobaMessageFunc(this.onMsg_show));
		}

		private void OnDisable()
		{
			this.expBar.CoroutineClear();
		}

		private void Init()
		{
			this.mSummonerIcon = base.transform.Find("BelowEffPanel/Fx_playerlvup/zhong/Sprite").GetComponent<MeshRenderer>();
			this.expBar = base.transform.Find("UICircleProgressBar").GetComponent<UICircleProgressBar>();
			this.mCurLevel = base.transform.Find("StaticPanel/Level").GetComponent<UILabel>();
			this.mCurLevelTemp = base.transform.Find("StaticPanel/LevelTemp").GetComponent<UILabel>();
			this.mGainExpLabel = base.transform.Find("StaticPanel/AddNum").GetComponent<UILabel>();
			this.mAboveEffPanel = base.transform.Find("AboveEffPanel");
			SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(ModelManager.Instance.Get_userData_X().Avatar.ToString());
			if (dataById != null)
			{
				this.mSummonerIcon.material.SetTexture("_MainTex", ResourceManager.Load<Texture>(dataById.headportrait_icon, true, true, null, 0, false));
			}
			this.initFlag = true;
		}

		private void onMsg_show(MobaMessage msg)
		{
			this.gainExp = ModelManager.Instance.Get_Settle_SummonerExpDelta();
			this.level = ModelManager.Instance.Get_Settle_SummonerExpLevel();
			this.currentExp = ModelManager.Instance.Get_Settle_SummonerExpCurrent();
			this.requiredExp = ModelManager.Instance.Get_Settle_SummonerExpCurRequired();
			this.mCurLevel.text = this.level.ToString();
			this.Play();
		}

		public void Show(int deltaExp, long fromExp = -1L)
		{
			long exp;
			if (fromExp >= 0L)
			{
				exp = fromExp;
			}
			else
			{
				exp = ModelManager.Instance.Get_userData_X().Exp;
			}
			this.gainExp = deltaExp;
			this.level = CharacterDataMgr.instance.GetUserLevel(exp);
			this.currentExp = CharacterDataMgr.instance.GetUserCurrentExp(exp);
			this.requiredExp = CharacterDataMgr.instance.GetUserNextLevelExp(exp);
			this.mCurLevel.text = this.level.ToString();
			this.Play();
		}

		private void onLevelUp_LevelLabel()
		{
			Settlement_Summoner.levelUpCount++;
			int num = this.level + Settlement_Summoner.levelUpCount;
			this.mCurLevelTemp.text = num.ToString();
			this.mCurLevel.GetComponent<TweenAlpha>().ResetToBeginning();
			this.mCurLevel.GetComponent<TweenPosition>().ResetToBeginning();
			this.mCurLevelTemp.GetComponent<TweenAlpha>().ResetToBeginning();
			this.mCurLevelTemp.GetComponent<TweenPosition>().ResetToBeginning();
			this.mCurLevel.GetComponent<TweenAlpha>().PlayForward();
			this.mCurLevel.GetComponent<TweenPosition>().PlayForward();
			this.mCurLevelTemp.GetComponent<TweenAlpha>().PlayForward();
			this.mCurLevelTemp.GetComponent<TweenPosition>().PlayForward();
			if (num >= 30 || num <= 1)
			{
				return;
			}
			int experience = BaseDataMgr.instance.GetDataById<SysSummonersLevelVo>((num + 1).ToString()).experience;
			int experience2 = BaseDataMgr.instance.GetDataById<SysSummonersLevelVo>(num.ToString()).experience;
			this.expBar.Play(-1f, -1f, (float)(experience - experience2));
			this.mCurLevel.text = (num - 1).ToString();
		}

		private void onLevelUp_TweenScale()
		{
			GameObject gameObject = NGUITools.AddChild(this.mAboveEffPanel.gameObject, this.Fx_playerlvuptx);
			gameObject.transform.GetComponent<RenderQueueMgr>().panel = this.mAboveEffPanel.GetComponent<UIPanel>();
			gameObject.transform.localPosition = -this.mAboveEffPanel.transform.localPosition;
			TweenScale component = base.transform.GetComponent<TweenScale>();
			component.from = new Vector3(1.05f, 1.05f, 1.05f);
			component.ResetToBeginning();
			component.PlayForward();
		}

		public void Play()
		{
			this.cMgr.StartCoroutine(this.PlayBarTween(), true);
		}

		private void RefreshLabel(float factor)
		{
			this.mGainExpLabel.text = string.Format("+{0}", (int)((double)this.gainExp * (0.5 + 0.5 * (double)factor)));
		}

		[DebuggerHidden]
		private IEnumerator VelocityAttenuationCall(int step, float duration, Callback<float> callMethod)
		{
			Settlement_Summoner.<VelocityAttenuationCall>c__Iterator10E <VelocityAttenuationCall>c__Iterator10E = new Settlement_Summoner.<VelocityAttenuationCall>c__Iterator10E();
			<VelocityAttenuationCall>c__Iterator10E.step = step;
			<VelocityAttenuationCall>c__Iterator10E.duration = duration;
			<VelocityAttenuationCall>c__Iterator10E.callMethod = callMethod;
			<VelocityAttenuationCall>c__Iterator10E.<$>step = step;
			<VelocityAttenuationCall>c__Iterator10E.<$>duration = duration;
			<VelocityAttenuationCall>c__Iterator10E.<$>callMethod = callMethod;
			return <VelocityAttenuationCall>c__Iterator10E;
		}

		private float GetOffset(int word)
		{
			return 114f + 11f * (float)word.ToString().Length;
		}

		[DebuggerHidden]
		private IEnumerator PlayBarTween()
		{
			Settlement_Summoner.<PlayBarTween>c__Iterator10F <PlayBarTween>c__Iterator10F = new Settlement_Summoner.<PlayBarTween>c__Iterator10F();
			<PlayBarTween>c__Iterator10F.<>f__this = this;
			return <PlayBarTween>c__Iterator10F;
		}
	}
}
