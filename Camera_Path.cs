using System;
using System.Collections;
using UnityEngine;

public class Camera_Path : MonoBehaviour
{
	public bool isOnDisable;

	public bool isTrans;

	public iTween.EaseType easetype = iTween.EaseType.linear;

	public iTween.LoopType looptype;

	public string pathname;

	public float delay;

	public float time;

	public float speed;

	public Transform[] trans;

	private Vector3[] path;

	private Hashtable args1 = new Hashtable();

	private void Awake()
	{
		if (base.GetComponent<iTweenPath>())
		{
			this.pathname = base.GetComponent<iTweenPath>().pathName;
		}
	}

	private void Start()
	{
		this.path = iTweenPath.GetPath(this.pathname);
		this.args1.Add("path", this.path);
		this.args1.Add("delay", this.delay);
		this.args1.Add("loopType", this.looptype);
		if (this.isTrans)
		{
			Transform[] array = this.trans;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				iTween.MoveTo(transform.gameObject, this.args1);
			}
		}
		else
		{
			iTween.MoveTo(base.gameObject, this.args1);
		}
	}

	private void OnEnable()
	{
		if (this.isTrans)
		{
			Transform[] array = this.trans;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				transform.gameObject.SetActive(true);
				iTween.MoveTo(transform.gameObject, this.args1);
			}
		}
		else
		{
			iTween.MoveTo(base.gameObject, this.args1);
		}
	}

	private void OnDisable()
	{
		if (this.isOnDisable)
		{
			if (this.isTrans)
			{
				Transform[] array = this.trans;
				for (int i = 0; i < array.Length; i++)
				{
					Transform transform = array[i];
					transform.gameObject.SetActive(false);
					transform.position = this.path[0];
				}
			}
			else
			{
				base.transform.localPosition = this.path[0];
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.S))
		{
		}
	}
}
