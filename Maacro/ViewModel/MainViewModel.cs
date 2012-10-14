using Maacro.Model;
using Maacro.ViewModel.Messaging;
using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using System.Linq;
using Maacro.Services;
using System.Diagnostics;

namespace Maacro.ViewModel
{
    public class MainViewModel : MaacroViewModel
    {
        private string _ApplicationTitle = "Maacro";
        public bool _EditingEnabled = true;
        private readonly MaacroViewModel _configurationVm;
        private readonly MaacroViewModel _buildingVm;
        private readonly MaacroViewModel _playbackVm;
        private readonly IMacroDataStorage _macroStorage;
        private MaacroViewModel _CurrentViewModel;        

        public MainViewModel(ConfigurationViewModel configurationViewModel, BuildingViewModel buildingViewModel, PlaybackViewModel playbackViewModel, IMacroDataStorage macroStorage)
        {
            _configurationVm = configurationViewModel;
            _buildingVm = buildingViewModel;
            _playbackVm = playbackViewModel;
            _macroStorage = macroStorage;
            
            CurrentViewModel = _configurationVm;
            CurrentViewModel.Activate();

            var v = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).OfType<AssemblyFileVersionAttribute>().FirstOrDefault();
            if (v != null)
                this.ApplicationTitle = string.Concat("Maacro v", v.Version);

            this.GotoConfiguration = ReactiveCommand.Create(x => true, o =>
                {
                    this.CurrentViewModel.Deactivate();
                    this.CurrentViewModel = _configurationVm;
                    this.CurrentViewModel.Activate();

                });

            this.GotoBuilding = ReactiveCommand.Create(x => true, o =>
                {
                    this.CurrentViewModel.Deactivate();
                    this.CurrentViewModel = _buildingVm;
                    this.CurrentViewModel.Activate();
                });


            this.GotoPlayback = ReactiveCommand.Create(x => true, o =>
                {
                    this.CurrentViewModel.Deactivate();
                    this.CurrentViewModel = _playbackVm;
                    this.CurrentViewModel.Activate();
                });

            this.MessageBus.Listen<PlaybackStartedMessage>().Subscribe(msg =>
                {
                    this.EditingEnabled = false;
                }
            );

            this.MessageBus.Listen<PlaybackStoppedMessage>().Subscribe(msg =>
                {
                    this.EditingEnabled = true;
                }
            );
        }

        public MaacroViewModel CurrentViewModel
        {
            get { return _CurrentViewModel; }
            set 
            {
                if (value != _CurrentViewModel)
                    _CurrentViewModel = this.RaiseAndSetIfChanged(vm => vm.CurrentViewModel, value); 
            }
        }

        public string ApplicationTitle
        {
            get { return _ApplicationTitle; }
            set { _ApplicationTitle = this.RaiseAndSetIfChanged(vm => vm.ApplicationTitle, value); }
        }

        public bool EditingEnabled
        {
            get { return _EditingEnabled; }
            set { _EditingEnabled = this.RaiseAndSetIfChanged(vm => vm.EditingEnabled, value); }
        }

        public ICommand GotoConfiguration { get; set; }
        public ICommand GotoBuilding { get; set; }
        public ICommand GotoPlayback { get; set; }
        public ICommand SaveMacroData
        {
            get
            {
                var saveCmd = new ReactiveCommand();
                saveCmd.Subscribe(p =>
                {
                    try
                    {
                        this._macroStorage.Save(MacroData.Current);
                    }
                    catch
                    {
                    }
                });
                return saveCmd;
            }
        }
    }
}