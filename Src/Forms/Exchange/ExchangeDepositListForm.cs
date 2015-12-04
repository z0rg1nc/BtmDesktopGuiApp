using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib.ExchangeServerSession;
using BtmI2p.GeneralClientInterfaces.ExchangeServer;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged.MyObservableCollections;
using BtmI2p.MyNotifyPropertyChanged.Winforms;
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
	public partial class ExchangeDepositListForm : Form
	{
        private readonly IExchangeServerSessionModelData _sessionModel;
	    private readonly Func<ExchangeServerSession> _sessionGetter;
        public ExchangeDepositListForm(
			IExchangeServerSessionModelData sessionModel,
            Func<ExchangeServerSession> sessionGetter)
		{
			Assert.NotNull(sessionModel);
            Assert.NotNull(sessionGetter);
			_sessionModel = sessionModel;
            _sessionGetter = sessionGetter;
			InitializeComponent();
		}

	    private ObservableCollectionProxyFilter<ExchangeDepositClientInfo> _proxyFilter;
        private MyFilteredObservableCollectionImpl<ExchangeDepositClientInfo> _filteredObservableCollection;
	    private MyObservableCollectionProxyComparer<ExchangeDepositClientInfo> _proxyComparer;
        private MyOrderedObservableCollection<ExchangeDepositClientInfo> _orderedObservableCollection;
	    private MyObservableCollectionProxyN _nProxy;
        private MyNFirstObservableCollectionImpl<ExchangeDepositClientInfo> _nFirstObservableCollection;
        public static ExchangeDepositListFormDesignerLocStrings DesignerLocStrings = new ExchangeDepositListFormDesignerLocStrings();
        public static ExchangeDepositListFormLocStrings LocStrings = new ExchangeDepositListFormLocStrings();
        private void InitCommonView()
        {
            this.dateColumnHeader.Text = DesignerLocStrings.DateColumnHeaderText;
            this.currencyCodeColumnHeader.Text = DesignerLocStrings.CurrencyCodeColumnHeaderText;
            this.valueColumnHeader.Text = DesignerLocStrings.ValueColumnHeaderText;
            this.estimatedPsFeeColumnHeader.Text = DesignerLocStrings.EstimatedPsFeeColumnHeaderText;
            this.estimatedFeeColumnHeader.Text = DesignerLocStrings.EstimatedFeeColumnHeaderText;
            this.totalColumnHeader.Text = DesignerLocStrings.TotalColumnHeaderText;
            this.accountColumnHeader.Text = DesignerLocStrings.AccountColumnHeaderText;
            this.statusColumnHeader.Text = DesignerLocStrings.StatusColumnHeaderText;
            this.statusCommentColumnHeader.Text = DesignerLocStrings.StatusCommentColumnHeaderText;
            this.paymentDetailsColumnHeader.Text = DesignerLocStrings.PaymentDetailsColumnHeaderText;
            this.validUntilColumnHeader.Text = DesignerLocStrings.ValidUntilColumnHeaderText;
            this.depositGuidColumnHeader.Text = DesignerLocStrings.DepositGuidColumnHeaderText;
            this.copyPaymentDetailsToolStripMenuItem.Text = DesignerLocStrings.CopyPaymentDetailsToolStripMenuItemText;
            this.copyStatusTextToolStripMenuItem.Text = DesignerLocStrings.CopyStatusTextToolStripMenuItemText;
            this.prolongDepositFor1DayToolStripMenuItem.Text = DesignerLocStrings.ProlongDepositFor1DayToolStripMenuItemText;
            this.columnAutowidthByHeaderToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItemText;
            this.columnAutowidthByColumnToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByColumnToolStripMenuItemText;
            this.checkBox1.Text = DesignerLocStrings.CheckBox1Text;
            this.checkBox2.Text = DesignerLocStrings.CheckBox2Text;
            this.checkBox3.Text = DesignerLocStrings.CheckBox3Text;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_Entry, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_Empty, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }
        private async void ExchangeDepositListForm_Shown(object sender, EventArgs e)
		{
			_stateHelper.SetInitializedState();
            InitCommonView();
		    _proxyFilter = new ObservableCollectionProxyFilter<ExchangeDepositClientInfo>(
		        async deposit => await Task.FromResult(true)
		    );
		    _filteredObservableCollection =
		        await MyFilteredObservableCollectionImpl<ExchangeDepositClientInfo>.CreateInstance(
		            _sessionModel.DepositList,
		            _proxyFilter
		        ).ConfigureAwait(false);
		    _proxyComparer = new MyObservableCollectionProxyComparer<ExchangeDepositClientInfo>(
                Comparer<ExchangeDepositClientInfo>.Create(
                    (deposit1,deposit2) => -deposit1.CreatedDate.CompareTo(deposit2.CreatedDate)
                )
            );
		    _orderedObservableCollection = await MyOrderedObservableCollection<ExchangeDepositClientInfo>.CreateInstance(
		        _filteredObservableCollection,
		        _proxyComparer
		    ).ConfigureAwait(false);
		    _nProxy = new MyObservableCollectionProxyN(100);
            _nFirstObservableCollection = await MyNFirstObservableCollectionImpl<ExchangeDepositClientInfo>.CreateInstance(
		        _orderedObservableCollection,
		        _nProxy
		    ).ConfigureAwait(false);
            var binding = await ListViewCollectionChangedOneWayBinding<ExchangeDepositClientInfo>.CreateInstance(
                    depositListView,
                    _nFirstObservableCollection,
                    null,
                    (deposit, item) =>
                    {
                        item.Tag = deposit;
                        item.BackColor = deposit.Status.With(
                            _ =>
                            {
                                switch (_)
                                {
                                    case EExchangeDepositStatus.Expired:
                                    case EExchangeDepositStatus.Error:
                                        return Color.Red;
                                    case EExchangeDepositStatus.PaymentReceived:
                                        return Color.LawnGreen;
                                    case EExchangeDepositStatus.PaymentDetailsReceived:
                                        return Color.Yellow;
                                    default:
                                        return Color.White;
                                }
                            }
                        );
                        item.SubItems[0].Text = $"{deposit.CreatedDate}";
                        item.SubItems[1].Text = deposit.CurrencyCode;
                        item.SubItems[2].Text = $"{deposit.ExpectedValuePos:G29}";
                        item.SubItems[3].Text = $"{deposit.EstimatedPaymentSystemFeeNeg:G29}";
                        item.SubItems[4].Text = $"{deposit.EstimatedFeeNeg:G29}";
                        var totalValuePos = deposit.ExpectedValuePos
                            + (-deposit.EstimatedPaymentSystemFeeNeg)
                            + (-deposit.EstimatedFeeNeg);
                        item.SubItems[5].Text = $"{totalValuePos:G29}";
                        item.SubItems[6].Text = $"{deposit.AccountGuid}";
                        var depositStatusLocDict =
                            ClientGuiMainForm.LocStrings.ExchangeServerLocStringsInstance.ExchangeDepositStatusDict;
                        item.SubItems[7].Text = $"{depositStatusLocDict[deposit.Status]}";
                        item.SubItems[8].Text = $"{deposit.StatusComment}";
                        item.SubItems[9].Text = "...";
                        item.SubItems[10].Text = $"{deposit.ValidUntil}";
                        item.SubItems[11].Text = $"{deposit.DepositGuid}";
                        foreach (ColumnHeader column in depositListView.Columns)
                        {
                            column.Width = -1;
                        }
                    }
                );
		    _asyncSubscriptions.Add(
		        new CompositeMyAsyncDisposable(
                    binding,
                    _nFirstObservableCollection,
                    _orderedObservableCollection,
                    _filteredObservableCollection
                )
            );
		}
        /**/
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly List<IMyAsyncDisposable> _asyncSubscriptions = new List<IMyAsyncDisposable>();
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly DisposableObjectStateHelper _stateHelper
			= new DisposableObjectStateHelper("ExchangeDepositListForm");

		private async void ExchangeDepositListForm_FormClosing(object sender, FormClosingEventArgs e)
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

		private void ExchangeDepositListForm_Load(object sender, EventArgs e)
		{

		}

		private void depositListView_MouseUp(object sender, MouseEventArgs e)
		{
			ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
				{
					if (e.Button == MouseButtons.Right)
					{
					    if (depositListView.SelectedItems.Count != 0)
					    {
					        contextMenu_Entry.Show(Cursor.Position);
					    }
					    else
					    {
					        contextMenu_Empty.Show(Cursor.Position);
					    }
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
					using (await depositListView.GetLockSem().GetDisposable())
					{
						var firstSelectedItem = depositListView.SelectedItems.With(
							_ => _.Count > 0 ? _[0] : null);
						if (firstSelectedItem == null)
							return;
						var currentItemDeposit = (ExchangeDepositClientInfo) firstSelectedItem.Tag;
					    if (currentItemDeposit == null)
					        return;
					    if (string.IsNullOrWhiteSpace(currentItemDeposit.PaymentDetailsSerialized))
					        return;
					    if (currentItemDeposit.Status.In(
					        EExchangeDepositStatus.Created,
					        EExchangeDepositStatus.SentToPaymentService,
                            EExchangeDepositStatus.Error
                            ))
					    {
					        return;
					    }
                        if (currentItemDeposit.Status.In(
                            EExchangeDepositStatus.Expired,
                            EExchangeDepositStatus.PaymentReceived))
                        {
                            if (
                                await MessageBoxAsync.ShowAsync(this,
                                    LocStrings.Messages.PaymentDetailsExpired,
                                    ClientGuiMainForm.LocStrings.Messages.InformationMessageCaption,
                                    MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Error
                                ) == DialogResult.Cancel
                            )
                                return;
                        }
					    var pd = currentItemDeposit.PaymentDetailsSerialized.ParseJsonToType<ExchangePaymentDetails>();
                        pd.CheckMe();
                        new ExchangePaymentDetailsEditOrShowForm(
                            pd,
                            EExchangePaymentDetailsEditOrShowFormMode.Show, 
                            "",
                            true
                        ).Show(this);
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
                    using (await depositListView.GetLockSem().GetDisposable())
                    {
                        var firstSelectedItem = depositListView.SelectedItems.With(
                            _ => _.Count > 0 ? _[0] : null);
                        if (firstSelectedItem == null)
                            return;
                        var currentItemDeposit = (ExchangeDepositClientInfo)firstSelectedItem.Tag;
                        Assert.NotNull(currentItemDeposit);
                        Clipboard.SetText(
                            currentItemDeposit.StatusComment.With(
                                _ => _ == "" ? " " : _
                            )
                        );
                    }
                },
                _stateHelper,
                _logger
            );
        }

        private void columnAutowidthByHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            depositListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void columnAutowidthByColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            depositListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void prolongDepositFor1DayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                async () =>
                {
                    var firstSelectedItem = depositListView.SelectedItems.With(
                            _ => _.Count > 0 ? _[0] : null);
                    if (firstSelectedItem == null)
                        return;
                    var currentItemDeposit = (ExchangeDepositClientInfo)firstSelectedItem.Tag;
                    if(currentItemDeposit == null)
                        return;
                    if (currentItemDeposit.Status != EExchangeDepositStatus.PaymentDetailsReceived)
                        return;
                    var session = _sessionGetter();
                    if(session == null)
                        return;
                    var locStrings = ClientGuiMainForm.LocStrings.ExchangeServerLocStringsInstance
                        .DepositListFormLocStringsInstance;
                    using (var wrp = await ProgressCancelFormWraper.CreateInstance(
                        locStrings.ProlongDepositProgressFormCaption,this))
                    {
                        await session.ProlongDeposit(
                            currentItemDeposit.DepositGuid,
                            currentItemDeposit.AccountGuid,
                            wrp.Token
                        );
                        await MessageBoxAsync.ShowAsync(
                            this,
                            locStrings.ProlongDepositSuccess,
                            "",
                            icon: MessageBoxIcon.Information
                        );
                    }
                },
                _stateHelper,
                _logger
            );
        }

	    private string _orderPropName = nameof(ExchangeDepositClientInfo.CreatedDate);
	    private bool _orderAsc = false;
        private void depositListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    var columnNum = e.Column;
                    if (columnNum.In(0, 1, 2, 5, 6, 7, 10))
                    {
                        string newOrderPropName = 
                            columnNum == 0 ? nameof(ExchangeDepositClientInfo.CreatedDate)
                            : columnNum == 1 ? nameof(ExchangeDepositClientInfo.CurrencyCode) 
                            : columnNum == 2 ? nameof(ExchangeDepositClientInfo.ExpectedValuePos)
                            : columnNum == 5 ? nameof(ExchangeDepositClientInfo.FinalPaidTotalValuePos)
                            : columnNum == 6 ? nameof(ExchangeDepositClientInfo.AccountGuid)
                            : columnNum == 7 ? nameof(ExchangeDepositClientInfo.Status)
                            : nameof(ExchangeDepositClientInfo.ValidUntil);
                        var newOrderAsc = _orderPropName == newOrderPropName ? !_orderAsc : true;
                        _orderPropName = newOrderPropName;
                        _orderAsc = newOrderAsc;
                        _proxyComparer.Comparer = Comparer<ExchangeDepositClientInfo>.Create(
                            (dep1, dep2) =>
                            {
                                int result;
                                switch (columnNum)
                                {
                                    default:
                                    case 0:
                                        result = dep1.CreatedDate.CompareTo(dep2.CreatedDate);
                                        break;
                                    case 1:
                                        result = dep1.CurrencyCode.CompareTo(dep2.CurrencyCode);
                                        break;
                                    case 2:
                                        result = dep1.ExpectedValuePos.CompareTo(dep2.ExpectedValuePos);
                                        break;
                                    case 5:
                                        result = dep1.FinalPaidTotalValuePos.CompareTo(dep2.FinalPaidTotalValuePos);
                                        break;
                                    case 6:
                                        result = dep1.AccountGuid.CompareTo(dep2.AccountGuid);
                                        break;
                                    case 7:
                                        result = ((sbyte) dep1.Status).CompareTo((sbyte) dep2.Status);
                                        break;
                                    case 10:
                                        result = dep1.ValidUntil.CompareTo(dep2.ValidUntil);
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ChangeFilter();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ChangeFilter();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            ChangeFilter();
        }

	    private void ChangeFilter()
	    {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    var selectActive = checkBox1.Checked;
                    var selectComplete = checkBox2.Checked;
                    var selectFaultOrExpired = checkBox3.Checked;
                    _proxyFilter.Predicate = async deposit =>
                    {
                        if (selectActive && deposit.Status.In(
                                EExchangeDepositStatus.Created,
                                EExchangeDepositStatus.PaymentDetailsReceived,
                                EExchangeDepositStatus.SentToPaymentService
                            ))
                        {
                            return true;
                        }
                        if (selectComplete && deposit.Status.In(EExchangeDepositStatus.PaymentReceived))
                            return true;
                        if (selectFaultOrExpired &&
                            deposit.Status.In(EExchangeDepositStatus.Error, EExchangeDepositStatus.Expired))
                            return true;
                        return await Task.FromResult(false);
                    };
                },
                _stateHelper,
                _logger
            );
	    }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    var newN = int.Parse((string)comboBox1.SelectedItem);
                    _nProxy.N = newN;
                },
                _stateHelper,
                _logger
            );
        }
    }

    public class ExchangeDepositListFormLocStrings
    {
        public class MessagesLocStrings
        {
            public string PaymentDetailsExpired = "Payment details expired, just for reading!";
        }
        public MessagesLocStrings Messages = new MessagesLocStrings();
    }

    public class ExchangeDepositListFormDesignerLocStrings
    {
        public string DateColumnHeaderText = "Date";
        public string CurrencyCodeColumnHeaderText = "Currency code";
        public string ValueColumnHeaderText = "Value";
        public string EstimatedPsFeeColumnHeaderText = "Estimated payment system fee";
        public string EstimatedFeeColumnHeaderText = "Estimated fee";
        public string TotalColumnHeaderText = "Total";
        public string AccountColumnHeaderText = "Account GUID";
        public string StatusColumnHeaderText = "Status";
        public string StatusCommentColumnHeaderText = "Status comment";
        public string PaymentDetailsColumnHeaderText = "Payment details";
        public string ValidUntilColumnHeaderText = "Valid until";
        public string DepositGuidColumnHeaderText = "Deposit GUID";
        public string CopyPaymentDetailsToolStripMenuItemText = "View payment details";
        public string CopyStatusTextToolStripMenuItemText = "Copy status comment";
        public string ProlongDepositFor1DayToolStripMenuItemText = "Prolong deposit for 1 day more";
        public string ColumnAutowidthByHeaderToolStripMenuItemText = "Column autowidth by header";
        public string ColumnAutowidthByColumnToolStripMenuItemText = "Column autowidth by content";
        public string CheckBox1Text = "Active";
        public string CheckBox2Text = "Complete";
        public string CheckBox3Text = "Fault or expired";
        public string Text = "Deposit list";
    }
}
