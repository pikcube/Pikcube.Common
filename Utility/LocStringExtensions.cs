using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Pikcube.Common.Utility;

public static class LocStringExtensions
{ 
    private static LocManager GetInvariant()
    {
        LocManager manager = new();
        manager.SetLanguage("eng");
        return manager;
    }

    private static readonly LocManager Invariant = GetInvariant();

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

    extension(LocManager locManager)
    {
        /// <summary>
        /// Get an english localication manager.
        /// </summary>
        public static LocManager Invariant => Invariant;
    }
}