using System;

namespace Pathfinding.Ionic.Zip
{
	public enum ZipEntrySource
	{
		None,
		FileSystem,
		Stream,
		ZipFile,
		WriteDelegate,
		JitStream,
		ZipOutputStream
	}
}
