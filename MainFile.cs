using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace Pikcube.Common;

/// <summary>
/// Pikcube.Common Initializer
/// </summary>
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    /// <summary>
    /// The ModId
    /// </summary>
    public const string ModId = "Pikcube.Common"; //At the moment, this is used only for the Logger and harmony names.

    /// <summary>
    /// A logger instance for the library
    /// </summary>
    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    /// <summary>
    /// Called when this mod is initalized by the game
    /// </summary>
    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}