namespace Showmatics.Blazor.Client.Infrastructure.Common;

public static class ApplicationConstants
{
    public static readonly List<string> SupportedImageFormats = new()
    {
        ".jpeg",
        ".jpg",
        ".png"
    };
    public static readonly List<string> SupportedFileFormats = new()
    {
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".csv",
    };
    public static readonly string StandardImageFormat = "image/jpeg";
    public static readonly string StandardFileFormat = "application/octet-stream";
    public static readonly int MaxImageWidth = 1500;
    public static readonly int MaxImageHeight = 1500;
    public static readonly long MaxAllowedSize = 1000000; // Allows Max File Size of 1 Mb.
}
