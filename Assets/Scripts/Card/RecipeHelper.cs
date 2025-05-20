using System.Linq;
using Registry.Data;

namespace Card
{
    public class RecipeHelper
    {
        public static CardPrototype DefaultMergeTwoCards(CardPrototype card1, CardPrototype card2)
        {
            var card = new CardPrototype();
            card.Name = "杂交卡片";
            card.Cost = card1.Cost + card2.Cost + 1;
            card.Desc = "使用" + card1.Name + "和" + card2.Name;
            card.Actions = card1.Actions.Concat(card2.Actions).ToList();
            return card;
        }
    }
}