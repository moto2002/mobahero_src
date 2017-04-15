using Assets.Scripts.Model;
using Com.Game.Data;
using GUIFramework;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class BarragePlayerCtrl : BaseView<BarragePlayerCtrl>
	{
		internal class BarrageDataPacket
		{
			private string mUserSign;

			private string mText;

			private string mFormatId;

			private int mFontSize;

			private Color32 mColor;

			private Color32 mOutlineColor;

			private bool mIsGradient;

			private Color32 mGradientTopColor;

			private Color32 mGradientBottomColor;

			private string mSpriteName;

			private string mSummonerId;

			public string TextContent
			{
				get
				{
					return this.mText;
				}
			}

			public string content
			{
				get
				{
					return string.Format("{0}<{1}", this.mUserSign, this.mText);
				}
				set
				{
					if (value == null)
					{
						return;
					}
					string[] array = value.Split(new char[]
					{
						'<'
					});
					this.mUserSign = array[0];
					this.mText = array[1];
				}
			}

			public string formatId
			{
				get
				{
					return this.mFormatId;
				}
				set
				{
					if (this.mFormatId == value)
					{
						return;
					}
					this.mFormatId = value;
					this.SetFormat(this.mFormatId);
				}
			}

			public int fontSize
			{
				get
				{
					return this.mFontSize;
				}
				private set
				{
					this.mFontSize = value;
				}
			}

			public Color32 color
			{
				get
				{
					return this.mColor;
				}
			}

			public string colorStr
			{
				set
				{
					this.mColor = ModelManager.Instance.Get_ColorByString_X(value);
				}
			}

			public Color32 outlineColor
			{
				get
				{
					return this.mOutlineColor;
				}
			}

			private string outlineColorStr
			{
				set
				{
					this.mOutlineColor = ((!(value == "[]")) ? ModelManager.Instance.Get_ColorByString_X(value) : ModelManager.Instance.Get_ColorByString_X("0,0,0,160"));
				}
			}

			public bool isGradient
			{
				get
				{
					return this.mIsGradient;
				}
				set
				{
					this.mIsGradient = value;
				}
			}

			public Color32 gradientTop
			{
				get
				{
					return this.mGradientTopColor;
				}
			}

			private string gradientTopStr
			{
				set
				{
					this.mGradientTopColor = ModelManager.Instance.Get_ColorByString_X(value);
				}
			}

			public Color32 gradientBottom
			{
				get
				{
					return this.mGradientBottomColor;
				}
			}

			private string gradientBottomStr
			{
				set
				{
					this.mGradientBottomColor = ModelManager.Instance.Get_ColorByString_X(value);
				}
			}

			public string spriteName
			{
				get
				{
					return this.mSpriteName;
				}
				private set
				{
					this.mSpriteName = ((!(value == "[]")) ? value : null);
				}
			}

			public string summonerId
			{
				get
				{
					return this.mSummonerId;
				}
				set
				{
					this.mSummonerId = value;
				}
			}

			private void SetFormat(string _id)
			{
				SysBulletScreenFormatVo sysBulletScreenFormatVo = ModelManager.Instance.Get_BarrageFormat_X(_id);
				bool flag = sysBulletScreenFormatVo.isGradient == 1;
				this.isGradient = flag;
				this.colorStr = ((!flag) ? sysBulletScreenFormatVo.font_color : "255,255,255,255");
				this.gradientTopStr = ((!flag) ? "255,255,255,255" : sysBulletScreenFormatVo.gradient_top);
				this.gradientBottomStr = ((!flag) ? "255,255,255,255" : sysBulletScreenFormatVo.gradient_bottom);
				this.outlineColorStr = sysBulletScreenFormatVo.outline_color;
				this.fontSize = sysBulletScreenFormatVo.font_size;
				this.spriteName = sysBulletScreenFormatVo.bullet_screen_background;
			}
		}

		private GameObject mAnchorCache;

		private UIGrid mAnchorGrid;

		private GameObject mItemCache;

		private GameObject mItemPool;

		private CoroutineManager mCoroutineMgr;

		private float mRecordTime;

		private int mRecordTrack;

		private Task _checkEggTask;

		private readonly string[] _eggStrArrShangdian = new string[]
		{
			"商人<来啊来啊",
			"商人<新的装备正等待着主人",
			"商人<我给你看个宝贝~"
		};

		private readonly string[] _eggStrArrJushou = new string[]
		{
			"巨兽<要搞事？",
			"巨兽<你得向我证明你的力量",
			"巨兽<还不是得来抱我大腿，小东西~"
		};

		private readonly string[] _eggStrArrChishe = new string[]
		{
			"赤蛇<你怎么不去打巨兽？",
			"赤蛇<知天知地，胜乃不穷",
			"赤蛇<知彼知己，胜乃不殆"
		};

		private readonly string[] _eggStrArrLanbuff = new string[]
		{
			"蓝魔<法力！也可以叫做查克拉",
			"蓝魔<啊！魔法的力量！",
			"蓝魔<来啊！互相伤害啊！"
		};

		private readonly string[] _eggStrArrHongbuff = new string[]
		{
			"红魔<把惩戒放下，我们谈谈",
			"红魔<我诞生的时候还没有你呐！",
			"红魔<其实我这个红圈只是地下埋了个手电筒，不是什么魔法"
		};

		private readonly string[] _eggStrArrJinkuang = new string[]
		{
			"金矿<不就是钱吗？",
			"金矿<没有什么不是金币解决不了的问题",
			"金矿<有钱能使鬼推磨~"
		};

		private readonly string[] _eggStrArrXiaobing = new string[]
		{
			"小兵<就不能来个靠谱点的吗",
			"小兵<还好你不能反补",
			"小兵<是！"
		};

		public int lineNum
		{
			get
			{
				return this.mAnchorGrid.transform.childCount;
			}
			set
			{
				this.UpdateLine(value, 60f);
			}
		}

		public float spacing
		{
			set
			{
				this.UpdateLine(this.lineNum, value);
			}
		}

		public BarragePlayerCtrl()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Battle/BarragePlayerView");
		}

		public override void Init()
		{
			base.Init();
			this.mCoroutineMgr = new CoroutineManager();
			this.mItemCache = (Resources.Load("Prefab/UI/Battle/BarrageItem") as GameObject);
			this.mAnchorCache = this.transform.Find("AnchorCache").gameObject;
			this.mAnchorGrid = this.transform.Find("AnchorGrid").GetComponent<UIGrid>();
			this.mItemPool = this.transform.Find("Pool").gameObject;
			this.UpdateLine(this.lineNum, 60f);
		}

		public override void HandleAfterOpenView()
		{
			this.mCoroutineMgr.StartCoroutine(this.TryGettingBarrage(), true);
		}

		public override void HandleBeforeCloseView()
		{
			this.mCoroutineMgr.StopAllCoroutine();
		}

		private void UpdateLine(int num, float spacing)
		{
			int childCount = this.mAnchorGrid.transform.childCount;
			if (num == childCount || num <= 0 || num > 10)
			{
				return;
			}
			if (num < childCount)
			{
				for (int i = num; i < childCount; i++)
				{
					UnityEngine.Object.Destroy(this.mAnchorGrid.transform.GetChild(i).gameObject);
				}
			}
			else
			{
				for (int j = childCount; j < num; j++)
				{
					NGUITools.AddChild(this.mAnchorGrid.gameObject, this.mAnchorCache);
				}
			}
			this.mAnchorGrid.cellHeight = spacing;
			this.mAnchorGrid.Reposition();
		}

		[DebuggerHidden]
		private IEnumerator TryGettingBarrage()
		{
			BarragePlayerCtrl.<TryGettingBarrage>c__IteratorCC <TryGettingBarrage>c__IteratorCC = new BarragePlayerCtrl.<TryGettingBarrage>c__IteratorCC();
			<TryGettingBarrage>c__IteratorCC.<>f__this = this;
			return <TryGettingBarrage>c__IteratorCC;
		}

		public void LaunchMyBarrage(string _get)
		{
			BarragePlayerCtrl.BarrageDataPacket barrageDataPacket = this.DataUnpacking(_get);
			this.Launch(barrageDataPacket);
			this.CheckEgg(barrageDataPacket.TextContent);
		}

		public void LaunchLocalBarrage(string content)
		{
			if (!base.IsOpen)
			{
				return;
			}
			BarragePlayerCtrl.BarrageDataPacket data = this.DataUnpacking(content + "|255,206,0,190|0");
			this.Launch(data);
		}

		private bool ShowIt(ref BarragePlayerCtrl.BarrageDataPacket _data)
		{
			bool result = false;
			BarrageSceneType sceneType = Singleton<BarrageEmitterView>.Instance.sceneType;
			string summonerId = _data.summonerId;
			string b = ModelManager.Instance.Get_userData_X().SummonerId.ToString();
			if (summonerId == b)
			{
				return false;
			}
			switch (sceneType)
			{
			case BarrageSceneType.SelectHero:
			case BarrageSceneType.BattleIn:
			{
				List<ReadyPlayerSampleInfo> list = new List<ReadyPlayerSampleInfo>();
				if (Singleton<PvpManager>.Instance.IsObserver)
				{
					return true;
				}
				list = Singleton<PvpManager>.Instance.OurPlayers;
				foreach (ReadyPlayerSampleInfo current in list)
				{
					if (current.SummerId.ToString() == summonerId && !Singleton<StatisticView>.Instance.blockedSummonerList.Contains(summonerId))
					{
						result = true;
					}
				}
				break;
			}
			case BarrageSceneType.WatcherMode:
			case BarrageSceneType.WatcherMode_SelectHero:
				foreach (ReadyPlayerSampleInfo current2 in Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.LM))
				{
					if (current2.SummerId.ToString() == summonerId)
					{
						_data.formatId = "2";
						bool result2 = true;
						return result2;
					}
				}
				foreach (ReadyPlayerSampleInfo current3 in Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.BL))
				{
					if (current3.SummerId.ToString() == summonerId)
					{
						_data.formatId = "3";
						bool result2 = true;
						return result2;
					}
				}
				result = true;
				break;
			}
			return result;
		}

		private BarragePlayerCtrl.BarrageDataPacket DataUnpacking(string pData)
		{
			BarragePlayerCtrl.BarrageDataPacket barrageDataPacket = new BarragePlayerCtrl.BarrageDataPacket();
			string[] array = pData.Split(new char[]
			{
				'|'
			});
			if (array.Length != 3)
			{
				return null;
			}
			barrageDataPacket.content = array[0];
			barrageDataPacket.summonerId = array[2];
			if (array[1].Length > 2)
			{
				barrageDataPacket.formatId = "1";
				barrageDataPacket.colorStr = array[1];
			}
			else
			{
				barrageDataPacket.formatId = array[1];
			}
			return barrageDataPacket;
		}

		private BarrageItem GetItem()
		{
			int childCount = this.mItemPool.transform.childCount;
			BarrageItem component;
			if (childCount == 0)
			{
				component = NGUITools.AddChild(this.mItemPool, this.mItemCache).GetComponent<BarrageItem>();
			}
			else
			{
				component = this.mItemPool.transform.GetChild(childCount - 1).GetComponent<BarrageItem>();
			}
			component.gameObject.SetActive(true);
			return component;
		}

		private int Scheduling(BarragePlayerCtrl.BarrageDataPacket data)
		{
			int num = 0;
			int num2 = this.CheckFollowTrack(data);
			if (num2 != -1)
			{
				return num2;
			}
			List<int> list = new List<int>();
			for (int i = 0; i < this.mAnchorGrid.transform.childCount; i++)
			{
				list.Add(this.mAnchorGrid.transform.GetChild(i).childCount);
			}
			for (int j = 0; j < this.mAnchorGrid.transform.childCount; j++)
			{
				if (list[j] < list[num])
				{
					num = j;
				}
			}
			if (ModelManager.Instance.Get_userData_X().SummonerId.ToString() == data.summonerId)
			{
				this.mRecordTrack = num;
				this.mRecordTime = Time.time;
			}
			return num;
		}

		private int CheckFollowTrack(BarragePlayerCtrl.BarrageDataPacket data)
		{
			string a = ModelManager.Instance.Get_userData_X().SummonerId.ToString();
			if (a != data.summonerId)
			{
				return -1;
			}
			if (Time.time - this.mRecordTime >= 7f)
			{
				return -1;
			}
			return this.mRecordTrack;
		}

		private void BindData(BarragePlayerCtrl.BarrageDataPacket data, BarrageItem item)
		{
			string text = data.content;
			text = text.Replace("*#001*", "|");
			text = text.Replace("*#002*", "<");
			item.text = text;
			item.color = data.color;
			item.outlineColor = data.outlineColor;
			item.spriteName = data.spriteName;
			item.fontSize = data.fontSize;
			item.isGradient = data.isGradient;
			if (data.isGradient)
			{
				item.gradientTop = data.gradientTop;
				item.gradientBottom = data.gradientBottom;
			}
		}

		private void Launch(BarragePlayerCtrl.BarrageDataPacket data)
		{
			BarrageItem item = this.GetItem();
			int index = this.Scheduling(data);
			item.transform.parent = this.mAnchorGrid.transform.GetChild(index);
			item.tweenFrom = this.mAnchorGrid.transform.GetChild(index).position;
			this.BindData(data, item);
			item.tweenTo_x = item.tweenFrom_x - 2000f - (float)item.length;
			item.tweenDuration = (((float)item.length <= 600f) ? 6f : 8f);
			item.Play();
		}

		private void CheckEgg(string playerSendContent)
		{
			if (string.IsNullOrEmpty(playerSendContent))
			{
				return;
			}
			if (this._checkEggTask != null)
			{
				this.mCoroutineMgr.StopCoroutine(this._checkEggTask);
			}
			this._checkEggTask = this.mCoroutineMgr.StartCoroutine(this.CheckEgg_IEnumerator(playerSendContent), true);
		}

		[DebuggerHidden]
		private IEnumerator CheckEgg_IEnumerator(string playerSendContent)
		{
			BarragePlayerCtrl.<CheckEgg_IEnumerator>c__IteratorCD <CheckEgg_IEnumerator>c__IteratorCD = new BarragePlayerCtrl.<CheckEgg_IEnumerator>c__IteratorCD();
			<CheckEgg_IEnumerator>c__IteratorCD.playerSendContent = playerSendContent;
			<CheckEgg_IEnumerator>c__IteratorCD.<$>playerSendContent = playerSendContent;
			<CheckEgg_IEnumerator>c__IteratorCD.<>f__this = this;
			return <CheckEgg_IEnumerator>c__IteratorCD;
		}
	}
}
