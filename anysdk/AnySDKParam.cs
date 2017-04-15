using System;
using System.Collections.Generic;

namespace anysdk
{
	public struct AnySDKParam
	{
		private enum ParamType
		{
			kParamTypeNull,
			kParamTypeInt,
			kParamTypeFloat,
			kParamTypeBool,
			kParamTypeString,
			kParamTypeStringMap,
			kParamTypeMap
		}

		private AnySDKParam.ParamType _type;

		private int _intValue;

		private float _floatValue;

		private bool _boolValue;

		private string _strValue;

		private string _strMapValue;

		public AnySDKParam(int nValue)
		{
			this._intValue = nValue;
			this._floatValue = 0f;
			this._boolValue = false;
			this._strValue = null;
			this._strMapValue = null;
			this._type = AnySDKParam.ParamType.kParamTypeInt;
		}

		public AnySDKParam(float nValue)
		{
			this._intValue = 0;
			this._floatValue = nValue;
			this._boolValue = false;
			this._strValue = null;
			this._strMapValue = null;
			this._type = AnySDKParam.ParamType.kParamTypeFloat;
		}

		public AnySDKParam(bool nValue)
		{
			this._intValue = 0;
			this._floatValue = 0f;
			this._boolValue = nValue;
			this._strValue = null;
			this._strMapValue = null;
			this._type = AnySDKParam.ParamType.kParamTypeBool;
		}

		public AnySDKParam(string nValue)
		{
			this._intValue = 0;
			this._floatValue = 0f;
			this._boolValue = false;
			this._strValue = nValue;
			this._strMapValue = null;
			this._type = AnySDKParam.ParamType.kParamTypeString;
		}

		public AnySDKParam(Dictionary<string, string> nValue)
		{
			this._intValue = 0;
			this._floatValue = 0f;
			this._boolValue = false;
			this._strValue = null;
			this._strMapValue = AnySDKUtil.dictionaryToString(nValue);
			this._type = AnySDKParam.ParamType.kParamTypeStringMap;
		}

		public int getCurrentType()
		{
			return (int)this._type;
		}

		public int getIntValue()
		{
			return this._intValue;
		}

		public float getFloatValue()
		{
			return this._floatValue;
		}

		public bool getBoolValue()
		{
			return this._boolValue;
		}

		public string getStringValue()
		{
			return this._strValue;
		}

		public string getStrMapValue()
		{
			return this._strMapValue;
		}
	}
}
