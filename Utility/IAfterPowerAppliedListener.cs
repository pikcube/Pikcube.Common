using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a model that listens for when a power is removed.
/// </summary>
public interface IAfterPowerAppliedListener
{
    /// <summary>
    /// Invoked asyncrnously after a power is removed and has called its AfterRemoved function.
    /// </summary>
    /// <param name="powerModel">The power being applied.</param>
    /// <param name="cardSource">The card source.</param>
    /// <param name="applier">The creature that applied the power.</param>

    Task AfterPowerAppliedAsync(PowerModel powerModel, CardModel? cardSource, Creature? applier);
}