using System.Collections.Generic;
using Card;
using Registry.Data;
using UnityEngine;

namespace Registry
{
    public class CardDataManager
    {
        private bool _hasLoaded = false;

        public const string CardFolderRoot = "Cards/";
        public static string[] SubFolders = {"Generic", "Attack", "Misc", "RU", "Test"};
        private readonly List<CardPrototype> _listCards = new();

        public void DebugLoadedCardInfo()
        {
            Debug.Log("Loading cards, total number:" + this._listCards.Count);
            foreach (var card in this._listCards)
            {
                Debug.Log("name:" + card.Name);
            }
        }

        public void LoadFromFile()
        {
            if (_hasLoaded) return;
            this._hasLoaded = true;
            foreach (var subFolder in SubFolders)
            {
                var fullPath = CardFolderRoot + subFolder;
                var cardList = Resources.LoadAll<TextAsset>(fullPath);
                foreach (var card in cardList)
                {
                    this._listCards.Add(CardPrototype.CreateFromJson(card.text));
                }
            }

            DebugLoadedCardInfo();
        }

        public CardPrototype Find(string cardName)
        {
            return this._listCards.Find(c => c.ID.Equals(cardName));
        }
    }
}