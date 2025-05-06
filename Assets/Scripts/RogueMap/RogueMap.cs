using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueMap
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
        public readonly NodeType Type;
        public readonly int Difficulty;
        public float PosX;
        public float PosY;

        public MapNode(NodeType type, int difficulty=-1)
        {
            this.Type = type;
            this.Difficulty = difficulty;
        }
    }
    public class MapEdge
    {
        public MapNode From;
        public MapNode To;
    }
    
    public class RogueMap
    {
        public readonly Random Random;
        public readonly List<MapNode>[] AllNodes;
        public readonly List<MapEdge> AllEdges = new();
        
        public int PlayerCurrentLayer = 0;
        public MapNode PlayerCurrentNode = null;

        private RogueMap(int layer, int? seed=null)
        {
            this.Random = seed.HasValue ? new Random(seed.Value) : new Random();
            this.AllNodes = new List<MapNode>[layer];
            foreach (var l in Enumerable.Range(0, layer))
            {
                AllNodes[l] = new List<MapNode>();
            }
        }

        public int GetLayer()
        {
            return AllNodes.Length;
        }

        private List<NodeType> LegalTypesAtLayer(int layer)
        {
            if (layer is 0 or 1)
            {
                return new List<NodeType> { NodeType.FIGHT };
            }
            
            if (layer == this.GetLayer()-1)
            {
                return new List<NodeType> { NodeType.BOSS };
            }
            
            if (layer == this.GetLayer()-2)
            {
                return new List<NodeType> { NodeType.REST };
            }

            return new List<NodeType> { NodeType.FIGHT, NodeType.ELITE_FIGHT, NodeType.REST, NodeType.EVENT };
        }

        private void GenerateNodesAtLayer(int layer)
        {
            if (layer == this.GetLayer()-1)
            {
                var node = new MapNode(NodeType.BOSS);
                node.PosX = .5F;
                node.PosY = this.GetLayer() / (this.GetLayer() + 1F);
                this.AllNodes[layer].Add(node);
                return;
            }

            var thisLayerNodeNum = layer == 0? 1: Random.Next(2, Math.Clamp(2 * this.AllNodes[layer - 1].Count, 3, 6));
            foreach (var idx in Enumerable.Range(0, thisLayerNodeNum))
            {
                var legalTypes = this.LegalTypesAtLayer(layer);
                var node = new MapNode(legalTypes[Random.Next(legalTypes.Count)], difficulty:layer/3);
                node.PosX = (idx + 1F) / (thisLayerNodeNum + 1F);
                node.PosY = (layer + 1F) / (this.GetLayer() + 1F);
                this.AllNodes[layer].Add(node);
            }
        }

        private void GenerateEdges()
        {
            for (var layer = 1; layer < this.GetLayer(); layer++)
            {
                var connectedLowerNodes = new HashSet<MapNode>();

                // 每个上层节点连接到最接近的下层节点
                foreach (var upper in this.AllNodes[layer])
                {
                    var closest = this.AllNodes[layer - 1].OrderBy(lower => Math.Abs(lower.PosX - upper.PosX)).First();
                    this.AllEdges.Add(new MapEdge { From = closest, To = upper });
                    connectedLowerNodes.Add(closest);
                }

                // 每个没有连接的下层节点连接到最接近的上层节点
                foreach (var lower in this.AllNodes[layer - 1])
                {
                    if (!connectedLowerNodes.Contains(lower))
                    {
                        var closest = this.AllNodes[layer].OrderBy(upper => Math.Abs(upper.PosX - lower.PosX)).First();
                        this.AllEdges.Add(new MapEdge { From = lower, To = closest });
                    }
                }
            }
        }

        public static RogueMap GenerateRandomMap(int layer)
        {
            var map = new RogueMap(layer);
            foreach (var l in Enumerable.Range(0, layer))
            {
                map.GenerateNodesAtLayer(l);
            }
            map.GenerateEdges();
            return map;
        }
    }
}