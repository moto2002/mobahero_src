using Com.Game.Data;
using Com.Game.Manager;
using GUIFramework;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class BirthView : BaseView<BirthView>
	{
		private UISprite B_BG;

		private Transform B_Mode;

		private Transform B_UIEffect;

		private CoroutineManager B_coroutineManager = new CoroutineManager();

		private string HeroID;

		private GameObject Effect_Birth;

		private GameObject HeroObj;

		private ParticleSystem[] particle;

		public Animator[] animator;

		public BirthView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Birth/BirthView");
		}

		public override void Init()
		{
			base.Init();
			this.Effect_Birth = (Resources.Load("Prefab/Effects/UIEffect/FX_uihero_birth") as GameObject);
			this.B_BG = this.transform.Find("BG").GetComponent<UISprite>();
			this.B_Mode = this.transform.Find("Mode");
			this.B_UIEffect = this.transform.Find("UIEffect");
			UIEventListener.Get(this.B_BG.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBGBtn);
		}

		public override void RegisterUpdateHandler()
		{
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void RefreshUI()
		{
			this.UpdateView();
		}

		public override void Destroy()
		{
			this.HeroObj = null;
			this.B_coroutineManager = new CoroutineManager();
			base.Destroy();
		}

		private void UpdateView()
		{
			NGUITools.SetActiveChildren(this.B_Mode.gameObject, false);
			NGUITools.SetActiveChildren(this.B_UIEffect.gameObject, false);
			this.B_coroutineManager.StopAllCoroutine();
			this.B_coroutineManager.StartCoroutine(this.ShowEffectAndModel(), true);
		}

		[DebuggerHidden]
		private IEnumerator ShowEffectAndModel()
		{
			BirthView.<ShowEffectAndModel>c__Iterator112 <ShowEffectAndModel>c__Iterator = new BirthView.<ShowEffectAndModel>c__Iterator112();
			<ShowEffectAndModel>c__Iterator.<>f__this = this;
			return <ShowEffectAndModel>c__Iterator;
		}

		public void SetHeroID(string heroId)
		{
			this.HeroID = heroId;
		}

		public void Play()
		{
			this.particle = this.B_UIEffect.GetComponentsInChildren<ParticleSystem>();
			this.animator = this.B_UIEffect.GetComponentsInChildren<Animator>();
			for (int i = 0; i < this.particle.Length; i++)
			{
				this.particle[i].Play();
			}
			for (int j = 0; j < this.animator.Length; j++)
			{
				this.animator[j].speed = 1f;
				this.animator[j].Play(0, 0, 0f);
			}
		}

		public void Stop()
		{
			this.particle = this.B_UIEffect.GetComponentsInChildren<ParticleSystem>();
			this.animator = this.B_UIEffect.GetComponentsInChildren<Animator>();
			for (int i = 0; i < this.particle.Length; i++)
			{
				this.particle[i].Stop();
			}
			for (int j = 0; j < this.animator.Length; j++)
			{
				this.animator[j].speed = 0f;
				this.animator[j].Play(0, 0, 0f);
			}
		}

		private void ShowUnitEffect(GameObject obj)
		{
		}

		private string GetHeroModelName(string hero_id)
		{
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(hero_id);
			return heroMainData.model_id;
		}

		private GameObject GetHeroMode(string heroID)
		{
			string heroModelName = this.GetHeroModelName(heroID);
			this.HeroObj = ResourceManager.Load<GameObject>(heroModelName + "_smodel", true, true, null, 0, false);
			return this.HeroObj;
		}

		private void ChangeLayer(GameObject objct, string layer = "Unit")
		{
			objct.layer = LayerMask.NameToLayer(layer);
			Transform[] componentsInChildren = objct.transform.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				transform.gameObject.layer = LayerMask.NameToLayer(layer);
			}
		}

		private void ClickBGBtn(GameObject obj = null)
		{
			this.B_coroutineManager.StopAllCoroutine();
			CtrlManager.CloseWindow(WindowID.BirthView);
		}
	}
}
