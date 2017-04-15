using System;

public class Asset
{
	public string Family
	{
		get;
		private set;
	}

	public bool LoadAsync
	{
		get;
		private set;
	}

	public string Name
	{
		get;
		private set;
	}

	public bool Persistent
	{
		get;
		private set;
	}

	public bool PreloadOnly
	{
		get;
		private set;
	}

	private Asset(string name, string family, bool persistent, bool loadAsync, bool preloadOnly)
	{
		this.Name = name;
		this.Family = family;
		this.Persistent = persistent;
		this.LoadAsync = this.LoadAsync;
		this.PreloadOnly = preloadOnly;
	}

	public static Asset Create(string assetName, string family, bool persistent = false, bool loadAsync = false, bool preloadOnly = false)
	{
		return new Asset(assetName, family, persistent, loadAsync, preloadOnly);
	}
}
