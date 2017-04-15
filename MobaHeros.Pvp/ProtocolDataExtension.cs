using MobaProtocol.Data;
using System;

namespace MobaHeros.Pvp
{
	public static class ProtocolDataExtension
	{
		public static string GetHeroId(this ReadyPlayerSampleInfo info)
		{
			if (info == null || info.heroInfo == null)
			{
				return null;
			}
			return info.heroInfo.heroId;
		}
	}
}
