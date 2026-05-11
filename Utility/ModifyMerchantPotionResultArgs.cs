using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines the args passed to the ModifyMerchantPotionResult event.
/// </summary>
/// <param name="potion">The original potion.</param>
public class ModifyMerchantPotionResultArgs(PotionModel? potion)
{
    /// <summary>
    /// The original unmodified potion.
    /// </summary>
    public PotionModel? Original { get; } = potion;
    /// <summary>
    /// The current potion to be presented.
    /// </summary>
    public PotionModel? NewPotion { get; set; } = potion;
}