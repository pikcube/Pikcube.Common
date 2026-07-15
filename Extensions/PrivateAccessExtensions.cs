using HarmonyLib;
using Pikcube.Common.Utility;

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
        public PrivatePropertyWrapper<T, TRet> PrivatePropertyWrapper<TRet>(string name)
        {
            return new PrivatePropertyWrapper<T, TRet>(instance, name);
        }

        /// <summary>
        /// Get an inaccessible field's value through Access Tools.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <typeparam name="TRet">The type of the property.</typeparam>
        /// <returns>The current property value.</returns>
        public PrivateFieldWrapper<T, TRet> PrivateFieldWrapper<TRet>(string name)
        {
            return new PrivateFieldWrapper<T, TRet>(instance, name);
        }
    }
}