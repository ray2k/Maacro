using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Maacro.ViewModel
{
    // blatantly borrowed from http://www.codeproject.com/Tips/125583/ScrollIntoView-for-a-DataGrid-when-using-MVVM
    public class ScrollIntoViewBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid)
            {
                DataGrid grid = (sender as DataGrid);
                if (grid.SelectedItem != null)
                {
                    grid.Dispatcher.BeginInvoke(
                        new Action(
                            delegate()
                            {
                                if (grid.SelectedItem != null)
                                {
                                    grid.UpdateLayout();
                                    grid.ScrollIntoView(grid.SelectedItem, null);
                                }
                            }
                        )
                    );
                }
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -= new SelectionChangedEventHandler(AssociatedObject_SelectionChanged);
        }
    }
}
