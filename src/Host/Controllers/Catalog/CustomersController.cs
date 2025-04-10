using ShowMatic.Server.Application.Catalog.Customers.Create;
using ShowMatic.Server.Application.Catalog.Customers.Delete;
using ShowMatic.Server.Application.Catalog.Customers.Export;
using ShowMatic.Server.Application.Catalog.Customers.GetDetails;
using ShowMatic.Server.Application.Catalog.Customers.Search;
using ShowMatic.Server.Application.Catalog.Customers.Update;
using Showmatics.Host.Controllers;

namespace ShowMatic.Server.Host.Controllers.Catalog;

public class CustomersController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.Customers)]
    [OpenApiOperation("Search customers using available filters.", "")]
    public Task<PaginationResponse<GetCustomerResponse>> SearchAsync(SearchCustomersRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet]
    [MustHavePermission(FSHAction.View, FSHResource.Customers)]
    [OpenApiOperation("Get customer details.", "")]
    public Task<GetCustomerDetailsResponse> GetAsync(int id)
    {
        return Mediator.Send(new GetCustomerRequest() { Id = id});
    }

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.Customers)]
    [OpenApiOperation("Create a new customer.", "")]
    public Task<int> CreateAsync(CreateCustomerRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:int}")]
    [MustHavePermission(FSHAction.Update, FSHResource.Customers)]
    [OpenApiOperation("Update a customer.", "")]
    public async Task<ActionResult<int>> UpdateAsync(UpdateCustomerRequest request, int id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:int}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Customers)]
    [OpenApiOperation("Delete a customer.", "")]
    public Task<int> DeleteAsync(int id)
    {
        return Mediator.Send(new DeleteCustomerRequest(id));
    }

    [HttpPost("export")]
    [MustHavePermission(FSHAction.Export, FSHResource.Customers)]
    [OpenApiOperation("Export a customers.", "")]
    public async Task<FileResult> ExportAsync(ExportCustomersRequest filter)
    {
        var result = await Mediator.Send(filter);
        return File(result, "application/octet-stream", "CustomerExports");
    }
}