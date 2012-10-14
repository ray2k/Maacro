using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bddify;
using Shouldly;
using Xunit;
using Maacro.Model;
using Maacro.Services;
using Moq;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace Maacro.Tests.Services
{
    public class MacroDataPersistenceServiceSpecs
    {
        public MacroData TestMacroData;
        public MacroDataPersistenceService ServiceInstance;
        public Mock<IMacroDataStorage> MockStorage;

        public void Given_some_macro_data()
        {
            TestMacroData = new MacroData();
            TestMacroData.Deployment.Add(new DeploymentSlot());
            TestMacroData.ScreenElements.Add(new ScreenElement());
            TestMacroData.HeroPageCount = 100;
            TestMacroData.UIDelay = 100;

            MockStorage = new Mock<IMacroDataStorage>();
            MockStorage.Setup(p => p.Save(TestMacroData));

            ServiceInstance = new MacroDataPersistenceService(MockStorage.Object);
        }

        public void When_it_changes_content()
        {
            using (ServiceInstance)
            {
                new TestScheduler().With(scheduler => 
                    {
                        ServiceInstance.Attach(TestMacroData);      
                        scheduler.Start();                                          
                        TestMacroData.UIDelay = 9;
                        scheduler.AdvanceByMs(5100);
                    });
            }
        }

        public void Then_it_should_be_persisted_to_storage()
        {
            MockStorage.VerifyAll();
        }

        [Fact]
        public void MacroData_Should_Be_Persisted_When_Any_Change_Is_Observed()
        {
            this.Bddify();
        }
    }
}
