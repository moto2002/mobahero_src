using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BottleSystemView
{
	public class BottleViewBottomLeft : MonoBehaviour
	{
		private const int COUNT = 10;

		private const float X = -1090f;

		private const float Y = 25f;

		private UIGrid itemsGrid;

		private Transform details;

		private Transform itemBG;

		private BottleSystemItem bottlesystemitem;

		private BottleSystemItem temp;

		private void Awake()
		{
			this.Initialize();
		}

		private void OnEnable()
		{
			this.Register();
			this.details.gameObject.SetActive(false);
		}

		private void OnDisable()
		{
			this.Unregister();
		}

		private void Register()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23053, new MobaMessageFunc(this.OnOpenView));
		}

		private void Unregister()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23053, new MobaMessageFunc(this.OnOpenView));
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
			this.bottlesystemitem = Resources.Load<BottleSystemItem>("Prefab/UI/Bottle/BottleSystemItem");
			this.itemsGrid = base.transform.Find("ItemsGrid").GetComponent<UIGrid>();
			this.details = base.transform.Find("Detail");
			this.itemBG = this.details.Find("GoldenFrame/ItemBG");
		}

		private void InitUI()
		{
			int level = ModelManager.Instance.Get_BottleData_Level();
			GridHelper.FillGrid<BottleSystemItem>(this.itemsGrid, this.bottlesystemitem, 10, delegate(int idx, BottleSystemItem comp)
			{
				comp.name = idx + 1 + string.Empty;
				comp.Init(level, idx);
				comp.DragCallBack = new Callback<GameObject, bool>(this.ShowDetails);
			});
			this.itemsGrid.Reposition();
		}

		private void ShowDetails(GameObject obj, bool isIn)
		{
			if (null != obj)
			{
				this.ParseType(obj.GetComponent<BottleSystemItem>());
				this.details.gameObject.SetActive(isIn);
				this.temp = obj.GetComponent<BottleSystemItem>();
			}
			else if (this.temp.Position == obj.GetComponent<BottleSystemItem>().Position)
			{
				this.details.gameObject.SetActive(false);
			}
			else
			{
				this.ParseType(obj.GetComponent<BottleSystemItem>());
				this.temp = obj.GetComponent<BottleSystemItem>();
			}
		}

		private void ParseType(BottleSystemItem item)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			text = item.Type;
			text2 = item.Name;
			this.details.GetChild(1).GetChild(0).GetComponent<UILabel>().text = text;
			this.details.GetChild(1).GetChild(1).GetComponent<UILabel>().text = text2;
			switch (item.ItemType)
			{
			case 1:
				this.itemBG.GetChild(1).GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(item.Icon_detail, true, true, null, 0, false);
				this.details.GetChild(1).GetChild(1).GetComponent<UILabel>().text = item.ItemCount.ToString();
				break;
			case 2:
				this.itemBG.GetChild(0).GetChild(0).GetComponent<UISprite>().spriteName = item.Icon;
				this.itemBG.GetChild(1).GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(item.Icon_detail, true, true, null, 0, false);
				break;
			case 3:
				this.itemBG.GetChild(1).GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(item.Icon_detail, true, true, null, 0, false);
				break;
			}
			this.details.localPosition = new Vector3(-1090f, 25f, 0f) + new Vector3((float)(102 * item.Position), 0f, 0f);
			this.itemBG.GetChild(0).gameObject.SetActive(item.ItemType == 2 && "符文" == item.Type);
			this.itemBG.GetChild(1).gameObject.SetActive("符文" != item.Type);
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
	}
}
