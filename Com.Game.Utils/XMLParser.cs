using System;
using System.Collections;

namespace Com.Game.Utils
{
	public static class XMLParser
	{
		private const char LT = '<';

		private const char GT = '>';

		private const char SQR = ']';

		private const char DASH = '-';

		private const char SPACE = ' ';

		private const char QUOTE = '"';

		private const char SLASH = '/';

		private const char QMARK = '?';

		private const char EQUALS = '=';

		private const char NEWLINE = '\n';

		private const char EXCLAMATION = '!';

		public static XMLNode Parse(string content)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			XMLNodeList xMLNodeList = new XMLNodeList();
			XMLNode xMLNode = new XMLNode();
			xMLNode["_text"] = string.Empty;
			XMLNode xMLNode2 = xMLNode;
			for (int i = 0; i < content.Length; i++)
			{
				char c3;
				char c2;
				char c = c2 = (c3 = '\0');
				char c4 = content[i];
				if (i + 1 < content.Length)
				{
					c2 = content[i + 1];
				}
				if (i + 2 < content.Length)
				{
					c = content[i + 2];
				}
				if (i > 0)
				{
					c3 = content[i - 1];
				}
				if (flag)
				{
					if (c4 == '?' && c2 == '>')
					{
						flag = false;
						i++;
					}
				}
				else if (!flag8 && c4 == '<' && c2 == '?')
				{
					flag = true;
				}
				else if (flag2)
				{
					if (c3 == '-' && c4 == '-' && c2 == '>')
					{
						flag2 = false;
						i++;
					}
				}
				else if (!flag8 && c4 == '<' && c2 == '!')
				{
					if (content.Length > i + 9 && content.Substring(i, 9) == "<![CDATA[")
					{
						flag3 = true;
						i += 8;
					}
					else
					{
						flag2 = true;
					}
				}
				else if (flag3)
				{
					if (c4 == ']' && c2 == ']' && c == '>')
					{
						flag3 = false;
						i += 2;
					}
					else
					{
						text4 += c4;
					}
				}
				else if (flag4)
				{
					if (flag5)
					{
						if (c4 == ' ')
						{
							flag5 = false;
						}
						else if (c4 == '>')
						{
							flag5 = false;
							flag4 = false;
						}
						if (!flag5 && text3.Length > 0)
						{
							if (text3[0] == '/')
							{
								if (text4.Length > 0)
								{
									XMLNode xMLNode3;
									Hashtable expr_222 = xMLNode3 = xMLNode2;
									string key;
									object expr_22A = key = "_text";
									object arg = xMLNode3[key];
									expr_222[expr_22A] = arg + text4;
								}
								text4 = string.Empty;
								text3 = string.Empty;
								xMLNode2 = xMLNodeList.Pop();
							}
							else
							{
								if (text4.Length > 0)
								{
									XMLNode xMLNode4;
									Hashtable expr_271 = xMLNode4 = xMLNode2;
									string key;
									object expr_279 = key = "_text";
									object arg = xMLNode4[key];
									expr_271[expr_279] = arg + text4;
								}
								text4 = string.Empty;
								XMLNode xMLNode5 = new XMLNode();
								xMLNode5["_text"] = string.Empty;
								xMLNode5["_name"] = text3;
								if (!xMLNode2.ContainsKey(text3))
								{
									xMLNode2[text3] = new XMLNodeList();
								}
								XMLNodeList xMLNodeList2 = xMLNode2[text3] as XMLNodeList;
								xMLNodeList2.Push(xMLNode5);
								xMLNodeList.Push(xMLNode2);
								xMLNode2 = xMLNode5;
								text3 = string.Empty;
							}
						}
						else
						{
							text3 += c4;
						}
					}
					else if (!flag8 && c4 == '/' && c2 == '>')
					{
						flag4 = false;
						flag6 = false;
						flag7 = false;
						if (text != string.Empty)
						{
							if (text2 != string.Empty)
							{
								xMLNode2["@" + text] = text2;
							}
							else
							{
								xMLNode2["@" + text] = true;
							}
						}
						i++;
						xMLNode2 = xMLNodeList.Pop();
						text = string.Empty;
						text2 = string.Empty;
					}
					else if (!flag8 && c4 == '>')
					{
						flag4 = false;
						flag6 = false;
						flag7 = false;
						if (text != string.Empty)
						{
							xMLNode2["@" + text] = text2;
						}
						text = string.Empty;
						text2 = string.Empty;
					}
					else if (flag6)
					{
						if (c4 == ' ' || c4 == '=')
						{
							flag6 = false;
							flag7 = true;
						}
						else
						{
							text += c4;
						}
					}
					else if (flag7)
					{
						if (c4 == '"')
						{
							if (flag8)
							{
								flag7 = false;
								xMLNode2["@" + text] = text2;
								text2 = string.Empty;
								text = string.Empty;
								flag8 = false;
							}
							else
							{
								flag8 = true;
							}
						}
						else if (flag8)
						{
							text2 += c4;
						}
						else if (c4 == ' ')
						{
							flag7 = false;
							xMLNode2["@" + text] = text2;
							text2 = string.Empty;
							text = string.Empty;
						}
					}
					else if (c4 != ' ')
					{
						flag6 = true;
						text = string.Empty + c4;
						text2 = string.Empty;
						flag8 = false;
					}
				}
				else if (c4 == '<')
				{
					flag4 = true;
					flag5 = true;
				}
				else
				{
					text4 += c4;
				}
			}
			return xMLNode;
		}
	}
}
