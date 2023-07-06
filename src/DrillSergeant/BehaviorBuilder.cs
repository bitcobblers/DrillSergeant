using System.Threading;

namespace DrillSergeant
{
    public static class BehaviorBuilder
    {
        private static readonly AsyncLocal<Behavior?> Instance = new();

        /// <summary>
        /// Gets the current behavior to test.
        /// </summary>
        public static Behavior? CurrentBehavior => Instance.Value;

        /// <summary>
        /// Creates a new behavior to test.
        /// </summary>
        /// <returns>The new behavior to build.</returns>
        public static Behavior New() => New(new { });

        /// <summary>
        /// Creates a new behavior to build.
        /// </summary>
        /// <param name="input">The input parameters for the behavior.</param>
        /// <returns>The new behavior to build.</returns>
        public static Behavior New(object input)
        {
            Instance.Value = new Behavior(input);

            return Instance.Value;
        }

        /// <summary>
        /// Clears the current behavior.
        /// </summary>
        internal static void Clear() => Instance.Value = null;
    }
}
