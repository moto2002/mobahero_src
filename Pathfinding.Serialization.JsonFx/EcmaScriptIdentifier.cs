using System;
using System.Collections.Generic;

namespace Pathfinding.Serialization.JsonFx
{
	public class EcmaScriptIdentifier : IJsonSerializable
	{
		private readonly string identifier;

		public string Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		public EcmaScriptIdentifier() : this(null)
		{
		}

		public EcmaScriptIdentifier(string ident)
		{
			this.identifier = ((!string.IsNullOrEmpty(ident)) ? EcmaScriptIdentifier.EnsureValidIdentifier(ident, true) : string.Empty);
		}

		void IJsonSerializable.ReadJson(JsonReader reader)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		void IJsonSerializable.WriteJson(JsonWriter writer)
		{
			if (string.IsNullOrEmpty(this.identifier))
			{
				writer.TextWriter.Write("null");
			}
			else
			{
				writer.TextWriter.Write(this.identifier);
			}
		}

		public static string EnsureValidIdentifier(string varExpr, bool nested)
		{
			return EcmaScriptIdentifier.EnsureValidIdentifier(varExpr, nested, true);
		}

		public static string EnsureValidIdentifier(string varExpr, bool nested, bool throwOnEmpty)
		{
			if (string.IsNullOrEmpty(varExpr))
			{
				if (throwOnEmpty)
				{
					throw new ArgumentException("Variable expression is empty.");
				}
				return string.Empty;
			}
			else
			{
				varExpr = varExpr.Replace(" ", string.Empty);
				if (!EcmaScriptIdentifier.IsValidIdentifier(varExpr, nested))
				{
					throw new ArgumentException("Variable expression \"" + varExpr + "\" is not supported.");
				}
				return varExpr;
			}
		}

		public static bool IsValidIdentifier(string varExpr, bool nested)
		{
			if (string.IsNullOrEmpty(varExpr))
			{
				return false;
			}
			if (nested)
			{
				string[] array = varExpr.Split(new char[]
				{
					'.'
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string varExpr2 = array2[i];
					if (!EcmaScriptIdentifier.IsValidIdentifier(varExpr2, false))
					{
						return false;
					}
				}
				return true;
			}
			if (EcmaScriptIdentifier.IsReservedWord(varExpr))
			{
				return false;
			}
			bool flag = false;
			for (int j = 0; j < varExpr.Length; j++)
			{
				char c = varExpr[j];
				if (!flag || !char.IsDigit(c))
				{
					if (!char.IsLetter(c) && c != '_' && c != '$')
					{
						return false;
					}
					flag = true;
				}
			}
			return true;
		}

		private static bool IsReservedWord(string varExpr)
		{
			if (varExpr != null)
			{
				if (EcmaScriptIdentifier.<>f__switch$map0 == null)
				{
					EcmaScriptIdentifier.<>f__switch$map0 = new Dictionary<string, int>(61)
					{
						{
							"null",
							0
						},
						{
							"false",
							0
						},
						{
							"true",
							0
						},
						{
							"break",
							0
						},
						{
							"case",
							0
						},
						{
							"catch",
							0
						},
						{
							"continue",
							0
						},
						{
							"debugger",
							0
						},
						{
							"default",
							0
						},
						{
							"delete",
							0
						},
						{
							"do",
							0
						},
						{
							"else",
							0
						},
						{
							"finally",
							0
						},
						{
							"for",
							0
						},
						{
							"function",
							0
						},
						{
							"if",
							0
						},
						{
							"in",
							0
						},
						{
							"instanceof",
							0
						},
						{
							"new",
							0
						},
						{
							"return",
							0
						},
						{
							"switch",
							0
						},
						{
							"this",
							0
						},
						{
							"throw",
							0
						},
						{
							"try",
							0
						},
						{
							"typeof",
							0
						},
						{
							"var",
							0
						},
						{
							"void",
							0
						},
						{
							"while",
							0
						},
						{
							"with",
							0
						},
						{
							"abstract",
							0
						},
						{
							"boolean",
							0
						},
						{
							"byte",
							0
						},
						{
							"char",
							0
						},
						{
							"class",
							0
						},
						{
							"const",
							0
						},
						{
							"double",
							0
						},
						{
							"enum",
							0
						},
						{
							"export",
							0
						},
						{
							"extends",
							0
						},
						{
							"final",
							0
						},
						{
							"float",
							0
						},
						{
							"goto",
							0
						},
						{
							"implements",
							0
						},
						{
							"import",
							0
						},
						{
							"int",
							0
						},
						{
							"interface",
							0
						},
						{
							"long",
							0
						},
						{
							"native",
							0
						},
						{
							"package",
							0
						},
						{
							"private",
							0
						},
						{
							"protected",
							0
						},
						{
							"public",
							0
						},
						{
							"short",
							0
						},
						{
							"static",
							0
						},
						{
							"super",
							0
						},
						{
							"synchronized",
							0
						},
						{
							"throws",
							0
						},
						{
							"transient",
							0
						},
						{
							"volatile",
							0
						},
						{
							"let",
							0
						},
						{
							"yield",
							0
						}
					};
				}
				int num;
				if (EcmaScriptIdentifier.<>f__switch$map0.TryGetValue(varExpr, out num))
				{
					if (num == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static EcmaScriptIdentifier Parse(string value)
		{
			return new EcmaScriptIdentifier(value);
		}

		public override bool Equals(object obj)
		{
			EcmaScriptIdentifier ecmaScriptIdentifier = obj as EcmaScriptIdentifier;
			if (ecmaScriptIdentifier == null)
			{
				return base.Equals(obj);
			}
			return (string.IsNullOrEmpty(this.identifier) && string.IsNullOrEmpty(ecmaScriptIdentifier.identifier)) || StringComparer.Ordinal.Equals(this.identifier, ecmaScriptIdentifier.identifier);
		}

		public override string ToString()
		{
			return this.identifier;
		}

		public override int GetHashCode()
		{
			if (this.identifier == null)
			{
				return 0;
			}
			return this.identifier.GetHashCode();
		}

		public static implicit operator string(EcmaScriptIdentifier ident)
		{
			if (ident == null)
			{
				return string.Empty;
			}
			return ident.identifier;
		}

		public static implicit operator EcmaScriptIdentifier(string ident)
		{
			return new EcmaScriptIdentifier(ident);
		}
	}
}
