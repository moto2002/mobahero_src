using System;
using UnityEngine;

public class lsAnim : NewMono
{
	public void easyInOutPosAndScaleTo(Vector3 pos, Vector3 scale, float animLength)
	{
		AnimationClip animationClip = new AnimationClip();
		AnimationCurve curve = AnimationCurve.EaseInOut(0f, base.trans.localPosition.x, animLength, pos.x);
		AnimationCurve curve2 = AnimationCurve.EaseInOut(0f, base.trans.localPosition.y, animLength, pos.y);
		AnimationCurve curve3 = AnimationCurve.EaseInOut(0f, base.trans.localPosition.z, animLength, pos.z);
		AnimationCurve curve4 = AnimationCurve.EaseInOut(0f, base.trans.localScale.x, animLength, scale.x);
		AnimationCurve curve5 = AnimationCurve.EaseInOut(0f, base.trans.localScale.y, animLength, scale.y);
		AnimationCurve curve6 = AnimationCurve.EaseInOut(0f, base.trans.localScale.z, animLength, scale.z);
		Type typeFromHandle = typeof(Transform);
		animationClip.SetCurve("", typeFromHandle, "localPosition.x", curve);
		animationClip.SetCurve("", typeFromHandle, "localPosition.y", curve2);
		animationClip.SetCurve("", typeFromHandle, "localPosition.z", curve3);
		animationClip.SetCurve("", typeFromHandle, "localScale.x", curve4);
		animationClip.SetCurve("", typeFromHandle, "localScale.y", curve5);
		animationClip.SetCurve("", typeFromHandle, "localScale.z", curve6);
		animationClip.wrapMode = WrapMode.Once;
		base.anim.AddClip(animationClip, "pos_scale");
		base.anim.Play("pos_scale");
	}

	public void easyInOutPosAndRotTo(Vector3 pos, Quaternion rot, float animLength)
	{
		AnimationClip animationClip = new AnimationClip();
		AnimationCurve curve = AnimationCurve.EaseInOut(0f, base.trans.localPosition.x, animLength, pos.x);
		AnimationCurve curve2 = AnimationCurve.EaseInOut(0f, base.trans.localPosition.y, animLength, pos.y);
		AnimationCurve curve3 = AnimationCurve.EaseInOut(0f, base.trans.localPosition.z, animLength, pos.z);
		AnimationCurve curve4 = AnimationCurve.EaseInOut(0f, base.trans.localRotation.x, animLength, rot.x);
		AnimationCurve curve5 = AnimationCurve.EaseInOut(0f, base.trans.localRotation.y, animLength, rot.y);
		AnimationCurve curve6 = AnimationCurve.EaseInOut(0f, base.trans.localRotation.z, animLength, rot.z);
		AnimationCurve curve7 = AnimationCurve.EaseInOut(0f, base.trans.localRotation.w, animLength, rot.w);
		Type typeFromHandle = typeof(Transform);
		animationClip.SetCurve("", typeFromHandle, "localPosition.x", curve);
		animationClip.SetCurve("", typeFromHandle, "localPosition.y", curve2);
		animationClip.SetCurve("", typeFromHandle, "localPosition.z", curve3);
		animationClip.SetCurve("", typeFromHandle, "localRotation.x", curve4);
		animationClip.SetCurve("", typeFromHandle, "localRotation.y", curve5);
		animationClip.SetCurve("", typeFromHandle, "localRotation.z", curve6);
		animationClip.SetCurve("", typeFromHandle, "localRotation.w", curve7);
		animationClip.wrapMode = WrapMode.Once;
		base.anim.AddClip(animationClip, "ls_move");
		base.anim.Play("ls_move");
	}

	public void easyInOutFromTo(Vector3 fromPos, Vector3 toPos, Quaternion fromRot, Quaternion toRot, float animLength)
	{
		base.trans.localPosition = fromPos;
		base.trans.localRotation = fromRot;
		this.easyInOutPosAndRotTo(toPos, toRot, animLength);
	}
}
