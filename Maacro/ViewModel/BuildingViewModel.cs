using Maacro.Model;
using Maacro.ViewModel.Messaging;
using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace Maacro.ViewModel
{
    public class BuildingViewModel : MaacroViewModel
    {
        private int _SelectedPageNumber = 1;
        private int _SelectedSlotNumber = 1;        
        private DeploymentSlot _SelectedDeploymentSlot;
        private bool _CanAddToDeployment = false;
        private int _TotalPages = 1;        

        public BuildingViewModel()
        {
            this.DeploymentSlots = MacroData.Current.Deployment;
            this.TotalPages = MacroData.Current.HeroPageCount;
            this.CanAddToDeployment = false;
            this.SelectedSlotNumber = 1;
            this.SelectedPageNumber = 1;

            this.AddToDeployment = new ReactiveCommand(scheduler: RxApp.DeferredScheduler)
                .WithSubscription(o =>
                     {
                         this.DeploymentSlots.Add(
                             new DeploymentSlot(this.SelectedPageNumber, this.SelectedSlotNumber));

                         this.CanAddToDeployment = (this.DeploymentSlots.Count < 8);
                     }
             );

            // this command message comes from the view (usercontrol) itself
            this.MessageBus.Listen<RemoveSelectedFromDeploymentCommand>().Subscribe(msg =>
                {
                    if (this.SelectedDeploymentSlot != null)
                        this.DeploymentSlots.Remove(this.SelectedDeploymentSlot);

                    this.CanAddToDeployment = (this.DeploymentSlots.Count < 8);
                }
            );

            this.ObservableForProperty(vm => vm.TotalPages).Subscribe(oc =>
                {
                    MacroData.Current.HeroPageCount = oc.Value;
                }
            );

        }

        protected override void OnActivated()
        {
            this.CanAddToDeployment = this.DeploymentSlots.Count < 8;
        }

        public DeploymentSlot SelectedDeploymentSlot
        {
            get { return _SelectedDeploymentSlot; }
            set { _SelectedDeploymentSlot = this.RaiseAndSetIfChanged(vm => vm.SelectedDeploymentSlot, value); }
        }

        public int SelectedPageNumber
        {
            get { return _SelectedPageNumber; }
            set { _SelectedPageNumber = this.RaiseAndSetIfChanged(vm => vm.SelectedPageNumber, value); }
        }

        public int SelectedSlotNumber
        {
            get { return _SelectedSlotNumber; }
            set { _SelectedSlotNumber = this.RaiseAndSetIfChanged(vm => vm.SelectedSlotNumber, value); }
        }

        public bool CanAddToDeployment
        {
            get { return _CanAddToDeployment; }
            set { _CanAddToDeployment = this.RaiseAndSetIfChanged(vm => vm.CanAddToDeployment, value); }
        }

        public int TotalPages
        {
            get { return _TotalPages; }
            set 
            { 
                _TotalPages = this.RaiseAndSetIfChanged(vm => vm.TotalPages, value);
                
                if (SelectedPageNumber > TotalPages)
                    SelectedPageNumber = TotalPages;
            }
        }

        public ICommand AddToDeployment { get; set; }

        public ReactiveCollection<DeploymentSlot> DeploymentSlots { get; private set; }
    }
}
