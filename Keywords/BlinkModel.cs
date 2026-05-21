using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Keywords;

/// <summary>
/// Exhaust this card. At the end of the turn, return this card to your hand.
/// </summary>
public class BlinkModel() : CustomSingletonModel(true, false)
{
    /// <summary>
    /// Exhaust this card. At the end of the turn, return this card to your hand.
    /// </summary>
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Blink = 0;

    private static readonly HashSet<CardModel> BlinkCardsToRestore = [];

    /// <inheritdoc />
    public override Task BeforeCombatStart()
    {
        BlinkCardsToRestore.Clear();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(CardModel card,
        bool isAutoPlay,
        ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        if (!card.Keywords.Contains(Blink))
        {
            return (pileType, position);
        }

        BlinkCardsToRestore.Add(card);
        return (PileType.Exhaust, CardPilePosition.Top);

    }

    /// <inheritdoc/>
    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.PlayerCombatState is null)
        {
            return;
        }

        CardModel[] cards = [.. player.PlayerCombatState.AllCards.Where(c => BlinkCardsToRestore.Contains(c))];

        List<Task> tasks = [];

        foreach (CardModel c in cards)
        {
            BlinkCardsToRestore.Remove(c);
            if (c.Pile == player.PlayerCombatState.ExhaustPile)
            {
                tasks.Add(CardPileCmd.Add(c, PileType.Draw, CardPilePosition.Top));
            }
        }
        tasks.Add(Task.Delay(TimeSpan.FromSeconds(0.15)));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Exhaust target card, at the end of this turn, move it from your exhaust pile to the top of your draw pile.
    /// </summary>
    /// <param name="choiceContext">The current player choice context.</param>
    /// <param name="card">The card to blink.</param>
    public static async Task BlinkCardAsync(PlayerChoiceContext choiceContext, CardModel card)
    {
        await CardCmd.Exhaust(choiceContext, card);
        if (card.Keywords.Contains(Blink))
        {
            BlinkCardsToRestore.Add(card);
        }
        else
        {
            card.AddKeyword(BlinkedModel.Blinked);
        }

        await BetterHooks.OnBlinkAsync(choiceContext, card);
    }
}