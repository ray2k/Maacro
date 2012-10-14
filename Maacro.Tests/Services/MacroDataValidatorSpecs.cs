using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Bddify;
using Shouldly;
using Maacro.Model;
using Maacro.Services;

namespace Maacro.Tests.Services
{
    public abstract class MacroDataValidatorSpec
    {
        public MacroData TestMacro;
        public IList<string> Errors;
    }

    public class ValidatingAnEmptyMacro : MacroDataValidatorSpec
    {
        public void Given_some_macro_data()
        {
            TestMacro = MacroData.CreateDefault();
        }

        public void When_it_does_not_have_a_deployment_defined()
        {            
        }

        public void And_when_it_does_not_have_required_screen_elements_defined()
        {
        }

        public void Then_it_should_fail_validation()
        {
            Errors = new MacroDataValidator().Validate(TestMacro);
            Errors.ShouldNotBe(null);
            Errors.ShouldContain("Minimal deployment order has not been set");
            Errors.ShouldContain("Screen element 'Collect All' does not have a mouse location defined");
            Errors.ShouldContain("Screen element 'Confirm Button' does not have a mouse location defined");            
        }

        [Fact]
        public void MacroData_Requires_Deployment_And_ScreenElements_To_Be_Valid()
        {
            this.Bddify();
        }
    }

    public class ValidatingAnIncompleteMacro : MacroDataValidatorSpec
    {
        public void Given_some_macro_data()
        {
            TestMacro = MacroData.CreateDefault();
        }

        public void When_it_has_a_deployment_defined()
        {
            TestMacro.Deployment.Add(new DeploymentSlot() { PageNumber = 3, SlotNumber = 3 });
            TestMacro.HeroPageCount = 4;
        }

        public void And_when_it_has_general_screen_elements_defined()
        {
            var lookup = TestMacro.ScreenElements.ToDictionary(p => p.ElementType);

            lookup[ScreenElementType.NextHeroPage].X = 9;
            lookup[ScreenElementType.PrevHeroPage].X = 9;
            lookup[ScreenElementType.CollectAll].X = 9;
            lookup[ScreenElementType.ConfirmButton].X = 9;

            lookup[ScreenElementType.NextHeroPage].Y = 9;
            lookup[ScreenElementType.PrevHeroPage].Y = 9;
            lookup[ScreenElementType.CollectAll].Y = 9;
            lookup[ScreenElementType.ConfirmButton].Y = 9;
        }

        public void And_when_it_does_not_have_screen_elements_to_support_the_hero_deployment()
        {
            // no op: will not have valid X,Y for JetBay1, HeroSlot3
        }

        public void Then_it_should_fail_validation()
        {
            Errors = new MacroDataValidator().Validate(TestMacro);
            Errors.ShouldNotBe(null);
            Errors.ShouldContain("Screen element 'Jet Bay 1' does not have a mouse location defined");
            Errors.ShouldContain("Screen element 'Hero Slot 3' does not have a mouse location defined");
        }

        [Fact]
        public void MacroData_Requires_Deployment_And_ScreenElements_To_Be_Valid()
        {
            this.Bddify();
        }
    }
}
