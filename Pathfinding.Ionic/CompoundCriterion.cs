using Pathfinding.Ionic.Zip;
using System;
using System.Text;

namespace Pathfinding.Ionic
{
	internal class CompoundCriterion : SelectionCriterion
	{
		internal LogicalConjunction Conjunction;

		internal SelectionCriterion Left;

		private SelectionCriterion _Right;

		internal SelectionCriterion Right
		{
			get
			{
				return this._Right;
			}
			set
			{
				this._Right = value;
				if (value == null)
				{
					this.Conjunction = LogicalConjunction.NONE;
				}
				else if (this.Conjunction == LogicalConjunction.NONE)
				{
					this.Conjunction = LogicalConjunction.AND;
				}
			}
		}

		internal override bool Evaluate(string filename)
		{
			bool flag = this.Left.Evaluate(filename);
			switch (this.Conjunction)
			{
			case LogicalConjunction.AND:
				if (flag)
				{
					flag = this.Right.Evaluate(filename);
				}
				break;
			case LogicalConjunction.OR:
				if (!flag)
				{
					flag = this.Right.Evaluate(filename);
				}
				break;
			case LogicalConjunction.XOR:
				flag ^= this.Right.Evaluate(filename);
				break;
			default:
				throw new ArgumentException("Conjunction");
			}
			return flag;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(").Append((this.Left == null) ? "null" : this.Left.ToString()).Append(" ").Append(this.Conjunction.ToString()).Append(" ").Append((this.Right == null) ? "null" : this.Right.ToString()).Append(")");
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			bool flag = this.Left.Evaluate(entry);
			switch (this.Conjunction)
			{
			case LogicalConjunction.AND:
				if (flag)
				{
					flag = this.Right.Evaluate(entry);
				}
				break;
			case LogicalConjunction.OR:
				if (!flag)
				{
					flag = this.Right.Evaluate(entry);
				}
				break;
			case LogicalConjunction.XOR:
				flag ^= this.Right.Evaluate(entry);
				break;
			}
			return flag;
		}
	}
}
