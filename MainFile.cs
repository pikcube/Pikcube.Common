using System.Data;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using Pikcube.Common.Patches;
using Pikcube.Common.Utility;

namespace Pikcube.Common;

/// <summary>
/// Pikcube.Common Initializer
/// </summary>
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    internal static List<DarvOptionPatches.BetterDarvRelicSet> RelicSetCache { get; } = [];

    /// <summary>
    /// The ModId
    /// </summary>
    public const string ModId = "Pikcube.Common"; //At the moment, this is used only for the Logger and harmony names.

    /// <summary>
    /// A logger instance for the library
    /// </summary>
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    /// <summary>
    /// Called when this mod is initalized by the game
    /// </summary>
    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();

        BetterHooks.AfterOneTimeInitialization += BetterHooks_AfterOneTimeInitialization;
        ModHelper.SubscribeForCombatStateHooks(ModId, CombatStateHooks);
    }

    private static IEnumerable<AbstractModel> CombatStateHooks(CombatState combatState)
    {
        yield return ModelDb.GetModel<Keywords.BlinkModel>();
    }

    private static void BetterHooks_AfterOneTimeInitialization()
    {
        FieldInfo fieldInfo = AccessTools.Field(typeof(Darv), "_validRelicSets");
        object val = fieldInfo.GetValue(null) ?? throw new NoNullAllowedException();

        MethodInfo method = AccessTools.DeclaredMethod(val.GetType(), "GetEnumerator", []);
        object enumerator = method.Invoke(val, []) ?? throw new NoNullAllowedException();

        PropertyInfo current = AccessTools.DeclaredProperty(enumerator.GetType(), "Current");
        MethodInfo moveNext = AccessTools.DeclaredMethod(enumerator.GetType(), "MoveNext");
        while (moveNext.Invoke(enumerator, []) is true)
        {
            object curVal = current.GetValue(enumerator) ?? throw new NoNullAllowedException();
            
            FieldInfo filterInfo = AccessTools.DeclaredField(curVal.GetType(), "filter");
            FieldInfo relicInfo = AccessTools.DeclaredField(curVal.GetType(), "relics");

            Func<Player, bool> filter = (Func<Player, bool>?)filterInfo.GetValue(curVal) ?? throw new NoNullAllowedException();
            RelicModel[] relics = (RelicModel[]?)relicInfo.GetValue(curVal) ?? throw new NoNullAllowedException();

            RelicSetCache.Add(new DarvOptionPatches.BetterDarvRelicSet
            {
                Filter = filter,
                Relics = relics
            });
        }
    }

}