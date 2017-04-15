using System;
using System.Collections.Generic;
using UnityEngine;

public class NewMono : MonoBehaviour
{
	private Transform _trans;

	private AudioSource _as;

	private Animation _anim;

	private Animator _mator;

	private Rigidbody _rb;

	private lsAnim _lsAnim;

	private Renderer _rd;

	private Dictionary<string, GameObject> _cachedChildren = null;

	public Transform trans
	{
		get
		{
			Transform result;
			if (base.gameObject != null)
			{
				if (this._trans == null)
				{
					this._trans = base.GetComponent<Transform>();
				}
				result = this._trans;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}

	public AudioSource audioSource
	{
		get
		{
			if (this._as == null)
			{
				this._as = this.getComponent<AudioSource>(true);
			}
			return this._as;
		}
	}

	public Animation anim
	{
		get
		{
			if (this._anim == null)
			{
				this._anim = this.getComponent<Animation>(true);
			}
			return this._anim;
		}
	}

	public Animator mator
	{
		get
		{
			if (this._mator == null)
			{
				this._mator = this.getComponent<Animator>(true);
			}
			return this._mator;
		}
	}

	public Rigidbody rb
	{
		get
		{
			if (this._rb == null)
			{
				this._rb = this.getComponent<Rigidbody>(true);
			}
			return this._rb;
		}
	}

	public lsAnim lsAnim
	{
		get
		{
			if (this._lsAnim == null)
			{
				this._lsAnim = this.getComponent<lsAnim>(true);
			}
			return this._lsAnim;
		}
	}

	public Renderer rd
	{
		get
		{
			if (this._rd == null)
			{
				this._rd = this.getComponent<Renderer>(true);
			}
			return this._rd;
		}
	}

	protected void guiLog(int width, int height, object log)
	{
		Vector2 vector = Camera.main.WorldToScreenPoint(this.trans.position);
		GUI.TextArea(new Rect(vector.x - (float)(width / 2), (float)Screen.height - vector.y - (float)(height / 2), (float)width, (float)height), log.ToString());
	}

	public T getComponent<T>(bool addIfNull = true) where T : Component
	{
		T t = base.GetComponent<T>();
		if (t == null && addIfNull)
		{
			t = base.gameObject.AddComponent<T>();
		}
		return t;
	}

	public T getComponentInParent<T>() where T : Component
	{
		Transform transform = this.trans;
		T result;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				result = component;
				return result;
			}
			transform = transform.parent;
		}
		result = default(T);
		return result;
	}

	protected GameObject getChild(string childName)
	{
		if (this._cachedChildren == null)
		{
			this._cachedChildren = new Dictionary<string, GameObject>();
		}
		GameObject result;
		if (!this._cachedChildren.ContainsKey(childName))
		{
			Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				if (transform.name == childName)
				{
					this._cachedChildren.Add(childName, transform.gameObject);
					result = transform.gameObject;
					return result;
				}
			}
			throw new NotFoundException(string.Format("{0,-15}'s child gameObject:{1,-15} is not found!", this.trans.parent.name, childName));
		}
		result = this._cachedChildren[childName];
		return result;
	}

	public static int time2Frame(float time, float frameRate)
	{
		return Mathf.CeilToInt(time * frameRate);
	}

	public static float frame2Time(int frame, float frameRate)
	{
		return (float)frame / frameRate;
	}
}
