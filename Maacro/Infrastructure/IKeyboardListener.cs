using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Maacro.Infrastructure
{
    public interface IKeyboardListener : IInputListener
    {
        event KeyEventHandler KeyUp;        
    }
}
