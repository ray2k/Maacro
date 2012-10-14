using Maacro.Model;
using Maacro.Services;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using NinjectAdapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace Maacro.Infrastructure
{
    public static class BootStrapper
    {
        internal static IKernel Kernel { get; private set; }

        public static void Initialize()
        {
            InitializeDependencies();
            
            InitializeMacroData();
        }

        public static void Shutdown()
        {
            Kernel.Dispose();
        }

        public static void InitializeDependencies()
        {
            Kernel = new StandardKernel();
            Kernel.Load(new MaacroModule());

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(Kernel));
        }

        internal static void InitializeMacroData()
        {
            var storage = ServiceLocator.Current.GetInstance<IMacroDataStorage>();
            var saved = storage.Load() ?? MacroData.CreateDefault();

            MacroData.SetCurrent(saved);            

            ServiceLocator.Current.GetInstance<MacroDataPersistenceService>().Attach(MacroData.Current);
        }
    }
}
