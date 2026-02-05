namespace GameLogic.SaveLoad
{
    public class SaveSummary
    {
        public int SlotIndex;
        public int MapIndex;
        public int MapCount;
        public int CurrentLayer;
        public int LayerCount;
        public int Hp;
        public int MaxHp;
        public int CardCount;
        public int MaxCardCount;
        public int ItemCount;

        public string ToDisplayText()
        {
            return
                $"存档编号：{SlotIndex}\n" +
                $"位于地图：{MapIndex + 1}/{MapCount}\n" +
                $"当前层数：{CurrentLayer + 1}/{LayerCount}\n" +
                $"当前血量：{Hp}/{MaxHp}\n" +
                $"持有卡牌：{CardCount}/{MaxCardCount}\n" +
                $"物品数量：{ItemCount}";
        }
    }
}