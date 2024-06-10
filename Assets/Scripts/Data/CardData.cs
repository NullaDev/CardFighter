using System;
using System.Collections.Generic;
using Card;
using UnityEngine;

namespace Data
{
    public class CardData: MonoBehaviour
    {
        public static readonly string CardFolder = "Cards/";
        public static CardData Instance;
        public List<CardPrototype> CardList = new();

        void Start()
        {
            Debug.Log("Loaded card number:" + this.CardList.Count);
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
    }
}