namespace Pikcube.Common.Utility;

public static class ArrayExtensions
{
    extension<T>(IList<T> iList)
    {
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