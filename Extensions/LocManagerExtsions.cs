using MegaCrit.Sts2.Core.Localization;

namespace Pikcube.Common.Extensions;

/// <summary>
/// Extensions on the LocManager class
/// </summary>
public static class LocManagerExtsions
{
    private static LocManager GetInvariant()
    {
        LocManager manager = new();
        manager.SetLanguage("eng");
        return manager;
    }

    private static readonly LocManager Invariant = GetInvariant();

    extension(LocManager locManager)
    {
        /// <summary>
        /// Get an english localication manager.
        /// </summary>
        public static LocManager Invariant => Invariant;
    }
}