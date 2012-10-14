using Maacro.Model;
using Maacro.ViewModel;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;
using Shouldly;
using Maacro.Infrastructure;

namespace Maacro.Tests.ViewModel
{
    public class ConfigurationViewModelTests
    {
        public Mock<IMouseListener> MockMouseListener;
        public Mock<IKeyboardListener> MockKeyboardListener;

        public ConfigurationViewModelTests()
        {
            MockMouseListener = new Mock<IMouseListener>();
            MockKeyboardListener = new Mock<IKeyboardListener>();
        }

        [Fact]
        public void ConfigurationViewModel_Should_Apply_Mouse_Position_To_Selected_ScreenElement_When_F2_Is_Pressed()
        {
            MacroData.SetCurrent(MacroData.CreateDefault());

            var vm = new ConfigurationViewModel(MockMouseListener.Object, MockKeyboardListener.Object);
            vm.CurrentMouseXPosition = 1;
            vm.CurrentMouseYPosition = 2;
            vm.SelectedScreenElement = new ScreenElement();

            MockKeyboardListener.Setup(p => p.Start());
            MockKeyboardListener.Setup(p => p.Stop());
            vm.Activate();
            MockKeyboardListener.Raise(p => p.KeyUp += null, new KeyEventArgs(Keys.F2));
            vm.Deactivate();            

            vm.SelectedScreenElement.X.ShouldBe(1);
            vm.SelectedScreenElement.Y.ShouldBe(2);
            MockKeyboardListener.VerifyAll();
        }

        [Fact]
        public void ConfigurationViewModel_Should_Update_Mouse_Position_When_It_Moves()
        {
            MacroData.SetCurrent(MacroData.CreateDefault());

            var vm = new ConfigurationViewModel(MockMouseListener.Object, MockKeyboardListener.Object);
            vm.CurrentMouseXPosition = 1;
            vm.CurrentMouseYPosition = 2;
            vm.SelectedScreenElement = new ScreenElement();

            MockMouseListener.Setup(p => p.Start());
            MockMouseListener.Setup(p => p.Stop());
            vm.Activate();
            MockMouseListener.Raise(p => p.MouseMove += null, new MouseEventArgs(MouseButtons.Left, 2, 4, 5, 0));
            vm.Deactivate();

            vm.CurrentMouseXPosition.ShouldBe(4);
            vm.CurrentMouseYPosition.ShouldBe(5);
            MockMouseListener.VerifyAll();
        }

        [Fact]
        public void ConfigurationViewModel_Should_Move_To_Previous_ScreenElement_When_F1_Pressed()
        {
            MacroData.SetCurrent(MacroData.CreateDefault());

            var vm = new ConfigurationViewModel(MockMouseListener.Object, MockKeyboardListener.Object);
            vm.CurrentMouseXPosition = 1;
            vm.CurrentMouseYPosition = 2;
            vm.SelectedScreenElement = MacroData.Current.ScreenElements[1];

            MockKeyboardListener.Setup(p => p.Start());
            MockKeyboardListener.Setup(p => p.Stop());
            vm.Activate();
            MockKeyboardListener.Raise(p => p.KeyUp += null, new KeyEventArgs(Keys.F1));
            vm.Deactivate();

            vm.SelectedScreenElement.ShouldBeSameAs(MacroData.Current.ScreenElements[0]);
            MockKeyboardListener.VerifyAll();
        }

        [Fact]
        public void ConfigurationViewModel_Should_Move_To_Next_ScreenElement_When_F3_Pressed()
        {
            MacroData.SetCurrent(MacroData.CreateDefault());

            var vm = new ConfigurationViewModel(MockMouseListener.Object, MockKeyboardListener.Object);
            vm.CurrentMouseXPosition = 1;
            vm.CurrentMouseYPosition = 2;
            vm.SelectedScreenElement = MacroData.Current.ScreenElements[0];

            MockKeyboardListener.Setup(p => p.Start());
            MockKeyboardListener.Setup(p => p.Stop());
            vm.Activate();
            MockKeyboardListener.Raise(p => p.KeyUp += null, new KeyEventArgs(Keys.F3));
            vm.Deactivate();

            vm.SelectedScreenElement.ShouldBeSameAs(MacroData.Current.ScreenElements[1]);
            MockKeyboardListener.VerifyAll();
        }
    }
}
