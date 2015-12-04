using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms
{
    public partial class AddContactForm : Form
    {
        public static AddContactFormLocStrings LocStrings 
            = new AddContactFormLocStrings();
        private readonly TaskCompletionSource<AddContactFormData> _tcs;
        public AddContactForm(
            TaskCompletionSource<AddContactFormData> tcs
            )
        {
            _tcs = tcs;
            InitializeComponent();
        }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private void button1_Click(object sender, EventArgs e)
        {
            string contactAlias = textBox2.Text;
            if (string.IsNullOrWhiteSpace(contactAlias))
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.EmptyUserAliasErrorMessage
                );
                return;
            }
            Guid contactGuid;
            if (!Guid.TryParse(textBox1.Text, out contactGuid))
            {
                _log.Trace("Parse guid error '{0}'", textBox1.Text);
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.InvalidUserGuidErrorMessage
                );
                return;
            }
            _tcs.TrySetResult(new AddContactFormData
            {
                Alias = contactAlias,
                UserGuid = contactGuid
            });
            Close();
        }

        private void AddContactForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _tcs.TrySetResult(null);
        }

        private void InitCommonView()
        {
            this.label1.Text = LocStrings.Label1Text;
            this.label2.Text = LocStrings.Label2Text;
            this.button1.Text = LocStrings.Button1Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }

        private void AddContactForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
        }
    }

    public class AddContactFormData
    {
        public Guid UserGuid;
        public string Alias;
    }

    public class AddContactFormLocStrings
    {
        public string EmptyUserAliasErrorMessage = "Empty user alias";
        public string InvalidUserGuidErrorMessage = "Invalid user guid";
        /**/
        public string Label1Text = "User guid:";
        public string Label2Text = "Alias:";
        public string Button1Text = "Add";
        public string TextInit = "Add contact";
    }
}
