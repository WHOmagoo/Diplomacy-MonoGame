using AutoMapper.Internal;
using DiplomacyEngine.Model.Types;

namespace DiplomacyEngine.Model.Orders;

public class Support : Order
{
    public OrderResult Result { get; }
    public List<ITargetable?> TargetsExpected { get; }
    public List<ITargetable> Targets { get; }
    public Unit Source { get; }
    public double Priority { get; }

    private int index;
    
    private bool? _isExecuted;

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
        switch (index)
        {
            
            case 1:
                return FirstTargets();
            case 2:
                return SecondTargets();
            default:
                return Enumerable.Empty<ITargetable>();
        }
    }

    private IEnumerable<ITargetable> FirstTargets()
    {
        Attack thisAttack = new Attack(Source);

        return Source.Location.Neighbors.GetNeighborsWithinDegree(2)
            .Where(l =>
            {
                if (l.OccupiedBy is null)
                {
                    return false;
                }
                Attack a = new Attack(l.OccupiedBy);
                return a.GetNextTargetable().Intersect(thisAttack.GetNextTargetable()).Any();
            });
    }

    private IEnumerable<ITargetable> SecondTargets()
    {
        Attack thisAttack = new Attack(Source);

        if (PreviousTarget() is Unit supporting)
        {
            Attack otherAttack = new Attack(supporting);

            return thisAttack.GetNextTargetable().Intersect(otherAttack.GetNextTargetable());
        }

        return Enumerable.Empty<ITargetable>();
    }

    private ITargetable? PreviousTarget()
    {
        return index == 0 ? null : Targets[index - 1];
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