using System;
using System.IO;

namespace Assets.Scripts.Model
{
	public class JSONData : JSONNode
	{
		private string m_Data;

		public override string Value
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		public JSONData(string aData)
		{
			this.m_Data = aData;
		}

		public JSONData(float aData)
		{
			this.AsFloat = aData;
		}

		public JSONData(double aData)
		{
			this.AsDouble = aData;
		}

		public JSONData(bool aData)
		{
			this.AsBool = aData;
		}

		public JSONData(int aData)
		{
			this.AsInt = aData;
		}

		public override string ToString()
		{
			return this.ToString(string.Empty);
		}

		public override string ToString(string aPrefix)
		{
			return aPrefix + JSONNode.Escape(this.m_Data) + aPrefix;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			JSONData jSONData = new JSONData(string.Empty);
			jSONData.AsInt = this.AsInt;
			if (jSONData.m_Data == this.m_Data)
			{
				aWriter.Write(4);
				aWriter.Write(this.AsInt);
				return;
			}
			jSONData.AsFloat = this.AsFloat;
			if (jSONData.m_Data == this.m_Data)
			{
				aWriter.Write(7);
				aWriter.Write(this.AsFloat);
				return;
			}
			jSONData.AsDouble = this.AsDouble;
			if (jSONData.m_Data == this.m_Data)
			{
				aWriter.Write(5);
				aWriter.Write(this.AsDouble);
				return;
			}
			jSONData.AsBool = this.AsBool;
			if (jSONData.m_Data == this.m_Data)
			{
				aWriter.Write(6);
				aWriter.Write(this.AsBool);
				return;
			}
			aWriter.Write(3);
			aWriter.Write(this.m_Data);
		}
	}
}
