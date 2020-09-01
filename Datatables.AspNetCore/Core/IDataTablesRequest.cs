
using System.Collections.Generic;

namespace Datatables.AspNetCore.Core
{
    public interface IDataTablesRequest
    {
        int Draw { get; }
        int Start { get; }
        int Length { get; }
        ISearch Search { get; }
        IEnumerable<IColumn> Columns { get; }
        IDictionary<string, object> AdditionalParameters { get; }
    }
}
