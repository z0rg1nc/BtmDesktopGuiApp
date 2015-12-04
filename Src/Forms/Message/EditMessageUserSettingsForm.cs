using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Message;
using BtmI2p.BitMoneyClient.Gui.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.GeneralClientInterfaces.MessageServer;
using BtmI2p.MiscUtils;

namespace BtmI2p.BitMoneyClient.Gui
{
    public partial class EditMessageUserSettingsForm : Form
    {
        public static EditMessageUserSettingsFormLocStrings LocStrings
            = new EditMessageUserSettingsFormLocStrings();
        private readonly IMessageClientSettings _localSettings;
        private readonly MessageClientSettingsOnServerClientInfo _settingsOnServer;

        private readonly Func<MessageClientSettingsOnServerClientInfo,CancellationToken,Task>
            _saveSettingsOnServerFunc;
        public EditMessageUserSettingsForm(
            IMessageClientSettings localSettings,
            MessageClientSettingsOnServerClientInfo settingsOnServer,
            Func<MessageClientSettingsOnServerClientInfo,CancellationToken,Task> 
                saveSettingsOnServerFunc 
        )
        {
            _saveSettingsOnServerFunc = saveSettingsOnServerFunc;
            _localSettings = localSettings;
            _settingsOnServer = settingsOnServer;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _localSettings.SendIAmOnlineMessages = checkBox1.Checked;
            ClientGuiMainForm.ShowInfoMessage(this, LocStrings.SettingsChangedInfoMessage);
        }
        
        private async void button2_Click(object sender, EventArgs e)
        {
            Guid authGuid;
            if (!Guid.TryParse(textBox1.Text, out authGuid))
            {
                ClientGuiMainForm.ShowErrorMessage(this, LocStrings.PermissionGuidParseErrorMessage);
                return;
            }
            decimal unauthorizedIncomMessageFee;
            if (!decimal.TryParse(textBox2.Text, out unauthorizedIncomMessageFee))
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.UnauthorizedIncomeMessageFeeParseErrorMessage
                );
                return;
            }
            using (var cts = new CancellationTokenSource())
            {
                var onProgressLoadTcs = new TaskCompletionSource<object>();
                var progressForm = new ProgressCancelForm(
                    cts,
                    onProgressLoadTcs,
                    LocStrings.ProgressFormCaption
                    );
                progressForm.Show(this);
                await onProgressLoadTcs.Task;
                MessageClientSettingsOnServerClientInfo settingsOnServer = _settingsOnServer;
                settingsOnServer.AuthGuid = authGuid;
                settingsOnServer.AutoRenewAuthGuid = checkBox2.Checked;
                settingsOnServer.UnauthorizedIncomeMessageFee = unauthorizedIncomMessageFee;
                try
                {
                    progressForm.ReportProgress(LocStrings.ProgressFormStart, 10);
                    await _saveSettingsOnServerFunc(settingsOnServer, cts.Token);
                    progressForm.ReportProgress(LocStrings.ProgressFormFinish, 100);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exc)
                {
                    ClientGuiMainForm.ShowErrorMessage(this,
                        LocStrings.SaveSettingsErrorMessage.Inject(
                            new
                            {
                                ErrorMessage = exc.Message
                            }
                            )
                        );
                }
                finally
                {
                    progressForm.SetProgressComplete();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text
                = $"{MiscFuncs.GenGuidWithFirstBytes(0)}";
        }

        private void EditMessageUserSettingsForm_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void EditMessageUserSettingsForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            checkBox1.Checked = _localSettings.SendIAmOnlineMessages;
            /**/
            textBox1.Text = string.Format("{0}", _settingsOnServer.AuthGuid);
            checkBox2.Checked = _settingsOnServer.AutoRenewAuthGuid;
            label4.Text = string.Format(
                "0 < x <= {0:0.00}",
                MessageServerClientLimitations
                    .MaxUnauthorizedIncomeMessageFee
            );
            textBox2.Text = string.Format(
                "{0:0.00}",
                _settingsOnServer.UnauthorizedIncomeMessageFee
            );
        }

        private void InitCommonView()
        {
            this.label5.Text = LocStrings.Label5Text;
            this.label1.Text = LocStrings.Label1Text;
            this.button1.Text = LocStrings.Button1Text;
            this.checkBox1.Text = LocStrings.Checkbox1Text;
            this.checkBox2.Text = LocStrings.Checkbox2Text;
            this.label6.Text = LocStrings.Label6Text;
            this.label4.Text = LocStrings.Label4Text;
            this.label3.Text = LocStrings.Label3Text;
            this.button3.Text = LocStrings.Button3Text;
            this.label2.Text = LocStrings.Label2Text;
            this.button2.Text = LocStrings.Button2Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    public class EditMessageUserSettingsFormLocStrings
    {
        public string SettingsChangedInfoMessage = "Settings changed";
        public string PermissionGuidParseErrorMessage = "Permission GUID parse error";
        public string UnauthorizedIncomeMessageFeeParseErrorMessage
            = "Unauthorized income message fee parse error";

        public string ProgressFormCaption = "Sending my settings to message server";
        public string ProgressFormStart = "Start";
        public string ProgressFormFinish = "Finish";

        public string SaveSettingsErrorMessage = "Save settings error '{ErrorMessage}'";
        /**/
        public string Label5Text = "Local";
        public string Label1Text = "*Need relog to take effect";
        public string Button1Text = "Save";
        public string Checkbox1Text = "Show \"I\'m online\" status to contact list.";
        public string Label6Text = "On server";
        public string Label4Text = "(Restriction)";
        public string Label3Text = "Unauthorized income message fee:";
        public string Button3Text = "Renew";
        public string Label2Text = "Write to me permission GUID ";
        public string Button2Text = "Save";
        public string TextInit = "Edit message user settings";
        public string Checkbox2Text = "Auto renew";
    }
}
