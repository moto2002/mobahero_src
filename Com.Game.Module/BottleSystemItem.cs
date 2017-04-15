using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class BottleSystemItem : MonoBehaviour
	{
		[SerializeField]
		private Transform item;

		[SerializeField]
		private UISprite frame;

		[SerializeField]
		private UISprite icon_S;

		[SerializeField]
		private UITexture icon_T;

		private byte cardindex;

		private int position;

		private int level;

		private int itemType;

		private int itemID;

		private int itemCount;

		private int width;

		private int height;

		private bool isGot;

		private bool isNowGot;

		private bool isItemID;

		private bool isSprite;

		private string icon;

		private string icon_detail;

		private string type;

		private new string name;

		public Callback<GameObject, bool> DragCallBack;

		public byte CardIndex
		{
			get
			{
				return this.cardindex;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		public int Level
		{
			get
			{
				return this.level;
			}
		}

		public int ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		public int ItemID
		{
			get
			{
				return this.itemID;
			}
		}

		public int ItemCount
		{
			get
			{
				return this.itemCount;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
		}

		public bool IsGot
		{
			get
			{
				return this.isGot;
			}
		}

		public bool IsNowGot
		{
			get
			{
				return this.isNowGot;
			}
			set
			{
				this.isNowGot = value;
			}
		}

		public bool IsItemID
		{
			get
			{
				return this.isItemID;
			}
		}

		public string Icon
		{
			get
			{
				return this.icon;
			}
		}

		public string Icon_detail
		{
			get
			{
				return this.icon_detail;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public void Init(int level, int idx)
		{
			this.position = idx;
			this.ParseDetail(level);
			this.ReShape();
			UIEventListener.Get(base.transform.gameObject).onMobileHover = new UIEventListener.BoolDelegate(this.DragItem);
		}

		private void ParseDetail(int level)
		{
			int num = Tools_ParsePrice.ParseCollectRange(level, false);
			if (num == 0)
			{
				num = 10;
			}
			string text = string.Empty;
			string[] array = new string[]
			{
				string.Empty
			};
			text = BaseDataMgr.instance.GetDataById<SysMagicbottleLevelVo>(num.ToString()).viewItems;
			if (!string.IsNullOrEmpty(text))
			{
				array = text.Split(new char[]
				{
					','
				});
			}
			if (array != null && array.Length != 0)
			{
				this.TableSource(array[this.position]);
				this.AllotFrame(array[this.position]);
			}
		}

		private void TableSource(string position)
		{
			this.isSprite = false;
			SysMagicbottleGoodViewVo dataById = BaseDataMgr.instance.GetDataById<SysMagicbottleGoodViewVo>(position);
			string[] array = dataById.type.Split(new char[]
			{
				'|'
			});
			this.itemType = int.Parse(array[0]);
			if (array != null && 1 <= array.Length)
			{
				string text = array[0];
				switch (text)
				{
				case "1":
				{
					SysCurrencyVo dataById2 = BaseDataMgr.instance.GetDataById<SysCurrencyVo>(array[1]);
					this.type = dataById2.name;
					this.icon = dataById.viewIcon;
					this.icon_detail = dataById.loadingIcon;
					this.width = 90;
					this.height = 90;
					break;
				}
				case "2":
				{
					string text2 = string.Empty;
					text2 = array[1];
					if ("0" == text2)
					{
						this.type = LanguageManager.Instance.GetStringById("Currency_Rune");
						this.isSprite = true;
						this.width = 74;
						this.height = 86;
					}
					else
					{
						this.type = LanguageManager.Instance.GetStringById("Customization_Type_" + text2);
						this.isSprite = false;
						this.width = 90;
						this.height = 90;
					}
					this.name = LanguageManager.Instance.GetStringById(dataById.viewName);
					this.icon = dataById.viewIcon;
					this.icon_detail = dataById.loadingIcon;
					break;
				}
				case "3":
				{
					string text3 = array[1];
					switch (text3)
					{
					case "1":
						this.icon = dataById.viewIcon;
						this.icon_detail = dataById.loadingIcon;
						this.type = LanguageManager.Instance.GetStringById("Currency_Hero");
						this.name = LanguageManager.Instance.GetStringById(dataById.viewName);
						this.width = 90;
						this.height = 90;
						break;
					case "2":
						this.icon = dataById.viewIcon;
						this.icon_detail = dataById.loadingIcon;
						this.type = LanguageManager.Instance.GetStringById("Currency_Skin");
						this.name = LanguageManager.Instance.GetStringById(dataById.viewName);
						this.width = 90;
						this.height = 90;
						break;
					case "3":
						this.icon = dataById.viewIcon;
						this.icon_detail = dataById.loadingIcon;
						this.width = 90;
						this.height = 90;
						break;
					case "4":
						this.icon = dataById.viewIcon;
						this.icon_detail = dataById.loadingIcon;
						this.width = 90;
						this.height = 90;
						break;
					}
					break;
				}
				}
			}
		}

		private void ReShape()
		{
			this.icon_T.gameObject.SetActive(true);
			this.icon_S.gameObject.SetActive(false);
			this.icon_T.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			if (this.isSprite)
			{
				this.icon_T.gameObject.SetActive(false);
				this.icon_S.gameObject.SetActive(true);
				this.icon_S.spriteName = this.icon;
			}
			else
			{
				this.icon_T.mainTexture = ResourceManager.Load<Texture>(this.icon, true, true, null, 0, false);
				this.icon_T.SetDimensions(this.width, this.height);
			}
		}

		private void DragItem(GameObject obj, bool isIn)
		{
			if (null != obj)
			{
				this.DragCallBack(obj, isIn);
			}
		}

		private void AllotFrame(string position)
		{
			SysMagicbottleGoodViewVo dataById = BaseDataMgr.instance.GetDataById<SysMagicbottleGoodViewVo>(position);
			int quality = dataById.quality;
			if (quality != 0)
			{
				this.frame.spriteName = "Magic_bottle_icons_frame_0" + quality;
			}
		}
	}
}
