using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Rewards;

public class CardBundleReward : Reward
{
    private List<List<CardCreationResult>> Bundles { get; } = [];
    private CardCreationOptions Options { get; init; }
    private int MinimumBundleSize { get; init; }
    private int MinimumBundleCount { get; init; }
    private IEnumerable<IEnumerable<CardModel>> InitialCardsToOffer { get; init; }
    protected override RewardType RewardType => RewardType.Card;
    public override int RewardsSetIndex => 8;
    public override LocString Description => new LocString("powers", "PIKCUBE-CARDBUNDLEDESCRIPTION");

    private bool _isPopulated = false;
    public override bool IsPopulated => _isPopulated;

    public CardBundleReward(CardCreationOptions options, Player player, int bundleCount = 2, int bundleSize = 3) :
        this(options, null, options.Source, player, bundleCount, bundleSize)
    {
    }

    public CardBundleReward(IEnumerable<IEnumerable<CardModel>> cardsToOffer, CardCreationSource source, Player player)  :
        this(null, cardsToOffer, source, player, 0, 0)
    {
    }

    public CardBundleReward(CardReward cardReward, int bundleCount = 2, int bundleSize = 3) : 
        this(cardReward.GetOptions(), [cardReward.Cards], cardReward.GetOptions().Source, cardReward.Player, bundleCount, bundleSize)
    {
    }

    public CardBundleReward(CardCreationOptions? options, IEnumerable<IEnumerable<CardModel>>? cardsToOffer, CardCreationSource source,
        Player player, int bundleCount, int bundleSize) : base(player)
    {
        if (options is null && cardsToOffer is null)
        {
            throw new ArgumentException($"{nameof(options)} and {nameof(cardsToOffer)} can't both be null");
        }

        Options = options ?? new CardCreationOptions(cardsToOffer?.SelectMany(c=>c) ?? [], source, CardRarityOddsType.Uniform)
                      .WithFlags(CardCreationFlags.NoCardPoolModifications | CardCreationFlags.NoCardModelModifications);
        InitialCardsToOffer = cardsToOffer ?? [];
        MinimumBundleCount = bundleCount;
        MinimumBundleSize = bundleSize;
    }


    public override async Task Populate()
    {
        if (IsPopulated)
        {
            return;
        }

        _isPopulated = true;

        foreach (IEnumerable<CardModel> initialBundle in InitialCardsToOffer)
        {
            List<CardCreationResult> bundle = [..initialBundle.Select(c => new CardCreationResult(c))];
            if (Hook.TryModifyCardRewardOptions(Player.RunState, Player, bundle, Options, out List<AbstractModel> modifiers))
                await TaskHelper.RunSafely(Hook.AfterModifyingCardRewardOptions(Player.RunState, modifiers));
            if (bundle.Count < MinimumBundleSize)
            {
                int toAdd = MinimumBundleSize - bundle.Count;
                bundle.AddRange(CardFactory.CreateForReward(Player, toAdd, Options));

            }
            Bundles.Add(bundle);
        }

        while (Bundles.Count < MinimumBundleCount)
        {
            Bundles.Add([..CardFactory.CreateForReward(Player, MinimumBundleSize, Options)]);
        }
    }

    protected override async Task<bool> OnSelect()
    {
        throw new NotImplementedException();
    }

    public override void MarkContentAsSeen()
    {
    }
}