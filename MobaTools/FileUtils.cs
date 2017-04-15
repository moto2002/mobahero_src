using Com.Game.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MobaTools
{
	public class FileUtils
	{
		public static string projectName
		{
			get
			{
				string[] commandLineArgs = Environment.GetCommandLineArgs();
				for (int i = 0; i < commandLineArgs.Length; i++)
				{
					string text = commandLineArgs[i];
					if (text.StartsWith("project"))
					{
						return text.Split(new char[]
						{
							"-"[0]
						})[1];
					}
				}
				return string.Empty;
			}
		}

		public static void CreateFile(string path, string name, string info)
		{
			FileInfo fileInfo = new FileInfo(path + "//" + name);
			StreamWriter streamWriter;
			if (!fileInfo.Exists)
			{
				streamWriter = fileInfo.CreateText();
			}
			else
			{
				streamWriter = fileInfo.AppendText();
			}
			streamWriter.WriteLine(info);
			streamWriter.Close();
			streamWriter.Dispose();
		}

		public static void CreateModelFile(string path, string name, byte[] info, int length)
		{
			FileInfo fileInfo = new FileInfo(path + "//" + name);
			if (!fileInfo.Exists)
			{
				Stream stream = fileInfo.Create();
				stream.Write(info, 0, length);
				stream.Close();
				stream.Dispose();
				return;
			}
		}

		public static void DeleteFile(string path, string name)
		{
			File.Delete(path + "//" + name);
		}

		public static ArrayList LoadFileByArray(string path, string name)
		{
			StreamReader streamReader = null;
			try
			{
				streamReader = File.OpenText(path + "//" + name);
			}
			catch (Exception ex)
			{
				ClientLogger.Warn("warn\n" + ex.Message);
				return null;
			}
			ArrayList arrayList = new ArrayList();
			string value;
			while ((value = streamReader.ReadLine()) != null)
			{
				arrayList.Add(value);
			}
			streamReader.Close();
			streamReader.Dispose();
			return arrayList;
		}

		public static string LoadFile(string path, string name)
		{
			StreamReader streamReader = null;
			string result;
			try
			{
				streamReader = File.OpenText(path + "//" + name);
				string text = streamReader.ReadToEnd();
				streamReader.Close();
				streamReader.Dispose();
				result = text;
			}
			catch (Exception var_2_35)
			{
				streamReader.Close();
				streamReader.Dispose();
				result = null;
			}
			return result;
		}

		public static string LoadFileByLine(string path, string name)
		{
			FileInfo fileInfo = new FileInfo(path + "//" + name);
			if (!fileInfo.Exists)
			{
				return "error";
			}
			StreamReader streamReader = File.OpenText(path + "//" + name);
			string result;
			if ((result = streamReader.ReadLine()) == null)
			{
			}
			streamReader.Close();
			streamReader.Dispose();
			return result;
		}

		private static void FindFiles(string path, string filter, out string[] files)
		{
			List<string> list = new List<string>();
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (!directoryInfo.Exists)
			{
				files = null;
				return;
			}
			FileInfo[] files2 = directoryInfo.GetFiles();
			if (files2 != null)
			{
				for (int i = 0; i < files2.Length; i++)
				{
					if (filter == null || filter == string.Empty)
					{
						list.Add(files2[i].FullName);
					}
					else
					{
						string[] array = files2[i].FullName.Split(new char[]
						{
							'.'
						});
						if (array[array.Length - 1].Equals(filter))
						{
							list.Add(files2[i].FullName);
						}
					}
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			if (directories != null)
			{
				for (int j = 0; j < directories.Length; j++)
				{
					string[] array2;
					FileUtils.FindFiles(directories[j].FullName, filter, out array2);
					if (array2 != null)
					{
						for (int k = 0; k < array2.Length; k++)
						{
							if (filter == null || filter == string.Empty)
							{
								list.Add(files2[j].FullName);
							}
							else
							{
								string[] array3 = files2[k].FullName.Split(new char[]
								{
									'.'
								});
								if (array3[array3.Length - 1].Equals(filter))
								{
									list.Add(array2[k]);
								}
							}
						}
					}
				}
			}
			files = list.ToArray();
		}

		public static string[] FindFiles(string path, string filter)
		{
			string[] array;
			FileUtils.FindFiles(path, filter, out array);
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
				}
			}
			return array;
		}

		public static bool isFileExist(string path)
		{
			return File.Exists(path);
		}

		public static string GetArtFileNames(string assetPath, int trimOff)
		{
			string text = string.Empty;
			string[] files = Directory.GetFiles(assetPath);
			if (files.Length > 0)
			{
				for (int i = 0; i < files.Length; i++)
				{
					string text2 = files[i].Remove(0, assetPath.Length);
					if (!text2.Contains(".meta"))
					{
						text2 = text2.Remove(text2.Length - trimOff, trimOff);
						if (i + 1 < files.Length - 1)
						{
							text = text + text2 + ", ";
						}
						else
						{
							text += text2;
						}
					}
				}
			}
			return text;
		}

		public static string GetFiles(string assetPath, int trimOff)
		{
			string text = string.Empty;
			string[] files = Directory.GetFiles(assetPath);
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				string text3 = text2.Remove(0, assetPath.Length + 1);
				if (!text3.Contains(".meta"))
				{
					text3 = text3.Remove(text3.Length - trimOff, trimOff);
					string text4 = text;
					text = string.Concat(new object[]
					{
						text4,
						'"',
						text3,
						'"',
						", "
					});
				}
			}
			return text;
		}

		public static string[] GetFiles(string assetPath, SearchOption option = SearchOption.TopDirectoryOnly)
		{
			if (Directory.Exists(assetPath))
			{
				string[] files = Directory.GetFiles(assetPath, "*.*", option);
				List<string> list = new List<string>();
				if (files != null && files.Length > 0)
				{
					for (int i = 0; i < files.Length; i++)
					{
						if (!files[i].Contains(".meta"))
						{
							list.Add(files[i]);
						}
					}
				}
				return list.ToArray();
			}
			return null;
		}

		public static void GetAllFiles(string assetPath, out string[] filelist)
		{
			List<string> list = new List<string>();
			DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
			if (!directoryInfo.Exists)
			{
				filelist = null;
				return;
			}
			FileInfo[] files = directoryInfo.GetFiles();
			if (files != null)
			{
				for (int i = 0; i < files.Length; i++)
				{
					if (!files[i].Name.Contains(".meta") && (files[i].Name.Contains(".assetbundle") || files[i].Name.Contains(".unity3d")))
					{
						list.Add(files[i].FullName);
					}
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			if (directories != null)
			{
				for (int j = 0; j < directories.Length; j++)
				{
					string[] array;
					FileUtils.GetAllFiles(directories[j].FullName, out array);
					if (array.Length > 0)
					{
						for (int k = 0; k < array.Length; k++)
						{
							list.Add(array[k]);
						}
					}
				}
			}
			filelist = list.ToArray();
		}

		public static void DeleteFolder(string dir)
		{
			if (Directory.Exists(dir))
			{
				string[] fileSystemEntries = Directory.GetFileSystemEntries(dir);
				for (int i = 0; i < fileSystemEntries.Length; i++)
				{
					string text = fileSystemEntries[i];
					if (File.Exists(text))
					{
						FileInfo fileInfo = new FileInfo(text);
						if (fileInfo.Attributes.ToString().IndexOf("ReadOnly") != -1)
						{
							fileInfo.Attributes = FileAttributes.Normal;
						}
						fileInfo.Delete();
					}
					else if (Directory.Exists(text))
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(text);
						if (directoryInfo.GetFiles().Length != 0)
						{
							FileUtils.DeleteFolder(directoryInfo.FullName);
						}
						Directory.Delete(directoryInfo.FullName);
					}
				}
			}
		}

		public static void CopyDirectory(string abssourcePath, string absdestinationPath, string filter = "")
		{
			if (abssourcePath.EndsWith("/*"))
			{
				abssourcePath = abssourcePath.Substring(0, abssourcePath.IndexOf("/*"));
				string[] directories = Directory.GetDirectories(abssourcePath);
				for (int i = 0; i < directories.Length; i++)
				{
					string text = directories[i];
					DirectoryInfo directoryInfo = new DirectoryInfo(text);
					string absdestinationPath2 = Path.Combine(absdestinationPath, directoryInfo.Name);
					FileUtils.CopyDirectory(text, absdestinationPath2, string.Empty);
				}
			}
			else
			{
				if (abssourcePath.EndsWith("/"))
				{
					abssourcePath = abssourcePath.Substring(0, abssourcePath.Length - 1);
				}
				ClientLogger.Info("==> CopyDirectory : sourcePath = " + abssourcePath + ",destinationPath = " + absdestinationPath);
				DirectoryInfo directoryInfo2 = new DirectoryInfo(abssourcePath);
				Directory.CreateDirectory(absdestinationPath);
				FileSystemInfo[] fileSystemInfos = directoryInfo2.GetFileSystemInfos();
				for (int j = 0; j < fileSystemInfos.Length; j++)
				{
					FileSystemInfo fileSystemInfo = fileSystemInfos[j];
					if (!fileSystemInfo.Name.Contains(".svn") && !fileSystemInfo.Name.Contains(".meta"))
					{
						string text2 = Path.Combine(absdestinationPath, fileSystemInfo.Name);
						if (fileSystemInfo is DirectoryInfo)
						{
							Directory.CreateDirectory(text2);
							FileUtils.CopyDirectory(fileSystemInfo.FullName, text2, string.Empty);
						}
						else if (filter == string.Empty || fileSystemInfo.FullName.Contains(filter))
						{
							File.Copy(fileSystemInfo.FullName, text2, true);
						}
					}
				}
			}
		}

		public static void CopyFileToFolder(string sourceFile, string destinationFolder, string filter = "")
		{
			if (!Directory.Exists(destinationFolder))
			{
				Directory.CreateDirectory(destinationFolder);
			}
			FileInfo fileInfo = new FileInfo(sourceFile);
			string destFileName = destinationFolder + fileInfo.Name;
			if (filter == string.Empty || sourceFile.Contains(filter))
			{
				File.Copy(sourceFile, destFileName, true);
			}
		}
	}
}
