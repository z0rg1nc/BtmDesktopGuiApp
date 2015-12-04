using System;
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
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    public partial class ExchangeNewOrderForm : Form
    {
	    private readonly IExchangeClientModel _exchangeModel;
		private IExchangeServerSessionModelData _data => _exchangeModel.SessionModel.Data;
        private readonly Func<ExchangeServerSession> _exchangeSessionGetter;
	    private readonly ClientGuiMainForm _mainFormInstance;
	    private readonly ExchangeSecurityOrderClientInfo _originOrder;

        public ExchangeNewOrderForm(
			IExchangeClientModel exchangeModel,
			Func<ExchangeServerSession> exchangeSessionGetter,
			ClientGuiMainForm mainFormInstance,
			ExchangeSecurityOrderClientInfo originOrder
		)
        {
			Assert.NotNull(exchangeModel);
	        _exchangeModel = exchangeModel;
			Assert.NotNull(exchangeSessionGetter);
	        _exchangeSessionGetter = exchangeSessionGetter;
			Assert.NotNull(mainFormInstance);
	        _mainFormInstance = mainFormInstance;
	        _originOrder = originOrder;
            InitializeComponent();
        }
		
		private void ExchangeNewOrderForm_Shown(object sender, EventArgs e)
		{
			_stateHelper.SetInitializedState();
			InitCommonView();
			if (!_mainFormInstance.CheckExchangeClientConstraints())
			{
				Close();
				return;
			}
			InitLoad();
            UpdateBackColor();
		}

	    private void InitSubscriptions()
	    {
	    }

	    private void InitLoad()
	    {
		    InitSubscriptions();
			ClientGuiMainForm.BeginInvokeProper(
				this,
				_stateHelper,
				_log,
				async () =>
				{
					var locStrings = ClientGuiMainForm.LocStrings
						.ExchangeServerLocStringsInstance.AddNewOrderLocStringsInstance;
					using (
						var wrp = await ProgressCancelFormWraper.CreateInstance(
							locStrings.InitLoadProgressFormCaption, this
						)
					)
					{
						var data = _exchangeModel.SessionModel.Data;
						var session = _exchangeSessionGetter();
						Assert.NotNull(session);
						wrp.ProgressInst.ReportProgress(10, locStrings.CurrencyListLoading);
						await ExchangeServerSession.GetUpdatableValueTemplate(
							() => data.CurrencyCollection,
							session.UpdateCurrencyList,
							() => data.CurrencyListUpdated,
							wrp.Token
						);
						wrp.ProgressInst.ReportProgress(30, locStrings.SecurityListLoading);
						await ExchangeServerSession.GetUpdatableValueTemplate(
							() => data.SecurityCollection,
							session.UpdateSecurityList,
							() => data.SecurityListUpdated,
							wrp.Token
						);
						await ExchangeServerSession.GetUpdatableValueTemplate(
							() => data.CurrencyPairSecurityInfoList,
							session.UpdateSecurityCurrencyPair,
							() => data.CurrencyPairSecurityInfoListUpdated,
							wrp.Token
						);
						wrp.ProgressInst.ReportProgress(50, locStrings.AccountListLoading);
						await ExchangeServerSession.GetUpdatableValueTemplate(
							() => data.AccountCollection,
							session.UpdateAccountList,
							() => data.AccountListUpdated,
							wrp.Token
						);
						await ExchangeServerSession.GetUpdatableValueTemplate(
							() => data.AccountBalanceCollection,
							session.UpdateAccountBalanceInfos,
							() => data.AccountBalanceListUpdated,
							wrp.Token
						);
					}
					/**/
				    var securityList = (await _data.SecurityCollection.GetDeepCopyAsync()).NewItems;
					securityList.Select(_ => _.Code).ToList().With(
						_ =>
						{
							securityListComboBox.DataSource = _;
							if (_originOrder != null && _.Contains(_originOrder.SecurityCode))
								securityListComboBox.SelectedItem = _originOrder.SecurityCode;
						}
					);
					/**/
					if (_originOrder != null)
					{
						buySideRadioButton.Checked = _originOrder.Side == EExchangeOrderSide.Buy;
						sellSideRadioButton.Checked = _originOrder.Side == EExchangeOrderSide.Sell;
						priceNumericUpDown.Value = _originOrder.Price;
						qtyNumericUpDown.Value = _originOrder.With(
							_ =>
							{
								switch (_.Status)
								{
									case EExchangeOrderStatus.Active:
									case EExchangeOrderStatus.Cancelled:
										return _.Qty - _.FilledQty;
									default:
										return _.Qty;
								}
							}
						);
					}
				}
			);
	    }

	    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly DisposableObjectStateHelper _stateHelper
			= new DisposableObjectStateHelper("ExchangeNewOrderForm");
	    private readonly Logger _log = LogManager.GetCurrentClassLogger();
		private async void ExchangeNewOrderForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			_cts.Cancel();
			await _stateHelper.MyDisposeAsync();
            _cts.Dispose();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			ClientGuiMainForm.HandleControlActionProper(
                this,
                async () =>
				{
					if (!_mainFormInstance.CheckExchangeClientConstraints())
						return;
					var session = _exchangeSessionGetter();
					Assert.NotNull(session);
					var locStrings = ClientGuiMainForm.LocStrings
						.ExchangeServerLocStringsInstance.AddNewOrderLocStringsInstance;
					/**/
					var selectedSecCode = (string)securityListComboBox.SelectedItem;
					Assert.False(string.IsNullOrWhiteSpace(selectedSecCode));
					var side = buySideRadioButton.Checked 
						? EExchangeOrderSide.Buy 
						: EExchangeOrderSide.Sell;
					var price = priceNumericUpDown.Value;
					Assert.True(price > 0);
					var qty = (long) qtyNumericUpDown.Value;
					Assert.True(qty > 0);
					var baseAccountGuid = (Guid)baseAccountComboBox.SelectedValue;
					var secondAccountGuid = (Guid)secondAccountComboBox.SelectedValue;
					using (var wrp = await ProgressCancelFormWraper.CreateInstance(
						locStrings.AddOrderProgressFormCaption, this))
					{
						wrp.ProgressInst.ReportProgress(20, locStrings.AddOrderProgressFormReport1);
						await session.AddNewOrder(
							selectedSecCode,
							side,
							price,
							qty,
							baseAccountGuid,
							secondAccountGuid,
							wrp.Token
						);
					}
					Close();
				},
				_stateHelper,
				_log
			);
		}

	    public static ExchangeNewOrderFormLocStrings LocStrings
		    = new ExchangeNewOrderFormLocStrings();
		private void InitCommonView()
		{
			this.priceLabel.Text = LocStrings.PriceLabelText.Inject(
				new
				{
					Lot = 0
				}
			);
			this.securityCodeLabel.Text = LocStrings.SecurityCodeLabelText;
			this.sideLabel.Text = LocStrings.SideLabelText;
			this.buySideRadioButton.Text = LocStrings.BuySideRadioButtonText;
			this.sellSideRadioButton.Text = LocStrings.SellSideRadioButtonText;
			this.qtyLabel.Text = LocStrings.QtyLabelText.Inject(
				new
				{
					MaxQty = 0
				}
			);
			this.baseAccountLabel.Text = LocStrings.BaseAccountLabelText;
			this.secondAccountLabel.Text = LocStrings.SecondAccountLabelText;
			this.addNewOrderButton.Text = LocStrings.AddNewOrderButtonText;
		    this.totalVolumeLabel.Text = LocStrings.TotalVolumeLabelText.Inject(
		        new
		        {
		            TotalVolume = "0"
		        }
		    );
			this.Text = LocStrings.Text;
			ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
		}

        private void UpdateTotalVolume()
        {
            ClientGuiMainForm.BeginInvokeProper(
                this,
                _stateHelper,
                _log,
                async () =>
                {
                    await Task.Delay(0);
                    decimal totalVolume;
                    decimal totalLots;
                    try
                    {
                        var price = priceNumericUpDown.Value;
                        var qty = qtyNumericUpDown.Value;
                        totalVolume = price*qty;
                    }
                    catch
                    {
                        totalVolume = 0.0m;
                        
                    }
                    try
                    {
                        var selectedSecCode = securityListComboBox.SelectedItem
                            .With(_ =>
                            {
                                if (_ == null)
                                    return "";
                                return (string) _;
                            }
                        );
                        var selectedSecurityInfo = await _data.SecurityCollection.FirstOrDefaultDeepCopyAsync(
                            __ => __.Code == selectedSecCode
                            );
                        if (selectedSecurityInfo != null)
                        {
                            var qty = qtyNumericUpDown.Value;
                            var lot = selectedSecurityInfo.Lot;
                            totalLots = lot*qty;
                        }
                        else
                            totalLots = 0.0m;
                    }
                    catch
                    {
                        totalLots = 0.0m;
                    }
                    totalVolumeLabel.Text = LocStrings.TotalVolumeLabelText.Inject(
                        new
                        {
                            TotalVolume = $"{totalVolume:G29} \\ {totalLots:G29}"
                        }
                    );
                }
            );
        }

        private void UpdateMaxQtyLabel()
	    {
			ClientGuiMainForm.BeginInvokeProper(
				this,
				_stateHelper,
				_log,
				async () =>
				{
					long maxQty = await 0.WithAsync(async none =>
					{
						try
						{
							if (buySideRadioButton.Checked)
							{
								var price = priceNumericUpDown.Value;
								if (price == 0)
									return 0;
								var selectedValue = baseAccountComboBox.SelectedValue;
								if (selectedValue == null)
									return 0;
								var selectedBaseAccountGuid = (Guid) selectedValue;
								var balanceInfo = await _data.AccountBalanceCollection.FirstOrDefaultDeepCopyAsync(
                                    _ => _.AccountGuid == selectedBaseAccountGuid
								);
								if (balanceInfo == null)
									return 0;
								checked
								{
									return (long) Math.Floor(balanceInfo.TotalAvailableBalance / price);
								}
							}
							else
							{
								var selectedSecCode = securityListComboBox.SelectedItem
									.With(_ =>
									{
										if (_ == null)
											return "";
										return (string)_;
									}
								);
							    var selectedSecurityInfo = await _data.SecurityCollection.FirstOrDefaultDeepCopyAsync(
							        __ => __.Code == selectedSecCode
							    );
								if (selectedSecurityInfo == null)
									return 0;
								Assert.True(selectedSecurityInfo.SecurityType.In(
									EExchangeSecurityType.CurrencyPair));
								var selectedValue = secondAccountComboBox.SelectedValue;
								if (selectedValue == null)
									return 0;
								var selectedSecondAccountGuid = (Guid)selectedValue;
								var balanceInfo = await _data.AccountBalanceCollection.FirstOrDefaultDeepCopyAsync(
                                    _ => _.AccountGuid == selectedSecondAccountGuid
								);
								if (balanceInfo == null)
									return 0;
								checked
								{
									return (long)Math.Floor(balanceInfo.TotalAvailableBalance / selectedSecurityInfo.Lot);
								}
							}
						}
						catch (OverflowException)
						{
							return 0;
						}
						catch (Exception exc)
						{
							MiscFuncs.HandleUnexpectedError(
								exc,
								_log
							);
							return 0;
						}
					});
					qtyLabel.Text = LocStrings.QtyLabelText.Inject(
						new
						{
							MaxQty = maxQty
						}
					);
				}
			);
	    }

	    private void securityListComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ClientGuiMainForm.HandleControlActionProper(
                this,
				async () =>
				{
					var selectedSecCode = (string)securityListComboBox.SelectedItem;
					if (string.IsNullOrWhiteSpace(selectedSecCode))
						return;
				    var securityInfo = await _exchangeModel.SessionModel.Data.SecurityCollection.FirstOrDefaultDeepCopyAsync(
				        sec => sec.Code == selectedSecCode);
					if (securityInfo == null)
						return;
					if (!securityInfo.SecurityType.In(EExchangeSecurityType.CurrencyPair))
						throw new NotSupportedException();
					priceLabel.Text = LocStrings.PriceLabelText.Inject(
						new
						{
							Lot = securityInfo.Lot
						}
					);
					priceNumericUpDown.Minimum = securityInfo.MinPrice;
					priceNumericUpDown.Maximum = securityInfo.MaxPrice;
					priceNumericUpDown.DecimalPlaces = securityInfo.Scale;
					priceNumericUpDown.Increment = securityInfo.PriceStep;
					/**/
				    var accountBalancesList =
				        (await _data.AccountBalanceCollection.GetDeepCopyAsync().ConfigureAwait(false)).NewItems;
					/**/
					var baseAccountList = await _data.AccountCollection
                        .WhereAsync(_ => _.CurrencyCode == securityInfo.BaseCurrencyCode);
					var baseAccountDataSource = baseAccountList.Select(
						accountInfo =>
						{
							var balance = accountBalancesList
								.FirstOrDefault(_ => _.AccountGuid == accountInfo.AccountGuid)
							    ?? new ExchangeAccountTotalBalanceInfo()
							    {
								    AccountGuid = accountInfo.AccountGuid
							    };
							return new {
								Name = $"{balance.TotalAvailableBalance} {accountInfo.CurrencyCode} ({accountInfo.AccountGuid})",
								Value = accountInfo.AccountGuid
							};
						}
					).ToList();
					baseAccountComboBox.DataSource = baseAccountDataSource;
					baseAccountComboBox.DisplayMember = "Name";
					baseAccountComboBox.ValueMember = "Value";
					baseAccountComboBox.Refresh();
					/**/
					var currecyPairSecurityInfos = await _data.CurrencyPairSecurityInfoList.WithAsyncLockSem(
						list => list.ToDictionary(_ => _.SecCode)
					);
					var secondAccountList = await _data.AccountCollection
                        .WhereAsync(_ =>
							!currecyPairSecurityInfos.ContainsKey(securityInfo.Code)
							|| _.CurrencyCode == currecyPairSecurityInfos[securityInfo.Code].SecondCurrencyCode
						);
					var secondAccountDataSource = secondAccountList.Select(
						accountInfo =>
						{
							var balance = accountBalancesList
								.FirstOrDefault(_ => _.AccountGuid == accountInfo.AccountGuid)
							              ?? new ExchangeAccountTotalBalanceInfo()
							              {
								              AccountGuid = accountInfo.AccountGuid
							              };
							return new{
								Name = $"{balance.TotalAvailableBalance} {accountInfo.CurrencyCode} ({accountInfo.AccountGuid})",
								Value = accountInfo.AccountGuid
							};
						}
					).ToList();
					secondAccountComboBox.DataSource = secondAccountDataSource; 
					secondAccountComboBox.DisplayMember = "Name";
					secondAccountComboBox.ValueMember = "Value";
					secondAccountComboBox.Refresh();
				},
				_stateHelper,
				_log
			);
		}

		private void baseAccountComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateMaxQtyLabel();
		}

		private void secondAccountComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateMaxQtyLabel();
		}

		private void priceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateMaxQtyLabel();
            UpdateTotalVolume();
		}

        private void UpdateBackColor()
        {
            var backColor = buySideRadioButton.Checked ? Color.LightGreen : Color.Orange;
            this.BackColor = backColor;
        }

        private void buySideRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			UpdateMaxQtyLabel();
            UpdateBackColor();
		}

		private void sellSideRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			UpdateMaxQtyLabel();
		    UpdateBackColor();
		}

        private void qtyNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateTotalVolume();
        }
    }
    public class ExchangeNewOrderFormLocStrings
	{
		public string PriceLabelText = "Price (lot {Lot})";
		public string SecurityCodeLabelText = "Security code:";
		public string SideLabelText = "Side";
		public string BuySideRadioButtonText = "Buy";
		public string SellSideRadioButtonText = "Sell";
		public string QtyLabelText = "Qty (max {MaxQty} without fees)";
		public string BaseAccountLabelText = "Base account:";
		public string SecondAccountLabelText = "Second account:";
		public string AddNewOrderButtonText = "Add";
	    public string TotalVolumeLabelText = "Total {TotalVolume}";
		public string Text = "New order";
	}
}
