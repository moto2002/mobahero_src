using System;
using System.IO;
using System.Security.Cryptography;

namespace MobaTools
{
	public class MD5Creater
	{
		public static string GenerateMd5Code(string filePathName)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			FileStream fileStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.Read);
			byte[] value = mD5CryptoServiceProvider.ComputeHash(fileStream);
			string result = BitConverter.ToString(value);
			fileStream.Close();
			return result;
		}
	}
}
