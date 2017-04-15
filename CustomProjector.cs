using System;
using UnityEngine;

[ExecuteInEditMode]
public class CustomProjector : MonoBehaviour
{
	public float m_nearClipPlane = 0.1f;

	public float m_farClipPlane = 5f;

	public float m_fieldOfView = 60f;

	public float m_aspectRatio = 1f;

	public bool m_isOrthographic;

	public float m_orthographicSize = 1f;

	public GameObject recieverObj;

	public GameObject targetObj;

	private static Material s_gizmoMaterial;

	public void DrawFrustum(Color color)
	{
		Debug.Log("frd " + base.transform.forward);
		Matrix4x4 lhs = this.uvfProjectionMatrix();
		Vector3 forward = base.transform.forward;
		float nearClipPlane = this.m_nearClipPlane;
		float farClipPlane = this.m_farClipPlane;
		Vector4 vector = forward;
		vector.w = -nearClipPlane - Vector3.Dot(base.transform.position, forward);
		vector /= farClipPlane - nearClipPlane;
		Vector4 v = base.transform.position + farClipPlane * forward;
		v.w = 1f;
		vector *= (lhs * v).w;
		lhs.SetRow(2, vector);
		lhs = lhs.inverse;
		Vector4 a = lhs * new Vector4(0f, 0f, 0f, 1f);
		Vector4 a2 = lhs * new Vector4(1f, 0f, 0f, 1f);
		Vector4 a3 = lhs * new Vector4(0f, 1f, 0f, 1f);
		Vector4 a4 = lhs * new Vector4(1f, 1f, 0f, 1f);
		Vector4 a5 = lhs * new Vector4(0f, 0f, 1f, 1f);
		Vector4 a6 = lhs * new Vector4(1f, 0f, 1f, 1f);
		Vector4 a7 = lhs * new Vector4(0f, 1f, 1f, 1f);
		Vector4 a8 = lhs * new Vector4(1f, 1f, 1f, 1f);
		Vector3 from = a / a.w;
		Vector3 vector2 = a2 / a2.w;
		Vector3 vector3 = a3 / a3.w;
		Vector3 vector4 = a4 / a4.w;
		Vector3 vector5 = a5 / a5.w;
		Vector3 vector6 = a6 / a6.w;
		Vector3 vector7 = a7 / a7.w;
		Vector3 to = a8 / a8.w;
		Gizmos.color = color;
		Gizmos.DrawLine(from, vector2);
		Gizmos.DrawLine(from, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(from, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, to);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector5, vector7);
		Gizmos.DrawLine(vector7, to);
		Gizmos.DrawLine(vector6, to);
	}

	private void OnDrawGizmosSelected()
	{
		this.DrawFrustum(new Color(1f, 0f, 0f, 0.25f));
	}

	public void createRecieverObj(string cnam)
	{
		RecieverObjCtrl recieverObjCtrl = this.targetObj.GetComponentInChildren<RecieverObjCtrl>();
		GameObject gameObject;
		MeshRenderer meshRenderer;
		if (recieverObjCtrl == null)
		{
			gameObject = new GameObject("recieverObj");
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			recieverObjCtrl = gameObject.AddComponent<RecieverObjCtrl>();
		}
		else
		{
			gameObject = recieverObjCtrl.gameObject;
			meshRenderer = gameObject.GetComponent<MeshRenderer>();
			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		}
		gameObject.transform.parent = this.targetObj.transform;
		recieverObjCtrl.light = base.transform;
		this.recieverObj = gameObject;
		meshRenderer.material = recieverObjCtrl.createMat();
		recieverObjCtrl.updatamat();
		recieverObjCtrl.skintarget = recieverObjCtrl.transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();
		recieverObjCtrl.setclip(cnam);
		recieverObjCtrl.updatashadowtex();
		recieverObjCtrl.initialize(gameObject.transform.localPosition, gameObject.transform.rotation);
		recieverObjCtrl.updatamesh();
		recieverObjCtrl.updataproj1(this.localProjMat());
	}

	public Mesh createRecieverMesh(Vector3[] verts)
	{
		Vector3[] array = new Vector3[4];
		Vector3[] array2 = new Vector3[4];
		int[] triangles = new int[]
		{
			0,
			1,
			2,
			2,
			1,
			3
		};
		Mesh mesh = new Mesh();
		mesh.name = "autoShadow";
		Plane plane = new Plane(new Vector3(0f, 1f, 0f), 0f);
		this.GetPlaneIntersectionT(array, plane);
		if (verts != null)
		{
			verts[0] = array[0];
		}
		verts[1] = array[1];
		verts[2] = array[2];
		verts[3] = array[3];
		array2[0] = (array2[1] = (array2[2] = (array2[3] = plane.normal)));
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		return mesh;
	}

	private void Update()
	{
		if (this.recieverObj == null)
		{
			return;
		}
		Material sharedMaterial = this.recieverObj.GetComponent<Renderer>().sharedMaterial;
		if (sharedMaterial == null)
		{
			return;
		}
		sharedMaterial.SetMatrix("_xProjector", this.localProjMat());
	}

	public Matrix4x4 uvfProjectionMatrix()
	{
		Matrix4x4 matrix4x;
		if (this.m_isOrthographic)
		{
			float num = this.m_aspectRatio * this.m_orthographicSize;
			float orthographicSize = this.m_orthographicSize;
			matrix4x = Matrix4x4.Ortho(-num, num, -orthographicSize, orthographicSize, this.m_nearClipPlane, this.m_farClipPlane);
		}
		else
		{
			matrix4x = Matrix4x4.Perspective(this.m_fieldOfView, this.m_aspectRatio, this.m_nearClipPlane, this.m_farClipPlane);
		}
		matrix4x.m00 *= 0.5f;
		matrix4x.m02 += 0.5f * matrix4x.m32;
		matrix4x.m03 += 0.5f * matrix4x.m33;
		matrix4x.m11 *= 0.5f;
		matrix4x.m12 += 0.5f * matrix4x.m32;
		matrix4x.m13 += 0.5f * matrix4x.m33;
		float num2 = 1f / (this.m_farClipPlane - this.m_nearClipPlane);
		matrix4x.m22 = num2;
		matrix4x.m23 = -num2 * this.m_nearClipPlane;
		matrix4x *= base.transform.worldToLocalMatrix;
		return matrix4x;
	}

	public Matrix4x4 prjMatrix()
	{
		Matrix4x4 lhs = this.uvfProjectionMatrix();
		Vector3 forward = base.transform.forward;
		float nearClipPlane = this.m_nearClipPlane;
		float farClipPlane = this.m_farClipPlane;
		Vector4 vector = forward;
		vector.w = -nearClipPlane - Vector3.Dot(base.transform.position, forward);
		vector /= farClipPlane - nearClipPlane;
		Vector4 v = base.transform.position + farClipPlane * forward;
		v.w = 1f;
		vector *= (lhs * v).w;
		lhs.SetRow(2, vector);
		return lhs.inverse;
	}

	public Matrix4x4 localProjMat()
	{
		Matrix4x4 lhs = default(Matrix4x4);
		Matrix4x4 rhs = default(Matrix4x4);
		Matrix4x4 rhs2 = default(Matrix4x4);
		lhs.SetRow(0, new Vector4(0.5f, 0f, 0f, 0.5f));
		lhs.SetRow(1, new Vector4(0f, 0.5f, 0f, 0.5f));
		lhs.SetRow(2, new Vector4(0f, 0f, 0.5f, 0.5f));
		lhs.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		rhs.SetRow(0, new Vector4(1f / this.m_orthographicSize, 0f, 0f, 0f));
		rhs.SetRow(1, new Vector4(0f, 1f / this.m_orthographicSize, 0f, 0f));
		rhs.SetRow(2, new Vector4(0f, 0f, -2f / (this.m_farClipPlane - this.m_nearClipPlane), 0f));
		rhs.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		rhs2 = base.transform.localToWorldMatrix.inverse;
		return lhs * rhs * rhs2;
	}

	public Matrix4x4 uvProjectionMatrix()
	{
		Matrix4x4 lhs = default(Matrix4x4);
		Matrix4x4 rhs = default(Matrix4x4);
		Matrix4x4 rhs2 = default(Matrix4x4);
		lhs.SetRow(0, new Vector4(0.5f, 0f, 0f, 0.5f));
		lhs.SetRow(1, new Vector4(0f, 0.5f, 0f, 0.5f));
		lhs.SetRow(2, new Vector4(0f, 0f, 0.5f, 0.5f));
		lhs.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		rhs.SetRow(0, new Vector4(1f / this.m_orthographicSize, 0f, 0f, 0f));
		rhs.SetRow(1, new Vector4(0f, 1f / this.m_orthographicSize, 0f, 0f));
		rhs.SetRow(2, new Vector4(0f, 0f, -2f / (this.m_farClipPlane - this.m_nearClipPlane), 0f));
		rhs.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		rhs2 = base.transform.localToWorldMatrix.inverse;
		return lhs * rhs * rhs2 * this.recieverObj.transform.localToWorldMatrix;
	}

	public void GetPlaneIntersectionT(Vector3[] vertices, Plane plane)
	{
		Vector3 forward = base.transform.forward;
		if (0f <= Vector3.Dot(forward, plane.normal))
		{
			vertices[0] = (vertices[1] = (vertices[2] = (vertices[3] = Vector3.zero)));
			return;
		}
		Vector3 right = this.targetObj.transform.right;
		Vector3 up = this.targetObj.transform.up;
		Vector3 forward2 = this.targetObj.transform.forward;
		BoxCollider boxCollider = this.targetObj.GetComponentInChildren<BoxCollider>();
		if (boxCollider == null)
		{
			Renderer componentInChildren = this.targetObj.GetComponentInChildren<Renderer>();
			boxCollider = componentInChildren.gameObject.AddComponent<BoxCollider>();
		}
		Bounds bounds = boxCollider.bounds;
		boxCollider = this.targetObj.GetComponentInChildren<BoxCollider>();
		if (boxCollider != null)
		{
			UnityEngine.Object.DestroyImmediate(boxCollider);
		}
		else
		{
			boxCollider = this.targetObj.GetComponent<BoxCollider>();
			if (boxCollider != null)
			{
				UnityEngine.Object.DestroyImmediate(boxCollider);
			}
		}
		float num = Mathf.Abs(Vector3.Dot(right, forward));
		float num2 = bounds.extents.x * Mathf.Sqrt(Mathf.Max(0f, 1f - num * num));
		float num3 = Mathf.Abs(Vector3.Dot(up, forward));
		float num4 = bounds.extents.y * Mathf.Sqrt(Mathf.Max(0f, 1f - num3 * num3));
		float num5 = Mathf.Abs(Vector3.Dot(forward2, forward));
		float num6 = bounds.extents.z * Mathf.Sqrt(Mathf.Max(0f, 1f - num5 * num5));
		Vector3 vector;
		if (num4 <= num2 && num6 <= num2)
		{
			vector = this.targetObj.transform.right;
		}
		else if (num2 <= num4 && num6 <= num4)
		{
			vector = this.targetObj.transform.up;
		}
		else
		{
			vector = this.targetObj.transform.forward;
		}
		Vector3 vector2 = Vector3.Cross(forward, vector).normalized;
		vector = Vector3.Cross(vector2, forward);
		num2 = bounds.extents.x * Mathf.Abs(Vector3.Dot(right, vector)) + bounds.extents.y * Mathf.Abs(Vector3.Dot(up, vector)) + bounds.extents.z * Mathf.Abs(Vector3.Dot(forward2, vector));
		num4 = bounds.extents.x * Mathf.Abs(Vector3.Dot(right, vector2)) + bounds.extents.y * Mathf.Abs(Vector3.Dot(up, vector2)) + bounds.extents.z * Mathf.Abs(Vector3.Dot(forward2, vector2));
		Vector3 position = base.transform.position;
		vector *= num2;
		vector2 *= num4;
		Vector3 vector3 = position - vector - vector2;
		Vector3 vector4 = position - vector + vector2;
		Vector3 vector5 = position + vector - vector2;
		Vector3 vector6 = position + vector + vector2;
		float num7 = 1f / Vector3.Dot(forward, plane.normal);
		vertices[0] = vector3 - num7 * (plane.distance + Vector3.Dot(vector3, plane.normal)) * forward;
		vertices[1] = vector4 - num7 * (plane.distance + Vector3.Dot(vector4, plane.normal)) * forward;
		vertices[2] = vector5 - num7 * (plane.distance + Vector3.Dot(vector5, plane.normal)) * forward;
		vertices[3] = vector6 - num7 * (plane.distance + Vector3.Dot(vector6, plane.normal)) * forward;
	}

	public bool GetPlaneIntersection(Vector3[] vertices, Plane plane)
	{
		if (this.m_isOrthographic)
		{
			float d = this.m_orthographicSize * this.m_aspectRatio;
			float orthographicSize = this.m_orthographicSize;
			Vector3 b = d * base.transform.right;
			Vector3 b2 = orthographicSize * base.transform.up;
			Vector3 forward = base.transform.forward;
			Vector3 position = base.transform.position;
			Vector3 vector = position - b - b2;
			Vector3 vector2 = position - b + b2;
			Vector3 vector3 = position + b - b2;
			Vector3 vector4 = position + b + b2;
			float num = 1f / Vector3.Dot(forward, plane.normal);
			vertices[0] = vector - num * (plane.distance + Vector3.Dot(vector, plane.normal)) * forward;
			vertices[1] = vector2 - num * (plane.distance + Vector3.Dot(vector2, plane.normal)) * forward;
			vertices[2] = vector3 - num * (plane.distance + Vector3.Dot(vector3, plane.normal)) * forward;
			vertices[3] = vector4 - num * (plane.distance + Vector3.Dot(vector4, plane.normal)) * forward;
		}
		else
		{
			float num2 = Mathf.Tan(0.008726646f * this.m_fieldOfView);
			float d2 = num2 * this.m_aspectRatio;
			Vector3 b3 = d2 * base.transform.right;
			Vector3 b4 = num2 * base.transform.up;
			Vector3 forward2 = base.transform.forward;
			Vector3 position2 = base.transform.position;
			Vector3 vector5 = forward2 - b3 - b4;
			Vector3 vector6 = forward2 - b3 + b4;
			Vector3 vector7 = forward2 + b3 - b4;
			Vector3 vector8 = forward2 + b3 + b4;
			float num3 = Vector3.Dot(plane.normal, position2) + plane.distance;
			vertices[0] = position2 - num3 / Vector3.Dot(plane.normal, vector5) * vector5;
			vertices[1] = position2 - num3 / Vector3.Dot(plane.normal, vector6) * vector6;
			vertices[2] = position2 - num3 / Vector3.Dot(plane.normal, vector7) * vector7;
			vertices[3] = position2 - num3 / Vector3.Dot(plane.normal, vector8) * vector8;
		}
		return true;
	}
}
