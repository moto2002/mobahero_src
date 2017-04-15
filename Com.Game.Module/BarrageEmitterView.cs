using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using GUIFramework;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using MobaServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class BarrageEmitterView : BaseView<BarrageEmitterView>
	{
		private const float CELL_WIDTH = 262f;

		private const float CELL_HEIGHT = 83f;

		private const float CELL_HALF_HEIGHT = 41.5f;

		private UIButton mPanelBtn;

		private UIAnchor anchor;

		private BoxCollider mCollider;

		private Vector3 mColliderCenter_Record = Vector3.one;

		private Vector3 mColliderSize_Record = Vector3.one;

		private GameObject mPopupPanel;

		private UIGrid mPopupGrid;

		private List<BarrageEmitItemCtrl> mGridBtnList;

		private List<Vector2[]> mGridBtnListBounds;

		private UISprite mGridBg;

		private UICamera mViewCamera;

		private UILabel mInputLabel;

		private GameObject mInputSubmit;

		private CoroutineManager mCoroutineMgr = new CoroutineManager();

		private CoroutineManager mCoroutineMgrForCD = new CoroutineManager();

		private Vector2 mPressEndPos = default(Vector2);

		private Task task_PressPathRecorder;

		private Task task_PressTrigger;

		private Task task_PressBehavior;

		public GameObject mItemCache;

		private BarrageEmitItemCtrl mAnchorCache;

		private string mBarragePrefix;

		private GameObject mCDBg;

		private UISprite mCD;

		private UILabel mCDLabel;

		private UILabel _eggTipLabel;

		public UIToggle talkToggle;

		public GameObject fxUILoading;

		private BarrageSceneType mSceneType = BarrageSceneType.BattleIn;

		private static float SENDERBTN_HEIGHT = 94f;

		private bool isPressMode;

		private bool panelActiveRecord;

		private Stack<float> mEmittingTimesRecord = new Stack<float>();

		public string barragePrefix
		{
			get
			{
				return this.mBarragePrefix;
			}
		}

		public BarrageSceneType sceneType
		{
			get
			{
				return this.mSceneType;
			}
			set
			{
				if (value != this.mSceneType)
				{
					this.mSceneType = value;
					this.ResetSceneType();
				}
			}
		}

		public BarrageEmitterView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Battle/BarrageEmitterView");
		}

		public override void Init()
		{
			base.Init();
			this.BindObject();
		}

		private void BindBtn()
		{
			if (this.sceneType == BarrageSceneType.SelectHero || this.sceneType == BarrageSceneType.WatcherMode_SelectHero)
			{
				this.transform.Find("Button/Button1").gameObject.SetActive(true);
				this.mPanelBtn = this.transform.Find("Button/Button1").GetComponent<UIButton>();
				this.transform.Find("Button/Button2").gameObject.SetActive(false);
			}
			else
			{
				this.transform.Find("Button/Button2").gameObject.SetActive(true);
				this.mPanelBtn = this.transform.Find("Button/Button2").GetComponent<UIButton>();
				this.transform.Find("Button/Button1").gameObject.SetActive(false);
			}
			this.mCollider = this.mPanelBtn.GetComponent<BoxCollider>();
			this.mCDBg = this.mPanelBtn.transform.FindChild("CDBg").gameObject;
			this.mCD = this.mCDBg.transform.FindChild("CD").GetComponent<UISprite>();
			this.mCDLabel = this.mCDBg.transform.FindChild("CDLabel").GetComponent<UILabel>();
			UIEventListener.Get(this.mPanelBtn.gameObject).onPress = new UIEventListener.BoolDelegate(this.OnPressPanelBtn);
		}

		protected override void BindObject()
		{
			this.mItemCache = ResourceManager.LoadPath<GameObject>("Prefab/UI/Battle/BarrageItem", null, 0);
			this.anchor = this.transform.Find("Button").GetComponent<UIAnchor>();
			this.BindBtn();
			this.mPopupPanel = this.transform.Find("PopupPanel").gameObject;
			this.mAnchorCache = this.mPopupPanel.transform.FindChild("AnchorCache").GetComponent<BarrageEmitItemCtrl>();
			this.mPopupGrid = this.transform.Find("PopupPanel/PopupGrid").GetComponent<UIGrid>();
			this.mGridBg = this.mPopupPanel.transform.FindChild("Bg").GetComponent<UISprite>();
			this.mInputLabel = this.transform.Find("PopupPanel/InputBox/TextBox/Input").GetComponent<UILabel>();
			this.mInputSubmit = this.transform.Find("PopupPanel/InputBox/Btn").gameObject;
			this.talkToggle = this.transform.Find("TalkToggle").GetComponent<UIToggle>();
			this.fxUILoading = this.talkToggle.transform.Find("Fx_UI_loading_small").gameObject;
			this._eggTipLabel = this.mPopupPanel.transform.FindChild("EggTipLabel").GetComponent<UILabel>();
			this.mViewCamera = this.transform.GetComponentInParent<UICamera>();
			this.mGridBtnList = new List<BarrageEmitItemCtrl>();
			this.mGridBtnListBounds = new List<Vector2[]>();
			UIEventListener.Get(this.mInputSubmit).onClick = new UIEventListener.VoidDelegate(this.OnClickSubmit);
			UIEventListener.Get(this.mPopupPanel.transform.Find("PressMask").gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickMask);
		}

		public override void HandleAfterOpenView()
		{
			this.mPanelBtn.gameObject.SetActive(false);
			this.ResetSceneType();
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(Singleton<PvpManager>.Instance.BattleId.ToString());
		}

		public override void HandleBeforeCloseView()
		{
			this.mCoroutineMgr.StopAllCoroutine();
			this.mCoroutineMgrForCD.StopAllCoroutine();
		}

		public bool IsCharacter(char c)
		{
			return c > '\u007f';
		}

		private int WordsCounter(string _input)
		{
			int num = 0;
			char[] array = _input.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (this.IsCharacter(array[i]))
				{
					num += 2;
				}
				else
				{
					num++;
				}
			}
			return num;
		}

		private void ResetSceneType()
		{
			if (this.sceneType == BarrageSceneType.SelectHero || this.sceneType == BarrageSceneType.WatcherMode_SelectHero)
			{
				this.BindBtn();
				this.anchor.side = UIAnchor.Side.BottomLeft;
				this.anchor.enabled = true;
				this.mGridBg.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.mGridBg.transform.localPosition = new Vector3(10f, -417f, 0f);
				this.mPopupGrid.pivot = UIWidget.Pivot.BottomLeft;
				this.mPopupGrid.transform.localPosition = new Vector3(10f, -362f, 0f);
			}
			else
			{
				this.BindBtn();
				this.anchor.side = UIAnchor.Side.TopLeft;
				this.anchor.enabled = true;
				this.mGridBg.transform.localRotation = Quaternion.Euler(new Vector3(0f, -180f, 180f));
				this.mGridBg.transform.localPosition = new Vector3(0f, 299f, 0f);
				this.mPopupGrid.pivot = UIWidget.Pivot.TopLeft;
				this.mPopupGrid.transform.localPosition = new Vector3(0f, 242.48f, 0f);
			}
			this.InitData();
		}

		private void InitData()
		{
			CtrlManager.OpenWindow(WindowID.BarragePlayerView, null);
			Singleton<BarragePlayerCtrl>.Instance.lineNum = 3;
			ModelManager.Instance.BarrageQueue_Clear();
			this.mCoroutineMgr.StopAllCoroutine();
			this.mCoroutineMgr.StartCoroutine(this.UpdateUICompent(), true);
		}

		[DebuggerHidden]
		private IEnumerator UpdateUICompent()
		{
			BarrageEmitterView.<UpdateUICompent>c__IteratorC7 <UpdateUICompent>c__IteratorC = new BarrageEmitterView.<UpdateUICompent>c__IteratorC7();
			<UpdateUICompent>c__IteratorC.<>f__this = this;
			return <UpdateUICompent>c__IteratorC;
		}

		public static string DataPacking(string textContent, string textFormat = "1")
		{
			long summonerId = ModelManager.Instance.Get_userData_X().SummonerId;
			string arg = (!(textFormat == "1")) ? textFormat : ModelManager.Instance.Get_UserBarrageColor_X();
			return string.Format("{0}|{1}|{2}", textContent, arg, summonerId);
		}

		private string GetBarragePrefix()
		{
			string result;
			if (this.mSceneType != BarrageSceneType.BattleIn)
			{
				result = ModelManager.Instance.Get_userData_X().NickName;
			}
			else
			{
				int _userLobbyId = Singleton<PvpManager>.Instance.MyLobbyUserId;
				ReadyPlayerSampleInfo readyPlayerSampleInfo = Singleton<PvpManager>.Instance.PvpPlayers.FirstOrDefault((ReadyPlayerSampleInfo x) => x.newUid == _userLobbyId);
				string heroId = readyPlayerSampleInfo.heroInfo.heroId;
				result = LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetHeroMainData(heroId).name);
			}
			return result;
		}

		public static void SetFormat(string formatId, BarrageItem item)
		{
			SysBulletScreenFormatVo sysBulletScreenFormatVo = ModelManager.Instance.Get_BarrageFormat_X(formatId);
			item.fontSize = sysBulletScreenFormatVo.font_size;
			item.color = ModelManager.Instance.Get_ColorByString_X(sysBulletScreenFormatVo.font_color);
			item.outlineColor = ModelManager.Instance.Get_ColorByString_X(sysBulletScreenFormatVo.outline_color);
			item.spriteName = sysBulletScreenFormatVo.bullet_screen_background;
			if (sysBulletScreenFormatVo.isGradient == 1)
			{
				item.isGradient = true;
				item.gradientTop = ModelManager.Instance.Get_ColorByString_X(sysBulletScreenFormatVo.gradient_top);
				item.gradientBottom = ModelManager.Instance.Get_ColorByString_X(sysBulletScreenFormatVo.gradient_bottom);
			}
			else
			{
				item.isGradient = false;
			}
		}

		private void ExpandCollider()
		{
			this.mCollider.center = this.mColliderCenter_Record;
			this.mCollider.size = this.mColliderSize_Record;
		}

		private void ResetCollider()
		{
			this.mCollider.center = Vector3.zero;
			this.mCollider.size = Vector3.one * 92f;
		}

		[DebuggerHidden]
		private IEnumerator PressPathRecorder()
		{
			BarrageEmitterView.<PressPathRecorder>c__IteratorC8 <PressPathRecorder>c__IteratorC = new BarrageEmitterView.<PressPathRecorder>c__IteratorC8();
			<PressPathRecorder>c__IteratorC.<>f__this = this;
			return <PressPathRecorder>c__IteratorC;
		}

		private Vector2[] GetColliderCorners(Transform obj)
		{
			BoxCollider component = obj.GetComponent<BoxCollider>();
			if (component == null)
			{
				obj.GetComponentInChildren<BoxCollider>();
			}
			if (component == null)
			{
				return null;
			}
			Vector2 vector = obj.position;
			float x = component.bounds.extents.x;
			float y = component.bounds.extents.y;
			return new Vector2[]
			{
				new Vector2(vector.x - x, vector.y - y),
				new Vector2(vector.x - x, vector.y + y),
				new Vector2(vector.x + x, vector.y + y),
				new Vector2(vector.x + x, vector.y - y)
			};
		}

		private bool IsInBounds(Vector2 target, Vector2[] bounds)
		{
			return bounds != null && bounds.Length == 4 && (target.x > bounds[0].x && target.x < bounds[2].x && target.y > bounds[0].y) && target.y < bounds[2].y;
		}

		private int GetPressOnIndex(Vector2 posNow)
		{
			int result = -1;
			if (this.mGridBtnListBounds.Count == 0)
			{
				return -1;
			}
			if (posNow.x < this.mGridBtnListBounds[0][0].x || posNow.x > this.mGridBtnListBounds[0][2].x)
			{
				return -1;
			}
			for (int i = 0; i < this.mGridBtnListBounds.Count; i++)
			{
				if (this.mGridBtnListBounds[i] == null)
				{
					return -1;
				}
				if (posNow.y > this.mGridBtnListBounds[i][0].y && posNow.y < this.mGridBtnListBounds[i][1].y)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		private void OnClickSubmit(GameObject obj = null)
		{
			this.mInputLabel.text = FilterWorder.Instance.ReplaceKeyword(this.mInputLabel.text);
			string text = this.mInputLabel.text;
			int num = this.WordsCounter(text);
			if (num > 40)
			{
				Singleton<TipView>.Instance.ShowViewSetText("自定弹幕长度不可超过20个字", 1f);
				return;
			}
			text = text.Replace("|", "*#001*");
			text = text.Replace("<", "*#002*");
			if (string.IsNullOrEmpty(text) || text == "在这里输入你想发的话")
			{
				Singleton<TipView>.Instance.ShowViewSetText("消息为空", 1f);
				this.ForcedClosePanel();
				return;
			}
			text = FilterWorder.Instance.ReplaceKeyword(text);
			string msg = BarrageEmitterView.DataPacking(this.mBarragePrefix + "<" + text, "1");
			if (this.sceneType == BarrageSceneType.SelectHero || this.sceneType == BarrageSceneType.WatcherMode_SelectHero)
			{
				ModelManager.Instance.Send_C2PCaption_2GameServer(msg);
			}
			else
			{
				ModelManager.Instance.Send_C2PCaption(msg);
			}
			this.RecordEmitting();
			this.mInputLabel.GetComponent<UIInput>().value = string.Empty;
			this.mPressEndPos = Vector3.zero;
			this.ForcedClosePanel();
		}

		public void SubmitByEnter(GameObject obj = null)
		{
			if (Singleton<BarrageEmitterView>.Instance.gameObject != null && this.mPopupPanel.activeSelf)
			{
				if (string.IsNullOrEmpty(this.mInputLabel.text) || this.mInputLabel.text == "在这里输入你想发的话")
				{
					this.ForcedClosePanel();
					return;
				}
				this.OnClickSubmit(null);
			}
			else
			{
				this.OnPressPanelBtn(null, true);
			}
		}

		private void OnClickMask(GameObject obj = null)
		{
			this.mPressEndPos = Vector3.zero;
			this.ForcedClosePanel();
		}

		public void NewbieClickPanelBtn()
		{
			this.OnPressPanelBtn(null, true);
			this.OnPressPanelBtn(null, false);
		}

		[DebuggerHidden]
		private IEnumerator PressTimeTrigger()
		{
			BarrageEmitterView.<PressTimeTrigger>c__IteratorC9 <PressTimeTrigger>c__IteratorC = new BarrageEmitterView.<PressTimeTrigger>c__IteratorC9();
			<PressTimeTrigger>c__IteratorC.<>f__this = this;
			return <PressTimeTrigger>c__IteratorC;
		}

		[DebuggerHidden]
		private IEnumerator PressBehavior()
		{
			BarrageEmitterView.<PressBehavior>c__IteratorCA <PressBehavior>c__IteratorCA = new BarrageEmitterView.<PressBehavior>c__IteratorCA();
			<PressBehavior>c__IteratorCA.<>f__this = this;
			return <PressBehavior>c__IteratorCA;
		}

		private void ForcedClosePanel()
		{
			this.panelActiveRecord = true;
			this.OnPressPanelBtn(null, false);
		}

		private void OnPressPanelBtn(GameObject obj, bool isPress)
		{
			if (isPress)
			{
				if (this._eggTipLabel.alpha < 0.5f && PlayerPrefs.HasKey("BarrageEggCount"))
				{
					this._eggTipLabel.alpha = 0.85f;
				}
				this.mPressEndPos = Vector3.zero;
				this.isPressMode = false;
				this.mPanelBtn.normalSprite = "Barrage_btn_press";
				this.ExpandCollider();
				this.task_PressTrigger = this.mCoroutineMgr.StartCoroutine(this.PressTimeTrigger(), true);
				this.task_PressPathRecorder = this.mCoroutineMgr.StartCoroutine(this.PressPathRecorder(), true);
				this.task_PressBehavior = this.mCoroutineMgr.StartCoroutine(this.PressBehavior(), true);
				this.panelActiveRecord = this.mPopupPanel.activeInHierarchy;
				this.mPopupPanel.SetActive(true);
				MobaMessageManagerTools.SendClientMsg(ClientV2V.BattleController_Close, null, false);
			}
			else
			{
				this.CheckReleasePoint();
				this.mPanelBtn.normalSprite = "Barrage_btn_normal";
				if (this.task_PressTrigger != null)
				{
					this.mCoroutineMgr.StopCoroutine(this.task_PressTrigger);
					this.task_PressTrigger = null;
				}
				if (this.task_PressPathRecorder != null)
				{
					this.mCoroutineMgr.StopCoroutine(this.task_PressPathRecorder);
					this.task_PressPathRecorder = null;
				}
				if (this.task_PressBehavior != null)
				{
					this.mCoroutineMgr.StopCoroutine(this.task_PressBehavior);
					this.task_PressBehavior = null;
				}
				if (this.isPressMode || this.panelActiveRecord)
				{
					this.ResetCollider();
					this.mPopupPanel.SetActive(false);
					MobaMessageManagerTools.SendClientMsg(ClientV2V.BattleController_Open, null, false);
				}
			}
		}

		private void CheckReleasePoint()
		{
			int pressOnIndex = this.GetPressOnIndex(this.mPressEndPos);
			if (pressOnIndex != -1)
			{
				this.mGridBtnList[pressOnIndex].Send();
			}
		}

		public void RecordEmitting()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (this.mEmittingTimesRecord.Count >= 1)
			{
				float num = realtimeSinceStartup - this.mEmittingTimesRecord.Peek();
				if (num > 5f)
				{
					this.mEmittingTimesRecord.Clear();
				}
			}
			this.mEmittingTimesRecord.Push(realtimeSinceStartup);
			this.CheckTimes();
		}

		private void CheckTimes()
		{
			if (this.mEmittingTimesRecord.Count < 3)
			{
				return;
			}
			this.mEmittingTimesRecord.Clear();
			this.mCoroutineMgrForCD.StopAllCoroutine();
			this.mCoroutineMgrForCD.StartCoroutine(this.Run_EmitCD(), true);
		}

		private void FinishCD()
		{
			if (this.mCDBg != null)
			{
				this.mCDBg.SetActive(false);
			}
			this.mPressEndPos = Vector3.zero;
		}

		[DebuggerHidden]
		private IEnumerator Run_EmitCD()
		{
			BarrageEmitterView.<Run_EmitCD>c__IteratorCB <Run_EmitCD>c__IteratorCB = new BarrageEmitterView.<Run_EmitCD>c__IteratorCB();
			<Run_EmitCD>c__IteratorCB.<>f__this = this;
			return <Run_EmitCD>c__IteratorCB;
		}

		public void FlyOut()
		{
			this.transform.GetComponent<TweenPosition>().PlayForward();
		}

		public void FlyIn()
		{
			this.transform.GetComponent<TweenPosition>().PlayReverse();
		}
	}
}
