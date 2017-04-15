using System;
using System.Collections.Generic;

namespace MobaFrame.SkillAction
{
	public class CharacterEffectAction : CompositeAction
	{
		public string effectIds;

		public int triggerType;

		private Dictionary<int, List<string>> character_effects;

		protected override void OnInit()
		{
			base.OnInit();
			this.Parse();
		}

		protected override bool doAction()
		{
			List<string> list = null;
			if (this.character_effects != null && this.character_effects.ContainsKey(this.triggerType))
			{
				list = this.character_effects[this.triggerType];
			}
			switch (this.triggerType)
			{
			case 1:
			case 6:
			case 7:
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						this.AddAction(ActionManager.PlayEffect(list[i], base.unit, null, null, true, string.Empty, null));
					}
				}
				break;
			case 2:
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (StringUtils.CheckValid(list[j]))
						{
							this.AddAction(ActionManager.PlayEffect(list[j], base.unit, null, null, true, string.Empty, null));
						}
					}
				}
				else if (!base.unit.isBuilding)
				{
					base.unit.RemoveSelf(0f);
				}
				break;
			}
			return true;
		}

		private void Parse()
		{
			this.character_effects = new Dictionary<int, List<string>>();
			string[] stringValue = StringUtils.GetStringValue(this.effectIds, ',');
			for (int i = 0; i < stringValue.Length; i++)
			{
				if (!(stringValue[i] == "Default"))
				{
					string[] stringValue2 = StringUtils.GetStringValue(stringValue[i], '|');
					if (stringValue2 != null)
					{
						int key = int.Parse(stringValue2[0]);
						string text = stringValue2[1];
						if (StringUtils.CheckValid(text))
						{
							if (!this.character_effects.ContainsKey(key))
							{
								this.character_effects.Add(key, new List<string>());
							}
							this.character_effects[key].Add(text);
						}
					}
				}
			}
		}

		public bool IsDeathEffect()
		{
			return this.triggerType == 2;
		}
	}
}
