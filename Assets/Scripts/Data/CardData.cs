using System;
using System.Collections.Generic;
using Card;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Data
{
    public class CardData : MonoBehaviour
    {
        public static CardData Instance;

        private const string CardFolderRoot = "Cards/";
        public static string[] SubFolders = {"Generic", "Attack", "Test"};
        private List<CardPrototype> CardList = new();

        void Start()
        {
            Debug.Log("Loading cards, total number:" + this.CardList.Count);
            foreach (var card in this.CardList)
            {
                Debug.Log("name:" + card.Name);
            }
        }

        void Update()
        {

        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            foreach (var subFolder in SubFolders)
            {
                var fullPath = CardFolderRoot + subFolder;
                var cardList = Resources.LoadAll<TextAsset>(fullPath);
                foreach (var card in cardList)
                {
                    this.CardList.Add(CardPrototype.CreateFromJson(card.text));
                }            
            }
        }

        public CardPrototype Find(string cardName)
        {
            return this.CardList.Find(c => c.ID.Equals(cardName));
        }
    }
}