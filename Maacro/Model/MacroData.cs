using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace Maacro.Model
{
    public class MacroData : ReactiveObject
    {
        private int _UIDelay = 0;
        private int _HeroPageCount = 0;        
        private ReactiveCollection<ScreenElement> _ScreenElements;
        private ReactiveCollection<DeploymentSlot> _Deployment;
        private static object _lock = new object();

        public static void SetCurrent(MacroData currentData)
        {
            lock (_lock)
            {
                MacroData.Current = currentData;
            }
        }

        public static MacroData Current { get; private set; }

        public static MacroData CreateDefault()
        {
            return new MacroData(ScreenElement.GetDefaults(), new DeploymentSlot[0], 1500, 1);
        }

        public MacroData(IEnumerable<ScreenElement> elements, IEnumerable<DeploymentSlot> deployment, int uiDelay, int heroPageCount)
        {
            _Deployment = new ReactiveCollection<DeploymentSlot>(deployment);
            _ScreenElements = new ReactiveCollection<ScreenElement>(elements);
            _Deployment.ChangeTrackingEnabled = true;
            _ScreenElements.ChangeTrackingEnabled = true;
            _UIDelay = uiDelay;
            _HeroPageCount = heroPageCount;
        }

        public MacroData()
            : this(new ScreenElement[0], new DeploymentSlot[0], 1500, 1)
        {   
        }

        public ReactiveCollection<ScreenElement> ScreenElements 
        {
            get { return _ScreenElements; }
        }

        public ReactiveCollection<DeploymentSlot> Deployment
        {
            get { return _Deployment; }
        }
        
        public int UIDelay
        {
            get { return _UIDelay; }
            set { _UIDelay = this.RaiseAndSetIfChanged(md => md.UIDelay, value); }
        }

        public int HeroPageCount
        {
            get { return _HeroPageCount; }
            set { _HeroPageCount = this.RaiseAndSetIfChanged(md => md.HeroPageCount, value); }
        }
    }
}
