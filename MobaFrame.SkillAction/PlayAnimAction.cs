using Com.Game.Module;
using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class PlayAnimAction : BaseAction
	{
		public string performId;

		public AnimationType actionType;

		public float actionDelayTime;

		public float actionTime;

		private PerformData data;

		public PerformData mData
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		protected override void OnInit()
		{
			base.OnInit();
			this.data = Singleton<PerformDataManager>.Instance.GetVo(this.performId);
			this.actionDelayTime = this.data.config.action_delay;
			this.actionType = this.data.action_type;
			this.actionTime = this.data.config.action_time;
		}

		protected override bool doAction()
		{
			if (this.data.action_type == AnimationType.Conjure || this.data.action_type == AnimationType.ComboAttack)
			{
				if (base.unit.CurAnimationAction != null)
				{
					base.unit.CurAnimationAction.Destroy();
				}
				base.unit.CurAnimationAction = this;
			}
			base.mCoroutineManager.StartCoroutine(this.PlayAnim_Coroutinue(), true);
			return true;
		}

		protected override void OnSendStart()
		{
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			base.mCoroutineManager.StopAllCoroutine();
		}

		[DebuggerHidden]
		private IEnumerator PlayAnim_Coroutinue()
		{
			PlayAnimAction.<PlayAnim_Coroutinue>c__Iterator6F <PlayAnim_Coroutinue>c__Iterator6F = new PlayAnimAction.<PlayAnim_Coroutinue>c__Iterator6F();
			<PlayAnim_Coroutinue>c__Iterator6F.<>f__this = this;
			return <PlayAnim_Coroutinue>c__Iterator6F;
		}
	}
}
