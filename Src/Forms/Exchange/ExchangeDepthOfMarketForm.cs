using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Exchange;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib.ExchangeServerSession;
using BtmI2p.GeneralClientInterfaces.ExchangeServer;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    public partial class ExchangeDepthOfMarketForm : Form
    {
		private readonly IExchangeClientModel _exchangeModel;
		private IExchangeServerSessionModelData SessionData 
            => _exchangeModel.SessionModel.Data;
        private readonly Func<ExchangeServerSession> _exchangeSessionGetter;
		private readonly ClientGuiMainForm _mainFormInstance;
	    private readonly string _secCode;
        public ExchangeDepthOfMarketForm(
			string secCode,
			IExchangeClientModel exchangeModel,
			Func<ExchangeServerSession> exchangeSessionGetter,
			ClientGuiMainForm mainFormInstance
		)
        {
			Assert.NotNull(secCode);
			Assert.InRange(secCode.Length,1,20);
	        _secCode = secCode;
			Assert.NotNull(exchangeModel);
			_exchangeModel = exchangeModel;
			Assert.NotNull(exchangeSessionGetter);
			_exchangeSessionGetter = exchangeSessionGetter;
			Assert.NotNull(mainFormInstance);
			_mainFormInstance = mainFormInstance;
            InitializeComponent();
        }
		private readonly DisposableObjectStateHelper _stateHelper
			= new DisposableObjectStateHelper();
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

		private async void ExchangeDepthOfMarketForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_cts.Cancel();
			await _stateHelper.MyDisposeAsync();
			foreach (var subscription in _subscriptions)
			{
				subscription.Dispose();
			}
			_subscriptions.Clear();
			await SessionData.SecCodesToUpdateDomEntries.WithAsyncLockSem(
				_ => _.Remove(_secCode)
			);
			MyNotifyPropertyChangedArgs.RaiseProperyChanged(
				SessionData,
				_ => _.SecCodesToUpdateDomEntries
			);
            _cts.Dispose();
		}
        public static ExchangeDepthOfMarketFormDesignerLocStrings DesignerLocStrings = new ExchangeDepthOfMarketFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.columnHeader1.Text = DesignerLocStrings.ColumnHeader1Text;
            this.columnHeader2.Text = DesignerLocStrings.ColumnHeader2Text;
            this.columnHeader3.Text = DesignerLocStrings.ColumnHeader3Text;
            this.newOrderToolStripMenuItem.Text = DesignerLocStrings.NewOrderToolStripMenuItemText;
            this.cancelOrdersToolStripMenuItem.Text = DesignerLocStrings.CancelOrdersToolStripMenuItemText;
            this.label1.Text = DesignerLocStrings.Label1Text;
            this.label2.Text = DesignerLocStrings.Label2Text;
            this.chartToolStripMenuItem.Text = DesignerLocStrings.ChartToolStripMenuItemText;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_domEntry, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }
        private void ExchangeDepthOfMarketForm_Shown(object sender, EventArgs e)
		{
			_stateHelper.SetInitializedState();
            InitCommonView();
			ClientGuiMainForm.BeginInvokeProper(
				this,
				_stateHelper,
				_log,
				async () =>
				{
					ClientGuiMainForm.FlattenListViewItems(
						domListView.Items,
						20,
						() =>
						{
							var newItem = new ListViewItem();
							newItem.Font = domListView.Font;
							newItem.SubItems.AddRange(
								Enumerable.Repeat(
									"", 
									domListView.Columns.Count - 1
								).ToArray()
							);
							return newItem;
						}
					);
					for (int i = 0; i < 10; i++)
					{
						domListView.Items[i].BackColor = Color.Orange;
						domListView.Items[i].Tag = (DomEntry)null;
					}
					for (int i = 10; i < 20; i++)
					{
						domListView.Items[i].BackColor = Color.LightGreen;
						domListView.Items[i].Tag = (DomEntry)null;
					}
					/**/
					_subscriptions.Add(
						SessionData.PropertyChangedSubject.Where(
							_ => _.PropertyName == SessionData.MyNameOfProperty(__ => __.DomEntries)
						).BufferNotEmpty(TimeSpan.FromSeconds(2.0))
						.Subscribe(_ => UpdateDomListView())
					);
					UpdateDomListView();
					await SessionData.SecCodesToUpdateDomEntries.WithAsyncLockSem(
						_ => _.AddLast(_secCode)
					);
					MyNotifyPropertyChangedArgs.RaiseProperyChanged(
						SessionData,
						_ => _.SecCodesToUpdateDomEntries
					);
				}
			);
		} 
		private readonly SemaphoreSlim _updateDomListViewLockSem = new SemaphoreSlim(1);
		private void UpdateDomListView()
	    {
			ClientGuiMainForm.BeginInvokeProper(
				this,
				_stateHelper,
				_log,
				async () =>
				{
					var lockSemCalledWrapper = _updateDomListViewLockSem.GetCalledWrapper();
					lockSemCalledWrapper.Called = true;
					using (await _updateDomListViewLockSem.GetDisposable(true))
					{
						while (!_cts.IsCancellationRequested && lockSemCalledWrapper.Called)
						{
							lockSemCalledWrapper.Called = false;
							var currentDom = await SessionData.DomEntries.WithAsyncLockSem(
								_ => _.ContainsKey(_secCode) ? _[_secCode] : new List<DomEntry>()
							);
							var myOrdersPriceList = (
                                await SessionData.OrderList.WhereAsync(
                                    _ => _.SecurityCode == _secCode && _.Status == EExchangeOrderStatus.Active
                                )
                            ).Select(_ => _.Price)
							.Distinct()
							.ToList();
							using (await domListView.GetLockSem().GetDisposable())
							{
								var selectedPrice = domListView.SelectedItems.Cast<ListViewItem>()
									.Where(_ => _.Tag != null).Select(_ => ((DomEntry) _.Tag).Price).Distinct().ToList();
								domListView.SuspendLayout();
								try
								{
                                    /*Ask*/
                                    var askDomEntries = currentDom
                                        .Where(_ => _.Side == EExchangeOrderSide.Sell)
                                        .OrderBy(_ => _.Price)
                                        .ToList();
                                    for (int i = 0; i < (10 - askDomEntries.Count); i++)
									{
										var item = domListView.Items[i];
										item.Tag = (DomEntry)null;
										item.Selected = false;
										item.SubItems[0].Text = string.Empty;
										item.SubItems[1].Text = string.Empty;
										item.SubItems[2].Text = string.Empty;
										item.Font = new Font(item.Font, FontStyle.Regular);
									}
									for (
										int itemIndex = 9, askEntryIndex = 0; 
										itemIndex >= 0 && askEntryIndex < askDomEntries.Count; 
										itemIndex--, askEntryIndex++
									)
									{
										var item = domListView.Items[itemIndex];
										var domEntry = askDomEntries[askEntryIndex];
										item.Tag = domEntry;
										item.SubItems[0].Text = "";
										item.SubItems[1].Text = $"{domEntry.Price}";
										item.SubItems[2].Text = $"{domEntry.Volume}";
										item.Font = new Font(
											item.Font, 
											myOrdersPriceList.Contains(domEntry.Price) 
												? FontStyle.Bold 
												: FontStyle.Regular
										);
										item.Selected = selectedPrice.Contains(domEntry.Price);
									}
									/*Bid*/
									var bidDomEntries = currentDom
										.Where(_ => _.Side == EExchangeOrderSide.Buy)
										.OrderByDescending(_ => _.Price)
										.ToList();
                                    for (int i = 10 + bidDomEntries.Count; i < 20; i++)
                                    {
                                        var item = domListView.Items[i];
                                        item.Tag = (DomEntry)null;
                                        item.Selected = false;
                                        item.SubItems[0].Text = string.Empty;
                                        item.SubItems[1].Text = string.Empty;
                                        item.SubItems[2].Text = string.Empty;
                                        item.Font = new Font(item.Font, FontStyle.Regular);
                                    }
                                    for (
										int i = 10, j = 0; 
										i < 20 && j < bidDomEntries.Count; 
										i++, j++
									)
									{
										var item = domListView.Items[i];
										var domEntry = bidDomEntries[j];
										item.Tag = domEntry;
										item.SubItems[0].Text = $"{domEntry.Volume}";
										item.SubItems[1].Text = $"{domEntry.Price}";
										item.SubItems[2].Text = "";
										item.Font = new Font(
											item.Font,
											myOrdersPriceList.Contains(domEntry.Price)
												? FontStyle.Bold
												: FontStyle.Regular
										);
										item.Selected = selectedPrice.Contains(domEntry.Price);
									}
								}
								finally
								{
									domListView.ResumeLayout();
								}
							}
						}
					}
				}
			);
	    }

		private void domListView_MouseUp(object sender, MouseEventArgs e)
		{
			
		}

		private void newOrderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClientGuiMainForm.HandleControlActionProper(
                this,
                async () =>
				{
					var selectedDomEntry = await domListView.WithAsyncLockSem(
						_ => _.SelectedItems
							.Cast<ListViewItem>()
							.Select(item => (DomEntry) item.Tag)
							.FirstOrDefault()
					);
					var newOrderForm = new ExchangeNewOrderForm(
						_exchangeModel,
						_exchangeSessionGetter,
						_mainFormInstance,
						selectedDomEntry == null
							? null
							: new ExchangeSecurityOrderClientInfo()
							{
								SecurityCode = _secCode,
								Qty = 1,
								FilledQty = 0,
								Status = EExchangeOrderStatus.Active,
								Price = selectedDomEntry.Price,
								Side = selectedDomEntry.Side
							}
					);
					newOrderForm.Show(this);
				},
				_stateHelper,
				_log
			);
		}
        // Cancel orders with selected price
        private void cancelOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                async () =>
                {
                    var selectedDomEntry = await domListView.WithAsyncLockSem(
                        _ => _.SelectedItems
                            .Cast<ListViewItem>()
                            .Select(item => (DomEntry)item.Tag)
                            .FirstOrDefault()
                    );
                    if (selectedDomEntry == null)
                        return;
                    var price = selectedDomEntry.Price;
                    var activeOrders = await _exchangeModel.SessionModel.Data.OrderList.WhereAsync(
                        _ => _.Status == EExchangeOrderStatus.Active && _.Price == price
                    ).ConfigureAwait(false);
                    if (activeOrders.Count == 0)
                        return;
                    var locStrings =
                        ClientGuiMainForm.LocStrings.ExchangeServerLocStringsInstance.DepthOfMarketLocStringsInstance;
                    if (
                        await MessageBoxAsync.ShowAsync(
                            this,
                            locStrings.CancelOrderConfirmations.InjectSingleValue(
                                "N",
                                activeOrders.Count
                            ),
                            locStrings.ConfirmationCaption,
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question
                        ) == DialogResult.OK
                    )
                    {
                        if (!_mainFormInstance.CheckExchangeClientConstraints())
                            return;
                        var exchangeSession = _exchangeSessionGetter();
                        Assert.NotNull(exchangeSession);
                        foreach (var activeOrder in activeOrders)
                        {
                            var locStringsCancel = ClientGuiMainForm.LocStrings
                                .ExchangeServerLocStringsInstance.CancelOrderLocStringsInstance;
                            using (
                                var wrp = await ProgressCancelFormWraper.CreateInstance(
                                    locStringsCancel.ProgressFormCaption.Inject(
                                        new
                                        {
                                            OrderGuid = activeOrder.OrderGuid
                                        }
                                    ),
                                    this
                                )
                            )
                            {
                                wrp.ProgressInst.ReportProgress(20, locStringsCancel.ProgressFormReport1);
                                await exchangeSession.CancelOrder(
                                    activeOrder.OrderGuid,
                                    CancellationTokenSource.CreateLinkedTokenSource(
                                        wrp.Token,
                                        _cts.Token
                                    ).Token
                                );
                            }
                        }
                    }
                },
                _stateHelper,
                _log
            );
        }
        //Chart
        private void chartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    new ExchangeChartCandlesForm(
                        _exchangeModel.SessionModel.Data,
                        _secCode
                    ).Show();
                },
                _stateHelper,
                _log
            );
        }
    }
    public class ExchangeDepthOfMarketFormDesignerLocStrings
    {
        public string ColumnHeader1Text = "Bid";
        public string ColumnHeader2Text = "Price";
        public string ColumnHeader3Text = "Ask";
        public string NewOrderToolStripMenuItemText = "New order";
        public string CancelOrdersToolStripMenuItemText = "Cancel orders";
        public string Label1Text = "Actions:";
        public string Label2Text = "Depth of market:";
        public string ChartToolStripMenuItemText = "Chart";
        public string Text = "Depth of market";
    }
}
