using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.HomeChatView
{
	public class ChatItem : MonoBehaviour
	{
		private const string SPACE7 = "       ";

		private const string SPACE6 = "      ";

		[SerializeField]
		private UISprite normalChat;

		[SerializeField]
		private UISprite headPortrait;

		[SerializeField]
		private UISprite frame;

		[SerializeField]
		private UILabel nickName;

		[SerializeField]
		private UILabel chatContent;

		[SerializeField]
		private UIAtlas atlas;

		private List<EmotionData> listEmotionData = new List<EmotionData>();

		public Callback<GameObject> ClickCallBack;

		private int index;

		private int line = 1;

		private string strFinal;

		private ChatType chatType = ChatType.Global;

		private UserData userdata;

		private AgentBaseInfo agentInfoData;

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public int Line
		{
			get
			{
				return this.line;
			}
		}

		public string StrFinal
		{
			get
			{
				return this.strFinal;
			}
		}

		public ChatType GetChatType
		{
			get
			{
				return this.chatType;
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		public void Init(AgentBaseInfo abinfo, string str, int idx, bool isSelf, ChatType type)
		{
			UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowSomeDetails);
			this.agentInfoData = abinfo;
			this.chatType = type;
			this.ParseData(str);
			this.normalChat.height = ((this.line != 1) ? 114 : 62);
			if (isSelf)
			{
				this.normalChat.spriteName = ((this.line != 1) ? "Home_chatting_talking_frame_05" : "Home_chatting_talking_frame_06");
			}
			else
			{
				this.normalChat.spriteName = ((this.line != 1) ? "Home_chatting_talking_frame_01" : "Home_chatting_talking_frame_02");
			}
			if (this.chatType == ChatType.VIP && !isSelf)
			{
				this.normalChat.spriteName = ((this.line != 1) ? "Home_chatting_talking_frame_03" : "Home_chatting_talking_frame_04");
			}
			if (type != ChatType.GM)
			{
				string text = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(abinfo.head.ToString()).headportrait_icon;
				if (text != null)
				{
					this.headPortrait.spriteName = text;
				}
				text = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(abinfo.headFrame.ToString()).pictureframe_icon;
				if (text != null)
				{
					this.frame.spriteName = text;
				}
			}
			this.nickName.text = abinfo.NickName;
			this.nickName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(abinfo.CharmRankvalue);
			this.chatContent.text = this.strFinal;
			this.GenerateEmotion();
		}

		private void ShowSomeDetails(GameObject go)
		{
			Singleton<HomeChatview>.Instance.playerInfoController.ShowPlayer(this.agentInfoData);
			if (this.ClickCallBack != null)
			{
				this.ClickCallBack(go);
			}
		}

		private void ParseData(string str)
		{
			this.listEmotionData.Clear();
			int length = str.Length;
			int num = -6;
			BetterList<string> listOfSprites = this.atlas.GetListOfSprites();
			for (int num2 = 0; num2 != length; num2++)
			{
				if (str[num2] == '#' && num2 + 4 < length && str[num2 + 1] == 'e')
				{
					EmotionData item = default(EmotionData);
					item.serialNumber = str.Substring(num2 + 2, 3);
					if (listOfSprites.Contains(item.serialNumber))
					{
						if (this.IsShowVIPStr(str, num2))
						{
							int num3 = (num != 0) ? (num + 6) : (num + 7);
							item.strContent = str.Substring(num3, num2 + 5 - num3).Replace(str.Substring(num2, 5), (num2 != 0) ? "      " : "       ");
							item.position = num2;
							item.row = 1;
							item.totalConSpace = item.strContent.Length;
							item.totalNonSpace = (float)(str.Substring(num3, num2 + 5 - num3).Length - 5) + 1.5f;
							this.listEmotionData.Add(item);
							num = num2;
							Regex regex = new Regex(str.Substring(num2, 5));
							str = regex.Replace(str, (num2 != 0) ? "      " : "       ", 1);
							length = str.Length;
						}
					}
				}
			}
			string text = string.Empty;
			if (this.listEmotionData != null && this.listEmotionData.Count != 0)
			{
				if (this.listEmotionData[this.listEmotionData.Count - 1].position == 0)
				{
					if (str.Length != 7)
					{
						text = str.Substring(this.listEmotionData[this.listEmotionData.Count - 1].position + 7);
					}
				}
				else
				{
					text = str.Substring(this.listEmotionData[this.listEmotionData.Count - 1].position + 6);
				}
			}
			else
			{
				text = str;
			}
			string text2 = null;
			for (int num4 = 0; num4 != this.listEmotionData.Count; num4++)
			{
				text2 += this.listEmotionData[num4].strContent;
			}
			if (!string.IsNullOrEmpty(text))
			{
				text2 += text;
			}
			this.CalculateLineFeed(text2);
		}

		private void CalculateLineFeed(string tarStr)
		{
			if (this.listEmotionData == null || this.listEmotionData.Count == 0)
			{
				int length = tarStr.Length;
				if (length > 15)
				{
					this.line = 2;
					tarStr = tarStr.Insert(15, "\n");
				}
				this.strFinal = tarStr;
				return;
			}
			float num = 0f;
			for (int num2 = 0; num2 != this.listEmotionData.Count; num2++)
			{
				num += this.listEmotionData[num2].totalNonSpace;
			}
			if (num > 15f)
			{
				this.line = 2;
				int num3 = 0;
				for (int num4 = this.listEmotionData.Count - 1; num4 != -1; num4--)
				{
					float num5 = 0f;
					for (int num6 = 0; num6 != num4; num6++)
					{
						num5 += this.listEmotionData[num6].totalNonSpace;
					}
					if (num5 <= 15f)
					{
						num3 = num4;
						break;
					}
				}
				if (this.listEmotionData.Count == num3 + 1)
				{
					int startIndex;
					if ((double)num - 1.5 > 15.0)
					{
						startIndex = Convert.ToInt32(num + (float)this.listEmotionData.Count * 4.5f) - Convert.ToInt32((double)num - 1.5 - 15.0 + 6.0);
					}
					else
					{
						startIndex = Convert.ToInt32(num + (float)this.listEmotionData.Count * 4.5f) - 6;
					}
					tarStr = tarStr.Insert(startIndex, "\n");
					this.strFinal = tarStr;
					EmotionData value = this.listEmotionData[num3];
					value.row = 2;
					value.position++;
					this.listEmotionData[num3] = value;
				}
				else
				{
					int position = this.listEmotionData[num3].position;
					tarStr = tarStr.Insert(position, "\n");
					this.strFinal = tarStr;
					for (int num7 = num3; num7 != this.listEmotionData.Count; num7++)
					{
						EmotionData value2 = this.listEmotionData[num7];
						value2.row = 2;
						value2.position++;
						this.listEmotionData[num7] = value2;
					}
				}
			}
			else
			{
				int length2 = tarStr.Substring(this.listEmotionData[this.listEmotionData.Count - 1].position + 6).Length;
				if (num + (float)length2 > 15f)
				{
					this.line = 2;
					int startIndex2 = Convert.ToInt32(num + (float)this.listEmotionData.Count * 4.5f) + Convert.ToInt32(15f - num);
					tarStr = tarStr.Insert(startIndex2, "\n");
				}
				this.strFinal = tarStr;
			}
		}

		private void GenerateEmotion()
		{
			BetterList<Vector3> betterList = new BetterList<Vector3>();
			BetterList<int> indices = new BetterList<int>();
			List<GameObject> list = new List<GameObject>();
			this.chatContent.pivot = UIWidget.Pivot.Left;
			this.chatContent.UpdateNGUIText();
			NGUIText.PrintCharacterPositions(this.chatContent.text, betterList, indices);
			int num = 0;
			for (int num2 = 0; num2 != this.listEmotionData.Count; num2++)
			{
				GameObject gameObject = new GameObject();
				UISprite uISprite = gameObject.AddComponent<UISprite>();
				uISprite.transform.parent = this.chatContent.transform.parent;
				uISprite.depth = this.chatContent.depth + 1;
				uISprite.transform.localScale = new Vector3(1f, 1f, 1f);
				uISprite.name = "emotion" + num2;
				uISprite.atlas = this.atlas;
				uISprite.spriteName = this.listEmotionData[num2].serialNumber;
				uISprite.SetDimensions(48, 48);
				if (this.listEmotionData != null && this.listEmotionData.Count > 1)
				{
					if (this.listEmotionData[0].position == 0 && num2 != 0)
					{
						num = -8;
					}
					else
					{
						num = 0;
					}
				}
				int num3;
				if (this.listEmotionData.Any((EmotionData col) => col.row == 2))
				{
					float y = betterList[this.listEmotionData.FirstOrDefault((EmotionData col) => col.row == 2).position * 2 - 1].y;
					float y2 = betterList[0].y;
					if (Mathf.Abs(y - y2) > 48f)
					{
						num3 = 48;
					}
					else
					{
						num3 = 0;
					}
				}
				else
				{
					num3 = 0;
				}
				int i = (2 * this.listEmotionData[num2].position != 0) ? (2 * this.listEmotionData[num2].position - 1) : 0;
				bool flag = this.listEmotionData[num2].row == 2;
				uISprite.transform.localPosition = new Vector3(this.chatContent.transform.localPosition.x, 0f, this.chatContent.transform.localPosition.z) + betterList[i] + new Vector3((float)(uISprite.width / 2 + -(float)this.chatContent.spacingX + ((!flag) ? num : 0)), (float)(-(float)this.chatContent.spacingY / 2) - 97.5f + (float)((!flag) ? 4 : num3), 0f);
			}
		}

		private bool IsShowVIPStr(string str, int i)
		{
			string text = str.Substring(i, 5);
			if (this.chatType != ChatType.VIP)
			{
				if (text.CompareTo("#e062") == 0)
				{
					return false;
				}
			}
			else if (i != 0 && text.CompareTo("#e062") == 0)
			{
				return false;
			}
			return true;
		}
	}
}
