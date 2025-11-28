using System;
using System.Collections.Generic;
using System.Linq;
using Registry.Data;
using UnityEngine;
using Random = System.Random;

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
        public float Complexity;
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
            var specific = Config.SpecificLayerParams?.FirstOrDefault(lp => lp.Layer == layer);
            var nodeWeights = specific?.NodeTypeWeights ?? Config.DefaultLayerParams;
            return ParseNodeDict(nodeWeights);
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

        private float GetComplexity(NodeType nodeType, int layer)
        {
            var specific = Config.SpecificLayerParams?.FirstOrDefault(lp => lp.Layer == layer);
            if (specific?.Complexity > 0)
                return specific.Complexity;
            
            var config = this.Config.Complexity;
            return nodeType switch
            {
                NodeType.FIGHT => config.NormalStart + config.NormalRamp * layer,
                NodeType.ELITE_FIGHT => config.EliteStart + config.NormalRamp * layer,
                _ => 0
            };
        }

        private void GenerateNodes()
        {
            for (var layer = 0; layer < Config.Layers; layer++)
            {
                var count = ComputeNodeCountBasedOnPrevious(layer);
                for (var i = 0; i < count; i++)
                {
                    var nodeType = WeightedRandomNodeType(layer);
                    var node = new MapNode
                    {
                        Complexity = GetComplexity(nodeType, layer),
                        Type = nodeType,
                        PosX = (i + 1f) / (count + 1f),
                        PosY = (layer + 1f) / (Config.Layers + 1f)
                    };
                    Layers[layer].Add(node);
                }
            }
        }
        
        private int ComputeNodeCountBasedOnPrevious(int layer)
        {
            var specific = Config.SpecificLayerParams?.FirstOrDefault(lp => lp.Layer == layer);
            if (specific?.NodeCount > 0)
                return specific.NodeCount;
            
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