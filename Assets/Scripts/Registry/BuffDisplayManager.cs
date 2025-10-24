using System;
using System.Collections.Generic;
using System.IO;
using Registry.Data;
using UnityEngine;

namespace Registry
{
    public class BuffDisplayManager
    {
        public bool HasLoaded = false;

        private static readonly string BuffFolderRoot = Path.Combine(Application.dataPath, "../GameData/Render/BuffDisplay");
        private readonly List<BuffDisplayInfo> _listBuffInfos = new();

        public void DebugLoadedBuffInfo()
        {
            Debug.Log($"[BuffDisplayManager] Loaded {this._listBuffInfos.Count} buffs.");
            // foreach (var buff in this._listBuffInfos)
            // {
            //     Debug.Log($"name: {buff.Name}");
            // }
        }
        
        public void LoadFromFile()
        {
            if (HasLoaded) return;
            this.HasLoaded = true;

            if (!Directory.Exists(BuffFolderRoot))
            {
                Debug.LogError($"[BuffDisplayManager] Buff folder not found: {BuffFolderRoot}");
                return;
            }

            var jsonFiles = Directory.GetFiles(BuffFolderRoot, "*.json", SearchOption.AllDirectories);
            if (jsonFiles.Length == 0)
            {
                Debug.LogWarning($"[CardDataManager] No buff json files found in {BuffFolderRoot}");
                return;
            }
            
            foreach (var file in jsonFiles)
            {
                try
                {
                    var jsonText = File.ReadAllText(file);
                    var buff = BuffDisplayInfo.CreateFromJson(jsonText);
                    if (buff != null)
                        _listBuffInfos.Add(buff);
                    else
                        Debug.LogWarning($"[BuffDisplayManager] Failed to parse buff file: {file}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BuffDisplayManager] Error loading {file}: {e.Message}");
                }
            }

            DebugLoadedBuffInfo();
        }
        
        public BuffDisplayInfo Find(string buffName)
        {
            return this._listBuffInfos.Find(c => c.ID.Equals(buffName));
        }
    }
    
}