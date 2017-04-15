using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_contentBase : MonoBehaviour
	{
		public EActivityType AType
		{
			get;
			set;
		}

		public object Info
		{
			get;
			set;
		}

		public object CurActivityOrNotice
		{
			get;
			set;
		}

		public Action<Activity_contentBase> AckInitFinish
		{
			get;
			set;
		}

		public virtual EActivityModuleType GetModuleType()
		{
			return EActivityModuleType.eNone;
		}

		[DebuggerHidden]
		public virtual IEnumerator RefreshUI(Func<IEnumerator> ieBreak)
		{
			return new Activity_contentBase.<RefreshUI>c__IteratorB2();
		}
	}
}
