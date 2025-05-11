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
            return vec switch
            {
                > 0 => EntityFacing.RIGHT,
                < 0 => EntityFacing.LEFT,
                _ => EntityFacing.DEFAULT
            };
        }
    }
}