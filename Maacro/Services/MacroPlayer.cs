using Maacro.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maacro.Services
{
    public class MacroPlayer : IMacroPlayer
    {
        private Task PlaybackTask { get; set; }
        private ManualResetEvent WaitHandle = new ManualResetEvent(false);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;
        
        private static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        public MacroPlayer()
        {
            _iterationStartedSubject = new ScheduledSubject<int>(RxApp.DeferredScheduler);
            _stepCompletedSubject = new ScheduledSubject<int>(RxApp.DeferredScheduler);
            _stepStartedSubject = new ScheduledSubject<StepStartedInfo>(RxApp.DeferredScheduler);
            _stepProgressUpdatedSubject = new ScheduledSubject<int>(RxApp.DeferredScheduler);
        }

        public void Play(IEnumerable<MacroStep> macro)
        {
            PlaybackTask = new Task(() => PlayMacro(macro));
            PlaybackTask.Start();
        }

        public void Stop()
        {
            WaitHandle.Set();

            if (PlaybackTask != null)
                PlaybackTask.Wait();

            WaitHandle.Reset();
        }

        private ISubject<int> _iterationStartedSubject;
        private ISubject<int> _stepCompletedSubject;
        private ISubject<StepStartedInfo> _stepStartedSubject;
        private ISubject<int> _stepProgressUpdatedSubject;

        public IObservable<int> IterationStarted
        {
            get
            {
                return _iterationStartedSubject;
            }
        }

        public IObservable<int> StepCompleted
        {
            get
            {
                return _stepCompletedSubject;
            }
        }

        public IObservable<StepStartedInfo> StepStarted
        {
            get
            {
                return _stepStartedSubject;
            }
        }

        public IObservable<int> StepProgressUpdated
        {
            get
            {
                return _stepProgressUpdatedSubject;
            }
        }

        private void PlayMacro(IEnumerable<MacroStep> macro)
        {
            bool breakout = false;

            int iteration = 1;
            int stepNumber = 1;

            while (true)
            {
                _iterationStartedSubject.OnNext(iteration);
                stepNumber = 1;

                foreach (var step in macro)
                {
                    if (WaitHandle.WaitOne(1))
                    {
                        breakout = true;
                        break;
                    }

                    if (step is MacroClickStep)
                        Play(step as MacroClickStep, stepNumber);

                    if (step is MacroDelayStep)
                        Play(step as MacroDelayStep, stepNumber);

                    stepNumber++;
                }

                iteration++;                

                if (breakout == true)
                    break;
            }
        }

        private void Play(MacroDelayStep macroDelayStep, int stepNumber)
        {
            int totalHalfSecondWaits = (int) Math.Floor( (double) macroDelayStep.Delay / 500.0);
            int lastRemainderWait = (macroDelayStep.Delay % 500);
            bool skipLastWait = false;
            int totalWaits = 0;

            if (lastRemainderWait == 0)
                totalWaits = totalHalfSecondWaits;
            else
                totalHalfSecondWaits = totalHalfSecondWaits++;

            _stepStartedSubject.OnNext(new StepStartedInfo(stepNumber, totalWaits));

            for (int i = 0; i < totalHalfSecondWaits; i++)
            {
                if (WaitHandle.WaitOne(1))
                {
                    skipLastWait = true;
                    break;
                }
                
                _stepProgressUpdatedSubject.OnNext(i + 1);                
                Thread.Sleep(500);
            }

            if (!skipLastWait)
            {
                _stepProgressUpdatedSubject.OnNext(totalWaits);
                Thread.Sleep(lastRemainderWait);
            }
            
            _stepCompletedSubject.OnNext(stepNumber);
        }

        private void Play(MacroClickStep macroClickStep, int stepNumber)
        {
            if (WaitHandle.WaitOne(1))
                return;

            _stepStartedSubject.OnNext(new StepStartedInfo(stepNumber, 1));

            _stepProgressUpdatedSubject.OnNext(1);
            LeftMouseClick(macroClickStep.X, macroClickStep.Y);
            
            _stepCompletedSubject.OnNext(stepNumber);
        }
    }
}
