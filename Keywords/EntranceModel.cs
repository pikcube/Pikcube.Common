using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Extensions;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Keywords;

/// <summary>
/// Exhaust target card, at the end of this turn, place it on top of your draw pile.
/// </summary>
[UsedImplicitly]
public class EntranceModel() : CustomSingletonModel(HookType.Combat)
{
    /// <summary>
    /// Exhaust target card, at the end of this turn, place it on top of your draw pile.
    /// </summary>
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Entrance = 0;

    private static readonly HashSet<CardModel> EntranceCardsToRestore = [];

    /// <inheritdoc />
    public override Task BeforeCombatStart()
    {
        EntranceCardsToRestore.Clear();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Exhaust target card, at the end of this turn, move it from your exhaust pile to the top of your draw pile.
    /// </summary>
    /// <param name="choiceContext">The current player choice context.</param>
    /// <param name="card">The card to entrance.</param>
    public static async Task EntranceCardAsync(PlayerChoiceContext choiceContext, CardModel card)
    {
        await card.ExhaustAsync(choiceContext);

        if (card.Keywords.Contains(Entrance))
        {
            EntranceCardsToRestore.Add(card);
        }
        else
        {
            card.AddKeyword(EntrancedModel.Entranced);
        }

        await BetterHooks.OnEntranceAsync(choiceContext, card);
    }

    /// <inheritdoc/>
    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.PlayerCombatState is null)
        {
            return;
        }

        CardModel[] cards = [.. player.PlayerCombatState.AllCards.Where(c => EntranceCardsToRestore.Contains(c))];

        List<Task> tasks = [];
        foreach(CardModel c in cards)
        {
            EntranceCardsToRestore.Remove(c);
            if (c.Pile == player.PlayerCombatState.ExhaustPile)
            {
                tasks.Add(CardPileCmd.Add(c, PileType.Draw, CardPilePosition.Top));
            }
        }
        await Task.WhenAll(tasks);

    }
}