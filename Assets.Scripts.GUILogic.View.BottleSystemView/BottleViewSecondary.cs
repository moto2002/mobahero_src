using Com.Game.Module;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BottleSystemView
{
	public class BottleViewSecondary : MonoBehaviour
	{
		[SerializeField]
		private Transform secondCtrl;

		[SerializeField]
		private Transform mainTitle_1;

		[SerializeField]
		private Transform mainTitle_2;

		[SerializeField]
		private Transform title_A;

		[SerializeField]
		private Transform title_B;

		[SerializeField]
		private Transform title_C;

		[SerializeField]
		private Transform description_A;

		[SerializeField]
		private Transform description_B;

		[SerializeField]
		private Transform description_C;

		[SerializeField]
		private Transform playGame;

		[SerializeField]
		private Transform purchaseExp;

		[SerializeField]
		private UISprite bottleMark;

		[SerializeField]
		private Transform block;

		private void Awake()
		{
			this.InitUI();
		}

		private void OnEnable()
		{
			this.block.gameObject.SetActive(true);
			this.ShowPanel(false);
			this.Register();
		}

		private void OnDisable()
		{
			this.Unregister();
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Register()
		{
			MobaMessageManager.RegistMessage((ClientMsg)21045, new MobaMessageFunc(this.ActionSecondary));
		}

		private void Unregister()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)21045, new MobaMessageFunc(this.ActionSecondary));
		}

		private void InitUI()
		{
			UIEventListener.Get(this.playGame.gameObject).onClick = new UIEventListener.VoidDelegate(this.PlayGame);
			UIEventListener.Get(this.purchaseExp.gameObject).onClick = new UIEventListener.VoidDelegate(this.PurchaseExpBall);
			UIEventListener.Get(this.block.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseSecondary);
		}

		private void ActionSecondary(MobaMessage msg)
		{
			if (msg != null)
			{
				LackState lackState = (LackState)((int)msg.Param);
				if (lackState == LackState.None)
				{
					this.ShowPanel(false);
				}
				else
				{
					this.ChangePanelContent((int)lackState);
					this.ShowPanel(true);
				}
			}
		}

		private void ShowPanel(bool isActive = false)
		{
			this.secondCtrl.gameObject.SetActive(isActive);
		}

		private void ChangePanelContent(int num)
		{
			this.ParseString(num);
		}

		private void ParseString(int num)
		{
			int[] expr_06 = new int[]
			{
				0,
				1,
				2,
				3
			};
			string text = "配表";
			string text2 = "问题";
			string text3 = "请";
			string text4 = "bindata";
			string text5 = "之后";
			string text6 = "等待";
			string text7 = "更新";
			string text8 = "查看";
			if (num != 1)
			{
				if (num == 2)
				{
					this.bottleMark.spriteName = "Magic_bottle_icons_05";
					text = LanguageManager.Instance.GetStringById("MagicBottle_GoodTips_MainTitle_1");
					text2 = LanguageManager.Instance.GetStringById("MagicBottle_GoodTips_MainTitle_2");
					text3 = LanguageManager.Instance.GetStringById("MagicBottle_GoodTips_Title_1");
					text4 = LanguageManager.Instance.GetStringById("MagicBottle_GoodTips_Title_2");
					text5 = LanguageManager.Instance.GetStringById("MagicBottle_GoodTips_Title_3");
					text6 = LanguageManager.Instance.GetStringById("MagicBottle_GoodTips_Descripsion_1");
					int[] expr_1BD = new int[5];
					expr_1BD[1] = 3;
					expr_1BD[3] = 1;
					int[] format = expr_1BD;
					text6 = this.ParseStringFormat(text6, format);
					text7 = LanguageManager.Instance.GetStringById("MagicBottle_GoodTips_Descripsion_2");
					int[] expr_1E8 = new int[5];
					expr_1E8[1] = 2;
					expr_1E8[3] = 2;
					format = expr_1E8;
					text7 = this.ParseStringFormat(text7, format);
					text8 = LanguageManager.Instance.GetStringById("MagicBottle_GoodTips_Descripsion_3");
					int[] expr_213 = new int[4];
					expr_213[0] = 3;
					expr_213[2] = 1;
					format = expr_213;
					text8 = this.ParseStringFormat(text8, format);
				}
			}
			else
			{
				this.bottleMark.spriteName = "Magic_bottle_icons_04";
				text = LanguageManager.Instance.GetStringById("MagicBottle_NomalTips_MainTitle_1");
				text2 = LanguageManager.Instance.GetStringById("MagicBottle_NomalTips_MainTitle_2");
				text3 = LanguageManager.Instance.GetStringById("MagicBottle_NomalTips_Title_1");
				text4 = LanguageManager.Instance.GetStringById("MagicBottle_NomalTips_Title_2");
				text5 = LanguageManager.Instance.GetStringById("MagicBottle_NomalTips_Title_3");
				text6 = LanguageManager.Instance.GetStringById("MagicBottle_NomalTips_Descripsion_1");
				int[] expr_D9 = new int[3];
				expr_D9[1] = 1;
				int[] format = expr_D9;
				text6 = this.ParseStringFormat(text6, format);
				text7 = LanguageManager.Instance.GetStringById("MagicBottle_NomalTips_Descripsion_2");
				int[] expr_100 = new int[5];
				expr_100[1] = 2;
				expr_100[3] = 2;
				format = expr_100;
				text7 = this.ParseStringFormat(text7, format);
				text8 = LanguageManager.Instance.GetStringById("MagicBottle_NomalTips_Descripsion_3");
				int[] expr_12B = new int[4];
				expr_12B[0] = 3;
				expr_12B[2] = 1;
				format = expr_12B;
				text8 = this.ParseStringFormat(text8, format);
			}
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text3) || string.IsNullOrEmpty(text4) || string.IsNullOrEmpty(text5))
			{
				return;
			}
			this.mainTitle_1.GetComponent<UILabel>().text = text;
			this.mainTitle_2.GetComponent<UILabel>().text = text2;
			this.title_A.GetComponent<UILabel>().text = text3;
			this.title_B.GetComponent<UILabel>().text = text4;
			this.title_C.GetComponent<UILabel>().text = text5;
			this.description_A.GetComponent<UILabel>().text = text6;
			this.description_B.GetComponent<UILabel>().text = text7;
			this.description_C.GetComponent<UILabel>().text = text8;
		}

		private string ParseStringFormat(string s, int[] format)
		{
			string text = string.Empty;
			string[] array = s.Split(new char[]
			{
				'|'
			});
			if (array.Length != format.Length)
			{
				CtrlManager.ShowMsgBox("mismatch string array length!!!", "the length of string array mismatched the formats'", delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
			}
			else
			{
				for (int i = 0; i < array.Length; i++)
				{
					switch (format[i])
					{
					case 0:
						array[i] = "[00e0c8]" + array[i] + "[-]";
						break;
					case 1:
						array[i] = "[ff0054]" + array[i] + "[-]";
						break;
					case 2:
						array[i] = "[00ff36]" + array[i] + "[-]";
						break;
					case 3:
						array[i] = "[ffea00]" + array[i] + "[-]";
						break;
					}
				}
				for (int j = 0; j < array.Length; j++)
				{
					text += array[j];
				}
			}
			return text;
		}

		private void PlayGame(GameObject obj)
		{
			if (null != obj)
			{
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			}
		}

		private void PurchaseExpBall(GameObject obj)
		{
			if (null != obj)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemAddExpBall, null, false);
			}
		}

		private void CloseSecondary(GameObject obj)
		{
			if (null != obj)
			{
				this.ShowPanel(false);
			}
		}
	}
}
