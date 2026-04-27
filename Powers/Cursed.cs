using BaseLib.Abstracts;
using Godot;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Vfx;

namespace Pikcube.Common.Powers;

/// <summary>
/// Custom Power that emulates the Cursed Debuff from Dicey Dungeons. Causes cards to have a 50% chance to be played 0 times. <br/>
/// Decrements by 1 when succesfully trigggered, and is removed at the end of the turn if any stacks remain. <br/>
/// Cards that aren't played are always sent to the discard pile and still expend their energy cost.
/// </summary>
[UsedImplicitly]
public class Cursed : CustomPowerModel
{
    /// <inheritdoc />
    public override PowerType Type => PowerType.Debuff;

    /// <inheritdoc />
    public override PowerStackType StackType => PowerStackType.Counter;
    private List<CardModel> CursedCards { get; } = [];

    /// <inheritdoc />
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner != Owner.Player || card.IsDupe || Owner.Player.RunState.Rng.CombatTargets.NextBool() is not true)
        {
            return playCount;
        }

        Flash();

        CursedCards.Add(card);

        return 0;
    }

    /// <inheritdoc />
    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        if (Owner.Player is null || !CursedCards.Contains(card))
        {
            return;
        }

        CursedCards.Remove(card);

        if (Owner.Player.NetId == RunManager.Instance.NetService.NetId)
        {
            if (Owner.Player == card.Owner && NGame.Instance is not null)
            {
                AudioStream curseSound = GD.Load<AudioStream>("res://Pikcube.Common/curse.ogg");
                AudioStreamPlayer player = new()
                {
                    Stream = curseSound,
                    VolumeDb = 3f
                };
                NGame.Instance.AddChild(player);
                player.Play();
                player.Finished += player.QueueFree;
            }

            NCard? findOnTable = NCard.FindOnTable(card) ?? NCard.Create(card);

            if (findOnTable is not null && NGame.Instance?.CurrentRunNode is not null && NCombatRoom.Instance is not null)
            {
                NGame.Instance.CurrentRunNode.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(new Color(0.3f, 0.3f, 0.3f, 0.66f), new Color(1.0f, 1.0f, 1f, 0.33f)));
                NCombatRoom.Instance.Ui.AddChildSafely(SilentExhaustVfx.Create(findOnTable));
            }
        }

        if (card.Pile?.Type is PileType.Play)
        {
            await CardPileCmd.Add(card, PileType.Discard);
        }

        await PowerCmd.Decrement(this);
    }

    /// <inheritdoc />
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (!side.HasFlag(CombatSide.Player))
        {
            return;
        }

        await PowerCmd.Remove(this);
    }
}