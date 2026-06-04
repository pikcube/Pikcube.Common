using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Keywords;

/// <summary>
/// Marks cards that have a 50% chance of being Entranced instead of being played.
/// </summary>
[UsedImplicitly]
public class CursedModel() : CustomSingletonModel(HookType.Combat)
{
    /// <summary>
    /// Card has a 50% chance to be exhausted for the turn instead of played.
    /// </summary>
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Cursed = 0;
}