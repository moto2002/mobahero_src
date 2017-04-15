using System;
using System.Collections;

namespace JPush
{
	public class CustomEvent
	{
		private string _type;

		private Hashtable _arguments = new Hashtable();

		public string type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		public Hashtable arguments
		{
			get
			{
				return this._arguments;
			}
			set
			{
				this._arguments = value;
			}
		}

		public CustomEvent(string eventType = "")
		{
			this._type = eventType;
		}
	}
}
