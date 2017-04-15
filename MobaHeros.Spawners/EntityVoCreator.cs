using System;
using System.Collections.Generic;

namespace MobaHeros.Spawners
{
	public class EntityVoCreator
	{
		public delegate string ResMapper(string oldId, EntityType entityType, TeamType team);

		private static readonly EntityVoCreator.ResMapper TrivalMapper = (string id, EntityType type, TeamType team) => id;

		private readonly EntityVoCreator.ResMapper _mapper;

		public EntityVoCreator(EntityVoCreator.ResMapper mapper)
		{
			this._mapper = mapper;
			if (this._mapper == null)
			{
				this._mapper = EntityVoCreator.TrivalMapper;
			}
		}

		public List<EntityVo> GetEntityVos(string npcConfig, EntityType type, TeamType team)
		{
			if (npcConfig == null || npcConfig == "[]" || npcConfig == string.Empty)
			{
				return null;
			}
			List<EntityVo> list = new List<EntityVo>();
			string[] stringValue = StringUtils.GetStringValue(npcConfig, ',');
			for (int i = 0; i < stringValue.Length; i++)
			{
				EntityVo entityVo = this.GetEntityVo(stringValue[i], type, team, 0);
				list.Add(entityVo);
			}
			return list;
		}

		public List<EntityVo> GetPvpEntityVos(string npcConfig, EntityType type, TeamType team, int startUniqId)
		{
			if (npcConfig == null || npcConfig == "[]" || npcConfig == string.Empty)
			{
				return null;
			}
			List<EntityVo> list = new List<EntityVo>();
			string[] stringValue = StringUtils.GetStringValue(npcConfig, ',');
			for (int i = 0; i < stringValue.Length; i++)
			{
				EntityVo entityVo = this.GetEntityVo(stringValue[i], type, team, startUniqId++);
				list.Add(entityVo);
			}
			return list;
		}

		public EntityVo GetEntityVo(string npcConfig, EntityType type, TeamType team, int uniqId = 0)
		{
			if (npcConfig == null || npcConfig == "[]" || npcConfig == string.Empty)
			{
				return null;
			}
			string[] stringValue = StringUtils.GetStringValue(npcConfig, '|');
			EntityVo result;
			if (type == EntityType.Hero)
			{
				string npc_id = stringValue[0];
				int ai_type = int.Parse(stringValue[1]);
				int level = int.Parse(stringValue[2]);
				int star = int.Parse(stringValue[3]);
				int quality = int.Parse(stringValue[4]);
				result = new EntityVo(type, npc_id, level, star, quality, 0f, 0f, ai_type, 0)
				{
					uid = uniqId
				};
			}
			else
			{
				string oldId = stringValue[0];
				int ai_type2 = int.Parse(stringValue[1]);
				int pos = int.Parse(stringValue[2]);
				string npc_id2 = this._mapper(oldId, type, team);
				if (type == EntityType.Tower || type == EntityType.Home)
				{
					string priority = string.Empty;
					if (stringValue.Length == 4)
					{
						priority = stringValue[3];
					}
					result = new EntityVo(type, npc_id2, pos, ai_type2, priority, "Default", 0)
					{
						uid = uniqId
					};
				}
				else
				{
					result = new EntityVo(type, npc_id2, pos, ai_type2, string.Empty, "Default", 0)
					{
						uid = uniqId
					};
				}
			}
			return result;
		}

		public string GetCorrectModelId(string npcId, EntityType type, TeamType team)
		{
			return this._mapper(npcId, type, team);
		}
	}
}
