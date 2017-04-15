using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewSkillEffect : MonoBehaviour
	{
		private enum LV
		{
			None,
			Lv1,
			Lv2,
			Lv3,
			Lv4
		}

		[SerializeField]
		private UILabel damageName;

		[SerializeField]
		private UILabel lv1;

		[SerializeField]
		private UILabel lv2;

		[SerializeField]
		private UILabel lv3;

		[SerializeField]
		private UILabel lv4;

		private int order;

		private int maxLevel;

		public int Order
		{
			get
			{
				return this.order;
			}
		}

		public int MaxLevel
		{
			get
			{
				return this.maxLevel;
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Init(string skillName, int index)
		{
			this.order = index + 1;
			SysSkillLevelupVo dataById = BaseDataMgr.instance.GetDataById<SysSkillLevelupVo>(skillName + "_lv01");
			this.maxLevel = dataById.skill_levelmax;
			this.lv4.gameObject.SetActive(4 == this.maxLevel);
			for (int num = 0; num != this.maxLevel; num++)
			{
				SysSkillDescVo dataById2 = BaseDataMgr.instance.GetDataById<SysSkillDescVo>(skillName + "_lv0" + (num + 1));
				if (dataById2 != null)
				{
					switch (this.order)
					{
					case 1:
						this.damageName.text = LanguageManager.Instance.GetStringById(dataById2.effect_desc1);
						this.Allocate(dataById2.effect_num1, num + 1);
						break;
					case 2:
						this.damageName.text = LanguageManager.Instance.GetStringById(dataById2.effect_desc2);
						this.Allocate(dataById2.effect_num2, num + 1);
						break;
					case 3:
						this.damageName.text = LanguageManager.Instance.GetStringById(dataById2.effect_desc3);
						this.Allocate(dataById2.effect_num3, num + 1);
						break;
					case 4:
						this.damageName.text = LanguageManager.Instance.GetStringById(dataById2.effect_desc4);
						this.Allocate(dataById2.effect_num4, num + 1);
						break;
					case 5:
						this.damageName.text = LanguageManager.Instance.GetStringById(dataById2.effect_desc5);
						this.Allocate(dataById2.effect_num5, num + 1);
						break;
					}
				}
			}
		}

		private void Allocate(string ID, int idx)
		{
			switch (idx)
			{
			case 1:
				this.ParseData(ID, this.lv1);
				break;
			case 2:
				this.ParseData(ID, this.lv2);
				break;
			case 3:
				this.ParseData(ID, this.lv3);
				break;
			case 4:
				this.ParseData(ID, this.lv4);
				break;
			}
		}

		private void ParseData(string effectID, UILabel uiLabel)
		{
			float num = 0f;
			if (effectID.CompareTo("[]") == 0)
			{
				return;
			}
			effectID = effectID.Trim(new char[]
			{
				'$'
			});
			bool flag = float.TryParse(effectID, out num);
			if (flag)
			{
				uiLabel.text = effectID;
			}
			else if (effectID.Substring(effectID.Length - 1, 1).CompareTo("%") == 0)
			{
				uiLabel.text = effectID;
			}
			else
			{
				string text = this.CutName(effectID);
				string iD = string.Empty;
				iD = effectID.Remove(0, text.Length + 1);
				if (text.CompareTo("Dam") == 0)
				{
					uiLabel.text = this.DamageValueReturn(iD);
				}
				if (text.CompareTo("Buff") == 0)
				{
					uiLabel.text = this.BuffValueReturn(iD);
				}
				if (text.CompareTo("Higheff") == 0)
				{
					uiLabel.text = this.HigheffValueReturn(iD);
				}
			}
		}

		private string DamageValueReturn(string ID)
		{
			if (string.IsNullOrEmpty(ID))
			{
				return string.Empty;
			}
			string text = string.Empty;
			string[] array = new string[0];
			SysSkillDamageVo dataById = BaseDataMgr.instance.GetDataById<SysSkillDamageVo>(ID);
			array = dataById.formula.Split(new char[]
			{
				'|'
			});
			if (array.Length >= 3)
			{
				if (array[0].CompareTo("4") == 0)
				{
					if (array.Length > 3)
					{
						if (array[3].CompareTo("1") == 0)
						{
							text = float.Parse(array[2]).ToString("P0");
							text = text.Replace(" ", string.Empty);
						}
					}
					else
					{
						text = array[2];
					}
				}
				else
				{
					text = array[3];
				}
			}
			return text.TrimStart(new char[]
			{
				'-'
			});
		}

		private string BuffValueReturn(string ID)
		{
			if (string.IsNullOrEmpty(ID))
			{
				return string.Empty;
			}
			string result = string.Empty;
			string[] array = ID.Split(new char[]
			{
				','
			});
			if (array.Length > 1)
			{
				SysSkillBuffVo dataById = BaseDataMgr.instance.GetDataById<SysSkillBuffVo>(array[0]);
				if (array[1].CompareTo("buff_time") == 0)
				{
					result = dataById.buff_time.ToString();
				}
				if (array[1].CompareTo("max_layers") == 0)
				{
					result = dataById.max_layers.ToString();
				}
			}
			return result;
		}

		private string HigheffValueReturn(string ID)
		{
			if (string.IsNullOrEmpty(ID))
			{
				return string.Empty;
			}
			string result = string.Empty;
			string[] array = ID.Split(new char[]
			{
				','
			});
			if (array.Length > 1)
			{
				SysSkillHigheffVo dataById = BaseDataMgr.instance.GetDataById<SysSkillHigheffVo>(array[0]);
				if (array[1].CompareTo("higheff_type") == 0)
				{
					result = dataById.higheff_type.Split(new char[]
					{
						'|'
					})[1];
				}
			}
			return result;
		}

		private string CutName(string name)
		{
			int length = name.IndexOf("_");
			return name.Substring(0, length);
		}
	}
}
