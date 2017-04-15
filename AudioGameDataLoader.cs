using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class AudioGameDataLoader
{
	public class audioBindstruct
	{
		public int idx;

		public string desc;

		public string eventstr;

		public float time;

		public bool endevent;
	}

	private bool loaded;

	private static AudioGameDataLoader _instance;

	public Dictionary<string, List<AudioGameDataLoader.audioBindstruct>> _heroVoice = new Dictionary<string, List<AudioGameDataLoader.audioBindstruct>>();

	public Dictionary<string, List<AudioGameDataLoader.audioBindstruct>> _skillSfx = new Dictionary<string, List<AudioGameDataLoader.audioBindstruct>>();

	public Dictionary<string, List<AudioGameDataLoader.audioBindstruct>> _spellSfx = new Dictionary<string, List<AudioGameDataLoader.audioBindstruct>>();

	public static AudioGameDataLoader instance
	{
		get
		{
			if (AudioGameDataLoader._instance == null)
			{
				AudioGameDataLoader._instance = new AudioGameDataLoader();
			}
			return AudioGameDataLoader._instance;
		}
	}

	public AudioGameDataLoader()
	{
		AudioGameDataLoader._instance = this;
	}

	public bool isLoaded()
	{
		return this.loaded;
	}

	public void clearaudioBind()
	{
		this._heroVoice.Clear();
		this._skillSfx.Clear();
		this._spellSfx.Clear();
	}

	public void Load()
	{
		if (!this.loaded)
		{
			this.loadToAudioXml();
		}
	}

	public void loadToAudioXml()
	{
		this.clearaudioBind();
		this.loaded = true;
		string text = "Assets/Resources/AudioBind.xml";
		XmlDocument xmlDocument;
		if (!File.Exists(text))
		{
			xmlDocument = new XmlDocument();
			TextAsset textAsset = Resources.Load("AudioBind", typeof(TextAsset)) as TextAsset;
			if (!(textAsset != null))
			{
				return;
			}
			xmlDocument.Load(new MemoryStream(textAsset.bytes));
		}
		else
		{
			xmlDocument = new XmlDocument();
			xmlDocument.Load(text);
		}
		XmlElement documentElement = xmlDocument.DocumentElement;
		if (documentElement == null)
		{
			xmlDocument.Load(Application.dataPath + "Resources/AudioBind.xml");
			documentElement = xmlDocument.DocumentElement;
		}
		if (documentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = documentElement.SelectNodes("/HeroAudio");
		XmlNodeList xmlNodeList2 = xmlNodeList[0].SelectNodes("Hero");
		foreach (XmlNode xmlNode in xmlNodeList2)
		{
			XmlElement xmlElement = (XmlElement)xmlNode;
			string attribute = xmlElement.GetAttribute("name");
			if (!this._heroVoice.ContainsKey(attribute))
			{
				this._heroVoice.Add(attribute, new List<AudioGameDataLoader.audioBindstruct>());
			}
			XmlNodeList xmlNodeList3 = xmlElement.SelectNodes("binder");
			foreach (XmlNode xmlNode2 in xmlNodeList3)
			{
				AudioGameDataLoader.audioBindstruct audioBindstruct = new AudioGameDataLoader.audioBindstruct();
				XmlElement xmlElement2 = (XmlElement)xmlNode2;
				audioBindstruct.idx = int.Parse(xmlElement2.GetAttribute("idx"));
				audioBindstruct.desc = xmlElement2.GetAttribute("desc");
				audioBindstruct.eventstr = xmlElement2.GetAttribute("event");
				audioBindstruct.time = float.Parse(xmlElement2.GetAttribute("time"));
				if (this._heroVoice[attribute] == null)
				{
					this._heroVoice[attribute] = new List<AudioGameDataLoader.audioBindstruct>();
				}
				this._heroVoice[attribute].Add(audioBindstruct);
			}
		}
		xmlNodeList2 = xmlNodeList[0].SelectNodes("Spell");
		foreach (XmlNode xmlNode3 in xmlNodeList2)
		{
			XmlElement xmlElement3 = (XmlElement)xmlNode3;
			string attribute2 = xmlElement3.GetAttribute("id");
			if (!this._spellSfx.ContainsKey(attribute2))
			{
				this._spellSfx.Add(attribute2, new List<AudioGameDataLoader.audioBindstruct>());
			}
			XmlNodeList xmlNodeList4 = xmlElement3.SelectNodes("binder");
			foreach (XmlNode xmlNode4 in xmlNodeList4)
			{
				AudioGameDataLoader.audioBindstruct audioBindstruct2 = new AudioGameDataLoader.audioBindstruct();
				XmlElement xmlElement4 = (XmlElement)xmlNode4;
				audioBindstruct2.idx = int.Parse(xmlElement4.GetAttribute("idx"));
				audioBindstruct2.desc = xmlElement4.GetAttribute("desc");
				audioBindstruct2.eventstr = xmlElement4.GetAttribute("event");
				audioBindstruct2.time = float.Parse(xmlElement4.GetAttribute("time"));
				if (!string.IsNullOrEmpty(xmlElement4.GetAttribute("endevt")))
				{
					audioBindstruct2.endevent = bool.Parse(xmlElement4.GetAttribute("endevt"));
				}
				if (this._spellSfx[attribute2] == null)
				{
					this._spellSfx[attribute2] = new List<AudioGameDataLoader.audioBindstruct>();
				}
				this._spellSfx[attribute2].Add(audioBindstruct2);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
