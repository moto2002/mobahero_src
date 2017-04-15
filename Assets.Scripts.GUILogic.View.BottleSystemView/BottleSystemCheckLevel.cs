using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BottleSystemView
{
	[RequireComponent(typeof(TweenRotation), typeof(TweenScale))]
	public class BottleSystemCheckLevel : MonoBehaviour
	{
		private Transform viewFx;

		private Dictionary<string, SysMagicbottleExpVo> bottle_info;

		private SpriteRenderer sp;

		private TweenRotation tween_rotation;

		private TweenScale tween_scale;

		private int lastLevel;

		private int lastRange;

		private bool isOnce;

		private string lastIcon;

		public Transform pathX;

		private Task t_playview;

		private Task t_pathX;

		[SerializeField]
		private Transform[] bottleFX;

		private void Awake()
		{
			this.Initialize();
		}

		private void OnEnable()
		{
			if (null != this.viewFx)
			{
				this.viewFx.gameObject.SetActive(false);
			}
			base.StopAllCoroutines();
			for (int i = 0; i < this.pathX.childCount; i++)
			{
				this.pathX.GetChild(i).gameObject.SetActive(false);
			}
			this.Register();
		}

		private void OnDisable()
		{
			this.Unregister();
			base.StopAllCoroutines();
			for (int i = 0; i < this.pathX.childCount; i++)
			{
				this.pathX.GetChild(i).gameObject.SetActive(false);
			}
		}

		private void Register()
		{
			MobaMessageManager.RegistMessage((ClientMsg)21041, new MobaMessageFunc(this.ActionOnceAddExp));
			MobaMessageManager.RegistMessage((ClientMsg)21042, new MobaMessageFunc(this.ActionLongAddExp));
			MobaMessageManager.RegistMessage((ClientMsg)21043, new MobaMessageFunc(this.ActionOnDraw));
			MobaMessageManager.RegistMessage((ClientMsg)21044, new MobaMessageFunc(this.ActionOnLevelUp));
			MobaMessageManager.RegistMessage((ClientMsg)23053, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.RegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.RegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		private void Unregister()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)21041, new MobaMessageFunc(this.ActionOnceAddExp));
			MobaMessageManager.UnRegistMessage((ClientMsg)21042, new MobaMessageFunc(this.ActionLongAddExp));
			MobaMessageManager.UnRegistMessage((ClientMsg)21043, new MobaMessageFunc(this.ActionOnDraw));
			MobaMessageManager.UnRegistMessage((ClientMsg)21044, new MobaMessageFunc(this.ActionOnLevelUp));
			MobaMessageManager.UnRegistMessage((ClientMsg)23053, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.UnRegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.UnRegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
			this.bottle_info = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleExpVo>();
			this.sp = base.GetComponent<SpriteRenderer>();
			this.tween_rotation = base.GetComponent<TweenRotation>();
			this.tween_scale = base.GetComponent<TweenScale>();
			UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OpenBottleBook);
		}

		private void InitUI()
		{
			this.viewFx = Singleton<BottleSystemView>.Instance.transform.Find("FxEffects/view");
			this.lastLevel = ModelManager.Instance.Get_BottleData_Level();
			this.lastRange = Tools_ParsePrice.Level_Check<SysMagicbottleExpVo>(this.lastLevel, this.bottle_info, false);
			this.lastIcon = BaseDataMgr.instance.GetDataById<SysMagicbottleExpVo>(this.lastRange.ToString()).largeIcon;
			Sprite sprite = Resources.Load<Sprite>("Texture/MagicBottleHD/" + this.lastIcon);
			this.sp.sprite = sprite;
			this.ChangeBottleEffect(this.lastIcon);
			this.viewFx.gameObject.SetActive(false);
		}

		private void CheckLevel(int level)
		{
			string text = this.ReturnMaxExp(level);
			if (text != null && text.CompareTo(this.lastIcon) != 0)
			{
				Sprite sprite = Resources.Load<Sprite>("Texture/MagicBottleHD/" + text);
				this.sp.sprite = sprite;
				this.ChangeBottleEffect(text);
				this.lastIcon = text;
			}
		}

		private string ReturnMaxExp(int level)
		{
			this.bottle_info = BaseDataMgr.instance.GetTypeDicByType<SysMagicbottleExpVo>();
			int num = Tools_ParsePrice.Level_Check<SysMagicbottleExpVo>(level, this.bottle_info, false);
			if (this.bottle_info != null && num != this.lastRange)
			{
				string largeIcon = this.bottle_info[num.ToString()].largeIcon;
				this.lastLevel = level;
				this.lastRange = num;
				return largeIcon;
			}
			return this.lastIcon;
		}

		private void RotateScale()
		{
			if (!this.isOnce)
			{
				base.StartCoroutine("PathXCircle");
			}
			this.tween_rotation.style = UITweener.Style.Loop;
			this.tween_rotation.duration = 0.1f;
			this.tween_rotation.from = new Vector3(0f, 0f, -1f);
			this.tween_rotation.to = new Vector3(0f, 0f, 1f);
			this.tween_scale.style = UITweener.Style.Once;
			this.tween_scale.duration = 0.1f;
			this.tween_scale.from = new Vector3(100f, 100f, 100f);
			this.tween_scale.to = new Vector3(103f, 103f, 103f);
			this.tween_scale.ResetToBeginning();
			this.tween_scale.PlayForward();
			this.tween_rotation.PlayForward();
		}

		private void CancelRotateScale()
		{
			base.StopCoroutine("PathXCircle");
			this.tween_rotation.ResetToBeginning();
			this.tween_scale.ResetToBeginning();
			this.tween_rotation.enabled = false;
			this.tween_scale.style = UITweener.Style.Once;
			this.tween_scale.duration = 0.1f;
			this.tween_scale.from = new Vector3(103f, 103f, 103f);
			this.tween_scale.to = new Vector3(100f, 100f, 100f);
			this.tween_scale.PlayForward();
		}

		private void ActionOnceAddExp(MobaMessage msg)
		{
			float time = 1f;
			if (msg != null)
			{
				time = (float)msg.Param;
			}
			List<int> list = this.RandomPath();
			for (int i = 0; i < this.pathX.childCount; i++)
			{
				this.pathX.GetChild(i).gameObject.SetActive(false);
			}
			this.pathX.GetChild(0).gameObject.SetActive(false);
			this.isOnce = true;
			base.Invoke("CancelRotateScale", time);
			this.pathX.GetChild(0).gameObject.SetActive(true);
			AudioMgr.Play("Play_XMP_Levelup", null, false, false);
			for (int j = 0; j < list.Count; j++)
			{
				this.pathX.GetChild(list[j]).gameObject.SetActive(true);
			}
			this.RotateScale();
			this.isOnce = false;
		}

		private void ActionLongAddExp(MobaMessage msg)
		{
			bool flag = true;
			if (msg != null)
			{
				flag = (bool)msg.Param;
			}
			if (flag)
			{
				this.RotateScale();
			}
			else
			{
				this.CancelRotateScale();
			}
		}

		private void ActionOnDraw(MobaMessage msg)
		{
			this.AfterDraw();
		}

		private void ActionOnLevelUp(MobaMessage msg)
		{
			if (msg != null)
			{
				int level = (int)msg.Param;
				this.CheckLevel(level);
			}
		}

		private void OnOpenView(MobaMessage msg)
		{
			if (msg != null)
			{
				MagicBottleData magicBottleData = (MagicBottleData)msg.Param;
				if (magicBottleData != null)
				{
					this.InitUI();
				}
			}
		}

		private void ChangeBottleEffect(string iconCom)
		{
			for (int i = 0; i < this.bottleFX.Length; i++)
			{
				string strB = string.Empty;
				string text = string.Empty;
				this.bottleFX[i].gameObject.SetActive(false);
				strB = iconCom.Substring(iconCom.Length - 2, 2);
				text = this.bottleFX[i].name.Substring(this.bottleFX[i].name.Length - 2, 2);
				if (text.CompareTo(strB) == 0)
				{
					this.bottleFX[i].gameObject.SetActive(true);
				}
			}
		}

		private List<int> RandomPath()
		{
			List<int> list = new List<int>();
			while (list.Count < 3)
			{
				int item = UnityEngine.Random.Range(1, 10);
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private void AfterDraw()
		{
			this.tween_scale.ResetToBeginning();
			this.tween_scale.style = UITweener.Style.Once;
			this.tween_scale.duration = 4f;
			this.tween_scale.from = new Vector3(100f, 100f, 100f);
			this.tween_scale.to = new Vector3(130f, 130f, 130f);
			AudioMgr.Play("Play_FX_OpenBottle", null, false, false);
			this.tween_scale.PlayForward();
			this.viewFx.gameObject.SetActive(false);
			this.viewFx.gameObject.SetActive(true);
			base.StartCoroutine("PlayView");
		}

		[DebuggerHidden]
		private IEnumerator PlayView()
		{
			BottleSystemCheckLevel.<PlayView>c__Iterator115 <PlayView>c__Iterator = new BottleSystemCheckLevel.<PlayView>c__Iterator115();
			<PlayView>c__Iterator.<>f__this = this;
			return <PlayView>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator PathXCircle()
		{
			BottleSystemCheckLevel.<PathXCircle>c__Iterator116 <PathXCircle>c__Iterator = new BottleSystemCheckLevel.<PathXCircle>c__Iterator116();
			<PathXCircle>c__Iterator.<>f__this = this;
			return <PathXCircle>c__Iterator;
		}

		private void OnPeerConnected(MobaMessage msg)
		{
			base.StopAllCoroutines();
			for (int i = 0; i < this.pathX.childCount; i++)
			{
				this.pathX.GetChild(i).gameObject.SetActive(false);
			}
		}

		private void OnPeerDisconnected(MobaMessage msg)
		{
			base.StopAllCoroutines();
			for (int i = 0; i < this.pathX.childCount; i++)
			{
				this.pathX.GetChild(i).gameObject.SetActive(false);
			}
			this.CancelRotateScale();
		}

		private void OpenBottleBook(GameObject go)
		{
			if (null != go && BottleViewCtrl.GetInstance().drawState == DrawState.Nothing)
			{
				CtrlManager.OpenWindow(WindowID.BottleBookView, null);
			}
		}
	}
}
