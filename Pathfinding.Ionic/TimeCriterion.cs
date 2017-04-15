using Pathfinding.Ionic.Zip;
using System;
using System.IO;
using System.Text;

namespace Pathfinding.Ionic
{
	internal class TimeCriterion : SelectionCriterion
	{
		internal ComparisonOperator Operator;

		internal WhichTime Which;

		internal DateTime Time;

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Which.ToString()).Append(" ").Append(EnumUtil.GetDescription(this.Operator)).Append(" ").Append(this.Time.ToString("yyyy-MM-dd-HH:mm:ss"));
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			DateTime x;
			switch (this.Which)
			{
			case WhichTime.atime:
				x = File.GetLastAccessTime(filename).ToUniversalTime();
				break;
			case WhichTime.mtime:
				x = File.GetLastWriteTime(filename).ToUniversalTime();
				break;
			case WhichTime.ctime:
				x = File.GetCreationTime(filename).ToUniversalTime();
				break;
			default:
				throw new ArgumentException("Operator");
			}
			return this._Evaluate(x);
		}

		private bool _Evaluate(DateTime x)
		{
			bool result;
			switch (this.Operator)
			{
			case ComparisonOperator.GreaterThan:
				result = (x > this.Time);
				break;
			case ComparisonOperator.GreaterThanOrEqualTo:
				result = (x >= this.Time);
				break;
			case ComparisonOperator.LesserThan:
				result = (x < this.Time);
				break;
			case ComparisonOperator.LesserThanOrEqualTo:
				result = (x <= this.Time);
				break;
			case ComparisonOperator.EqualTo:
				result = (x == this.Time);
				break;
			case ComparisonOperator.NotEqualTo:
				result = (x != this.Time);
				break;
			default:
				throw new ArgumentException("Operator");
			}
			return result;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			DateTime x;
			switch (this.Which)
			{
			case WhichTime.atime:
				x = entry.AccessedTime;
				break;
			case WhichTime.mtime:
				x = entry.ModifiedTime;
				break;
			case WhichTime.ctime:
				x = entry.CreationTime;
				break;
			default:
				throw new ArgumentException("??time");
			}
			return this._Evaluate(x);
		}
	}
}
