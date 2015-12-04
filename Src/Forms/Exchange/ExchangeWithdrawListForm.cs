using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib.ExchangeServerSession;
using BtmI2p.GeneralClientInterfaces.ExchangeServer;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged.MyObservableCollections;
using BtmI2p.MyNotifyPropertyChanged.Winforms;
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
	public partial class ExchangeWithdrawListForm : Form
	{
		private readonly IExchangeServerSessionModelData _sessionModel;
		public ExchangeWithdrawListForm(
			IExchangeServerSessionModelData sessionModel)
		{
			Assert.NotNull(sessionModel);
			_sessionModel = sessionModel;
			InitializeComponent();
		}
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly List<IMyAsyncDisposable> _asyncSubscriptions = new List<IMyAsyncDisposable>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly DisposableObjectStateHelper _stateHelper
			= new DisposableObjectStateHelper("ExchangeWithdrawListForm");

		private async void ExchangeWithdrawListForm_FormClosing(object sender, FormClosingEventArgs e)
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
                await asyncSubscription.MyDisposeAsync().ConfigureAwait(false);
            }
            _asyncSubscriptions.Clear();
            _cts.Dispose();
        }
        private readonly ObservableCollectionProxyFilter<ExchangeWithdrawClientInfo> _proxyFilter
            = new ObservableCollectionProxyFilter<ExchangeWithdrawClientInfo>(
                async trade => await Task.FromResult(true)
            );
        private readonly MyObservableCollectionProxyComparer<ExchangeWithdrawClientInfo> _proxyComparer
            = new MyObservableCollectionProxyComparer<ExchangeWithdrawClientInfo>(
                Comparer<ExchangeWithdrawClientInfo>.Create(
                    (wth1,wth2) => -wth1.CreatedDate.CompareTo(wth2.CreatedDate)
                )
            );
        private readonly MyObservableCollectionProxyN _proxyN
            = new MyObservableCollectionProxyN(100);
        /**/
        public static ExchangeWithdrawListFormDesignerLocStrings DesignerLocStrings = new ExchangeWithdrawListFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.dateHeader.Text = DesignerLocStrings.DateHeaderText;
            this.currencyCodeHeader.Text = DesignerLocStrings.CurrencyCodeHeaderText;
            this.valueHeader.Text = DesignerLocStrings.ValueHeaderText;
            this.estimatedPsFeeHeader.Text = DesignerLocStrings.EstimatedPsFeeHeaderText;
            this.estimatedFeeHeader.Text = DesignerLocStrings.EstimatedFeeHeaderText;
            this.totalValueHeader.Text = DesignerLocStrings.TotalValueHeaderText;
            this.finalPsFeeHeader.Text = DesignerLocStrings.FinalPsFeeHeaderText;
            this.finalFeeHeader.Text = DesignerLocStrings.FinalFeeHeaderText;
            this.statusHeader.Text = DesignerLocStrings.StatusHeaderText;
            this.statusCommentHeader.Text = DesignerLocStrings.StatusCommentHeaderText;
            this.paymentDetailsHeader.Text = DesignerLocStrings.PaymentDetailsHeaderText;
            this.accountHeader.Text = DesignerLocStrings.AccountHeaderText;
            this.withdrawGuidHeader.Text = DesignerLocStrings.WithdrawGuidHeaderText;
            this.lockedFundsGuidHeader.Text = DesignerLocStrings.LockedFundsGuidHeaderText;
            this.copyStatusTextToolStripMenuItem.Text = DesignerLocStrings.CopyStatusTextToolStripMenuItemText;
            this.copyPaymentDetailsToolStripMenuItem.Text = DesignerLocStrings.CopyPaymentDetailsToolStripMenuItemText;
            this.columnAutowidthByHeaderToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItemText;
            this.columnAutowidthByContentToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByContentToolStripMenuItemText;
            this.checkBox3.Text = DesignerLocStrings.CheckBox3Text;
            this.checkBox1.Text = DesignerLocStrings.CheckBox1Text;
            this.checkBox2.Text = DesignerLocStrings.CheckBox2Text;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_withdrawEntry, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_withdrawEmpty, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }
        private async void ExchangeWithdrawListForm_Shown(object sender, EventArgs e)
		{
			_stateHelper.SetInitializedState();
            InitCommonView();
            var filteredCollection = await MyFilteredObservableCollectionImpl.CreateInstance(
                _sessionModel.WithrawList,
                _proxyFilter
                );
            var orderedCollection = await MyOrderedObservableCollection.CreateInstance(
                filteredCollection,
                _proxyComparer
                );
            var nFirstCollection = await MyNFirstObservableCollectionImpl.CreateInstance(
                orderedCollection,
                _proxyN
            );
            var binding = await ListViewCollectionChangedOneWayBinding<ExchangeWithdrawClientInfo>.CreateInstance(
		        withdrawListView,
                nFirstCollection,
		        null,
		        (withdraw, item) =>
		        {
		            item.Tag = withdraw;
		            item.BackColor = withdraw.Status.In(
		                EExchangeWithdrawStatus.Created,
		                EExchangeWithdrawStatus.InQueue
		                )
		                ? Color.White
		                : withdraw.Status == EExchangeWithdrawStatus.Complete
		                    ? Color.LawnGreen
		                    : withdraw.Status == EExchangeWithdrawStatus.Error
		                        ? Color.Red
		                        : Color.Yellow;
		            item.SubItems[0].Text = $"{withdraw.CreatedDate}";
		            item.SubItems[1].Text = $"{withdraw.CurrencyCode}";
		            item.SubItems[2].Text = $"{withdraw.ValueNeg:G29}";
		            item.SubItems[3].Text = $"{withdraw.EstimatedPaymentSystemFeeNeg:G29}";
		            item.SubItems[4].Text = $"{withdraw.EstimatedFeeNeg:G29}";
		            item.SubItems[5].Text =
		                $"{(withdraw.ValueNeg + withdraw.EstimatedFeeNeg + withdraw.EstimatedPaymentSystemFeeNeg):G29}";
		            item.SubItems[6].Text = $"{withdraw.FinalPaymentSystemFeeNeg:G29}";
		            item.SubItems[7].Text = $"{withdraw.FinalFeeNeg:G29}";
		            var statusDict = ClientGuiMainForm.LocStrings.ExchangeServerLocStringsInstance.ExchangeWithdrawStatusDict;
		            item.SubItems[8].Text = $"{statusDict[withdraw.Status]}";
		            item.SubItems[9].Text = $"{withdraw.StatusComment}";
		            item.SubItems[10].Text = "...";
		            item.SubItems[11].Text = $"{withdraw.AccountGuid}";
		            item.SubItems[12].Text = $"{withdraw.WithdrawGuid}";
		            item.SubItems[13].Text = $"{withdraw.LockedFundsGuid}";
		        }
		    );
            _asyncSubscriptions.Add(
                new CompositeMyAsyncDisposable(
                    binding,
                    nFirstCollection,
                    orderedCollection,
                    filteredCollection
                )
            );
		}

        private void withdrawListView_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void withdrawListView_MouseUp(object sender, MouseEventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        if (withdrawListView.SelectedIndices.Count == 0)
                        {
                            contextMenu_withdrawEmpty.Show(
                                Cursor.Position
                            );
                        }
                        else
                        {
                            contextMenu_withdrawEntry.Show(
                                Cursor.Position
                            );
                        }
                    }
                },
                _stateHelper,
                _logger
            );
        }

        private void copyStatusTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                async () =>
                {
                    using (await withdrawListView.GetLockSem().GetDisposable())
                    {
                        var selectedItem = withdrawListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
                        if (selectedItem == null)
                            return;
                        var tag = (ExchangeWithdrawClientInfo) selectedItem.Tag;
                        Clipboard.SetText(tag.StatusComment.With(
                                _ => _ == "" ? " " : _
                            ));
                    }
                },
                _stateHelper,
                _logger
            );
        }

        private void copyPaymentDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                async () =>
                {
                    using (await withdrawListView.GetLockSem().GetDisposable())
                    {
                        var selectedItem = withdrawListView.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
                        if (selectedItem == null)
                            return;
                        var tag = selectedItem.Tag as ExchangeWithdrawClientInfo;
                        if (tag == null || string.IsNullOrWhiteSpace(tag.PaymentDetailsSerialized))
                            return;
                        var pd = tag.PaymentDetailsSerialized.ParseJsonToType<ExchangePaymentDetails>();
                        pd.CheckMe();
                        new ExchangePaymentDetailsEditOrShowForm(
                            pd,
                            EExchangePaymentDetailsEditOrShowFormMode.Show,
                            ""
                        ).Show(this);
                    }
                },
                _stateHelper,
                _logger
            );
        }

        private void columnAutowidthByHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            withdrawListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void columnAutowidthByContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            withdrawListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    var newN = int.Parse((string)comboBox1.SelectedItem);
                    _proxyN.N = newN;
                },
                _stateHelper,
                _logger
            );
        }

	    private void UpdateFilter()
	    {
	        var processing = checkBox1.Checked;
	        var complete = checkBox2.Checked;
	        var fault = checkBox3.Checked;
	        _proxyFilter.Predicate = async wth1 =>
	        {
	            if (complete && wth1.Status == EExchangeWithdrawStatus.Complete)
	                return true;
	            if (fault && wth1.Status == EExchangeWithdrawStatus.Error)
	                return true;
	            if (processing && wth1.Status.In(
                    EExchangeWithdrawStatus.Created,
                    EExchangeWithdrawStatus.InQueue,
                    EExchangeWithdrawStatus.Processing
                ))
	                return true;
	            return await Task.FromResult(false);
	        };
	    }

	    private void checkBox1_CheckedChanged(object sender, EventArgs e)
	    {
	        UpdateFilter();
	    }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilter();
        }
        private string _orderPropName = nameof(ExchangeWithdrawClientInfo.CreatedDate);
        private bool _orderAsc = false;
        private void withdrawListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    var columnNum = e.Column;
                    if (columnNum.In(0, 1, 2, 8, 11, 12, 13))
                    {
                        string newOrderPropName =
                            columnNum == 0 ? nameof(ExchangeWithdrawClientInfo.CreatedDate)
                            : columnNum == 1 ? nameof(ExchangeWithdrawClientInfo.CurrencyCode)
                            : columnNum == 2 ? nameof(ExchangeWithdrawClientInfo.ValueNeg)
                            : columnNum == 8 ? nameof(ExchangeWithdrawClientInfo.Status)
                            : columnNum == 11 ? nameof(ExchangeWithdrawClientInfo.AccountGuid)
                            : columnNum == 12 ? nameof(ExchangeWithdrawClientInfo.WithdrawGuid)
                            : nameof(ExchangeWithdrawClientInfo.LockedFundsGuid);
                        var newOrderAsc = _orderPropName == newOrderPropName ? !_orderAsc : true;
                        _orderPropName = newOrderPropName;
                        _orderAsc = newOrderAsc;
                        _proxyComparer.Comparer = Comparer<ExchangeWithdrawClientInfo>.Create(
                            (wth1, wth2) =>
                            {
                                int result;
                                switch (columnNum)
                                {
                                    default:
                                    case 0:
                                        result = wth1.CreatedDate.CompareTo(wth2.CreatedDate);
                                        break;
                                    case 1:
                                        result = wth1.CurrencyCode.CompareTo(wth2.CurrencyCode);
                                        break;
                                    case 2:
                                        result = wth1.ValueNeg.CompareTo(wth2.ValueNeg);
                                        break;
                                    case 8:
                                        result = wth1.Status.CompareTo(wth2.Status);
                                        break;
                                    case 11:
                                        result = wth1.AccountGuid.CompareTo(wth2.AccountGuid);
                                        break;
                                    case 12:
                                        result = wth1.WithdrawGuid.CompareTo(wth2.WithdrawGuid);
                                        break;
                                    case 13:
                                        result = wth1.LockedFundsGuid.CompareTo(wth2.LockedFundsGuid);
                                        break;
                                }
                                return newOrderAsc ? result : -result;
                            }
                        );
                    }
                },
                _stateHelper,
                _logger
            );
        }
    }
    public class ExchangeWithdrawListFormDesignerLocStrings
    {
        public string DateHeaderText = "Date";
        public string CurrencyCodeHeaderText = "Currency code";
        public string ValueHeaderText = "Value";
        public string EstimatedPsFeeHeaderText = "Estimated payment system fee";
        public string EstimatedFeeHeaderText = "Estimated fee";
        public string TotalValueHeaderText = "Total value";
        public string FinalPsFeeHeaderText = "Final payment system fee";
        public string FinalFeeHeaderText = "Final fee";
        public string StatusHeaderText = "Status";
        public string StatusCommentHeaderText = "Status comment";
        public string PaymentDetailsHeaderText = "Payment details";
        public string AccountHeaderText = "Account GUID";
        public string WithdrawGuidHeaderText = "Withdraw GUID";
        public string LockedFundsGuidHeaderText = "Locked fund GUID";
        public string CopyStatusTextToolStripMenuItemText = "Copy status comment";
        public string CopyPaymentDetailsToolStripMenuItemText = "View payment details";
        public string ColumnAutowidthByHeaderToolStripMenuItemText = "Column autowidth by header";
        public string ColumnAutowidthByContentToolStripMenuItemText = "Column autowidth by content";
        public string CheckBox3Text = "Fault";
        public string CheckBox1Text = "Processing";
        public string CheckBox2Text = "Complete";
        public string Text = "Withdraw list";
    }
}
