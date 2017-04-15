using Assets.Scripts.GUILogic.View.BattleEquipment;
using Com.Game.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class ShopInfo
	{
		private bool bOpen;

		private bool inArea;

		private SysBattleShopVo config;

		private int dealCounter;

		private EBattleShopState state;

		private List<string> items;

		private Stack<RollbackInfo> pveStack;

		public bool Open
		{
			get
			{
				return this.bOpen;
			}
			set
			{
				this.bOpen = value;
			}
		}

		public EBattleShopType ShopType
		{
			get
			{
				return (EBattleShopType)this.config.type;
			}
		}

		public string ShopID
		{
			get
			{
				return this.config.id;
			}
		}

		public bool InArea
		{
			get
			{
				return this.inArea;
			}
			set
			{
				if (this.inArea != value)
				{
					this.inArea = value;
					bool flag = ModelManager.Instance.Get_BattleShop_playerAlive();
					if (flag && !this.inArea)
					{
						this.DealCounter = 0;
						this.RollbackStack.Clear();
					}
				}
			}
		}

		public SysBattleShopVo Config
		{
			get
			{
				return this.config;
			}
		}

		public EBattleShopState State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		public int DealCounter
		{
			get
			{
				return this.dealCounter;
			}
			set
			{
				if (this.dealCounter != value)
				{
					this.dealCounter = value;
					MobaMessageManagerTools.SendClientMsg(ClientC2V.BattleShop_dealCounter, this.dealCounter, false);
				}
			}
		}

		public Stack<RollbackInfo> RollbackStack
		{
			get
			{
				return this.pveStack ?? new Stack<RollbackInfo>();
			}
			set
			{
				this.pveStack = value;
			}
		}

		public List<string> ShopItems
		{
			get
			{
				List<string> arg_1B_0;
				if ((arg_1B_0 = this.items) == null)
				{
					arg_1B_0 = (this.items = new List<string>());
				}
				return arg_1B_0;
			}
			private set
			{
				this.items = value;
			}
		}

		public ShopInfo(SysBattleShopVo con, bool inA = false)
		{
			this.bOpen = false;
			this.inArea = inA;
			this.config = con;
			this.dealCounter = 0;
			this.state = EBattleShopState.eIdle;
			this.pveStack = new Stack<RollbackInfo>();
			this.ShopItems = BattleEquipTools_config.GetShopItems(con);
		}

		public void Clear()
		{
			this.inArea = false;
			this.config = null;
			this.dealCounter = 0;
			this.state = EBattleShopState.eIdle;
			this.pveStack = null;
		}

		public void OnPlayerLiveChanged(bool alive)
		{
			if (alive && !this.InArea)
			{
				this.DealCounter = 0;
				this.RollbackStack.Clear();
			}
		}

		public void ResetShop()
		{
			this.State = EBattleShopState.eIdle;
			this.DealCounter = 0;
			this.RollbackStack.Clear();
		}

		public static ShopInfo operator ++(ShopInfo shop)
		{
			shop.DealCounter++;
			return shop;
		}

		public static ShopInfo operator --(ShopInfo shop)
		{
			if (shop.DealCounter > 0)
			{
				shop.DealCounter--;
			}
			return shop;
		}
	}
}
