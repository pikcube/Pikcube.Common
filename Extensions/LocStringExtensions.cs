using MegaCrit.Sts2.Core.Localization;

namespace Pikcube.Common.Extensions;

/// <summary>
/// Extensions on the LocString class
/// </summary>
public static class LocStringExtensions
{
    extension(LocString locString)
    {
        /// <summary>
        /// Get the raw english localization for a string.
        /// </summary>
        /// <returns>The raw english string without formatting.</returns>
        public string GetInvariantRawText()
        {
            return LocManager.Invariant.GetTable(locString.LocTable).GetRawText(locString.LocEntryKey);
        }
    }
}