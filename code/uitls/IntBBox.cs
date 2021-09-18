namespace TerryBros.Utils
{
    public class IntBBox
    {
        public IntVector3 Mins;
        public IntVector3 Maxs;
        public IntVector3 Center;

        public IntBBox(BBox bbox)
        {
            Mins = new IntVector3(bbox.Mins);
            Maxs = new IntVector3(bbox.Maxs);
            Center = new IntVector3(bbox.Center);
        }

        public IntBBox(IntVector3 Mins, IntVector3 Maxs)
        {
            this.Mins = Mins;
            this.Maxs = Maxs;

            Center = calculateCenter(Mins, Maxs);
        }

        private IntVector3 calculateCenter(IntVector3 Mins, IntVector3 Maxs)
        {
            return Mins + (Maxs - Mins) / 2;
        }

        public static IntBBox Zero => new IntBBox(IntVector3.Zero, IntVector3.Zero);
    }
}
