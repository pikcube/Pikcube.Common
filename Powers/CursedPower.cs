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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using Pikcube.Common.Extensions;
using Pikcube.Common.Keywords;
using Pikcube.Common.Utility;
using Pikcube.Common.Vfx;

namespace Pikcube.Common.Powers;

/// <summary>
/// Custom Power that emulates the Cursed Debuff from Dicey Dungeons. Causes cards to have a 50% chance to be played 0 times. <br/>
/// Decrements by 1 when succesfully trigggered, and is removed at the end of the turn if any stacks remain. <br/>
/// Cards that aren't played are exhuasted for the turn, and are placed on top of the draw pile at the end of the round.
/// </summary>
[UsedImplicitly]
public class CursedPower : CustomPowerModel
{ 
    /// <inheritdoc />
    public override PowerType Type => PowerType.Debuff;

    /// <inheritdoc />
    public override PowerStackType StackType => PowerStackType.Counter;

    private List<CardModel> ValidCards { get; set; } = null!;
    private List<CardModel> CursedCards { get; set; } = null!;
    private List<CardModel> IgnoredCards { get; set; } = null!;
    private Player? OwningPlayer { get; set; }

    ///// <inheritdoc />
    //protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    //[
    //    BlinkTip
    //];

    //internal static readonly IHoverTip BlinkTip = HoverTipFactory.FromKeyword(EntranceModel.Entrance);

    /// <inheritdoc />
    protected override void AfterCloned()
    {
        base.AfterCloned();
        OwningPlayer = null;
        ValidCards = [];
        CursedCards = [];
        IgnoredCards = [];
    }

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

        ValidCards.Clear();

        ValidCards.AddRange(owningPlayerPlayerCombatState.DrawPile.Cards);
        ValidCards.AddRange(owningPlayerPlayerCombatState.Hand.Cards);
        ValidCards.AddRange(owningPlayerPlayerCombatState.DiscardPile.Cards);
        ValidCards.AddRange(owningPlayerPlayerCombatState.PlayPile.Cards);

        foreach (CardModel card in ValidCards)
        {
            card.AddTempKeyword(CursedModel.Cursed, this, true);
        }
    }

    /// <inheritdoc />
    public override Task BeforeCardAutoPlayed(CardModel card, Creature? target, AutoPlayType type)
    {
        if (card.Owner != OwningPlayer || type == AutoPlayType.SlyDiscard)
        {
            return Task.CompletedTask;
        }

        IgnoredCards.Add(card);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (OwningPlayer is null)
        {
            return playCount;
        }
        bool isIgnored = IgnoredCards.Remove(card);

        if (isIgnored || !ValidCards.Contains(card) || card.Keywords.All(c => c != CursedModel.Cursed) || card.Owner != OwningPlayer || card.IsDupe || OwningPlayer.RunState.Rng.CombatCardSelection.NextBool() is not true)
        {
            return playCount;
        }

        CursedCards.Add(card);

        return 0;
    }

    /// <inheritdoc />
    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        if (OwningPlayer is null || !CursedCards.Contains(card))
        {
            return;
        }

        CursedCards.Remove(card);
        Flash();

        if (OwningPlayer.NetId == RunManager.Instance.NetService.NetId)
        {
            if (OwningPlayer == card.Owner && NGame.Instance is not null)
            {
                AudioStream curseSound = GD.Load<AudioStream>("res://Pikcube.Common/curse.ogg");
                SettingsSave settings = SaveManager.Instance.SettingsSave;
                AudioStreamPlayer player = new()
                {
                    Stream = curseSound,
                    VolumeDb = 3f,
                    VolumeLinear = settings.VolumeSfx * settings.VolumeMaster
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

        CardPileAddResult moveCard = await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);

        await PowerCmd.Decrement(this);
    }

    /// <inheritdoc />
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
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
        TempKeywordManager.DestroyKeywordsEarly(this);
        if (OwningPlayer is null)
        {
            return Task.CompletedTask;
        }
        ValidCards.Clear();
        IgnoredCards.Clear();
        CursedCards.Clear();
        return Task.CompletedTask;
    }
}