using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.AesHelper;
using BtmI2p.GeneralClientInterfaces.WalletServer;
using BtmI2p.MiscUtils;
using BtmI2p.ObjectStateLib;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Wallet
{
    public partial class SendFundsForm : Form
    {
        private readonly TaskCompletionSource<SendFundsFormData> _tcs;
        private readonly Guid _senderWalletId;
        public SendFundsForm(
            TaskCompletionSource<SendFundsFormData> tcs,
            Guid senderWalletId,
            SendFundsFormData initData = null
        )
        {
            InitializeComponent();
            if (initData != null)
                _formData = initData;
            _senderWalletId = senderWalletId;
            _tcs = tcs;
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void SendFundsForm_FormClosing(
            object sender, FormClosingEventArgs e)
        {
            _tcs.TrySetResult(null);
            await _stateHelper.MyDisposeAsync().ConfigureAwait(false);
        }
        private readonly SendFundsFormData _formData 
            = new SendFundsFormData();

        private async Task TryParseFormData(bool getPassword = true)
        {
            if (
                !long.TryParse(
                    textBox1.Text,
                    out _formData.Amount
                )
            )
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.TransferAmountParseError
                );
                return;
            }
            if (_formData.Amount <= 0)
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.TransferAmountZeroOrLessError
                );
                return;
            }
            if (
                !Guid.TryParse(
                    comboBox1.Text,
                    out _formData.ReceiverWalletGuid
                )
            )
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.ReceiverWalletGuidParseError
                );
                return;
            }
            if (
                !WalletServerRestrictionsForClient
                    .CheckSimpleTransferWalletTo(
                        _formData.ReceiverWalletGuid
                    )
            )
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.WrongReceiverWalletGuidTypeError
                );
                return;
            }
            if (getPassword)
            {
                var walletCertPassword
                    = await EnterPasswordForm.CreateAndReturnResult(
                        LocStrings.WalletCertPassRequestText,
                        this
                    );
                if (walletCertPassword == null)
                {
                    ClientGuiMainForm.ShowErrorMessage(this,
                        LocStrings.Messages.EmptyWalletCertPasswordError
                    );
                    return;
                }
                _formData.WalletCertPass = walletCertPassword;
            }
            _formData.AnonymousTransfer = checkBox2.Checked;
            if (checkBox1.Checked)
            {
                _formData.CommentBytes
                    = Convert.FromBase64String(textBox2.Text);
            }
            else
            {
                _formData.CommentBytes
                    = Encoding.UTF8.GetBytes(textBox2.Text);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await TryParseFormData();
            _tcs.TrySetResult(_formData);
            Close();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void UpdateFees()
        {
            long amount = 0;
            if (!long.TryParse(textBox1.Text, out amount) || amount <= 0)
            {
                label6.Text = LocStrings.UnknownFeeText;
            }
            else
            {
                label6.Text = string.Format(
                    "{0}",
                    WalletServerTransferFeeHelper.GetFeePos(
                        1,
                        amount,
                        Encoding.UTF8.GetByteCount(textBox2.Text)
                    )
                );
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateFees();
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                textBox2.SelectAll();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            UpdateFees();
        }

        private void SendFundsForm_Shown(object sender, EventArgs e)
        {
            _stateHelper.SetInitializedState();
            InitCommonView();
            // Load from _formData
            textBox1.Text
                = $"{_formData.Amount}";
            comboBox1.Text
                = $"{_formData.ReceiverWalletGuid}";
            try
            {
                textBox2.Text = Encoding.UTF8.GetString(_formData.CommentBytes);
            }
            catch
            {
                checkBox1.Checked = true;
                textBox2.Text = Convert.ToBase64String(_formData.CommentBytes);
            }
            checkBox2.Checked = _formData.AnonymousTransfer;
			button1.Select();
        }
        //To invoice
        private void button3_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                async () =>
                {
                    await TryParseFormData(false);
                    var invoiceData = _formData.ToInvoiceData();
                    Clipboard.SetText(invoiceData.WriteObjectToJson());
                    ClientGuiMainForm.ShowInfoMessage(this,
                        LocStrings.Messages.InvoiceDataCopiedToClipboardInfo
                    );
                },
                _stateHelper,
                _log
            );
        }
        private readonly DisposableObjectStateHelper _stateHelper 
            = new DisposableObjectStateHelper("SendFundsForm");

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static SendFundsFormLocStrings LocStrings
            = new SendFundsFormLocStrings();

        private void InitCommonView()
        {
            this.label1.Text = LocStrings.Label1Text;
            this.label2.Text = LocStrings.Label2Text;
            this.button1.Text = LocStrings.Button1Text;
            this.button2.Text = LocStrings.Button2Text;
            this.label4.Text = LocStrings.Label4Text;
            this.checkBox2.Text = LocStrings.Checkbox2Text;
            this.label5.Text = LocStrings.Label5Text;
            this.label6.Text = LocStrings.Label6Text;
            this.label7.Text = LocStrings.Label7Text;
            this.checkBox1.Text = LocStrings.Checkbox1Text;
            this.button3.Text = LocStrings.Button3Text;
            this.Text = LocStrings.TextInit;
            label8.Text = string.Format(
                "{0}",
                _senderWalletId
            );
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
    }

    public class SendFundsFormLocStrings
    {
        public class MessagesLocStrings
        {
            public string TransferAmountParseError
                = "Transfer amount parse error";
            public string TransferAmountZeroOrLessError
                = "Transfer amount <= 0";
            public string ReceiverWalletGuidParseError
                = "Receiver wallet GUID parse error";
            public string WrongReceiverWalletGuidTypeError
                = "Wrong receiver wallet GUID type";
            public string EmptyWalletCertPasswordError
                = "Empty wallet certificate password";
            public string InvoiceDataCopiedToClipboardInfo
                = "Invoice data copied to clipboard";
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
        /**/
        public string WalletCertPassRequestText = "Enter wallet certificate password";
        public string UnknownFeeText = "Unknown";
        /**/
        public string Label1Text = "Transfer amount:";
        public string Label2Text = "Receiver wallet GUID:";
        public string Button1Text = "OK";
        public string Button2Text = "Cancel";
        public string Label4Text = "Comment:";
        public string Checkbox2Text = "Anonymous transfer";
        public string Label5Text = "Fees:";
        public string Label6Text = "Unknown";
        public string Label7Text = "Sender wallet GUID:";
        public string Checkbox1Text = "Base64";
        public string Button3Text = "To invoice";
        public string TextInit = "Send Funds";
    }

    public class SendFundsFormData
    {
        public long Amount = 0;
        public Guid ReceiverWalletGuid = Guid.Empty;
        public AesProtectedByteArray WalletCertPass;
        public byte[] CommentBytes = Encoding.UTF8.GetBytes("");
        public bool AnonymousTransfer = false;

        public BitmoneyInvoiceData ToInvoiceData()
        {
            return new BitmoneyInvoiceData()
            {
                CommentBytes = CommentBytes,
                ForceAnonymousTransfer = AnonymousTransfer,
                TransferAmount = Amount,
                WalletTo = ReceiverWalletGuid
            };
        }
    }
}
