using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.ObjectStateLib;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MiningLocal
{
    public partial class EditMiningLocalSettingsForm : Form
    {
	    private readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly DisposableObjectStateHelper _stateHelper
			= new DisposableObjectStateHelper("EditMiningLocalSettingsForm");
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		/**/
        private readonly IMiningTaskManagerSettings _settings;
        public EditMiningLocalSettingsForm(
            IMiningTaskManagerSettings settings
        )
        {
            _settings = settings;
            InitializeComponent();
			_stateHelper.SetInitializedState();
        }

        private static bool IsPowerOfTwo(ulong x)
        {
            return (x & (x - 1)) == 0;
        }
        private void button6_Click(object sender, EventArgs e)
        {
	        ClientGuiMainForm.HandleControlActionProper(this,
                async () =>
		        {
			        EMiningNativeSupport nativeSupport;
			        int concurrentTaskCount;
			        int threadsPerTask;
			        bool passProblemToExternalSolver;
			        string externalSolverPath;
			        string externalSolverArguments;
			        string solutionsFolder;
			        if (
				        !int.TryParse(
					        textBox5.Text,
					        out concurrentTaskCount
					        )
				        || concurrentTaskCount < 1
				        || concurrentTaskCount > 100
				        )
			        {
				        ClientGuiMainForm.ShowErrorMessage(this,
                            LocStrings.Messages.BadTaskCountError
					        );
				        return;
			        }
			        if (
				        !int.TryParse(
					        textBox6.Text,
					        out threadsPerTask
					        )
				        || threadsPerTask < 1
				        || threadsPerTask > 100
				        || !IsPowerOfTwo((ulong) threadsPerTask)
				        )
			        {
				        ClientGuiMainForm.ShowErrorMessage(this,
                            LocStrings.Messages.BadThreadsCountError
					        );
				        return;
			        }
			        nativeSupport = radioButton1.Checked
				        ? EMiningNativeSupport.None
				        : radioButton2.Checked
					        ? EMiningNativeSupport.WinDll
					        : EMiningNativeSupport.LinuxSo;
			        passProblemToExternalSolver = checkBox1.Checked;
			        externalSolverPath = textBox1.Text;
			        externalSolverArguments = textBox2.Text;
			        solutionsFolder = textBox4.Text;
			        if (passProblemToExternalSolver)
			        {
				        if (!File.Exists(externalSolverPath))
				        {
					        ClientGuiMainForm.ShowErrorMessage(this,
                                LocStrings.Messages.ExternalSolverFileNotExistError
						        );
					        return;
				        }
				        if (!Directory.Exists(solutionsFolder))
				        {
					        ClientGuiMainForm.ShowErrorMessage(this,
                                LocStrings.Messages.SolutionsFoderDoesntExistError
						        );
					        return;
				        }
			        }
			        /**/
			        if (_settings.NativeSupport != nativeSupport)
				        _settings.NativeSupport = nativeSupport;
			        if (_settings.RunningTaskPoolSize != concurrentTaskCount)
				        _settings.RunningTaskPoolSize = concurrentTaskCount;
			        if (_settings.ThreadsPerTask != threadsPerTask)
				        _settings.ThreadsPerTask = threadsPerTask;
			        if (_settings.PassProblemsToExternalSolver != passProblemToExternalSolver)
				        _settings.PassProblemsToExternalSolver = passProblemToExternalSolver;
			        if (_settings.ExternalSolverExeFullPath != externalSolverPath)
				        _settings.ExternalSolverExeFullPath = externalSolverPath;
			        if (_settings.ExternalSolverCommandArguments != externalSolverArguments)
				        _settings.ExternalSolverCommandArguments = externalSolverArguments;
			        if (_settings.SolutionsFolder != solutionsFolder)
				        _settings.SolutionsFolder = solutionsFolder;
			        Close();
		        },
		        _stateHelper,
		        _log
		    );
        }

        private void EditMiningLocalSettingsForm_Shown(object sender, EventArgs e)
        {
			ClientGuiMainForm.BeginInvokeProper(
				this,
				_stateHelper,
				_log,
				() =>
				{
					InitCommonView();
					if (_settings.NativeSupport == EMiningNativeSupport.None)
					{
						radioButton1.Checked = true;
					}
					else if (_settings.NativeSupport == EMiningNativeSupport.WinDll)
					{
						radioButton2.Checked = true;
					}
					else
					{
						radioButton3.Checked = true;
					}
					textBox4.Text = $"{_settings.SolutionsFolder}";
					textBox5.Text = $"{_settings.RunningTaskPoolSize}";
					textBox6.Text = $"{_settings.ThreadsPerTask}";
					checkBox1.Checked = _settings.PassProblemsToExternalSolver;
					textBox1.Text = _settings.ExternalSolverExeFullPath;
					textBox2.Text = _settings.ExternalSolverCommandArguments;
				}
			);
        }

        private void InitCommonView()
        {
            this.checkBox1.Text = LocStrings.Checkbox1Text;
            this.button6.Text = LocStrings.Button6Text;
            this.label16.Text = LocStrings.Label16Text;
            this.label17.Text = LocStrings.Label17Text;
            this.label1.Text = LocStrings.Label1Text;
            this.groupBox1.Text = LocStrings.Groupbox1Text;
            this.label4.Text = LocStrings.Label4Text;
            this.label3.Text = LocStrings.Label3Text;
            this.label2.Text = LocStrings.Label2Text;
            this.label6.Text = LocStrings.Label6Text;
            this.radioButton1.Text = LocStrings.RadioButton1Text;
            this.radioButton2.Text = LocStrings.RadioButton2Text;
            this.radioButton3.Text = LocStrings.RadioButton3Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }

        public static EditMiningLocalSettingsFormLocStrings LocStrings
            = new EditMiningLocalSettingsFormLocStrings();

		private async void EditMiningLocalSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			_cts.Cancel();
			await _stateHelper.MyDisposeAsync();
            _cts.Dispose();
		}
    }

    public class EditMiningLocalSettingsFormLocStrings
    {
        public class MessagesLocStrings
        {
            public string BadTaskCountError = "Bad concurrent task count";
            public string BadThreadsCountError = "Bad threads per task count";
            public string ExternalSolverFileNotExistError 
                = "External solver filepath doesn't exist";
            public string SolutionsFoderDoesntExistError
                = "Solutions folder doesn't exist";
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
        /**/
        public string Checkbox1Text = "Pass problem to external solver";
        public string Button6Text = "Save";
        public string Label16Text = "Threads per task:";
        public string Label17Text = "Concurrent task count:";
        public string Label1Text = "Solver .exe path:";
        public string Groupbox1Text = "External solver options";
        public string Label4Text = "Solutions folder:";
        public string Label3Text = "Use next vars: ";
        public string Label2Text = "Arguments:";
        public string TextInit = "Edit mining local settings";
        public string Label6Text = "Native support";
        public string RadioButton1Text = "None";
        public string RadioButton2Text = "Windows(x86\\x64)";
        public string RadioButton3Text = "Linux(x86\\x64)";
    }
}
