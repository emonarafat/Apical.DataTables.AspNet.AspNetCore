namespace Datatables.AspNetCore.Core
{
    public interface ISearch
    {
        string Value { get; }
        bool IsRegex { get; }
    }
}
