using System;

namespace Com.Game.Utils
{
	internal class EntityVoHelper
	{
		public static EntityVo GetEntityVo(string npc_config, EntityType type, TeamType team, Func<string, EntityType, TeamType, string> modelIdModifier = null)
		{
			if (npc_config != null && npc_config != "[]" && npc_config != string.Empty)
			{
				string[] stringValue = StringUtils.GetStringValue(npc_config, '|');
				EntityVo result;
				if (type == EntityType.Hero)
				{
					string npc_id = stringValue[0];
					int ai_type = int.Parse(stringValue[1]);
					int level = int.Parse(stringValue[2]);
					int star = int.Parse(stringValue[3]);
					int quality = int.Parse(stringValue[4]);
					result = new EntityVo(type, npc_id, level, star, quality, 0f, 0f, ai_type, 0);
				}
				else
				{
					string text = stringValue[0];
					int ai_type2 = int.Parse(stringValue[1]);
					int pos = int.Parse(stringValue[2]);
					if (modelIdModifier != null)
					{
						text = modelIdModifier(text, type, team);
					}
					result = new EntityVo(type, text, pos, ai_type2, string.Empty, "Default", 0);
				}
				return result;
			}
			return null;
		}
	}
}
