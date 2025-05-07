using System.Collections.Generic;
using GameLogic;
using UnityEngine;

namespace Data
{
    public class StageData : MonoBehaviour
    {
        public static StageData Instance;
        
        private const string StageFolder = "Stages/";

        public readonly Dictionary<int, List<StageConfig>> NormalStages = new();
        public readonly Dictionary<int, List<StageConfig>> EliteStages = new();
        
        void Start()
        {
            Debug.Log("Loading stages...");
            foreach (var (difficulty, stages) in this.NormalStages)
            {
                Debug.Log("difficulty: " + difficulty + ", stage num: " + stages.Count);
            }
        }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            LoadFromFile();
        }
        
        private void LoadFromFile()
        {
            var stageList = Resources.LoadAll<TextAsset>(StageFolder);
            foreach (var stageTxt in stageList)
            {
                var config = StageConfig.CreateFromJson(stageTxt.text);
                var difficulty = config.Difficulty;
                if (!NormalStages.ContainsKey(difficulty))
                {
                    NormalStages[difficulty] = new List<StageConfig>();
                }
                NormalStages[difficulty].Add(config);
            }
        }

        public StageConfig GetNormalStage(int layer)
        {
            var difficulty = layer / 3;
            var listStage = NormalStages[difficulty];
            return listStage[Random.Range(0, listStage.Count)];
        }
        
        public StageConfig GetEliteStage(int layer)
        {
            var difficulty = layer / 5;
            var listStage = EliteStages[difficulty];
            return listStage[Random.Range(0, listStage.Count)];
        }
    }
}