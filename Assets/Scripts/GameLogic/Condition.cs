using GameLogic.Entity;

namespace GameLogic
{
    public static class Condition
    {
        public static bool CheckCondition(string condition, EntityBase self, EntityBase target, BattleField battleField)
        {
            switch (condition)
            {
                case "face_to_face":
                    var selfIndex = battleField.GetEntityIndex(self);
                    var targetIndex = battleField.GetEntityIndex(target);

                    if (self.Facing == EntityFacing.Default || target.Facing == EntityFacing.Default)
                        return false;

                    var isFacingEachOther =
                        (selfIndex < targetIndex && self.Facing == EntityFacing.Right && target.Facing == EntityFacing.Left) ||
                        (selfIndex > targetIndex && self.Facing == EntityFacing.Left && target.Facing == EntityFacing.Right);

                    return isFacingEachOther;

                // case "is_in_insight": ...
                // case "is_super_armor": ...

                default:
                    return false;
            }
        }
    }
}