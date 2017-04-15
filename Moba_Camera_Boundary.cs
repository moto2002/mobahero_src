using System;
using UnityEngine;

public class Moba_Camera_Boundary : MonoBehaviour
{
	public Moba_Camera_Boundaries.BoundaryType type = Moba_Camera_Boundaries.BoundaryType.none;

	public bool isActive = true;

	private void Start()
	{
		Moba_Camera_Boundaries.AddBoundary(this, this.type);
		if (LayerMask.NameToLayer(Moba_Camera_Boundaries.boundaryLayer) != -1)
		{
			base.gameObject.layer = LayerMask.NameToLayer(Moba_Camera_Boundaries.boundaryLayer);
		}
		else
		{
			Moba_Camera_Boundaries.SetBoundaryLayerExist(false);
			base.collider.isTrigger = true;
		}
	}
}
