using Maacro.Services;
using Maacro.ViewModel;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maacro.Infrastructure
{
    public class MaacroModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMacroDataPersistenceService>().To<MacroDataPersistenceService>().InSingletonScope();
            Bind<IMacroDataStorage>().To<MacroDataStorage>().InSingletonScope();            
            Bind<PlaybackViewModel>().ToSelf().InSingletonScope();
            Bind<ConfigurationViewModel>().ToSelf().InSingletonScope();
            Bind<BuildingViewModel>().ToSelf().InSingletonScope();
            Bind<MainViewModel>().ToSelf().InSingletonScope();
            Bind<KeyboardHookListener>().ToSelf().InSingletonScope();
            Bind<Hooker>().To<GlobalHooker>().InTransientScope();
            Bind<IMacroGenerator>().To<MacroGenerator>().InTransientScope();
            Bind<IMacroBuilder>().To<MacroBuilder>().InTransientScope();
            Bind<IMacroPlayer>().To<MacroPlayer>().InTransientScope();
            Bind<IMacroDataValidator>().To<MacroDataValidator>().InSingletonScope();
            Bind<IKeyboardListener>().To<KeyboardListenerAdapter>().InTransientScope();
            Bind<IMouseListener>().To<MouseListenerAdapter>().InTransientScope();
        }
    }
}
