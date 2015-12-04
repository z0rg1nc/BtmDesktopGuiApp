using System;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib.WalletServerSession;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Forms.Wallet
{
    public partial class EditWalletSettingsForm : Form
    {
        private readonly IWalletServerSessionModel _sessionModel;
        public EditWalletSettingsForm(
            IWalletServerSessionModel sessionModel
        )
        {
            InitializeComponent();
            _sessionModel = sessionModel;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            long minTransferAmount;
            if (!long.TryParse(textBox1.Text, out minTransferAmount))
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.ParseMinTransferAmountError
                );
                return;
            }
            if (minTransferAmount < 1)
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.WrongMinTransferAmountError
                );
                return;
            }
            var setting = _sessionModel.ClientSettingsOnServer;
            setting.MinIncomeTransferBalance 
                = minTransferAmount;
            MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                _sessionModel,
                x => x.ClientSettingsOnServer
            );
            Close();
        }

        private void EditWalletSettingsForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            var settings = _sessionModel.ClientSettingsOnServer;
            textBox1.Text = string.Format(
                "{0}",
                settings.MinIncomeTransferBalance
            );
        }
        public static EditWalletSettingsFormLocStrings
            LocStrings = new EditWalletSettingsFormLocStrings();

        private void InitCommonView()
        {
            this.label1.Text = LocStrings.Label1Text;
            this.button1.Text = LocStrings.Button1Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
    }

    public class EditWalletSettingsFormLocStrings
    {
        public class MessagesLocStrings
        {
            public string ParseMinTransferAmountError
                = "Parse min income transfer amount error";
            public string WrongMinTransferAmountError
                = "minTransferAmount < 1";
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
        /**/
        public string Label1Text = "Deny income transfers with \r\namount less than:";
        public string Button1Text = "Save";
        public string TextInit = "Edit wallet settings";

    }
}
