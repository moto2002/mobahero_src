using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using MobaHeros;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewNature : MonoBehaviour
	{
		private Transform Nature_Transform;

		private UIScrollView NatureScroll;

		private Transform Natures;

		private object[] mgs;

		private string heroNPC;

		private PropViewNature instance;

		private Dictionary<string, string> Attributes = new Dictionary<string, string>();

		private Dictionary<string, string> ExtraAttrs = new Dictionary<string, string>();

		private Dictionary<string, string> AttributesBackUp = new Dictionary<string, string>();

		private string HP = string.Empty;

		private string MP = string.Empty;

		private string ATK = string.Empty;

		private string AP = string.Empty;

		private string ARMOR = string.Empty;

		private string MR = string.Empty;

		private string AS = string.Empty;

		private string AR = string.Empty;

		private string CS = string.Empty;

		private string APT = string.Empty;

		private string MPT = string.Empty;

		private string HPR = string.Empty;

		private string MPR = string.Empty;

		private string MS = string.Empty;

		public string HeroNPC
		{
			get
			{
				return this.heroNPC;
			}
		}

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.sacriviewChangeHero,
				ClientV2C.propviewChangeToggle
			};
			this.Initialize();
			this.instance = this;
		}

		private void OnEnable()
		{
			this.Register();
		}

		private void OnDisable()
		{
			this.Unregister();
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Initialize()
		{
			this.Attributes.Clear();
			this.Nature_Transform = base.transform.Find("HeroNature");
			this.NatureScroll = this.Nature_Transform.Find("NatureScroll").GetComponent<UIScrollView>();
			this.Natures = this.Nature_Transform.Find("NatureScroll/nature");
		}

		private void OnMsg_sacriviewChangeHero(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string value = string.Empty;
				value = (string)msg.Param;
				if (!string.IsNullOrEmpty(value))
				{
					this.heroNPC = value;
					this.ChangeNature();
					UIToggle activeToggle = UIToggle.GetActiveToggle(8);
					if (activeToggle.name.CompareTo("DetailsBtn") == 0)
					{
						this.ExtraAttribute(true);
					}
				}
			}
		}

		private void OnMsg_propviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				PropertyType propertyType = (PropertyType)((int)msg.Param);
				this.Nature_Transform.gameObject.SetActive(propertyType == PropertyType.Nature);
				if (propertyType == PropertyType.Nature)
				{
					this.NatureScroll.ResetPosition();
				}
				this.ExtraAttribute(propertyType == PropertyType.Nature);
			}
		}

		private void ChangeNature()
		{
			this.SetValue();
			int childCount = this.Natures.childCount;
			for (int num = 0; num != childCount; num++)
			{
				this.Natures.GetChild(num).GetChild(0).GetComponent<UILabel>().text = this.Attributes[this.Natures.GetChild(num).name];
			}
		}

		private void SetValue()
		{
			HeroAttr attr = new HeroAttr(this.heroNPC);
			this.HP = this.GetDes(attr, AttrType.HpMax);
			this.MP = this.GetDes(attr, AttrType.MpMax);
			this.ATK = this.GetDes(attr, AttrType.Attack);
			this.AP = this.GetDes(attr, AttrType.MagicPower);
			this.ARMOR = this.GetDes(attr, AttrType.Armor);
			this.MR = this.GetDes(attr, AttrType.MagicResist);
			this.AS = this.GetDes(attr, AttrType.AttackSpeed);
			this.AR = this.GetDes(attr, AttrType.AttackRange);
			this.CS = this.GetDes(attr, AttrType.PhysicCritProp);
			this.APT = this.GetDes(attr, AttrType.ArmorCut);
			this.MPT = this.GetDes(attr, AttrType.MagicResistanceCut);
			this.HPR = this.GetDes(attr, AttrType.HpRestore);
			this.MPR = this.GetDes(attr, AttrType.MpRestore);
			this.MS = this.GetDes(attr, AttrType.MoveSpeed);
			this.Attributes["HP"] = this.HP;
			this.Attributes["MP"] = this.MP;
			this.Attributes["ATK"] = this.ATK;
			this.Attributes["AP"] = this.AP;
			this.Attributes["ARMOR"] = this.ARMOR;
			this.Attributes["MR"] = this.MR;
			this.Attributes["AS"] = this.AS;
			this.Attributes["AR"] = this.AR;
			this.Attributes["CS"] = this.CS;
			this.Attributes["APT"] = this.APT;
			this.Attributes["MPT"] = this.MPT;
			this.Attributes["HPR"] = this.HPR;
			this.Attributes["MPR"] = this.MPR;
			this.Attributes["MS"] = this.MS;
		}

		private void ExtraAttribute(bool isActive)
		{
			if (isActive)
			{
				this.SetValue();
				this.ExtraAttrs.Clear();
				this.AttributesBackUp.Clear();
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.heroNPC);
				if (heroInfoData != null)
				{
					if (heroInfoData.Ep_1 != 0)
					{
						this.ParseRunesNature(heroInfoData.Ep_1);
					}
					if (heroInfoData.Ep_2 != 0)
					{
						this.ParseRunesNature(heroInfoData.Ep_2);
					}
					if (heroInfoData.Ep_3 != 0)
					{
						this.ParseRunesNature(heroInfoData.Ep_3);
					}
					if (heroInfoData.Ep_4 != 0)
					{
						this.ParseRunesNature(heroInfoData.Ep_4);
					}
					if (heroInfoData.Ep_5 != 0)
					{
						this.ParseRunesNature(heroInfoData.Ep_5);
					}
					if (heroInfoData.Ep_6 != 0)
					{
						this.ParseRunesNature(heroInfoData.Ep_6);
					}
				}
				int childCount = this.Natures.childCount;
				string text = string.Empty;
				List<UILabel> list = new List<UILabel>();
				for (int num = 0; num != childCount; num++)
				{
					list.Add(this.Natures.GetChild(num).GetComponent<UILabel>());
				}
				if (this.ExtraAttrs.Count != 0)
				{
					foreach (KeyValuePair<string, string> current in this.ExtraAttrs)
					{
						text = LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(current.Key).attrName);
						for (int num2 = 0; num2 != childCount; num2++)
						{
							if (text.CompareTo(list[num2].text) == 0)
							{
								this.Attributes[list[num2].name] = this.Attributes[list[num2].name] + "[00ff00] +" + current.Value + "[-]";
							}
						}
					}
				}
				if (this.AttributesBackUp.Count != 0)
				{
					foreach (KeyValuePair<string, string> current2 in this.AttributesBackUp)
					{
						text = LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(current2.Key).attrName);
						for (int num3 = 0; num3 != childCount; num3++)
						{
							if (text.CompareTo(list[num3].text) == 0)
							{
								this.Attributes[list[num3].name] = this.Attributes[list[num3].name] + "[ffff00] +" + current2.Value + "/级[-]";
							}
						}
					}
				}
				for (int num4 = 0; num4 != this.Natures.childCount; num4++)
				{
					this.Natures.GetChild(num4).GetChild(0).GetComponent<UILabel>().text = this.Attributes[this.Natures.GetChild(num4).name];
				}
			}
		}

		private void ParseRunesNature(int modelid)
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(modelid.ToString());
			if (dataById != null)
			{
				int rune_type = dataById.rune_type;
				string[] array = dataById.attribute.Split(new char[]
				{
					','
				});
				if (rune_type == 1)
				{
					if (array != null && array.Count<string>() == 1)
					{
						string[] array2 = array[0].Split(new char[]
						{
							'|'
						});
						if (array2 != null && array.Count<string>() != 0)
						{
							if (this.ExtraAttrs.ContainsKey(array2[0]))
							{
								this.ExtraAttrs[array2[0]] = this.CaptureString(this.ExtraAttrs[array2[0]], array[0]);
							}
							else
							{
								string text = array2[1];
								bool flag = 0 == text.Substring(text.Length - 1, 1).CompareTo("%");
								this.ExtraAttrs[array2[0]] = ((!flag) ? float.Parse(text).ToString(array2[2]) : text);
							}
						}
					}
				}
				else if (rune_type == 2 && array != null && array.Count<string>() == 1)
				{
					string[] array2 = array[0].Split(new char[]
					{
						'|'
					});
					if (array2 != null && array.Count<string>() != 0)
					{
						if (this.AttributesBackUp.ContainsKey(array2[0]))
						{
							this.AttributesBackUp[array2[0]] = this.CaptureString(this.AttributesBackUp[array2[0]], array[0]);
						}
						else
						{
							string text2 = array2[1];
							bool flag2 = 0 == text2.Substring(text2.Length - 1, 1).CompareTo("%");
							this.AttributesBackUp[array2[0]] = ((!flag2) ? float.Parse(text2).ToString(array2[2]) : text2);
						}
					}
				}
			}
		}

		private string GetDes(HeroAttr attr, AttrType type)
		{
			string result = string.Empty;
			float basicAttr = attr.GetBasicAttr(type);
			float num = attr.GetAttr(type) - basicAttr;
			if (type == AttrType.HitProp || type == AttrType.DodgeProp || type == AttrType.PhysicCritProp)
			{
				result = basicAttr.ToString("P0");
			}
			else
			{
				result = basicAttr.ToString("f2");
			}
			return result;
		}

		private string CaptureString(string strTarget, string strSource)
		{
			if (string.IsNullOrEmpty(strSource))
			{
				return null;
			}
			float num = 0f;
			string text = string.Empty;
			string empty = string.Empty;
			string[] array = strSource.Split(new char[]
			{
				'|'
			});
			if (array != null && 2 < array.Length)
			{
				if (array[1].Substring(array[1].Length - 1, 1).CompareTo("%") == 0)
				{
					num = float.Parse(array[1].Remove(array[1].LastIndexOf("%"), 1)) / 100f;
					text = "%";
				}
				else
				{
					num = float.Parse(array[1]);
					text = array[2];
				}
			}
			float num2;
			if (strTarget.Substring(strTarget.Length - 1, 1).CompareTo("%") == 0)
			{
				num2 = float.Parse(strTarget.Remove(strTarget.LastIndexOf("%"), 1)) / 100f;
			}
			else
			{
				num2 = float.Parse(strTarget);
			}
			num2 += num;
			return (text.CompareTo("%") != 0) ? num2.ToString(text) : string.Format("{0:p}", num2);
		}
	}
}
