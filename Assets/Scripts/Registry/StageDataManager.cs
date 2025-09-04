using System.Collections.Generic;
using Registry.Data;
using UnityEngine;

namespace Registry
{
    public class StageDataManager
    {
        private bool _hasLoaded = false;

        private const string StageFolderRoot = "Stages/";
        public static string[] SubFolders = {"Tutorial", "Fight", "Elite", "Boss", "Test"};

        public readonly Dictionary<int, List<StageConfig>> NormalStages = new();
        public readonly Dictionary<int, List<StageConfig>> EliteStages = new();
        public readonly List<StageConfig> BossStages = new();
        public readonly List<StageConfig> MiscStages = new();
        
        public void DebugLoadedStageInfo()
        {
            Debug.Log("Loading stages...");
            Debug.Log("Normal stages:");
            foreach (var (difficulty, stages) in this.NormalStages)
            {
                Debug.Log("difficulty: " + difficulty + ", stage num: " + stages.Count);
            }
            Debug.Log("Elite stages:");
            foreach (var (difficulty, stages) in this.EliteStages)
            {
                Debug.Log("difficulty: " + difficulty + ", stage num: " + stages.Count);
            }

        }
        
        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            this._hasLoaded = true;
            foreach (var subFolder in SubFolders)
            {
                var fullPath = StageFolderRoot + subFolder;
                var stageList = Resources.LoadAll<TextAsset>(fullPath);
                foreach (var stageTxt in stageList)
                {
                    var config = StageConfig.CreateFromJson(stageTxt.text);
                    var difficulty = config.Difficulty;
                    switch (config.Type)
                    {
                        case "Fight":
                            if (!NormalStages.ContainsKey(difficulty))
                            {
                                NormalStages[difficulty] = new List<StageConfig>();
                            }
                            NormalStages[difficulty].Add(config);
                            break;
                        case "Elite":
                            if (!EliteStages.ContainsKey(difficulty))
                            {
                                EliteStages[difficulty] = new List<StageConfig>();
                            }
                            EliteStages[difficulty].Add(config);
                            break;
                        case "Boss":
                            BossStages.Add(config);
                            break;
                        default:
                            MiscStages.Add(config);
                            break;
                    }
                }
            }

            DebugLoadedStageInfo();
        }

        public StageConfig GetNormalStage(int layer)
        {
            var difficulty = layer / 2;
            var listStage = NormalStages[difficulty];
            return listStage[Random.Range(0, listStage.Count)];
        }
        
        public StageConfig GetEliteStage(int layer)
        {
            var difficulty = (layer - 2) / 5;
            var listStage = EliteStages[difficulty];
            return listStage[Random.Range(0, listStage.Count)];
        }
        
        public StageConfig GetBossStage()
        {
            return BossStages[Random.Range(0, BossStages.Count)];
        }
    }
}