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
public class TempKeywordManager() : CustomSingletonModel(HookType.Combat)
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
        (CardModel, CardKeyword, object?, bool)[] toRemove = [.. CurrentTempKeywords.Where(trio => trio.Item3 is not null && trio.Item3 == source)];

        foreach ((CardModel, CardKeyword, object?, bool) trio in toRemove)
        {
            trio.Item1.RemoveKeyword(trio.Item2);
            CurrentTempKeywords.Remove(trio);
        }
    }

    internal static void DestroyKeywordsEarly<T>(T instance, CardKeyword keyword) where T : CardModel
    {
        CurrentTempKeywords.RemoveAll(trio => trio.Item1 == instance && trio.Item2 == keyword);
    }


    private static List<(CardModel, CardKeyword, object?, bool)> CurrentTempKeywords { get; } = [];

    static TempKeywordManager()
    {
        BetterHooks.AfterRunInitialized += BetterHooks_AfterRunInitialized;
        BetterHooks.AfterCardCloned += BetterHooks_AfterCardCloned;
    }

    private static void BetterHooks_AfterCardCloned(CardModel original, CardModel clone)
    {
        (CardModel clone, CardKeyword, object?, bool)[] toAdd = [.. CurrentTempKeywords
            .Where(pair => pair.Item1 == original)
            .Select(pair => (clone, pair.Item2, pair.Item3, pair.Item4))];

        CurrentTempKeywords.AddRange(toAdd);
    }

    private static void BetterHooks_AfterRunInitialized(RunState runState)
    {
        CurrentTempKeywords.Clear();
    }

    internal static void Register<T>(T instance, CardKeyword keyword, object? source, bool isManualDestoryRequired) where T : CardModel
    {
        CurrentTempKeywords.Add((instance, keyword, source, isManualDestoryRequired));
        instance.AddKeyword(keyword);
    }

    private static void Clear()
    {
        foreach ((CardModel cardModel, CardKeyword cardKeyword, object? _, bool _) in CurrentTempKeywords.Where(set => !set.Item4))
        {
            cardModel.RemoveKeyword(cardKeyword);
        }
        CurrentTempKeywords.RemoveAll(set => !set.Item4);
    }

    internal static bool IsTempKeyword(CardKeyword keyword, CardModel cardModel)
    {
        return CurrentTempKeywords.Any(tuple => tuple.Item1 == cardModel && tuple.Item2 == keyword);
    }
}