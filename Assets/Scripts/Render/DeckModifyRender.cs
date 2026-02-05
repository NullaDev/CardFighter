using System.Collections.Generic;
using GameLogic.Card;
using GameLogic.Runtime;
using JetBrains.Annotations;
using Registry;
using Registry.Data;
using Render.Component;
using Render.Interact;
using SceneControl;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class DeckModifyRender: MonoBehaviour
    {
        public GameObject BackpackCardPrefab;
        public GameObject BackpackCardGrid;
        public GameObject DeckCardPrefab;
        public GameObject DeckCardGrid;

        public GameObject RecipeSlot1;
        public GameObject RecipeSlot2;
        public GameObject RecipeSlotResult;
        
        private readonly List<GameObject> _listBackpackCards = new();
        private readonly List<GameObject> _listDeckCards = new();
        
        public void RenderBackpackCards(List<(CardPrototype, int)> cards)
        {
            var deckModify = GameObject.Find("DeckModify").GetComponent<DeckModifyControl>();
            
            foreach (var card in _listBackpackCards)
            {
                GameObject.Destroy(card);
            }
            _listBackpackCards.Clear();
            
            foreach (var (cardPrototype, count) in cards)
            {
                var cardObject = GameObject.Instantiate(BackpackCardPrefab, BackpackCardGrid.transform);
                var interact = cardObject.GetComponent<DeckModifyBackpackInteract>();
                interact.SetCard(cardPrototype);
                var render = cardObject.GetComponent<DeckModifyBackpackCardRender>();
                render.Render(new CardInstance(cardPrototype), count, deckModify.ChosenCard == cardPrototype);
                _listBackpackCards.Add(cardObject);
            }
        }

        public void RenderDeckCards()
        {
            foreach (var card in _listDeckCards)
            {
                GameObject.Destroy(card);
            }
            _listDeckCards.Clear();

            var playerData = PlayerData.Instance;
            var allCards = playerData.CardOperations.GetAllCards();
            while (allCards.Count < 2 + CardOperationsInBattle.MaxCardCount)
            {
                allCards.Add(null);
            }
            
            foreach (var cardPrototype in allCards)
            {
                var card = GameObject.Instantiate(DeckCardPrefab, DeckCardGrid.transform);
                var cardRender = card.GetComponent<DeckModifyDeckCardRender>();
                var interact = card.GetComponent<DeckModifyDeckInteract>();
                if (cardPrototype != null)
                {
                    cardRender.RenderCard(new CardInstance(cardPrototype));
                    interact.SetCard(cardPrototype);
                }
                else
                {
                    cardRender.RenderEmpty();
                }
                _listDeckCards.Add(card);
            }
        }

        public void RenderSlots([CanBeNull] CardPrototype card1, [CanBeNull] CardPrototype card2, [CanBeNull] CardPrototype cardResult)
        {
            var render1 = RecipeSlot1.transform.Find("CardNoInteract").GetComponent<CardRender>();
            var cover1 = RecipeSlot1.transform.Find("EmptySlotCover").GetComponent<Image>();
            var coverText1 = RecipeSlot1.transform.Find("EmptySlotText").GetComponent<Text>();
            if (card1 != null)
            {
                render1.RenderCard(new CardInstance(card1));
                cover1.enabled = false;
                coverText1.enabled = false;
            }
            else
            {
                render1.RenderCard(new CardInstance(CommonCards.DoNothing));
                cover1.enabled = true;
                coverText1.enabled = true;
            }
            
            var render2 = RecipeSlot2.transform.Find("CardNoInteract").GetComponent<CardRender>();
            var cover2 = RecipeSlot2.transform.Find("EmptySlotCover").GetComponent<Image>();
            var coverText2 = RecipeSlot2.transform.Find("EmptySlotText").GetComponent<Text>();
            if (card2 != null)
            {
                render2.RenderCard(new CardInstance(card2));
                cover2.enabled = false;
                coverText2.enabled = false;
            }
            else
            {
                render2.RenderCard(new CardInstance(CommonCards.DoNothing));
                cover2.enabled = true;
                coverText2.enabled = true;
            }
            
            var render3 = RecipeSlotResult.transform.Find("CardNoInteract").GetComponent<CardRender>();
            var cover3 = RecipeSlotResult.transform.Find("EmptySlotCover").GetComponent<Image>();
            var coverText3 = RecipeSlotResult.transform.Find("EmptySlotText").GetComponent<Text>();
            if (cardResult != null)
            {
                render3.RenderCard(new CardInstance(cardResult));
                cover3.enabled = false;
                coverText3.enabled = false;
            }
            else
            {
                render3.RenderCard(new CardInstance(CommonCards.DoNothing));
                cover3.enabled = true;
                coverText3.enabled = true;
            }
        }
    }
}