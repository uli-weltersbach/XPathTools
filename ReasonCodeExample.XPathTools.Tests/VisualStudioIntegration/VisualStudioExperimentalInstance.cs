using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration
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
                if(_process == null)
                {
                    return null;
                }
                if(_mainWindow == null)
                {
                    var processIdCondition = new PropertyCondition(AutomationElement.ProcessIdProperty, _process.Id);
                    _mainWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, processIdCondition);
                }
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
            var executablePath = FindLatestVisualStudioUsingVswhere();
            if(!executablePath.Exists)
            {
                throw new FileNotFoundException($"Didn't find Visual Studio executable at \"{executablePath}\".");
            }
            // The VisualStudio process spawns a new process with a different ID.
            Process.Start(new ProcessStartInfo(executablePath.FullName, "/RootSuffix Exp /ResetSkipPkgs"));
            WaitUntillStarted(TimeSpan.FromMinutes(3));
        }

        public void Stop()
        {
            var process = FindExperimentalInstance();
            if(process == null)
            {
                return;
            }
            if(process.HasExited)
            {
                return;
            }
            process.Kill();
        }

        private FileInfo FindLatestVisualStudioUsingVswhere()
        {
            var programsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var vswhereExePath = new FileInfo(Path.Combine(programsFolder, "Microsoft Visual Studio", "Installer", "vswhere.exe"));
            if (!vswhereExePath.Exists)
            {
                throw new FileNotFoundException($"Didn't find vswhere.exe at \"{vswhereExePath}\".");
            }
            Process process = new Process();
            process.StartInfo.FileName = vswhereExePath.FullName;
            process.StartInfo.Arguments = "-latest";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (!string.IsNullOrWhiteSpace(error))
            {
                throw new Exception($"Error calling vswhere.exe (\"{vswhereExePath}\"): {error}");
            }
            if (string.IsNullOrWhiteSpace(output))
            {
                throw new Exception($"vswhere.exe (\"{vswhereExePath}\") output was null or empty.");
            }
            var match = Regex.Match(output, "^productPath: (.*)$", RegexOptions.Multiline);
            if (match.Success)
            {
                return new FileInfo(match.Groups[1].Value.Trim());
            }
            throw new Exception($"vswhere.exe (\"{vswhereExePath}\") output didn't include product path: {output}");
        }

        private void WaitUntillStarted(TimeSpan timeoutDuration)
        {
            var timeout = DateTime.UtcNow.Add(timeoutDuration);
            while(DateTime.UtcNow < timeout)
            {
                if(MainWindow == null)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
                else
                {
                    return;
                }
            }
            throw new TimeoutException($"Visual Studio wasn't started within {timeoutDuration.TotalSeconds} seconds.");
        }

        public void OpenXmlFile(string content, int? caretPosition)
        {
            OpenNewFileDialog();
            OpenNewXmlFile();
            InsertContentIntoNewXmlFile(content);
            if(caretPosition.HasValue)
            {
                SetCaretPosition(caretPosition.Value);
            }
        }

        private void OpenNewFileDialog()
        {
            MainWindow.FindDescendantByText("File").LeftClick();
            MainWindow.FindDescendantByText("New").LeftClick();
            MainWindow.FindDescendantByText("File...").LeftClick();
        }

        private void OpenNewXmlFile()
        {
            MainWindow.FindDescendantByText("XML File").LeftClick();
            MainWindow.FindDescendantByText("Open").LeftClick();
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

        public void ClickContextMenuEntry(string entryName)
        {
            // Use "shift F10" shortcut to open context menu
            SendKeys.SendWait("+{F10}");
            MainWindow.FindDescendantByText(entryName).LeftClick();
        }

        public IList<AutomationElement> GetContextMenuSubMenuCommands(string subMenuName, Regex commandName)
        {
            ClickContextMenuEntry(subMenuName);
            var descendants = MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ProcessIdProperty, _process.Id));
            return (from AutomationElement descendant in descendants
                    where descendant.GetSupportedProperties().Contains(AutomationElement.NameProperty)
                    let elementName = descendant.GetCurrentPropertyValue(AutomationElement.NameProperty)
                    where elementName != null
                    where commandName.IsMatch(elementName.ToString())
                    select descendant).Distinct().ToArray();
        }
    }
}
