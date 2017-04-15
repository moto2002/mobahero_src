using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Pathfinding.Serialization.JsonFx
{
	public class DataWriterProvider : IDataWriterProvider
	{
		private readonly IDataWriter DefaultWriter;

		private readonly IDictionary<string, IDataWriter> WritersByExt = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);

		private readonly IDictionary<string, IDataWriter> WritersByMime = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);

		public IDataWriter DefaultDataWriter
		{
			get
			{
				return this.DefaultWriter;
			}
		}

		public DataWriterProvider(IEnumerable<IDataWriter> writers)
		{
			if (writers != null)
			{
				foreach (IDataWriter current in writers)
				{
					if (this.DefaultWriter == null)
					{
						this.DefaultWriter = current;
					}
					if (!string.IsNullOrEmpty(current.ContentType))
					{
						this.WritersByMime[current.ContentType] = current;
					}
					if (!string.IsNullOrEmpty(current.ContentType))
					{
						string key = DataWriterProvider.NormalizeExtension(current.FileExtension);
						this.WritersByExt[key] = current;
					}
				}
			}
		}

		public IDataWriter Find(string extension)
		{
			extension = DataWriterProvider.NormalizeExtension(extension);
			if (this.WritersByExt.ContainsKey(extension))
			{
				return this.WritersByExt[extension];
			}
			return null;
		}

		public IDataWriter Find(string acceptHeader, string contentTypeHeader)
		{
			foreach (string current in DataWriterProvider.ParseHeaders(acceptHeader, contentTypeHeader))
			{
				if (this.WritersByMime.ContainsKey(current))
				{
					return this.WritersByMime[current];
				}
			}
			return null;
		}

		[DebuggerHidden]
		public static IEnumerable<string> ParseHeaders(string accept, string contentType)
		{
			DataWriterProvider.<ParseHeaders>c__Iterator0 <ParseHeaders>c__Iterator = new DataWriterProvider.<ParseHeaders>c__Iterator0();
			<ParseHeaders>c__Iterator.accept = accept;
			<ParseHeaders>c__Iterator.contentType = contentType;
			<ParseHeaders>c__Iterator.<$>accept = accept;
			<ParseHeaders>c__Iterator.<$>contentType = contentType;
			DataWriterProvider.<ParseHeaders>c__Iterator0 expr_23 = <ParseHeaders>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		public static string ParseMediaType(string type)
		{
			using (IEnumerator<string> enumerator = DataWriterProvider.SplitTrim(type, ';').GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return string.Empty;
		}

		[DebuggerHidden]
		private static IEnumerable<string> SplitTrim(string source, char ch)
		{
			DataWriterProvider.<SplitTrim>c__Iterator1 <SplitTrim>c__Iterator = new DataWriterProvider.<SplitTrim>c__Iterator1();
			<SplitTrim>c__Iterator.source = source;
			<SplitTrim>c__Iterator.ch = ch;
			<SplitTrim>c__Iterator.<$>source = source;
			<SplitTrim>c__Iterator.<$>ch = ch;
			DataWriterProvider.<SplitTrim>c__Iterator1 expr_23 = <SplitTrim>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		private static string NormalizeExtension(string extension)
		{
			if (string.IsNullOrEmpty(extension))
			{
				return string.Empty;
			}
			return Path.GetExtension(extension);
		}
	}
}
