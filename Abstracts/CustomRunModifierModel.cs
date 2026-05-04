using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Abstracts;

/// <summary>
/// Model for custom run modifiers with autoadd support
/// </summary>
public abstract class CustomRunModifierModel(CustomRunType runType, CustomRunModifierInfo info) : ModifierModel, ICustomModel
{
    internal CustomRunModifierInfo Info => info;

    internal CustomRunType RunType => runType;

    /// <inheritdoc />
    protected override string IconPath => GetImagePath("modifiers/" + Id.Entry.ToLowerInvariant() + ".png");

    /// <summary>
    /// Function to get the path to the image within your pck file.
    /// </summary>
    /// <param name="innerPath">The path relivive to your mod id directory</param>
    /// <returns>The resolved path into the pck</returns>
    public abstract string GetImagePath(string innerPath);
}