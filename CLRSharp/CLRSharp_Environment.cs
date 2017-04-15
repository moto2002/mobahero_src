using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CLRSharp
{
	public class CLRSharp_Environment : ICLRSharp_Environment
	{
		private Dictionary<string, ICLRType> mapType = new Dictionary<string, ICLRType>();

		public List<Assembly> assemblylist;

		private List<string> moduleref = new List<string>();

		private Dictionary<Type, ICrossBind> crossBind = new Dictionary<Type, ICrossBind>();

		public string version
		{
			get
			{
				return "0.48.1Beta";
			}
		}

		public ICLRSharp_Logger logger
		{
			get;
			private set;
		}

		public CLRSharp_Environment(ICLRSharp_Logger logger)
		{
			this.logger = logger;
			logger.Log_Warning("CLR# Ver:" + this.version + " Inited.");
			this.RegCrossBind(new CrossBind_IEnumerable());
			this.RegCrossBind(new CrossBind_IEnumerator());
			this.RegCrossBind(new CrossBind_IDisposable());
		}

		public void LoadModule(Stream dllStream)
		{
			this.LoadModule(dllStream, null, null);
		}

		public void LoadModule(Stream dllStream, Stream pdbStream, ISymbolReaderProvider debugInfoLoader)
		{
			ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(dllStream);
			bool flag = debugInfoLoader != null && pdbStream != null;
			if (flag)
			{
				moduleDefinition.ReadSymbols(debugInfoLoader.GetSymbolReader(moduleDefinition, pdbStream));
			}
			bool hasAssemblyReferences = moduleDefinition.HasAssemblyReferences;
			if (hasAssemblyReferences)
			{
				foreach (AssemblyNameReference current in moduleDefinition.AssemblyReferences)
				{
					bool flag2 = !this.moduleref.Contains(current.Name);
					if (flag2)
					{
						this.moduleref.Add(current.Name);
					}
					bool flag3 = !this.moduleref.Contains(current.FullName);
					if (flag3)
					{
						this.moduleref.Add(current.FullName);
					}
				}
			}
			bool hasTypes = moduleDefinition.HasTypes;
			if (hasTypes)
			{
				foreach (TypeDefinition current2 in moduleDefinition.Types)
				{
					this.mapType[current2.FullName] = new Type_Common_CLRSharp(this, current2);
				}
			}
		}

		public void AddSerachAssembly(Assembly assembly)
		{
			bool flag = this.assemblylist == null;
			if (flag)
			{
				this.assemblylist = new List<Assembly>();
			}
			this.assemblylist.Add(assembly);
		}

		public void LoadModule_OnlyName(Stream dllStream)
		{
			ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(dllStream);
			bool flag = !this.moduleref.Contains(moduleDefinition.Name);
			if (flag)
			{
				this.moduleref.Add(moduleDefinition.Name);
			}
			bool hasAssemblyReferences = moduleDefinition.HasAssemblyReferences;
			if (hasAssemblyReferences)
			{
				foreach (AssemblyNameReference current in moduleDefinition.AssemblyReferences)
				{
					bool flag2 = !this.moduleref.Contains(current.Name);
					if (flag2)
					{
						this.moduleref.Add(current.Name);
					}
				}
			}
		}

		public string[] GetAllTypes()
		{
			string[] array = new string[this.mapType.Count];
			this.mapType.Keys.CopyTo(array, 0);
			return array;
		}

		public string[] GetModuleRefNames()
		{
			return this.moduleref.ToArray();
		}

		public ICLRType GetType(string fullname)
		{
			ICLRType result;
			try
			{
				ICLRType iCLRType = null;
				bool flag = this.mapType.TryGetValue(fullname, out iCLRType);
				bool flag2 = !flag;
				if (flag2)
				{
					List<ICLRType> list = new List<ICLRType>();
					bool flag3 = fullname.Contains("<>") || fullname.Contains("/");
					if (flag3)
					{
						string[] array = fullname.Split(new char[]
						{
							'/'
						});
						ICLRType iCLRType2 = this.GetType(array[0]);
						bool flag4 = iCLRType2 is ICLRType_Sharp;
						if (flag4)
						{
							for (int i = 1; i < array.Length; i++)
							{
								iCLRType2 = iCLRType2.GetNestType(this, array[i]);
							}
							result = iCLRType2;
							return result;
						}
					}
					string text = fullname;
					bool flag5 = text.Contains("<");
					if (flag5)
					{
						string text2 = "";
						int num = 0;
						int num2 = 0;
						int j = 0;
						while (j < fullname.Length)
						{
							bool flag6 = fullname[j] == '/';
							if (flag6)
							{
								goto IL_2AF;
							}
							bool flag7 = fullname[j] == '<';
							if (flag7)
							{
								bool flag8 = j != 0;
								if (flag8)
								{
									num++;
								}
								bool flag9 = num == 1;
								if (!flag9)
								{
									goto IL_2AF;
								}
								num2 = j;
								text2 += "[";
							}
							else
							{
								bool flag10 = fullname[j] == '>';
								if (flag10)
								{
									bool flag11 = num == 1;
									if (flag11)
									{
										string fullname2 = text.Substring(num2 + 1, j - num2 - 1);
										ICLRType type = this.GetType(fullname2);
										list.Add(type);
										bool flag12 = !type.IsEnum() && type is ICLRType_Sharp;
										if (flag12)
										{
											type = this.GetType(typeof(CLRSharp_Instance));
										}
										text2 = text2 + "[" + type.FullNameWithAssembly + "]";
										num2 = j;
									}
									num--;
									bool flag13 = num == 0;
									if (!flag13)
									{
										bool flag14 = num < 0;
										if (flag14)
										{
											num = 0;
										}
										goto IL_2AF;
									}
									text2 += "]";
								}
								else
								{
									bool flag15 = fullname[j] == ',';
									if (flag15)
									{
										bool flag16 = num == 1;
										if (flag16)
										{
											string fullname2 = text.Substring(num2 + 1, j - num2 - 1);
											ICLRType type2 = this.GetType(fullname2);
											list.Add(type2);
											bool flag17 = !type2.IsEnum() && type2 is ICLRType_Sharp;
											if (flag17)
											{
												type2 = this.GetType(typeof(CLRSharp_Instance));
											}
											text2 = text2 + "[" + type2.FullNameWithAssembly + "],";
											num2 = j;
										}
										goto IL_2AF;
									}
									goto IL_2AF;
								}
							}
							IL_2DE:
							j++;
							continue;
							IL_2AF:
							bool flag18 = num == 0;
							if (flag18)
							{
								text2 += text[j].ToString();
							}
							goto IL_2DE;
						}
						text = text2;
					}
					text = text.Replace('/', '+');
					Type type3 = Type.GetType(text);
					bool flag19 = type3 == null;
					if (flag19)
					{
						bool flag20 = this.assemblylist != null;
						if (flag20)
						{
							foreach (Assembly current in this.assemblylist)
							{
								type3 = current.GetType(text);
								bool flag21 = type3 != null;
								if (flag21)
								{
									break;
								}
							}
						}
						bool flag22 = type3 == null;
						if (flag22)
						{
							foreach (string current2 in this.moduleref)
							{
								type3 = Type.GetType(text + "," + current2);
								bool flag23 = type3 != null;
								if (flag23)
								{
									text = text + "," + current2;
									break;
								}
							}
						}
					}
					bool flag24 = type3 != null;
					if (flag24)
					{
						bool flag25 = !type3.FullName.Contains("CLRSharp.CLRSharp_Instance");
						if (flag25)
						{
							flag = this.mapType.TryGetValue(type3.FullName, out iCLRType);
							bool flag26 = flag;
							if (flag26)
							{
								result = iCLRType;
								return result;
							}
							iCLRType = new Type_Common_System(this, type3, list.ToArray());
							this.mapType[type3.FullName] = iCLRType;
							result = iCLRType;
							return result;
						}
						else
						{
							iCLRType = new Type_Common_System(this, type3, list.ToArray());
							this.mapType[fullname] = iCLRType;
						}
					}
				}
				result = iCLRType;
			}
			catch (Exception innerException)
			{
				throw new Exception("Error in getType:" + fullname, innerException);
			}
			return result;
		}

		public ICLRType GetType(Type systemType)
		{
			ICLRType iCLRType = null;
			bool flag = this.mapType.TryGetValue(systemType.FullName, out iCLRType);
			bool flag2 = !flag;
			if (flag2)
			{
				iCLRType = new Type_Common_System(this, systemType, null);
				this.mapType[systemType.FullName] = iCLRType;
			}
			return iCLRType;
		}

		public void RegType(ICLRType type)
		{
			this.mapType[type.FullName] = type;
		}

		public void RegCrossBind(ICrossBind bind)
		{
			this.crossBind[bind.Type] = bind;
		}

		public ICrossBind GetCrossBind(Type type)
		{
			ICrossBind result = null;
			this.crossBind.TryGetValue(type, out result);
			return result;
		}
	}
}
