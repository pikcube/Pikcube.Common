using MegaCrit.Sts2.Core.Entities.Merchant;

namespace Pikcube.Common.Utility;

/// <summary>
/// Interface defining a model that listens for when the Merchant generates potions
/// </summary>
public interface IModifyMerchantPotionBlacklist
{
    /// <summary>
    /// Method nvoked immediately before generating potions for the merchant
    /// </summary>
    /// <param name="entry">The entry we are modifying</param>
    /// <param name="e">The args containing the current blacklist</param>
    public void ModifyMerchantPotionBlacklist(MerchantPotionEntry entry, MerchantPotionBlacklistArgs e);
}