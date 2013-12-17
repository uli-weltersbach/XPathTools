using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Automation;
using EnvDTE;
using Process = System.Diagnostics.Process;

namespace ReasonCodeExample.XPathInformation.Tests
{
    internal class VisualStudioExperimentalInstance : IDisposable
    {
        private const string CommandLineArguments = "/RootSuffix Exp";
        private const string ExperimentalInstanceWindowTitle = "Experimental Instance";
        private const string DevelopmentEnvironmentProcessName = "devenv";
        private FileInfo _executable = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe"));
        private DirectoryInfo _extensionsDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\VisualStudio\11.0Exp\Extensions"));
        private AutomationElement _mainWindow;
        private int _processId;
        private IntPtr _processHandle;

        public VisualStudioExperimentalInstance()
        {
            if (_executable.Exists)
                StartProgram();
            else
                throw new FileNotFoundException("Executable not found.", _executable.FullName);
        }

        public DTE DevelopmentEnvironment
        {
            get;
            private set;
        }

        public AutomationElement MainWindow
        {
            get { return _mainWindow ?? (_mainWindow = FindMainWindow()); }
        }

        public void Dispose()
        {
            Process process = Process.GetProcessById(_processId);
            process.Kill();
        }

        private void StartProgram()
        {
            // Start new "devenv"-process
            Process.Start(_executable.FullName, CommandLineArguments);
            while (_processId == 0)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                Process process = Process.GetProcesses().Where(IsDevelopmentEnvironmentEnvironment).FirstOrDefault(IsExperimentalInstance);
                _processId = process == null ? 0 : process.Id;
                _processHandle = process == null ? IntPtr.Zero : process.Handle;
            }
            DevelopmentEnvironment = GetDTE(_processId);
        }

        private bool IsDevelopmentEnvironmentEnvironment(Process process)
        {
            return process.ProcessName.Equals(DevelopmentEnvironmentProcessName, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsExperimentalInstance(Process process)
        {
            return process.MainWindowTitle.Contains(ExperimentalInstanceWindowTitle);
        }

        private AutomationElement FindMainWindow()
        {
            AutomationElement root = AutomationElement.RootElement;
            return root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ProcessIdProperty, _processId));
        }

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        private DTE GetDTE(int processId)
        {
            string progId = "!VisualStudio.DTE.11.0:" + processId.ToString();
            object runningObject = null;

            IBindCtx bindCtx = null;
            IRunningObjectTable rot = null;
            IEnumMoniker enumMonikers = null;

            try
            {
                Marshal.ThrowExceptionForHR(CreateBindCtx(reserved: 0, ppbc: out bindCtx));
                bindCtx.GetRunningObjectTable(out rot);
                rot.EnumRunning(out enumMonikers);

                IMoniker[] moniker = new IMoniker[1];
                IntPtr numberFetched = IntPtr.Zero;
                while (enumMonikers.Next(1, moniker, numberFetched) == 0)
                {
                    IMoniker runningObjectMoniker = moniker[0];

                    string name = null;

                    try
                    {
                        if (runningObjectMoniker != null)
                            runningObjectMoniker.GetDisplayName(bindCtx, null, out name);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Do nothing, there is something in the ROT that we do not have access to.
                    }

                    if (!string.IsNullOrEmpty(name) && string.Equals(name, progId, StringComparison.Ordinal))
                    {
                        Marshal.ThrowExceptionForHR(rot.GetObject(runningObjectMoniker, out runningObject));
                        break;
                    }
                }
            }
            finally
            {
                if (enumMonikers != null)
                    Marshal.ReleaseComObject(enumMonikers);

                if (rot != null)
                    Marshal.ReleaseComObject(rot);

                if (bindCtx != null)
                    Marshal.ReleaseComObject(bindCtx);
            }

            return (DTE)runningObject;
        }
    }
}