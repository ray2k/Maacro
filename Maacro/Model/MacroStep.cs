using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Maacro.Model
{
    [DebuggerDisplay("Step {StepNumber} : {Description}")]
    public abstract class MacroStep
    {
        public string Description { get; set; }
        public string Comment { get; set; }
        public int StepNumber { get; set; }
    }
}
