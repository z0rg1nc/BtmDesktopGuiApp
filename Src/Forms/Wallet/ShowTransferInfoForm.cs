using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Wallet
{
    public partial class ShowTransferInfoForm : Form
    {
        private readonly WalletFormModelTransferInfo _transferInfo;
        public ShowTransferInfoForm(
            WalletFormModelTransferInfo transferInfo
        )
        {
            _transferInfo = transferInfo;
            InitializeComponent();
        }

        private void RefreshCommentTextbox()
        {
            if (_transferInfo.CommentBytes == null
                || _transferInfo.CommentBytes.Length == 0)
            {
                textBox7.Text = string.Empty;
                return;
            }
            if (radioButton1.Checked)
            {
                try
                {
                    textBox7.Text = Encoding.UTF8.GetString(_transferInfo.CommentBytes);
                }
                catch
                {
                    textBox7.Text = LocStrings.UnvalidUtf8StringText;
                }
            }
            else
            {
                textBox7.Text = Convert.ToBase64String(_transferInfo.CommentBytes);
            }
        }
        
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RefreshCommentTextbox();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            RefreshCommentTextbox();
        }

        private void textBox7_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                textBox7.SelectAll();
            }
        }
        private void ShowTransferInfoForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            /***/
            textBox1.Text = $"{_transferInfo.TransferStatus}";
            if (_transferInfo.TransferStatus == WalletFormModelTransferStatus.SendError)
                textBox1.Text += $" ({_transferInfo.ErrCode} {_transferInfo.ErrMessage})";
            /**/
            textBox1.Font = new Font(
                textBox1.Font,
                _transferInfo.AnonymousTransfer
                    ? FontStyle.Italic
                    : FontStyle.Bold
            );
            textBox2.Text
                = string.Format("{0}", _transferInfo.Amount);
            textBox9.Text
                = string.Format("{0}", _transferInfo.Fee);
            /**/
            if (
                _transferInfo.TransferStatus
                    == WalletFormModelTransferStatus.PreparedToSend
                || _transferInfo.TransferStatus
                    == WalletFormModelTransferStatus.SendError
            )
            {
                textBox3.Text
                    = string.Format("{0}", Guid.Empty);
            }
            else
            {
                textBox3.Text
                    = string.Format("{0}", _transferInfo.TransferGuid);
            }
            /**/
            textBox4.Text
                = string.Format("{0}", _transferInfo.WalletFrom);
            textBox5.Text
                = string.Format("{0}", _transferInfo.WalletTo);
            textBox6.Text
                = string.Format("{0:G}", _transferInfo.SentTime);
            if (_transferInfo.TransferStatus != WalletFormModelTransferStatus.Received)
            {
                textBox8.Text
                    = string.Format("{0}", _transferInfo.RequestGuid);
            }
            else
            {
                textBox8.Text
                    = string.Format("{0}", Guid.Empty);
            }
            RefreshCommentTextbox();
        }
        public static ShowTransferInfoFormLocStrings LocStrings
            = new ShowTransferInfoFormLocStrings();

        private void InitCommonView()
        {
            this.label1.Text = LocStrings.Label1Text;
            this.label2.Text = LocStrings.Label2Text;
            this.label3.Text = LocStrings.Label3Text;
            this.label4.Text = LocStrings.Label4Text;
            this.label5.Text = LocStrings.Label5Text;
            this.label6.Text = LocStrings.Label6Text;
            this.label7.Text = LocStrings.Label7Text;
            this.label9.Text = LocStrings.Label9Text;
            this.radioButton1.Text = LocStrings.Radiobutton1Text;
            this.radioButton2.Text = LocStrings.Radiobutton2Text;
            this.label8.Text = LocStrings.Label8Text;
            this.Text = LocStrings.TextInit;

            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
    }

    public class ShowTransferInfoFormLocStrings
    {
        public class MessagesLocStrings
        {
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
        /**/
        public string UnvalidUtf8StringText = "Unvalid UTF8 string";
        public string ToMyselfDirectionText = "To myself";
        public string SendDirectionText = "Send";
        public string ReceiveDirectionText = "Receive";
        /**/
        public string Label1Text = "Status:";
        public string Label2Text = "Amount:";
        public string Label3Text = "Transfer GUID:";
        public string Label4Text = "Wallet from:";
        public string Label5Text = "Wallet to:";
        public string Label6Text = "Sent time:";
        public string Label7Text = "Comment:";
        public string Label9Text = "Fee:";
        public string Radiobutton1Text = "UTF8 string";
        public string Radiobutton2Text = "Base64";
        public string Label8Text = "Request GUID:";
        public string TextInit = "Transfer info";

    }
}
