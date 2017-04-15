using Assets.Scripts.Model;
using GUIFramework;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class PropertyView : BaseView<PropertyView>
	{
		private CoroutineManager coroutine;

		private Task task_smodel;

		public string[] RecordPropertyHeroName = new string[0];

		private string heroNPC;

		private int skinID;

		private long heroID;

		public string HeroNpc
		{
			get
			{
				return this.heroNPC;
			}
			set
			{
				if (Singleton<PvpSelectHeroView>.Instance.gameObject != null && Singleton<PvpSelectHeroView>.Instance.gameObject.activeInHierarchy)
				{
					this.heroNPC = value;
				}
			}
		}

		public long HeroID
		{
			get
			{
				return this.heroID;
			}
		}

		public PropertyView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "PropertyView");
			this.WindowTitle = LanguageManager.Instance.GetStringById("HeroAltar_Title_HeroAltar");
		}

		public override void Init()
		{
			this.GetHeroList();
			this.coroutine = new CoroutineManager();
		}

		public override void RegisterUpdateHandler()
		{
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			this.Hidden(false);
			MobaMessageManager.RegistMessage((ClientMsg)21049, new MobaMessageFunc(this.OnClickArrow));
			MobaMessageManager.RegistMessage((ClientMsg)21046, new MobaMessageFunc(this.OnChangeNPC));
		}

		public override void CancelUpdateHandler()
		{
			this.Hidden(true);
			MobaMessageManager.UnRegistMessage((ClientMsg)21049, new MobaMessageFunc(this.OnClickArrow));
			MobaMessageManager.UnRegistMessage((ClientMsg)21046, new MobaMessageFunc(this.OnChangeNPC));
		}

		public override void RefreshUI()
		{
		}

		public override void HandleAfterOpenView()
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewBuySkinSuccess, null, false);
		}

		public void OnSkinMsg(MobaMessage msg)
		{
		}

		public override void Destroy()
		{
		}

		private void OnChangeNPC(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				this.coroutine.StopAllCoroutine();
				string text = string.Empty;
				text = (string)msg.Param;
				this.heroNPC = text;
				HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.heroNPC);
				if (heroInfoData != null)
				{
					this.heroID = heroInfoData.HeroId;
				}
				else
				{
					this.heroID = 0L;
				}
			}
		}

		private void OnClickArrow(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				bool flag = (bool)msg.Param;
				if (this.RecordPropertyHeroName.Length > 1)
				{
					int num = 0;
					string[] recordPropertyHeroName = this.RecordPropertyHeroName;
					for (int i = 0; i < recordPropertyHeroName.Length; i++)
					{
						if (recordPropertyHeroName[i] == this.heroNPC)
						{
							num = i;
							break;
						}
					}
					if (flag)
					{
						this.heroNPC = recordPropertyHeroName[(num + 1) % recordPropertyHeroName.Length];
					}
					else
					{
						this.heroNPC = recordPropertyHeroName[((num - 1 >= 0) ? (num - 1) : (num - 1 + recordPropertyHeroName.Length)) % recordPropertyHeroName.Length];
					}
					MobaMessageManagerTools.SendClientMsg(ClientV2C.sacriviewChangeHero, this.heroNPC, false);
				}
			}
		}

		private void GetHeroList()
		{
			if (Singleton<SacrificialView>.Instance.HeroinOrder != null)
			{
				this.RecordPropertyHeroName = Singleton<SacrificialView>.Instance.HeroinOrder.ToArray();
				if ((this.RecordPropertyHeroName == null || this.RecordPropertyHeroName.Length == 0) && CharacterDataMgr.instance.AllHeros != null)
				{
					this.RecordPropertyHeroName = CharacterDataMgr.instance.AllHeros.ToArray();
				}
			}
		}

		private void Hidden(bool isShow)
		{
			if (CtrlManager.GetCtrl<MainBgCtrl>(WindowID.MainBg) != null && CtrlManager.GetCtrl<MainBgCtrl>(WindowID.MainBg).uiWindow != null)
			{
				CtrlManager.GetCtrl<MainBgCtrl>(WindowID.MainBg).uiWindow.gameObj.SetActive(isShow);
			}
		}
	}
}
