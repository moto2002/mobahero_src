using System;
using UnityEngine;

public class ListenMan : MonoBehaviour
{
	public const string manId = "ListenMan";

	public AudioListener lisn;

	public Transform mainHeroTrans;

	private static ListenMan man;

	public static ListenMan Instance
	{
		get
		{
			if (ListenMan.man == null)
			{
				ListenMan.WalkMan();
			}
			return ListenMan.man;
		}
	}

	private void Awake()
	{
		AudioListener[] array = UnityEngine.Object.FindObjectsOfType<AudioListener>();
		if (array != null)
		{
			AudioListener[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				AudioListener audioListener = array2[i];
				audioListener.enabled = false;
			}
		}
		this.lisn.enabled = true;
		PlayerControlMgr.Instance.RegisterPlayerChangeCallBack("ListenMan", new PlayerControlMgr.PlayerChangeAction(this.ChangeMainHero), null);
	}

	private void OnDisable()
	{
		PlayerControlMgr.Instance.UnRegisterPlayerChangeCallBack("ListenMan", new PlayerControlMgr.PlayerChangeAction(this.ChangeMainHero));
	}

	public void ChangeMainHero()
	{
		Units player = PlayerControlMgr.Instance.GetPlayer();
		if (player != null)
		{
			base.transform.parent = player.transform;
			base.transform.localPosition = Vector3.zero;
		}
	}

	private static void WalkMan()
	{
		GameObject original = Resources.Load<GameObject>("Audio/ListenMan");
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		ListenMan.man = gameObject.GetComponent<ListenMan>();
	}
}
