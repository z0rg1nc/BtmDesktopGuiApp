using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.GeneralClientInterfaces.WalletServer;
using BtmI2p.MiscUtils;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Wallet
{
    public partial class ProcessWalletInvoiceForm : Form
    {
        private BitmoneyInvoiceData _initInvoiceData;
        private readonly TaskCompletionSource<Tuple<BitmoneyInvoiceData, Guid>> _tcs;
        private readonly List<Tuple<string, Guid>> _walletListData;
        private readonly Guid _initWallet;
        public ProcessWalletInvoiceForm(
            BitmoneyInvoiceData initInvoiceData,
            TaskCompletionSource<Tuple<BitmoneyInvoiceData,Guid>> tcs,
            List<Tuple<string,Guid>> walletListData,
            Guid initWallet = default(Guid)
        )
        {
            _tcs = tcs;
            _initInvoiceData = initInvoiceData;
            _walletListData = walletListData;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _tcs.TrySetResult(null);
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BitmoneyInvoiceData invoiceData;
            try
            {
                invoiceData = textBox1.Text.ParseJsonToType<BitmoneyInvoiceData>();
            }
            catch (Exception)
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.ParseInvoiceDataError
                );
                return;
            }
            try
            {
                invoiceData.CheckMe();
            }
            catch (Exception exc)
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.WrongInvoiceDataError.Inject(
                        new
                        {
                            ErrorMessage = exc.Message
                        }
                    )
                );
                return;
            }
            Guid walletGuid;
            try
            {
                walletGuid = (Guid) comboBox1.SelectedValue;
            }
            catch (Exception)
            {
                ClientGuiMainForm.ShowErrorMessage(this,
                    LocStrings.Messages.WalletFromNotSelected
                );
                return;
            }
            _tcs.TrySetResult(
                new Tuple<BitmoneyInvoiceData, Guid>(
                    invoiceData,
                    walletGuid
                )
            );
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var invoiceData = textBox1.Text.ParseJsonToType<BitmoneyInvoiceData>();
                invoiceData.CheckMe();
            }
            catch
            {
                BackColor = Color.LightPink;
                return;
            }
            BackColor = Color.LightGreen;
        }
        
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                textBox1.SelectAll();
            }
        }

        private void ProcessWalletInvoiceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _tcs.TrySetResult(null);
        }

        private void ProcessWalletInvoiceForm_Shown(object sender, EventArgs e)
        {
            InitCommonView();
            textBox1.Text = _initInvoiceData.WriteObjectToJson();
            comboBox1.DataSource = _walletListData;
            comboBox1.DisplayMember = "Item1";
            comboBox1.ValueMember = "Item2";
            var index = _walletListData.FindIndex(_ => _.Item2 == _initWallet);
            if (index >= 0)
                comboBox1.SelectedIndex = index;
			textBox1.Select();
        }

        private void InitCommonView()
        {
            this.label3.Text = LocStrings.Label3Text;
            this.button1.Text = LocStrings.Button1Text;
            this.button2.Text = LocStrings.Button2Text;
            this.Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
        public static ProcessWalletInvoiceFormLocStrings LocStrings
            = new ProcessWalletInvoiceFormLocStrings();
    }

    public class ProcessWalletInvoiceFormLocStrings
    {
        public class MessagesLocStrings
        {
            public string ParseInvoiceDataError
                = "Parse invoice data error";
            public string WrongInvoiceDataError
                = "Wrong invoice data: '{ErrorMessage}'";
            public string WalletFromNotSelected
                = "WalletFrom not selected";
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
        /**/
        public string Label3Text = "WalletFrom GUID";
        public string Button1Text = "OK";
        public string Button2Text = "Cancel";
        public string TextInit = "Process wallet invoice";
    }
}
