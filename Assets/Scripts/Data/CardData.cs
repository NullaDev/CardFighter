using System;
using System.Collections.Generic;
using Card;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Data
{
    public class CardData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static readonly string CardFolder = "Cards/";
        public static CardData Instance;
        public List<CardPrototype> CardList = new();

        // 卡牌展示
        private Vector2 OriginalPos;
        private Vector3 OriginalScale;
        private Color OriginalColor;


        void Start()
        {
            Debug.Log("Loaded card number:" + this.CardList.Count);
            foreach (var card in this.CardList)
            {
                Debug.Log("name:" + card.Name);
            }
            OriginalScale = transform.localScale;
            OriginalColor = transform.Find("CardBorder").GetComponent<SpriteRenderer>().color;
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

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = OriginalScale;
            transform.Find("CardBorder").GetComponent<SpriteRenderer>().material.SetColor("", OriginalColor);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = OriginalScale * 1.5f;
            transform.Find("CardBorder").GetComponent<SpriteRenderer>().material.SetColor("", Color.yellow);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OriginalPos = transform.GetComponent<RectTransform>().anchoredPosition;

        }


        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out pos
            ))
            {
                transform.GetComponent<RectTransform>().anchoredPosition = pos;   
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.GetComponent<RectTransform>().anchoredPosition = OriginalPos;
        }
    }
}