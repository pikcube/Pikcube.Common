using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
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
    public override LocString Description => new("powers", "PIKCUBE-CARDBUNDLEDESCRIPTION");

    private bool _isPopulated;
    public override bool IsPopulated => _isPopulated;

    public CardBundleReward(CardCreationOptions options, Player player, int bundleCount = 2, int bundleSize = 3) :
        this(options, null, options.Source, player, bundleCount, bundleSize)
    {
    }

    public CardBundleReward(IEnumerable<IEnumerable<CardModel>> cardsToOffer, CardCreationSource source, Player player)  :
        this(null, cardsToOffer, source, player, 0, 0)
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
            while (bundle.Count < MinimumBundleSize)
            {
                bundle.AddRange(CardFactory.CreateForReward(Player, 1, Options));
            }
            Bundles.Add(bundle);
        }

        while (Bundles.Count < MinimumBundleCount)
        {
            List<CardCreationResult> bundle = [];
            while (bundle.Count < MinimumBundleSize)
            {
                bundle.AddRange(CardFactory.CreateForReward(Player, 1, Options));
            }
            Bundles.Add(bundle);
        }
    }

    protected override async Task<bool> OnSelect()
    {
        await Populate();
        List<List<CardModel>> bundleCards = [[ModelDb.Card<Skip>().CreateNewInstance(Player), ModelDb.Card<Skip>().CreateNewInstance(Player), ModelDb.Card<Skip>().CreateNewInstance(Player)], .. Bundles
            .Select(bundle => bundle
                .Select(cardResult => cardResult.Card.CreateNewInstance(Player))
                .ToList())];

        bool bundleSelected = false;

        foreach (CardModel card in await FromChooseABundleScreen(Player, bundleCards, Generate()))
        {
            bundleSelected = true; 
            await CardPileCmd.Add(card, PileType.Deck);
        }

        return bundleSelected;
    }

    public IReadOnlyList<CardRewardAlternative> Generate()
    {
        List<CardRewardAlternative> alternatives = new List<CardRewardAlternative>();
        alternatives.Add(new CardRewardAlternative("Skip", PostAlternateCardRewardAction.DismissScreenAndKeepReward));
        return alternatives.Count <= 2 ? (IReadOnlyList<CardRewardAlternative>)alternatives : throw new InvalidOperationException("More than 2 card reward alternatives are not supported.");
    }

    public override void MarkContentAsSeen()
    {
    }

    public static async Task<IEnumerable<CardModel>> FromChooseABundleScreen(
        Player player,
        IReadOnlyList<IReadOnlyList<CardModel>> bundles,
        IReadOnlyList<CardRewardAlternative> extraOptions)
    {
        if (CombatManager.Instance.IsEnding || bundles.Count == 0)
        {
            return [];
        }
        uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
        IReadOnlyList<CardModel> cards;
        if (TestMode.IsOn)
        {
            cards = bundles[0];
        }
        else if (LocalContext.IsMe(player) && RunManager.Instance.NetService.Type != NetGameType.Replay)
        {
            cards = (await NChooseABundleSelectionScreen.ShowScreen(bundles).CardsSelected()).FirstOrDefault() ?? [];
            if (cards.Any(c => c is Skip))
            {
                cards = [];
            }
            RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId, PlayerChoiceResult.FromIndex(bundles.IndexOf(cards)));
        }
        else
        {
            int index = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId)).AsIndex();
            cards = index < 0 ? [] : bundles[index];
        }
        string str = string.Join(",", cards.Select((Func<CardModel, string>)(c => c.Id.Entry)));
        Log.Info($"Player {player.NetId} chose cards [{str}]");
        return cards;
    }
}

[Pool(typeof(TokenCardPool))]
public class Skip : CustomCardModel
{
    public Skip() : base(-1, CardType.Status, CardRarity.Token, TargetType.None, false, true)
    {
    }
}