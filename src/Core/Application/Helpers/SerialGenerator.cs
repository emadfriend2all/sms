using ShowMatic.Server.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Showmatics.Application.Helpers;

public static class SerialGenerator
{
    public static string Generate(SerialSettings settings)
    {
        return settings.Prefex + settings.PrefexSeparator ?? string.Empty +
            settings.Code + settings.CodeSeparator ?? string.Empty + settings.Sufex;
    }
}