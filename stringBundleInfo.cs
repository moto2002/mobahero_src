using System;

public class stringBundleInfo
{
	public string[] BackupBundles;

	private string m_bundleName;

	public int NumberOfBundles;

	public int NumberOfDownloadableBundles;

	public int NumberOfLocaleBundles;

	public int NumberOfDownloadableLocaleBundles;

	public Type TypeOf;

	public bool Updatable;

	public string BundleName
	{
		get
		{
			return this.m_bundleName;
		}
		set
		{
			this.m_bundleName = value;
		}
	}

	public stringBundleInfo()
	{
		this.NumberOfBundles = 1;
		this.NumberOfLocaleBundles = 1;
	}
}
