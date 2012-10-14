using Maacro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maacro.Services
{
    public class MacroDataValidator : Maacro.Services.IMacroDataValidator
    {
        public IList<string> Validate(MacroData macroData)
        {
            List<string> result = new List<string>();

            if (macroData.Deployment.Count < 2)
                result.Add("Minimal deployment order has not been set");

            CheckRequiredScreenElements(macroData, result);
            
            return result;
        }

        private void CheckRequiredScreenElements(MacroData source, IList<string> errors)
        {
            List<ScreenElementType> required = new List<ScreenElementType>()
            {
                ScreenElementType.CollectAll,
                ScreenElementType.ConfirmButton
            };

            if (source.Deployment.Count > 0)
            {
                int maxPage = source.Deployment.Max(p => p.PageNumber);
                if (maxPage > 1)
                {
                    required.Add(ScreenElementType.NextHeroPage);
                    required.Add(ScreenElementType.PrevHeroPage);
                }
            }

            var elementsByType = source.ScreenElements.ToDictionary(p => p.ElementType);

            for (int bayNumber = 1; bayNumber <= source.Deployment.Count; bayNumber++)
            {
                var bayElementType = ScreenElement.GetScreenElementTypeForBayNumber(bayNumber);
                required.Add(bayElementType);
            }

            source.Deployment.Select(p => p.SlotNumber)
                .OrderBy(p => p)
                .Distinct()
                .ToList()
                .ForEach(sn =>
                    {
                        var slotElementType = ScreenElement.GetScreenElementTypeForSlotNumber(sn);
                        required.Add(slotElementType);
                    }
            );
            
            foreach (var req in required)
            {
                if (!elementsByType[req].HasLocationSet)
                    errors.Add("Screen element '{0}' does not have a mouse location defined", elementsByType[req].Name);
            }
        }
    }
}
