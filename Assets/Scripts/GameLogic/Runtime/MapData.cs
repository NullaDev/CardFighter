using System.Linq;
using GameLogic.Map;
using GameLogic.Option;
using Registry;
using Registry.Data;

namespace GameLogic.Runtime
{
    public class MapData
    {
        public bool Initialized = false;
        public static MapData Instance = new();

        private MapData() {}

        public GlobalMap GlobalMap;
        public int CurrentMapIndex = 0;
        public RogueMap CurrentMap => GlobalMap?.Maps?[CurrentMapIndex];
        public float CurrentMapHpModifier => CurrentMap?.Config.HPMultiplier?? 1f;
        public float CurrentMapAttackModifier => CurrentMap?.Config.AttackMultiplier?? 1f;

        public int CurrentLayer = 0;
        public int CurrentNodeIndex = 0;
        public MapNode CurrentNode => CurrentMap?.Layers?[CurrentLayer]?[CurrentNodeIndex];
        public NodeType CurrentNodeType => CurrentNode?.Type ?? NodeType.FIGHT;
        
        public StageConfig CurrentStageConfig;
        
        public void Initialize()
        {
            if (Initialized) return;

            GlobalMap = new GlobalMap(StaticDataManager.GlobalMapDataManager.LoadedConfig, 19260817);
            CurrentMapIndex = 0;
            CurrentLayer = 0;
            CurrentNodeIndex = 0;
            Initialized = true;
        }
        
        public bool MoveToNode(MapNode node)
        {
            var result = CurrentMap?.Layers
                    .SelectMany((layerNodes, layer) =>
                        layerNodes.Select((n, idx) => new { n, layer, idx }))
                    .FirstOrDefault(x => x.n == node);
            if (result == null) return false;

            CurrentLayer = result.layer;
            CurrentNodeIndex = result.idx;
            return true;
        }

        public bool HasNextLayer() =>
            (CurrentMap?.Layers != null) && (CurrentLayer + 1 < CurrentMap.Layers.Length);

        public bool HasNextMap() =>
            (GlobalMap?.Maps != null) && (CurrentMapIndex + 1 < GlobalMap.Maps.Count);

        public void MoveToNextMap()
        {
            CurrentMapIndex++;
            CurrentLayer = 0;
            CurrentNodeIndex = 0;
        }
        
        public void Reset()
        {
            Initialized = false;
            GlobalMap = null;

            CurrentMapIndex = 0;
            CurrentLayer = 0;
            CurrentNodeIndex = 0;

            CurrentStageConfig = null;
        }
    }
}