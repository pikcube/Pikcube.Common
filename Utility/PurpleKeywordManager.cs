using BaseLib.Abstracts;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Extensions;
using MethodInfo = System.Reflection.MethodInfo;

namespace Pikcube.Common.Utility;

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

    private static void BetterHooks_ModifyCardText(CardModel card, ref List<string> lines)
    {
        foreach (CardKeyword keyword in card.Keywords.Where(keyword => IsPurpleKeyword(keyword, card)))
        {
            string original = (string?)GetTextMethod.Invoke(null, [keyword]) ?? string.Empty;
            string newValue = original.Replace("gold]", "purple]");
            lines.TryReplaceValue(original, newValue);
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