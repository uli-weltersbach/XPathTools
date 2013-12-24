using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Automation;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    internal class VisualStudioExperimentalInstance : IDisposable
    {
        private AutomationElement _mainWindow;

        public AutomationElement MainWindow
        {
            get
            {
                var process = FindExperimentalInstance();
                if (process == null)
                    return null;
                if (_mainWindow == null)
                    _mainWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id));
                return _mainWindow;
            }
        }

        public void WaitUntillStarted()
        {
            WaitUntillStarted(TimeSpan.FromMinutes(3));
        }

        private void WaitUntillStarted(TimeSpan timeoutDuration)
        {
            DateTime timeout = DateTime.UtcNow.Add(timeoutDuration);
            while (DateTime.UtcNow < timeout)
            {
                if (MainWindow == null)
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                else
                    return;
            }
            throw new TimeoutException(string.Format("Visual Studio wasn't started within {0}.", timeoutDuration));
        }

        public void Dispose()
        {
            Stop();
        }

        public void ReStart()
        {
            Stop();
            string programsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            FileInfo executablePath = new FileInfo(Path.Combine(programsFolder, "Microsoft Visual Studio 11.0", "Common7", "IDE", "devenv.exe"));
            if (!executablePath.Exists)
                throw new FileNotFoundException(string.Format("Didn't find Visual Studio executable at \"{0}\".", executablePath));
            // The VisualStudio process spawns a new process with a different ID.
            Process.Start(new ProcessStartInfo(executablePath.FullName, "/RootSuffix Exp"));
        }

        private Process FindExperimentalInstance()
        {
            return Process.GetProcessesByName("devenv").FirstOrDefault(p => p.MainWindowTitle.ToLower().Contains("experimental instance"));
        }

        public void Stop()
        {
            var process = FindExperimentalInstance();
            if (process == null)
                return;
            if (process.HasExited)
                return;
            process.Kill();
        }
    }
}