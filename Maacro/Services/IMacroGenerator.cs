using Maacro.Model;
using System;
using System.Collections.Generic;

namespace Maacro.Services
{
    public interface IMacroGenerator
    {
        IEnumerable<MacroStep> GenerateMacro(IEnumerable<DeploymentSlot> deployment, IEnumerable<ScreenElement> screenElements, int uiDelay, int totalHeroPages, DeployLength length);
    }
}
