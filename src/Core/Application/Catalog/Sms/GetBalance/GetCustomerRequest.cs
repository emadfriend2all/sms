namespace ShowMatic.Server.Application.Catalog.Smss.GetDetails;

public class GetBalanceRequest : IRequest<GetBalanceResponse>
{
    public int Id { get; set; }
}

public class GetSmsRequestHandler : IRequestHandler<GetBalanceRequest, GetBalanceResponse>
{
    public async Task<GetBalanceResponse> Handle(GetBalanceRequest request, CancellationToken cancellationToken)
    {
        var response = await ApiHelper.GetAsync<GetBalanceResponse>("/balance");
        return response;
    }
}