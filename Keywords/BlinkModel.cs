using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Keywords;

/// <summary>
/// At the start of each turn, return this card to your hand.
/// </summary>
[UsedImplicitly]
public class BlinkModel() : CustomSingletonModel(true, false)
{
    /// <summary>
    /// At the start of each turn, return this card to your hand.
    /// </summary>
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Blink = 0;

    /// <inheritdoc />
    public override bool ShouldReceiveCombatHooks => true;

    /// <inheritdoc />
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player.PlayerCombatState is null)
        {
            return;
        }

        List<CardModel> blinkCards = [.. player.PlayerCombatState.AllCards.Where(c => c.Keywords.Contains(Blink))];
        foreach (CardModel card in blinkCards)
        {
            card.RemoveKeyword(Blink);
            if (card.Pile?.Type != PileType.Hand)
            {
                CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Hand), 0.2F);
            }
        }
    }
}