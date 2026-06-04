using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
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


    /// <inheritdoc />
    public override Task BeforeCombatStart()
    {
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
            card.AddPurpleKeyword(EntrancedModel.Entranced);
        }
        else
        {
            card.AddPurpleKeyword(EntrancedModel.Entranced);
        }

        await BetterHooks.OnEntranceAsync(choiceContext, card);
    }
}