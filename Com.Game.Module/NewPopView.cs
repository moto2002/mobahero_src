using GUIFramework;
using MobaHeros;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class NewPopView : BaseView<NewPopView>
	{
		private UIGrid btnGrid;

		private GameObject btnOK;

		private GameObject btnCancel;

		private UILabel textOK;

		private UILabel textCancel;

		private UILabel content;

		private UILabel title;

		private Task task;

		private PopViewParam param;

		private bool valid;

		public NewPopView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Common/NewPopView");
		}

		public override void Init()
		{
			base.Init();
			this.btnGrid = this.transform.Find("Anchor/Panel/Btns").GetComponent<UIGrid>();
			this.btnOK = this.transform.Find("Anchor/Panel/Btns/Btn_OK").gameObject;
			this.btnCancel = this.transform.Find("Anchor/Panel/Btns/Btn_Cancel").gameObject;
			this.textOK = this.transform.Find("Anchor/Panel/Btns/Btn_OK/Label").GetComponent<UILabel>();
			this.textCancel = this.transform.Find("Anchor/Panel/Btns/Btn_Cancel/Label").GetComponent<UILabel>();
			this.content = this.transform.Find("Anchor/Panel/Text/Content").GetComponent<UILabel>();
			this.title = this.transform.Find("Anchor/Panel/Text/Title").GetComponent<UILabel>();
			UIEventListener.Get(this.btnOK).onClick = new UIEventListener.VoidDelegate(this.OnBtnOk);
			UIEventListener.Get(this.btnCancel).onClick = new UIEventListener.VoidDelegate(this.OnBtnCancel);
			this.AnimationRoot = this.gameObject;
			this.param = new PopViewParam();
			this.valid = true;
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23011, new MobaMessageFunc(this.SetParam));
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23011, new MobaMessageFunc(this.SetParam));
		}

		public override void HandleBeforeCloseView()
		{
			base.HandleBeforeCloseView();
			Task.Clear(ref this.task);
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.CreateTask();
			this.gameObject.layer = LayerMask.NameToLayer("UIEffect");
		}

		private void CreateTask()
		{
			Task.Clear(ref this.task);
			if (this.param != null && this.param.Task != null)
			{
				this.task = new Task(this.param.Task.Run(this.param, this), true);
			}
		}

		private void SetParam(MobaMessage msg)
		{
			this.SetParam(msg.Param as PopViewParam);
		}

		public void SetParam(PopViewParam p)
		{
			if (p == null)
			{
				return;
			}
			this.param = p;
			this.CreateTask();
			this.RefreshUI();
			AutoTestController.InvokeTestLogic(AutoTestTag.All, delegate
			{
				this.OnBtnOk(null);
			}, 1f);
		}

		public override void RefreshUI()
		{
			if (this.valid)
			{
				this.title.text = this.param.Title;
				this.content.text = this.param.Content;
				this.textOK.text = this.param.OK;
				this.textCancel.text = this.param.Cancel;
				this.btnCancel.SetActive(this.param.ShowOne);
				this.btnGrid.Reposition();
			}
		}

		public void SetContent(string text)
		{
			if (this.valid)
			{
				this.content.text = text;
			}
		}

		public void SetButtonText(string text, bool ok)
		{
			if (this.valid)
			{
				((!ok) ? this.textCancel : this.textOK).text = text;
			}
		}

		private void OnBtnCancel(GameObject button = null)
		{
			CtrlManager.CloseWindow(WindowID.NewPopView);
			this.Func_CallBack(EPopViewRet.eCancel);
		}

		private void OnBtnOk(GameObject obj = null)
		{
			CtrlManager.CloseWindow(WindowID.NewPopView);
			this.Func_CallBack(EPopViewRet.eOk);
		}

		private void Func_CallBack(EPopViewRet e)
		{
			if (this.param != null)
			{
				if (this.param.Callback_enum != null)
				{
					this.param.Callback_enum(EPopViewRet.eOk);
				}
				else if (this.param.Callback_void != null)
				{
					this.param.Callback_void();
				}
				else if (this.param.Callback_bool != null)
				{
					this.param.Callback_bool(e == EPopViewRet.eOk);
				}
			}
		}
	}
}
