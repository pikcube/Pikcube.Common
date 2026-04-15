using System.Data;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

public static class CardRewardExtensions
{
    extension<T>(T instance) where T : CardReward
    {
        public CardCreationOptions GetOptions()
        {
            MethodInfo? property = AccessTools.DeclaredPropertyGetter(typeof(T), "Options") ?? throw new NoNullAllowedException();
            return (CardCreationOptions?)property.Invoke(instance, []) ?? throw new NoNullAllowedException();
        }
    }
}