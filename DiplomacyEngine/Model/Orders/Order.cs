using DiplomacyEngine.Model.Types;

namespace DiplomacyEngine.Model.Orders;

public abstract class Order
{
    public OrderResult Result { get; }
    public List<ITargetable?> TargetsExpected { get; }
    public List<ITargetable> Targets { get; }
    public Unit Source { get; }
    
    List<Order> _targetetdBy;
    public double Priority { get; }

    public bool? IsExecuted { get; set; }

    public bool ValidateTargets()
    {
        return TargetsExpected.Zip(Targets, (expected, actual) => expected?.IsType(actual)).All(r => r ?? false);
    }

    public void NotifyTargets()
    {
        foreach (Order o in Targets)
        {
            o.TargetedBy(this); 
        }
    }

    public void TargetedBy(Order order)
    {
        _targetetdBy.Add(order);
    }

    /**
     * Returns true if Execution has been updated, false otherwise
     */
    public bool UpdateIsExecution()
    {
        if (IsExecuted.HasValue)
        {
            return false;
        }

        List<bool> results = new List<bool>(_targetetdBy.Count);
        foreach (Order o in _targetetdBy)
        {
            if (o.Priority >= Priority)
            {
                bool? isExecuted = !o.IsExecuted;
                
                //Since we short circuit, we can coalesce null to false, to make the last step easier
                results.Add(isExecuted ?? false);
                
                //Short circuit evaluation when canceled
                if (isExecuted == false)
                {
                    IsExecuted = false;
                    return true;
                }
            }
        }

        bool result = results.Min();
        IsExecuted = result ? true : null;
        return result;
    }
    public abstract bool? ValidateExecution();

    public abstract IEnumerable<ITargetable> GetNextTargetable();

    public abstract bool SelectNextTarget(ITargetable target);
 
    public abstract bool SelectTarget(ITargetable target, int index);
}