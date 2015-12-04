using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Exchange;
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
    public partial class ExchangeSecurityListForm : Form
    {
		private readonly IExchangeClientModel _exchangeModel;
		private IExchangeServerSessionModelData _sessionData => _exchangeModel.SessionModel.Data;
        private readonly Func<ExchangeServerSession> _exchangeSessionGetter;
	    private readonly ClientGuiMainForm _mainFormInstance;
        public ExchangeSecurityListForm(
			IExchangeClientModel exchangeModel,
			Func<ExchangeServerSession> exchangeSessionGetter,
			ClientGuiMainForm mainFormInstance
        )
        {
			Assert.NotNull(exchangeModel);
	        _exchangeModel = exchangeModel;
			Assert.NotNull(exchangeSessionGetter);
	        _exchangeSessionGetter = exchangeSessionGetter;
			Assert.NotNull(mainFormInstance);
	        _mainFormInstance = mainFormInstance;
            InitializeComponent();
        }
        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper("ExchangeSecurityListForm");
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly List<IDisposable> _subscriptions = new List<IDisposable>(); 
        private readonly List<IMyAsyncDisposable> _asyncSubscriptions = new List<IMyAsyncDisposable>();
        public static ExchangeSecurityListFormDesignerLocStrings DesignerLocStrings = new ExchangeSecurityListFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.codeHeader.Text = DesignerLocStrings.CodeHeaderText;
            this.typeHeader.Text = DesignerLocStrings.TypeHeaderText;
            this.parentCodeHeader.Text = DesignerLocStrings.ParentCodeHeaderText;
            this.statusHeader.Text = DesignerLocStrings.StatusHeaderText;
            this.descriptionHeader.Text = DesignerLocStrings.DescriptionHeaderText;
            this.baseCurrencyHeader.Text = DesignerLocStrings.BaseCurrencyHeaderText;
            this.scaleHeader.Text = DesignerLocStrings.ScaleHeaderText;
            this.priceStepHeader.Text = DesignerLocStrings.PriceStepHeaderText;
            this.lotHeader.Text = DesignerLocStrings.LotHeaderText;
            this.expirationHeader.Text = DesignerLocStrings.ExpirationHeaderText;
            this.minPriceHeader.Text = DesignerLocStrings.MinPriceHeaderText;
            this.maxPriceHeader.Text = DesignerLocStrings.MaxPriceHeaderText;
            this.depthOfMarketToolStripMenuItem.Text = DesignerLocStrings.DepthOfMarketToolStripMenuItemText;
            this.chartToolStripMenuItem.Text = DesignerLocStrings.ChartToolStripMenuItemText;
            this.columnAutowidthByHeaderToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItemText;
            this.columnAutowidthByContentToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByContentToolStripMenuItemText;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_SelectedSecurity, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_Empty, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }
        private async void ExchangeSecurityListForm_Shown(object sender, EventArgs e)
        {
			_stateHelper.SetInitializedState();
            InitCommonView();
	        var binding = await ListViewCollectionChangedOneWayBinding<ExchangeSecurityClientInfo>.CreateInstance(
	            securityListView,
	            _sessionData.SecurityCollection,
	            null,
	            (securityInfo, item) =>
	            {
	                item.Tag = securityInfo;
	                item.SubItems[0].Text = $"{securityInfo.Code}";
	                item.SubItems[1].Text = $"{securityInfo.SecurityType}";
	                item.SubItems[2].Text = $"{securityInfo.ParentSecurityCode}";
	                item.SubItems[3].Text = $"{securityInfo.Status}";
	                item.SubItems[4].Text = $"{securityInfo.Description}";
	                item.SubItems[5].Text = $"{securityInfo.BaseCurrencyCode}";
	                item.SubItems[6].Text = $"{securityInfo.Scale}";
	                item.SubItems[7].Text = $"{securityInfo.PriceStep}";
	                item.SubItems[8].Text = $"{securityInfo.Lot}";
	                item.SubItems[9].Text = $"{securityInfo.Expiration}";
	                item.SubItems[10].Text = $"{securityInfo.MinPrice}";
	                item.SubItems[11].Text = $"{securityInfo.MaxPrice}";
	            }
	        );
            _asyncSubscriptions.Add(binding);
        }

        private async void ExchangeSecurityListForm_FormClosing(object sender, FormClosingEventArgs e)
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

		private void securityListView_MouseUp(object sender, MouseEventArgs e)
		{
			ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
				{
					if (e.Button == MouseButtons.Right)
					{
						if(_selectedSecurity != null)
							contextMenu_SelectedSecurity.Show(
								Cursor.Position
							);
						else
						{
						    contextMenu_Empty.Show(
                                Cursor.Position
                            );
						}
					}
				},
				_stateHelper,
				_log
			);
		}
		//private string _selectedSecurityCode = "";
        private ExchangeSecurityClientInfo _selectedSecurity = null;
		private void securityListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			ClientGuiMainForm.BeginInvokeProper(
				this,
				_stateHelper,
				_log,
				async () =>
				{
                    _selectedSecurity = securityListView.SelectedItems.With(
                        _ => _.Count > 0 
                            ? (ExchangeSecurityClientInfo)_[0].Tag 
                            : default(ExchangeSecurityClientInfo)
                    );
				    await Task.Delay(0);
				}
			);
		}

		private void depthOfMarketToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
				{
					if (_selectedSecurity == null)
						return;
					new ExchangeDepthOfMarketForm(
                        _selectedSecurity.Code,
						_exchangeModel,
						_exchangeSessionGetter,
						_mainFormInstance
					).Show();
				},
				_stateHelper,
				_log
			);
		}

        private void chartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(
                this,
                () =>
                {
                    if (_selectedSecurity == null)
                        return;
                    new ExchangeChartCandlesForm(
                        _exchangeModel.SessionModel.Data,
                        _selectedSecurity.Code
                    ).Show();
                },
                _stateHelper,
                _log
            );
        }

        private void columnAutowidthByHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            securityListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void columnAutowidthByContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            securityListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
    public class ExchangeSecurityListFormDesignerLocStrings
    {
        public string CodeHeaderText = "Code";
        public string TypeHeaderText = "Type";
        public string ParentCodeHeaderText = "Parent code";
        public string StatusHeaderText = "Status";
        public string DescriptionHeaderText = "Description";
        public string BaseCurrencyHeaderText = "Base currency code";
        public string ScaleHeaderText = "Scale";
        public string PriceStepHeaderText = "Price step";
        public string LotHeaderText = "Lot";
        public string ExpirationHeaderText = "Expiration";
        public string MinPriceHeaderText = "Min price";
        public string MaxPriceHeaderText = "Max price";
        public string DepthOfMarketToolStripMenuItemText = "Depth of market";
        public string ChartToolStripMenuItemText = "Chart";
        public string ColumnAutowidthByHeaderToolStripMenuItemText = "Column autowidth by header";
        public string ColumnAutowidthByContentToolStripMenuItemText = "Column autowidth by content";
        public string Text = "Security list";
    }
}
