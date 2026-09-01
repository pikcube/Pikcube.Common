using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Pikcube.Common.Extensions;

/// <summary>
/// Extensions on the LocString class
/// </summary>
public static class LocStringExtensions
{
    extension(LocString instance)
    {
        /// <summary>
        /// Get the raw english localization for a string.
        /// </summary>
        /// <returns>The raw english string without formatting.</returns>
        public string GetInvariantRawText()
        {
            return LocManager.Invariant.GetTable(instance.LocTable).GetRawText(instance.LocEntryKey);
        }

        /// <summary>
        /// Add the <see cref="DynamicVarSet"/> to the <see cref="LocString"/>>
        /// </summary>
        /// <param name="dynamicVars">The <see cref="DynamicVarSet"/> to add.</param>
        /// <returns>The <see cref="LocString"/>> with the <see cref="DynamicVarSet"/> added.</returns>
        public LocString WithDynamicVars(DynamicVarSet dynamicVars)
        {
            foreach (DynamicVar d in dynamicVars.Values)
            {
                instance.Add(d);
            }

            return instance;
        }
    }
}