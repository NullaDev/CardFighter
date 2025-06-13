using System.Collections.Generic;
using Registry.Data;
using UnityEngine;

namespace Registry
{
    public class BuffDisplayManager
    {
        public bool HasLoaded = false;

        private const string BuffFolderRoot = "Buffs/";
        private readonly List<BuffDisplayInfo> _listBuffInfos = new();

        public void DebugLoadedBuffInfo()
        {
            Debug.Log("Loading buffs, total number:" + this._listBuffInfos.Count);
            // foreach (var buff in this._listBuffInfos)
            // {
            //     Debug.Log("name:" + buff.Name);
            // }
        }
        
        public void LoadFromFile()
        {
            if (HasLoaded) return;
            this.HasLoaded = true;

            var buffList = Resources.LoadAll<TextAsset>(BuffFolderRoot);
            foreach (var buff in buffList)
            {
                this._listBuffInfos.Add(BuffDisplayInfo.CreateFromJson(buff.text));
            }    
            DebugLoadedBuffInfo();
        }
        
        public BuffDisplayInfo Find(string buffName)
        {
            return this._listBuffInfos.Find(c => c.ID.Equals(buffName));
        }
    }
    
}