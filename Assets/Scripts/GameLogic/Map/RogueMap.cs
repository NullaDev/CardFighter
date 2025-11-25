using System;
using System.Collections.Generic;
using System.Linq;
using Registry.Data;

namespace GameLogic.Map
{
    public enum NodeType
    {
        FIGHT,
        ELITE_FIGHT,
        REST,
        EVENT,
        BOSS
    }
    
    public class MapNode
    {
        public NodeType Type;
        public int Layer;
        public float PosX;
        public float PosY;
    }
    public class MapEdge
    {
        public MapNode From;
        public MapNode To;
    }
    
    public class RogueMap
    {
        public readonly RogueMapConfig Config;
        private readonly Random _rng;

        public readonly List<MapNode>[] Layers;
        public readonly List<MapEdge> Edges = new();
        
        public RogueMap(RogueMapConfig cfg, int seed)
        {
            this.Config = cfg;
            this._rng = new Random(seed);

            Layers = new List<MapNode>[cfg.Layers];
            for (var i = 0; i < cfg.Layers; i++)
                Layers[i] = new List<MapNode>();

            GenerateNodes();
            GenerateEdges();
        }

        public MapNode GetStartNode()
        {
            return this.Layers[0].First();
        }

        private Dictionary<NodeType, int> GetLayerWeights(int layer)
        {
            var specific = Config.LayerParams?.FirstOrDefault(lp => lp.Layer == layer);
            return ParseNodeDict(specific != null ? specific.NodeTypeWeights : Config.DefaultLayerParams);
        }

        private static Dictionary<NodeType, int> ParseNodeDict(Dictionary<string, int> src)
        {
            Dictionary<NodeType, int> dst = new();
            foreach (var kv in src)
            {
                if (Enum.TryParse<NodeType>(kv.Key, out var type))
                    dst[type] = kv.Value;
            }
            return dst;
        }

        private NodeType WeightedRandomNodeType(int layer)
        {
            var dict = GetLayerWeights(layer);
            var roll = _rng.Next(dict.Values.Sum());
            foreach (var kv in dict)
            {
                roll -= kv.Value;
                if (roll < 0)
                    return kv.Key;
            }
            return NodeType.FIGHT;
        }

        private void GenerateNodes()
        {
            for (var layer = 0; layer < Config.Layers; layer++)
            {
                var count = ComputeNodeCountBasedOnPrevious(layer);
                for (var i = 0; i < count; i++)
                {
                    var node = new MapNode
                    {
                        Layer = layer,
                        Type = WeightedRandomNodeType(layer),
                        PosX = (i + 1f) / (count + 1f),
                        PosY = (layer + 1f) / (Config.Layers + 1f)
                    };
                    Layers[layer].Add(node);
                }
            }
        }
        
        private int ComputeNodeCountBasedOnPrevious(int layer)
        {
            if (layer == 0)
                return 1;
            
            var config = Config.NodeCountRange;
            var prev = Layers[layer - 1].Count;
            var maxByFactor = (int)(prev * config.Factor);
            var finalMax = Math.Clamp(maxByFactor, config.Min, config.Max);
            return _rng.Next(config.Min, finalMax + 1);
        }

        private void GenerateEdges()
        {
            for (var layer = 1; layer < Config.Layers; layer++)
            {
                var prev = Layers[layer - 1];
                var curr = Layers[layer];

                HashSet<MapNode> linkedPrev = new();

                foreach (var upper in curr)
                {
                    var closest = prev.OrderBy(n => Math.Abs(n.PosX - upper.PosX)).First();
                    Edges.Add(new MapEdge { From = closest, To = upper });
                    linkedPrev.Add(closest);
                }

                foreach (var lower in prev)
                {
                    if (!linkedPrev.Contains(lower))
                    {
                        var closest = curr.OrderBy(n => Math.Abs(n.PosX - lower.PosX)).First();
                        Edges.Add(new MapEdge { From = lower, To = closest });
                    }
                }
            }
        }
    }
}