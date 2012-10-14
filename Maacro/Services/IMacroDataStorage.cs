using Maacro.Model;
using System;
using System.Threading.Tasks;

namespace Maacro.Services
{
    public interface IMacroDataStorage
    {
        MacroData Load();
        void Save(MacroData macroData);
        Task SaveAsync(MacroData macroData);
    }
}
