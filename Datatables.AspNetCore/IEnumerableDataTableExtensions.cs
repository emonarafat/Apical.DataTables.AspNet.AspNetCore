
using Datatables.AspNetCore.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Datatables.AspNetCore
{
    public static class IEnumerableDataTableExtensions
    {
        public static IEnumerable<T> Compute<T>(this IEnumerable<T> data, Core.IDataTablesRequest request,
            out int filteredDataCount)
        {
            filteredDataCount = 0;
            if (!data.Any() || request == null)
                return data;

            // Global filtering.
            // Filter is being manually applied due to in-memmory (IEnumerable) data.
            // If you want something rather easier, check IEnumerableExtensions Sample.
            // var filteredData = data.Where(_item => _item.Hostname.Contains(request.Search.Value));
            var filteredData = Enumerable.Empty<T>();

            // Inutile de faire une recherche s'il n'y a rien à chercher.
            if (!string.IsNullOrEmpty(request.Search.Value))
            {
                var filteredColumn = request.Columns.Where(c => c.IsSearchable);
                filteredData = filteredColumn.Select(sColumn =>
                data.First()
                .GetType()
                .GetProperty(sColumn.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance))
                    .Select(propertyInfo => data.PropertyContains(propertyInfo, request.Search.Value))
                    .Aggregate(filteredData, (current, columnResult) => current.Concat(columnResult));

                // Pour éviter les doublons
                filteredData = filteredData.Distinct();
            }
            else
            {
                filteredData = data;
            }

            // Ordering filtred data
            var orderedColumn = request.Columns.Where(c => c.IsSortable && c.Sort != null);
            filteredData = orderedColumn.Aggregate(filteredData, (current, sColumn) => current.OrderBy(sColumn));

            // Paging filtered data.
            // Paging is rather manual due to in-memmory (IEnumerable) data.
            // var dataPage = filteredData.OrderBy(d => d.ID).Skip(request.Start);
            var dataPage = filteredData.Skip(request.Start);
            if (request.Length != -1) dataPage = dataPage.Take(request.Length);

            filteredDataCount = filteredData.Count();
            return dataPage;
        }
        //public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> entities, string propertyName)
        //{
        //    if (!entities.Any() || string.IsNullOrEmpty(propertyName))
        //        return entities;

        //    var x = propertyName.Split(" ");

        //    var propertyInfo = entities.First().GetType().GetProperty(x[0], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        //    if (x.Length == 1)
        //    {
        //        return entities.OrderBy(e => propertyInfo.GetValue(e, null));
        //    }
        //    return entities.OrderByDescending(e => propertyInfo.GetValue(e, null));

        //}
        public static PropertyInfo GetProp(Type baseType, string propertyName)
        {
            string[] parts = propertyName.Split('.');

            return (parts.Length > 1)
                ? GetProp(baseType.GetProperty(parts[0]).PropertyType, parts.Skip(1).Aggregate((a, i) => a + "." + i))
                : baseType.GetProperty(propertyName);
        }
        // https://github.com/ALMMa/datatables.aspnet/issues/58
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> entities, Core.IColumn column)
        {
            if (!entities.Any() || column == null)
                return entities;

            var propertyInfo = entities.First().GetType().GetProperty(column.Field,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                return entities;
            }
            if (column.Sort.Direction == SortDirection.Ascending)
                return entities.OrderBy(e => propertyInfo.GetValue(e, null));
            return entities.OrderByDescending(e => propertyInfo.GetValue(e, null));
        }

        // Inspire : https://stackoverflow.com/questions/22104050/linq-to-entities-does-not-recognize-the-method-system-object-getvalue
        // and : https://stackoverflow.com/questions/4553836/how-to-create-an-expression-tree-to-do-the-same-as-startswith
        public static IEnumerable<T> PropertyContains<T>(this IEnumerable<T> data, PropertyInfo propertyInfo,
            string value)
        {
            var param = Expression.Parameter(typeof(T));
            var m = Expression.MakeMemberAccess(param, propertyInfo);
            var c = Expression.Constant(value.ToUpper(), typeof(string));
            var mi_contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var mi_tostring = typeof(object).GetMethod("ToString");
            var mi_toupper = typeof(string).GetMethod("ToUpper", new Type[] { });
            Expression call = Expression.Call(Expression.Call(Expression.Call(m, mi_tostring), mi_toupper), mi_contains, c);

            var lambda = Expression.Lambda<Func<T, bool>>(call, param);

            return data.AsQueryable().Where(lambda);
        }
    }


}
