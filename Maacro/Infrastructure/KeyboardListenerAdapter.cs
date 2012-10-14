using MouseKeyboardActivityMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Maacro.Infrastructure
{
    public class KeyboardListenerAdapter : IKeyboardListener
    {
        private KeyboardHookListener _source;

        public KeyboardListenerAdapter(KeyboardHookListener source)
        {
            this._source = source;
            this._source.KeyUp += OnKeyUp;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (this.KeyUp != null)
                this.KeyUp(_source, e);
        }

        public event KeyEventHandler KeyUp;        

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
