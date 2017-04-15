using System;
using System.Collections;
using System.Diagnostics;

namespace MobaHeros
{
	internal class AutoTestController
	{
		public static bool UseAI
		{
			get
			{
				return GlobalSettings.Instance.autoTestSetting.enablePlayerAI;
			}
		}

		public static bool IsEnable(AutoTestTag tag)
		{
			return GlobalSettings.Instance.autoTestSetting.testLevel >= tag;
		}

		public static void AddHandler(string name, AutoTestTag tag, EventHandler<AutoTestEventBase> handler)
		{
			if (AutoTestController.IsEnable(tag))
			{
			}
		}

		public static void RemoveHandler(string name, AutoTestTag tag, EventHandler<AutoTestEventBase> handler)
		{
			if (AutoTestController.IsEnable(tag))
			{
			}
		}

		public static void TriggerEvent(AutoTestEventBase msg)
		{
			if (AutoTestController.IsEnable(msg.Tag))
			{
			}
		}

		public static void InvokeTestLogic(AutoTestTag tag, Action dele, float delay = 1f)
		{
			if (!AutoTestController.IsEnable(tag) || dele == null)
			{
				return;
			}
			if (delay < 1.401298E-45f)
			{
				dele();
			}
			else
			{
				new Task(AutoTestController.Delay(dele, delay), true);
			}
		}

		[DebuggerHidden]
		private static IEnumerator Delay(Action act, float delay)
		{
			AutoTestController.<Delay>c__Iterator1DB <Delay>c__Iterator1DB = new AutoTestController.<Delay>c__Iterator1DB();
			<Delay>c__Iterator1DB.delay = delay;
			<Delay>c__Iterator1DB.act = act;
			<Delay>c__Iterator1DB.<$>delay = delay;
			<Delay>c__Iterator1DB.<$>act = act;
			return <Delay>c__Iterator1DB;
		}
	}
}
