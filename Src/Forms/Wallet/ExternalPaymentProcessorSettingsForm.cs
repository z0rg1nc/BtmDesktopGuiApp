using System;
using System.Threading;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.MiscUtils;
using BtmI2p.ObjectStateLib;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Wallet
{
    public partial class ExternalPaymentProcessorSettingsForm : Form
    {
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly DisposableObjectStateHelper _stateHelper
			= new DisposableObjectStateHelper("ExternalPaymentProcessorSettingsForm");
		private readonly Logger _log = LogManager.GetCurrentClassLogger();
		/**/
        private readonly IExternalPaymentProcessorSettings _settings;
        public ExternalPaymentProcessorSettingsForm(
            IExternalPaymentProcessorSettings settings
        )
        {
            _settings = settings;
            InitializeComponent();
			_stateHelper.SetInitializedState();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
	        ClientGuiMainForm.HandleControlActionProper(this,
                async () =>
		        {
			        using (await _settings.LockSem.GetDisposable())
			        {
				        if (_settings.ProcessReceivedTransfers != checkBox3.Checked)
				        {
					        _settings.ProcessReceivedTransfers = checkBox3.Checked;
				        }
				        if (_settings.ProcessSentTransfers != checkBox2.Checked)
				        {
					        _settings.ProcessSentTransfers = checkBox2.Checked;
				        }
				        if (_settings.ProcessSendTransferFaults != checkBox4.Checked)
				        {
					        _settings.ProcessSendTransferFaults = checkBox4.Checked;
				        }
				        if (_settings.CommandLineArguments != textBox2.Text)
				        {
					        _settings.CommandLineArguments = textBox2.Text;
				        }
				        if (_settings.ExternalProcessorAppPath != textBox1.Text)
				        {
					        _settings.ExternalProcessorAppPath = textBox1.Text;
				        }
				        if (
					        ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
						        .UseExternalPaymentProcessor != checkBox1.Checked
					        )
				        {
					        ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
						        .UseExternalPaymentProcessor = checkBox1.Checked;
				        }
			        }
			        Close();
		        },
		        _stateHelper,
		        _log
		    );
        }
        
        private void ExternalPaymentProcessorSettingsForm_Shown(object sender, EventArgs e)
        {
	        ClientGuiMainForm.BeginInvokeProper(
		        this,
		        _stateHelper,
		        _log,
		        () =>
		        {
			        InitCommonView();
			        checkBox1.Checked
				        = ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
					        .UseExternalPaymentProcessor;
			        textBox1.Text = _settings.ExternalProcessorAppPath;
			        textBox2.Text = _settings.CommandLineArguments;
			        checkBox2.Checked = _settings.ProcessSentTransfers;
			        checkBox3.Checked = _settings.ProcessReceivedTransfers;
			        checkBox4.Checked = _settings.ProcessSendTransferFaults;
		        }
			);
        }
        public static ExternalPaymentProcessorSettingsFormLocStrings LocStrings
            = new ExternalPaymentProcessorSettingsFormLocStrings();

        private void InitCommonView()
        {
            this.checkBox1.Text = LocStrings.Checkbox1Text;
            this.label3.Text = LocStrings.Label3Text;
            this.label2.Text = LocStrings.Label2Text;
            this.label1.Text = LocStrings.Label1Text;
            this.button1.Text = LocStrings.Button1Text;
            this.checkBox4.Text = LocStrings.Checkbox4Text;
            this.checkBox3.Text = LocStrings.Checkbox3Text;
            this.checkBox2.Text = LocStrings.Checkbox2Text;
            this.groupBox3.Text = LocStrings.Groupbox3Text;
            this.Text = LocStrings.TextInit;
            this.textBox3.Text = LocStrings.TextBox3Text;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }

		private async void ExternalPaymentProcessorSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			_cts.Cancel();
			await _stateHelper.MyDisposeAsync();
            _cts.Dispose();
		}
    }

    public class ExternalPaymentProcessorSettingsFormLocStrings
    {
        public string Checkbox1Text = "Send payments details to\r\n an external application.";
        public string Label3Text = "Use next variables:";
        public string Label2Text = "Command line arguments";
        public string Label1Text = "Application path";
        public string Button1Text = "Save";
        public string Checkbox4Text = "Send error";
        public string Checkbox3Text = "Received";
        public string Checkbox2Text = "Sent";
        public string Groupbox3Text = "External processor options";
        public string TextInit = "External payment processor settings";
        public string TextBox3Text 
            = @"
__PAYMENT_TYPE__ (0 - Send error, 1 - Sent, 2 - Received)
__REQUEST_GUID__
__TRANSFER_GUID__
__WALLET_FROM__
__WALLET_TO__
__ANONYMOUS_INT__ (1 - yes, 0 - no)
__AMOUNT__
__FEE__
__COMMENT_BYTES_B64__
__TIME_UTC__ (yyyyMMddHHmmss)
__SEND_ERROR_CODE__ (0 - no errors)
__SEND_SERVER_GENERAL_ERROR_CODE__ (100 - no errors)
__SEND_SERVER_ERROR_CODE__ (0 - no errors)
__SEND_ERROR_MESSAGE__
__AUTH_OTHER_WALLET_CERT__ (1\0)
__AUTH_COMMENT_KEY__ (1\0)
__AUTH_PAYMENT_DETAILS__ (1\0)
";
    }
}
