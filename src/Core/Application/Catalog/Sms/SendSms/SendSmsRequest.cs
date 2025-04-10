using Mapster;
using ShowMatic.Server.Application.Catalog.Smss.GetDetails;
using Showmatics.Domain.Common.Events;

namespace ShowMatic.Server.Application.Catalog.Smss.Create;

public record SendSmsCommand(string PhoneNumber, string Message) : IRequest<SendSmsResponse>;
public record SendSmsResponse(string? status, string? message);
public class SendSmsRequestHandler : IRequestHandler<SendSmsCommand, SendSmsResponse>
{
    public async Task<SendSmsResponse> Handle(SendSmsCommand request, CancellationToken cancellationToken)
    {
        var barqRequest = new BarqSendSmsCommand(request.PhoneNumber, "Edikhark", "plain", request.Message, null, null);
        var barqResponse = await ApiHelper.PostAsync<BarqSendSmsCommand, BarqSendSmsResponse>("/sms/send", barqRequest);
        return new SendSmsResponse(barqResponse.Status, barqResponse.Message);
    }
}


// Send SMS Request
public record BarqSendSmsCommand(string recipient, string sender_id, string type, string message, DateTime? schedule_time, string? dlt_template_id);

// Send SMS Response
public record BarqSendSmsResponse(
    string? Status,
    string? Message,
    SmsData? Data
);

public record SmsData(
    string Uid,
    string To,
    string From,
    string Message,
    string Status,
    string Cost,
    int SmsCount
);