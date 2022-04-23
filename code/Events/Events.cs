namespace TerryBros.Events
{
    public static class TBEvent
    {
        public static class Level
        {
            /// <summary>
            /// Triggered when a level is restarted.
            /// </summary>
            public const string RESTART = "terrybros.level.restart";

            /// <summary>
            /// Triggered when one player reached the goal. <c>TerryBrosPlayer</c> object is passed to events.
            /// </summary>
            public const string GOAL_REACHED = "terrybros.level.goalreached";

            /// <summary>
            /// Triggered when a level is finished.
            /// </summary>
            public const string FINISHED = "terrybros.level.finished";

            /// <summary>
            /// Triggered when a level was loaded completely.
            /// </summary>
            public const string LOADED = "terrybros.level.loaded";

            /// <summary>
            /// Triggered when a level was cleared completely.
            /// </summary>
            public const string CLEARED = "terrybros.level.cleared";
        }
    }
}
