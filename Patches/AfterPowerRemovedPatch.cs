using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal class AfterPowerRemovedPatch
{
    [UsedImplicitly]
    static IEnumerable<MethodBase> TargetMethods()
    {
        MethodInfo? afterAppliedMethod = typeof(PowerModel).GetMethod(nameof(PowerModel.AfterRemoved));
        if (afterAppliedMethod is not null)
        {
            yield return afterAppliedMethod;
        }

        foreach (Type? type in AccessTools.AllTypes()
                     .Where(t => t.IsSubclassOf(typeof(PowerModel)) && !t.IsAbstract))
        {
            MethodInfo? method = type.GetMethod(nameof(PowerModel.AfterRemoved),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method != null && method.DeclaringType != typeof(PowerModel))
            {
                yield return method;
            }
        }
    }

    [UsedImplicitly]
    static Task Postfix(Task __result, PowerModel __instance, Creature? oldOwner)
    {
        return BetterHooks.OnPowerRemvoedAsync(__result, __instance, oldOwner);
    }
}