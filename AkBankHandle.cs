using System;
using UnityEngine;

public class AkBankHandle
{
	public int m_RefCount;

	public uint m_BankID;

	public string bankName;

	public AkCallbackManager.BankCallback bankCallback;

	public int RefCount
	{
		get
		{
			return this.m_RefCount;
		}
	}

	public AkBankHandle(string name)
	{
		this.bankName = name;
		this.bankCallback = null;
	}

	public bool LoadBank(bool ignoreError = false)
	{
		if (this.m_RefCount == 0)
		{
			AKRESULT aKRESULT = AkSoundEngine.LoadBank(this.bankName, -1, out this.m_BankID);
			if (aKRESULT != AKRESULT.AK_Success)
			{
				if (!ignoreError)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"WwiseUnity: Bank ",
						this.bankName,
						" failed to load (",
						aKRESULT.ToString(),
						")"
					}));
				}
				return false;
			}
		}
		return true;
	}

	public void LoadBankAsync(AkCallbackManager.BankCallback callback = null)
	{
		if (this.m_RefCount == 0)
		{
			this.bankCallback = callback;
			AkSoundEngine.LoadBank(this.bankName, new AkCallbackManager.BankCallback(AkBankManager.GlobalBankCallback), this, -1, out this.m_BankID);
		}
		this.IncRef();
	}

	public void IncRef()
	{
		this.m_RefCount++;
	}

	public void DecRef()
	{
		this.m_RefCount--;
		if (this.m_RefCount == 0)
		{
			AkBankManager.BanksToUnload.Add(this.m_BankID);
		}
	}
}
