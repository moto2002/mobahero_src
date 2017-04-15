using System;
using System.Xml;

namespace MobaClient
{
	public static class NetAddressHelper
	{
		public static string GetNetAddress(string name)
		{
			string text = string.Empty;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load("config.xml");
			XmlElement documentElement = xmlDocument.DocumentElement;
			XmlNodeList elementsByTagName = documentElement.GetElementsByTagName("NetType");
			foreach (XmlNode xmlNode in elementsByTagName)
			{
				string attribute = ((XmlElement)xmlNode).GetAttribute("name");
				Console.WriteLine(attribute);
				XmlNodeList elementsByTagName2 = ((XmlElement)xmlNode).GetElementsByTagName("IpAddress");
				if (elementsByTagName2.Count == 1 && attribute == name)
				{
					text = elementsByTagName2[0].InnerText;
					Console.WriteLine(text);
				}
			}
			return text;
		}
	}
}
