using System.Linq;

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
            card.Behaviors = card1.Behaviors.Concat(card2.Behaviors).ToList();
            return card;
        }
    }
}