using Maacro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading;
using Shouldly;

namespace Maacro.Tests
{
    public class MacroDataSpecs
    {
        public MacroData TestInstance = new MacroData();

        private void AssertNotification<T>(IObservable<T> observable, Action<MacroData> action)
        {
            bool notified = false;

            observable.Subscribe(item =>
                {
                    notified = true;
                }
            );

            action(TestInstance);

            notified.ShouldBe(true);
        }

        [Fact]
        public void MacroData_Should_Raise_Notifications_When_ObjectGraph_Changes()
        {
            MacroData md = new MacroData();

            AssertNotification(TestInstance.Changed, p => p.UIDelay = 100);
            AssertNotification(TestInstance.Changed, p => p.HeroPageCount = 100);
            
            AssertNotification(TestInstance.Deployment.Changed, p => p.Deployment.Add(new DeploymentSlot()));
            AssertNotification(TestInstance.Deployment.Changed, p => p.Deployment[0].PageNumber = 99);

            AssertNotification(TestInstance.ScreenElements.Changed, p => p.ScreenElements.Add(new ScreenElement()));
            AssertNotification(TestInstance.ScreenElements.Changed, p => p.ScreenElements[0].ElementType = ScreenElementType.HeroSlot3);
        }
    }
}
