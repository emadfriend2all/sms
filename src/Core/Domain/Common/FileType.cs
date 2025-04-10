using System.ComponentModel;

namespace Showmatics.Domain.Common;

public enum FileType
{
    [Description(".jpg,.png,.jpeg")]
    Image,
    [Description(".pdf,.doc,.docx,.xls,.xlsx,.csv")]
    File
}