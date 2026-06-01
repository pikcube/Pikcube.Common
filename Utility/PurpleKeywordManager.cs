using BaseLib.Abstracts;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Used to make Keywords appear Purple.
/// </summary>
[UsedImplicitly]
public class PurpleKeywordManager() : CustomSingletonModel(HookType.Combat)
{
    private static List<(CardModel, CardKeyword)> RegisteredPurpleKeywords { get; } = [];


    internal static void Register<T>(T instance, CardKeyword keyword) where T : CardModel
    {
        instance.AddKeyword(keyword);
        RegisteredPurpleKeywords.Add((instance, keyword));
        instance.KeywordsChanged += InstanceOnKeywordsChanged;
        return;

        void InstanceOnKeywordsChanged()
        {
            if (instance.Keywords.Contains(keyword))
            {
                return;
            }

            RegisteredPurpleKeywords.Remove((instance, keyword));
            instance.KeywordsChanged -= InstanceOnKeywordsChanged;
        }
    }

    internal static bool IsPurpleKeyword(CardKeyword keyword, CardModel cardModel)
    {
        return RegisteredPurpleKeywords.Any(purp => purp.Item1 == cardModel && purp.Item2 == keyword) || TempKeywordManager.IsTempKeyword(keyword, cardModel);
    }

    /// <inheritdoc />
    public override Task BeforeCombatStart()
    {
        RegisteredPurpleKeywords.Clear();
        return Task.CompletedTask;
    }
}