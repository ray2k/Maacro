using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maacro.Model
{
    public class MacroClickStep : MacroStep
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ScreenElementType ScreenElement { get; set; }
    }
}
