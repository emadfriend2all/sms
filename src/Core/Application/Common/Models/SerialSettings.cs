using ShowMatic.Server.Application.Enums;
using Showmatics.Application.Helpers;

namespace Showmatics.Application.Common.Models;

public class SerialSettings
{
    public string Id { get; set; }
    public string? Prefex { get; set; }
    public string? PrefexSeparator { get; set; }
    public string? Code { get; set; }
    public string? CodeSeparator { get; set; }
    public string? Sufex { get; set; }
}