using System;
using UnityEngine;

public class NgAnimation
{
	public static AnimationClip SetAnimation(Animation tarAnimation, int tarIndex, AnimationClip srcClip)
	{
		int num = 0;
		AnimationClip[] array = new AnimationClip[tarAnimation.GetClipCount() - tarIndex + 1];
		foreach (AnimationState animationState in tarAnimation)
		{
			if (tarIndex == num)
			{
				tarAnimation.RemoveClip(animationState.clip);
			}
			if (tarIndex < num)
			{
				array[num - tarIndex - 1] = animationState.clip;
				tarAnimation.RemoveClip(animationState.clip);
			}
		}
		tarAnimation.AddClip(srcClip, srcClip.name);
		for (int i = 0; i < array.Length; i++)
		{
			tarAnimation.AddClip(array[i], array[i].name);
		}
		return srcClip;
	}

	public static AnimationState GetAnimationByIndex(Animation anim, int nIndex)
	{
		int num = 0;
		foreach (AnimationState result in anim)
		{
			if (num == nIndex)
			{
				return result;
			}
			num++;
		}
		return null;
	}
}
