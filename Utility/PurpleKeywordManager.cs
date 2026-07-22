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
    /// <inheritdoc />
    public override Task BeforeCombatStart()
    {
        lock (RegisteredPurpleKeywords)
        {
            foreach (PurpleKeywordInstance pki in RegisteredPurpleKeywords)
            {
                pki.Card.KeywordsChanged -= pki.KeywordChanged;
            }
            RegisteredPurpleKeywords.Clear();
        }
        return Task.CompletedTask;
    }

    internal static List<PurpleKeywordInstance> RegisteredPurpleKeywords { get; } = [];


    internal static void Register<T>(T instance, CardKeyword keyword) where T : CardModel
    {
        lock (RegisteredPurpleKeywords)
        {
            instance.AddKeyword(keyword);
            PurpleKeywordInstance pki = new(instance, keyword);
            instance.KeywordsChanged += pki.KeywordChanged;
            RegisteredPurpleKeywords.Add(pki);
        }
    }

    internal static bool IsPurpleKeyword(CardKeyword keyword, CardModel cardModel)
    {
        return RegisteredPurpleKeywords.Any(purp => purp.Card == cardModel && purp.Keyword == keyword) || 
               TempKeywordManager.IsTempKeyword(keyword, cardModel);
    }
}

internal record PurpleKeywordInstance(CardModel Card, CardKeyword Keyword)
{
    public void KeywordChanged()
    {
        if (Card.Keywords.Contains(Keyword))
        {
            return;
        }

        PurpleKeywordManager.RegisteredPurpleKeywords.Remove(this);
        Card.KeywordsChanged -= KeywordChanged;
    }
}