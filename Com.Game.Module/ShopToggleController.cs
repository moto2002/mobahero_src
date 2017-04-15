using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class ShopToggleController : MonoBehaviour
	{
		public UISprite _icon;

		public UILabel _label;

		public GameObject _chosenSprite;

		private ShopDataNew mData;

		public ShopDataNew data
		{
			get
			{
				return this.mData;
			}
		}

		public void SetData(ShopDataNew _data)
		{
			this.mData = _data;
			this._label.text = LanguageManager.Instance.GetStringById(this.mData.Name);
			this._icon.spriteName = this.mData.Picture;
			this._icon.MakePixelPerfect();
		}

		public void SetColliderEnable(bool isEnable)
		{
			base.transform.GetComponent<BoxCollider>().enabled = isEnable;
		}
	}
}
