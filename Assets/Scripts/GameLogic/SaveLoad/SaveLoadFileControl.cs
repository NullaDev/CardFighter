using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameLogic.Map;
using GameLogic.Runtime;
using Newtonsoft.Json;
using Registry;
using Registry.Data;
using UnityEngine;

namespace GameLogic.SaveLoad
{
    public static class SaveLoadFileControl
    {
        public const int MaxSaveCount = 40;
        
        private const string GlobalMapConfigFile = "global_map.json";
        private const string MapDataFile        = "map_state.json";
        private const string PlayerDataFile     = "player.json";
        private const string MiscDataFile       = "misc.json";

        private static readonly string SaveRootPath =
            Path.Combine(Application.dataPath, "../GameData/SaveData");
        
        public static void Save(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);

            var saveDir = GetSaveDir(slotIndex);
            EnsureDirectory(saveDir);

            StoreMapConfig(saveDir);
            StoreMapData(saveDir);
            StorePlayerData(saveDir);
            StoreMiscData(saveDir);
        }

        public static bool Load(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);

            var saveDir = GetSaveDir(slotIndex);
            if (!Directory.Exists(saveDir))
            {
                Debug.LogError($"Save directory not found: {saveDir}");
                return false;
            }

            try
            {
                LoadMiscData(saveDir);
                LoadMapConfig(saveDir); 
                LoadMapData(saveDir);
                LoadPlayerData(saveDir);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load save slot {slotIndex}:\n{e}");
                return false;
            }

            return true;
        }
        
        public static bool HasSaveAt(int slotIndex)
        {
            if (slotIndex is < 0 or >= MaxSaveCount)
                return false;

            var saveDir = GetSaveDir(slotIndex);
            if (!Directory.Exists(saveDir))
                return false;

            return
                File.Exists(Path.Combine(saveDir, GlobalMapConfigFile)) &&
                File.Exists(Path.Combine(saveDir, MapDataFile)) &&
                File.Exists(Path.Combine(saveDir, PlayerDataFile)) &&
                File.Exists(Path.Combine(saveDir, MiscDataFile));
        }
        
        private static string GetSaveDir(int slotIndex)
        {
            return Path.Combine(SaveRootPath, slotIndex.ToString());
        }

        private static void ValidateSlotIndex(int slotIndex)
        {
            if (slotIndex is < 0 or >= MaxSaveCount)
                throw new ArgumentOutOfRangeException(
                    nameof(slotIndex),
                    $"Save slot must be between 0 and {MaxSaveCount - 1}"
                );
        }

        private static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        
        public static SaveSummary GetSaveSummary(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);

            var saveDir = GetSaveDir(slotIndex);
            if (!Directory.Exists(saveDir))
                return null;

            try
            {
                var mapConfigPath = Path.Combine(saveDir, GlobalMapConfigFile);
                var mapConfigJson = File.ReadAllText(mapConfigPath);
                var mapConfig = GlobalMapConfig.CreateFromJson(mapConfigJson);

                var mapStatePath = Path.Combine(saveDir, MapDataFile);
                var mapStateJson = File.ReadAllText(mapStatePath);
                var mapState = JsonConvert.DeserializeObject<MapSaveData>(mapStateJson);

                var playerPath = Path.Combine(saveDir, PlayerDataFile);
                var playerJson = File.ReadAllText(playerPath);
                var player = JsonConvert.DeserializeObject<PlayerSaveData>(playerJson);

                if (mapState == null || player == null || mapConfig == null)
                    return null;

                var currentMap = mapConfig.Maps[mapState.MapIndex];

                return new SaveSummary
                {
                    SlotIndex = slotIndex,
                    MapIndex = mapState.MapIndex,
                    MapCount = mapConfig.Maps.Count,
                    CurrentLayer = mapState.Layer,
                    LayerCount = currentMap.Layers,
                    Hp = player.Hp,
                    MaxHp = player.MaxHp,
                    CardCount = player.HeldCards?.Count ?? 0,
                    MaxCardCount = player.HeldCards?.Values.Sum() ?? 0,
                    ItemCount = player.HeldItems?.Count ?? 0
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read save summary for slot {slotIndex}:\n{e}");
                return null;
            }
        }
        
        private static void StoreMapConfig(string saveDir)
        {
            var globalMap = MapData.Instance.GlobalMap;
            if (globalMap?.Config == null)
                throw new InvalidOperationException("GlobalMapConfig is not initialized.");

            var path = Path.Combine(saveDir, GlobalMapConfigFile);
            var json = globalMap.Config.ToJson(pretty: true);
            File.WriteAllText(path, json);
        }

        private static void StoreMapData(string saveDir)
        {
            var mapData = MapData.Instance;
            if (mapData is not { Initialized: true })
                throw new System.InvalidOperationException("MapData is not initialized.");

            var saveData = new MapSaveData
            {
                MapIndex = mapData.CurrentMapIndex,
                Layer = mapData.ConfirmedLayer,
                NodeIndex = mapData.ConfirmedNodeIndex
            };

            var path = Path.Combine(saveDir, MapDataFile);

            var json = JsonConvert.SerializeObject(
                saveData,
                Formatting.Indented
            );

            File.WriteAllText(path, json);
        }

        private static void StorePlayerData(string saveDir)
        {
            var player = PlayerData.Instance;

            var save = new PlayerSaveData
            {
                PlayerClass = player.PlayerClass.ToString(),
                Hp = player.Hp,
                MaxHp = player.MaxHp,
                InitialInGameCost = player.InitialInGameCost,
                MaxInGameCost = player.MaxInGameCost,
                MaxCarryCost = player.MaxCarryCost,
                InGameGold = player.InGameGold,
                HeldCards = new Dictionary<string, int>(),
                CardOperations = new List<string>(),
                HeldItems = new List<string>()
            };

            foreach (var (card, count) in player.HeldCards)
            {
                if (card != null)
                    save.HeldCards[card.ID] = count;
            }

            foreach (var card in player.CardOperations.GetAllCards())
            {
                if (card != null)
                    save.CardOperations.Add(card.ID);
            }

            foreach (var item in player.HeldItems)
            {
                if (item != null)
                    save.HeldItems.Add(item.ID);
            }

            var json = JsonConvert.SerializeObject(
                save,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );

            var path = Path.Combine(saveDir, PlayerDataFile);
            File.WriteAllText(path, json);
        }

        private static void StoreMiscData(string saveDir)
        {
            var misc = MiscData.Instance;
            if (misc == null)
                throw new System.InvalidOperationException("MiscData is not initialized.");

            var saveData = new MiscSaveData
            {
                Seed = misc.Seed
            };

            var path = Path.Combine(saveDir, MiscDataFile);

            var json = JsonConvert.SerializeObject(
                saveData,
                Formatting.Indented
            );

            File.WriteAllText(path, json);
        }
        
        private static void LoadMiscData(string saveDir)
        {
            var path = Path.Combine(saveDir, MiscDataFile);
            if (!File.Exists(path))
                throw new FileNotFoundException("Misc save file not found.", path);
            var json = File.ReadAllText(path);
            var save = JsonConvert.DeserializeObject<MiscSaveData>(json);
            if (save == null)
                throw new Exception("Failed to deserialize MiscSaveData.");

            MiscData.Instance.InitSeed(save.Seed);
        }
        
        private static void LoadMapConfig(string saveDir)
        {
            var path = Path.Combine(saveDir, GlobalMapConfigFile);
            if (!File.Exists(path))
                throw new FileNotFoundException("GlobalMapConfig file not found.", path);
            var json = File.ReadAllText(path);
            
            GlobalMapConfig config;
            try
            {
                config = GlobalMapConfig.CreateFromJson(json);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to parse GlobalMapConfig.", e);
            }

            MapData.Instance.InitializeFromSave(new GlobalMap(config, MiscData.Instance.Seed));
        }
        
        private static void LoadMapData(string saveDir)
        {
            var path = Path.Combine(saveDir, MapDataFile);
            if (!File.Exists(path))
                throw new FileNotFoundException("MapData save file not found.", path);

            var json = File.ReadAllText(path);
            var save = JsonConvert.DeserializeObject<MapSaveData>(json);

            if (save == null)
                throw new Exception("Failed to deserialize MapSaveData.");

            var mapData = MapData.Instance;

            if (!mapData.Initialized || mapData.GlobalMap == null)
                throw new InvalidOperationException("MapData is not initialized before loading map state.");

            mapData.CurrentMapIndex = save.MapIndex;
            mapData.CurrentLayer = mapData.ConfirmedLayer = save.Layer;
            mapData.CurrentNodeIndex = mapData.ConfirmedNodeIndex = save.NodeIndex;
        }
        
        private static void LoadPlayerData(string saveDir)
        {
            var path = Path.Combine(saveDir, PlayerDataFile);
            if (!File.Exists(path))
                throw new FileNotFoundException("Player save file not found.", path);

            var json = File.ReadAllText(path);
            var save = JsonConvert.DeserializeObject<PlayerSaveData>(json);

            if (save == null)
                throw new Exception("Failed to deserialize PlayerSaveData.");

            var player = PlayerData.Instance;

            if (!Enum.TryParse(save.PlayerClass, out PlayerClass playerClass))
            {
                Debug.LogError($"Unknown PlayerClass in save: {save.PlayerClass}");
                playerClass = PlayerClass.GENERIC;
            }
            player.PlayerClass = playerClass;
            player.Hp = save.Hp;
            player.MaxHp = save.MaxHp;
            player.InitialInGameCost = save.InitialInGameCost;
            player.MaxInGameCost = save.MaxInGameCost;
            player.MaxCarryCost = save.MaxCarryCost;
            player.InGameGold = save.InGameGold;

            player.HeldCards.Clear();
            foreach (var (id, count) in save.HeldCards)
            {
                var card = Registry.StaticDataManager.CardDataManager.Find(id);
                if (card != null)
                    player.HeldCards[card] = count;
                else
                    Debug.LogError($"Card not found when loading save: {id}");
            }

            player.CardOperations.Clear();
            if (save.CardOperations == null || save.CardOperations.Count < 2)
            {
                throw new Exception(
                    $"Invalid CardOperations in save: expected at least 2 cards (MoveSlot, TurnSlot), " +
                    $"but got {(save.CardOperations?.Count ?? 0)}"
                );
            }
            {
                var moveId = save.CardOperations[0];
                var moveCard = Registry.StaticDataManager.CardDataManager.Find(moveId);
                if (moveCard == null)
                    throw new Exception($"MoveSlot card not found when loading save: {moveId}");
                player.CardOperations.SetMoveSlot(moveCard);
            }
            {
                var turnId = save.CardOperations[1];
                var turnCard = Registry.StaticDataManager.CardDataManager.Find(turnId);
                if (turnCard == null)
                    throw new Exception($"TurnSlot card not found when loading save: {turnId}");
                player.CardOperations.SetTurnSlot(turnCard);
            }
            for (var i = 2; i < save.CardOperations.Count; i++)
            {
                var id = save.CardOperations[i];
                var card = Registry.StaticDataManager.CardDataManager.Find(id);
                if (card != null)
                {
                    player.CardOperations.AddCard(card);
                }
                else
                {
                    Debug.LogError($"Card operation not found when loading save: {id}");
                }
            }

            player.HeldItems.Clear();
            foreach (var id in save.HeldItems)
            {
                var item = Registry.StaticDataManager.HeldItemDataManager.Find(id);
                if (item != null)
                    player.HeldItems.Add(item);
                else
                    Debug.LogError($"Item not found when loading save: {id}");
            }
        }
    }
}