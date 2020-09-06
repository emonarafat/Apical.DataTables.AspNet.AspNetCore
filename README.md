# Apical.DataTables.AspNet.AspNetCore
AspNetCore implementation for DataTables.AspNet.
Fork from [datatables.aspnet](https://github.com/ALMMa/datatables.aspnet).
Removes Newtonsoft.Json.Net  dependency and use  System.Text.Json instead.

- [Apical.DataTables.AspNet.AspNetCore](https://www.nuget.org/packages/Apical.DataTables.AspNet.AspNetCore) with support for AspNetCore, 
dependency injection and automatic binders

## Basic Integration
```csharp
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddMvc();

			// DataTables.AspNet registration with default options.
			services.RegisterDataTables();
        }
```
## Controller Action
```csharp
 /// <summary>
        /// This is your data method.
        /// DataTables will query this (HTTP GET) to fetch data to display.
        /// </summary>
        /// <param name="request">
        /// This represents your DataTables request.
        /// It's automatically binded using the default binder and settings.
        /// 
        /// You should use IDataTablesRequest as your model, to avoid unexpected behavior and allow
        /// custom binders to be attached whenever necessary.
        /// </param>
        /// <returns>
        /// Return data here, with a json-compatible result.
        /// </returns>
        public IActionResult PageData(IDataTablesRequest request)
        {
            // Nothing important here. Just creates some mock data.
            var data = Models.SampleEntity.GetSampleData();

            // Global filtering.
            // Filter is being manually applied due to in-memmory (IEnumerable) data.
            // If you want something rather easier, check IEnumerableExtensions Sample.
            var filteredData = String.IsNullOrWhiteSpace(request.Search.Value)
				? data
				: data.Where(_item => _item.Name.Contains(request.Search.Value));

            // Paging filtered data.
            // Paging is rather manual due to in-memmory (IEnumerable) data.
            var dataPage = filteredData.Skip(request.Start).Take(request.Length);

            // Response creation. To create your response you need to reference your request, to avoid
            // request/response tampering and to ensure response will be correctly created.
            var response = DataTablesResponse.Create(request, data.Count(), filteredData.Count(), dataPage);

            // Easier way is to return a new 'DataTablesJsonResult', which will automatically convert your
            // response to a json-compatible content, so DataTables can read it when received.
            return new DataTablesJsonResult(response, true);
        }
 ```
