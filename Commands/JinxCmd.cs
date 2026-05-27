using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Extensions;
using Pikcube.Common.Powers;

namespace Pikcube.Common.Commands;

/// <summary>
/// Command for apply a Jinx to a creature that will execute on a later turn.
/// </summary>
public static class JinxCmd
{
    /// <summary>
    /// Power that executes a function after a set number of turns.
    /// </summary>
    /// <param name="choiceContext">The current PlayerChoiceContext</param>
    /// <param name="target">The creature to apply the Jinx to</param>
    /// <param name="turnDelay">The number of turns to wait before executing the action. A value of 1 will execute the function at the start of next turn.</param>
    /// <param name="isDebuff">Determines whether this power should be blocked by Artifact.</param>
    /// <param name="description">An explanation of what will happen when the countdown reaches 0.</param>
    /// <param name="function">The function to execute after the delay.</param>
    /// <param name="applier">Optionally pass the applier.</param>
    /// <param name="cardSource">Optionally pass the card source.</param>
    /// <param name="isSilent">Optionally pass true to apply this power silently.</param>
    public static async Task JinxAsync(PlayerChoiceContext choiceContext, Creature target, int turnDelay, bool isDebuff, LocString description, Func<PlayerChoiceContext, Creature, Task> function, Creature? applier, CardModel? cardSource, bool isSilent = false)
    {
        if (turnDelay < 1)
        {
            turnDelay = 1;
        }

        JinxPower jinxPower = ModelDb.Power<JinxPower>().StrongMutableClone();
        jinxPower.PrepareForApplication(isDebuff, function, description);
        await PowerCmd.Apply(choiceContext, jinxPower, target, turnDelay, applier, cardSource, isSilent);
    }

    /// <summary>
    /// Power that executes a function after a set number of turns.
    /// </summary>
    /// <param name="choiceContext">The current PlayerChoiceContext</param>
    /// <param name="targets">All enemies to apply this power to.</param>
    /// <param name="turnDelay">The number of turns to wait before executing the action. A value of 1 will execute the function at the start of next turn.</param>
    /// <param name="isDebuff">Determines whether this power should be blocked by Artifact.</param>
    /// <param name="description">An explanation of what will happen when the countdown reaches 0.</param>
    /// <param name="function">The function to execute after the delay.</param>
    /// <param name="applier">Optionally pass the applier.</param>
    /// <param name="cardSource">Optionally pass the card source.</param>
    /// <param name="isSilent">Optionally pass true to apply this power silently.</param>
    public static async Task JinxAsync(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, int turnDelay, bool isDebuff, LocString description, Func<PlayerChoiceContext, Creature, Task> function, Creature? applier, CardModel? cardSource, bool isSilent = false)
    {
        if (turnDelay < 1)
        {
            turnDelay = 1;
        }

        foreach (Creature target in targets)
        {
            await JinxAsync(choiceContext, target, turnDelay, isDebuff, description, function, applier, cardSource, isSilent);
        }
    }
}