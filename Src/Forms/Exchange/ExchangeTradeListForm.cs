using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    public partial class ExchangeTradeListForm : Form
    {
        private readonly IExchangeServerSessionModelData _sessionModel;
        public ExchangeTradeListForm(
            IExchangeServerSessionModelData sessionModel)
        {
            _sessionModel = sessionModel;
            InitializeComponent();
        }

        private readonly ObservableCollectionProxyFilter<ExchangeSecurityTradeClientInfo> _proxyFilter
            = new ObservableCollectionProxyFilter<ExchangeSecurityTradeClientInfo>(
                async trade => await Task.FromResult(true)
            );

        private readonly MyObservableCollectionProxyComparer<ExchangeSecurityTradeClientInfo> _proxyComparer
            = new MyObservableCollectionProxyComparer<ExchangeSecurityTradeClientInfo>(
                Comparer<ExchangeSecurityTradeClientInfo>.Create(
                    (trd1, trd2) => -trd1.Date.CompareTo(trd2.Date)
                )
            );

        private readonly MyObservableCollectionProxyN _proxyN 
            = new MyObservableCollectionProxyN(100);
        public static ExchangeTradeListFormDesignerLocStrings DesignerLocStrings = new ExchangeTradeListFormDesignerLocStrings();

        private void InitCommonView()
        {
            this.dateHeader.Text = DesignerLocStrings.DateHeaderText;
            this.secCodeHeader.Text = DesignerLocStrings.SecCodeHeaderText;
            this.qtyHeader.Text = DesignerLocStrings.QtyHeaderText;
            this.priceHeader.Text = DesignerLocStrings.PriceHeaderText;
            this.bidOrderGuidHeader.Text = DesignerLocStrings.BidOrderGuidHeaderText;
            this.askOrderGuidHeader.Text = DesignerLocStrings.AskOrderGuidHeaderText;
            this.tradeGuidHeader.Text = DesignerLocStrings.TradeGuidHeaderText;
            this.checkBox1.Text = DesignerLocStrings.CheckBox1Text;
            this.checkBox2.Text = DesignerLocStrings.CheckBox2Text;
            this.sideHeader.Text = DesignerLocStrings.SideHeaderText;
            this.columnAutowidthByHeaderToolStripMenuItem.Text =
                DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItemText;
            this.columnAutowidthByContentToolStripMenuItem.Text =
                DesignerLocStrings.ColumnAutowidthByContentToolStripMenuItemText;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenuStrip1,
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }

        private async void ExchangeTradeListForm_Shown(object sender, EventArgs e)
        {
            _stateHelper.SetInitializedState();
            InitCommonView();
            var filteredCollection =
                await MyFilteredObservableCollectionImpl<ExchangeSecurityTradeClientInfo>.CreateInstance(
                    _sessionModel.TradesCollection,
                    _proxyFilter
                );
            var orderedCollection = await MyOrderedObservableCollection<ExchangeSecurityTradeClientInfo>.CreateInstance(
                filteredCollection,
                _proxyComparer
            );
            var firstNCollection =
                await MyNFirstObservableCollectionImpl<ExchangeSecurityTradeClientInfo>.CreateInstance(
                    orderedCollection,
                    _proxyN
                );
            var locStrings =
                ClientGuiMainForm.LocStrings.ExchangeServerLocStringsInstance.ShowTradeListFormLocStringsInstance;
            var binding = await ListViewCollectionChangedOneWayBinding<ExchangeSecurityTradeClientInfo>.CreateInstance(
                listView1,
                firstNCollection,
                null,
                (trade, item) =>
                {
                    item.Tag = trade;
                    var tradeSide = trade.GetTradeSide();
                    item.BackColor = tradeSide == EExchangeOrderSide.Buy
                        ? Color.LightGreen
                        : Color.Orange;
                    item.SubItems[0].Text = $"{trade.Date}";
                    item.SubItems[1].Text = $"{trade.SecCode}";
                    item.SubItems[2].Text = tradeSide == EExchangeOrderSide.Buy ? locStrings.BuySide : locStrings.SellSide;
                    item.SubItems[3].Text = $"{trade.Qty}";
                    item.SubItems[4].Text = $"{trade.Price:G29}";
                    item.SubItems[5].Text = $"{trade.BidOrderGuid}";
                    item.SubItems[6].Text = $"{trade.AskOrderGuid}";
                    item.SubItems[7].Text = $"{trade.TradeGuid}";
                }
            );
            _asyncSubscriptions.Add(
                new CompositeMyAsyncDisposable(
                    binding,
                    firstNCollection,
                    orderedCollection,
                    filteredCollection
                )
            );
        }

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        /**/
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly List<IMyAsyncDisposable> _asyncSubscriptions = new List<IMyAsyncDisposable>(); 
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper();

        private async void ExchangeTradeListForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void ExchangeTradeListForm_FormClosed(object sender, FormClosedEventArgs e)
        {
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

        private void UpdateFilterPredicate()
        {
            var buy = checkBox1.Checked;
            var sell = checkBox2.Checked;
            _proxyFilter.Predicate = async trade =>
            {
                if (buy && trade.GetTradeSide() == EExchangeOrderSide.Buy)
                    return true;
                if (sell && trade.GetTradeSide() == EExchangeOrderSide.Sell)
                    return true;
                return await Task.FromResult(false);
            };
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilterPredicate();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFilterPredicate();
        }

        private string _orderPropName = nameof(ExchangeSecurityTradeClientInfo.Date);
        private bool _orderAsc = false;
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    var columnNum = e.Column;
                    if (columnNum.In(0, 1, 2, 3, 4, 5, 6, 7))
                    {
                        string newOrderPropName =
                            columnNum == 0 ? nameof(ExchangeSecurityTradeClientInfo.Date)
                            : columnNum == 1 ? nameof(ExchangeSecurityTradeClientInfo.SecCode)
                            : columnNum == 2 ? nameof(ExchangeSecurityTradeClientInfo.GetTradeSide)
                            : columnNum == 3 ? nameof(ExchangeSecurityTradeClientInfo.Qty)
                            : columnNum == 4 ? nameof(ExchangeSecurityTradeClientInfo.Price)
                            : columnNum == 5 ? nameof(ExchangeSecurityTradeClientInfo.BidOrderGuid)
                            : columnNum == 6 ? nameof(ExchangeSecurityTradeClientInfo.AskOrderGuid)
                            : nameof(ExchangeSecurityTradeClientInfo.TradeGuid);
                        var newOrderAsc = _orderPropName == newOrderPropName ? !_orderAsc : true;
                        _orderPropName = newOrderPropName;
                        _orderAsc = newOrderAsc;
                        _proxyComparer.Comparer = Comparer<ExchangeSecurityTradeClientInfo>.Create(
                            (trd1, trd2) =>
                            {
                                int result;
                                switch (columnNum)
                                {
                                    default:
                                    case 0:
                                        result = trd1.Date.CompareTo(trd2.Date);
                                        break;
                                    case 1:
                                        result = trd1.SecCode.CompareTo(trd2.SecCode);
                                        break;
                                    case 2:
                                        result = trd1.GetTradeSide().CompareTo(trd2.GetTradeSide());
                                        break;
                                    case 3:
                                        result = trd1.Qty.CompareTo(trd2.Qty);
                                        break;
                                    case 4:
                                        result = trd1.Price.CompareTo(trd2.Price);
                                        break;
                                    case 5:
                                        result = trd1.BidOrderGuid.CompareTo(trd2.BidOrderGuid);
                                        break;
                                    case 6:
                                        result = trd1.AskOrderGuid.CompareTo(trd2.AskOrderGuid);
                                        break;
                                    case 7:
                                        result = trd1.TradeGuid.CompareTo(trd2.TradeGuid);
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
    public class ExchangeTradeListFormDesignerLocStrings
    {
        public string DateHeaderText = "Date";
        public string SecCodeHeaderText = "Security code";
        public string QtyHeaderText = "Qty";
        public string PriceHeaderText = "Price";
        public string BidOrderGuidHeaderText = "Bid order GUID";
        public string AskOrderGuidHeaderText = "Ask order GUID";
        public string TradeGuidHeaderText = "Trade GUID";
        public string CheckBox1Text = "Buy";
        public string CheckBox2Text = "Sell";
        public string SideHeaderText = "Side";
        public string ColumnAutowidthByHeaderToolStripMenuItemText = "Column autowidth by header";
        public string ColumnAutowidthByContentToolStripMenuItemText = "Column autowidth by content";
        public string Text = "Trade list";
    }
}
