using Assets.Scripts.Character.Control;
using Com.Game.Utils;
using GUIFramework;
using MobaHeros;
using MobaHeros.Pvp;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class TeamSignalView : BaseView<TeamSignalView>
	{
		private UISprite _bigRegion;

		public TeamSignalView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Pvp/TeamSignalView");
		}

		public override void Init()
		{
			this._bigRegion = this.transform.Find("big").GetComponent<UISprite>();
			UIEventListener.Get(this._bigRegion.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickAboveView);
		}

		public override void HandleBeforeCloseView()
		{
			base.HandleBeforeCloseView();
			this.EnableTouchController(true);
		}

		private void OnClickAboveView(GameObject go)
		{
			this.Log("OnClickAboveView");
			Vector3 mousePosition = Input.mousePosition;
			if (this.IsClickBadWindow(mousePosition))
			{
				this.Log("click ui");
				TeamSignalManager.End();
				return;
			}
			TeamSignalType curMode = TeamSignalManager.CurMode;
			if (curMode == TeamSignalType.Defense)
			{
				Units units = this.PickupTarget();
				if (units != null && units.isMyTeam)
				{
					TeamSignalManager.TrySendTeamTargetNotify(curMode, units.unique_id);
				}
			}
			else if (curMode == TeamSignalType.Fire || curMode == TeamSignalType.Converge)
			{
				Units units2 = this.PickupTarget();
				if (units2 != null && units2.isEnemy)
				{
					TeamSignalManager.TrySendTeamTargetNotify(TeamSignalType.Fire, units2.unique_id);
				}
				else
				{
					Vector3? vector = this.PickupPos();
					if (vector.HasValue)
					{
						TeamSignalManager.TrySendTeamPosNotify(TeamSignalType.Converge, vector.Value);
					}
				}
			}
			else
			{
				Vector3? vector2 = this.PickupPos();
				if (vector2.HasValue)
				{
					TeamSignalManager.TrySendTeamPosNotify(curMode, vector2.Value);
				}
			}
			TeamSignalManager.End();
		}

		private Vector3? PickupPos()
		{
			Ray ray = BattleCameraMgr.Instance.BattleCamera.ScreenPointToRay(Input.mousePosition);
			int layerMask = 1 << LayerMask.NameToLayer("Ground");
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 200f, layerMask))
			{
				this.Log("hit " + raycastHit.point);
				return new Vector3?(raycastHit.point);
			}
			return null;
		}

		private Units PickupTarget()
		{
			ScenePicker scenePicker = new ScenePicker(Input.mousePosition);
			return scenePicker.PickBestUnit((Units x) => (x.tag == "Hero" || x.tag == "Player" || x.tag == "Monster" || x.tag == "Building" || x.tag == "Home") && x.unique_id != Singleton<PvpManager>.Instance.MyHeroUniqueId && (x.tag != "Monster" || (x.tag == "Monster" && x.teamType == 2)));
		}

		private static Camera GetUICamera()
		{
			Transform transform = ViewTree.Instance.transform.FindChild("Camera");
			return (!transform) ? null : transform.GetComponent<Camera>();
		}

		private static bool IsChildOfWindow(Transform trans, IView[] windows)
		{
			TeamSignalView.<IsChildOfWindow>c__AnonStorey26A <IsChildOfWindow>c__AnonStorey26A = new TeamSignalView.<IsChildOfWindow>c__AnonStorey26A();
			<IsChildOfWindow>c__AnonStorey26A.comps = trans.GetComponentsInParent<TUIWindow>();
			int j;
			for (j = 0; j < <IsChildOfWindow>c__AnonStorey26A.comps.Length; j++)
			{
				if (<IsChildOfWindow>c__AnonStorey26A.comps[j])
				{
					IView view = Array.Find<IView>(windows, (IView x) => x != null && x.uiWindow == <IsChildOfWindow>c__AnonStorey26A.comps[j]);
					if (view != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static IView[] GetBadWindows()
		{
			return new IView[]
			{
				CtrlManager.GetCtrl<GoldView>(WindowID.GoldView),
				CtrlManager.GetCtrl<NewPopView>(WindowID.NewPopView),
				CtrlManager.GetCtrl<SkillView>(WindowID.SkillView),
				CtrlManager.GetCtrl<MiniMapView>(WindowID.MiniMapView),
				CtrlManager.GetCtrl<ShowEquipmentPanelView>(WindowID.ShowEquipmentPanelView)
			};
		}

		private bool IsClickBadWindow(Vector3 mousePos)
		{
			Camera uICamera = TeamSignalView.GetUICamera();
			if (!uICamera)
			{
				return false;
			}
			Ray ray = uICamera.ScreenPointToRay(mousePos);
			RaycastHit[] array = Physics.RaycastAll(ray, 1000f, LayerMask.NameToLayer("GUI"));
			IView[] badWindows = TeamSignalView.GetBadWindows();
			for (int i = 0; i < array.Length; i++)
			{
				if (TeamSignalView.IsChildOfWindow(array[i].transform, badWindows))
				{
					return true;
				}
			}
			return false;
		}

		private void EnableTouchController(bool enabled)
		{
			Units unit = MapManager.Instance.GetUnit(Singleton<PvpManager>.Instance.MyHeroUniqueId);
			if (!unit)
			{
				return;
			}
			BattleTouchController unitComponent = unit.GetUnitComponent<BattleTouchController>();
			if (unitComponent != null)
			{
				unitComponent.Enable(enabled);
			}
			else
			{
				ClientLogger.Error("cannot found BattleTouchController");
			}
		}

		private void Log(string msg)
		{
		}
	}
}
