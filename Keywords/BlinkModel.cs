using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Pikcube.Common.Extensions;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Keywords;

/// <summary>
/// Exhaust this card. At the end of the turn, return this card to your hand.
/// </summary>
public class BlinkModel() : CustomSingletonModel(HookType.Combat)
{
    private static List<CardModel> ShouldBlinkList { get; } = [];

    /// <inheritdoc />
    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        ShouldBlinkList.Clear();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Exhaust this card. At the end of the turn, return this card to your hand.
    /// </summary>
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Blink = 0;

    /// <inheritdoc />
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(CardModel card,
        bool isAutoPlay,
        ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        if (card.Keywords.Contains(Blink) || card.ShouldBlinkOnNextPlay)
        {
            return (PileType.Exhaust, CardPilePosition.Bottom);
        }


        return (pileType, position);
    }


    /// <inheritdoc />
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Card.Keywords.Contains(Blink) && !cardPlay.Card.ShouldBlinkOnNextPlay)
        {
            return;
        }

        cardPlay.Card.AddPurpleKeyword(BlinkedModel.Blinked);

        await BetterHooks.OnBlinkAsync(choiceContext, cardPlay.Card);
        cardPlay.Card.ShouldBlinkOnNextPlay = false;
    }


    /// <summary>
    /// Exhaust target card, at the end of this turn, move it from your exhaust pile to the top of your draw pile.
    /// </summary>
    /// <param name="choiceContext">The current player choice context.</param>
    /// <param name="card">The card to blink.</param>
    /// <param name="skipVisuals">True if the visuals should be skipped.</param>
    public static async Task BlinkCardAsync(PlayerChoiceContext choiceContext, CardModel card, bool skipVisuals = false)
    {
        await card.ExhaustAsync(choiceContext, false, skipVisuals);
        card.AddPurpleKeyword(BlinkedModel.Blinked);
        await BetterHooks.OnBlinkAsync(choiceContext, card);
    }

    /// <summary>
    /// Exhaust target card, at the end of this turn, move it from your exhaust pile to the top of your draw pile.
    /// </summary>
    /// <param name="choiceContext">The current player choice context.</param>
    /// <param name="cards">The cards to blink.</param>
    /// <param name="skipVisuals">True if the visuals should be skipped.</param>
    public static async Task BlinkCardsAsync(PlayerChoiceContext choiceContext, IEnumerable<CardModel> cards, bool skipVisuals = false)
    {
        CardModel[] c = [..cards];
        foreach (CardModel card in c)
        {
            await card.ExhaustAsync(choiceContext, skipVisuals: skipVisuals);
            card.AddPurpleKeyword(BlinkedModel.Blinked);
        }
        foreach (CardModel card in c)
        {
            await BetterHooks.OnBlinkAsync(choiceContext, card);
        }
    }

    internal static bool ShouldBlink<T>(T instance) where T : CardModel
    {
        return !instance.ExhaustOnNextPlay && ShouldBlinkList.Any(c => c == instance);
    }

    internal static void SetShouldBlink<T>(T instance, bool value) where T : CardModel
    {
        if (value)
        {
            ShouldBlinkList.Add(instance);
        }
        else
        {
            ShouldBlinkList.Remove(instance);
        }
    }
}