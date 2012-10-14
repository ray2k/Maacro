using Maacro.ViewModel.Messaging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Maacro.Views
{
    /// <summary>
    /// Interaction logic for BuildingView.xaml
    /// </summary>
    public partial class BuildingView : UserControl
    {
        public BuildingView()
        {
            InitializeComponent();
            this.SetRuntimeBackground();
        }

        private void lbDeploymentOrder_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                MessageBus.Current.SendMessage<RemoveSelectedFromDeploymentCommand>(new RemoveSelectedFromDeploymentCommand());
        }   
    }
}
