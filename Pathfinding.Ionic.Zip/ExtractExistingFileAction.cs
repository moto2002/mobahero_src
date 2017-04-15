using System;

namespace Pathfinding.Ionic.Zip
{
	public enum ExtractExistingFileAction
	{
		Throw,
		OverwriteSilently,
		DoNotOverwrite,
		InvokeExtractProgressEvent
	}
}
