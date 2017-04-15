using System;

public static class Tools_GetLoadingFrame
{
	private const string SpriteNameBaiyin = "Loading_cardboard_baiyin";

	private const string SpriteNameHuangjin = "Loading_cardboard_huangjin";

	private const string SpriteNameBaijin = "Loading_cardboard_baijin";

	private const string SpriteNameZuanshi = "Loading_cardboard_zuanshi";

	private const string SpriteNameDashi = "Loading_cardboard_dashi";

	private const string SpriteNameWangzhe = "Loading_cardboard_Wangzhe";

	public static void GetLoadingFrame(this ToolsFacade facade, int index, UISprite target)
	{
		target.gameObject.SetActive(true);
		switch (index)
		{
		case 2:
			target.spriteName = "Loading_cardboard_baiyin";
			break;
		case 3:
			target.spriteName = "Loading_cardboard_huangjin";
			break;
		case 4:
			target.spriteName = "Loading_cardboard_baijin";
			break;
		case 5:
			target.spriteName = "Loading_cardboard_zuanshi";
			break;
		case 6:
			target.spriteName = "Loading_cardboard_dashi";
			break;
		case 7:
			target.spriteName = "Loading_cardboard_Wangzhe";
			break;
		default:
			target.gameObject.SetActive(false);
			break;
		}
	}
}
