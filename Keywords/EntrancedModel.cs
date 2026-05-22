using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Keywords;

/// <summary>
/// Second half of the Entrance keyword
/// At the end of your turn, place this card on top of your discard pile.
/// </summary>
[UsedImplicitly]
public class EntrancedModel() : CustomSingletonModel(HookType.Combat)
{
    /// <summary>
    /// At the end of this turn, place this card on top of your draw pile.
    /// </summary>
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Entranced = 0;

    /// <inheritdoc />
    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.PlayerCombatState is null)
        {
            return;
        }

        List<CardModel> entrancedCards = [.. player.PlayerCombatState.AllCards.Where(c => c.Keywords.Contains(Entranced))];
        List<Task> tasks = [];
        foreach (CardModel card in entrancedCards)
        {
            card.RemoveKeyword(Entranced);
            if (card.Pile?.Type != PileType.Draw)
            {
                tasks.Add(CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top));
            }
        }
        await Task.WhenAll(tasks);
    }
}