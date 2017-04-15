using System;
using System.IO;

namespace MediaKit.Processor
{
	internal class OGGStream
	{
		public readonly bool Preload;

		public readonly string Path;

		public FileStream InFile;

		public MemoryStream InMem;

		public bool Initializing;

		public bool Ready;

		public string Error;

		public int RefCount;

		public OGGStream(string path, bool preload)
		{
			this.Preload = preload;
			this.Path = path;
			this.Initializing = true;
		}
	}
}
