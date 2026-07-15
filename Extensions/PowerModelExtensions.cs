using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Extensions;

/// <summary>
/// Extension on all types that Implement PowerModel
/// </summary>
public static class PowerModelExtensions
{
    extension<T>(T instance) where T : PowerModel
    {
        /// <summary>
        /// Apply a power asyncronously using PowerCmd.Apply.
        /// </summary>
        /// <param name="choiceContext">The current choice context</param>
        /// <param name="target">The target creature</param>
        /// <param name="amount">The amount to apply</param>
        /// <param name="applier">The creature that applied this power (if any)</param>
        /// <param name="cardSource">The card that applied this power (if any)</param>
        /// <param name="silent">When true, don't show the power being applied</param>
        /// <returns>The power that was just applied.</returns>
        public static Task<T?> ApplyAsync(PlayerChoiceContext choiceContext, Creature target, decimal amount, Creature? applier, CardModel? cardSource, bool silent = false)
        {
            return PowerCmd.Apply<T>(choiceContext, target, amount, applier, cardSource, silent);
        }

        /// <summary>
        /// Apply a power asyncronously using PowerCmd.Apply.
        /// </summary>
        /// <param name="choiceContext">The current choice context</param>
        /// <param name="target">The target player</param>
        /// <param name="amount">The amount to apply</param>
        /// <param name="applier">The creature that applied this power (if any)</param>
        /// <param name="cardSource">The card that applied this power (if any)</param>
        /// <param name="silent">When true, don't show the power being applied</param>
        /// <returns>The power that was just applied.</returns>
        public static Task<T?> ApplyAsync(PlayerChoiceContext choiceContext, Player target, decimal amount, Creature? applier, CardModel? cardSource, bool silent = false)
        {
            return PowerCmd.Apply<T>(choiceContext, target.Creature, amount, applier, cardSource, silent);
        }

        /// <summary>
        /// Apply a power asyncronously using PowerCmd.Apply.
        /// </summary>
        /// <param name="choiceContext">The current choice context</param>
        /// <param name="target">The target creature</param>
        /// <param name="amount">The amount to apply</param>
        /// <param name="applier">The player that applied this power (if any)</param>
        /// <param name="cardSource">The card that applied this power (if any)</param>
        /// <param name="silent">When true, don't show the power being applied</param>
        /// <returns>The power that was just applied.</returns>
        public static Task<T?> ApplyAsync(PlayerChoiceContext choiceContext, Creature target, decimal amount, Player? applier, CardModel? cardSource, bool silent = false)
        {
            return PowerCmd.Apply<T>(choiceContext, target, amount, applier?.Creature, cardSource, silent);
        }

        /// <summary>
        /// Apply a power asyncronously using PowerCmd.Apply.
        /// </summary>
        /// <param name="choiceContext">The current choice context</param>
        /// <param name="target">The target player</param>
        /// <param name="amount">The amount to apply</param>
        /// <param name="applier">The player that applied this power (if any)</param>
        /// <param name="cardSource">The card that applied this power (if any)</param>
        /// <param name="silent">When true, don't show the power being applied</param>
        /// <returns>The power that was just applied.</returns>
        public static Task<T?> ApplyAsync(PlayerChoiceContext choiceContext, Player target, decimal amount, Player? applier, CardModel? cardSource, bool silent = false)
        {
            return PowerCmd.Apply<T>(choiceContext, target.Creature, amount, applier?.Creature, cardSource, silent);
        }

        /// <summary>
        /// Apply a power asyncronously using PowerCmd.Apply.
        /// </summary>
        /// <param name="choiceContext">The current choice context</param>
        /// <param name="targets">The target creatures</param>
        /// <param name="amount">The amount to apply</param>
        /// <param name="applier">The creature that applied this power (if any)</param>
        /// <param name="cardSource">The card that applied this power (if any)</param>
        /// <param name="silent">When true, don't show the power being applied</param>
        /// <returns>The power that was just applied.</returns>
        public static Task<IReadOnlyList<T>> ApplyAsync(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, decimal amount, Creature? applier, CardModel? cardSource, bool silent = false)
        {
            return PowerCmd.Apply<T>(choiceContext, targets, amount, applier, cardSource, silent);
        }

        /// <summary>
        /// Apply a power asyncronously using PowerCmd.Apply.
        /// </summary>
        /// <param name="choiceContext">The current choice context</param>
        /// <param name="targets">The target players</param>
        /// <param name="amount">The amount to apply</param>
        /// <param name="applier">The creature that applied this power (if any)</param>
        /// <param name="cardSource">The card that applied this power (if any)</param>
        /// <param name="silent">When true, don't show the power being applied</param>
        /// <returns>The power that was just applied.</returns>
        public static Task<IReadOnlyList<T>> ApplyAsync(PlayerChoiceContext choiceContext, IEnumerable<Player> targets, decimal amount, Creature? applier, CardModel? cardSource, bool silent = false)
        {
            return PowerCmd.Apply<T>(choiceContext, targets.Select(p => p.Creature), amount, applier, cardSource, silent);
        }

        /// <summary>
        /// Apply a power asyncronously using PowerCmd.Apply.
        /// </summary>
        /// <param name="choiceContext">The current choice context</param>
        /// <param name="targets">The target creatures</param>
        /// <param name="amount">The amount to apply</param>
        /// <param name="applier">The player that applied this power (if any)</param>
        /// <param name="cardSource">The card that applied this power (if any)</param>
        /// <param name="silent">When true, don't show the power being applied</param>
        /// <returns>The power that was just applied.</returns>
        public static Task<IReadOnlyList<T>> ApplyAsync(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, decimal amount, Player? applier, CardModel? cardSource, bool silent = false)
        {
            return PowerCmd.Apply<T>(choiceContext, targets, amount, applier?.Creature, cardSource, silent);
        }

        /// <summary>
        /// Apply a power asyncronously using PowerCmd.Apply.
        /// </summary>
        /// <param name="choiceContext">The current choice context</param>
        /// <param name="targets">The target players</param>
        /// <param name="amount">The amount to apply</param>
        /// <param name="applier">The player that applied this power (if any)</param>
        /// <param name="cardSource">The card that applied this power (if any)</param>
        /// <param name="silent">When true, don't show the power being applied</param>
        /// <returns>The power that was just applied.</returns>
        public static Task<IReadOnlyList<T>> ApplyAsync(PlayerChoiceContext choiceContext, IEnumerable<Player> targets, decimal amount, Player? applier, CardModel? cardSource, bool silent = false)
        {
            return PowerCmd.Apply<T>(choiceContext, targets.Select(p => p.Creature), amount, applier?.Creature, cardSource, silent);
        }
    }
}