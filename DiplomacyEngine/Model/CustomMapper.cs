namespace DiplomacyEngine.Model;

public class CustomMapper
{
    private int val = 0;
    public CustomMapper(int val)
    {
        this.val = val;
    }

    public override string ToString()
    {
        return "1: " + val.ToString();
    }
    
    public static implicit operator CustomMapper2(CustomMapper cm)
    {
        return new CustomMapper2(cm.val);
    }
}

public class CustomMapper2
{
    private string val;

    public CustomMapper2(int val)
    {
        this.val = val.ToString();
    }

    public static implicit operator CustomMapper(CustomMapper2 cm)
    {
        return new CustomMapper(int.Parse(cm.val));
    }

    public override string ToString()
    {
        return "2: " + val;
    }
}