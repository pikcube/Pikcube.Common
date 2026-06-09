using HarmonyLib;

namespace Pikcube.Common.Extensions;

/// <summary>
/// Extensions to make Access Tools faster. Throws an exception if any type is wrong, which is intentional.
/// </summary>
public static class PrivateAccessExtensions
{
    extension<T>(T instance)
    {
        /// <summary>
        /// Get an inaccessible property's value through Access Tools.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <typeparam name="TRet">The type of the property.</typeparam>
        /// <returns>The current property value.</returns>
        public TRet? GetPrivateProperty<TRet>(string name)
        {
            return (TRet?)AccessTools.DeclaredProperty(typeof(T), name).GetValue(instance);
        }
    }
}