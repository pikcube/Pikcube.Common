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

    internal void PrepareForApplication(bool isDebuff, Func<PlayerChoiceContext, Creature, Task> function, LocString description)
    {
        IsPrepared = true;
        IsDebuff = isDebuff;
        OnCountdownFinished = function;

        foreach (KeyValuePair<string, object> obj in description.Variables)
        {
            SmartDescription.AddObj(obj.Key, obj.Value);
        }

        DynamicDescription = description.GetFormattedText();
    }

    /// <inheritdoc />
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (IsPrepared)
        {
            return;
        }

        throw new InvalidOperationException("Jinx power must be applied through JinxCmd");
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
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    /// <inheritdoc />
    public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
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
        if (Owner.IsAlive)
        {
            await PowerCmd.Remove(this);
        }
    }
}