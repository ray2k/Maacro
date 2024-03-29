﻿using Maacro.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Maacro.Services
{
    public class MacroGenerator : IMacroGenerator
    {
        private MacroBuilder Builder { get; set; }
        private Dictionary<int, ScreenElementType> BayLookup { get; set; }
        private Dictionary<int, ScreenElementType> SlotLookup { get; set; }

        public MacroGenerator()
        {            
        }

        public IEnumerable<MacroStep> GenerateMacro(IEnumerable<DeploymentSlot> deployment, IEnumerable<ScreenElement> screenElements,  int uiDelay, int totalHeroPages, DeployLength length)
        {
            this.Builder = new MacroBuilder(screenElements, uiDelay);
            int deploymentNumber = 1;
            SetupBayLookup(screenElements);
            SetupSlotLookup(screenElements);

            foreach (var toDeploy in deployment)
            {
                Trace.WriteLine(string.Format("Writing steps for Page {0} Slot {1}", toDeploy.PageNumber, toDeploy.SlotNumber));
                
                Builder.AddDelay("Startup delay")
                        .AddClick(GetBayFor(deploymentNumber))
                        .AddDelay()
                        .AddClick(GetBayFor(deploymentNumber)) // click deploy
                        .AddDelay();

                if (deploymentNumber == 1)
                {
                    int pagesToDecrement = totalHeroPages - 1;
                    for (int i = 0; i < pagesToDecrement; i++)
                    {
                        Builder.AddClick(ScreenElementType.PrevHeroPage, "to get back to page 1");
                        Builder.AddDelay();
                    }
                }

                if (toDeploy.PageNumber > 1)
                {
                    int pagesToIncrement = toDeploy.PageNumber - 1;
                    for (int i = 0; i < pagesToIncrement; i++)
                    {
                        Builder.AddClick(ScreenElementType.NextHeroPage, "to get to target page").AddDelay();
                    }
                }

                Builder.AddClick(GetSlotFor(toDeploy.SlotNumber))
                       .AddDelay()
                       .AddClick(ScreenElementType.ConfirmButton);
                
                deploymentNumber++;
            }

            var deployDelay = TimeSpan.Zero;

            if (length == DeployLength.TwentyMinute)
            {
                deployDelay = new TimeSpan(0, 20, 3);

                int remainingDelay = (int) deployDelay.TotalMilliseconds;
                int totalDelayAccountedFor = 0;

                while (totalDelayAccountedFor < (int)deployDelay.TotalMilliseconds)
                {
                    int batchSizeInMs = (int)TimeSpan.FromMinutes(4.0).TotalMilliseconds;
                    if (remainingDelay > batchSizeInMs)
                    {
                        Builder.AddDelay(batchSizeInMs);
                        Builder.AddClick(ScreenElementType.NextHeroPage);
                        
                        totalDelayAccountedFor += batchSizeInMs;
                        remainingDelay -= batchSizeInMs;
                    }
                    else
                    {
                        Builder.AddDelay(remainingDelay); //whatever's left thats < batchsize                        
                        totalDelayAccountedFor += remainingDelay;
                        remainingDelay = 0;
                    }
                }
            }
            else
            {
                deployDelay = new TimeSpan(0, 3, 3);
                Builder.AddDelay((int)deployDelay.TotalMilliseconds);
            }            

            int restockingDelay = deployment.Count() * 2750;

            Builder.AddClick(ScreenElementType.JetBay1)
                   .AddDelay()
                   .AddClick(ScreenElementType.CollectAll)
                   .AddDelay(restockingDelay);

            return Builder.GetMacro();
        }

        private void SetupSlotLookup(IEnumerable<ScreenElement> screenElements)
        {
            if (SlotLookup != null)
                return;

            SlotLookup = new Dictionary<int, ScreenElementType>()
            {
                { 1, ScreenElementType.HeroSlot1 },
                { 2, ScreenElementType.HeroSlot2 },
                { 3, ScreenElementType.HeroSlot3 },
                { 4, ScreenElementType.HeroSlot4 },
                { 5, ScreenElementType.HeroSlot5 }
            };
        }

        private void SetupBayLookup(IEnumerable<ScreenElement> screenElements)
        {
            if (BayLookup != null)
                return;

            BayLookup = new Dictionary<int, ScreenElementType>()
            {
                { 1, ScreenElementType.JetBay1 },
                { 2, ScreenElementType.JetBay2 },
                { 3, ScreenElementType.JetBay3 },
                { 4, ScreenElementType.JetBay4 },
                { 5, ScreenElementType.JetBay5 },
                { 6, ScreenElementType.JetBay6 },
                { 7, ScreenElementType.JetBay7 },
                { 8, ScreenElementType.JetBay8 }
            };
        }

        private ScreenElementType GetBayFor(int bayNumber)
        {
            if (bayNumber < 1 || bayNumber > 8)
                throw new InvalidOperationException("Only bays 1 - 8 are defined");

            return BayLookup[bayNumber];
        }

        private ScreenElementType GetSlotFor(int slotNumber)
        {
            if (slotNumber < 1 || slotNumber > 5)
                throw new InvalidOperationException("Only slots 1 - 5 are defined");

            return SlotLookup[slotNumber];
        }
    }
}
