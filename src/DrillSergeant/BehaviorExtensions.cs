using System;

namespace DrillSergeant
{
    public static class BehaviorExtensions
    {
        /// <summary>
        /// Marks an object as being owned by the current behavior being tested.
        /// </summary>
        /// <typeparam name="T">The type of object to take ownership of.</typeparam>
        /// <param name="instance">The object instance.</param>
        /// <returns>The object passed in.</returns>
        public static T? OwnedByBehavior<T>(this T? instance) where T : IDisposable
        {
            BehaviorBuilder.Owns(instance);
            return instance;
        }
    }
}
