using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleSettlement
{
	public class Settlement_NormalRewards : MonoBehaviour
	{
		private bool initFlag;

		private Transform mCoin;

		private UILabel mGainCoinLabel;

		private UICircleProgressBar coinBar;

		private Transform mBottle;

		private UILabel mBottleLevel;

		private UILabel mBottleLevelTemp;

		private UILabel mGainBottleExpLabel;

		private UITexture mBottleTexture;

		private UICircleProgressBar bottleBar;

		private Transform mRate;

		private UILabel mCurRateLabel;

		private UILabel mRateArcadeEffLabel;

		private UILabel mProficiencyExpLabel;

		private UICircleProgressBar proficiencyBar;

		private GameObject Fx_jiesuan;

		private EffectPlayTool Fx_bottlelvuptx;

		private CoroutineManager cMgr = new CoroutineManager();

		private int mDeltaCoin_int;

		private int mDeltaBottle_int;

		private int mDeltaRate_int;

		private char mScore_char = 'X';

		private AnimationCurve centerScaleCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0.6318f),
			new Keyframe(1f, 0.75f),
			new Keyframe(2f, 0.8464f),
			new Keyframe(3f, 0.9049f),
			new Keyframe(4f, 0.9417f),
			new Keyframe(5f, 0.968f),
			new Keyframe(6f, 0.975f),
			new Keyframe(7f, 0.9887f),
			new Keyframe(8f, 0.9962f),
			new Keyframe(9f, 0.9987f),
			new Keyframe(10f, 1f)
		});

		private AnimationCurve sideScaleCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0.2f),
			new Keyframe(1f, 0.3405f),
			new Keyframe(2f, 0.4746f),
			new Keyframe(3f, 0.5874f),
			new Keyframe(4f, 0.6732f),
			new Keyframe(5f, 0.7454f),
			new Keyframe(6f, 0.8219f),
			new Keyframe(7f, 0.8613f),
			new Keyframe(8f, 0.9006f),
			new Keyframe(9f, 0.932f),
			new Keyframe(10f, 0.9561f),
			new Keyframe(11f, 0.9728f),
			new Keyframe(12f, 0.9849f),
			new Keyframe(13f, 0.9924f),
			new Keyframe(14f, 0.998f),
			new Keyframe(15f, 1f)
		});

		private static int bottle_levelUpCount;

		private void Awake()
		{
			if (!this.initFlag)
			{
				this.Init();
			}
			Settlement_NormalRewards.bottle_levelUpCount = 0;
			MobaMessageManager.RegistMessage((ClientMsg)23032, new MobaMessageFunc(this.onMsg_showNormal));
		}

		private void OnDestroy()
		{
			this.ClearResources();
			this.cMgr.StopAllCoroutine();
			this.Fx_jiesuan = null;
			this.Fx_bottlelvuptx = null;
			MobaMessageManager.UnRegistMessage((ClientMsg)23032, new MobaMessageFunc(this.onMsg_showNormal));
		}

		public void ClearResources()
		{
			if (this.mBottleTexture != null && this.mBottleTexture.mainTexture != null)
			{
				this.mBottleTexture.mainTexture = null;
			}
		}

		private void OnDisable()
		{
			this.bottleBar.CoroutineClear();
			this.coinBar.CoroutineClear();
			this.proficiencyBar.CoroutineClear();
		}

		private void Init()
		{
			this.mCoin = base.transform.FindChild("Coin");
			this.mBottle = base.transform.FindChild("Bottle");
			this.mRate = base.transform.FindChild("Rate");
			this.mGainCoinLabel = this.mCoin.FindChild("number").GetComponent<UILabel>();
			this.coinBar = this.mCoin.FindChild("UICircleProgressBar").GetComponent<UICircleProgressBar>();
			this.mBottleLevel = this.mBottle.FindChild("BottleLevel").GetComponent<UILabel>();
			this.mBottleLevelTemp = this.mBottle.FindChild("BottleLevelTemp").GetComponent<UILabel>();
			this.mGainBottleExpLabel = this.mBottle.FindChild("number").GetComponent<UILabel>();
			this.mBottleTexture = this.mBottle.FindChild("Texture").GetComponent<UITexture>();
			this.bottleBar = this.mBottle.FindChild("UICircleProgressBar").GetComponent<UICircleProgressBar>();
			this.mCurRateLabel = this.mRate.FindChild("rank/label").GetComponent<UILabel>();
			this.mRateArcadeEffLabel = this.mRate.FindChild("arcadeEff").GetComponent<UILabel>();
			this.mProficiencyExpLabel = this.mRate.FindChild("number").GetComponent<UILabel>();
			this.proficiencyBar = this.mRate.FindChild("UICircleProgressBar").GetComponent<UICircleProgressBar>();
			this.Fx_jiesuan = base.transform.FindChild("Fx_jiesuan").gameObject;
			this.Fx_bottlelvuptx = base.transform.FindChild("Fx_bottlelvuptx").GetComponent<EffectPlayTool>();
			this.mCoin.FindChild("GainCoinLabel").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_Gold");
			this.mBottle.FindChild("BottleNameLabel").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_MagicButtleLvl");
			this.mRate.FindChild("TitleLabel").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_MatchScore");
			this.mRate.FindChild("ProficiencyLabel").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_HeroProficiency");
			this.mDeltaCoin_int = ModelManager.Instance.Get_Settle_CoinDelta();
			this.mDeltaBottle_int = ModelManager.Instance.Get_Settle_BottleExpDelta();
			this.mDeltaRate_int = ModelManager.Instance.Get_Settle_ProficiencyExpDelta();
			this.mScore_char = ModelManager.Instance.Get_Settle_LevelScore();
			int lv = ModelManager.Instance.Get_Settle_BottleExpLevel();
			this.mBottleLevel.text = lv.ToString();
			this.mBottleTexture.mainTexture = ResourceManager.Load<Texture>(this.GetBottleIcon(lv), true, true, null, 0, false);
			this.initFlag = true;
		}

		private string GetBottleIcon(int _lv)
		{
			Dictionary<string, SysMagicbottleExpVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleExpVo>();
			int num = Tools_ParsePrice.Level_Check<SysMagicbottleExpVo>(_lv, typeDicByType, false);
			return BaseDataMgr.instance.GetDataById<SysMagicbottleExpVo>(num.ToString()).smallIcon;
		}

		private void onMsg_showNormal(MobaMessage msg)
		{
			this.Play();
			this.Fx_jiesuan.SetActive(true);
		}

		private void onLevelUp_bottleLabel()
		{
			this.mBottleLevel.text = (ModelManager.Instance.Get_Settle_BottleExpLevel() + Settlement_NormalRewards.bottle_levelUpCount).ToString();
			Settlement_NormalRewards.bottle_levelUpCount++;
			int nextLevel = ModelManager.Instance.Get_Settle_BottleExpLevel() + Settlement_NormalRewards.bottle_levelUpCount;
			this.mBottleLevelTemp.text = nextLevel.ToString();
			this.mBottleLevel.GetComponent<TweenAlpha>().ResetToBeginning();
			this.mBottleLevel.GetComponent<TweenPosition>().ResetToBeginning();
			this.mBottleLevelTemp.GetComponent<TweenAlpha>().ResetToBeginning();
			this.mBottleLevelTemp.GetComponent<TweenPosition>().ResetToBeginning();
			this.mBottleLevel.GetComponent<TweenAlpha>().PlayForward();
			this.mBottleLevel.GetComponent<TweenPosition>().PlayForward();
			this.mBottleLevelTemp.GetComponent<TweenAlpha>().PlayForward();
			this.mBottleLevelTemp.GetComponent<TweenPosition>().PlayForward();
			if (nextLevel <= 1)
			{
				return;
			}
			List<object> source = BaseDataMgr.instance.GetDicByType<SysMagicbottleExpVo>().Values.ToList<object>();
			SysMagicbottleExpVo sysMagicbottleExpVo = (SysMagicbottleExpVo)(from obj in source
			where ((SysMagicbottleExpVo)obj).levelRange <= nextLevel
			select obj).LastOrDefault<object>();
			int exp = sysMagicbottleExpVo.exp;
			this.mBottleTexture.mainTexture = ResourceManager.Load<Texture>(sysMagicbottleExpVo.smallIcon, true, true, null, 0, false);
			this.bottleBar.Play(-1f, -1f, (float)exp);
		}

		private void onLevelUp_bottle()
		{
			this.Fx_bottlelvuptx.transform.localPosition = this.mBottle.transform.localPosition;
			this.Fx_bottlelvuptx.Play();
		}

		private void onLevelUp_prociency()
		{
			SysHeroAttainmentLevelVo sysHeroAttainmentLevelVo = ModelManager.Instance.Get_ProficiencyNextLevel(ModelManager.Instance.Get_Settle_ProficiencyExpCurrent());
			this.proficiencyBar.Play(-1f, -1f, (float)sysHeroAttainmentLevelVo.skilled_scores);
			int exp = ModelManager.Instance.Get_Settle_ProficiencyExpCurrent() + this.mDeltaRate_int;
			SysHeroAttainmentLevelVo sysHeroAttainmentLevelVo2 = ModelManager.Instance.Get_ProficiencyLevel(exp);
			SysHeroAttainmentLevelVo sysHeroAttainmentLevelVo3 = ModelManager.Instance.Get_ProficiencyNextLevel(exp);
			if (sysHeroAttainmentLevelVo2 != null && sysHeroAttainmentLevelVo3 != null)
			{
				this.proficiencyBar.Play(-1f, -1f, (float)(sysHeroAttainmentLevelVo3.skilled_scores - sysHeroAttainmentLevelVo2.skilled_scores));
			}
		}

		private void Play()
		{
			this.mCurRateLabel.text = this.mScore_char.ToString();
			this.mGainCoinLabel.text = string.Format("+{0}", (int)((double)this.mDeltaCoin_int * 0.5));
			this.mGainBottleExpLabel.text = string.Format("+{0}", (int)((double)this.mDeltaBottle_int * 0.5));
			this.mProficiencyExpLabel.text = string.Format("+{0}", (int)((double)this.mDeltaRate_int * 0.5));
			this.cMgr.StartCoroutine(this.PlayScaleTween(), true);
			this.cMgr.StartCoroutine(this.PlayBarTween(), true);
		}

		private float GetOffset(int word)
		{
			return 114f + 11f * (float)word.ToString().Length;
		}

		[DebuggerHidden]
		private IEnumerator PlayBarTween()
		{
			Settlement_NormalRewards.<PlayBarTween>c__Iterator108 <PlayBarTween>c__Iterator = new Settlement_NormalRewards.<PlayBarTween>c__Iterator108();
			<PlayBarTween>c__Iterator.<>f__this = this;
			return <PlayBarTween>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator PlayScaleTween()
		{
			Settlement_NormalRewards.<PlayScaleTween>c__Iterator109 <PlayScaleTween>c__Iterator = new Settlement_NormalRewards.<PlayScaleTween>c__Iterator109();
			<PlayScaleTween>c__Iterator.<>f__this = this;
			return <PlayScaleTween>c__Iterator;
		}

		private void RefreshLabel(float factor)
		{
			this.mGainCoinLabel.text = string.Format("+{0}", (int)((double)this.mDeltaCoin_int * (0.5 + 0.5 * (double)factor)));
			this.mGainBottleExpLabel.text = string.Format("+{0}", (int)((double)this.mDeltaBottle_int * (0.5 + 0.5 * (double)factor)));
			this.mProficiencyExpLabel.text = string.Format("+{0}", (int)((double)this.mDeltaRate_int * (0.5 + 0.5 * (double)factor)));
		}

		[DebuggerHidden]
		private IEnumerator VelocityAttenuationCall(int step, float duration, Callback<float> callMethod)
		{
			Settlement_NormalRewards.<VelocityAttenuationCall>c__Iterator10A <VelocityAttenuationCall>c__Iterator10A = new Settlement_NormalRewards.<VelocityAttenuationCall>c__Iterator10A();
			<VelocityAttenuationCall>c__Iterator10A.step = step;
			<VelocityAttenuationCall>c__Iterator10A.duration = duration;
			<VelocityAttenuationCall>c__Iterator10A.callMethod = callMethod;
			<VelocityAttenuationCall>c__Iterator10A.<$>step = step;
			<VelocityAttenuationCall>c__Iterator10A.<$>duration = duration;
			<VelocityAttenuationCall>c__Iterator10A.<$>callMethod = callMethod;
			return <VelocityAttenuationCall>c__Iterator10A;
		}

		[DebuggerHidden]
		private IEnumerator ArcadeEffect(float duration)
		{
			Settlement_NormalRewards.<ArcadeEffect>c__Iterator10B <ArcadeEffect>c__Iterator10B = new Settlement_NormalRewards.<ArcadeEffect>c__Iterator10B();
			<ArcadeEffect>c__Iterator10B.duration = duration;
			<ArcadeEffect>c__Iterator10B.<$>duration = duration;
			<ArcadeEffect>c__Iterator10B.<>f__this = this;
			return <ArcadeEffect>c__Iterator10B;
		}
	}
}
