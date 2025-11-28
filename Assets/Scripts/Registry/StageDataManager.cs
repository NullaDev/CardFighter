using System;
using System.Collections.Generic;
using System.IO;
using Registry.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Registry
{
    public class StageDataManager
    {
        private bool _hasLoaded = false;

        private static readonly string StageFolderRoot = Path.Combine(Application.dataPath, "../GameData/Stages");

        public readonly Dictionary<int, List<StageConfig>> NormalStages = new();
        public readonly Dictionary<int, List<StageConfig>> EliteStages = new();
        public readonly List<StageConfig> BossStages = new();
        public readonly List<StageConfig> MiscStages = new();
        
        public void DebugLoadedStageInfo()
        {
            Debug.Log($"[StageDataManager] Loaded stages from: {StageFolderRoot}");
            Debug.Log("Normal stages:");
            foreach (var (difficulty, stages) in NormalStages)
                Debug.Log($"  difficulty: {difficulty}, count: {stages.Count}");

            Debug.Log("Elite stages:");
            foreach (var (difficulty, stages) in EliteStages)
                Debug.Log($"  difficulty: {difficulty}, count: {stages.Count}");

            Debug.Log($"Boss stages: {BossStages.Count}");
            Debug.Log($"Misc stages: {MiscStages.Count}");

        }
        
        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            this._hasLoaded = true;
            if (!Directory.Exists(StageFolderRoot))
            {
                Debug.LogError($"[StageDataManager] Folder not found: {StageFolderRoot}");
                return;
            }

            var jsonFiles = Directory.GetFiles(StageFolderRoot, "*.json", SearchOption.AllDirectories);
            if (jsonFiles.Length == 0)
            {
                Debug.LogWarning($"[StageDataManager] No stage json files found in {StageFolderRoot}");
                return;
            }

            foreach (var file in jsonFiles)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var config = StageConfig.CreateFromJson(json);
                    if (config == null)
                    {
                        Debug.LogWarning($"[StageDataManager] Failed to parse stage: {file}");
                        continue;
                    }

                    var difficulty = config.Difficulty;
                    switch (config.Type)
                    {
                        case "Fight":
                            if (!NormalStages.ContainsKey(difficulty))
                                NormalStages[difficulty] = new List<StageConfig>();
                            NormalStages[difficulty].Add(config);
                            break;
                        case "Elite":
                            if (!EliteStages.ContainsKey(difficulty))
                                EliteStages[difficulty] = new List<StageConfig>();
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
                catch (Exception e)
                {
                    Debug.LogError($"[StageDataManager] Error loading {file}: {e.Message}");
                }
            }

            DebugLoadedStageInfo();
        }

        public StageConfig GetNormalStage(float complexity)
        {
            var c = Math.Clamp((int)complexity, 0, NormalStages.Count);
            var listStage = NormalStages[c];
            return listStage[Random.Range(0, listStage.Count)];
        }
        
        public StageConfig GetEliteStage(float complexity)
        {
            var c = Math.Clamp((int)complexity, 0, EliteStages.Count);
            var listStage = EliteStages[c];
            return listStage[Random.Range(0, listStage.Count)];
        }
        
        public StageConfig GetBossStage()
        {
            return BossStages[Random.Range(0, BossStages.Count)];
        }
    }
}