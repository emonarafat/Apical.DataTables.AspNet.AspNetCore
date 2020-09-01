namespace Datatables.AspNetCore.Core
{
    public interface IColumn
    {
        string Name { get; }
        string Field { get; }
        bool IsSearchable { get; }
        ISearch Search { get; }
        bool IsSortable { get; }
        ISort Sort { get; }

        bool SetSort(int order, string direction);
    }
}
