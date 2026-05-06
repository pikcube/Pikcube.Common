using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Patches;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a model that is listening to modify a Card's Hover Tips
/// </summary>
public interface IModifyHoverTipsListener
{
    /// <summary>
    /// Invoked immediately after getting the hover tips for a card. Modify HoverTipEventArgs.NewHovetTips to change the return value.
    /// </summary>
    /// <param name="sender">The card being modified</param>
    /// <param name="e">The current Hover Tips</param>
    public void ModifyCardHoverTips(CardModel sender, HoverTipEventArgs e);
}