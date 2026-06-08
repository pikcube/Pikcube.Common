using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;
using System.Reflection;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal class AfterPowerAppliedPatch
{
    [UsedImplicitly]
    static IEnumerable<MethodBase> TargetMethods()
    {
        MethodInfo? afterAppliedMethod = typeof(PowerModel).GetMethod(nameof(PowerModel.AfterApplied));
        if (afterAppliedMethod is not null)
        {
            yield return afterAppliedMethod;
        }

        foreach (Type? type in AccessTools.AllTypes()
                     .Where(t => t.IsSubclassOf(typeof(PowerModel)) && !t.IsAbstract))
        {
            MethodInfo? method = type.GetMethod(nameof(PowerModel.AfterApplied),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method != null && method.DeclaringType != typeof(PowerModel))
            {
                yield return method;
            }
        }
    }

    [UsedImplicitly]
    static Task Postfix(Task __result, PowerModel __instance, Creature? applier, CardModel? cardSource)
    {
        return BetterHooks.OnPowerAppliedAsync(__result, __instance, applier, cardSource);
    }
}