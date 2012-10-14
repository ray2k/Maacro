using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maacro.ViewModel
{
    public abstract class MaacroViewModel : ReactiveObject
    {
        public bool IsActive { get; private set; }

        protected IMessageBus MessageBus
        {
            get
            {
                return ReactiveUI.MessageBus.Current;
            }
        }

        public void Activate()
        {
            this.IsActive = true;
            this.OnActivated();
        }

        protected virtual void OnActivated()
        {
        }

        protected virtual void OnDeactivated()
        {
        }

        public void Deactivate()
        {
            this.IsActive = false;
            this.OnDeactivated();
        }
    }
}
