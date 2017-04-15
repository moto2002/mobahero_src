using System;
using System.Collections.Generic;

namespace MobaClient.MemoryDB
{
	public class MobaTable
	{
		private List<MobaRow> deleteRows;

		private string ip;

		private bool isUpdate;

		private List<string> listSql;

		public Dictionary<byte, object> mParms;

		private int port;

		private List<MobaRow> rows;

		private string tableKey;

		private string tableName;

		public int Count
		{
			get
			{
				return this.rows.Count;
			}
		}

		public List<MobaRow> DeleteRows
		{
			get
			{
				return this.deleteRows;
			}
			set
			{
				this.deleteRows = value;
			}
		}

		public string Ip
		{
			get
			{
				return this.ip;
			}
			set
			{
				this.ip = value;
			}
		}

		public bool IsUpdate
		{
			get
			{
				return this.isUpdate;
			}
			set
			{
				this.isUpdate = value;
			}
		}

		public MobaRow this[int index]
		{
			get
			{
				MobaRow result;
				if (index <= this.rows.Count)
				{
					if (this.rows.Count > 0)
					{
						result = this.rows[index];
						return result;
					}
					Log.error("超出row的最大索引值");
				}
				result = null;
				return result;
			}
			set
			{
				if (index > this.rows.Count)
				{
					Log.error("超出row的最大索引值");
				}
				else if (this.rows.Count > 0)
				{
					this.rows[index] = value;
				}
				else
				{
					Log.error("超出row的最大索引值");
				}
			}
		}

		public MobaRow this[MobaRow row]
		{
			get
			{
				MobaRow result;
				foreach (MobaRow current in this.rows)
				{
					if (current == row)
					{
						result = current;
						return result;
					}
				}
				result = null;
				return result;
			}
			set
			{
				bool flag = false;
				foreach (MobaRow current in this.rows)
				{
					if (current == row)
					{
						row = value;
						flag = true;
					}
				}
				if (!flag)
				{
					Log.error("找不到指定的MobaRow");
					throw new Exception("找不到指定的MobaRow");
				}
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
			set
			{
				this.port = value;
			}
		}

		public List<MobaRow> Rows
		{
			get
			{
				return this.rows;
			}
			set
			{
				this.rows = value;
			}
		}

		public string TableKey
		{
			get
			{
				return this.tableKey;
			}
			set
			{
				this.tableKey = value;
			}
		}

		public string TableName
		{
			get
			{
				return this.tableName;
			}
			set
			{
				this.tableName = value;
			}
		}

		public MobaTable(string name, string mTableKey)
		{
			this.rows = new List<MobaRow>();
			this.deleteRows = new List<MobaRow>();
			this.listSql = new List<string>();
			this.tableName = name;
			this.ip = "";
			this.port = 0;
			this.tableKey = mTableKey;
			this.isUpdate = false;
		}

		public MobaTable(string name, string mTableKey, string mIp, int mPort)
		{
			this.rows = new List<MobaRow>();
			this.deleteRows = new List<MobaRow>();
			this.listSql = new List<string>();
			this.tableName = name;
			this.ip = mIp;
			this.port = mPort;
			this.tableKey = mTableKey;
			this.isUpdate = false;
		}

		public void Add(MobaRow row)
		{
			row.RowState = MobaRowState.Insert;
			this.rows.Add(row);
		}

		public void Clear()
		{
			this.rows.Clear();
		}

		public void CopyToDictionary(Dictionary<byte, object> dic)
		{
			if (dic.Count > 0)
			{
				this.Refresh();
				this.deleteRows.Clear();
				this.Clear();
				foreach (KeyValuePair<byte, object> current in dic)
				{
					if (current.Key == 0)
					{
						Dictionary<string, string> dictionary = (Dictionary<string, string>)current.Value;
						this.tableName = dictionary["TableName"];
						this.tableKey = dictionary["TableKey"];
					}
					else
					{
						string[] array = (string[])current.Value;
						int num = 0;
						string rowName = string.Empty;
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							string text = array2[i];
							if (num == 0)
							{
								rowName = text;
							}
							else if (this.rows.Count < num)
							{
								MobaRow mobaRow = new MobaRow();
								mobaRow.Add(rowName, text);
								this.rows.Add(mobaRow);
							}
							else
							{
								this.rows[num - 1].Add(rowName, text);
							}
							num++;
						}
					}
				}
			}
		}

		public void CopyToDictionaryAndParms(Dictionary<byte, object> dic)
		{
			if (dic.Count > 0)
			{
				this.Refresh();
				this.deleteRows.Clear();
				this.Clear();
				foreach (KeyValuePair<byte, object> current in dic)
				{
					if (current.Key == 0)
					{
						Dictionary<string, string> dictionary = (Dictionary<string, string>)current.Value;
						this.tableName = dictionary["TableName"];
						this.tableKey = dictionary["TableKey"];
					}
					else if (current.Key == 1)
					{
						this.mParms = (Dictionary<byte, object>)current.Value;
					}
					else
					{
						string[] array = (string[])current.Value;
						int num = 0;
						string rowName = string.Empty;
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							string text = array2[i];
							if (num == 0)
							{
								rowName = text;
							}
							else if (this.rows.Count < num)
							{
								MobaRow mobaRow = new MobaRow();
								mobaRow.Add(rowName, text);
								this.rows.Add(mobaRow);
							}
							else
							{
								this.rows[num - 1].Add(rowName, text);
							}
							num++;
						}
					}
				}
			}
		}

		public bool Delete(int index)
		{
			bool result;
			if (this.rows.Count > 0 && this.rows.Count <= index)
			{
				this.rows[index].RowState = MobaRowState.Delete;
				this.rows.RemoveAt(index);
				this.deleteRows.Add(this.rows[index]);
				result = true;
			}
			else
			{
				Log.error("超出row的最大索引值");
				result = false;
			}
			return result;
		}

		public bool Delete(MobaRow row)
		{
			bool result;
			try
			{
				this[row].RowState = MobaRowState.Delete;
				this.deleteRows.Add(row);
				this.rows.Remove(row);
				result = true;
			}
			catch
			{
				throw;
			}
			return result;
		}

		public List<string> GetColumnText(string mColumnName)
		{
			List<string> list = new List<string>();
			string empty = string.Empty;
			foreach (MobaRow current in this.Rows)
			{
				if (current.TryGetValue(mColumnName, out empty))
				{
					list.Add(empty);
				}
			}
			return list;
		}

		public bool GetDB(string sql)
		{
			bool result;
			try
			{
				result = true;
			}
			catch
			{
				throw;
			}
			return result;
		}

		public static Dictionary<byte, object> GetParms(Dictionary<byte, object> dic)
		{
			return (Dictionary<byte, object>)dic[1];
		}

		public void Refresh()
		{
			foreach (MobaRow current in this.rows)
			{
				current.RowState = MobaRowState.Noting;
				foreach (KeyValuePair<string, MobaColumn> current2 in current.RowItem)
				{
					current2.Value.MobaColumnState = MobaRowState.Noting;
				}
			}
			foreach (MobaRow current in this.deleteRows)
			{
				current.RowState = MobaRowState.Noting;
			}
		}

		public MobaRow SelectRow(string strName, string strValue)
		{
			MobaRow result;
			foreach (MobaRow current in this.rows)
			{
				if (current[strName].MobaColumnText.IndexOf(strValue) != -1)
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}

		public MobaRow SelectRowEqual(string strName, string strValue)
		{
			MobaRow result;
			foreach (MobaRow current in this.rows)
			{
				if (current[strName].MobaColumnText == strValue)
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}

		public MobaRow[] SelectRows(string strName, string strValue)
		{
			List<MobaRow> list = new List<MobaRow>();
			foreach (MobaRow current in this.rows)
			{
				if (current[strName].MobaColumnText.IndexOf(strValue) != -1)
				{
					list.Add(current);
				}
			}
			MobaRow[] result;
			if (list.Count > 0)
			{
				MobaRow[] array = new MobaRow[list.Count];
				list.CopyTo(array);
				result = array;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public MobaRow[] SelectRowsEqual(string strName, string strValue)
		{
			List<MobaRow> list = new List<MobaRow>();
			foreach (MobaRow current in this.rows)
			{
				if (current[strName].MobaColumnText == strValue)
				{
					list.Add(current);
				}
			}
			MobaRow[] result;
			if (list.Count > 0)
			{
				MobaRow[] array = new MobaRow[list.Count];
				list.CopyTo(array);
				result = array;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public List<MobaRow> SelectRowsList(string strName, string strValue)
		{
			List<MobaRow> list = new List<MobaRow>();
			foreach (MobaRow current in this.rows)
			{
				if (current[strName].MobaColumnText.IndexOf(strValue) != -1)
				{
					list.Add(current);
				}
			}
			List<MobaRow> result;
			if (list.Count > 0)
			{
				result = list;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public List<MobaRow> SelectRowsListEqual(string strName, string strValue)
		{
			List<MobaRow> list = new List<MobaRow>();
			foreach (MobaRow current in this.rows)
			{
				if (current[strName].MobaColumnText == strValue)
				{
					list.Add(current);
				}
			}
			List<MobaRow> result;
			if (list.Count > 0)
			{
				result = list;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public Dictionary<byte, object> TableToDictionary()
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			int num = 0;
			if (this.Count > 0)
			{
				foreach (KeyValuePair<string, MobaColumn> current in this.rows[0].RowItem)
				{
					num++;
					string[] array = new string[this.rows.Count + 1];
					array[0] = current.Key;
					int num2 = 1;
					foreach (MobaRow current2 in this.Rows)
					{
						array[num2] = current2[current.Key].MobaColumnText;
						num2++;
					}
					dictionary.Add((byte)num, array);
				}
			}
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2.Add("TableName", this.tableName);
			dictionary2.Add("TableKey", this.tableKey);
			Dictionary<byte, object> result;
			if (!dictionary.ContainsKey(0))
			{
				dictionary.Add(0, dictionary2);
				result = dictionary;
			}
			else
			{
				dictionary[0] = dictionary2;
				result = dictionary;
			}
			return result;
		}

		public Dictionary<byte, object> TableToDictionary(Dictionary<byte, object> mRang)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			int num = 1;
			if (this.Count > 0)
			{
				foreach (KeyValuePair<string, MobaColumn> current in this.rows[0].RowItem)
				{
					num++;
					string[] array = new string[this.rows.Count + 1];
					array[0] = current.Key;
					int num2 = 1;
					foreach (MobaRow current2 in this.Rows)
					{
						array[num2] = current2[current.Key].MobaColumnText;
						num2++;
					}
					dictionary.Add((byte)num, array);
				}
			}
			dictionary[0] = new Dictionary<string, string>
			{
				{
					"TableName",
					this.tableName
				},
				{
					"TableKey",
					this.tableKey
				}
			};
			dictionary[1] = mRang;
			return dictionary;
		}

		public void UpdateTable()
		{
			try
			{
				if (this.rows.Count > 0)
				{
					string str = string.Empty;
					string text = string.Empty;
					string text2 = string.Empty;
					foreach (MobaRow current in this.rows)
					{
						switch (current.RowState)
						{
						case MobaRowState.Insert:
						{
							str = "Insert into " + this.tableName + "(";
							text = ") values('";
							int num = 0;
							string[] columnsName = current.GetColumnsName();
							for (int i = 0; i < columnsName.Length; i++)
							{
								string text3 = columnsName[i];
								num++;
								if (text3 != this.tableKey)
								{
									if (num == current.GetColumnsName().Length)
									{
										str += text3;
										text = text + current[text3] + "')";
									}
									else
									{
										str = str + text3 + ",";
										text = text + current[text3] + "','";
									}
								}
							}
							text2 = str + text;
							this.listSql.Add(text2);
							break;
						}
						case MobaRowState.Update:
						{
							text2 = string.Format("Update {0} set ", this.tableName);
							int num = 0;
							string[] columnsName = current.GetColumnsName();
							for (int i = 0; i < columnsName.Length; i++)
							{
								string text3 = columnsName[i];
								num++;
								if (text3 != this.tableKey)
								{
									if (num == current.GetColumnsName().Length)
									{
										text2 += string.Format("{0}='{1}' where {2}='{3}'", new object[]
										{
											text3,
											current[text3],
											this.tableKey,
											current[this.tableKey]
										});
									}
									else
									{
										text2 += string.Format("{0}='{1}',", text3, current[text3]);
									}
								}
							}
							this.listSql.Add(text2);
							break;
						}
						}
					}
					foreach (MobaRow current in this.deleteRows)
					{
						text2 = string.Format("Delete {0} where {1}={2}", this.tableName, this.tableKey, current[this.tableKey]);
						this.listSql.Add(text2);
					}
					this.listSql.Clear();
					this.Refresh();
					this.deleteRows.Clear();
				}
			}
			catch
			{
				throw;
			}
		}
	}
}
