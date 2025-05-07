using System;
using System.Collections.Generic;
using System.Linq;
using RogueMap;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Render
{
    public class RogueMapRender: MonoBehaviour
    {
        public GameObject NodePrefab;
        public GameObject NodeContainer;
        public GameObject EdgeContainer;

        private Random _random = new Random(19260817);
        
        private Dictionary<MapNode, GameObject> _listNodes = new();
        private List<GameObject> _listEdges = new();
        
        public static GameObject DrawLine(Vector3 start, Vector3 end, Transform parent, Color color, float thickness = 5f)
        {
            GameObject lineObj = new GameObject("UILine", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            lineObj.transform.SetParent(parent, false);

            RectTransform rectTransform = lineObj.GetComponent<RectTransform>();
            Image image = lineObj.GetComponent<Image>();
            image.color = color;

            // 计算中点和方向
            Vector2 start2D = new Vector2(start.x, start.y);
            Vector2 end2D = new Vector2(end.x, end.y);
            Vector2 dir = end2D - start2D;
            Vector2 midpoint = (start2D + end2D) / 2f;

            // 设置位置
            rectTransform.position = midpoint;

            // 设置旋转
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);

            // 设置宽度（长度）和高度（粗细）
            rectTransform.sizeDelta = new Vector2(dir.magnitude, thickness);

            return lineObj;
        }

        public void RenderRogueMap(RogueMap.RogueMap map)
        {
            foreach (var (node, obj) in _listNodes)
            {
                GameObject.Destroy(obj);
            }
            foreach (var edge in _listEdges)
            {
                GameObject.Destroy(edge);
            }
            _listNodes.Clear();
            _listEdges.Clear();

            foreach (var lineNode in map.AllNodes)
            {
                foreach (var node in lineNode)
                {
                    var nodeEntity = GameObject.Instantiate(NodePrefab, NodeContainer.transform);
                    _listNodes[node] = nodeEntity;

                    var nodeInteract = nodeEntity.GetComponent<NodeInteract>();
                    nodeInteract.Node = node;

                    var nodeText = nodeEntity.transform.Find("NodeText").GetComponent<Text>();;
                    nodeText.text = node.Type switch
                    {
                        NodeType.FIGHT => "戰",
                        NodeType.ELITE_FIGHT => "驍",
                        NodeType.REST => "憩",
                        NodeType.EVENT => "變",
                        NodeType.BOSS => "魁",
                        _ => "無"
                    };
                    
                    var width = NodeContainer.GetComponent<RectTransform>().rect.width;
                    var height = NodeContainer.GetComponent<RectTransform>().rect.height;
                    var x = (float)(node.PosX + 0.04 * _random.NextDouble() - 0.02) * width;
                    var y = (float)(node.PosY + 0.04 * _random.NextDouble() - 0.02) * height;
                    
                    var buttonRect = nodeEntity.GetComponent<RectTransform>();
                    buttonRect.anchoredPosition = new Vector2(x, y);
                }
            }
            
            foreach (var edge in map.AllEdges)
            {
                var point1 = _listNodes[edge.From].GetComponent<RectTransform>().position;
                var point2 = _listNodes[edge.To].GetComponent<RectTransform>().position;
                var lineObj = DrawLine(point1, point2, EdgeContainer.transform, Color.white);
                _listEdges.Add(lineObj);
            }

            ReRenderAccordingToPlayerPos(map);
        }

        public void ReRenderAccordingToPlayerPos(RogueMap.RogueMap map)
        {
            foreach (var (node, nodeEntity) in this._listNodes)
            {
                var bg = nodeEntity.transform.Find("NodeBG").GetComponent<Image>();
                if (map.PlayerCurrentNode == null)
                {
                    bg.color = node == map.GetStartNode() ? Color.red : Color.gray;
                }
                else if (node == map.PlayerCurrentNode)
                {
                    bg.color = Color.red;
                }
                else if (map.AllEdges.Any(edge => edge.From == map.PlayerCurrentNode && edge.To == node))
                {
                    bg.color = Color.green;
                }
                else
                {
                    bg.color = Color.gray;
                }
            }
        }

    }
}