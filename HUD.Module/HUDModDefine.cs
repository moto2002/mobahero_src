using System;
using System.Collections.Generic;

namespace HUD.Module
{
	public class HUDModDefine
	{
		public static Dictionary<EHUDModule, Type> mDicModType = new Dictionary<EHUDModule, Type>
		{
			{
				EHUDModule.BattleExploit,
				typeof(BattleExploitModule)
			},
			{
				EHUDModule.FPS,
				typeof(FPSModule)
			},
			{
				EHUDModule.DeathTimer,
				typeof(DeathTimerModule)
			},
			{
				EHUDModule.KillInfos,
				typeof(KillInfosModule)
			},
			{
				EHUDModule.FunctionBtns,
				typeof(FunctionBtnsModule)
			},
			{
				EHUDModule.ActionIndicator,
				typeof(ActionIndicator)
			},
			{
				EHUDModule.Buff,
				typeof(BuffModule)
			},
			{
				EHUDModule.PlayersIndicator,
				typeof(PlayersIndicator)
			},
			{
				EHUDModule.AttackIndicator,
				typeof(AttackIndicator)
			},
			{
				EHUDModule.BattleEvent,
				typeof(BattleEventModule)
			},
			{
				EHUDModule.Debug,
				typeof(DebugModule)
			},
			{
				EHUDModule.SiderTips,
				typeof(SiderTipsModule)
			},
			{
				EHUDModule.ChaosBattleExploit,
				typeof(ChaosBattleExploitModule)
			}
		};
	}
}
