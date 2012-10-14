/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Maacro"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Maacro.Infrastructure;
using Maacro.Model;
using Maacro.Services;
using Microsoft.Practices.ServiceLocation;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Windows;

namespace Maacro.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            if (!DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow))
                BootStrapper.Initialize();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ConfigurationViewModel Configuration
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConfigurationViewModel>();
            }
        }

        public PlaybackViewModel Playback
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlaybackViewModel>();
            }
        }

        public BuildingViewModel Building
        {
            get
            {
                return ServiceLocator.Current.GetInstance<BuildingViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            BootStrapper.Shutdown();
        }
    }
}