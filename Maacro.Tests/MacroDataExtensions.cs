using Maacro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maacro.Tests
{
    public static class MacroDataExtensions
    {
        public static MacroData AsValid(this MacroData macroData)
        {
            macroData.HeroPageCount = 10;
            macroData.UIDelay = 1500;

            var rnd = new Random(DateTime.Now.Millisecond);
            foreach (var se in macroData.ScreenElements)
            {
                se.X = rnd.Next(1, 100);
                se.Y = rnd.Next(1, 100);
            }

            foreach (var d in macroData.Deployment)
            {
                d.PageNumber = rnd.Next(1, 10);
                d.SlotNumber = rnd.Next(1, 5);
            }

            return macroData;
        }
    }
}
