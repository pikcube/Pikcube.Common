using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Extensions;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Keywords;

/// <summary>
/// Exhaust this card. At the end of the turn, return this card to your hand.
/// </summary>
public class BlinkModel() : CustomSingletonModel(HookType.Combat)
{
    /// <summary>
    /// Exhaust this card. At the end of the turn, return this card to your hand.
    /// </summary>
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Blink = 0;

    /// <inheritdoc />
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(CardModel card, bool isAutoPlay, ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        if (!card.Keywords.Contains(Blink))
        {
            return (pileType, position);
        }

        card.AddTempKeyword(BlinkedModel.Blinked);

        return (PileType.Exhaust, CardPilePosition.Top);
    }

    /// <inheritdoc />
    public override async Task AfterModifyingCardPlayResultPileOrPosition(CardModel card, PileType pileType, CardPilePosition position)
    {
        await BetterHooks.OnBlinkAsync(new BlockingPlayerChoiceContext(), card);
    }

    /// <summary>
    /// Exhaust target card, at the end of this turn, move it from your exhaust pile to the top of your draw pile.
    /// </summary>
    /// <param name="choiceContext">The current player choice context.</param>
    /// <param name="card">The card to blink.</param>
    public static async Task BlinkCardAsync(PlayerChoiceContext choiceContext, CardModel card)
    {
        await card.ExhaustAsync(choiceContext);
        card.AddTempKeyword(BlinkedModel.Blinked);
        await BetterHooks.OnBlinkAsync(choiceContext, card);
    }
}