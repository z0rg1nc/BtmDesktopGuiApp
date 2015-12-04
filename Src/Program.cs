using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.GeneralClientInterfaces;
using BtmI2p.MiscUtils;
using CommandLine;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui
{
    public class BitMoneyClientGuidOptions
    {
        [Option("use-mono", Required = false, DefaultValue = 0)]
        public int UseMonoRun { get; set; }
    }
    public static class BitMoneyClientProgram
    {
        public static BitMoneyClientGuidOptions CommandLineOptions = null;
        
        [STAThread]
        public static void Main()
        {
            const string curMethodName = "Main";
            var commandLineArgs = Environment.GetCommandLineArgs();
            _log.Trace(
                "{2} {0} command line args: '{1}'",
                curMethodName,
                commandLineArgs.WriteObjectToJson(),
                CommonClientConstants.CurrentClientVersionString
            );
            var options = new BitMoneyClientGuidOptions();
            if (!Parser.Default.ParseArguments(commandLineArgs, options))
                throw new ArgumentException("Wrong command line arguments");
            CommandLineOptions = options;
            /**/
            var startupPath = Application.StartupPath;
            string mutexName;
            using (var sha256 = new SHA256Managed())
            {
                mutexName
                    = Convert.ToBase64String(
                        sha256.ComputeHash(
                            Encoding.UTF8.GetBytes(
                                startupPath
                            )
                        )
                    );
            }
            var mutex = new Mutex(
                true, mutexName
            );
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    AppDomain.CurrentDomain.UnhandledException
                        += CurrentDomainOnUnhandledException;
                    try
                    {
                        CultureInfo.DefaultThreadCurrentUICulture
                            = CultureInfo.InvariantCulture;
                    }
                    catch (NotImplementedException)
                    {
                    }
                    try
                    {
                        Application.CurrentCulture
                            = CultureInfo.InvariantCulture;
                    }
                    catch (NotImplementedException)
                    {
                    }
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    var thread = new Thread(MonitoringDumpRequest)
                    {
                        IsBackground = true
                    };
                    thread.Start();
                    /**/
                    var clientGuiForm = new ClientGuiMainForm();
                    Application.Run(clientGuiForm);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                MessageBox.Show(
                    "Only one instance at a time",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static readonly Logger _log 
            = LogManager.GetCurrentClassLogger();
        private static void CurrentDomainOnUnhandledException(
            object sender, 
            UnhandledExceptionEventArgs unhandledExceptionEventArgs
        )
        {
            _log.Fatal(
                "UnhandledException {0}",
                ((Exception)unhandledExceptionEventArgs.ExceptionObject).ToString()
            );
        }

        private static void MonitoringDumpRequest()
        {
            const string curMethodName = "MonitoringDumpRequest";
            try
            {
                var dumpRequestFilePath = Path.Combine(".", "dumprequest");
                while (true)
                {
                    if (File.Exists(dumpRequestFilePath))
                    {
                        File.Delete(dumpRequestFilePath);
                        DumpDebugInfo.DumpToFile(
                            Path.Combine(
                                ".",
                                $"dump{DateTime.UtcNow:yyyyMMddHHmmss}.txt"
                                )
                        );
                    }
                    Thread.Sleep(2000);
                }
            }
            catch (Exception exc)
            {
                _log.Error(
                    "{0} unexpected error '{1}'",
                    curMethodName,
                    exc.ToString()
                );
            }
        }
    }
}
