using System;
using UnityEngine;

public class PostEffectX
{
	public static Gray gr;

	public static void DoGray()
	{
		GameObject gameObject = BattleCameraMgr.Instance.BattleCamera.gameObject;
		if (gameObject != null && PostEffectX.gr == null)
		{
			PostEffectX.gr = gameObject.AddComponent<Gray>();
		}
		PostEffectX.gr.DoGrayOnEffect(1f);
	}

	public static void FinishGray()
	{
		if (PostEffectX.gr != null)
		{
			PostEffectX.gr.Finish(1f);
		}
	}
}
