namespace GameLogic.Entity
{
    public enum EntityFacing
    {
        Left = -1,
        Default = 0,
        Right = 1,
    }

    public static class FacingHelper
    {
        public static EntityFacing GetFacing(int vec)
        {
            return vec switch
            {
                > 0 => EntityFacing.Right,
                < 0 => EntityFacing.Left,
                _ => EntityFacing.Default
            };
        }
    }
}