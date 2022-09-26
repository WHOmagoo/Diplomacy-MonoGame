using System.Collections.Immutable;

namespace DiplomacyEngine.Model.Orders;

public class OrderResult
{
    public static OrderResult Bounce { get; } = new OrderResult(nameof(Bounce), 1, null!);
    public static OrderResult Canceled { get; } = new OrderResult(nameof(Canceled), 1, null!);
    public static OrderResult Succeeded { get; } = new OrderResult(nameof(Succeeded), 1, null!);
    public static OrderResult Failed { get; } = new OrderResult(nameof(Failed), 1, null!);
    public string Name { get; }
    public double Priority { get; }
    public IReadOnlyCollection<Order> BecauseOf { get; }

    public OrderResult(string name, double priority, ICollection<Order> becauseOf)
    {
        Name = name;
        Priority = priority;
        BecauseOf = becauseOf.ToImmutableList(); 
    }

    public OrderResult MakeNew(ICollection<Order> becauseOf)
    {
        return new OrderResult(Name, Priority, becauseOf);
    }
    
}