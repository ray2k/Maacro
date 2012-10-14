using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Maacro.Model
{
    public class DeploymentSlot : ReactiveObject
    {
        private int _PageNumber;
        private int _SlotNumber;

        public DeploymentSlot()
        {
        }

        public DeploymentSlot(int pageNumber, int slotNumber)
        {
            this.PageNumber = pageNumber;
            this.SlotNumber = slotNumber;
        }

        public int PageNumber
        {
            get { return _PageNumber; }
            set { _PageNumber = this.RaiseAndSetIfChanged(ds => ds.PageNumber, value); }
        }

        public int SlotNumber
        {
            get { return _SlotNumber; }
            set { _SlotNumber = this.RaiseAndSetIfChanged(ds => ds.SlotNumber, value); }
        }
    }
}
