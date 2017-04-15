using Com.Game.Module;
using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class ScreenEffectAction : BaseHighEffAction
	{
		protected override void StartHighEff()
		{
			if (this.data.param1 == 1f)
			{
				if (!ViewTree.slashEffect1.gameObject.activeSelf)
				{
					ViewTree.slashEffect1.gameObject.SetActive(true);
					ViewTree.slashEffect1.GetComponentInChildren<ParticleSystem>().Play();
				}
			}
			else if (this.data.param1 == 2f && !ViewTree.slashEffect2.gameObject.activeSelf)
			{
				ViewTree.slashEffect2.gameObject.SetActive(true);
				ViewTree.slashEffect2.GetComponentInChildren<ParticleSystem>().Play();
			}
			this.Destroy();
		}

		protected override void StopHighEff()
		{
		}
	}
}
