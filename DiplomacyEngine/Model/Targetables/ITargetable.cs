using DiplomacyEngine.Model.Orders;

namespace DiplomacyEngine.Model.Types;

public interface ITargetable
{

    public int Capacity { get; }
    public bool IsType(ITargetable other);
    
    public IEnumerable<ITargetable> GetParentTypes();

    public void AddTargetedBy(Order order);

    public IEnumerable<Order> GetTargetedBy();
}