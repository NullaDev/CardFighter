using Registry;

namespace GameLogic.Option
{
    public enum OptionSubject
    {
        PlayerClass,
        Gold,
        Hp,
        MaxHp
    }
    
    public class OptionCondition
    {
        public OptionSubject Subject { get; set; }
        public RelationalOperator Operator { get; set; }
        public string Value { get; set; }

        public bool IsMet(PlayerData playerData)
        {
            switch (Subject)
            {
                case OptionSubject.PlayerClass:
                {
                    var playerClass = playerData.PlayerClass.ToString() ?? "";
                    return OperatorUtils.Compare(playerClass, Operator, Value ?? "");
                }

                case OptionSubject.Gold:
                    return OperatorUtils.Compare(playerData.InGameGold, Operator, ParseNumber(Value));

                case OptionSubject.Hp:
                    return OperatorUtils.Compare(playerData.Hp, Operator, ParseNumber(Value));

                case OptionSubject.MaxHp:
                    return OperatorUtils.Compare(playerData.MaxHp, Operator, ParseNumber(Value));

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