using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Exchange;
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
    public partial class ExchangeOrderListForm : Form
    {
		private readonly IExchangeClientModel _exchangeModel;

	    private IExchangeServerSessionModelData SessionData => _exchangeModel.SessionModel.Data;
        private readonly Func<ExchangeServerSession> _exchangeServerSessionGetter;
	    private readonly ClientGuiMainForm _mainFormInstance;
        public ExchangeOrderListForm(
			IExchangeClientModel exchangeModel,
			Func<ExchangeServerSession> exchangeServerSessionGetter,
			ClientGuiMainForm mainFormInstance
		)
        {
			Assert.NotNull(exchangeModel);
			_exchangeModel = exchangeModel;
			Assert.NotNull(exchangeServerSessionGetter);
			_exchangeServerSessionGetter = exchangeServerSessionGetter;
			Assert.NotNull(mainFormInstance);
	        _mainFormInstance = mainFormInstance;
            _proxyFilter.Predicate = async _ => await Task.FromResult(true);
            InitializeComponent();
        }

		private async void ExchangeOrderListForm_FormClosing(
			object sender, FormClosingEventArgs e)
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

	    private readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly DisposableObjectStateHelper _stateHelper 
			= new DisposableObjectStateHelper("ExchangeOrderListForm");
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly List<IMyAsyncDisposable> _asyncSubscriptions = new List<IMyAsyncDisposable>();
        private readonly ObservableCollectionProxyFilter<ExchangeSecurityOrderClientInfo> _proxyFilter 
            = new ObservableCollectionProxyFilter<ExchangeSecurityOrderClientInfo>(async _ => await Task.FromResult(true));
        private readonly MyObservableCollectionProxyComparer<ExchangeSecurityOrderClientInfo> _proxyComparer
            = new MyObservableCollectionProxyComparer<ExchangeSecurityOrderClientInfo>(
                Comparer<ExchangeSecurityOrderClientInfo>.Create(
                    (ord1, ord2) => -ord1.SentDate.CompareTo(ord2.SentDate)
                )
            );
        private readonly MyObservableCollectionProxyN _proxyN
            = new MyObservableCollectionProxyN(100);
        /**/
        public static ExchangeOrderListFormDesignerLocStrings DesignerLocStrings = new ExchangeOrderListFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.columnHeader9.Text = DesignerLocStrings.ColumnHeader9Text;
            this.columnHeader1.Text = DesignerLocStrings.ColumnHeader1Text;
            this.columnHeader2.Text = DesignerLocStrings.ColumnHeader2Text;
            this.columnHeader3.Text = DesignerLocStrings.ColumnHeader3Text;
            this.columnHeader4.Text = DesignerLocStrings.ColumnHeader4Text;
            this.columnHeader5.Text = DesignerLocStrings.ColumnHeader5Text;
            this.columnHeader6.Text = DesignerLocStrings.ColumnHeader6Text;
            this.columnHeader11.Text = DesignerLocStrings.ColumnHeader11Text;
            this.columnHeader7.Text = DesignerLocStrings.ColumnHeader7Text;
            this.columnHeader8.Text = DesignerLocStrings.ColumnHeader8Text;
            this.columnHeader10.Text = DesignerLocStrings.ColumnHeader10Text;
            this.checkBox4.Text = DesignerLocStrings.CheckBox4Text;
            this.checkBox2.Text = DesignerLocStrings.CheckBox2Text;
            this.checkBox3.Text = DesignerLocStrings.CheckBox3Text;
            this.checkBox5.Text = DesignerLocStrings.CheckBox5Text;
            this.checkBox1.Text = DesignerLocStrings.CheckBox1Text;
            this.columnAutowidthByHeaderToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItemText;
            this.columnAutowidthByContentToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByContentToolStripMenuItemText;
            this.cancelOrderToolStripMenuItem.Text = DesignerLocStrings.CancelOrderToolStripMenuItemText;
            this.newOrderToolStripMenuItem1.Text = DesignerLocStrings.NewOrderToolStripMenuItem1Text;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_noSelectedOrders, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_selectedOrder, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }
        private async void ExchangeOrderListForm_Shown(object sender, EventArgs e)
		{
			_stateHelper.SetInitializedState();
            InitCommonView();
            var filteredCollectionChanged =
                await MyFilteredObservableCollectionImpl<ExchangeSecurityOrderClientInfo>.CreateInstance(
                    SessionData.OrderList,
                    _proxyFilter
                );
            var orderedCollectionChanged =
                await MyOrderedObservableCollection<ExchangeSecurityOrderClientInfo>.CreateInstance(
                    filteredCollectionChanged,
                    _proxyComparer
                );
            var nFirstCollectionChanged =
                await MyNFirstObservableCollectionImpl<ExchangeSecurityOrderClientInfo>.CreateInstance(
                    orderedCollectionChanged,
                    _proxyN
                );
            var binding = await ListViewCollectionChangedOneWayBinding<ExchangeSecurityOrderClientInfo>.CreateInstance(
                listView1,
                nFirstCollectionChanged,
                null,
                (order, item) =>
                {
                    item.Tag = order;
                    item.Font = new Font(item.Font,
                        order.Status == EExchangeOrderStatus.Active
                            ? FontStyle.Italic
                            : order.Status == EExchangeOrderStatus.Cancelled
                                ? FontStyle.Strikeout
                                : FontStyle.Bold
                        );
                    item.BackColor = order.Side == EExchangeOrderSide.Buy
                        ? Color.LightGreen
                        : Color.Orange;
                    item.SubItems[0].Text = $"{order.SentDate}";
                    item.SubItems[1].Text = $"{order.SecurityCode}";
                    item.SubItems[2].Text = $"{order.Side}";
                    item.SubItems[3].Text = $"{order.Price}";
                    item.SubItems[4].Text = $"{order.Status}";
                    item.SubItems[5].Text = $"{order.Qty}";
                    item.SubItems[6].Text = $"{order.FilledQty}";
                    item.SubItems[7].Text = $"{order.Price * order.Qty}";
                    item.SubItems[8].Text = $"{order.BaseAccountGuid}";
                    item.SubItems[9].Text = $"{order.SecondAccountGuid}";
                    item.SubItems[10].Text = $"{order.OrderGuid}";
                }
                );
            _asyncSubscriptions.Add(
                new CompositeMyAsyncDisposable(
                    binding,
                    nFirstCollectionChanged,
                    orderedCollectionChanged,
                    filteredCollectionChanged
                )
            );
        }

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
		    UpdateFilterPredicate();
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
		    UpdateFilterPredicate();
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
		    UpdateFilterPredicate();
		}

		private void checkBox4_CheckedChanged(object sender, EventArgs e)
		{
		    UpdateFilterPredicate();
		}

		private void checkBox5_CheckedChanged(object sender, EventArgs e)
		{
		    UpdateFilterPredicate();
		}

        private void UpdateFilterPredicate()
        {
            var showActive = checkBox1.Checked;
            var showCancelled = checkBox2.Checked;
            var showFulfilled = checkBox3.Checked;
            var showBuyOrders = checkBox4.Checked;
            var showSellOrders = checkBox5.Checked;
            Func<ExchangeSecurityOrderClientInfo,Task<bool>> predicate = 
                async _ =>
                await Task.FromResult(
                (
                    (_.Status == EExchangeOrderStatus.Active && showActive)
                    || (_.Status == EExchangeOrderStatus.Cancelled && showCancelled)
                    || (_.Status == EExchangeOrderStatus.Fulfilled && showFulfilled)
                )
                && (
                    (_.Side == EExchangeOrderSide.Buy && showBuyOrders)
                    || (_.Side == EExchangeOrderSide.Sell && showSellOrders)
                )).ConfigureAwait(false);
            _proxyFilter.Predicate = predicate;
        }
        // Selected order menu, cancel order
        private void cancelOrderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClientGuiMainForm.HandleControlActionProper(
                this,
                async () =>
				{
					using (await listView1.GetLockSem().GetDisposable())
					{
					    var selectedOrder = listView1.SelectedItems
					        .Cast<ListViewItem>()
					        .Select(_ => _.Tag)
					        .Cast<ExchangeSecurityOrderClientInfo>()
					        .FirstOrDefault();
					    if (selectedOrder == null)
					        return;
                        var locStrings = ClientGuiMainForm.LocStrings
                            .ExchangeServerLocStringsInstance.CancelOrderLocStringsInstance;
                        if (selectedOrder.Status != EExchangeOrderStatus.Active)
					    {
                            ClientGuiMainForm.ShowErrorMessage(
                                this,
                                locStrings.OrderNotActiveErrorMessage
                            );
					        return;
					    }
					    var selectedOrderGuid = selectedOrder.OrderGuid;
						if (selectedOrderGuid == Guid.Empty)
							return;
						if (!_mainFormInstance.CheckExchangeClientConstraints())
							return;
						var session = _exchangeServerSessionGetter();
						Assert.NotNull(session);
						using (
							var wrp = await ProgressCancelFormWraper.CreateInstance(
								locStrings.ProgressFormCaption.Inject(
									new
									{
										OrderGuid = selectedOrderGuid
									}
								),
								this
							)
						)
						{
							wrp.ProgressInst.ReportProgress(20,locStrings.ProgressFormReport1);
							await session.CancelOrder(
								selectedOrderGuid,
								CancellationTokenSource.CreateLinkedTokenSource(
									wrp.Token,
									_cts.Token
								).Token
							);
						}
					}
				},
				_stateHelper,
				_log
			);
		}

		private void listView1_MouseUp(object sender, MouseEventArgs e)
		{
			ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
				{
					if (e.Button == MouseButtons.Right)
					{
						if (listView1.SelectedItems.Count == 0)
						{
							//contextMenu_noSelectedOrders.Show(Cursor.Position);
						}
						else
						{
							contextMenu_selectedOrder.Show(Cursor.Position);
						}
					}
				},
				_stateHelper,
				_log
			);
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
        // Selected item, new order
        private void newOrderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    var selectedOrder = listView1.SelectedItems
                            .Cast<ListViewItem>()
                            .Select(_ => _.Tag)
                            .Cast<ExchangeSecurityOrderClientInfo>()
                            .FirstOrDefault();
                    if (selectedOrder == null)
                        return;
                    var newOrderForm = new ExchangeNewOrderForm(
                        _exchangeModel,
                        _exchangeServerSessionGetter,
                        _mainFormInstance,
                        selectedOrder
                    );
                    newOrderForm.Show(this);
                },
                _stateHelper,
                _log
            );
        }
        private string _orderPropName = nameof(ExchangeSecurityOrderClientInfo.SentDate);
        private bool _orderAsc = false;
        // Sort
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    var columnNum = e.Column;
                    if (columnNum.In(0, 1, 2, 3, 4, 7, 8, 9, 10))
                    {
                        string newOrderPropName =
                            columnNum == 0 ? nameof(ExchangeSecurityOrderClientInfo.SentDate)
                            : columnNum == 1 ? nameof(ExchangeSecurityOrderClientInfo.SecurityCode)
                            : columnNum == 2 ? nameof(ExchangeSecurityOrderClientInfo.Side)
                            : columnNum == 3 ? nameof(ExchangeSecurityOrderClientInfo.Price)
                            : columnNum == 4 ? nameof(ExchangeSecurityOrderClientInfo.Status)
                            : columnNum == 7 ? "TotalVolumeProp"
                            : columnNum == 8 ? nameof(ExchangeSecurityOrderClientInfo.BaseAccountGuid)
                            : columnNum == 9 ? nameof(ExchangeSecurityOrderClientInfo.SecondAccountGuid)
                            : nameof(ExchangeSecurityOrderClientInfo.OrderGuid);
                        var newOrderAsc = _orderPropName == newOrderPropName ? !_orderAsc : true;
                        _orderPropName = newOrderPropName;
                        _orderAsc = newOrderAsc;
                        _proxyComparer.Comparer = Comparer<ExchangeSecurityOrderClientInfo>.Create(
                            (ord1, ord2) =>
                            {
                                int result;
                                switch (columnNum)
                                {
                                    default:
                                    case 0:
                                        result = ord1.SentDate.CompareTo(ord2.SentDate);
                                        break;
                                    case 1:
                                        result = ord1.SecurityCode.CompareTo(ord2.SecurityCode);
                                        break;
                                    case 2:
                                        result = ord1.Side.CompareTo(ord2.Side);
                                        break;
                                    case 3:
                                        result = ord1.Price.CompareTo(ord2.Price);
                                        break;
                                    case 4:
                                        result = ord1.Status.CompareTo(ord2.Status);
                                        break;
                                    case 7:
                                        result = (ord1.Price*ord1.Qty).CompareTo(ord2.Price*ord2.Qty);
                                        break;
                                    case 8:
                                        result = ord1.BaseAccountGuid.CompareTo(ord2.BaseAccountGuid);
                                        break;
                                    case 9:
                                        result = ord1.SecondAccountGuid.CompareTo(ord2.SecondAccountGuid);
                                        break;
                                    case 10:
                                        result = ord1.OrderGuid.CompareTo(ord2.OrderGuid);
                                        break;
                                }
                                return newOrderAsc ? result : -result;
                            }
                        );
                    }
                },
                _stateHelper,
                _log
            );
        }

        private void columnAutowidthByHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void columnAutowidthByContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.AutoResizeColumns(
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
                _log
            );
        }
    }
    public class ExchangeOrderListFormDesignerLocStrings
    {
        public string ColumnHeader9Text = "Date";
        public string ColumnHeader1Text = "Security code";
        public string ColumnHeader2Text = "Side";
        public string ColumnHeader3Text = "Price";
        public string ColumnHeader4Text = "Status";
        public string ColumnHeader5Text = "Qty";
        public string ColumnHeader6Text = "Filled qty";
        public string ColumnHeader11Text = "Total volume";
        public string ColumnHeader7Text = "Base account";
        public string ColumnHeader8Text = "Second account";
        public string ColumnHeader10Text = "Order GUID";
        public string CheckBox4Text = "Buy";
        public string CheckBox2Text = "Cancelled";
        public string CheckBox3Text = "Fullfilled";
        public string CheckBox5Text = "Sell";
        public string CheckBox1Text = "Active";
        public string ColumnAutowidthByHeaderToolStripMenuItemText = "Column autowidth by header";
        public string ColumnAutowidthByContentToolStripMenuItemText = "Column autowidth by content";
        public string CancelOrderToolStripMenuItemText = "Cancel order";
        public string NewOrderToolStripMenuItem1Text = "New order";
        public string Text = "Order list";
    }
}