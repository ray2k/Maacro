using Maacro.Model;
using System;
using System.Collections.Generic;

namespace Maacro.Services
{
    public interface IMacroDataValidator
    {
        IList<string> Validate(MacroData macroData);
    }
}
