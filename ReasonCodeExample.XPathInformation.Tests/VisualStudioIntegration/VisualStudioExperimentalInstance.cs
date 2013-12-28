using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    internal class VisualStudioExperimentalInstance
    {
        private AutomationElement _mainWindow;

        public AutomationElement MainWindow
        {
            get
            {
                Process process = FindExperimentalInstance();
                if (process == null)
                    return null;
                if (_mainWindow == null)
                    _mainWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id));
                return _mainWindow;
            }
        }

        private Process FindExperimentalInstance()
        {
            return Process.GetProcessesByName("devenv").FirstOrDefault(p => p.MainWindowTitle.ToLower().Contains("experimental instance"));
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
            WaitUntillStarted(TimeSpan.FromMinutes(3));
        }

        private void WaitUntillStarted(TimeSpan timeoutDuration)
        {
            DateTime timeout = DateTime.UtcNow.Add(timeoutDuration);
            while (DateTime.UtcNow < timeout)
            {
                if (MainWindow == null)
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                else
                    return;
            }
            throw new TimeoutException(string.Format("Visual Studio wasn't started within {0} seconds.", timeoutDuration.TotalSeconds));
        }

        public void Stop()
        {
            Process process = FindExperimentalInstance();
            if (process == null)
                return;
            if (process.HasExited)
                return;
            process.Kill();
        }

        public void OpenXmlFile(string content, int caretPosition)
        {
            MainWindow.FindDescendant("File").LeftClick();
            MainWindow.FindDescendant("New").LeftClick();
            MainWindow.FindDescendant("File...").LeftClick();

            // XML File entry in "New File" dialog.
            MainWindow.FindDescendant("XML File").LeftClick();
            MainWindow.FindDescendant("Open").LeftClick();

            WriteContent(content, caretPosition);
        }

        private static void WriteContent(string content, int caretPosition)
        {
            // Write content starting on new line
            SendKeys.SendWait("{End}");
            SendKeys.SendWait("{Enter}");
            SendKeys.SendWait(content);
            SendKeys.SendWait("{Home}");
            SendKeys.SendWait("{Right " + caretPosition + "}");
        }

        public void ExecuteContextMenuCommand(string menuText, string commandText)
        {
            // Use "shift F10" shortcut to open context menu
            SendKeys.SendWait("+{F10}");
            MainWindow.FindDescendant(menuText).LeftClick();
            MainWindow.FindDescendant(commandText).LeftClick();
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}