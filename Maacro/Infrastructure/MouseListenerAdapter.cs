using MouseKeyboardActivityMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Maacro.Infrastructure
{
    public class MouseListenerAdapter : IMouseListener
    {
        private MouseHookListener _source;

        public MouseListenerAdapter(MouseHookListener source)
        {
            _source = source;
            _source.MouseMoveExt += OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventExtArgs e)
        {
            if (MouseMove != null)
                MouseMove(sender, e);
        }

        public event EventHandler<MouseEventArgs> MouseMove;

        public void Start()
        {
            _source.Start();
        }

        public void Stop()
        {
            _source.Stop();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
