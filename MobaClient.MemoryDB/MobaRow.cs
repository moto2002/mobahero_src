using System;
using System.Collections.Generic;
using System.Data;

namespace MobaClient.MemoryDB
{
	[Serializable]
	public class MobaRow
	{
		private Dictionary<string, MobaColumn> item;

		private MobaColumn outValue;

		private MobaRowState rowState;

		public int Count
		{
			get
			{
				return this.RowItem.Count;
			}
		}

		public MobaColumn this[string rowName]
		{
			get
			{
				MobaColumn result;
				try
				{
					this.rowState = MobaRowState.Update;
					result = this.RowItem[rowName];
				}
				catch (Exception ex)
				{
					throw ex;
				}
				return result;
			}
			set
			{
				if (this.RowItem.TryGetValue(rowName, out this.outValue))
				{
					this.RowItem[rowName] = value;
				}
			}
		}

		public Dictionary<string, MobaColumn> RowItem
		{
			get
			{
				return this.item;
			}
			set
			{
				this.item = value;
			}
		}

		public MobaRowState RowState
		{
			get
			{
				return this.rowState;
			}
			set
			{
				this.rowState = value;
			}
		}

		public MobaRow()
		{
			this.item = new Dictionary<string, MobaColumn>();
			this.rowState = MobaRowState.Noting;
		}

		public MobaRow(List<string> listName)
		{
			this.item = new Dictionary<string, MobaColumn>();
			this.rowState = MobaRowState.Noting;
			foreach (string current in listName)
			{
				this.Add(current, "");
			}
		}

		public MobaRow(DataRow dr)
		{
			this.item = new Dictionary<string, MobaColumn>();
			this.rowState = MobaRowState.Noting;
			int num = 0;
			foreach (DataColumn dataColumn in dr.Table.Columns)
			{
				this.Add(dataColumn.ColumnName, dr[num].ToString());
				num++;
			}
		}

		public void Add(string rowName, string rowValue)
		{
			if (!this.RowItem.ContainsKey(rowName))
			{
				this.RowItem.Add(rowName, new MobaColumn(rowValue));
			}
		}

		public void Clear()
		{
			this.RowItem.Clear();
		}

		public bool ContainsKey(string mKey)
		{
			return this.RowItem.ContainsKey(mKey);
		}

		public MobaRow CopyTo()
		{
			MobaRow mobaRow = new MobaRow();
			foreach (KeyValuePair<string, MobaColumn> current in this.RowItem)
			{
				mobaRow.Add(current.Key, current.Value.MobaColumnText);
			}
			return mobaRow;
		}

		public void Delete(string rowName)
		{
			if (this.RowItem.ContainsKey(rowName))
			{
				this.RowItem.Remove(rowName);
			}
		}

		public string[] GetColumnsName()
		{
			string[] array = new string[this.RowItem.Keys.Count];
			this.RowItem.Keys.CopyTo(array, 0);
			return array;
		}

		public bool TryGetValue(string mKey, out string mValue)
		{
			MobaColumn mobaColumn = null;
			bool result;
			if (this.RowItem.TryGetValue(mKey, out mobaColumn))
			{
				mValue = mobaColumn.MobaColumnText;
				result = true;
			}
			else
			{
				mValue = null;
				result = false;
			}
			return result;
		}
	}
}
