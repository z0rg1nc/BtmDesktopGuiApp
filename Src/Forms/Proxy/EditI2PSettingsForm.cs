using System;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Gui.Proxy;

namespace BtmI2p.BitMoneyClient.Gui.Forms
{
    public partial class EditI2PSettingsForm : Form
    {
        private readonly IPublicProxySettings _publicProxySettings;
        private readonly IPrivateProxySettings _privateProxySettings;
        public EditI2PSettingsForm(
            IPublicProxySettings publicProxySettings,
            IPrivateProxySettings privateProxySettings
        )
        {
            _publicProxySettings = publicProxySettings;
            _privateProxySettings = privateProxySettings;
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            var samServerAddress = textBox1.Text;
            if (string.IsNullOrWhiteSpace(samServerAddress))
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.WrongSamServerAddressError
                );
                return;
            }
            int samServerPort;
            if (
                !int.TryParse(textBox2.Text, out samServerPort)
                || samServerPort < 0 || samServerPort > 65534)
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.WrongSamServerPortError
                );
                return;
            }
            var clientI2PKeys = textBox4.Text.Trim();
            if(clientI2PKeys.Length == 0)
                clientI2PKeys = null;
            decimal minBalanceForRefill;
            if (
                !decimal.TryParse(textBox3.Text, out minBalanceForRefill)
                || minBalanceForRefill <= 0.0m
                )
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.WrongMinBalanceToRefillError
                );
                return;
            }
            bool autoFill = checkBox1.Checked;
            /**/
            _publicProxySettings.SamServerAddress = samServerAddress;
            /**/
            _publicProxySettings.SamServerPort = samServerPort;
            /**/
            _publicProxySettings.AutoFillup = autoFill;
            /**/
            _publicProxySettings.AutoFillupMinBalance 
                = minBalanceForRefill;
            /**/
            _privateProxySettings.ClientPrivKeys = clientI2PKeys;
            Close();
        }

        private void EditI2PSettings_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            textBox1.Text = _publicProxySettings.SamServerAddress;
            textBox2.Text = string.Format("{0}", _publicProxySettings.SamServerPort);
            textBox4.Text = _privateProxySettings.ClientPrivKeys ?? "";
            checkBox1.Checked = _publicProxySettings.AutoFillup;
            textBox3.Text = string.Format("{0}", _publicProxySettings.AutoFillupMinBalance);
        }
        public static EditI2PSettingsFormLocStrings LocStrings
            = new EditI2PSettingsFormLocStrings();

        public void InitCommonView()
        {
            this.label1.Text = LocStrings.Label1Text;
            this.label2.Text = LocStrings.Label2Text;
            this.checkBox1.Text = LocStrings.Checkbox1Text;
            this.label3.Text = LocStrings.Label3Text;
            this.label4.Text = LocStrings.Label4Text;
            this.button1.Text = LocStrings.Button1Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
    }

    public class EditI2PSettingsFormLocStrings
    {
        public class MessagesLocStrings
        {
            public string WrongSamServerAddressError
                = "Wrong sam server address";
            public string WrongSamServerPortError
                = "Wrong sam server port";
            public string WrongMinBalanceToRefillError
                = "Wrong min balance for fill";
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
        /**/
        public string Label1Text = "Sam server address:";
        public string Label2Text = "Sam server port:";
        public string Checkbox1Text = "Auto fill";
        public string Label3Text = "Min balance for fill:";
        public string Label4Text = "Client i2p keys:";
        public string Button1Text = "Save";
        public string TextInit = "Edit I2P Settings";
    }
}
