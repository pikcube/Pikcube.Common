using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Code.Unions;

/// <summary>
/// A type that encapsualtes either a creature, player, or monster in combat.
/// </summary>
public readonly union CombatMember(Creature, Player, MonsterModel)
{
    /// <summary>
    /// Returns true if this combat member is a player.
    /// </summary>
    public bool IsPlayer => this switch
    {
        Creature creature => creature.IsPlayer,
        Player => true,
        MonsterModel => false,
        _ => throw new InvalidCastException()
    };

    /// <summary>
    /// Implicitly converts a combat member into a creature.
    /// </summary>
    public static implicit operator Creature(CombatMember combatMember) => combatMember switch
    {
        Creature c => c,
        Player p => p.Creature,
        MonsterModel m => m.Creature,
        _ => throw new InvalidCastException()
    };

    /// <summary>
    /// Ensures CombatMember remains null if the value would be null.
    /// </summary>
    public static implicit operator CombatMember?(Creature? creature)
    {
        if (creature is null)
        {
            return null;
        }

        return new CombatMember(creature);
    }

    /// <summary>
    /// Ensures CombatMember remains null if the value would be null.
    /// </summary>
    public static implicit operator CombatMember?(Player? player)
    {
        if (player is null)
        {
            return null;
        }

        return new CombatMember(player);
    }

    /// <summary>
    /// Ensures CombatMember remains null if the value would be null.
    /// </summary>
    public static implicit operator CombatMember?(MonsterModel? monsterModel)
    {
        if (monsterModel is null)
        {
            return null;
        }

        return new CombatMember(monsterModel);
    }
}