using BaseLib.Abstracts;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

/// <summary>
/// Internal Manager for Temp Keywords
/// </summary>
[UsedImplicitly]
public class TempKeywordManager() : CustomSingletonModel(true, true)
{
    /// <inheritdoc />
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            Clear();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task AfterCombatEnd(CombatRoom room)
    {
        Clear();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove all keywords registered by source early
    /// </summary>
    /// <param name="source">The object passed during registration</param>
    public static void DestroyKeywordsEarly(object source)
    {
        foreach ((CardModel cardModel, CardKeyword cardKeyword, object? _) in CurrentTempKeywords.Where(trio => trio.Item3 is not null && trio.Item3 == source))
        {
            cardModel.RemoveKeyword(cardKeyword);
        }
        CurrentTempKeywords.Clear();
    }

    internal static void DestroyKeywordsEarly<T>(T instance, CardKeyword keyword) where T : CardModel
    {
        CurrentTempKeywords.RemoveAll(trio => trio.Item1 == instance && trio.Item2 == keyword);
    }


    private static List<(CardModel, CardKeyword, object?)> CurrentTempKeywords { get; } = [];

    static TempKeywordManager()
    {
        BetterHooks.AfterRunInitialized += BetterHooks_AfterRunInitialized;
        BetterHooks.AfterCardCloned += BetterHooks_AfterCardCloned;
    }

    private static void BetterHooks_AfterCardCloned(CardModel original, CardModel clone)
    {
        CurrentTempKeywords.AddRange(CurrentTempKeywords.Where(pair => pair.Item1 == original).Select(pair => (clone, pair.Item2, pair.Item3)));
    }

    private static void BetterHooks_AfterRunInitialized(RunState runState)
    {
        CurrentTempKeywords.Clear();
    }

    internal static void Register<T>(T instance, CardKeyword keyword, object? source) where T : CardModel
    {
        instance.AddKeyword(keyword);
        CurrentTempKeywords.Add((instance, keyword, source));
    }

    private static void Clear()
    {
        foreach ((CardModel cardModel, CardKeyword cardKeyword, object? _) in CurrentTempKeywords)
        {
            cardModel.RemoveKeyword(cardKeyword);
        }
        CurrentTempKeywords.Clear();
    }
}