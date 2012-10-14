using Maacro.Infrastructure;
using Maacro.Model;
using Maacro.Services;
using Maacro.ViewModel.Messaging;
using MouseKeyboardActivityMonitor;
using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace Maacro.ViewModel
{
    public class PlaybackViewModel : MaacroViewModel
    {
        private int _UIDelay;
        private bool _PlaybackEnabled;
        private Visibility _ErrorVisibility;
        private string _ValidationErrorText;
        private string _UIDelayText;
        private volatile bool _generating = false;
        private string _IterationText = "Progress";
        private int _StepNumber = 0;
        private int _TotalSteps = 0;
        private string _StepText;
        private int _StepProgress = 0;
        private int _CurrentStepChunkCount = 0;
        private MacroStep _CurrentStep;

        protected IMacroDataValidator Validator { get; private set; }
        protected IMacroGenerator Generator { get; private set; }
        protected IMacroPlayer Player { get; private set; }
        protected IKeyboardListener KeyboardListener { get; private set; }

        public PlaybackViewModel(IMacroGenerator macroGenerator, IMacroDataValidator validator, IMacroPlayer player, IKeyboardListener keyboardListener)
        {
            this.Validator = validator;
            this.Generator = macroGenerator;
            this.Player = player;
            this.KeyboardListener = keyboardListener;
            this.UIDelay = 2000;
            this.StepProgress = 0;
            this.CurrentStepChunkCount = 1;
            this.TotalSteps = 1;            

            this.KeyboardListener.KeyUp += OnKeyUp;

            this.StartPlayback = ReactiveCommand.Create(o => true, o =>
                {
                    if (this.MacroSteps.Count == 0)
                        return;

                    this.PlaybackEnabled = false;

                    this.Player.Play(this.MacroSteps);
                    MessageBus.SendMessage<PlaybackStartedMessage>(new PlaybackStartedMessage());
                }
            );
            
            this.MacroSteps = new ReactiveCollection<MacroStep>();
            this.ValidationErrors = new ReactiveCollection<string>();

            this.ValidationErrors.CollectionCountChanged.Subscribe(cnt =>
                {
                    if (cnt == 0)
                    {
                        this.PlaybackEnabled = true;
                        this.ValidationErrorText = null;                        
                    }
                    else
                    {
                        this.PlaybackEnabled = false;
                        this.ValidationErrorText = string.Join("\r\n", this.ValidationErrors.ToArray());
                    }                    
                }
            );

            this.ObservableForProperty(p => p.UIDelay, false)
                .Subscribe(ch =>
                    {
                        if (_generating)
                            return;
                        
                        TryMacroGeneration();
                    }
            );

            this.Player.IterationStarted.Subscribe(p =>
                {
                    if (IsPlaying)
                    {
                        this.TotalSteps = this.MacroSteps.Count;
                        this.StepNumber = 0;
                        this.IterationText = string.Format("Progress (Iteration #{0})", p);                        
                    }
                }
            );

            this.Player.StepStarted.Subscribe(p =>
                {
                    if (IsPlaying)
                    {
                        this.StepText = string.Format("Step #{0}", p.StepNumber);
                        this.CurrentStepChunkCount = p.ProgressChunks;
                        this.CurrentStep = this.MacroSteps[p.StepNumber - 1];
                    }
                }
            );

            this.Player.StepCompleted.Subscribe(p =>
                {
                    if (IsPlaying)
                        this.StepNumber++;
                }
            );

            this.Player.StepProgressUpdated.Subscribe(p =>
                {
                    if (IsPlaying)
                        this.StepProgress = p;
                }
            );
        }

        private void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Player.Stop();

                this.CurrentStep = this.MacroSteps[0];
                this.CurrentStep = null; // so it is scrolled to top
                this.StepNumber = 0;
                this.IterationText = "Progress";
                this.StepText = "Step #";
                this.StepProgress = 0;                
                this.CurrentStepChunkCount = 1;
                this.TotalSteps = 1;
                PlaybackEnabled = true;
                MessageBus.SendMessage<PlaybackStoppedMessage>(new PlaybackStoppedMessage());
            }
        }

        protected override void OnDeactivated()
        {
            this.KeyboardListener.Stop();
            base.OnDeactivated();
        }

        protected void TryMacroGeneration()
        {
            _generating = true;
            var errors = this.Validator.Validate(MacroData.Current);

            foreach (var e in errors)
                this.ValidationErrors.Add(e);

            if (errors.Count > 0)
            {
                this.ErrorVisibility = Visibility.Visible;
                this.MacroSteps.Clear();
            }
            else
            {
                this.ErrorVisibility = Visibility.Collapsed;
                this.MacroSteps.Clear();

                var stepList = this.Generator.GenerateMacro(MacroData.Current.Deployment, MacroData.Current.ScreenElements, MacroData.Current.UIDelay, MacroData.Current.HeroPageCount);
                foreach (var step in stepList)
                    this.MacroSteps.Add(step);

                this.TotalSteps = this.MacroSteps.Count;
            }
            _generating = false;
        }
        
        protected override void OnActivated()
        {
            this.KeyboardListener.Start();
            this.ValidationErrors.Clear();

            TryMacroGeneration();
        }

        public bool PlaybackEnabled
        {
            get { return _PlaybackEnabled; }
            set { _PlaybackEnabled = this.RaiseAndSetIfChanged(vm => vm.PlaybackEnabled, value); }
        }

        public int UIDelay
        {
            get { return _UIDelay; }
            set 
            { 
                _UIDelay = this.RaiseAndSetIfChanged(vm => vm.UIDelay, value);
                MacroData.Current.UIDelay = _UIDelay;
                
                var ts = TimeSpan.FromMilliseconds((double)value);
                UIDelayText = string.Format("{0} second(s)", ts.TotalSeconds);
            }
        }

        public string UIDelayText
        {
            get { return _UIDelayText; }
            set { _UIDelayText = this.RaiseAndSetIfChanged(vm => vm.UIDelayText, value); }
        }

        public string ValidationErrorText
        {
            get { return _ValidationErrorText; }
            set { _ValidationErrorText = this.RaiseAndSetIfChanged(vm => vm.ValidationErrorText, value); }
        }

        public string IterationText
        {
            get { return _IterationText; }
            set { _IterationText = this.RaiseAndSetIfChanged(vm => vm.IterationText, value); }
        }

        public int StepNumber
        {
            get { return _StepNumber; }
            set { _StepNumber = this.RaiseAndSetIfChanged(vm => vm.StepNumber, value); }
        }

        public int StepProgress
        {
            get { return _StepProgress; }
            set { _StepProgress = this.RaiseAndSetIfChanged(vm => vm.StepProgress, value); }
        }

        public int TotalSteps
        {
            get { return _TotalSteps; }
            set { _TotalSteps = this.RaiseAndSetIfChanged(vm => vm.TotalSteps, value); }
        }

        public string StepText
        {
            get { return _StepText; }
            set { _StepText = this.RaiseAndSetIfChanged(vm => vm.StepText, value); }
        }

        public int CurrentStepChunkCount
        {
            get { return _CurrentStepChunkCount; }
            set { _CurrentStepChunkCount = this.RaiseAndSetIfChanged(vm => vm.CurrentStepChunkCount, value); }
        }    

        public ICommand StartPlayback { get; set; }

        public Visibility ErrorVisibility
        {
            get { return _ErrorVisibility; }
            set { _ErrorVisibility = this.RaiseAndSetIfChanged(vm => vm.ErrorVisibility, value); }
        }

        public ReactiveCollection<string> ValidationErrors { get; private set; }

        public ReactiveCollection<MacroStep> MacroSteps { get; private set; }

        public MacroStep CurrentStep
        {
            get { return _CurrentStep; }
            set { _CurrentStep = this.RaiseAndSetIfChanged(vm => vm.CurrentStep, value); }
        }

        protected bool IsPlaying
        {
            get { return !PlaybackEnabled; }
        }
    }
}
