namespace Pikcube.Common.Utility;

/// <summary>
/// Defines an extension block for extension methods on the IList interface
/// </summary>
public static class ArrayExtensions
{
    extension<T>(IList<T> iList)
    {
        /// <summary>
        /// Replaces the first occurrence of an item in a List with a new item (if possible).
        /// </summary>
        /// <param name="original">The item in the list to replace.</param>
        /// <param name="newValue">The replacement for the original item.</param>
        /// <returns>The index of the item on success, or -1 on failure.</returns>
        public int TryReplaceValue(T original, T newValue)
        {
            int index = iList.IndexOf(original);
            if (index != -1)
            {
                iList[index] = newValue;
            }

            return index;
        }
    }
}