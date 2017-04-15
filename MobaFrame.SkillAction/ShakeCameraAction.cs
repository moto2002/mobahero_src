using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class ShakeCameraAction : BaseStateAction
	{
		public static bool isShakeCamera;

		private Vector3 orgPosition;

		protected override bool IsInState
		{
			get
			{
				return ShakeCameraAction.isShakeCamera;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			ShakeCameraAction.isShakeCamera = true;
			this.orgPosition = Camera.main.transform.position;
		}

		protected override bool doAction()
		{
			base.AddActionToSkill(this);
			return base.doAction();
		}

		protected override void OnDestroy()
		{
			this.RemoveActionFromSkill(this);
			base.OnDestroy();
		}

		protected override void RevertState()
		{
			base.RevertState();
			Camera.main.transform.position = this.orgPosition;
			ShakeCameraAction.isShakeCamera = false;
			this.m_coroutinue.StopAllCoroutine();
		}

		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			ShakeCameraAction.<Coroutine>c__Iterator86 <Coroutine>c__Iterator = new ShakeCameraAction.<Coroutine>c__Iterator86();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}
	}
}
