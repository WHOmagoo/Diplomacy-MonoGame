using DiplomacyEngine.Model.Types;

namespace DiplomacyEngine.Model.Orders;

public class Attack : Order
{
    public OrderResult Result { get; }
    public List<ITargetable?> TargetsExpected { get; }
    public List<ITargetable> Targets { get; }
    public Unit Source { get; }
    public double Priority { get; }

    private bool? _isExecuted;
    
    private int index = 1;

    private int power = 1;
    public Attack(Unit source)
    {
        TargetsExpected = new List<ITargetable?>();
        TargetsExpected.Add(LocationType.Location);
        Source = source;
    }

    ITargetable? NextType()
    {
        if (index < TargetsExpected.Count)
        {
            return TargetsExpected[index];
        }

        return null;
    }

    // public override void NotifyTargets()
    // {
    //     foreach (var targetable in Targets)
    //     {
    //         targetable.AddTargetedBy(this);
    //     }
    // }
    //
    // public override bool? IsExecuted()
    // {
    //     return _isExecuted;
    // }

    public bool? Suceeds()
    {
        if (IsExecuted != true)
        {
            return IsExecuted;
        }


        return false;
        // if (supporting is null)
        // {
        //     return false;
        // }
        //
        // return supporting.Order.
    }
    
    public override bool? ValidateExecution()
    {

        if (_isExecuted.HasValue)
        {
            return _isExecuted;
        }
        
        foreach (var order in Source.Location.GetTargetedBy())
        {
            if (!(Priority < order.Priority)) continue;
            bool? executed = order.IsExecuted;
            _isExecuted = executed;
            return !executed;
        }

        _isExecuted = true;
        return _isExecuted;
    }

    public override IEnumerable<ITargetable> GetNextTargetable()
    {
        if(index < TargetsExpected.Count){
            return Source.Location.Neighbors.Where(l => Source.Type.Visits(l.Type) && (NextType()?.IsType(l.Type) ?? false));
        }

        return Enumerable.Empty<ITargetable>();
    }

    public override bool SelectNextTarget(ITargetable target)
    {
        if (!GetNextTargetable().Contains(target))
        {
            return false;
        }

        index++;
        
        Targets.Add(target);
        return true;
    }

    public override bool SelectTarget(ITargetable target, int index)
    {
        throw new NotImplementedException();
    }
}