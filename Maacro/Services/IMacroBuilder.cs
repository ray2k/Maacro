using Maacro.Model;
using System;

namespace Maacro.Services
{
    public interface IMacroBuilder
    {
        MacroBuilder AddClick(ScreenElementType element);
        MacroBuilder AddClick(ScreenElementType element, string comment);
        MacroBuilder AddDelay();
        MacroBuilder AddDelay(string comment);
        MacroBuilder AddDelay(int delayMs);
        MacroBuilder AddDelay(int delayMs, string comment);
    }
}
