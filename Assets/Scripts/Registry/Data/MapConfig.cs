using System;
using System.Collections.Generic;

namespace Registry.Data
{
    public class NodeCountRangeConfig
    {
        public int Min;
        public int Max;
        public float Factor;
    }
    
    public class ComplexityConfig
    {
        public float NormalStart = 0f;
        public float EliteStart = 0f;
        public float NormalRamp = 0.5f;
        public float EliteRamp = 0.17f;
    }
    
    public class RogueMapLayerParam
    {
        public int Layer;
        public Dictionary<string, int> NodeTypeWeights;
        public int NodeCount = -1;
        public int Complexity = -1;
    }
    
    public class RogueMapConfig
    {
        public string Id;
        public int Layers;

        public float AttackMultiplier = 1f;
        public float HPMultiplier = 1f;

        public NodeCountRangeConfig NodeCountRange;
        public ComplexityConfig Complexity;
        public Dictionary<string, int> DefaultLayerParams;
        public List<RogueMapLayerParam> SpecificLayerParams;
    }
    
    public class GlobalMapConfig
    {
        public List<RogueMapConfig> Maps;
        public static GlobalMapConfig CreateFromJson(string jsonString)
        {
            var cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<GlobalMapConfig>(jsonString);

            if (cfg == null)
                throw new Exception();

            foreach (var map in cfg.Maps)
            {
                map.NodeCountRange ??= new NodeCountRangeConfig{Min = 2, Max = 6, Factor = 2f};
                map.DefaultLayerParams ??= new Dictionary<string, int> {{"FIGHT", 100}};
            }
            return cfg;
        }
    }
}