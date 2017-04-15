using System;
using System.Collections.Generic;
using UnityEngine;

public class AtlasManager
{
	public class SpriteEntry : UISpriteData
	{
		public Texture2D tex;

		public bool temporaryTexture;
	}

	private static Dictionary<string, Texture2D> avatarResources = new Dictionary<string, Texture2D>();

	private static Texture2D GetTexture(GameObject prefab, string spriteName)
	{
		if (prefab != null && spriteName != null)
		{
			UISprite component = prefab.GetComponent<UISprite>();
			UIAtlas atlas = component.atlas;
			return AtlasManager.ExtractSprite(atlas, spriteName);
		}
		return null;
	}

	public static Texture2D GetAvatarByScene(string spriteName)
	{
		if (spriteName != null && AtlasManager.avatarResources.ContainsKey(spriteName))
		{
			return AtlasManager.avatarResources[spriteName];
		}
		return null;
	}

	public static void LoadAvatarByScene(string[] players, string[] enemies)
	{
	}

	public static void ClearAvatarByScene()
	{
		AtlasManager.avatarResources.Clear();
	}

	public static Texture2D ExtractSprite(UIAtlas atlas, string spriteName)
	{
		if (atlas.texture == null)
		{
			return null;
		}
		UISpriteData sprite = atlas.GetSprite(spriteName);
		if (sprite == null)
		{
			return null;
		}
		Texture2D tex = atlas.texture as Texture2D;
		AtlasManager.SpriteEntry spriteEntry = AtlasManager.ExtractSprite(sprite, tex);
		return spriteEntry.tex;
	}

	private static AtlasManager.SpriteEntry ExtractSprite(UISpriteData es, Texture2D tex)
	{
		return (!(tex != null)) ? null : AtlasManager.ExtractSprite(es, tex.GetPixels32(), tex.width, tex.height);
	}

	private static AtlasManager.SpriteEntry ExtractSprite(UISpriteData es, Color32[] oldPixels, int oldWidth, int oldHeight)
	{
		int num = Mathf.Clamp(es.x, 0, oldWidth);
		int num2 = Mathf.Clamp(es.y, 0, oldHeight);
		int num3 = Mathf.Min(num + es.width, oldWidth - 1);
		int num4 = Mathf.Min(num2 + es.height, oldHeight - 1);
		int num5 = Mathf.Clamp(es.width, 0, oldWidth);
		int num6 = Mathf.Clamp(es.height, 0, oldHeight);
		if (num5 == 0 || num6 == 0)
		{
			return null;
		}
		Color32[] array = new Color32[num5 * num6];
		for (int i = 0; i < num6; i++)
		{
			int num7 = num2 + i;
			if (num7 > num4)
			{
				num7 = num4;
			}
			for (int j = 0; j < num5; j++)
			{
				int num8 = num + j;
				if (num8 > num3)
				{
					num8 = num3;
				}
				int num9 = (num6 - 1 - i) * num5 + j;
				int num10 = (oldHeight - 1 - num7) * oldWidth + num8;
				array[num9] = oldPixels[num10];
			}
		}
		AtlasManager.SpriteEntry spriteEntry = new AtlasManager.SpriteEntry();
		spriteEntry.CopyFrom(es);
		spriteEntry.SetRect(0, 0, num5, num6);
		spriteEntry.temporaryTexture = true;
		spriteEntry.tex = new Texture2D(num5, num6);
		spriteEntry.tex.name = "AtlasManager_ExtractSprite";
		spriteEntry.tex.SetPixels32(array);
		spriteEntry.tex.mipMapBias = 0f;
		spriteEntry.tex.Apply();
		return spriteEntry;
	}
}
