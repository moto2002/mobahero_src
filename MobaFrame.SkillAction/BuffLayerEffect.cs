using System;

namespace MobaFrame.SkillAction
{
	public class BuffLayerEffect : BaseHighEffAction
	{
		private PlayEffectAction gthEffect;

		private string curPerform = string.Empty;

		protected override void OnInit()
		{
			base.OnInit();
		}

		private void DoBuffLayerEffect()
		{
			if (this.targetUnits != null && this.data.strParams1 != null)
			{
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null && this.data.param1 != 0f)
					{
						if (this.targetUnits[i].buffManager.IsHaveBuffGroup((int)this.data.param1))
						{
							int buffLayerByGroupId = this.targetUnits[i].buffManager.GetBuffLayerByGroupId((int)this.data.param1);
							if (buffLayerByGroupId == 0)
							{
								if (this.gthEffect != null)
								{
									this.gthEffect.Destroy();
								}
							}
							else
							{
								int num = buffLayerByGroupId - 1;
								if (this.data.strParams1.Length > num && this.curPerform != this.data.strParams1[num])
								{
									if (this.gthEffect != null)
									{
										this.gthEffect.Destroy();
									}
									this.gthEffect = ActionManager.PlayEffect(this.data.strParams1[num], this.targetUnits[i], null, null, true, string.Empty, null);
									this.curPerform = this.data.strParams1[num];
								}
							}
						}
						else if (this.gthEffect != null)
						{
							this.gthEffect.Destroy();
						}
					}
				}
			}
		}

		protected override void doStartHighEffect_Special()
		{
			this.DoBuffLayerEffect();
		}

		protected override void doStartHighEffect_Perform()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
			if (this.gthEffect != null)
			{
				this.gthEffect.Destroy();
			}
		}
	}
}
