using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class ScrificialCenter : MonoBehaviour
	{
		private Transform Type1;

		private UIGrid Type1_Grid;

		private SacrificialItem sacrificialItem;

		private UIScrollView HeroPanel;

		private Coroutine coroutine;

		private void Awake()
		{
			this.Initialize();
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
			if (this.coroutine != null)
			{
				base.StopAllCoroutines();
			}
			this.coroutine = null;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
			this.sacrificialItem = Resources.Load<SacrificialItem>("Prefab/UI/Sacrificial/SacrificialItem");
			this.Type1 = base.transform.Find("Scroll View/Type1");
			this.Type1_Grid = this.Type1.Find("Grid").GetComponent<UIGrid>();
			this.HeroPanel = base.transform.Find("Scroll View").GetComponent<UIScrollView>();
		}

		public void UpdateView(List<string> list1, List<string> list2, List<string> list3)
		{
			Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
			dictionary[1] = list1;
			dictionary[2] = list2;
			dictionary[3] = list3;
			if (this.coroutine != null)
			{
				base.StopCoroutine("UpdateHeroList");
			}
			this.coroutine = base.StartCoroutine("UpdateHeroList", dictionary);
		}

		[DebuggerHidden]
		private IEnumerator UpdateHeroList(Dictionary<int, List<string>> dic)
		{
			ScrificialCenter.<UpdateHeroList>c__Iterator16D <UpdateHeroList>c__Iterator16D = new ScrificialCenter.<UpdateHeroList>c__Iterator16D();
			<UpdateHeroList>c__Iterator16D.dic = dic;
			<UpdateHeroList>c__Iterator16D.<$>dic = dic;
			<UpdateHeroList>c__Iterator16D.<>f__this = this;
			return <UpdateHeroList>c__Iterator16D;
		}

		private void SetPosition(Transform transform)
		{
			if (transform.childCount == 0)
			{
				return;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				if (!transform.GetChild(i).gameObject.activeInHierarchy)
				{
					transform.GetChild(i).localPosition = Vector3.zero;
				}
			}
		}

		private void HeroNatureSacrificial(GameObject objct_1 = null)
		{
			Singleton<SacrificialView>.Instance.RecordName = objct_1.name;
			MobaMessageManagerTools.SendClientMsg(ClientC2V.OpenProperty, Singleton<SacrificialView>.Instance.RecordName, false);
		}
	}
}
