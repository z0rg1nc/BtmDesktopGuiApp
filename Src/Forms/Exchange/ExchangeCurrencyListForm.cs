using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib.ExchangeServerSession;
using BtmI2p.GeneralClientInterfaces.ExchangeServer;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged.Winforms;
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    public partial class ExchangeCurrencyListForm : Form
    {
        private readonly IExchangeServerSessionModelData _sessionModel;
        public ExchangeCurrencyListForm(
            IExchangeServerSessionModelData sessionModel)
        {
            Assert.NotNull(sessionModel);
            _sessionModel = sessionModel;
            InitializeComponent();
        }
        public static ExchangeCurrencyListFormDesignerLocStrings DesignerLocStrings = new ExchangeCurrencyListFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.codeHeader.Text = DesignerLocStrings.CodeHeaderText;
            this.descriptionHeader.Text = DesignerLocStrings.DescriptionHeaderText;
            this.scaleHeader.Text = DesignerLocStrings.ScaleHeaderText;
            this.minDepositHeader.Text = DesignerLocStrings.MinDepositHeaderText;
            this.maxDepositHeader.Text = DesignerLocStrings.MaxDepositHeaderText;
            this.depositInaccuracyHeader.Text = DesignerLocStrings.DepositInaccuracyHeaderText;
            this.newDepositFeeBtmHeader.Text = DesignerLocStrings.NewDepositFeeBtmHeaderText;
            this.depositFeeConstHeader.Text = DesignerLocStrings.DepositFeeConstHeaderText;
            this.depositFeePercentHeader.Text = DesignerLocStrings.DepositFeePercentHeaderText;
            this.minWithdrawHeader.Text = DesignerLocStrings.MinWithdrawHeaderText;
            this.maxWithdrawHeader.Text = DesignerLocStrings.MaxWithdrawHeaderText;
            this.newWithdrawFeeBtmHeader.Text = DesignerLocStrings.NewWithdrawFeeBtmHeaderText;
            this.withdrawFeeConstHeader.Text = DesignerLocStrings.WithdrawFeeConstHeaderText;
            this.withdrawFeePercentHeader.Text = DesignerLocStrings.WithdrawFeePercentHeaderText;
            this.columnAutowidthByHeaderToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItemText;
            this.columnAutowidthByContentToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByContentToolStripMenuItemText;
            this.label1.Text = DesignerLocStrings.Label1Text;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenuStrip1, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }
        private async void ExchangeCurrencyListForm_Shown(object sender, EventArgs e)
        {
            _stateHelper.SetInitializedState();
            InitCommonView();
            _asyncSubscriptions.Add(
                await ListViewCollectionChangedOneWayBinding<ExchangeCurrencyClientInfo>.CreateInstance(
                    currencyListView,
                    _sessionModel.CurrencyCollection,
                    null,
                    (currency, item) =>
                    {
                        item.Tag = currency;
                        item.SubItems[0].Text = $"{currency.Code}";
                        item.SubItems[1].Text = $"{currency.Description}";
                        item.SubItems[2].Text = $"{currency.Scale}";
                        item.SubItems[3].Text = $"{currency.MinDeposit:G29}";
                        item.SubItems[4].Text = $"{currency.MaxDeposit:G29}";
                        item.SubItems[5].Text = $"{currency.DepositInaccuracyConst:G29}";
                        item.SubItems[6].Text = $"{currency.NewDepositFeeConstBtmPos:G29}";
                        item.SubItems[7].Text = $"{currency.DepositFeeConst:G29}";
                        item.SubItems[8].Text = $"{currency.DepositFeePercent:G29}";
                        item.SubItems[9].Text = $"{currency.MinWithdrawPos:G29}";
                        item.SubItems[10].Text = $"{currency.MaxWithdrawPos:G29}";
                        item.SubItems[11].Text = $"{currency.NewWithdrawFeeConstBtmPos:G29}";
                        item.SubItems[12].Text = $"{currency.WithdrawFeeConstPos:G29}";
                        item.SubItems[13].Text = $"{currency.WithdrawFeePercent:G29}";
                    }
                )
            );
        }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly List<IMyAsyncDisposable> _asyncSubscriptions = new List<IMyAsyncDisposable>(); 
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper("ExchangeCurrencyListForm");
        private async void ExchangeCurrencyListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cts.Cancel();
            await _stateHelper.MyDisposeAsync();
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();
            foreach (var asyncSubscription in _asyncSubscriptions)
            {
                await asyncSubscription.MyDisposeAsync();
            }
            _asyncSubscriptions.Clear();
            _cts.Dispose();
        }

        private void columnAutowidthByHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currencyListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void columnAutowidthByContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currencyListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
    public class ExchangeCurrencyListFormDesignerLocStrings
    {
        public string CodeHeaderText = "Code";
        public string DescriptionHeaderText = "Description";
        public string ScaleHeaderText = "Scale";
        public string MinDepositHeaderText = "Min deposit";
        public string MaxDepositHeaderText = "Max deposit";
        public string DepositInaccuracyHeaderText = "Deposit inaccuracy";
        public string NewDepositFeeBtmHeaderText = "New deposit fee BTM";
        public string DepositFeeConstHeaderText = "Deposit fee const*";
        public string DepositFeePercentHeaderText = "Deposit fee %*";
        public string MinWithdrawHeaderText = "Min withdraw";
        public string MaxWithdrawHeaderText = "Max withdraw";
        public string NewWithdrawFeeBtmHeaderText = "New withdraw fee BTM";
        public string WithdrawFeeConstHeaderText = "Withdraw fee const*";
        public string WithdrawFeePercentHeaderText = "Withdraw fee %*";
        public string ColumnAutowidthByHeaderToolStripMenuItemText = "Column autowidth by header";
        public string ColumnAutowidthByContentToolStripMenuItemText = "Column autowidth by content";
        public string Label1Text = "* Doesn\'t include own payment system fees";
        public string Text = "Currency list";
    }
}
