using System;
using System.Diagnostics;
using UnityEngine;

public class DotRestricter : NewMono
{
	public const string LAYER_TRANSPARENTFX = "TransparentFX";

	private const float THICK_NESS = 1f;

	[SerializeField]
	private DotRestricter _nextDot;

	[SerializeField]
	private GameObject _col;

	public DotRestricter nextDot
	{
		get
		{
			return this._nextDot;
		}
		set
		{
			this._nextDot = value;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	[Conditional("UNITY_EDITOR")]
	public void buildCollider()
	{
		Vector3 localPosition = base.trans.localPosition;
		localPosition.y = 0f;
		base.trans.localPosition = localPosition;
		if (this._col != null)
		{
			UnityEngine.Object.DestroyImmediate(this._col);
		}
		this._col = new GameObject();
		this._col.layer = LayerMask.NameToLayer("TransparentFX");
		this._col.name = "Boundary Collider";
		Transform transform = this._col.transform;
		transform.parent = base.trans.parent;
		transform.position = (base.trans.position + this._nextDot.trans.position) / 2f;
		BoxCollider boxCollider = this._col.AddComponent<BoxCollider>();
		float x = Vector3.Distance(base.trans.position, this.nextDot.trans.position) + 0.01f;
		boxCollider.size = new Vector3(x, 30f, 1f);
		boxCollider.center = new Vector3(0f, 0f, -0.5f);
		Vector3 right = this._nextDot.trans.position - base.trans.position;
		right.y = 0f;
		transform.right = right;
		base.rb.constraints = RigidbodyConstraints.FreezeAll;
		base.rb.useGravity = false;
	}

	[Conditional("UNITY_EDITOR")]
	public void deleteCollider()
	{
		UnityEngine.Object.DestroyImmediate(this._col);
	}
}
