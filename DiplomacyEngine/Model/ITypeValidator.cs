namespace DiplomacyEngine.Model;

public interface ITypeValidator
{
    public string Domain();
    public bool IsType(ITypeValidator other);
}