using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a model that listens for when a power is applied.
/// </summary>
public interface IAfterPowerRemovedListener
{
    /// <summary>
    /// Invoked asyncrnously after a power is applied and has called its AfterApplied function.
    /// </summary>
    /// <param name="powerModel">The power being applied,</param>
    /// <param name="oldOwner">The original owner.</param>
    Task AfterPowerRemovedAsync(PowerModel powerModel, Creature? oldOwner);
}