namespace Pikcube.Common.Utility;

/// <summary>
/// Defines information about how to order custom run modifiers.
/// </summary>
public record CustomRunModifierInfo
{
    /// <summary>
    /// Defines information about how to order custom run modifiers.
    /// </summary>
    /// <param name="modId">Your mod id (likely in MainFile.ModId).</param>
    /// <param name="modifierName">The name of your modifier.</param>
    /// <param name="modifierPriority">Optional priority information to determine where your mod will be inserted into the list.</param>
    public CustomRunModifierInfo(string modId, string modifierName, ModifierPriority modifierPriority = ModifierPriority.PostfixGeneric)
    {
        ModId = modId;
        ModifierName = modifierName;
        Priority = modifierPriority;
    }
    /// <summary>
    /// The id of the mod this modifier came from.
    /// </summary>
    internal string ModId { get; }
    /// <summary>
    /// The name of the modifier to use when sorting.
    /// </summary>
    private string ModifierName { get; }
    /// <summary>
    /// Defines where in the modifier list to insert the modifier.
    /// </summary>
    public ModifierPriority Priority { get; }

    internal string Primary => Priority switch
    {
        ModifierPriority.PostfixGeneric => ModifierName,
        ModifierPriority.PostfixSegmented => ModId,
        ModifierPriority.PrefixGeneric => ModifierName,
        ModifierPriority.PrefixSegmented => ModId,
        ModifierPriority.Immediate => ModId,
        _ => throw new ArgumentOutOfRangeException()
    };

    internal string Secondary => Priority switch
    {
        ModifierPriority.PostfixGeneric => ModId,
        ModifierPriority.PostfixSegmented => ModifierName,
        ModifierPriority.PrefixGeneric => ModId,
        ModifierPriority.PrefixSegmented => ModifierName,
        ModifierPriority.Immediate => ModifierName,
        _ => throw new ArgumentOutOfRangeException()
    };
}

/// <summary>
/// Defines the priority of the modifier when populating the run list
/// </summary>
public enum ModifierPriority
{
    /// <summary>
    /// Modifier prepended before all other modifiers. If multiple modifiers declare an immediate priority, modifiers will be ordered by mod id, then by modifier name.
    /// </summary>
    Immediate = 0,
    /// <summary>
    /// Modifier prepended to the beginning of the list, ordered by mod id, then by modifier name.
    /// </summary>
    PrefixSegmented = 1,
    /// <summary>
    /// Modifier prepended to the beginning of the list after all prefix segments, ordered by modifier name, then by mod id.
    /// </summary>
    PrefixGeneric = 2,
    /// <summary>
    /// Modifier appended to the end of the list, ordered by mod id, then by modifier name.
    /// </summary>
    PostfixSegmented = 5,
    /// <summary>
    /// Modifier appended to the end of the list after all postfix segments, ordered by modifier name, then by mod id.
    /// </summary>
    PostfixGeneric = 6,
}