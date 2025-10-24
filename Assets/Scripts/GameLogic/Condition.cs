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
                
                case "target_facing_me":
                    selfIndex = battleField.GetEntityIndex(self);
                    targetIndex = battleField.GetEntityIndex(target);

                    if (target.Facing == EntityFacing.Default)
                        return false;

                    var targetFacingMe =
                        (selfIndex < targetIndex && target.Facing == EntityFacing.Left) ||
                        (selfIndex > targetIndex && target.Facing == EntityFacing.Right);

                    return targetFacingMe;
                
                case "target_back_to_me":
                    selfIndex = battleField.GetEntityIndex(self);
                    targetIndex = battleField.GetEntityIndex(target);

                    if (target.Facing == EntityFacing.Default)
                        return false;

                    var targetBackToMe =
                        (selfIndex < targetIndex && target.Facing == EntityFacing.Right) ||
                        (selfIndex > targetIndex && target.Facing == EntityFacing.Left);

                    return targetBackToMe;
                
                case "dealt_damage_to_player":
                    return target is Enemy { DealtDamageToPlayer: true };

                // case "is_in_insight": ...
                // case "is_super_armor": ...

                default:
                    return false;
            }
        }
    }
}