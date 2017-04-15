using Pathfinding.Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Pathfinding.Ionic
{
	public class FileSelector
	{
		private enum ParseState
		{
			Start,
			OpenParen,
			CriterionDone,
			ConjunctionPending,
			Whitespace
		}

		private static class RegexAssertions
		{
			public static readonly string PrecededByOddNumberOfSingleQuotes = "(?<=(?:[^']*'[^']*')*'[^']*)";

			public static readonly string FollowedByOddNumberOfSingleQuotesAndLineEnd = "(?=[^']*'(?:[^']*'[^']*')*[^']*$)";

			public static readonly string PrecededByEvenNumberOfSingleQuotes = "(?<=(?:[^']*'[^']*')*[^']*)";

			public static readonly string FollowedByEvenNumberOfSingleQuotesAndLineEnd = "(?=(?:[^']*'[^']*')*[^']*$)";
		}

		internal SelectionCriterion _Criterion;

		public string SelectionCriteria
		{
			get
			{
				if (this._Criterion == null)
				{
					return null;
				}
				return this._Criterion.ToString();
			}
			set
			{
				if (value == null)
				{
					this._Criterion = null;
				}
				else if (value.Trim() == string.Empty)
				{
					this._Criterion = null;
				}
				else
				{
					this._Criterion = FileSelector._ParseCriterion(value);
				}
			}
		}

		public bool TraverseReparsePoints
		{
			get;
			set;
		}

		public FileSelector(string selectionCriteria) : this(selectionCriteria, true)
		{
		}

		public FileSelector(string selectionCriteria, bool traverseDirectoryReparsePoints)
		{
			if (!string.IsNullOrEmpty(selectionCriteria))
			{
				this._Criterion = FileSelector._ParseCriterion(selectionCriteria);
			}
			this.TraverseReparsePoints = traverseDirectoryReparsePoints;
		}

		private static string NormalizeCriteriaExpression(string source)
		{
			string[][] array = new string[][]
			{
				new string[]
				{
					"([^']*)\\(\\(([^']+)",
					"$1( ($2"
				},
				new string[]
				{
					"(.)\\)\\)",
					"$1) )"
				},
				new string[]
				{
					"\\((\\S)",
					"( $1"
				},
				new string[]
				{
					"(\\S)\\)",
					"$1 )"
				},
				new string[]
				{
					"^\\)",
					" )"
				},
				new string[]
				{
					"(\\S)\\(",
					"$1 ("
				},
				new string[]
				{
					"\\)(\\S)",
					") $1"
				},
				new string[]
				{
					"(=)('[^']*')",
					"$1 $2"
				},
				new string[]
				{
					"([^ !><])(>|<|!=|=)",
					"$1 $2"
				},
				new string[]
				{
					"(>|<|!=|=)([^ =])",
					"$1 $2"
				},
				new string[]
				{
					"/",
					"\\"
				}
			};
			string input = source;
			for (int i = 0; i < array.Length; i++)
			{
				string pattern = FileSelector.RegexAssertions.PrecededByEvenNumberOfSingleQuotes + array[i][0] + FileSelector.RegexAssertions.FollowedByEvenNumberOfSingleQuotesAndLineEnd;
				input = Regex.Replace(input, pattern, array[i][1]);
			}
			string pattern2 = "/" + FileSelector.RegexAssertions.FollowedByOddNumberOfSingleQuotesAndLineEnd;
			input = Regex.Replace(input, pattern2, "\\");
			pattern2 = " " + FileSelector.RegexAssertions.FollowedByOddNumberOfSingleQuotesAndLineEnd;
			return Regex.Replace(input, pattern2, "\u0006");
		}

		private static SelectionCriterion _ParseCriterion(string s)
		{
			if (s == null)
			{
				return null;
			}
			s = FileSelector.NormalizeCriteriaExpression(s);
			if (s.IndexOf(" ") == -1)
			{
				s = "name = " + s;
			}
			string[] array = s.Trim().Split(new char[]
			{
				' ',
				'\t'
			});
			if (array.Length < 3)
			{
				throw new ArgumentException(s);
			}
			SelectionCriterion selectionCriterion = null;
			Stack<FileSelector.ParseState> stack = new Stack<FileSelector.ParseState>();
			Stack<SelectionCriterion> stack2 = new Stack<SelectionCriterion>();
			stack.Push(FileSelector.ParseState.Start);
			int i = 0;
			while (i < array.Length)
			{
				string text = array[i].ToLower();
				string text2 = text;
				if (text2 != null)
				{
					if (FileSelector.<>f__switch$map0 == null)
					{
						FileSelector.<>f__switch$map0 = new Dictionary<string, int>(14)
						{
							{
								"and",
								0
							},
							{
								"xor",
								0
							},
							{
								"or",
								0
							},
							{
								"(",
								1
							},
							{
								")",
								2
							},
							{
								"atime",
								3
							},
							{
								"ctime",
								3
							},
							{
								"mtime",
								3
							},
							{
								"length",
								4
							},
							{
								"size",
								4
							},
							{
								"filename",
								5
							},
							{
								"name",
								5
							},
							{
								"type",
								6
							},
							{
								string.Empty,
								7
							}
						};
					}
					int num;
					if (FileSelector.<>f__switch$map0.TryGetValue(text2, out num))
					{
						FileSelector.ParseState parseState;
						switch (num)
						{
						case 0:
						{
							parseState = stack.Peek();
							if (parseState != FileSelector.ParseState.CriterionDone)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							if (array.Length <= i + 3)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							LogicalConjunction conjunction = (LogicalConjunction)Enum.Parse(typeof(LogicalConjunction), array[i].ToUpper(), true);
							selectionCriterion = new CompoundCriterion
							{
								Left = selectionCriterion,
								Right = null,
								Conjunction = conjunction
							};
							stack.Push(parseState);
							stack.Push(FileSelector.ParseState.ConjunctionPending);
							stack2.Push(selectionCriterion);
							break;
						}
						case 1:
							parseState = stack.Peek();
							if (parseState != FileSelector.ParseState.Start && parseState != FileSelector.ParseState.ConjunctionPending && parseState != FileSelector.ParseState.OpenParen)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							if (array.Length <= i + 4)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							stack.Push(FileSelector.ParseState.OpenParen);
							break;
						case 2:
							parseState = stack.Pop();
							if (stack.Peek() != FileSelector.ParseState.OpenParen)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							stack.Pop();
							stack.Push(FileSelector.ParseState.CriterionDone);
							break;
						case 3:
						{
							if (array.Length <= i + 2)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							DateTime dateTime;
							try
							{
								dateTime = DateTime.ParseExact(array[i + 2], "yyyy-MM-dd-HH:mm:ss", null);
							}
							catch (FormatException)
							{
								try
								{
									dateTime = DateTime.ParseExact(array[i + 2], "yyyy/MM/dd-HH:mm:ss", null);
								}
								catch (FormatException)
								{
									try
									{
										dateTime = DateTime.ParseExact(array[i + 2], "yyyy/MM/dd", null);
									}
									catch (FormatException)
									{
										try
										{
											dateTime = DateTime.ParseExact(array[i + 2], "MM/dd/yyyy", null);
										}
										catch (FormatException)
										{
											dateTime = DateTime.ParseExact(array[i + 2], "yyyy-MM-dd", null);
										}
									}
								}
							}
							dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
							selectionCriterion = new TimeCriterion
							{
								Which = (WhichTime)Enum.Parse(typeof(WhichTime), array[i], true),
								Operator = (ComparisonOperator)EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]),
								Time = dateTime
							};
							i += 2;
							stack.Push(FileSelector.ParseState.CriterionDone);
							break;
						}
						case 4:
						{
							if (array.Length <= i + 2)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							string text3 = array[i + 2];
							long size;
							if (text3.ToUpper().EndsWith("K"))
							{
								size = long.Parse(text3.Substring(0, text3.Length - 1)) * 1024L;
							}
							else if (text3.ToUpper().EndsWith("KB"))
							{
								size = long.Parse(text3.Substring(0, text3.Length - 2)) * 1024L;
							}
							else if (text3.ToUpper().EndsWith("M"))
							{
								size = long.Parse(text3.Substring(0, text3.Length - 1)) * 1024L * 1024L;
							}
							else if (text3.ToUpper().EndsWith("MB"))
							{
								size = long.Parse(text3.Substring(0, text3.Length - 2)) * 1024L * 1024L;
							}
							else if (text3.ToUpper().EndsWith("G"))
							{
								size = long.Parse(text3.Substring(0, text3.Length - 1)) * 1024L * 1024L * 1024L;
							}
							else if (text3.ToUpper().EndsWith("GB"))
							{
								size = long.Parse(text3.Substring(0, text3.Length - 2)) * 1024L * 1024L * 1024L;
							}
							else
							{
								size = long.Parse(array[i + 2]);
							}
							selectionCriterion = new SizeCriterion
							{
								Size = size,
								Operator = (ComparisonOperator)EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1])
							};
							i += 2;
							stack.Push(FileSelector.ParseState.CriterionDone);
							break;
						}
						case 5:
						{
							if (array.Length <= i + 2)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							ComparisonOperator comparisonOperator = (ComparisonOperator)EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]);
							if (comparisonOperator != ComparisonOperator.NotEqualTo && comparisonOperator != ComparisonOperator.EqualTo)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							string text4 = array[i + 2];
							if (text4.StartsWith("'") && text4.EndsWith("'"))
							{
								text4 = text4.Substring(1, text4.Length - 2).Replace("\u0006", " ");
							}
							selectionCriterion = new NameCriterion
							{
								MatchingFileSpec = text4,
								Operator = comparisonOperator
							};
							i += 2;
							stack.Push(FileSelector.ParseState.CriterionDone);
							break;
						}
						case 6:
						{
							if (array.Length <= i + 2)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							ComparisonOperator comparisonOperator2 = (ComparisonOperator)EnumUtil.Parse(typeof(ComparisonOperator), array[i + 1]);
							if (comparisonOperator2 != ComparisonOperator.NotEqualTo && comparisonOperator2 != ComparisonOperator.EqualTo)
							{
								throw new ArgumentException(string.Join(" ", array, i, array.Length - i));
							}
							selectionCriterion = new TypeCriterion
							{
								AttributeString = array[i + 2],
								Operator = comparisonOperator2
							};
							i += 2;
							stack.Push(FileSelector.ParseState.CriterionDone);
							break;
						}
						case 7:
							stack.Push(FileSelector.ParseState.Whitespace);
							break;
						default:
							goto IL_7BA;
						}
						parseState = stack.Peek();
						if (parseState == FileSelector.ParseState.CriterionDone)
						{
							stack.Pop();
							if (stack.Peek() == FileSelector.ParseState.ConjunctionPending)
							{
								while (stack.Peek() == FileSelector.ParseState.ConjunctionPending)
								{
									CompoundCriterion compoundCriterion = stack2.Pop() as CompoundCriterion;
									compoundCriterion.Right = selectionCriterion;
									selectionCriterion = compoundCriterion;
									stack.Pop();
									parseState = stack.Pop();
									if (parseState != FileSelector.ParseState.CriterionDone)
									{
										throw new ArgumentException("??");
									}
								}
							}
							else
							{
								stack.Push(FileSelector.ParseState.CriterionDone);
							}
						}
						if (parseState == FileSelector.ParseState.Whitespace)
						{
							stack.Pop();
						}
						i++;
						continue;
					}
				}
				IL_7BA:
				throw new ArgumentException("'" + array[i] + "'");
			}
			return selectionCriterion;
		}

		public override string ToString()
		{
			return "FileSelector(" + this._Criterion.ToString() + ")";
		}

		private bool Evaluate(string filename)
		{
			return this._Criterion.Evaluate(filename);
		}

		[Conditional("SelectorTrace")]
		private void SelectorTrace(string format, params object[] args)
		{
			if (this._Criterion != null && this._Criterion.Verbose)
			{
				Console.WriteLine(format, args);
			}
		}

		public ICollection<string> SelectFiles(string directory)
		{
			return this.SelectFiles(directory, false);
		}

		public ReadOnlyCollection<string> SelectFiles(string directory, bool recurseDirectories)
		{
			if (this._Criterion == null)
			{
				throw new ArgumentException("SelectionCriteria has not been set");
			}
			List<string> list = new List<string>();
			try
			{
				if (Directory.Exists(directory))
				{
					string[] files = Directory.GetFiles(directory);
					string[] array = files;
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i];
						if (this.Evaluate(text))
						{
							list.Add(text);
						}
					}
					if (recurseDirectories)
					{
						string[] directories = Directory.GetDirectories(directory);
						string[] array2 = directories;
						for (int j = 0; j < array2.Length; j++)
						{
							string text2 = array2[j];
							if (this.TraverseReparsePoints)
							{
								if (this.Evaluate(text2))
								{
									list.Add(text2);
								}
								list.AddRange(this.SelectFiles(text2, recurseDirectories));
							}
						}
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (IOException)
			{
			}
			return list.AsReadOnly();
		}

		private bool Evaluate(ZipEntry entry)
		{
			return this._Criterion.Evaluate(entry);
		}

		public ICollection<ZipEntry> SelectEntries(ZipFile zip)
		{
			if (zip == null)
			{
				throw new ArgumentNullException("zip");
			}
			List<ZipEntry> list = new List<ZipEntry>();
			foreach (ZipEntry current in zip)
			{
				if (this.Evaluate(current))
				{
					list.Add(current);
				}
			}
			return list;
		}

		public ICollection<ZipEntry> SelectEntries(ZipFile zip, string directoryPathInArchive)
		{
			if (zip == null)
			{
				throw new ArgumentNullException("zip");
			}
			List<ZipEntry> list = new List<ZipEntry>();
			string text = (directoryPathInArchive != null) ? directoryPathInArchive.Replace("/", "\\") : null;
			if (text != null)
			{
				while (text.EndsWith("\\"))
				{
					text = text.Substring(0, text.Length - 1);
				}
			}
			foreach (ZipEntry current in zip)
			{
				if ((directoryPathInArchive == null || Path.GetDirectoryName(current.FileName) == directoryPathInArchive || Path.GetDirectoryName(current.FileName) == text) && this.Evaluate(current))
				{
					list.Add(current);
				}
			}
			return list;
		}
	}
}
