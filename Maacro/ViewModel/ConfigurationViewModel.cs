using Maacro.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using System.Windows;
using System.Collections.ObjectModel;
using ReactiveUI;
using Maacro.Infrastructure;

namespace Maacro.ViewModel
{
    public class ConfigurationViewModel : MaacroViewModel
    {
        private ScreenElement _SelectedScreenElement = null;
        private readonly IMouseListener _mouseListener;
        private readonly IKeyboardListener _keyboardListener;
        private int _CurrentMouseXPosition = 0;
        private int _CurrentMouseYPosition = 0;
        private string _HelpInfo;

        public ConfigurationViewModel(IMouseListener mouseListener, IKeyboardListener keyboardListener)
        {
            this.ScreenElements = MacroData.Current.ScreenElements;
            this._mouseListener = mouseListener;
            this._keyboardListener = keyboardListener;

            _mouseListener.MouseMove += OnMouseMove;
            _keyboardListener.KeyUp += OnKeyUp;

            this.SelectedScreenElement = this.ScreenElements.FirstOrDefault();
            
            this.HelpInfo = new StringBuilder()
                .AppendLine("Keyboard Shortcuts")
                .AppendLine("F1: \tSelect previous screen element")
                .AppendLine("F2: \tAssign current mouse position to selected screen element")                
                .AppendLine("F3: \tSelect next screen element")
                .AppendLine()
                .AppendLine("Screen Elements")
                .AppendLine("Jet Bay 1 - 8: \t\tpositions to select each jet bay")                
                .AppendLine("Hero Slot 1 - 5: \t\tpositions to select heroes in the Deploy screen")
                .AppendLine("Prev/Next Hero Page: \tthe left/right arrows on the Deploy screen")
                .AppendLine("Confirm Button: \t\tthe big Confirm button on the Deploy screen")
                .AppendLine("Collect All: \t\tthe Collect All button to collect when deploys are over.\r\nNote: this is assumed to become visible by clicking Jet Bay 1")
                .ToString();
        }

        public string HelpInfo
        {
            get { return _HelpInfo; }
            set { _HelpInfo = this.RaiseAndSetIfChanged(vm => vm.HelpInfo, value); }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.CurrentMouseXPosition = e.X;
            this.CurrentMouseYPosition = e.Y;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (SelectedScreenElement != null && e.KeyCode == Keys.F1)
            {
                // prev screen element
                var nextElement = this.ScreenElements.IndexOf(this.SelectedScreenElement);
                if (nextElement > 0)
                    this.SelectedScreenElement = this.ScreenElements[nextElement - 1];
            }

            if (SelectedScreenElement != null && e.KeyCode == Keys.F2)
            {
                this.SelectedScreenElement.X = this.CurrentMouseXPosition;
                this.SelectedScreenElement.Y = this.CurrentMouseYPosition;
            }           

            if (SelectedScreenElement != null && e.KeyCode == Keys.F3)
            {
                // next screen element
                var nextElement = this.ScreenElements.IndexOf(this.SelectedScreenElement);
                if (nextElement < (this.ScreenElements.Count - 1))
                    this.SelectedScreenElement = this.ScreenElements[nextElement + 1];
            }
        }

        protected override void OnActivated()
        {
            _mouseListener.Start();
            _keyboardListener.Start();
        }

        protected override void OnDeactivated()
        {
            _mouseListener.Stop();
            _keyboardListener.Stop();
        }

        public ReactiveCollection<ScreenElement> ScreenElements { get; private set; }

        public int CurrentMouseXPosition
        {
            get { return this._CurrentMouseXPosition; }
            set { this.RaiseAndSetIfChanged(vm => vm.CurrentMouseXPosition, value); }
        }

        public int CurrentMouseYPosition
        {
            get { return this._CurrentMouseYPosition; }
            set { this.RaiseAndSetIfChanged(vm => vm.CurrentMouseYPosition, value); }
        }        

        public ScreenElement SelectedScreenElement
        {
            get { return _SelectedScreenElement; }
            set { this.RaiseAndSetIfChanged(vm => vm.SelectedScreenElement, value); }
        }
    }
}
