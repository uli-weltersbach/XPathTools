using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    internal class VisualStudioExperimentalInstance
    {
        private AutomationElement _mainWindow;
        private Process _process;

        public AutomationElement MainWindow
        {
            get
            {
                _process = FindExperimentalInstance();
                if (_process == null)
                    return null;
                if (_mainWindow == null)
                    _mainWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ProcessIdProperty, _process.Id));
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
            OpenNewFileDialog();
            OpenNewXmlFile();
            InsertContentIntoNewXmlFile(content);
            SetCaretPosition(caretPosition);
        }

        private void OpenNewFileDialog()
        {
            MainWindow.FindDescendant("File").LeftClick();
            MainWindow.FindDescendant("New").LeftClick();
            MainWindow.FindDescendant("File...").LeftClick();
        }

        private void OpenNewXmlFile()
        {
            MainWindow.FindDescendant("XML File").LeftClick();
            MainWindow.FindDescendant("Open").LeftClick();
        }

        private void InsertContentIntoNewXmlFile(string content)
        {
            // Write content starting on a new line, after the XML declaration
            SendKeys.SendWait("{End}");
            SendKeys.SendWait("{Enter}");
            SendKeys.SendWait(content);
        }

        private void SetCaretPosition(int caretPosition)
        {
            // Go to the start of the line and move forward from there
            SendKeys.SendWait("{Home}");
            SendKeys.SendWait("{Right " + caretPosition + "}");
        }

        public IList<AutomationElement> GetContextMenuCommands(string subMenuName, Regex commandName)
        {
            // Use "shift F10" shortcut to open context menu
            SendKeys.SendWait("+{F10}");
            MainWindow.FindDescendant(subMenuName).LeftClick();
            AutomationElementCollection descendants = MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ProcessIdProperty, _process.Id));
            return (from AutomationElement descendant in descendants
                    where descendant.GetSupportedProperties().Contains(AutomationElement.NameProperty)
                    let elementName = descendant.GetCurrentPropertyValue(AutomationElement.NameProperty)
                    where elementName != null
                    where commandName.IsMatch(elementName.ToString())
                    select descendant).Distinct().ToArray();
        }
    }
}