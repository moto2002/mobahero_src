using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InvitationCooldown : Singleton<InvitationCooldown>
{
	public class CdRecord
	{
		public float ExpiresAt;

		public long FriendId;
	}

	public const float Cd = 17f;

	private readonly Dictionary<long, InvitationCooldown.CdRecord> _records = new Dictionary<long, InvitationCooldown.CdRecord>();

	public InvitationCooldown.CdRecord Get(long friendId)
	{
		InvitationCooldown.CdRecord cdRecord;
		if (!this._records.TryGetValue(friendId, out cdRecord))
		{
			return null;
		}
		if (cdRecord.ExpiresAt < Time.realtimeSinceStartup)
		{
			this._records.Remove(friendId);
			return null;
		}
		return cdRecord;
	}

	public void Invite(long friendId)
	{
		this._records[friendId] = new InvitationCooldown.CdRecord
		{
			ExpiresAt = Time.realtimeSinceStartup + 17f,
			FriendId = friendId
		};
	}

	public void Reset()
	{
		this._records.Clear();
	}
}
