using System;

namespace Pathfinding.Serialization.JsonFx
{
	public class JsonDeserializationException : JsonSerializationException
	{
		private int index = -1;

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public JsonDeserializationException(string message, int index) : base(message)
		{
			this.index = index;
		}

		public JsonDeserializationException(string message, Exception innerException, int index) : base(message, innerException)
		{
			this.index = index;
		}

		public void GetLineAndColumn(string source, out int line, out int col)
		{
			if (source == null)
			{
				throw new ArgumentNullException();
			}
			col = 1;
			line = 1;
			bool flag = false;
			for (int i = Math.Min(this.index, source.Length); i > 0; i--)
			{
				if (!flag)
				{
					col++;
				}
				if (source[i - 1] == '\n')
				{
					line++;
					flag = true;
				}
			}
		}
	}
}
