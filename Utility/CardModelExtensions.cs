using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

public static class CardModelExtensions
{
    extension<T>(T _) where T : CardModel
    {
        public static T CreateInstance(Player p)
        {
            return p.RunState.CreateCard<T>(p);
        }

        public static T CreateWithoutOwner()
        {
            return ModelDb.Card<T>();
        }

        public T CreateNewInstance(Player p)
        {
            return p.RunState.CreateCard<T>(p);
        }
    }
}