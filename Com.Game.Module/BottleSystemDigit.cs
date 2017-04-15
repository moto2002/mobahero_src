using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class BottleSystemDigit : MonoBehaviour
	{
		public enum Num
		{
			Unit,
			Decade,
			Hundred,
			Thousand,
			TenThousand
		}

		private int digit;

		private TweenPosition tweenPos;

		private TweenAlpha tweenAlp;

		private Material mat;

		private Texture2D tex;

		private BottleSystemDigit.Num num;

		private Task t_tween;

		private Task t_shine;

		private CoroutineManager coroutine = new CoroutineManager();

		private SpriteRenderer sr;

		private Vector3 oriPos;

		private _GPUParticleCtrl gpu;

		private UIPanel parPanel;

		private UIEffectSort ues;

		private Transform levelBG;

		public _GPUParticleCtrl GPU
		{
			get
			{
				return this.gpu;
			}
			set
			{
				this.gpu = value;
			}
		}

		public void Init(List<int> digits, int idx)
		{
			base.StopAllCoroutines();
			this.ues = base.transform.GetComponent<UIEffectSort>();
			this.levelBG = base.transform.parent.parent.Find("LevelBG");
			this.gpu = base.GetComponent<_GPUParticleCtrl>();
			this.parPanel = base.GetComponentInParent<UIPanel>();
			this.ues.panel = this.parPanel;
			this.ues.widgetInBack = this.levelBG.GetComponent<UISprite>();
			this.oriPos = base.transform.localPosition;
			int num = 0;
			this.sr = base.GetComponent<SpriteRenderer>();
			this.num = (BottleSystemDigit.Num)idx;
			if (digits != null)
			{
				this.digit = digits[idx];
			}
			this.mat = (Resources.Load("Prefab/UI/Bottle/ShinedFigures" + idx) as Material);
			Sprite sprite = Resources.Load<Sprite>("Texture/MagicBottle/Shined_figures_" + this.digit);
			this.sr.sprite = sprite;
			this.sr.material = this.mat;
			this.tweenPos = base.GetComponent<TweenPosition>();
			this.tweenAlp = base.GetComponent<TweenAlpha>();
			this.tweenPos.from = this.oriPos;
			this.tweenPos.to = this.oriPos;
			this.tweenAlp.from = 1f;
			this.tweenAlp.to = 1f;
			this.tweenPos.enabled = false;
			for (int i = digits.Count - 1; i >= 0; i--)
			{
				if (digits[i] != 0)
				{
					num = i;
					break;
				}
			}
			base.transform.gameObject.SetActive(true);
			if (this.num > (BottleSystemDigit.Num)num)
			{
				base.transform.gameObject.SetActive(false);
			}
		}

		public void CheckPosition(List<int> digits)
		{
			int num = 0;
			for (int i = digits.Count - 1; i >= 0; i--)
			{
				if (digits[i] != 0)
				{
					num = i;
					break;
				}
			}
			base.transform.gameObject.SetActive(true);
			if (this.num > (BottleSystemDigit.Num)num)
			{
				base.transform.gameObject.SetActive(false);
			}
			if (this.num == (BottleSystemDigit.Num)num)
			{
				base.transform.parent.GetComponent<UIGrid>().Reposition();
			}
		}

		public void CheckTween(List<int> digits)
		{
			int num = digits[(int)this.num];
			if (num != this.digit)
			{
				this.digit = num;
				Sprite sprite = Resources.Load<Sprite>("Texture/MagicBottle/Shined_figures_" + this.digit);
				this.sr.sprite = sprite;
			}
			else
			{
				this.sr.color = new Color(1f, 1f, 1f, 1f);
			}
			if (this.t_shine != null)
			{
				this.coroutine.StopCoroutine(this.t_shine);
			}
			this.t_shine = this.coroutine.StartCoroutine(this.DoShine(), true);
		}

		private void DoTween()
		{
			Vector3 localPosition = base.transform.localPosition;
			this.tweenPos.ResetToBeginning();
			this.tweenAlp.ResetToBeginning();
			this.tweenPos.method = UITweener.Method.EaseOut;
			this.tweenPos.from = localPosition;
			this.tweenPos.to = new Vector3(localPosition.x, localPosition.y - 100f, localPosition.z);
			this.tweenPos.duration = 0.8f;
			this.tweenAlp.from = this.sr.color.a;
			this.tweenAlp.to = 0f;
			this.tweenAlp.duration = 0.2f;
			this.tweenPos.PlayForward();
			this.tweenAlp.PlayForward();
			if (this.t_tween != null)
			{
				this.coroutine.StopCoroutine(this.t_tween);
			}
			this.t_tween = this.coroutine.StartCoroutine(this.DoTween_2(localPosition), true);
		}

		[DebuggerHidden]
		private IEnumerator DoTween_2(Vector3 temPos)
		{
			BottleSystemDigit.<DoTween_2>c__Iterator117 <DoTween_2>c__Iterator = new BottleSystemDigit.<DoTween_2>c__Iterator117();
			<DoTween_2>c__Iterator.temPos = temPos;
			<DoTween_2>c__Iterator.<$>temPos = temPos;
			<DoTween_2>c__Iterator.<>f__this = this;
			return <DoTween_2>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator DoShine()
		{
			BottleSystemDigit.<DoShine>c__Iterator118 <DoShine>c__Iterator = new BottleSystemDigit.<DoShine>c__Iterator118();
			<DoShine>c__Iterator.<>f__this = this;
			return <DoShine>c__Iterator;
		}
	}
}
