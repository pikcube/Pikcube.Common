using BaseLib.Abstracts;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Code.Extensions;
using MethodInfo = System.Reflection.MethodInfo;

namespace Pikcube.Common.Code.Utility;

/// <summary>
/// Used to make Keywords appear Purple.
/// </summary>
[UsedImplicitly]
public class PurpleKeywordManager() : CustomSingletonModel(HookType.Combat)
{
    static PurpleKeywordManager()
    {
        BetterHooks.ModifyCardText += BetterHooks_ModifyCardText;
    }

    private static Dictionary<CardKeyword, (string, string)> PurpleValue { get; } = [];

    private static void BetterHooks_ModifyCardText(CardModel card, ref List<string> lines)
    {
        ISet<CardKeyword> purpleKeywords = GetGlobalPurpleKeywords(card);
        foreach (CardKeyword keyword in card.Keywords.Where(keyword => IsPurpleKeyword(keyword, card, purpleKeywords)))
        {
            if (PurpleValue.TryGetValue(keyword, out (string, string) cache))
            {
                lines.TryReplaceValue(cache.Item1, cache.Item2);
            }
            else
            {
                string original = (string?)GetTextMethod.Invoke(null, [keyword]) ?? string.Empty;
                string newValue = original.Replace("gold]", "purple]");
                PurpleValue.Add(keyword, (original, newValue));
                lines.TryReplaceValue(original, newValue);
            }
        }
    }

    private static MethodInfo GetTextMethod => AccessTools.DeclaredMethod("CardKeywordExtensions:GetCardText");

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

    internal static HashSet<PurpleKeywordInstance> RegisteredPurpleKeywords { get; } = [];


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

    private static bool IsPurpleKeyword(CardKeyword keyword, CardModel cardModel, ISet<CardKeyword> globalPurpleKeywords)
    {
        return globalPurpleKeywords.Contains(keyword) || 
               RegisteredPurpleKeywords.Any(purp => purp.Card == cardModel && purp.Keyword == keyword) || 
               TempKeywordManager.IsTempKeyword(keyword, cardModel);
    }

    internal static ISet<CardKeyword> GetGlobalPurpleKeywords(CardModel cardModel)
    {
        HashSet<CardKeyword> purpleKeywords = [];
        foreach (IPurpleKeywordModifier pkm in cardModel.CombatState?.IterateHookListeners().OfType<IPurpleKeywordModifier>() ?? [])
        {
            HashSet<CardKeyword> keywords = [];
            pkm.TryModifyKeywordsInCombat(cardModel, keywords);
            keywords.IntersectWith(pkm.PurpleKeywords);
            purpleKeywords.UnionWith(keywords);
        }

        return purpleKeywords;
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