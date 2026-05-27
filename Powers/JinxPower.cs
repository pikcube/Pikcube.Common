using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Powers;

/// <summary>
/// Power for executing an arbitrary command later. Only apply using JinxCmd.
/// </summary>
public class JinxPower : CustomPowerModel
{
    private bool IsPrepared { get; set; }
    private bool IsDebuff { get; set; }
    private Func<PlayerChoiceContext, Creature, Task> OnCountdownFinished { get; set; } = (_, _) => Task.CompletedTask;

    /// <inheritdoc />
    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("JinxDescription", "Does something unique.")];

    private string DynamicDescription
    {
        set => ((StringVar)DynamicVars["JinxDescription"]).StringValue = value;
    }

    /// <inheritdoc />
    public override PowerType Type => GetTypeInternal();

    private PowerType GetTypeInternal()
    {
        if (IsPrepared && !IsDebuff)
        {
            return PowerType.Buff;
        }

        return PowerType.Debuff;
    }

    /// <inheritdoc />
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <inheritdoc />
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        if (Amount > 1)
        {
            await PowerCmd.Decrement(this);
            return;
        }

        Flash();

        await OnCountdownFinished(choiceContext, Owner);
        await PowerCmd.Remove(this);
    }

    internal void PrepareForApplication(bool isDebuff, Func<PlayerChoiceContext, Creature, Task> function, LocString description)
    {
        IsPrepared = true;
        IsDebuff = isDebuff;
        OnCountdownFinished = function;
        DynamicDescription = description.GetRawText();
    }

    /// <inheritdoc />
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        return IsPrepared ? Task.CompletedTask : throw new InvalidOperationException("Jinx power must be applied through JinxCmd");
    }
}