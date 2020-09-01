namespace Datatables.AspNetCore.Core
{
    public interface ISort
    {
        int Order { get; }
        SortDirection Direction { get; }
    }
}
