using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Keywords;

/// <summary>
/// At the end of the turn, return this card to your hand.
/// </summary>
public class BlinkedModel() : CustomSingletonModel(true, false)
{
    /// <summary>
    /// At the end of the turn, return this card to your hand.
    /// </summary>
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Blinked = 0;

    /// <inheritdoc />
    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.PlayerCombatState is null)
        {
            return;
        }

        List<CardModel> blinkCards = [.. player.PlayerCombatState.AllCards.Where(c => c.Keywords.Contains(Blinked))];
        foreach (CardModel card in blinkCards)
        {
            card.RemoveKeyword(Blinked);
            if (card.Pile?.Type != PileType.Hand)
            {
                await CardPileCmd.Add(card, PileType.Hand);
            }
        }
    }
}