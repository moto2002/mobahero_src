using System;
using UnityEngine;

namespace SkillMechanism
{
	public class AnimEventsListener : MobaMono
	{
		public const string EVENT_ATK = "atk";

		public const string EVENT_GENSPELL = "genSpell";

		private void atk()
		{
		}

		private void genSpell(string strParam)
		{
			SpellEventData spellEventData = AnimEventsListener.parseFromString(strParam);
			if (spellEventData == null)
			{
				Debug.LogError("unrecognized spell event:" + strParam);
				return;
			}
			try
			{
				this.genBindSpell(spellEventData);
			}
			catch (NotFoundException ex)
			{
				LSDebug.error(ex.errInfo);
			}
		}

		private void genBindSpell(SpellEventData sb)
		{
		}

		public static SpellEventData parseFromString(string strParam)
		{
			string[] array = strParam.Split(new char[]
			{
				','
			});
			if (array.Length != 3)
			{
				Debug.LogError(strParam + " is invalid!");
				return null;
			}
			string goName = array[0];
			string spellPath = array[1];
			BindType bindType = (BindType)int.Parse(array[2]);
			return new SpellEventData(goName, spellPath, bindType);
		}

		public static string encodeStrParam(SpellEventData sb)
		{
			return string.Format("{0},{1},{2}", sb.bindName, sb.spellPath, (int)sb.bindType);
		}
	}
}
