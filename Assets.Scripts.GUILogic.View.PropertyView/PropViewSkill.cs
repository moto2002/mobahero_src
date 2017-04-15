using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewSkill : MonoBehaviour
	{
		private const int skillCount = 4;

		private Transform heroSkills;

		private Transform Details;

		private Transform activeSkill;

		private Transform Lv4;

		private Transform transMana;

		private UIGrid skillGrid;

		private UIGrid skillEffectGrid;

		private UILabel skillName;

		private UILabel skillDescription;

		private UILabel passiveSkill;

		private UILabel coolDown;

		private UILabel typeRise;

		private UILabel manaCost;

		private UIAtlas atlas;

		private Transform bottomAnchor;

		private PropViewSkillItem skillItem;

		private PropViewSkillEffect skillEffect;

		private object[] mgs;

		private string heroNPC;

		private Color physicDamage = new Color(1f, 0.65882355f, 0.0627451f);

		private Color magicDamage = new Color(0.160784319f, 0.7921569f, 1f);

		private Color holyDamage = new Color(1f, 0.0627451f, 0.0627451f);

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
			this.bottomAnchor = base.transform.parent.Find("BottomAnchor");
			this.skillItem = Resources.Load<PropViewSkillItem>("Prefab/UI/Sacrificial/PropSkill");
			this.skillEffect = Resources.Load<PropViewSkillEffect>("Prefab/UI/Sacrificial/PropSkillEffect");
			this.heroSkills = base.transform.Find("HeroSkills");
			this.Details = this.bottomAnchor.transform.Find("Panel/Details");
			this.Lv4 = this.Details.Find("LvContent/LvCount/Lv4");
			this.skillGrid = this.heroSkills.Find("Skills").GetComponent<UIGrid>();
			this.skillEffectGrid = this.Details.Find("LvContent/LvGrid").GetComponent<UIGrid>();
			this.skillName = this.Details.Find("SkillName").GetComponent<UILabel>();
			this.passiveSkill = this.Details.Find("Type/Passive").GetComponent<UILabel>();
			this.activeSkill = this.Details.Find("Type/Active");
			this.transMana = this.activeSkill.Find("Mana");
			this.coolDown = this.activeSkill.Find("CoolDown/Content").GetComponent<UILabel>();
			this.typeRise = this.activeSkill.Find("Mana").GetComponent<UILabel>();
			this.manaCost = this.activeSkill.Find("Mana/Content").GetComponent<UILabel>();
			this.skillDescription = this.Details.Find("Description").GetComponent<UILabel>();
		}

		private void InitUI()
		{
			this.Details.gameObject.SetActive(false);
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.heroNPC);
			string[] AllSkillName = heroMainData.skill_id.Split(new char[]
			{
				','
			});
			SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(AllSkillName[0]);
			string skill_icon = skillMainData.skill_icon;
			string[] array = skill_icon.Split(new char[]
			{
				'_'
			});
			this.atlas = null;
			this.atlas = Resources.Load<UIAtlas>("Texture/Skiller/" + array[0] + "_" + array[1]);
			if (null == this.atlas)
			{
				return;
			}
			GridHelper.FillGrid<PropViewSkillItem>(this.skillGrid, this.skillItem, 4, delegate(int idx, PropViewSkillItem comp)
			{
				comp.name = idx.ToString();
				comp.DragCallBack = new Callback<GameObject, bool>(this.ChangeState);
				comp.Init(idx, AllSkillName[idx], this.atlas);
			});
			this.skillGrid.Reposition();
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
					this.InitUI();
				}
			}
		}

		private void OnMsg_propviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				PropertyType propertyType = (PropertyType)((int)msg.Param);
				this.heroSkills.gameObject.SetActive(propertyType == PropertyType.Info);
			}
		}

		private void ChangeState(GameObject obj, bool isIn)
		{
			this.Details.gameObject.SetActive(isIn);
			if (!isIn)
			{
				return;
			}
			PropViewSkillItem componentInParent = obj.GetComponentInParent<PropViewSkillItem>();
			this.ChangeSkillText((int)componentInParent.SkillEnum);
		}

		private void ChangeSkillText(int index)
		{
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.heroNPC);
			string[] AllSkillName = heroMainData.skill_id.Split(new char[]
			{
				','
			});
			SysSkillMainVo skillData = SkillUtility.GetSkillData(AllSkillName[index - 1], -1, -1);
			this.skillName.text = LanguageManager.Instance.GetStringById(skillData.skill_name);
			this.skillDescription.text = LanguageManager.Instance.GetStringById(skillData.skill_description);
			if (skillData.skill_trigger == 3)
			{
				this.activeSkill.gameObject.SetActive(false);
				this.passiveSkill.gameObject.SetActive(true);
				this.passiveSkill.text = LanguageManager.Instance.GetStringById("HeroAltar_Passive");
			}
			else
			{
				this.activeSkill.gameObject.SetActive(true);
				this.passiveSkill.gameObject.SetActive(false);
				this.DealWithActiveSkill(AllSkillName[index - 1]);
			}
			SysSkillDescVo dataById = BaseDataMgr.instance.GetDataById<SysSkillDescVo>(AllSkillName[index - 1] + "_lv01");
			SysSkillLevelupVo dataById2 = BaseDataMgr.instance.GetDataById<SysSkillLevelupVo>(AllSkillName[index - 1] + "_lv01");
			this.Lv4.gameObject.SetActive(4 == dataById2.skill_levelmax);
			for (int num = 0; num != this.skillEffectGrid.transform.childCount; num++)
			{
				this.skillEffectGrid.transform.GetChild(num).gameObject.SetActive(true);
			}
			GridHelper.FillGrid<PropViewSkillEffect>(this.skillEffectGrid, this.skillEffect, (dataById != null) ? dataById.effect_num : 0, delegate(int idx, PropViewSkillEffect comp)
			{
				comp.name = index.ToString();
				comp.Init(AllSkillName[index - 1], idx);
			});
			this.skillEffectGrid.Reposition();
		}

		private void DealWithActiveSkill(string skillName)
		{
			int num = 0;
			string text = string.Empty;
			string text2 = string.Empty;
			string costID = string.Empty;
			SysSkillDescVo dataById = BaseDataMgr.instance.GetDataById<SysSkillDescVo>(skillName + "_lv01");
			SysSkillLevelupVo dataById2 = BaseDataMgr.instance.GetDataById<SysSkillLevelupVo>(skillName + "_lv01");
			if (dataById != null)
			{
				if (dataById.coefficient.CompareTo("[]") != 0)
				{
					costID = dataById.coefficient;
				}
			}
			int num2 = this.CheckType(costID);
			if (num2 == 0)
			{
				this.transMana.gameObject.SetActive(false);
			}
			else
			{
				this.typeRise.text = ((num2 != 1) ? "法术加成" : "物理加成");
				this.manaCost.color = ((num2 != 1) ? this.magicDamage : this.physicDamage);
				text2 = this.CheckMana(costID);
				this.transMana.gameObject.SetActive(true);
			}
			if (dataById2 != null)
			{
				num = dataById2.skill_levelmax;
			}
			for (int num3 = 0; num3 != num; num3++)
			{
				dataById2 = BaseDataMgr.instance.GetDataById<SysSkillLevelupVo>(skillName + "_lv0" + (num3 + 1));
				text = text + dataById2.cd + "/";
			}
			text = text.TrimEnd(new char[]
			{
				'/'
			});
			this.coolDown.text = text;
			this.manaCost.text = text2;
		}

		private string CheckMana(string costID)
		{
			if (costID.CompareTo("[]") == 0 || string.IsNullOrEmpty(costID))
			{
				Singleton<TipView>.Instance.ShowViewSetText(this.heroNPC + "下的levelup表或skillmain中的‘damage’字段为空!!!", 1f);
				return string.Empty;
			}
			string result = string.Empty;
			SysSkillDamageVo dataById = BaseDataMgr.instance.GetDataById<SysSkillDamageVo>(costID);
			string[] array = new string[0];
			array = dataById.formula.Split(new char[]
			{
				'|'
			});
			if (array.Length > 2)
			{
				result = array[2];
			}
			return result;
		}

		private int CheckType(string costID)
		{
			if (costID.CompareTo("[]") == 0 || string.IsNullOrEmpty(costID))
			{
				return 0;
			}
			int result = 0;
			SysSkillDamageVo dataById = BaseDataMgr.instance.GetDataById<SysSkillDamageVo>(costID);
			string[] array = new string[0];
			array = dataById.formula.Split(new char[]
			{
				'|'
			});
			if (array.Length > 1)
			{
				int num = int.Parse(array[0]);
				if ((num == 2 || num == 3 || num == 21 || num == 25 || num == 28) && array.Length > 4)
				{
					result = int.Parse(array[4]);
				}
			}
			return result;
		}
	}
}
