using Assets.Scripts.Model;
using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewBottom : MonoBehaviour
	{
		private UIToggle heroInfoView;

		private UIToggle heroNatureView;

		private UIToggle heroCollectionView;

		private UIToggle heroRunesView;

		private Transform Arrow;

		private Transform LeftArrow;

		private Transform RightArrow;

		private Transform PurchaseBtn;

		private Transform transVideo;

		private Transform transStory;

		private new Transform collider;

		private UILabel heroInfoLabel;

		private UILabel heroNatureLabel;

		private UILabel heroCollectionLabel;

		private UISprite heroCollectionMark;

		private UILabel heroRunesLabel;

		private UISprite heroRunesMark;

		private PropertyType currType;

		private Dictionary<PropertyType, UIToggle> dictToggle;

		private object[] mgs;

		private PropertyType CurrType
		{
			get
			{
				return this.currType;
			}
			set
			{
				this.currType = value;
				this.SetColorChange(this.currType);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewChangeToggle, this.currType, false);
				if (this.currType == PropertyType.Rune)
				{
					MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewOpenView, SacrificialCtrl.GetInstance().HeroNPC, false);
				}
			}
		}

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.sacriviewChangeHero,
				ClientV2C.propviewInitToggle
			};
			this.Initialize();
		}

		private void OnEnable()
		{
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
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Initialize()
		{
			this.heroInfoView = base.transform.Find("Panel/Toggle/InfoBtn").GetComponent<UIToggle>();
			this.heroCollectionView = base.transform.Find("Panel/Toggle/CollocationBtn").GetComponent<UIToggle>();
			this.heroNatureView = base.transform.Find("Panel/Toggle/DetailsBtn").GetComponent<UIToggle>();
			this.heroRunesView = base.transform.Find("Panel/Toggle/Rune").GetComponent<UIToggle>();
			this.heroInfoLabel = this.heroInfoView.transform.Find("Label").GetComponent<UILabel>();
			this.heroNatureLabel = this.heroNatureView.transform.Find("Label").GetComponent<UILabel>();
			this.heroCollectionLabel = this.heroCollectionView.transform.Find("Label").GetComponent<UILabel>();
			this.heroRunesLabel = this.heroRunesView.transform.Find("Label").GetComponent<UILabel>();
			this.heroCollectionMark = this.heroCollectionView.transform.Find("Mark").GetComponent<UISprite>();
			this.heroRunesMark = this.heroRunesView.transform.Find("Mark").GetComponent<UISprite>();
			this.collider = base.transform.Find("Panel/Toggle/Collider");
			this.transVideo = base.transform.Find("Video");
			this.transStory = base.transform.Find("Story");
			this.Arrow = base.transform.Find("Panel/Arrow");
			this.LeftArrow = this.Arrow.Find("Left");
			this.RightArrow = this.Arrow.Find("Right");
			this.PurchaseBtn = base.transform.Find("Panel/Toggle/Purchase");
			this.currType = PropertyType.Info;
			this.dictToggle = new Dictionary<PropertyType, UIToggle>();
			this.dictToggle[PropertyType.Info] = this.heroInfoView;
			this.dictToggle[PropertyType.Collection] = this.heroCollectionView;
			this.dictToggle[PropertyType.Nature] = this.heroNatureView;
			this.dictToggle[PropertyType.Rune] = this.heroRunesView;
			EventDelegate.Add(this.heroInfoView.onChange, new EventDelegate.Callback(this.ClickBtn));
			EventDelegate.Add(this.heroCollectionView.onChange, new EventDelegate.Callback(this.ClickBtn));
			EventDelegate.Add(this.heroNatureView.onChange, new EventDelegate.Callback(this.ClickBtn));
			EventDelegate.Add(this.heroRunesView.onChange, new EventDelegate.Callback(this.ClickBtn));
			UIEventListener.Get(this.LeftArrow.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickArrow);
			UIEventListener.Get(this.RightArrow.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickArrow);
			UIEventListener.Get(this.collider.gameObject).onClick = new UIEventListener.VoidDelegate(this.HintSth);
			UIEventListener.Get(this.transVideo.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickComing);
			UIEventListener.Get(this.transStory.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickComing);
		}

		private void ClickBtn()
		{
			if (UIToggle.current.value)
			{
				foreach (KeyValuePair<PropertyType, UIToggle> current in this.dictToggle)
				{
					if (current.Value == UIToggle.current)
					{
						this.CurrType = current.Key;
						break;
					}
				}
			}
		}

		private void OnMsg_sacriviewChangeHero(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				this.heroRunesView.transform.gameObject.SetActive(false);
				string modelID = string.Empty;
				modelID = (string)msg.Param;
				if (ModelManager.Instance.Get_heroInfo_item_byModelID_X(modelID) == null)
				{
					this.heroCollectionView.GetComponent<BoxCollider>().enabled = false;
					this.CurrType = PropertyType.Info;
					this.dictToggle[this.currType].value = true;
				}
				else
				{
					this.heroCollectionView.GetComponent<BoxCollider>().enabled = true;
					this.heroRunesView.transform.gameObject.SetActive(true);
					this.heroRunesView.optionCanBeNone = true;
					this.heroRunesView.value = false;
					this.heroRunesView.optionCanBeNone = false;
				}
			}
		}

		private void OnMsg_propviewInitToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				PropertyType key = (PropertyType)((int)msg.Param);
				if (this.dictToggle != null && this.dictToggle.ContainsKey(key))
				{
					this.currType = key;
					this.dictToggle[key].value = true;
				}
			}
		}

		private void ClickArrow(GameObject obj = null)
		{
			if (null != obj)
			{
				string name = obj.name;
				if (name != null)
				{
					if (PropViewBottom.<>f__switch$map25 == null)
					{
						PropViewBottom.<>f__switch$map25 = new Dictionary<string, int>(2)
						{
							{
								"Left",
								0
							},
							{
								"Right",
								1
							}
						};
					}
					int num;
					if (PropViewBottom.<>f__switch$map25.TryGetValue(name, out num))
					{
						if (num != 0)
						{
							if (num == 1)
							{
								MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewClickLeftorRight, true, false);
							}
						}
						else
						{
							MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewClickLeftorRight, false, false);
						}
					}
				}
			}
		}

		private void HintSth(GameObject obj)
		{
			if (null != obj)
			{
				Singleton<TipView>.Instance.ShowViewSetText("请先购买该英雄！", 1f);
			}
		}

		private void ClickComing(GameObject obj)
		{
			if (null != obj)
			{
				Singleton<TipView>.Instance.ShowViewSetText("即将到来！！！", 1f);
			}
		}

		private void SetColorChange(PropertyType type)
		{
			this.heroInfoLabel.gradientTop = new Color(0f, 0.996078432f, 0.8901961f, 1f);
			this.heroInfoLabel.gradientBottom = Color.white;
			this.heroNatureLabel.gradientTop = new Color(0f, 0.996078432f, 0.8901961f, 1f);
			this.heroNatureLabel.gradientBottom = Color.white;
			this.heroCollectionLabel.gradientTop = new Color(0.6862745f, 0f, 0.294117659f, 1f);
			this.heroCollectionLabel.gradientBottom = new Color(0.9843137f, 0f, 0.466666669f, 1f);
			this.heroCollectionMark.spriteName = "Hero_personal_icons_ring_01";
			this.heroRunesLabel.gradientTop = new Color(0f, 0.996078432f, 0.8901961f, 1f);
			this.heroRunesLabel.gradientBottom = Color.white;
			this.heroRunesMark.spriteName = "Hero_information_icon_fuwen_01";
			switch (type)
			{
			case PropertyType.Info:
				this.heroInfoLabel.gradientTop = new Color(0.968627453f, 0.996078432f, 0.6627451f, 1f);
				this.heroInfoLabel.gradientBottom = Color.white;
				break;
			case PropertyType.Nature:
				this.heroNatureLabel.gradientTop = new Color(0.968627453f, 0.996078432f, 0.6627451f, 1f);
				this.heroNatureLabel.gradientBottom = Color.white;
				break;
			case PropertyType.Collection:
				this.heroCollectionLabel.gradientTop = new Color(0.996078432f, 0.164705887f, 0.466666669f, 1f);
				this.heroCollectionLabel.gradientBottom = new Color(0.996078432f, 0.8117647f, 0.6156863f, 1f);
				this.heroCollectionMark.spriteName = "Hero_personal_icons_ring_02";
				break;
			case PropertyType.Rune:
				this.transVideo.gameObject.SetActive(false);
				this.transStory.gameObject.SetActive(false);
				this.heroRunesLabel.gradientTop = new Color(0.968627453f, 0.996078432f, 0.6627451f, 1f);
				this.heroRunesLabel.gradientBottom = Color.white;
				this.heroRunesMark.spriteName = "Hero_information_icon_fuwen_02";
				break;
			}
		}
	}
}
