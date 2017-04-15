using Pathfinding.Ionic.Zip;
using System;
using System.IO;
using System.Text;

namespace Pathfinding.Ionic
{
	internal class TypeCriterion : SelectionCriterion
	{
		private char ObjectType;

		internal ComparisonOperator Operator;

		internal string AttributeString
		{
			get
			{
				return this.ObjectType.ToString();
			}
			set
			{
				if (value.Length != 1 || (value[0] != 'D' && value[0] != 'F'))
				{
					throw new ArgumentException("Specify a single character: either D or F");
				}
				this.ObjectType = value[0];
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("type ").Append(EnumUtil.GetDescription(this.Operator)).Append(" ").Append(this.AttributeString);
			return stringBuilder.ToString();
		}

		internal override bool Evaluate(string filename)
		{
			bool flag = (this.ObjectType != 'D') ? File.Exists(filename) : Directory.Exists(filename);
			if (this.Operator != ComparisonOperator.EqualTo)
			{
				flag = !flag;
			}
			return flag;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			bool flag = (this.ObjectType != 'D') ? (!entry.IsDirectory) : entry.IsDirectory;
			if (this.Operator != ComparisonOperator.EqualTo)
			{
				flag = !flag;
			}
			return flag;
		}
	}
}
