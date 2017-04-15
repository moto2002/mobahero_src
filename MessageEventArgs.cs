using System;
using System.Collections.Generic;

public class MessageEventArgs : EventArgs
{
	public Dictionary<string, string> messages;

	public MessageEventArgs()
	{
		this.messages = new Dictionary<string, string>();
	}

	public MessageEventArgs(MessageEventArgs copy)
	{
		this.messages = new Dictionary<string, string>(copy.messages);
	}

	public void AddMessage(string _key, string _value)
	{
		this.messages.Add(_key, _value);
	}

	public void AddMessageReplace(string _key, string _value)
	{
		if (this.messages.ContainsKey(_key))
		{
			this.messages[_key] = _value;
		}
		else
		{
			this.messages.Add(_key, _value);
		}
	}

	public void ClearMessage()
	{
		this.messages.Clear();
	}

	public bool ContainMessage(string _key)
	{
		return this.messages.ContainsKey(_key);
	}

	public string GetMessage(string _key)
	{
		if (!this.messages.ContainsKey(_key))
		{
			return null;
		}
		return this.messages[_key];
	}

	public void RemoveMessage(string _key)
	{
		this.messages.Remove(_key);
	}

	public void SetMessage(string _key, string _value)
	{
		this.messages[_key] = _value;
	}
}
