using System;
using System.Collections.Generic;
using Card;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Data
{
    public class CardData : MonoBehaviour
    {
        public static CardData Instance;

        private const string CardFolder = "Cards/";
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
            LoadFromFile();
            DontDestroyOnLoad(gameObject);
        }

        private void LoadFromFile()
        {
            var cardList = Resources.LoadAll<TextAsset>(CardFolder);
            foreach (var card in cardList)
            {
                this.CardList.Add(CardPrototype.CreateFromJson(card.text));
            }
        }

        public CardPrototype Find(string cardName)
        {
            return this.CardList.Find(c => c.Name.Equals(cardName));
        }
    }
}