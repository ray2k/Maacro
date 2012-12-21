using AForge.Imaging;
using Maacro.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        public void Play(IEnumerable<MacroStep> macro, bool clickVerification, int uiDelay)
        {
            _steps = macro.ToList();
            PlaybackTask = new Task(() => PlayMacro(macro, clickVerification, uiDelay));
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
        private IList<MacroStep> _steps = null;

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

        private void PlayMacro(IEnumerable<MacroStep> macro, bool verifyClicks, int uiDelay)
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
                        Play(step as MacroClickStep, stepNumber, verifyClicks, uiDelay);

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

        private void Play(MacroClickStep macroClickStep, int stepNumber, bool verify, int uiDelay)
        {
            if (WaitHandle.WaitOne(1))
                return;

            var elementLower = macroClickStep.ScreenElement.ToString().ToLower();

            if (verify == true && 
                (
                    elementLower.StartsWith("jetbay") || 
                    elementLower.StartsWith("collect")
                ))
            {
                PlayAndVerify(macroClickStep, stepNumber);
            }
            else 
            {
                _stepStartedSubject.OnNext(new StepStartedInfo(stepNumber, 1));

                _stepProgressUpdatedSubject.OnNext(1);
                LeftMouseClick(macroClickStep.X, macroClickStep.Y);

                _stepCompletedSubject.OnNext(stepNumber);
            }
        }

        private void PlayAndVerify(MacroClickStep macroClickStep, int stepNumber)
        {
            Debug.WriteLine("PlayAndVerify for #{0} {1}", macroClickStep.StepNumber, macroClickStep.Description);

            Bitmap beforeClickSmallImage = null;

            if (WaitHandle.WaitOne(1))
                return;

            int shortDelay = 500;

            // move cursor to position
            SetCursorPos(macroClickStep.X, macroClickStep.Y);

            // slight delay to allow hover/ui effects to show up
            if (WaitHandle.WaitOne(1))
                return;
            Debug.WriteLine("Waiting {0}ms", shortDelay);
            Thread.Sleep(shortDelay);
            if (WaitHandle.WaitOne(1))
                return;

            // take picture around cursor
            Debug.WriteLine("Collecting before image at {0} {1}", macroClickStep.X, macroClickStep.Y);
            beforeClickSmallImage = ExtractCursorBitmap(macroClickStep.X, macroClickStep.Y);

            Debug.WriteLine("Collecting before screen image for {0} {1}", macroClickStep.X, macroClickStep.Y);
            var beforeClickScreenImage = ExtractScreenBitmap();

            bool retrying = false;

            while (true)
            {
                if (retrying)
                    Debug.WriteLine("Retrying");

                // send mouse click
                Debug.WriteLine("Sending click");
                SetCursorPos(macroClickStep.X, macroClickStep.Y);
                mouse_event(MOUSEEVENTF_LEFTDOWN, macroClickStep.X, macroClickStep.Y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, macroClickStep.X, macroClickStep.Y, 0, 0);

                // another short delay
                if (WaitHandle.WaitOne(1))
                    return;
                Debug.WriteLine("Waiting {0}ms", shortDelay);
                Thread.Sleep(shortDelay);
                if (WaitHandle.WaitOne(1))
                    return;

                // take 'after' picture
                Debug.WriteLine("Collecting after click image at {0} {1}", macroClickStep.X, macroClickStep.Y);
                var afterClickSmallImage = ExtractCursorBitmap(macroClickStep.X, macroClickStep.Y);

                Debug.WriteLine("Collecting after screen image for {0} {1}", macroClickStep.X, macroClickStep.Y);
                var afterClickScreenImage = ExtractScreenBitmap();

                // compare, if too similar then UI did not respond in time or there
                // was interference (fuck playdom and their snow)
                if (AreTooSimilar(beforeClickSmallImage, afterClickSmallImage) == true)
                {
                    Debug.WriteLine("Before-click and after-click images are too similar for {0},{1}", macroClickStep.X, macroClickStep.Y);

#if DEBUG
                    beforeClickSmallImage.Save(string.Format("{0}_{1}_click_before.bmp", macroClickStep.X, macroClickStep.Y), ImageFormat.Bmp);           
                    afterClickSmallImage.Save(string.Format("{0}_{1}_click_after.bmp", macroClickStep.X, macroClickStep.Y), ImageFormat.Bmp);
#endif                    

                    if (AreTooSimilar(beforeClickScreenImage, afterClickScreenImage) == true)
                    {
                        Debug.WriteLine("Before-screen and after-screen images were too similar for {0},{1}", macroClickStep.X, macroClickStep.Y);

#if DEBUG
                        beforeClickScreenImage.Save(string.Format("{0}_{1}_screen_before.bmp", macroClickStep.X, macroClickStep.Y), ImageFormat.Bmp);
                        afterClickScreenImage.Save(string.Format("{0}_{1}_screen_after.bmp", macroClickStep.X, macroClickStep.Y), ImageFormat.Bmp);
#endif

                        afterClickSmallImage.Dispose();
                        retrying = true;
                        continue;
                    }
                    else
                    {
                        Debug.WriteLine("Before-screen and after-screen images were different enough for {0},{1}", macroClickStep.X, macroClickStep.Y);
                        beforeClickSmallImage.Dispose();
                        afterClickSmallImage.Dispose();
                        beforeClickSmallImage = null;

                        beforeClickScreenImage.Dispose();
                        beforeClickScreenImage = null;

                        retrying = false;
                        break;
                    }
                }
                else
                {
                    Debug.WriteLine("Before-click and after-click images are different enough for {0},{1}", macroClickStep.X, macroClickStep.Y);
                    // they were not too similar - UI click worked so break out of
                    // verification loop
                    beforeClickSmallImage.Dispose();
                    afterClickSmallImage.Dispose();
                    beforeClickSmallImage = null;

                    beforeClickScreenImage.Dispose();
                    beforeClickScreenImage = null;

                    retrying = false;
                    break;
                }
            }
        }

        private bool AreTooSimilar(Bitmap source, Bitmap updated)
        {
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);
            
            TemplateMatch[] matchings = tm.ProcessImage(source, updated);

            Debug.WriteLine("Similarity = {0}", matchings[0].Similarity);

            if (matchings[0].Similarity > 0.90)
                return true;
            else
                return false;
        }

        private Bitmap ExtractScreenBitmap()
        {
            var clickSteps = _steps.OfType<MacroClickStep>();

            var lowestY = clickSteps.Min(p => p.Y);
            var lowestX = clickSteps.Min(p => p.X);

            var highestY = clickSteps.Max(p => p.Y);
            var highestX = clickSteps.Max(p => p.X);

            int width = highestX - lowestX;
            int height = highestY - lowestY;

            var bmpScreenshot = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            var pos = new Point(lowestX, lowestY);
            using (Graphics g = Graphics.FromImage(bmpScreenshot))
            {
                g.CopyFromScreen(lowestX, lowestY, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }

            return bmpScreenshot;
        }

        private Bitmap ExtractCursorBitmap(int x, int y)
        {
            var bmpScreenshot = new Bitmap(32, 32, PixelFormat.Format24bppRgb);

            var pos = new Point(x, y);
            using (Graphics g = Graphics.FromImage(bmpScreenshot))
            {
                g.CopyFromScreen(pos.X - 16, pos.Y - 16, 0, 0, new Size(32, 32), CopyPixelOperation.SourceCopy);
            }

            return bmpScreenshot;
        }
    }
}
