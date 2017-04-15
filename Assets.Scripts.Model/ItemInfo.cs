using Com.Game.Data;
using System;

namespace Assets.Scripts.Model
{
	public class ItemInfo : IEquatable<ItemInfo>, IBEItem
	{
		private int pos;

		private int num;

		private int oid;

		private SysBattleItemsVo config;

		public string ID
		{
			get
			{
				return this.config.items_id;
			}
		}

		public SysBattleItemsVo Config
		{
			get
			{
				return this.config;
			}
		}

		public ColumnType Level
		{
			get
			{
				return (ColumnType)this.config.level;
			}
		}

		public BattleEquipType Type
		{
			get
			{
				return (BattleEquipType)this.config.type;
			}
		}

		public int RealPrice
		{
			get
			{
				return this.config.sell;
			}
		}

		public int Price
		{
			get
			{
				return this.config.sell;
			}
		}

		public int Pos
		{
			get
			{
				return this.pos;
			}
		}

		public int Num
		{
			get
			{
				return this.num;
			}
			set
			{
				this.num = value;
			}
		}

		public int OID
		{
			get
			{
				return this.oid;
			}
		}

		public ItemInfo()
		{
			this.pos = 0;
			this.num = 1;
			this.oid = 0;
			this.config = new SysBattleItemsVo();
		}

		public ItemInfo(SysBattleItemsVo vo)
		{
			this.config = vo;
		}

		public ItemInfo(int pos, int oid, int num, SysBattleItemsVo vo) : this(vo)
		{
			this.pos = pos;
			this.oid = oid;
			this.num = num;
		}

		public ItemInfo clone()
		{
			return new ItemInfo
			{
				pos = this.pos,
				num = this.num,
				oid = this.oid,
				config = this.config
			};
		}

		public bool Equals(ItemInfo other)
		{
			return other != null && other.ID.Equals(this.ID);
		}
	}
}
