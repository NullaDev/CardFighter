using System.Collections.Generic;
using System.Linq;
using Registry;
using Registry.Data;

namespace Item
{
    public class HeldItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string TextureName { get; set; }
        public string EffectText { get; set; }
        public string ExtraText { get; set; }
        
        public List<HeldItemEffect> Effects { get; set; } = new();

        public void PlayerTryObtain(PlayerData player)
        {
            if (player.HeldItems.Any(h => h.ID == this.ID))
                return;
            
            player.HeldItems.Add(this);
            foreach (var effect in Effects)
            {
                if (effect is not GrantCardOnObtainEffect grantEffect) continue;
                var gained = new List<CardPrototype>();
                for (var i = 0; i < grantEffect.Count; i++)
                {
                    var cardId = grantEffect.IsRandom ? grantEffect.CardIDs[player.Random.Next(grantEffect.CardIDs.Count)] : grantEffect.CardIDs[i % grantEffect.CardIDs.Count];
                    gained.Add(StaticDataManager.CardDataManager.Find(cardId));
                }

                foreach (var card in gained.Where(card => !player.HeldCards.TryAdd(card, 1)))
                {
                    player.HeldCards[card]++;
                }
            }
        }
    }
}