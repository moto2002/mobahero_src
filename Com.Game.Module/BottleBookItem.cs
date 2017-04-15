using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class BottleBookItem : MonoBehaviour
	{
		public UITexture Pic;

		public UISprite Pic_sprite;

		public UISprite Frame;

		public UISprite CouponIcon;

		public UILabel Name;

		public UILabel Type;

		public UILabel Count;

		private void OnDestroy()
		{
			this.Pic = null;
			this.Pic_sprite.atlas = null;
			base.StopAllCoroutines();
		}

		public void ShowPic(ItemBoookData data)
		{
			base.transform.GetComponent<UIDragScrollView>().enabled = true;
			this.SetQualityFrame(data._quality);
			this.SetTexture(data._icon, 140);
		}

		public void ShowName(string _name, BookItemType _type)
		{
			this.Name.gameObject.SetActive(true);
			this.Type.gameObject.SetActive(true);
			this.Name.text = LanguageManager.Instance.GetStringById(_name);
			this.Type.text = this.DisplayTypeName(_type);
		}

		private bool IsIllegalArgument(object _param)
		{
			if (_param == null)
			{
				ClientLogger.Error("BottleBookItem: Argument Illegal Null.");
				return true;
			}
			return false;
		}

		private void CreateBookItem(SysCurrencyVo currencyVo)
		{
			if (this.IsIllegalArgument(currencyVo))
			{
				return;
			}
			int currency_id = currencyVo.currency_id;
			switch (currency_id)
			{
			case 9:
				this.SetQualityFrame(0);
				this.SetTexture("Checkins_icons_little_cap", 150);
				return;
			case 10:
				IL_29:
				if (currency_id == 1)
				{
					this.SetQualityFrame(0);
					this.SetTexture("Checkins_icons_gold", 150);
					return;
				}
				if (currency_id != 2)
				{
					return;
				}
				this.SetQualityFrame(0);
				this.SetTexture("Checkins_icons_diamond", 146);
				return;
			case 11:
				this.SetQualityFrame(3);
				this.SetTexture("Checkins_icons_little_trumpet", 128);
				return;
			}
			goto IL_29;
		}

		private void CreateBookItem(SysHeroMainVo heroVo)
		{
			if (this.IsIllegalArgument(heroVo))
			{
				return;
			}
			this.SetQualityFrame(heroVo.quality);
			this.SetTexture(heroVo.avatar_icon, 140);
		}

		private void CreateBookItem(SysHeroSkinVo skinVo)
		{
			if (this.IsIllegalArgument(skinVo))
			{
				return;
			}
			this.SetQualityFrame(skinVo.quality);
			this.SetTexture(skinVo.avatar_icon, 140);
		}

		private void CreateBookItem(SysGameItemsVo itemVo)
		{
			if (this.IsIllegalArgument(itemVo))
			{
				return;
			}
			if (itemVo.items_id == "7777")
			{
				this.SetQualityFrame(0);
				this.SetTexture("Checkins_icons_exp_bottle", 140);
			}
			else if (itemVo.type == 4)
			{
				this.SetQualityFrame(itemVo.quality);
				UIAtlas component = Resources.Load<GameObject>("Texture/Runes/RuneIconsAtlas").GetComponent<UIAtlas>();
				this.SetSprite(component, itemVo.icon, 115, 132);
			}
			else
			{
				this.SetQualityFrame(itemVo.quality);
				this.SetTexture(itemVo.icon, 140);
			}
		}

		private void CreateBookItem(SysSummonersHeadportraitVo portraitVo)
		{
			if (this.IsIllegalArgument(portraitVo))
			{
				return;
			}
			this.SetQualityFrame(portraitVo.headportrait_quality);
			this.SetTexture(portraitVo.headportrait_icon, 140);
		}

		private void CreateBookItem(SysSummonersPictureframeVo frameVo)
		{
			if (this.IsIllegalArgument(frameVo))
			{
				return;
			}
			this.SetQualityFrame(frameVo.pictureframe_quality);
			this.SetTexture(frameVo.pictureframe_icon, new Vector3(0f, 2.5f, 0f), 168);
		}

		private void CreateBookItem_Exp(int num)
		{
			if (num <= 0)
			{
				return;
			}
			this.SetQualityFrame(0);
			this.SetTexture("Checkins_icons_exp_summoner", 140);
		}

		private void CreateBookItem(SysCouponVo couponVo)
		{
			if (this.IsIllegalArgument(couponVo))
			{
				return;
			}
			this.SetQualityFrame(couponVo.quality);
			if (couponVo.mother_type == 1)
			{
				SysHeroMainVo dataById = BaseDataMgr.instance.GetDataById<SysHeroMainVo>(couponVo.mother_id);
				if (this.IsIllegalArgument(dataById))
				{
					return;
				}
				this.SetTexture(dataById.avatar_icon, 140);
			}
			else if (couponVo.mother_type == 2)
			{
				SysHeroSkinVo dataById2 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(couponVo.mother_id);
				if (this.IsIllegalArgument(dataById2))
				{
					return;
				}
				this.SetTexture(dataById2.avatar_icon, 140);
			}
			this.CouponIcon.spriteName = "Magic_bottle_tujian_discount_" + couponVo.off_number;
		}

		private void CreateBookItem(SysGameBuffVo buffVo)
		{
			if (this.IsIllegalArgument(buffVo))
			{
				return;
			}
			this.SetQualityFrame(buffVo.quality);
			this.SetTexture(buffVo.icon, 140);
		}

		private void SetQualityFrame(int _quality = 0)
		{
			this.Frame.spriteName = "Checkins_icons_frame_0" + _quality;
		}

		private void SetItemCount(bool _isShowCount, int _count = 0)
		{
			if (!_isShowCount || _count <= 1)
			{
				this.Count.text = string.Empty;
			}
			else
			{
				this.Count.text = "x" + _count;
			}
		}

		private void SetItemName(bool _isShowName)
		{
			this.Name.gameObject.SetActive(_isShowName);
		}

		private void SetSprite(UIAtlas _atlas, string _spriteName, int width = 0, int height = 0)
		{
			this.IsTextureIcon(false);
			this.Pic_sprite.atlas = _atlas;
			this.Pic_sprite.spriteName = _spriteName;
			if (width == 0 || height == 0)
			{
				this.Pic_sprite.MakePixelPerfect();
			}
			else
			{
				this.Pic_sprite.width = width;
				this.Pic_sprite.height = height;
			}
		}

		private void SetTexture(string _picResLoadId, Vector3 _offsetPos, int _lengthOfSide = 140)
		{
			this.SetTexture(_picResLoadId, _lengthOfSide);
			this.Pic.transform.localPosition = _offsetPos;
		}

		private void SetTexture(string _picResLoadId, int _lengthOfSide = 140)
		{
			this.IsTextureIcon(true);
			this.Pic.transform.localPosition = Vector3.zero;
			this.Pic.SetDimensions(_lengthOfSide, _lengthOfSide);
			base.StartCoroutine(this.AsyncLoadPic(_picResLoadId));
		}

		private void IsTextureIcon(bool isTexture)
		{
			this.Pic.gameObject.SetActive(isTexture);
			this.Pic_sprite.gameObject.SetActive(!isTexture);
		}

		[DebuggerHidden]
		private IEnumerator AsyncLoadPic(string picName)
		{
			BottleBookItem.<AsyncLoadPic>c__Iterator113 <AsyncLoadPic>c__Iterator = new BottleBookItem.<AsyncLoadPic>c__Iterator113();
			<AsyncLoadPic>c__Iterator.picName = picName;
			<AsyncLoadPic>c__Iterator.<$>picName = picName;
			<AsyncLoadPic>c__Iterator.<>f__this = this;
			return <AsyncLoadPic>c__Iterator;
		}

		private string DisplayTypeName(BookItemType _type)
		{
			string result = string.Empty;
			switch (_type)
			{
			case BookItemType.HeroSkin:
				result = "皮肤";
				break;
			case BookItemType.Pets:
				result = "宠物";
				break;
			case BookItemType.Trailing:
				result = "拖尾特效";
				break;
			case BookItemType.LevelUp:
				result = "升级特效";
				break;
			case BookItemType.TownPortal:
				result = "回城特效";
				break;
			case BookItemType.Birth:
				result = "回城特效";
				break;
			case BookItemType.Death:
				result = "回城特效";
				break;
			case BookItemType.HeadPortrait:
				result = "头像";
				break;
			}
			return result;
		}

		public object SetData(ItemType itemType, string configId, bool isShowName = false, bool isShowNum = false, int count = 0)
		{
			this.CouponIcon.gameObject.SetActive(false);
			this.SetItemCount(isShowNum, count);
			this.SetItemName(isShowName);
			switch (itemType)
			{
			case ItemType.Rune:
			case ItemType.Bottle:
			case ItemType.NormalGameItem:
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(configId);
				this.CreateBookItem(dataById);
				if (isShowName)
				{
					this.Name.text = LanguageManager.Instance.GetStringById(dataById.name);
				}
				return dataById;
			}
			case ItemType.Diamond:
			case ItemType.Cap:
			case ItemType.Coin:
			case ItemType.Speaker:
			{
				SysCurrencyVo dataById2 = BaseDataMgr.instance.GetDataById<SysCurrencyVo>(configId);
				this.CreateBookItem(dataById2);
				if (isShowName)
				{
					this.Name.text = LanguageManager.Instance.GetStringById(dataById2.name);
				}
				return dataById2;
			}
			case ItemType.HeadPortrait:
			{
				SysSummonersHeadportraitVo dataById3 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(configId);
				this.CreateBookItem(dataById3);
				if (isShowName)
				{
					this.Name.text = LanguageManager.Instance.GetStringById(dataById3.headportrait_name);
				}
				return dataById3;
			}
			case ItemType.Hero:
			{
				SysHeroMainVo dataById4 = BaseDataMgr.instance.GetDataById<SysHeroMainVo>(configId);
				this.CreateBookItem(dataById4);
				if (isShowName)
				{
					this.Name.text = LanguageManager.Instance.GetStringById(dataById4.name);
				}
				return dataById4;
			}
			case ItemType.HeroSkin:
			{
				SysHeroSkinVo dataById5 = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(configId);
				this.CreateBookItem(dataById5);
				if (isShowName)
				{
					this.Name.text = LanguageManager.Instance.GetStringById(dataById5.name);
				}
				return dataById5;
			}
			case ItemType.Exp:
				this.CreateBookItem_Exp(count);
				if (isShowName)
				{
					this.Name.text = "召唤师经验";
				}
				return null;
			case ItemType.Coupon:
			{
				this.CouponIcon.gameObject.SetActive(true);
				SysCouponVo dataById6 = BaseDataMgr.instance.GetDataById<SysCouponVo>(configId);
				this.CreateBookItem(dataById6);
				if (isShowName)
				{
					this.Name.text = LanguageManager.Instance.GetStringById(dataById6.name);
				}
				return dataById6;
			}
			case ItemType.PortraitFrame:
			{
				SysSummonersPictureframeVo dataById7 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(configId);
				this.CreateBookItem(dataById7);
				if (isShowName)
				{
					this.Name.text = LanguageManager.Instance.GetStringById(dataById7.pictureframe_name);
				}
				return dataById7;
			}
			case ItemType.GameBuff:
			{
				SysGameBuffVo dataById8 = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(configId);
				this.CreateBookItem(dataById8);
				if (isShowName)
				{
					this.Name.text = LanguageManager.Instance.GetStringById(dataById8.name);
				}
				return dataById8;
			}
			}
			return null;
		}

		public SysGameItemsVo SetData(EquipmentInfoData equipData, bool isShowName = false, bool isShowNum = false)
		{
			this.CouponIcon.gameObject.SetActive(false);
			this.SetItemCount(isShowNum, equipData.Count);
			this.SetItemName(isShowName);
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(equipData.ModelId.ToString());
			this.CreateBookItem(dataById);
			if (isShowName)
			{
				this.Name.text = LanguageManager.Instance.GetStringById(dataById.name);
			}
			return dataById;
		}

		public SysHeroMainVo SetData(HeroInfoData heroData, bool isShowName = false)
		{
			this.CouponIcon.gameObject.SetActive(false);
			this.SetItemCount(false, 0);
			this.SetItemName(isShowName);
			SysHeroMainVo dataById = BaseDataMgr.instance.GetDataById<SysHeroMainVo>(heroData.ModelId);
			this.CreateBookItem(dataById);
			if (isShowName)
			{
				this.Name.text = LanguageManager.Instance.GetStringById(dataById.name);
			}
			return dataById;
		}
	}
}
