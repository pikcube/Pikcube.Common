using System.Data;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines an extension block that adds additional methods to CardReward
/// </summary>
public static class CardRewardExtensions
{
    extension<T>(T instance) where T : CardReward
    {
        /// <summary>
        /// Provides accesss to the private CardCreationOptions property.
        /// </summary>
        /// <returns>The value in CardReward.Options</returns>
        /// <exception cref="NoNullAllowedException"></exception>
        public CardCreationOptions GetOptions()
        {
            MethodInfo? property = AccessTools.DeclaredPropertyGetter(typeof(T), "Options") ?? throw new NoNullAllowedException();
            return (CardCreationOptions?)property.Invoke(instance, []) ?? throw new NoNullAllowedException();
        }
    }
}