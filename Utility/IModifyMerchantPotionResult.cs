using MegaCrit.Sts2.Core.Entities.Merchant;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a model that listens for when a merchant shop entry is filled with a potion.
/// </summary>
public interface IModifyMerchantPotionResult
{
    /// <summary>
    /// Method called immediately after the merchant shop entry is filled with a potion.
    /// </summary>
    /// <param name="entry">The shop entry being modified.</param>
    /// <param name="args">The ModifyMerchantPotionResultArgs</param>
    public void ModifyMerchantPotionResult(MerchantPotionEntry entry, ModifyMerchantPotionResultArgs args);
}