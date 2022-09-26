using DiplomacyEngine.Model.Types;

namespace DiplomacyEngine.Model.Orders;

public class CanceledOrder : Order
{
    public OrderResult Result { get; }
    public List<ITargetable?> TargetsExpected { get; }
    public List<ITargetable> Targets { get; }
    public Unit Source { get; }
    public double Priority { get; }
    
    // public override void NotifyTargets()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public override bool? IsExecuted()
    // {
    //     throw new NotImplementedException();
    // }

    public override bool? ValidateExecution()
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<ITargetable> GetNextTargetable()
    {
        throw new NotImplementedException();
    }

    public override bool SelectNextTarget(ITargetable target)
    {
        throw new NotImplementedException();
    }

    public override bool SelectTarget(ITargetable target, int index)
    {
        throw new NotImplementedException();
    }
}