using Assets.Scripts.GUILogic.View.BattleEquipment;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class BattleShopData
	{
		private EBattleShopContex shopContext;

		private string levelID;

		private BattleEquipType curMenuType;

		private ItemInfo curPItem;

		private SItemData curSItem;

		private SItemData curSRItem;

		private string curHItem;

		private IBEItem curTItem;

		private RollbackInfo rollBackInfo;

		private SItemData preSItem;

		private SItemData preSRItem;

		private Dictionary<ColumnType, Dictionary<string, SItemData>> sItems;

		private List<ItemInfo> pItems;

		private List<string> pItemsStr;

		private List<string> rItems;

		private List<RItemData> rItemsSub;

		private string syncRItem;

		private ShopInfo preOpenShop;

		private ShopInfo openShop;

		private ShopInfo dealingShop;

		private Dictionary<EBattleShopType, ShopInfo> dicShops;

		private List<List<SItemData>>[] sections;

		private int money;

		private bool playerAlive;

		private bool brawl_canBuy;

		private bool enable_quickRecommend;

		private bool b_initRitems;

		public EBattleShopContex ShopContext
		{
			get
			{
				return this.shopContext;
			}
			set
			{
				this.shopContext = value;
			}
		}

		public string LevelID
		{
			get
			{
				return this.levelID;
			}
			set
			{
				this.levelID = value;
			}
		}

		public BattleEquipType CurMenu
		{
			get
			{
				return this.curMenuType;
			}
			set
			{
				this.curMenuType = value;
			}
		}

		public SItemData CurSItem
		{
			get
			{
				return this.curSItem;
			}
			set
			{
				this.curSItem = value;
			}
		}

		public SItemData CurSRItem
		{
			get
			{
				return this.curSRItem;
			}
			set
			{
				this.curSRItem = value;
			}
		}

		public SItemData PreSItem
		{
			get
			{
				return this.preSItem;
			}
			set
			{
				this.preSItem = value;
			}
		}

		public SItemData PreSRItem
		{
			get
			{
				return this.preSRItem;
			}
			set
			{
				this.preSRItem = value;
			}
		}

		public Dictionary<ColumnType, Dictionary<string, SItemData>> SItems
		{
			get
			{
				BattleEquipTools_config.RegularSItems(ref this.sItems, false);
				return this.sItems;
			}
			set
			{
				this.sItems = value;
			}
		}

		public string CurHItem
		{
			get
			{
				string arg_1B_0;
				if ((arg_1B_0 = this.curHItem) == null)
				{
					arg_1B_0 = (this.curHItem = string.Empty);
				}
				return arg_1B_0;
			}
			set
			{
				this.curHItem = value;
			}
		}

		public RollbackInfo CurRollbackInfo
		{
			get
			{
				return this.rollBackInfo;
			}
			set
			{
				this.rollBackInfo = value;
			}
		}

		public List<ItemInfo> PItems
		{
			get
			{
				List<ItemInfo> arg_1B_0;
				if ((arg_1B_0 = this.pItems) == null)
				{
					arg_1B_0 = (this.pItems = new List<ItemInfo>());
				}
				return arg_1B_0;
			}
			set
			{
				this.pItems = value;
			}
		}

		public List<string> PItemsStr
		{
			get
			{
				List<string> arg_1B_0;
				if ((arg_1B_0 = this.pItemsStr) == null)
				{
					arg_1B_0 = (this.pItemsStr = new List<string>());
				}
				return arg_1B_0;
			}
			set
			{
				this.pItemsStr = value;
			}
		}

		public ItemInfo CurPItem
		{
			get
			{
				return this.curPItem;
			}
			set
			{
				this.curPItem = value;
			}
		}

		public List<string> RItems
		{
			get
			{
				List<string> arg_1B_0;
				if ((arg_1B_0 = this.rItems) == null)
				{
					arg_1B_0 = (this.rItems = new List<string>());
				}
				return arg_1B_0;
			}
			set
			{
				this.rItems = value;
			}
		}

		public List<RItemData> RItemsSub
		{
			get
			{
				List<RItemData> arg_1B_0;
				if ((arg_1B_0 = this.rItemsSub) == null)
				{
					arg_1B_0 = (this.rItemsSub = new List<RItemData>());
				}
				return arg_1B_0;
			}
			set
			{
				this.rItemsSub = value;
			}
		}

		public string SyncRItem
		{
			get
			{
				return this.syncRItem;
			}
			set
			{
				this.syncRItem = value;
			}
		}

		public IBEItem CurTItem
		{
			get
			{
				return this.curTItem;
			}
			set
			{
				this.curTItem = value;
			}
		}

		public ShopInfo PreOpenShop
		{
			get
			{
				return this.preOpenShop;
			}
			set
			{
				this.preOpenShop = value;
			}
		}

		public ShopInfo OpenShop
		{
			get
			{
				return this.openShop;
			}
			set
			{
				this.openShop = value;
			}
		}

		public ShopInfo DealingShop
		{
			get
			{
				return this.dealingShop;
			}
			set
			{
				this.dealingShop = value;
			}
		}

		public Dictionary<EBattleShopType, ShopInfo> DicShops
		{
			get
			{
				Dictionary<EBattleShopType, ShopInfo> arg_1B_0;
				if ((arg_1B_0 = this.dicShops) == null)
				{
					arg_1B_0 = (this.dicShops = new Dictionary<EBattleShopType, ShopInfo>());
				}
				return arg_1B_0;
			}
			set
			{
				this.dicShops = value;
			}
		}

		public List<List<SItemData>>[] Sections
		{
			get
			{
				List<List<SItemData>>[] arg_1C_0;
				if ((arg_1C_0 = this.sections) == null)
				{
					arg_1C_0 = (this.sections = new List<List<SItemData>>[2]);
				}
				return arg_1C_0;
			}
			set
			{
				this.sections = value;
			}
		}

		public int Money
		{
			get
			{
				return this.money;
			}
			set
			{
				this.money = value;
			}
		}

		public bool PlayerAlive
		{
			get
			{
				return this.playerAlive;
			}
			set
			{
				this.playerAlive = value;
			}
		}

		public bool Brawl_canBuy
		{
			get
			{
				return this.brawl_canBuy;
			}
			set
			{
				this.brawl_canBuy = value;
			}
		}

		public bool Enable_quickRecommend
		{
			get
			{
				return this.enable_quickRecommend;
			}
			set
			{
				this.enable_quickRecommend = value;
			}
		}

		public bool B_initRItems
		{
			get
			{
				return this.b_initRitems;
			}
			set
			{
				this.b_initRitems = value;
			}
		}

		public BattleShopData()
		{
			this.curMenuType = BattleEquipType.recommend;
			this.playerAlive = true;
		}

		public void ResetData()
		{
			this.ShopContext = EBattleShopContex.eNone;
			this.LevelID = string.Empty;
			this.CurMenu = BattleEquipType.none;
			this.CurPItem = null;
			this.CurSItem = null;
			this.CurTItem = null;
			this.CurHItem = string.Empty;
			this.CurRollbackInfo = null;
			this.PreSItem = null;
			this.SItems = null;
			this.PItems = null;
			this.PItemsStr = null;
			this.RItems = null;
			this.RItemsSub = null;
			this.SyncRItem = null;
			this.OpenShop = null;
			this.DealingShop = null;
			this.PreOpenShop = null;
			Dictionary<EBattleShopType, ShopInfo> dictionary = this.DicShops;
			foreach (KeyValuePair<EBattleShopType, ShopInfo> current in dictionary)
			{
				current.Value.Clear();
			}
			this.DicShops.Clear();
			this.DicShops = null;
			this.Sections = null;
			this.Money = 0;
			this.Brawl_canBuy = false;
			this.PlayerAlive = false;
			this.Enable_quickRecommend = false;
			this.b_initRitems = false;
		}
	}
}
