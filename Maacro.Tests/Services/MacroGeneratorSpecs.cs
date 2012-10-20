using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Bddify;
using Maacro.Services;
using Maacro.Model;
using Shouldly;

namespace Maacro.Tests.Services
{
    public class MacroGeneratorSpecs
    {
        public MacroGenerator Generator = new MacroGenerator();
        public List<MacroStep> MacroSteps = new List<MacroStep>();
        public List<DeploymentSlot> Deployment = new List<DeploymentSlot>();
        public List<ScreenElement> ScreenElements = new List<ScreenElement>();

        public void Given_A_Deployment_And_Set_Of_ScreenElements()
        {
            ScreenElements = ScreenElement.GetDefaults().ToList();
            Deployment = new List<DeploymentSlot>()
            {
                new DeploymentSlot() { PageNumber = 5, SlotNumber = 5 },
                new DeploymentSlot() { PageNumber = 3, SlotNumber = 2 },
                new DeploymentSlot() { PageNumber = 4, SlotNumber = 4 },
            };
        }

        public void When_A_Macro_Is_Generated()
        {
            MacroSteps = Generator.GenerateMacro(Deployment, ScreenElements, 1000, 6, DeployLength.ThreeMinute).ToList();
        }

        public void Then_The_Macro_Should_Deploy_All_Heroes_Accordingly()
        {
            MacroSteps.ShouldNotBe(null);

            var clicks = MacroSteps.OfType<MacroClickStep>().ToList();

            // click bay 1 and open
            clicks[0].ScreenElement.ShouldBe(ScreenElementType.JetBay1);
            clicks[1].ScreenElement.ShouldBe(ScreenElementType.JetBay1);

            // 4 prev clicks to get to page 1
            clicks[2].ScreenElement.ShouldBe(ScreenElementType.PrevHeroPage);
            clicks[3].ScreenElement.ShouldBe(ScreenElementType.PrevHeroPage);
            clicks[4].ScreenElement.ShouldBe(ScreenElementType.PrevHeroPage);
            clicks[5].ScreenElement.ShouldBe(ScreenElementType.PrevHeroPage);
            clicks[6].ScreenElement.ShouldBe(ScreenElementType.PrevHeroPage);

            // get to page 5
            clicks[7].ScreenElement.ShouldBe(ScreenElementType.NextHeroPage);
            clicks[8].ScreenElement.ShouldBe(ScreenElementType.NextHeroPage);
            clicks[9].ScreenElement.ShouldBe(ScreenElementType.NextHeroPage);
            clicks[10].ScreenElement.ShouldBe(ScreenElementType.NextHeroPage);

            // slot 5 and confirm
            clicks[11].ScreenElement.ShouldBe(ScreenElementType.HeroSlot5);
            clicks[12].ScreenElement.ShouldBe(ScreenElementType.ConfirmButton);

            // bay 2 and open
            clicks[13].ScreenElement.ShouldBe(ScreenElementType.JetBay2);
            clicks[14].ScreenElement.ShouldBe(ScreenElementType.JetBay2);

            // go to page 3
            clicks[15].ScreenElement.ShouldBe(ScreenElementType.NextHeroPage);
            clicks[16].ScreenElement.ShouldBe(ScreenElementType.NextHeroPage);

            // slot 2 and confirm
            clicks[17].ScreenElement.ShouldBe(ScreenElementType.HeroSlot2);
            clicks[18].ScreenElement.ShouldBe(ScreenElementType.ConfirmButton);

            // bay 3 and open
            clicks[19].ScreenElement.ShouldBe(ScreenElementType.JetBay3);
            clicks[20].ScreenElement.ShouldBe(ScreenElementType.JetBay3);

            // go to page 4
            clicks[21].ScreenElement.ShouldBe(ScreenElementType.NextHeroPage);
            clicks[22].ScreenElement.ShouldBe(ScreenElementType.NextHeroPage);
            clicks[23].ScreenElement.ShouldBe(ScreenElementType.NextHeroPage);

            // slot 4 and confirm
            clicks[24].ScreenElement.ShouldBe(ScreenElementType.HeroSlot4);
            clicks[25].ScreenElement.ShouldBe(ScreenElementType.ConfirmButton);

            clicks[26].ScreenElement.ShouldBe(ScreenElementType.JetBay1);
            clicks[27].ScreenElement.ShouldBe(ScreenElementType.CollectAll);

            clicks.Count.ShouldBe(28);
        }

        [Fact]
        public void Generated_Macros_Should_Deploy_According_To_MacroData_Definition()
        {
            this.Bddify();
        }
    }
}
