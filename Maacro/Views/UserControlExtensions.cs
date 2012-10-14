using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace Maacro.Views
{
    public static class UserControlExtensions
    {
        public static void SetRuntimeBackground(this UserControl source)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(source))
                source.Background = Brushes.Transparent;
        }
    }
}
