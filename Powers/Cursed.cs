using BaseLib.Abstracts;
using Godot;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Keywords;
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

    private Dictionary<Player, List<CardModel>> ValidCards { get; } = [];
    private Dictionary<Player, List<CardModel>> CursedCards { get; } = [];
    private Dictionary<Player, List<CardModel>> IgnoredCards { get; } = [];
    private Player? OwningPlayer { get; set; }

    /// <inheritdoc />
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(new LocString(locTable, "PIKCUBE-CURSED.blinkTitle"),
            new LocString(locTable, "PIKCUBE-CURSED.blinkDescription"))
    ];

    /// <inheritdoc />
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.Player is null)
        {
            await PowerCmd.Remove(this);
            return;
        }

        OwningPlayer = Owner.Player;

        PlayerCombatState? owningPlayerPlayerCombatState = OwningPlayer.PlayerCombatState;
        if (owningPlayerPlayerCombatState is null)
        {
            await PowerCmd.Remove(this);
            return;
        }

        ValidCards.TryAdd(Owner.Player, []);
        CursedCards.TryAdd(Owner.Player, []);
        IgnoredCards.TryAdd(Owner.Player, []);

        ValidCards[Owner.Player].AddRange(owningPlayerPlayerCombatState.DrawPile.Cards);
        ValidCards[Owner.Player].AddRange(owningPlayerPlayerCombatState.Hand.Cards);
        ValidCards[Owner.Player].AddRange(owningPlayerPlayerCombatState.DiscardPile.Cards);
        ValidCards[Owner.Player].AddRange(owningPlayerPlayerCombatState.PlayPile.Cards);
    }

    /// <inheritdoc />
    public override Task BeforeCardAutoPlayed(CardModel card, Creature? target, AutoPlayType type)
    {
        if (card.Owner != Owner.Player || type == AutoPlayType.SlyDiscard)
        {
            return Task.CompletedTask;
        }

        IgnoredCards[Owner.Player].Add(card);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        bool isIgnored = IgnoredCards[Owner.Player!].Remove(card);

        if (isIgnored || !ValidCards[Owner.Player!].Contains(card) || card.Owner != Owner.Player || card.IsDupe || card.Affliction is not null || Owner.Player.RunState.Rng.CombatCardSelection.NextBool() is not true)
        {
            return playCount;
        }

        CursedCards[Owner.Player].Add(card);

        return 0;
    }

    /// <inheritdoc />
    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        if (Owner.Player is null || !CursedCards[Owner.Player].Contains(card))
        {
            return;
        }

        CursedCards[Owner.Player].Remove(card);
        Flash();

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

        CardCmd.ApplyKeyword(card, BlinkModel.Blink);

        if (card.Pile?.Type is PileType.Play)
        {
            CardPileAddResult result = await CardPileCmd.Add(card, PileType.Exhaust);
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

    /// <inheritdoc />
    public override Task AfterRemoved(Creature oldOwner)
    {
        ValidCards[oldOwner.Player!].Clear();
        IgnoredCards[oldOwner.Player!].Clear();
        CursedCards[oldOwner.Player!].Clear();
        return Task.CompletedTask;
    }
}