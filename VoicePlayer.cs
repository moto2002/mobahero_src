using System;

public class VoicePlayer : MobaMono
{
	private float _initVol = 1f;

	protected void setInitVol(float initVol)
	{
		this._initVol = initVol;
		if (AudioMgr.Instance)
		{
			base.audioSource.volume = ((!AudioMgr.Instance.muteSound) ? this._initVol : 0f);
		}
	}

	protected virtual void Start()
	{
		base.addMsgLs(typeof(ToggleSoundMsg), new Action<GameMessage>(this.onToggleSound));
		if (AudioMgr.Instance)
		{
			base.audioSource.volume = ((!AudioMgr.Instance.muteSound) ? this._initVol : 0f);
		}
	}

	private void onToggleSound(GameMessage gameMsg)
	{
		ToggleSoundMsg toggleSoundMsg = gameMsg as ToggleSoundMsg;
		base.audioSource.volume = ((!toggleSoundMsg.on) ? 0f : this._initVol);
	}

	protected virtual void OnDestroy()
	{
		base.dropMsgs();
	}
}
