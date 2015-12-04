using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.AesHelper;
using BtmI2p.MiscClientForms;

namespace BtmI2p.BitMoneyClient.Gui.Forms
{
    public partial class EnterPasswordForm : Form
    {
        private readonly TaskCompletionSource<AesProtectedByteArray> _tcs
            = new TaskCompletionSource<AesProtectedByteArray>();
        public Task<AesProtectedByteArray> Result
        {
            get { return _tcs.Task; }
        }

        public static async Task<AesProtectedByteArray> CreateAndReturnResult(
            string caption,
            IWin32Window owner
        )
        {
            var passForm = new EnterPasswordForm(caption);
            await passForm.ShowFormAsync(owner);
            return await passForm.Result;
        }

        private readonly string _caption;
        public EnterPasswordForm(
            string caption
        )
        {
            InitializeComponent();
            _caption = caption;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetResultAndClose();
        }

        private void SetResultAndClose()
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                ClientGuiMainForm.ShowErrorMessage(
                    this,
                    LocStrings.Messages.EmptyPasswordError
                );
                return;
            }
            _tcs.TrySetResult(
                new AesProtectedByteArray(
                    new TempByteArray(
                        Encoding.UTF8.GetBytes(
                            textBox2.Text
                        )
                    )
                )
            );
            Close();
        }

        private void EnterPasswordForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _tcs.TrySetResult(null);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetResultAndClose();
            }
        }

        private void EnterPasswordForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            textBox2.Select();
        }

        private void InitCommonView()
        {
            this.label1.Text = LocStrings.Label1Text;
            this.button1.Text = LocStrings.Button1Text;
            this.Text = _caption;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
        public static EnterPasswordFormLocStrings LocStrings
            = new EnterPasswordFormLocStrings();
    }

    public class EnterPasswordFormLocStrings
    {
        public class LocMessages
        {
            public string EmptyPasswordError = "Empty password box";
        }
        public LocMessages Messages = new LocMessages();
        /**/
        public string Label1Text = "Password:";
        public string Button1Text = "OK";
        public string TextInit = "Caption";
    }
}
