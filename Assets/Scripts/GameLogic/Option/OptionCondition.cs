using System.Linq;
using GameLogic.Runtime;

namespace GameLogic.Option
{
    public enum OptionSubject
    {
        PlayerClass,
        Gold,
        Hp,
        MaxHp,
        HasCard,
        HasItem
    }
    
    public class OptionCondition
    {
        public OptionSubject Subject { get; set; }
        public RelationalOperator Operator { get; set; }
        public string Value { get; set; } = "";

        public bool IsMet(PlayerData playerData)
        {
            switch (Subject)
            {
                case OptionSubject.PlayerClass:
                {
                    var playerClass = playerData.PlayerClass.ToString() ?? "";
                    return OperatorUtils.Compare(playerClass, Operator, Value);
                }

                case OptionSubject.Gold:
                    return OperatorUtils.Compare(playerData.InGameGold, Operator, ParseNumber(Value));

                case OptionSubject.Hp:
                    return OperatorUtils.Compare(playerData.Hp, Operator, ParseNumber(Value));

                case OptionSubject.MaxHp:
                    return OperatorUtils.Compare(playerData.MaxHp, Operator, ParseNumber(Value));
                
                case OptionSubject.HasCard:
                    return playerData.HeldCards.Keys.Any(c=>c.ID.Equals(Value));
                
                case OptionSubject.HasItem:
                    return playerData.HeldItems.Any(i => i.ID.Equals(Value));

                default:
                    return false;
            }
        }

        private static double ParseNumber(string s) =>
            double.TryParse(s, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var v) ? v : 0d;
    }
}