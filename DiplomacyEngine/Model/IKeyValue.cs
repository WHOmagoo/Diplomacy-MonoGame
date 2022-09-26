namespace DiplomacyEngine.Model;

public interface IPrimaryKey
{
    string GetKey(bool includeType = false);
}
public interface IKeyValue<T> : IPrimaryKey
{
    KeyValuePair<string, T> GetKeyValuePair(bool includeType = false);
}