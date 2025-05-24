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
        public readonly int LayerDifficulty;
        public float PosX;
        public float PosY;

        public MapNode(NodeType type, int layerDifficulty=-1)
        {
            this.Type = type;
            this.LayerDifficulty = layerDifficulty;
        }
    }
    public class MapEdge
    {
        public MapNode From;
        public MapNode To;
    }
    
    public class RogueMap
    {
        public static RogueMap GlobalMap = null;
        
        public readonly Random Random;
        public readonly List<MapNode>[] AllNodes;
        public readonly List<MapEdge> AllEdges = new();
        
        public int PlayerCurrentLayer = -1;
        public MapNode PlayerCurrentNode = null;

        private RogueMap(int layer, int? seed=null)
        {
            this.Random = seed.HasValue ? new Random(seed.Value) : new Random();
            this.AllNodes = new List<MapNode>[layer];
            Enumerable.Range(0, layer).ToList().ForEach(l => AllNodes[l] = new List<MapNode>());
        }

        public int GetLayer()
        {
            return AllNodes.Length;
        }

        public MapNode GetStartNode()
        {
            return this.AllNodes[0].First();
        }

        public void SetPlayerNode(MapNode node)
        {
            this.PlayerCurrentNode = node;
            this.PlayerCurrentLayer = Enumerable.Range(0, this.GetLayer()).FirstOrDefault(l => AllNodes[l].Contains(node));
        }

        private Dictionary<NodeType, int> GetNodeTypeWeights(int layer)
        {
            if (layer is 0 or 1)
                return new Dictionary<NodeType, int> { { NodeType.FIGHT, 100 } };

            if (layer == this.GetLayer() - 1)
                return new Dictionary<NodeType, int> { { NodeType.BOSS, 100 } };

            if (layer == this.GetLayer() - 2)
                return new Dictionary<NodeType, int> { { NodeType.REST, 100 } };

            return new Dictionary<NodeType, int>
            {
                { NodeType.FIGHT, 40 },
                { NodeType.ELITE_FIGHT, 20 },
                { NodeType.REST, 20 },
                { NodeType.EVENT, 20 }
            };
        }
        
        private NodeType GetWeightedRandomNodeType(int layer)
        {
            var weights = GetNodeTypeWeights(layer);
            var total = weights.Values.Sum();
            var roll = Random.Next(total);

            foreach (var kvp in weights)
            {
                roll -= kvp.Value;
                if (roll < 0)
                    return kvp.Key;
            }

            return NodeType.FIGHT;
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
                var node = new MapNode(GetWeightedRandomNodeType(layer), layer);
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

                foreach (var upper in this.AllNodes[layer])
                {
                    var closest = this.AllNodes[layer - 1].OrderBy(lower => Math.Abs(lower.PosX - upper.PosX)).First();
                    this.AllEdges.Add(new MapEdge { From = closest, To = upper });
                    connectedLowerNodes.Add(closest);
                }

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
            Enumerable.Range(0, layer).ToList().ForEach(map.GenerateNodesAtLayer);
            map.GenerateEdges();
            return map;
        }
    }
}