using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewInfo : MonoBehaviour
	{
		private Transform heroInfo;

		private UILabel heroName;

		private UILabel heroType;

		private UILabel heroPosition;

		private UILabel heroFeatures;

		private UILabel heroDifficulty;

		private UISprite heroType_Sprite;

		private Transform skinPanel;

		private object[] mgs;

		private string heroNPC;

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
			this.skinPanel = base.transform.Find("SkinPanel");
			this.heroInfo = base.transform.Find("HeroInfo");
			this.heroName = base.transform.parent.Find("BottomAnchor/NameContent/Name").GetComponent<UILabel>();
			this.heroType = base.transform.parent.Find("BottomAnchor/NameContent/Type").GetComponent<UILabel>();
			this.heroType_Sprite = base.transform.parent.Find("BottomAnchor/NameContent/Texture").GetComponent<UISprite>();
			this.heroPosition = this.heroInfo.Find("Info/Text/CombatPosition/Label").GetComponent<UILabel>();
			this.heroFeatures = this.heroInfo.Find("Info/Text/HeroFeatures/Label").GetComponent<UILabel>();
			this.heroDifficulty = this.heroInfo.Find("Info/Text/Difficulty/Label").GetComponent<UILabel>();
		}

		private void OnMsg_propviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				PropertyType propertyType = (PropertyType)((int)msg.Param);
				this.heroInfo.gameObject.SetActive(propertyType == PropertyType.Info);
			}
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
					this.ChangeTxt();
				}
			}
		}

		private void ChangeTxt()
		{
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.heroNPC);
			if (LanguageManager.Instance.GetStringById(heroMainData.combat_positioning).Contains("|"))
			{
				this.heroPosition.text = LanguageManager.Instance.GetStringById(heroMainData.combat_positioning).Split(new char[]
				{
					'|'
				})[1];
			}
			else
			{
				this.heroPosition.text = "名字特点难度等待策划配表";
			}
			this.heroFeatures.text = LanguageManager.Instance.GetStringById(heroMainData.hero_feature);
			this.heroDifficulty.text = LanguageManager.Instance.GetStringById(heroMainData.control_difficulty);
			this.heroName.text = LanguageManager.Instance.GetStringById(heroMainData.name);
			this.heroType_Sprite.spriteName = this.GetHeroTextureType(heroMainData.character_type);
			this.heroType.text = PropViewInfo.ReturnType(heroMainData.character_type);
		}

		private string GetHeroTextureType(int type)
		{
			string result = string.Empty;
			if (type == 1)
			{
				result = "public_icon_02";
			}
			else if (type == 2)
			{
				result = "public_icon_03";
			}
			else if (type == 3)
			{
				result = "public_icon_04";
			}
			return result;
		}

		private static string ReturnType(int type)
		{
			string result = string.Empty;
			if (type == 1)
			{
				result = LanguageManager.Instance.GetStringById("HeroAltar_PowHero");
			}
			else if (type == 2)
			{
				result = LanguageManager.Instance.GetStringById("HeroAltar_AgiHero");
			}
			else if (type == 3)
			{
				result = LanguageManager.Instance.GetStringById("HeroAltar_IntHero");
			}
			return result;
		}
	}
}
