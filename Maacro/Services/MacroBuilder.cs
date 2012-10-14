using Maacro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maacro.Services
{
    public class MacroBuilder : Maacro.Services.IMacroBuilder
    {
        private List<MacroStep> _steps = new List<MacroStep>();

        public MacroBuilder(IEnumerable<ScreenElement> screenElements, int uiDelay)
        {
            this.ScreenElements = screenElements.ToDictionary(p => p.ElementType);
            this.UIDelay = uiDelay;
        }

        public MacroBuilder AddDelay(int delayMs)
        {
            return AddDelay(delayMs, string.Empty);
        }

        public MacroBuilder AddDelay()
        {
            return AddDelay(this.UIDelay, string.Empty);
        }

        public MacroBuilder AddClick(ScreenElementType element)
        {
            return AddClick(element, string.Format("Click {0}", element));
        }

        internal int UIDelay { get; private set; }
        internal IDictionary<ScreenElementType, ScreenElement> ScreenElements { get; private set; }

        internal IEnumerable<MacroStep> GetMacro()
        {
            return this._steps;
        }

        public MacroBuilder AddClick(ScreenElementType element, string comment)
        {
            _steps.Add(new MacroClickStep()
            {
                X = this.ScreenElements[element].X,
                Y = this.ScreenElements[element].Y,
                Description = string.Concat("Click ", element),
                StepNumber = _steps.Count + 1,
                Comment = comment,
                ScreenElement = element
            });

            return this;
        }

        public MacroBuilder AddDelay(string comment)
        {
            return AddDelay(this.UIDelay, comment);
        }

        public MacroBuilder AddDelay(int delayMs, string comment)
        {
            _steps.Add(new MacroDelayStep()
            {
                Delay = delayMs,
                Description = string.Format("Wait {0} milliseconds ({1} seconds)", delayMs, TimeSpan.FromMilliseconds(delayMs).TotalSeconds),
                StepNumber = _steps.Count + 1,
                Comment = comment
            });

            return this;
        }
    }
}
