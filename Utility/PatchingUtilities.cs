using HarmonyLib;
using System.Reflection;

namespace Pikcube.Common.Utility;

/// <summary>
/// A collection of functions to make patching easier.
/// </summary>
public static class PatchingUtilities
{
    /// <summary>
    /// Function to target all method overrides when patching.
    /// </summary>
    /// <param name="baseType">The base type with the abstract or virtual member declared.</param>
    /// <param name="methodName">The method name.</param>
    /// <returns>All overrides in addition to the base method if it is not abstract.</returns>
    public static IEnumerable<MethodBase> TargetMethodOverrides(Type baseType, string methodName)
    {
        MethodInfo? baseMethod = baseType.GetMethod(methodName);
        if (baseMethod is not null)
        {
            yield return baseMethod;
        }

        foreach (Type? type in AccessTools.AllTypes().Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract))
        {
            MethodInfo? method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method != null && method.DeclaringType != baseType)
            {
                yield return method;
            }
        }
    }

    /// <summary>
    /// Function to target all property getter overrides when patching.
    /// </summary>
    /// <param name="baseType">The base type with the abstract or virtual member declared.</param>
    /// <param name="methodName">The property name.</param>
    /// <returns>All overrides in addition to the base property if it is not abstract.</returns>
    public static IEnumerable<MethodBase> TargetGetterOverrides(Type baseType, string methodName)
    {
        MethodInfo? baseMethod = baseType.GetProperty(methodName)?.GetMethod;
        if (baseMethod is not null)
        {
            yield return baseMethod;
        }

        foreach (Type? type in AccessTools.AllTypes().Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract))
        {
            MethodInfo? method = type.GetProperty(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetMethod;

            if (method != null && method.DeclaringType != baseType)
            {
                yield return method;
            }
        }
    }

    /// <summary>
    /// Function to target all property setter overrides when patching.
    /// </summary>
    /// <param name="baseType">The base type with the abstract or virtual member declared.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>All overrides in addition to the base property if it is not abstract.</returns>
    public static IEnumerable<MethodBase> TargetSetterOverrides(Type baseType, string propertyName)
    {
        MethodInfo? baseMethod = baseType.GetProperty(propertyName)?.SetMethod;
        if (baseMethod is not null)
        {
            yield return baseMethod;
        }

        foreach (Type? type in AccessTools.AllTypes().Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract))
        {
            MethodInfo? method = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetMethod;

            if (method != null && method.DeclaringType != baseType)
            {
                yield return method;
            }
        }
    }
}