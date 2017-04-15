using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Game.Module
{
	public class ChapterData
	{
		public string mId;

		public List<string> mScenes = new List<string>();

		public SysBattleConfigVo mChapterCfg;

		public BattlesModel mChapterRecord;

		public Dictionary<string, SysBattleSceneVo> mDicLevelCfg = new Dictionary<string, SysBattleSceneVo>();

		public Dictionary<string, BattleSceneModel> mDicLevelRecord = new Dictionary<string, BattleSceneModel>();

		public string chapterName
		{
			get
			{
				return this.mChapterCfg.battle_name;
			}
		}

		public int LevelCount
		{
			get;
			set;
		}

		public int CurLevelIndex
		{
			get
			{
				return (this.mDicLevelRecord.Count != 0) ? (this.mDicLevelRecord.Count - 1) : -1;
			}
		}

		public int SelectedIndex
		{
			get;
			set;
		}

		public bool IsLocked
		{
			get
			{
				return this.mDicLevelRecord.Count == 0;
			}
		}

		public bool IsCompleted
		{
			get;
			set;
		}

		public ChapterData(SysBattleConfigVo chapterCfg)
		{
			this.mChapterCfg = chapterCfg;
			this.mScenes = chapterCfg.scene_id.Split(new char[]
			{
				','
			}).ToList<string>();
			this.mId = chapterCfg.battle_id;
			this.LevelCount = this.mScenes.Count;
			List<BattlesModel> list = ModelManager.Instance.Get_battleList_X();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].BattleId.ToString() == this.mId)
				{
					this.mChapterRecord = list[i];
					break;
				}
			}
			if (this.mChapterRecord != null && this.mChapterRecord.List != null)
			{
				this.IsCompleted = (this.LevelCount == this.mChapterRecord.List.Count);
				for (int j = 0; j < this.mChapterRecord.List.Count; j++)
				{
					this.mDicLevelRecord.Add(this.mChapterRecord.List[j].SceneId.ToString(), this.mChapterRecord.List[j]);
				}
			}
		}

		public SysBattleSceneVo GetLevelCfg(string levelId)
		{
			if (!this.mDicLevelCfg.ContainsKey(levelId))
			{
				SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(levelId);
				this.mDicLevelCfg.Add(levelId, dataById);
			}
			return this.mDicLevelCfg[levelId];
		}

		public BattleSceneModel GetLevelRecord(string levelId)
		{
			return (!this.mDicLevelRecord.ContainsKey(levelId)) ? null : this.mDicLevelRecord[levelId];
		}

		public bool IsNewLevel(string levelId)
		{
			BattleSceneModel levelRecord = this.GetLevelRecord(levelId);
			return levelRecord != null && levelRecord.Star == 0;
		}

		public bool IsLevelLocked(string levelId)
		{
			return this.GetLevelRecord(levelId) == null;
		}

		public int StarsNum(string levelId)
		{
			return (!this.mDicLevelRecord.ContainsKey(levelId)) ? -1 : ((int)this.mDicLevelRecord[levelId].Star);
		}

		public int DayRestCount(string levelId)
		{
			return (!this.mDicLevelRecord.ContainsKey(levelId)) ? 1 : this.mDicLevelRecord[levelId].DayRestCount;
		}
	}
}
