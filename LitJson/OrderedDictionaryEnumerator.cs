using System;
using System.Collections;

namespace LitJson
{
	internal class OrderedDictionaryEnumerator : IEnumerator, IDictionaryEnumerator
	{
		private IEnumerator list_enumerator;

		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		public DictionaryEntry Entry
		{
			get
			{
				JsonKeyValuePair jsonKeyValuePair = this.list_enumerator.Current as JsonKeyValuePair;
				return new DictionaryEntry(jsonKeyValuePair.Key, jsonKeyValuePair.Value);
			}
		}

		public object Key
		{
			get
			{
				return (this.list_enumerator.Current as JsonKeyValuePair).Key;
			}
		}

		public object Value
		{
			get
			{
				return (this.list_enumerator.Current as JsonKeyValuePair).Value;
			}
		}

		public OrderedDictionaryEnumerator(IEnumerator enumerator)
		{
			this.list_enumerator = enumerator;
		}

		public bool MoveNext()
		{
			return this.list_enumerator.MoveNext();
		}

		public void Reset()
		{
			this.list_enumerator.Reset();
		}
	}
}
