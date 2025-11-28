using System.Collections.Generic;
using Registry.Data;
using UnityEngine;
using Random = System.Random;

namespace GameLogic.Map
{
    public class GlobalMap
    {
        public readonly List<RogueMap> Maps = new();

        public GlobalMap(GlobalMapConfig config, int rngSeed)
        {
            var rng = new Random(rngSeed);

            foreach (var mapCfg in config.Maps)
            {
                var mapSeed = rng.Next();
                var rogueMap = new RogueMap(mapCfg, mapSeed);
                Maps.Add(rogueMap);
            }
        }
        
    }
}