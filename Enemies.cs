using System;

public class Enemies
{
	public int position;

	public int born_index;

	public string name;

	public int level;

	public int wait;

	public int number;

	public string type = string.Empty;

	public void setAttribute(string attribute, string text)
	{
		switch (attribute)
		{
		case "position":
			this.position = int.Parse(text);
			break;
		case "born_index":
			this.born_index = int.Parse(text);
			break;
		case "name":
			this.name = text;
			break;
		case "level":
			this.level = int.Parse(text);
			break;
		case "wait":
			this.wait = int.Parse(text);
			break;
		case "number":
			this.number = int.Parse(text);
			break;
		case "type":
			this.type = text;
			break;
		}
	}

	public void dumpAttributes()
	{
	}
}
