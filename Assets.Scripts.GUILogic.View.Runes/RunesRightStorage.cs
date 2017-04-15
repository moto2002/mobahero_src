using Assets.Scripts.GUILogic.View.PropertyView;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.Runes
{
	public class RunesRightStorage : MonoBehaviour
	{
		private Transform transStorage;

		private Transform transFilterVariety;

		private Transform transFilterClassification;

		private List<UIToggle> btnFilterVariety;

		private List<UIToggle> btnFilterClassification;

		private object[] mgs;

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.runesviewChangeToggle
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

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
			this.btnFilterVariety = new List<UIToggle>();
			this.btnFilterClassification = new List<UIToggle>();
			this.transStorage = base.transform.Find("Storage");
			this.transFilterVariety = this.transStorage.Find("FilterVariety");
			this.transFilterClassification = this.transStorage.Find("FilterClassification");
			for (int num = 0; num != this.transFilterVariety.transform.childCount; num++)
			{
				this.btnFilterVariety.Add(this.transFilterVariety.transform.GetChild(num).GetComponent<UIToggle>());
				UIEventListener.Get(this.transFilterVariety.transform.GetChild(num).gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFilterVariety);
			}
			for (int num2 = 0; num2 != this.transFilterClassification.transform.childCount; num2++)
			{
				this.btnFilterClassification.Add(this.transFilterClassification.transform.GetChild(num2).GetComponent<UIToggle>());
				UIEventListener.Get(this.transFilterClassification.transform.GetChild(num2).gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFilterClassification);
			}
		}

		private void OnMsg_runesviewChangeToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				RunesFunctionType runesFunctionType = (RunesFunctionType)((int)msg.Param);
				this.transStorage.gameObject.SetActive(runesFunctionType == RunesFunctionType.Storage);
			}
		}

		private void ClickFilterClassification(GameObject obj)
		{
			if (null != obj)
			{
				string name = obj.name;
				if (name.CompareTo("All") == 0)
				{
					MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewFilterClassification, RunesClassification.All, false);
					for (int num = 0; num != this.btnFilterVariety.Count; num++)
					{
						this.btnFilterVariety[num].optionCanBeNone = true;
						this.btnFilterVariety[num].value = false;
						this.btnFilterVariety[num].optionCanBeNone = false;
					}
					return;
				}
				RunesClassification runesClassification = (RunesClassification)((int)Enum.Parse(typeof(RunesClassification), name));
				MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewFilterClassification, runesClassification, false);
			}
		}

		private void ClickFilterVariety(GameObject obj)
		{
			if (null != obj)
			{
				string name = obj.name;
				RunesVariety runesVariety = (RunesVariety)((int)Enum.Parse(typeof(RunesVariety), name));
				MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewFilterVariety, runesVariety, false);
			}
		}
	}
}
