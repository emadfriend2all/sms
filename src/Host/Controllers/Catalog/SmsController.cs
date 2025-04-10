using Microsoft.AspNetCore.Authentication.JwtBearer;
using ShowMatic.Server.Application.Catalog.Smss.Create;
using ShowMatic.Server.Application.Catalog.Smss.GetDetails;
using Showmatics.Host.Controllers;

namespace ShowMatic.Server.Host.Controllers.Catalog;

[Authorize(AuthenticationSchemes = "Bearer")]
public class SmsController : VersionedApiController
{
    [HttpGet("GetBalance")]
    [MustHavePermission(FSHAction.View, FSHResource.SMS)]
    [OpenApiOperation("Get balance.", "")]
    public Task<GetBalanceResponse> GetBalanceAsync()
    {
        return Mediator.Send(new GetBalanceRequest());
    }

    [HttpPost("SendSms")]
    [MustHavePermission(FSHAction.Create, FSHResource.SMS)]
    [OpenApiOperation("Send sms.", "")]
    public Task<SendSmsResponse> SendSmsAsync(SendSmsCommand request)
    {
        return Mediator.Send(request);
    }
}