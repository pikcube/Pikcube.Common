using System.Reflection;
using HarmonyLib;

namespace Pikcube.Common.Utility;

/// <summary>
/// A simple to use wrapper for accessing private fields.
/// </summary>
/// <param name="parent">The object instance (if the field isn't static).</param>
/// <param name="name">The field's name.</param>
/// /// <typeparam name="TParent">The type of the class where the field is defined.</typeparam>
/// <typeparam name="T">The type of the field.</typeparam>

public readonly struct PrivateFieldWrapper<TParent, T>(TParent? parent, string name)
{
    private readonly FieldInfo _fieldInfo = AccessTools.DeclaredField(typeof(TParent), name);

    /// <summary>
    /// The field's value.
    /// </summary>
    public T? Value
    {
        get => (T?)_fieldInfo.GetValue(parent);
        set => _fieldInfo.SetValue(parent, value);
    }
}

/// <summary>
/// A simple to use wrapper for accessing private property.
/// </summary>
/// <param name="parent">The object instance (if the property isn't static).</param>
/// <param name="name">The property's name.</param>
/// /// <typeparam name="TParent">The type of the class where the property is defined.</typeparam>
/// <typeparam name="T">The type of the property.</typeparam>

public readonly struct PrivatePropertyWrapper<TParent, T>(TParent? parent, string name)
{
    private readonly PropertyInfo _fieldInfo = AccessTools.DeclaredProperty(typeof(TParent), name);

    /// <summary>
    /// The property's value.
    /// </summary>
    public T? Value
    {
        get => (T?)_fieldInfo.GetValue(parent);
        set => _fieldInfo.SetValue(parent, value);
    }
}