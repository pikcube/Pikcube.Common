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

[UsedImplicitly]
public class Cursed : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private List<CardModel> CursedCards { get; } = [];

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner != Owner.Player || card.IsDupe || Owner.Player.RunState.Rng.Shuffle.NextBool() is not true)
        {
            return playCount;
        }

        Flash();

        CursedCards.Add(card);

        return 0;
    }

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

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (!side.HasFlag(CombatSide.Player))
        {
            return;
        }

        await PowerCmd.Remove(this);
    }
}