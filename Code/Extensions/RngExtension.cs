using MegaCrit.Sts2.Core.Random;

namespace Pikcube.Common.Code.Extensions;

/// <summary>
/// Extensions on RNG
/// </summary>
public static class RngExtension
{
    extension(Rng instance)
    {
        /// <summary>
        /// Shuffle a list using an rng.
        /// </summary>
        /// <param name="items">The items to shuffle in place.</param>
        public void Shuffle<T>(List<T> items)
        {
            for (int n = items.Count - 1; n >= 0; --n)
            {
                int swapIndex = instance.NextInt(0, n + 1);
                T itemA = items[swapIndex];
                T itemB = items[n];
                items[n] = itemA;
                items[swapIndex] = itemB;
            }
        }
    }
}