using Maacro.Model;
using System;
using System.Reactive.Concurrency;

namespace Maacro.Services
{
    public interface IMacroDataPersistenceService : IDisposable
    {
        void Attach(MacroData macroData);
    }
}
