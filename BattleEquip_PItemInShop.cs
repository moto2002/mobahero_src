using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class BattleEquip_PItemInShop : MonoBehaviour
{
	[SerializeField]
	private UISprite mItemIcon;

	[SerializeField]
	private UISprite mFrame;

	[SerializeField]
	private UISprite mMask;

	[SerializeField]
	private UILabel mCD;

	[SerializeField]
	private UILabel mNum;

	[SerializeField]
	private UISprite mInitiativeFrame;

	[SerializeField]
	private UISprite mPassiveFrame;

	[SerializeField]
	private new Animation animation;

	private UIWidget widget;

	private GameObject rootGoObj;

	private ItemInfo itemData;

	private bool chooseState;

	private bool turnOnCD;

	private bool initiativeItem;

	private float cd;

	private float leftCD;

	private bool onCD;

	private CoroutineManager coroutineManager;

	private Callback<BattleEquip_PItemInShop> onClickItem;

	public Callback<BattleEquip_PItemInShop> OnClickItem
	{
		get
		{
			return this.onClickItem;
		}
		set
		{
			this.onClickItem = value;
		}
	}

	public ItemInfo ItemData
	{
		get
		{
			return this.itemData;
		}
		set
		{
			this.itemData = value;
			this.RefreshUI_item();
		}
	}

	public bool ChooseState
	{
		get
		{
			return this.chooseState;
		}
		set
		{
			if (this.chooseState != value)
			{
				this.chooseState = value;
				this.RefreshUI_chooseState();
			}
		}
	}

	public bool TurnOnCD
	{
		get
		{
			return this.turnOnCD;
		}
		set
		{
			this.turnOnCD = value;
		}
	}

	private bool OnCD
	{
		get
		{
			return this.onCD;
		}
		set
		{
			this.onCD = value;
			this.leftCD = ((!this.onCD) ? 0f : this.cd);
			this.mMask.gameObject.SetActive(this.onCD);
		}
	}

	public bool Initiative
	{
		get
		{
			return this.initiativeItem;
		}
	}

	private void Awake()
	{
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickItemCallback);
		this.OnCD = false;
		this.initiativeItem = false;
	}

	private void Update()
	{
		if (this.OnCD)
		{
			this.leftCD -= Time.deltaTime;
			if (this.leftCD > 0f)
			{
				this.mMask.fillAmount = ((this.cd == 0f) ? 1f : (this.leftCD / this.cd));
			}
			else
			{
				this.mMask.fillAmount = 0f;
				this.OnCD = false;
			}
		}
	}

	private void OnClickItemCallback(GameObject obj)
	{
		if (!this.OnCD)
		{
			if (this.TurnOnCD && this.initiativeItem)
			{
				this.OnCD = true;
			}
			if (this.OnClickItem != null)
			{
				this.OnClickItem(this);
			}
		}
	}

	private void RefreshUI_item()
	{
		ItemInfo itemInfo = this.ItemData;
		SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(itemInfo.ID);
		if (dataById == null)
		{
			Reminder.ReminderStr("battleItem为空id===>" + this.ItemData);
			return;
		}
		this.mItemIcon.spriteName = dataById.icon;
		this.mNum.gameObject.SetActive(itemInfo.Num > 1);
		this.mNum.text = itemInfo.Num.ToString();
	}

	public void ShowAni()
	{
		if (!this.animation.isPlaying)
		{
			if (this.widget == null)
			{
				this.widget = base.gameObject.GetComponent<UIWidget>();
				this.widget.alpha = 0f;
			}
			this.animation.Play();
			if (this.coroutineManager == null)
			{
				this.coroutineManager = new CoroutineManager();
			}
			this.coroutineManager.StartCoroutine(this.ReSetScale(), true);
		}
	}

	[DebuggerHidden]
	private IEnumerator ReSetScale()
	{
		BattleEquip_PItemInShop.<ReSetScale>c__IteratorF8 <ReSetScale>c__IteratorF = new BattleEquip_PItemInShop.<ReSetScale>c__IteratorF8();
		<ReSetScale>c__IteratorF.<>f__this = this;
		return <ReSetScale>c__IteratorF;
	}

	private void RefreshUI_chooseState()
	{
		this.mFrame.gameObject.SetActive(this.ChooseState);
	}
}
