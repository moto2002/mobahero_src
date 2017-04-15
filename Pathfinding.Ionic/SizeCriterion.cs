using Pathfinding.Ionic.Zip;
using System;
using System.IO;
using System.Text;

namespace Pathfinding.Ionic
{
	internal class SizeCriterion : SelectionCriterion
	{
		internal ComparisonOperator Operator;

		internal long Size;

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("size ").Append(EnumUtil.GetDescription(this.Operator)).Append(" ").Append(this.Size.ToString());
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			FileInfo fileInfo = new FileInfo(filename);
			return this._Evaluate(fileInfo.Length);
		}

		private bool _Evaluate(long Length)
		{
			bool result;
			switch (this.Operator)
			{
			case ComparisonOperator.GreaterThan:
				result = (Length > this.Size);
				break;
			case ComparisonOperator.GreaterThanOrEqualTo:
				result = (Length >= this.Size);
				break;
			case ComparisonOperator.LesserThan:
				result = (Length < this.Size);
				break;
			case ComparisonOperator.LesserThanOrEqualTo:
				result = (Length <= this.Size);
				break;
			case ComparisonOperator.EqualTo:
				result = (Length == this.Size);
				break;
			case ComparisonOperator.NotEqualTo:
				result = (Length != this.Size);
				break;
			default:
				throw new ArgumentException("Operator");
			}
			return result;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			return this._Evaluate(entry.UncompressedSize);
		}
	}
}
