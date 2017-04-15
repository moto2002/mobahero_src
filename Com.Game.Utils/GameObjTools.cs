using System;
using UnityEngine;

namespace Com.Game.Utils
{
	public static class GameObjTools
	{
		public static void PlayShowOrHideAnim(this GameObject go, bool showOrHide, EventDelegate.Callback callback = null)
		{
			if (null == go)
			{
				return;
			}
			if (showOrHide)
			{
				TweenScale tweenScale = go.GetComponent<TweenScale>();
				if (tweenScale == null)
				{
					tweenScale = go.AddComponent<TweenScale>();
				}
				tweenScale.from = Vector3.zero;
				tweenScale.to = Vector3.one;
				AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f),
					new Keyframe(0.8f, 1.05f),
					new Keyframe(1f, 1f)
				});
				tweenScale.animationCurve = animationCurve;
				tweenScale.duration = 0.25f;
				tweenScale.Begin();
				TweenAlpha.Begin(go, 0.2f, 1f);
				if (callback != null)
				{
					EventDelegate.Add(tweenScale.onFinished, callback, true);
				}
			}
			else
			{
				TweenScale.Begin(go, 0.1f, Vector3.one * 0.8f);
				TweenAlpha tweenAlpha = TweenAlpha.Begin(go, 0.1f, 0f);
				if (callback != null)
				{
					EventDelegate.Add(tweenAlpha.onFinished, callback, true);
				}
			}
		}
	}
}
