using Maacro.Model;
using Maacro.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using ReactiveUI;
using Maacro.ViewModel.Messaging;
using Microsoft.Reactive.Testing;
using System.Reactive.Concurrency;
using ReactiveUI.Testing;
using ReactiveUI.Xaml;

namespace Maacro.Tests.ViewModel
{
    public class BuildingViewModelTests
    {
        [Fact]
        public void BuildingViewModel_Should_Allow_Adding_To_Deployment_When_Deployment_Is_Incomplete()
        {
            var md = MacroData.CreateDefault();
            md.Deployment.Add(new DeploymentSlot());
            MacroData.SetCurrent(md);  

            BuildingViewModel bvm = new BuildingViewModel();
            bvm.Activate();
            bvm.CanAddToDeployment.ShouldBe(true);
            bvm.Deactivate();
        }

        [Fact]
        public void BuildingViewModel_Should_Disallow_Adding_To_Deployment_When_Deployment_Is_Complete()
        {
            var md = MacroData.CreateDefault();
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            MacroData.SetCurrent(md);

            BuildingViewModel bvm = new BuildingViewModel();
            bvm.Activate();
            bvm.CanAddToDeployment.ShouldBe(false);
            bvm.Deactivate();
        }

        [Fact]
        public void BuildingViewModel_Should_Update_Cached_HeroPageCount_When_TotalPages_Changes()
        {
            var md = MacroData.CreateDefault();
            md.Deployment.Add(new DeploymentSlot());
            MacroData.SetCurrent(md);

            BuildingViewModel bvm = new BuildingViewModel();
            bvm.Activate();
            bvm.TotalPages = 9;
            bvm.Deactivate();
            MacroData.Current.HeroPageCount.ShouldBe(9);
        }

        [Fact]
        public void BuildingViewModel_Should_Remove_From_Deployment_When_RemoveSelectedFromDeploymentCommand_Received_From_Bus()
        {
            var md = MacroData.CreateDefault();
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            MacroData.SetCurrent(md);

            var selected = md.Deployment[0];

            BuildingViewModel bvm = new BuildingViewModel();
            bvm.Activate();
            bvm.SelectedDeploymentSlot = selected;

            bvm.CanAddToDeployment.ShouldBe(false);
            bvm.DeploymentSlots.Count.ShouldBe(8);

            MessageBus.Current.SendMessage<RemoveSelectedFromDeploymentCommand>(new RemoveSelectedFromDeploymentCommand());                            

            md.Deployment.Count.ShouldBe(7);
            md.Deployment.ShouldNotContain(selected);
            bvm.CanAddToDeployment.ShouldBe(true);
            bvm.Deactivate();
        }

        [Fact]
        public void BuildingViewModel_Should_Add_Selected_To_Deployment_On_AddToDeployment_Command_Executed()
        {
            var md = MacroData.CreateDefault();
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            md.Deployment.Add(new DeploymentSlot());
            MacroData.SetCurrent(md);

            BuildingViewModel bvm = null;

            new TestScheduler().With(scheduler =>
            {
                scheduler.Start();
                bvm = new BuildingViewModel();
                bvm.Activate();
                bvm.SelectedPageNumber = 1;
                bvm.SelectedSlotNumber = 2;
                (bvm.AddToDeployment as ReactiveCommand).Execute(new object());
                scheduler.AdvanceByMs(5100);
            });                
            
            MacroData.Current.Deployment.Count.ShouldBe(8);
            MacroData.Current.Deployment.Count(p => p.PageNumber == 1 && p.SlotNumber == 2).ShouldBe(1);
            bvm.CanAddToDeployment.ShouldBe(false);
        }
    }
}
