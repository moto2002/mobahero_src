using System;
using UnityEngine;

public class SynthesisRuneSetPic : MonoBehaviour
{
	[SerializeField]
	private UIAtlas _atlasInst;

	[SerializeField]
	private MeshFilter _panelMesh;

	public void SetPic(string inName)
	{
		if (string.IsNullOrEmpty(inName))
		{
			return;
		}
		if (this._atlasInst == null || this._atlasInst.spriteList == null || this._atlasInst.spriteMaterial == null)
		{
			return;
		}
		if (this._panelMesh == null || this._panelMesh.mesh == null)
		{
			return;
		}
		Texture texture = ResourceManager.LoadPath<Texture>("Texture/Runes/RuneIcons_RGB", null, 0);
		Texture texture2 = ResourceManager.LoadPath<Texture>("Texture/Runes/RuneIcons_Alpha", null, 0);
		this._atlasInst.spriteMaterial.SetTexture("_MainTex", texture);
		this._atlasInst.spriteMaterial.SetTexture("_DetailTex", texture2);
		UISpriteData uISpriteData = null;
		for (int i = 0; i < this._atlasInst.spriteList.Count; i++)
		{
			UISpriteData uISpriteData2 = this._atlasInst.spriteList[i];
			if (uISpriteData2 != null && !string.IsNullOrEmpty(uISpriteData2.name) && uISpriteData2.name.Equals(inName))
			{
				uISpriteData = uISpriteData2;
				break;
			}
		}
		if (uISpriteData == null)
		{
			return;
		}
		Vector3[] vertices = this._panelMesh.mesh.vertices;
		if (vertices.Length != 4)
		{
			return;
		}
		int num = 0;
		int num2 = 1;
		int num3 = 2;
		int num4 = 3;
		float num5 = 0.01f;
		if (vertices[0].x > vertices[1].x + num5)
		{
			if (vertices[0].x > vertices[2].x + num5)
			{
				if (vertices[0].y > vertices[3].y + num5)
				{
					num3 = 0;
					num2 = 3;
					if (vertices[1].y > vertices[2].y + num5)
					{
						num4 = 1;
						num = 2;
					}
					else
					{
						num4 = 2;
						num = 1;
					}
				}
				else
				{
					num3 = 3;
					num2 = 0;
					if (vertices[1].y > vertices[2].y + num5)
					{
						num4 = 1;
						num = 2;
					}
					else
					{
						num4 = 2;
						num = 1;
					}
				}
			}
			else if (vertices[0].y > vertices[2].y + num5)
			{
				num3 = 0;
				num2 = 2;
				if (vertices[1].y > vertices[3].y + num5)
				{
					num4 = 1;
					num = 3;
				}
				else
				{
					num4 = 3;
					num = 1;
				}
			}
			else
			{
				num3 = 2;
				num2 = 0;
				if (vertices[1].y > vertices[3].y + num5)
				{
					num4 = 1;
					num = 3;
				}
				else
				{
					num4 = 3;
					num = 1;
				}
			}
		}
		else if (vertices[0].x < vertices[1].x - num5)
		{
			if (vertices[0].x < vertices[2].x - num5)
			{
				if (vertices[0].y > vertices[3].y + num5)
				{
					num4 = 0;
					num = 3;
					if (vertices[1].y > vertices[2].y + num5)
					{
						num3 = 1;
						num2 = 2;
					}
					else
					{
						num3 = 2;
						num2 = 1;
					}
				}
				else
				{
					num4 = 3;
					num = 0;
					if (vertices[1].y > vertices[2].y + num5)
					{
						num3 = 1;
						num2 = 2;
					}
					else
					{
						num3 = 2;
						num2 = 1;
					}
				}
			}
			else if (vertices[0].y > vertices[2].y + num5)
			{
				num4 = 0;
				num = 2;
				if (vertices[1].y > vertices[3].y + num5)
				{
					num3 = 1;
					num2 = 3;
				}
				else
				{
					num3 = 3;
					num2 = 1;
				}
			}
			else
			{
				num4 = 2;
				num = 0;
				if (vertices[1].y > vertices[3].y + num5)
				{
					num3 = 1;
					num2 = 3;
				}
				else
				{
					num3 = 3;
					num2 = 1;
				}
			}
		}
		else if (vertices[0].x > vertices[2].x + num5)
		{
			if (vertices[0].y > vertices[1].y + num5)
			{
				num3 = 0;
				num2 = 1;
				if (vertices[2].y > vertices[3].y + num5)
				{
					num4 = 2;
					num = 3;
				}
				else
				{
					num4 = 3;
					num = 2;
				}
			}
			else
			{
				num3 = 1;
				num2 = 0;
				if (vertices[2].y > vertices[3].y + num5)
				{
					num4 = 2;
					num = 3;
				}
				else
				{
					num4 = 3;
					num = 2;
				}
			}
		}
		else if (vertices[0].x < vertices[2].x - num5)
		{
			if (vertices[0].y > vertices[1].y + num5)
			{
				num4 = 0;
				num = 1;
				if (vertices[2].y > vertices[3].y + num5)
				{
					num3 = 2;
					num2 = 3;
				}
				else
				{
					num3 = 3;
					num2 = 2;
				}
			}
			else
			{
				num4 = 1;
				num = 0;
				if (vertices[2].y > vertices[3].y + num5)
				{
					num3 = 2;
					num2 = 3;
				}
				else
				{
					num3 = 3;
					num2 = 2;
				}
			}
		}
		int width = this._atlasInst.spriteMaterial.mainTexture.width;
		int height = this._atlasInst.spriteMaterial.mainTexture.height;
		int x = uISpriteData.x;
		int y = uISpriteData.y;
		int width2 = uISpriteData.width;
		int height2 = uISpriteData.height;
		if (width < 4 || height < 4)
		{
			return;
		}
		Vector2 vector = new Vector2((float)x * 1f / (float)width, (float)(height - y - height2) * 1f / (float)height);
		Vector2 vector2 = new Vector2((float)(x + width2) * 1f / (float)width, (float)(height - y - height2) * 1f / (float)height);
		Vector2 vector3 = new Vector2((float)(x + width2) * 1f / (float)width, (float)(height - y) * 1f / (float)height);
		Vector2 vector4 = new Vector2((float)x * 1f / (float)width, (float)(height - y) * 1f / (float)height);
		Vector2[] array = new Vector2[4];
		array[num] = vector;
		array[num2] = vector2;
		array[num3] = vector3;
		array[num4] = vector4;
		this._panelMesh.mesh.uv = array;
	}
}
