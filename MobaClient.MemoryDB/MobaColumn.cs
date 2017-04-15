using System;

namespace MobaClient.MemoryDB
{
	[Serializable]
	public class MobaColumn
	{
		private MobaRow myRow;

		private string outValue;

		private MobaRowState mobaColumnState;

		private string mobaColumnText;

		public MobaRow MyRow
		{
			get
			{
				return this.myRow;
			}
			set
			{
				this.myRow = value;
			}
		}

		public MobaRowState MobaColumnState
		{
			get
			{
				return this.mobaColumnState;
			}
			set
			{
				this.mobaColumnState = value;
			}
		}

		public string MobaColumnText
		{
			get
			{
				return this.mobaColumnText;
			}
			set
			{
				if (this.outValue != value.Trim())
				{
					this.outValue = value.Trim();
					this.mobaColumnText = value.Trim();
					this.mobaColumnState = MobaRowState.Update;
				}
			}
		}

		public MobaColumn()
		{
			this.mobaColumnState = MobaRowState.Noting;
			this.mobaColumnText = string.Empty;
		}

		public MobaColumn(string mText)
		{
			this.mobaColumnState = MobaRowState.Noting;
			this.mobaColumnText = mText;
		}

		public override string ToString()
		{
			return this.MobaColumnText;
		}
	}
}
