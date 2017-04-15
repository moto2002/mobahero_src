using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Localization")]
public class Localization : MonoBehaviour
{
	private static Localization mInstance;

	public static string[] knownLanguages;

	public static bool localizationHasBeenSet = false;

	[HideInInspector]
	public string startingLanguage = "English";

	[HideInInspector]
	public TextAsset[] languages;

	private static Dictionary<string, string> mOldDictionary = new Dictionary<string, string>();

	private static Dictionary<string, string[]> mDictionary = new Dictionary<string, string[]>();

	private static int mLanguageIndex = -1;

	private static string mLanguage;

	public static Dictionary<string, string[]> dictionary
	{
		get
		{
			if (!Localization.localizationHasBeenSet)
			{
				Localization.language = PlayerPrefs.GetString("Language", "English");
			}
			return Localization.mDictionary;
		}
	}

	public static bool isActive
	{
		get
		{
			return Localization.mInstance != null;
		}
	}

	public static Localization instance
	{
		get
		{
			if (Localization.mInstance == null)
			{
				Localization.mInstance = (UnityEngine.Object.FindObjectOfType(typeof(Localization)) as Localization);
				if (Localization.mInstance == null)
				{
					GameObject gameObject = new GameObject("_Localization");
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					Localization.mInstance = gameObject.AddComponent<Localization>();
				}
			}
			return Localization.mInstance;
		}
	}

	[Obsolete("Use Localization.language instead")]
	public string currentLanguage
	{
		get
		{
			return Localization.language;
		}
		set
		{
			Localization.language = value;
		}
	}

	public static string language
	{
		get
		{
			return Localization.mLanguage;
		}
		set
		{
			if (Localization.mLanguage != value)
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (Localization.mDictionary.Count == 0)
					{
						TextAsset textAsset = (!Localization.localizationHasBeenSet) ? (Resources.Load("Localization", typeof(TextAsset)) as TextAsset) : null;
						Localization.localizationHasBeenSet = true;
						if (textAsset == null || !Localization.LoadCSV(textAsset))
						{
							textAsset = (Resources.Load(value, typeof(TextAsset)) as TextAsset);
							if (textAsset != null)
							{
								Localization.Load(textAsset);
								return;
							}
						}
					}
					if (Localization.mDictionary.Count != 0 && Localization.SelectLanguage(value))
					{
						return;
					}
					if (Localization.mInstance != null && Localization.mInstance.languages != null)
					{
						int i = 0;
						int num = Localization.mInstance.languages.Length;
						while (i < num)
						{
							TextAsset textAsset2 = Localization.mInstance.languages[i];
							if (textAsset2 != null && textAsset2.name == value)
							{
								Localization.Load(textAsset2);
								return;
							}
							i++;
						}
					}
				}
				Localization.mOldDictionary.Clear();
				PlayerPrefs.DeleteKey("Language");
			}
		}
	}

	private void Awake()
	{
		if (Localization.mInstance == null)
		{
			Localization.mInstance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (Localization.mOldDictionary.Count == 0 && Localization.mDictionary.Count == 0)
			{
				Localization.language = PlayerPrefs.GetString("Language", this.startingLanguage);
			}
			if (string.IsNullOrEmpty(Localization.mLanguage) && this.languages != null && this.languages.Length > 0)
			{
				Localization.language = this.languages[0].name;
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnEnable()
	{
		if (Localization.mInstance == null)
		{
			Localization.mInstance = this;
		}
	}

	private void OnDisable()
	{
		Localization.localizationHasBeenSet = false;
		Localization.mLanguageIndex = -1;
		Localization.mDictionary.Clear();
		Localization.mOldDictionary.Clear();
	}

	private void OnDestroy()
	{
		if (Localization.mInstance == this)
		{
			Localization.mInstance = null;
		}
	}

	public static void Load(TextAsset asset)
	{
		ByteReader byteReader = new ByteReader(asset);
		Localization.Set(asset.name, byteReader.ReadDictionary());
	}

	public static bool LoadCSV(TextAsset asset)
	{
		ByteReader byteReader = new ByteReader(asset);
		BetterList<string> betterList = byteReader.ReadCSV();
		if (betterList.size < 2)
		{
			return false;
		}
		betterList[0] = "KEY";
		if (!string.Equals(betterList[0], "KEY"))
		{
			Debug.LogError("Invalid localization CSV file. The first value is expected to be 'KEY', followed by language columns.\nInstead found '" + betterList[0] + "'", asset);
			return false;
		}
		Localization.knownLanguages = new string[betterList.size - 1];
		for (int i = 0; i < Localization.knownLanguages.Length; i++)
		{
			Localization.knownLanguages[i] = betterList[i + 1];
		}
		Localization.mDictionary.Clear();
		while (betterList != null)
		{
			Localization.AddCSV(betterList);
			betterList = byteReader.ReadCSV();
		}
		return true;
	}

	private static bool SelectLanguage(string language)
	{
		Localization.mLanguageIndex = -1;
		if (Localization.mDictionary.Count == 0)
		{
			return false;
		}
		string[] array;
		if (Localization.mDictionary.TryGetValue("KEY", out array))
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == language)
				{
					Localization.mOldDictionary.Clear();
					Localization.mLanguageIndex = i;
					Localization.mLanguage = language;
					PlayerPrefs.SetString("Language", Localization.mLanguage);
					UIRoot.Broadcast("OnLocalize");
					return true;
				}
			}
		}
		return false;
	}

	private static void AddCSV(BetterList<string> values)
	{
		if (values.size < 2)
		{
			return;
		}
		string[] array = new string[values.size - 1];
		for (int i = 1; i < values.size; i++)
		{
			array[i - 1] = values[i];
		}
		Localization.mDictionary.Add(values[0], array);
	}

	public static void Set(string languageName, Dictionary<string, string> dictionary)
	{
		Localization.mLanguage = languageName;
		PlayerPrefs.SetString("Language", Localization.mLanguage);
		Localization.mOldDictionary = dictionary;
		Localization.localizationHasBeenSet = false;
		Localization.mLanguageIndex = -1;
		Localization.knownLanguages = new string[]
		{
			languageName
		};
		UIRoot.Broadcast("OnLocalize");
	}

	public static string Get(string key)
	{
		if (!Localization.localizationHasBeenSet)
		{
			Localization.language = PlayerPrefs.GetString("Language", "English");
		}
		string key2 = key + " Mobile";
		string[] array;
		string result;
		if (Localization.mLanguageIndex != -1 && Localization.mDictionary.TryGetValue(key2, out array))
		{
			if (Localization.mLanguageIndex < array.Length)
			{
				return array[Localization.mLanguageIndex];
			}
		}
		else if (Localization.mOldDictionary.TryGetValue(key2, out result))
		{
			return result;
		}
		if (Localization.mLanguageIndex != -1 && Localization.mDictionary.TryGetValue(key, out array))
		{
			if (Localization.mLanguageIndex < array.Length)
			{
				return array[Localization.mLanguageIndex];
			}
		}
		else if (Localization.mOldDictionary.TryGetValue(key, out result))
		{
			return result;
		}
		return key;
	}

	[Obsolete("Use Localization.Get instead")]
	public static string Localize(string key)
	{
		return Localization.Get(key);
	}

	public static bool Exists(string key)
	{
		if (Localization.mLanguageIndex != -1)
		{
			return Localization.mDictionary.ContainsKey(key);
		}
		return Localization.mOldDictionary.ContainsKey(key);
	}
}
