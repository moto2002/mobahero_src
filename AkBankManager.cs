using System;
using System.Collections.Generic;
using System.Threading;

public static class AkBankManager
{
	private static Dictionary<string, AkBankHandle> m_BankHandles = new Dictionary<string, AkBankHandle>();

	public static List<uint> BanksToUnload = new List<uint>();

	private static Mutex m_Mutex = new Mutex();

	public static void DoUnloadBanks()
	{
		for (int i = 0; i < AkBankManager.BanksToUnload.Count; i++)
		{
			AkSoundEngine.UnloadBank(AkBankManager.BanksToUnload[i], IntPtr.Zero, null, null);
		}
		AkBankManager.BanksToUnload.Clear();
	}

	public static void Reset()
	{
		AkBankManager.m_BankHandles.Clear();
		AkBankManager.BanksToUnload.Clear();
	}

	public static void GlobalBankCallback(uint in_bankID, IntPtr in_pInMemoryBankPtr, AKRESULT in_eLoadResult, uint in_memPoolId, object in_Cookie)
	{
		AkBankManager.m_Mutex.WaitOne();
		AkBankHandle akBankHandle = (AkBankHandle)in_Cookie;
		AkCallbackManager.BankCallback bankCallback = akBankHandle.bankCallback;
		if (in_eLoadResult != AKRESULT.AK_Success)
		{
			AkBankManager.m_BankHandles.Remove(akBankHandle.bankName);
		}
		AkBankManager.m_Mutex.ReleaseMutex();
		if (bankCallback != null)
		{
			bankCallback(in_bankID, in_pInMemoryBankPtr, in_eLoadResult, in_memPoolId, null);
		}
	}

	public static bool LoadBank(string name, int skin = 0)
	{
		if (name.Contains("[]"))
		{
			return false;
		}
		if (skin > 0)
		{
			name += skin;
		}
		bool result = false;
		AkBankManager.m_Mutex.WaitOne();
		AkBankHandle akBankHandle = null;
		if (!AkBankManager.m_BankHandles.TryGetValue(name, out akBankHandle))
		{
			akBankHandle = new AkBankHandle(name);
			AkBankManager.m_BankHandles.Add(name, akBankHandle);
			result = akBankHandle.LoadBank(skin > 0);
			AkBankManager.m_Mutex.ReleaseMutex();
		}
		else
		{
			akBankHandle.IncRef();
			AkBankManager.m_Mutex.ReleaseMutex();
		}
		return result;
	}

	public static void LoadBankAsync(string name, AkCallbackManager.BankCallback callback = null)
	{
		AkBankManager.m_Mutex.WaitOne();
		AkBankHandle akBankHandle = null;
		if (!AkBankManager.m_BankHandles.TryGetValue(name, out akBankHandle))
		{
			akBankHandle = new AkBankHandle(name);
			AkBankManager.m_BankHandles.Add(name, akBankHandle);
			AkBankManager.m_Mutex.ReleaseMutex();
			akBankHandle.LoadBankAsync(callback);
		}
		else
		{
			akBankHandle.IncRef();
			AkBankManager.m_Mutex.ReleaseMutex();
		}
	}

	public static void UnloadBank(string name, bool realy = false, int skin = 0)
	{
		if (name.Equals("[]"))
		{
			return;
		}
		if (skin > 0)
		{
			if (!AkBankManager.m_BankHandles.ContainsKey(name + skin))
			{
				return;
			}
			name += skin;
		}
		AkBankManager.m_Mutex.WaitOne();
		AkBankHandle akBankHandle = null;
		if (AkBankManager.m_BankHandles.TryGetValue(name, out akBankHandle))
		{
			if (!realy)
			{
				akBankHandle.DecRef();
				if (akBankHandle.RefCount == 0)
				{
					AkBankManager.m_BankHandles.Remove(name);
				}
			}
			else
			{
				akBankHandle.m_RefCount = 0;
				AkSoundEngine.UnloadBank(akBankHandle.m_BankID, IntPtr.Zero, null, null);
				AkBankManager.m_BankHandles.Remove(name);
			}
		}
		AkBankManager.m_Mutex.ReleaseMutex();
	}
}
