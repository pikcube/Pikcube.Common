using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines an extension block that adds additional methods to all types that implement CardModel
/// </summary>
public static class CardModelExtensions
{
    extension<T>(T instance) where T : CardModel
    {
        /// <summary>
        /// Creates a mutable instance of a card and sets the card's owner to the player.
        /// </summary>
        /// <param name="player">The player who this card belongs to</param>
        /// <returns>A mutable instance of T with the owner set.</returns>
        public static T CreateInstance(Player player)
        {
            return player.RunState.CreateCard<T>(player);
        }

        /// <summary>
        /// Create an immutable instance of the card.
        /// </summary>
        /// <returns>An immutable instance of T.</returns>
        public static T CreateWithoutOwner()
        {
            return ModelDb.Card<T>();
        }

        /// <summary>
        /// Creates a mutable instance of a card and sets the card's owner to the player.
        /// </summary>
        /// <param name="player">The player who this card will belong to.</param>
        /// <returns>A mutable instance of T with the owner set.</returns>
        public T CreateNewInstance(Player player)
        {
            return (T)player.RunState.CreateCard(instance.CanonicalInstance, player);
        }
    }
}