using Com.Game.Data;
using Com.Game.Utils;
using GUIFramework;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class AttachedPropertyView : BaseView<AttachedPropertyView>
	{
		private AttachedPropertyMediator mediator;

		private SysBattleSceneVo _sceneData;

		public AttachedPropertyMediator Mediator
		{
			get
			{
				return this.mediator;
			}
		}

		public SysBattleSceneVo SceneData
		{
			get
			{
				return this._sceneData;
			}
			set
			{
				this._sceneData = value;
			}
		}

		public AttachedPropertyView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Battle/AttachedPropertyView");
		}

		public override void Init()
		{
			base.Init();
			this.mediator = this.gameObject.GetComponent<AttachedPropertyMediator>();
			if (this.mediator == null)
			{
				ClientLogger.Error("no mediator to get!");
			}
			BattleAttrManager.Instance.Init();
		}

		public void Reset()
		{
			this.mediator.Start();
		}

		public override void Destroy()
		{
			if (this.mediator != null)
			{
				UnityEngine.Object.Destroy(this.mediator);
			}
			MyStatistic.Instance.End();
			base.Destroy();
		}
	}
}
