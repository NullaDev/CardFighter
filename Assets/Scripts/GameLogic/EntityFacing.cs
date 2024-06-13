namespace GameLogic
{
    public enum EntityFacing
    {
        LEFT = -1,
        DEFAULT = 0,
        RIGHT = 1,
    }

    public class FacingHelper
    {
        public static EntityFacing GetFacing(int vec)
        {
            if (vec > 0) return EntityFacing.RIGHT;
            if (vec < 0) return EntityFacing.LEFT;
            return EntityFacing.DEFAULT;
        }
    }
}