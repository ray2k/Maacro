using Maacro.Infrastructure;
using Maacro.Model;
using Maacro.Services;
using Maacro.ViewModel;
using Moq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using System.Reactive.Concurrency;
using System.Windows.Forms;

namespace Maacro.Tests.ViewModel
{
    public class PlaybackViewModelTests
    {
        public Mock<IMacroPlayer> MockPlayer { get; set; }
        public Mock<IMacroGenerator> MockGenerator { get; set; }
        public Mock<IMacroDataValidator> MockValidator { get; set; }
        public Mock<IKeyboardListener> MockListener { get; set; }

        public PlaybackViewModelTests()
        {
            MockPlayer = new Mock<IMacroPlayer>();
            MockGenerator = new Mock<IMacroGenerator>();
            MockValidator = new Mock<IMacroDataValidator>();
            MockListener = new Mock<IKeyboardListener>();
        }

        [Fact]
        public void PlaybackViewModel_Should_Stop_Playback_When_Escape_Key_Is_Pressed()
        {
            MacroData.SetCurrent(MacroData.CreateDefault().AsValid());
            var vm = CreateInstance();
            vm.MacroSteps.Add(new MacroClickStep());

            new TestScheduler().With(ts =>
                {
                    vm.StartPlayback.Execute(null);
                }
            );

            vm.PlaybackEnabled.ShouldBe(false);
            MockListener.Raise(p => p.KeyUp += null, new KeyEventArgs(Keys.Escape));
            vm.PlaybackEnabled.ShouldBe(true);

            MockPlayer.VerifyAll();
            MockListener.VerifyAll();
        }

        [Fact]
        public void PlaybackViewModel_Should_Play_Macro_And_Disable_Playback()
        {
            MacroData.SetCurrent(MacroData.CreateDefault().AsValid());
            var vm = CreateInstance();
            vm.MacroSteps.Add(new MacroClickStep());

            MockPlayer.Setup(p => p.Play(vm.MacroSteps));

            new TestScheduler().With(ts =>
                {
                    vm.StartPlayback.Execute(null);
                }
            );

            vm.PlaybackEnabled.ShouldBe(false);

            MockPlayer.VerifyAll();
        }

        [Fact]
        public void PlaybackViewModel_Should_Enable_Playback_When_Macro_Is_Valid_When_Activated()
        {
            MacroData.SetCurrent(MacroData.CreateDefault().AsValid());
            var vm = CreateInstance();            

            MockValidator.Setup(p => p.Validate(MacroData.Current)).Returns(new List<string>());
            MockGenerator.Setup(p => p.GenerateMacro(
                                MacroData.Current.Deployment, 
                                MacroData.Current.ScreenElements, 
                                MacroData.Current.UIDelay, 
                                MacroData.Current.HeroPageCount,
                                MacroData.Current.Length
                                )).Returns(
                new List<MacroStep>() { new MacroClickStep() }
            );
            
            vm.Activate();

            vm.PlaybackEnabled.ShouldBe(true);
            vm.MacroSteps.Count.ShouldBe(1);
            vm.ErrorVisibility.ShouldBe(System.Windows.Visibility.Collapsed);
            vm.ValidationErrors.Count.ShouldBe(0);

            MockPlayer.VerifyAll();
            MockGenerator.VerifyAll();
        }

        [Fact]
        public void PlaybackViewModel_Should_Disable_Playback_When_Macro_Is_Valid_When_Activated()
        {
            MacroData.SetCurrent(MacroData.CreateDefault().AsValid());
            var vm = CreateInstance();            

            MockValidator.Setup(p => p.Validate(MacroData.Current)).Returns(new List<string>()
                {
                    "some error"
                }
            );

            vm.Activate();

            vm.PlaybackEnabled.ShouldBe(false);
            vm.MacroSteps.Count.ShouldBe(0);
            vm.ErrorVisibility.ShouldBe(System.Windows.Visibility.Visible);
            vm.ValidationErrors.Count.ShouldBe(1);
            vm.ValidationErrors.ShouldContain("some error");

            MockValidator.VerifyAll();
        }

        protected PlaybackViewModel CreateInstance()
        {
            MockPlayer.SetupGet(p => p.IterationStarted).Returns(new ScheduledSubject<int>(RxApp.DeferredScheduler));
            MockPlayer.SetupGet(p => p.StepCompleted).Returns(new ScheduledSubject<int>(RxApp.DeferredScheduler));
            MockPlayer.SetupGet(p => p.StepProgressUpdated).Returns(new ScheduledSubject<int>(RxApp.DeferredScheduler));
            MockPlayer.SetupGet(p => p.StepStarted).Returns(new ScheduledSubject<StepStartedInfo>(RxApp.DeferredScheduler));

            var vm = new PlaybackViewModel(MockGenerator.Object, MockValidator.Object, MockPlayer.Object, MockListener.Object);

            return vm;
        }
    }
}
