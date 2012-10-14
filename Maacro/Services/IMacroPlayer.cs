using Maacro.Model;
using System;
using System.Collections.Generic;

namespace Maacro.Services
{
    public class StepStartedInfo
    {
        public StepStartedInfo(int stepNumber, int progressChunks)
        {
            this.StepNumber = stepNumber;
            this.ProgressChunks = progressChunks;
        }

        public int StepNumber { get; private set; }
        public int ProgressChunks { get; private set; }
    }

    public interface IMacroPlayer
    {
        void Play(IEnumerable<MacroStep> macro);
        void Stop();

        IObservable<int> IterationStarted { get; }
        IObservable<int> StepCompleted { get; }
        IObservable<StepStartedInfo> StepStarted { get; }
        IObservable<int> StepProgressUpdated { get; }
    }
}
