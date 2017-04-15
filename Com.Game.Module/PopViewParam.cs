using System;

namespace Com.Game.Module
{
	public class PopViewParam
	{
		private string _title;

		private string _content;

		private PopViewType _type;

		private Action<EPopViewRet> _callback_enum;

		private Action _callback_void;

		private Action<bool> _callback_bool;

		private string _strOk = "确定";

		private string _strCancel = "取消";

		private PopViewTask _popViewTask;

		public string Title
		{
			get
			{
				return (!string.IsNullOrEmpty(this._title)) ? this._title : "提示";
			}
		}

		public string Content
		{
			get
			{
				return (!string.IsNullOrEmpty(this._content)) ? this._content : string.Empty;
			}
		}

		public string OK
		{
			get
			{
				return (!string.IsNullOrEmpty(this._strOk)) ? this._strOk : ((!(LanguageManager.Instance.GetStringById("Button_Ok") != string.Empty)) ? "确定" : LanguageManager.Instance.GetStringById("Button_Ok"));
			}
		}

		public string Cancel
		{
			get
			{
				return (!string.IsNullOrEmpty(this._strCancel)) ? this._strCancel : ((!(LanguageManager.Instance.GetStringById("Button_Cancel") != string.Empty)) ? "取消" : LanguageManager.Instance.GetStringById("Button_Cancel"));
			}
		}

		public bool ShowOne
		{
			get
			{
				return this._type == PopViewType.PopTwoButton;
			}
		}

		public Action<EPopViewRet> Callback_enum
		{
			get
			{
				return this._callback_enum;
			}
		}

		public Action<bool> Callback_bool
		{
			get
			{
				return this._callback_bool;
			}
		}

		public Action Callback_void
		{
			get
			{
				return this._callback_void;
			}
		}

		public PopViewTask Task
		{
			get
			{
				return this._popViewTask;
			}
		}

		public PopViewParam()
		{
		}

		public PopViewParam(string title, string content, Action callback, PopViewType type = PopViewType.PopOneButton, string ok = "确定", string cancel = "取消", PopViewTask popViewTask = null) : this(title, content, type, ok, cancel, popViewTask)
		{
			this._callback_void = callback;
		}

		public PopViewParam(string title, string content, Action<bool> callback, PopViewType type = PopViewType.PopOneButton, string ok = "确定", string cancel = "取消", PopViewTask popViewTask = null) : this(title, content, type, ok, cancel, popViewTask)
		{
			this._callback_bool = callback;
		}

		public PopViewParam(string title, string content, Action<EPopViewRet> callback, PopViewType type = PopViewType.PopOneButton, string ok = "确定", string cancel = "取消", PopViewTask popViewTask = null) : this(title, content, type, ok, cancel, popViewTask)
		{
			this._callback_enum = callback;
		}

		public PopViewParam(string title, string content, PopViewType type = PopViewType.PopOneButton, string ok = "确定", string cancel = "取消", PopViewTask popViewTask = null)
		{
			this._title = title;
			this._content = content;
			this._type = type;
			this._strOk = ok;
			this._strCancel = cancel;
			this._popViewTask = popViewTask;
		}
	}
}
