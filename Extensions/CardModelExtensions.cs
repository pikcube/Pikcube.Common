using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Keywords;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Extensions;

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

        /// <summary>
        /// Add a Keyword to this card until the start of next turn.
        /// </summary>
        /// <param name="keyword">The keyword to temporarily add</param>
        /// <param name="source">Optional: The object registering the keyword. Allows for early destruction by calling DestroyKeywordsEarly</param>
        public void AddTempKeyword(CardKeyword keyword, object? source = null)
        {
            TempKeywordManager.Register(instance, keyword, source);
        }

        /// <summary>
        /// Remove a Keyword prematurely.
        /// </summary>
        /// <param name="keyword">The keyword to remove.</param>
        public void RemoveTempKeywordEarly(CardKeyword keyword)
        {
            TempKeywordManager.DestroyKeywordsEarly(instance, keyword);
        }

        /// <summary>
        /// Exhaust target card by calling CardCmd.ExhaustAsync
        /// </summary>
        /// <param name="choiceContext">The player choice context</param>
        /// <param name="causedByEthereal">True if caused by Etherial</param>
        /// <param name="skipVisuals">True if visuals should be skipped</param>
        public Task ExhaustAsync(PlayerChoiceContext choiceContext, bool causedByEthereal = false, bool skipVisuals = false)
        {
            return CardCmd.Exhaust(choiceContext, instance, causedByEthereal, skipVisuals);
        }

        /// <summary>
        /// Blink target card by calling BlinkModel.BlinkCardAsync
        /// </summary>
        /// <param name="choiceContext">The player choice context</param>
        public Task BlinkAsync(PlayerChoiceContext choiceContext)
        {
            return BlinkModel.BlinkCardAsync(choiceContext, instance);
        }

        /// <summary>
        /// Entrance target card by calling EntranceModel.EntranceCardAsync
        /// </summary>
        /// <param name="choiceContext">The player choice context</param>
        public Task EntranceAsync(PlayerChoiceContext choiceContext)
        {
            return EntranceModel.EntranceCardAsync(choiceContext, instance);
        }
    }
}