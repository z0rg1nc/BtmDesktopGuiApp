using System;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Message;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Forms.User
{
    public partial class ViewUserContactInfo : Form
    {
        private readonly ContactInfo _contactInfo;
        private readonly IMessageClientModel _messageClientModel;
        public ViewUserContactInfo(
            ContactInfo contactInfo,
            IMessageClientModel messageClientModel
        )
        {
            _contactInfo = contactInfo;
            _messageClientModel = messageClientModel;
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            _contactInfo.Alias = textBox2.Text;
            var settings = _messageClientModel.Settings;
            MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                settings,
                x => x.ContactInfoList
            );
            Close();
        }

        private void ViewUserContactInfo_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            textBox1.Text =
                string.Format("{0}", _contactInfo.UserId);
            textBox2.Text =
                string.Format("{0}", _contactInfo.Alias);
            textBox3.Text =
                string.Format(
                    "{0:0.00}",
                    _contactInfo.SettingsOnServer
                        .UnauthorizedIncomeMessageFee
                );
        }

        private void InitCommonView()
        {
            this.label1.Text = LocStrings.Label1Text;
            this.label2.Text = LocStrings.Label2Text;
            this.button1.Text = LocStrings.Button1Text;
            this.label3.Text = LocStrings.Label3Text;
            this.Text = LocStrings.TextInit;

            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
        public static ViewUserContactInfoLocStrings LocStrings 
            = new ViewUserContactInfoLocStrings();
    }

    public class ViewUserContactInfoLocStrings
    {
        public string Label1Text = "User guid:";
        public string Label2Text = "User alias:";
        public string Button1Text = "Save ";
        public string Label3Text = "Unauthorized income message fee:";
        public string TextInit = "User contact info";
    }
}
