namespace TerryBros.Utils
{
    public class IntBBox
    {
        public IntVector3 Mins { get; set; }
        public IntVector3 Maxs { get; set; }
        public IntVector3 Center { get; set; }

        public IntBBox(BBox bbox)
        {
            Mins = new(bbox.Mins);
            Maxs = new(bbox.Maxs);
            Center = new(bbox.Center);
        }

        public IntBBox(IntVector3 mins, IntVector3 maxs)
        {
            Mins = mins;
            Maxs = maxs;

            Center = CalculateCenter(Mins, Maxs);
        }

        private static IntVector3 CalculateCenter(IntVector3 mins, IntVector3 maxs)
        {
            return mins + (maxs - mins) / 2;
        }

        public static IntBBox Zero => new(IntVector3.Zero, IntVector3.Zero);
    }
}
