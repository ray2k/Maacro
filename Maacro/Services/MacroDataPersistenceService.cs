using Maacro.Model;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Maacro.Services
{
    public class MacroDataPersistenceService : IMacroDataPersistenceService
    {
        private readonly IMacroDataStorage _storage;
        private IDisposable _subscription;

        public MacroDataPersistenceService(IMacroDataStorage storage)
        {
            _storage = storage;
        }

        public void Attach(MacroData macroData)
        {
            _subscription = Observable.Merge(RxApp.DeferredScheduler,
                    macroData.Changed,
                    macroData.Deployment.Changed,
                    macroData.ScreenElements.Changed)                    
                    .Throttle(TimeSpan.FromSeconds(5.0), RxApp.DeferredScheduler)
                    .Subscribe(p =>
                        {
                            _storage.Save(macroData);
                        }
                    );
        }

        public void Dispose()
        {
            if (_subscription != null)
                _subscription.Dispose();
        }
    }
}