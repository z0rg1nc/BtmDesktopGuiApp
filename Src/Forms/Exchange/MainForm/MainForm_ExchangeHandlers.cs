using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Exchange;
using BtmI2p.BitMoneyClient.Gui.Forms.Exchange;
using BtmI2p.BitMoneyClient.Lib.ExchangeServerSession;
using BtmI2p.CryptFile.Lib;
using BtmI2p.GeneralClientInterfaces;
using BtmI2p.GeneralClientInterfaces.ExchangeServer;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private readonly SemaphoreSlim _exchangeActionLockSem = new SemaphoreSlim(1);
        private bool _exchangeActionInProgress;
        private ActionDisposable GetExchangeActionInProgressDisposable()
        {
            return new ActionDisposable(
                () => { _exchangeActionInProgress = true; },
                () => { _exchangeActionInProgress = false; }
            );
        }

        private async Task ConnectExchangeSession()
        {
            var locStrings = LocStrings.ExchangeServerLocStringsInstance;
            using (await _exchangeActionLockSem.GetDisposable())
            {
                using (GetExchangeActionInProgressDisposable())
                {
                    if (!_proxyModel.ProxyServerConnected)
                    {
                        ShowErrorMessage(
                            LocStrings.CommonMessages.ProxyServerIsNotConnectedError
                        );
                        return;
                    }
                    if (_proxyModel.ProxySessionModel.NewVersionAvailable)
                    {
                        ShowErrorMessage(
                            LocStrings.CommonMessages.UpdateClientFirstError
                        );
                        return;
                    }
                    using (await _exchangeModel.LockSem.GetDisposable())
                    {
                        if (_exchangeModel.ExchangeServerConnected)
                        {
                            ShowErrorMessage(
                                locStrings.Messages.ExchangeServerConnectedError
                            );
                            return;
                        }
                    }
                    if (
                        _proxyModel.ProxySessionModel.Balance
                        <=
                        GlobalModelInstance
                            .CommonPublicSettings
                            .BalanceRestrictions
                            .ConnectExchangeServerMinBalance
                        )
                    {
                        ShowErrorMessage(
                            locStrings.ConnectingLocStringsInstance
                                .ConnectingLowBalanceError.Inject(
                                    new
                                    {
                                        BalanceLimit
                                            = GlobalModelInstance
                                                .CommonPublicSettings
                                                .BalanceRestrictions
                                                .ConnectExchangeServerMinBalance
                                    }
                                )
                        );
                        return;
                    }
                    /**/
                    using (var cts = new CancellationTokenSource())
                    {
                        var onProgressLoadTcs = new TaskCompletionSource<object>();
                        var progressForm = new ProgressCancelForm(
                            cts,
                            onProgressLoadTcs,
                            locStrings.ConnectingLocStringsInstance.ProgressFormCaption
                            );
                        progressForm.Show(this);
                        await onProgressLoadTcs.Task;
                        try
                        {
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport1,
                                0
                                );
                            DefaultFolders.CreateFoldersIfNotExist();
                            var dirInfo
                                = new DirectoryInfo(
                                    DefaultFolders.ExchangeProfilesFolder
                                    );
                            var exchangeProfileFileList = dirInfo.GetFiles(
                                "*.aes256",
                                SearchOption.TopDirectoryOnly
                                );
                            if (exchangeProfileFileList.Length == 0)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.NoProfileFilesFoundError
                                    );
                                return;
                            }
                            var exchangeProfileTcs = new TaskCompletionSource<FileInfo>();
                            await (new SelectProfileForm(
                                exchangeProfileTcs,
                                exchangeProfileFileList,
                                locStrings.ConnectingLocStringsInstance.SelectExchangeProfileFormCaption
                                )).ShowFormAsync(this);
                            var exchangeProfileFileInfo = await exchangeProfileTcs.Task;
                            if (exchangeProfileFileInfo == null)
                            {
                                return;
                            }
                            var passForm = new EnterPasswordForm(
                                LocStrings.CommonText.EnterProfileFilePasswordRequest
                                );
                            await passForm.ShowFormAsync(this);
                            var exchangeProfileFilePassword
                                = await passForm.Result;
                            if (exchangeProfileFilePassword == null)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                    );
                                return;
                            }
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport2,
                                25
                                );
                            ExchangeClientProfile exchangeProfile;
                            try
                            {
                                exchangeProfile = ScryptPassEncryptedData
                                    .ReadFromFile<ExchangeClientProfile>(
                                        exchangeProfileFileInfo.FullName,
                                        exchangeProfileFilePassword
                                    );
                            }
                            catch (EnumException<ScryptPassEncryptedData.EGetValueT1ErrCodes> enumExc)
                            {
                                MiscFuncs.HandleUnexpectedError(enumExc, _log);
                                if (enumExc.ExceptionCode == ScryptPassEncryptedData.EGetValueT1ErrCodes.WrongPassword)
                                    ShowErrorMessage(
                                        LocStrings.CommonMessages.WrongPasswordError
                                        );
                                else
                                    throw;
                                return;
                            }
                            try
                            {
                                exchangeProfile.CheckMe();
                            }
                            catch (Exception exc)
                            {
                                MiscFuncs.HandleUnexpectedError(exc, _log);
                                ShowErrorMessage(
                                    locStrings.ConnectingLocStringsInstance
                                        .ProfileNotValid.Inject(
                                            new
                                            {
                                                ErrorMessage = exc.Message
                                            }
                                        )
                                    );
                                return;
                            }
                            var exchangeSettings =
                                ScryptPassEncryptedData
                                    .ReadFromFile<ExchangeClientSettings>(
                                        exchangeProfile.GetSettingsFilePath(),
                                        exchangeProfile.SettingsPass
                                    );
                            try
                            {
                                exchangeSettings.CheckMe();
                            }
                            catch (Exception exc)
                            {
                                ShowErrorMessage(
                                    locStrings.ConnectingLocStringsInstance
                                        .SettingsNotValid.Inject(
                                            new
                                            {
                                                ErrorMessage = exc.Message
                                            }
                                        )
                                    );
                                return;
                            }
                            var exchangeClientCertPassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    LocStrings.CommonText.EnterCertPasswordRequest,
                                    this
                                    );
                            if (exchangeClientCertPassword == null)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                    );
                                return;
                            }
                            try
                            {
                                using (var tempPass = exchangeClientCertPassword.TempData)
                                {
                                    exchangeProfile.ExchangeClientCert.CheckMe(
                                        true,
                                        tempPass.Data
                                        );
                                }
                                if (
                                    !LightCertificateRestrictions.IsValid(
                                        exchangeProfile.ExchangeClientCert
                                        )
                                    )
                                    throw new ArgumentOutOfRangeException(
                                        MyNameof.GetLocalVarName(
                                            () => exchangeProfile.ExchangeClientCert
                                            )
                                        );
                            }
                            catch
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.WrongPasswordError
                                    );
                                return;
                            }
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport3,
                                50
                                );
                            /**/
                            var exchangeServerSessionSettings
                                = new ExchangeServerSessionSettings
                                {
                                    ExchangeClientCert = exchangeProfile.ExchangeClientCert
                                };
                            _exchangeSession = await ExchangeServerSession.CreateInstance(
                                exchangeServerSessionSettings,
                                _exchangeModel.SessionModel,
                                _proxySession,
                                exchangeClientCertPassword,
                                cts.Token
                                );
                            using (await _exchangeModel.LockSem.GetDisposable())
                            {
                                _exchangeModel.ExchangeServerConnected = true;
                                _exchangeModel.ExchangeClientGuid =
                                    exchangeProfile.ExchangeClientCert.Id;
                                _exchangeModel.Profile = exchangeProfile;
                                _exchangeModel.Settings = exchangeSettings;
                                _exchangeModel.SettingsFilePath =
                                    exchangeProfile.GetSettingsFilePath();
                                _exchangeModel.ExchangeClientProfileName
                                    = exchangeProfile.ProfileName;
                                _exchangeModel.SettingsFilenamePassBytes
                                    = exchangeProfile.SettingsPass;
                            }
                            /**/
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport7,
                                90
                                );
                            await ExchangeServerSession.GetUpdatableValueTemplate(
                                () => _exchangeModel.SessionModel.Data.CurrencyCollection,
                                _exchangeSession.UpdateCurrencyList,
                                () => _exchangeModel.SessionModel.Data.CurrencyListUpdated,
                                cts.Token
                                );
                            /**/
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport8,
                                100
                                );
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception exc)
                        {
                            _log.Error(
                                "Connect exchange client error '{0}'",
                                exc.ToString()
                                );
                            ShowErrorMessage(
                                LocStrings.CommonMessages.UnexpectedErrorMessage.Inject(
                                    new
                                    {
                                        ErrorMessage = exc.Message
                                    }
                                    )
                                );
                        }
                        finally
                        {
                            progressForm.SetProgressComplete();
                        }
                    }
                }
            }
        }

        private void loginToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.ExchangeServerLocStringsInstance;
                if (_exchangeActionInProgress)
                {
                    ShowErrorMessage(
                        locStrings.Messages.ExchangeServerActionInProgressError
                    );
                    return;
                }
                await ConnectExchangeSession();
            });
        }

        private async Task DisconnectExchangeSession(
            bool dontShowProgressForm = false)
        {
            var locStrings = LocStrings.ExchangeServerLocStringsInstance;
            using (await _exchangeActionLockSem.GetDisposable())
            {
                using (GetExchangeActionInProgressDisposable())
                {
                    if (!_exchangeModel.ExchangeServerConnected)
                    {
                        ShowErrorMessage(
                            locStrings.Messages.ExchangeServerIsNotConnectedError
                        );
                        return;
                    }
                    ProgressCancelForm progressForm = null;
                    using (var cts = new CancellationTokenSource())
                    {
                        if (!dontShowProgressForm)
                        {
                            var onProgressLoadTcs = new TaskCompletionSource<object>();
                            progressForm = new ProgressCancelForm(
                                cts,
                                onProgressLoadTcs,
                                locStrings.DisconnectingLocStringsInstance.ProgressFormCaption
                            );
                            progressForm.Show(this);
                            await onProgressLoadTcs.Task;
                        }
                        try
                        {
                            await _exchangeSession.MyDisposeAsync();
                        }
                        finally
                        {
                            if (!dontShowProgressForm)
                                progressForm.SetProgressComplete();
                        }
                    }
                    _exchangeSession = null;
                    using (await _exchangeModel.LockSem.GetDisposable())
                    {
                        ExchangeClientModel.Reset(_exchangeModel);
                    }
                }
            }
        }

        private void logoutToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.ExchangeServerLocStringsInstance;
                if (_exchangeActionInProgress)
                {
                    ShowErrorMessage(
                        locStrings.Messages.ExchangeServerActionInProgressError
                    );
                    return;
                }
                await DisconnectExchangeSession();
            });
        }

        private void copyExchangeClientGUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(() =>
            {
                var locStrings = LocStrings.ExchangeServerLocStringsInstance;
                if (!_exchangeModel.ExchangeServerConnected)
                {
                    ShowErrorMessage(
                        locStrings.Messages.ExchangeServerIsNotConnectedError
                    );
                    return;
                }
                Clipboard.SetText(
	                $"{_exchangeModel.ExchangeClientGuid}"
	                );
                ShowInfoMessage(
                    locStrings.Messages.ExchangeClientGuidCopiedInfo
                );
            });
        }
        private void exchangeAccountsListView_MouseUp(object sender, MouseEventArgs e)
        {
            HandleControlActionProper(() =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (exchangeAccountsListView.SelectedIndices.Count == 0)
                    {
                        contextMenu_exchangeAccounts_Empty.Show(
                            Cursor.Position
                            );
                    }
                    else
                    {
                        contextMenu_exchangeAccounts_Entry.Show(
                            Cursor.Position
                            );
                    }
                }
            });
        }

        public bool CheckExchangeClientConstraints()
        {
            var locStrings = LocStrings.ExchangeServerLocStringsInstance;
            if (!_exchangeModel.ExchangeServerConnected)
            {
                ShowErrorMessage(
                    locStrings.Messages.ExchangeServerIsNotConnectedError
                );
				return false;
            }
	        return true;
        }
        // Create new account, not register new profile
        private void createNewAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings
                    .ExchangeServerLocStringsInstance
                    .RegisterNewAccountLocStringsInstance;
                if (!CheckExchangeClientConstraints())
                    return;
                using (var wrp = await ProgressCancelFormWraper.CreateInstance(
                    locStrings.ProgressFormCaption, this))
                {
                    wrp.ProgressInst.ReportProgress(
                        locStrings.ReportGetCurrencyList,
                        10
                    );
                    var currencyList = (await (await ExchangeServerSession.GetUpdatableValueTemplate(
                        () => _exchangeModel.SessionModel.Data.CurrencyCollection,
                        _exchangeSession.UpdateCurrencyList,
                        () => _exchangeModel.SessionModel.Data.CurrencyListUpdated,
                        wrp.Token
                    )).GetDeepCopyAsync()).NewItems;
                    Assert.NotNull(currencyList);
                    Assert.NotEmpty(currencyList);
                    wrp.ProgressInst.ReportProgress(
                        locStrings.SelectAcccountCurrency,
                        20
                    );
                    var t = new ExchangeNewAccountSelectCurrencyEditObjectForm
                    {
                        Currency = new EditObjectFormStringEnum
                        {
                            SelectedValue = currencyList[0].Code,
                            ValuesRange = currencyList.Select(_ => _.Code).ToList()
                        }
                    };
                    var editForm = EditObjectForm.CreateInstance(
                        EEditObjectFormMode.Edit,
                        t,
                        locStrings.SelectAcccountCurrency,
                        Font.Size
                    );
                    await editForm.ShowFormAsync(this);
                    if (!editForm.ValueChanged)
                    {
                        wrp.ProgressInst.ReportProgress(
                            locStrings.NoCurrencySelectedCancel,
                            30
                            );
                        return;
                    }
                    /**/
                    wrp.ProgressInst.ReportProgress(
                        locStrings.CurrencySelected,
                        30
                    );
                    var selectedCurrency = t.Currency.SelectedValue;
                    Assert.Contains(
                        selectedCurrency,
                        currencyList.Select(_ => _.Code)
                    );
                    wrp.ProgressInst.ReportProgress(
                        locStrings.ServerCommunication,
                        40
                    );
                    var accountGuid = await _exchangeSession.RegisterNewAccount(
                        selectedCurrency,
                        wrp.Token
                    );
                    var accountRegisteredMessage
                        = locStrings.AccountRegistered.Inject(
                            new
                            {
                                AccountGuid = accountGuid
                            }
                        );
                    wrp.ProgressInst.ReportProgress(
                        accountRegisteredMessage,
                        80
                    );
                    ShowInfoMessage(accountRegisteredMessage);
                    _exchangeSession.UpdateAccountList();
                }
            });
        }
        private void makeDefaultForTheCurrencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                if(!CheckExchangeClientConstraints())
					return;
                var locStrings = LocStrings.ExchangeServerLocStringsInstance
                    .MakeAccountDefaultLocStringsInstance;
                var selectedItems = exchangeAccountsListView.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    ShowErrorMessage(
                        LocStrings.ExchangeServerLocStringsInstance
                            .Messages.NoAccountSelectedError
                    );
                    return;
                }
                var selectedAccountGuid = (Guid)selectedItems[0].Tag;
                using (
                    var wrp = await ProgressCancelFormWraper.CreateInstance(
                        locStrings.ProgressFormCaption, 
                        this
                    )
                )
                {
                    wrp.ProgressInst.ReportProgress(locStrings.Report1Text,10);
                    await _exchangeSession.MakeAccountDefault(
                        selectedAccountGuid,
                        wrp.Token
                    );
                    _exchangeSession.UpdateAccountList();
                }
            });
        }
        private void copyAccountGUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
	            await Task.FromResult(0).ConfigureAwait(false);
                if(!CheckExchangeClientConstraints())
					return;
                var selectedItems = exchangeAccountsListView.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    ShowErrorMessage(
                        LocStrings.ExchangeServerLocStringsInstance
                            .Messages.NoAccountSelectedError
                    );
                    return;
                }
                var selectedAccountGuid = (Guid)selectedItems[0].Tag;
                Clipboard.SetText($"{selectedAccountGuid}");
                var locStrings = LocStrings.ExchangeServerLocStringsInstance
                    .CopyAccountGuidLocStringsInstance;
                ShowInfoMessage(locStrings.CompleteMessage);
            });
        }
        private void changePasswordToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            var locStrings = LocStrings.ExchangeServerLocStringsInstance
                .ExchangeChangePasswordLocStringsInstance;
            ChangePasswordTemplate<ExchangeClientProfile>(
                new ChangePasswordTemplateLocStrings()
                {
                    NewCertPasswordRequestText = locStrings.EnterNewCertPaswordText,
                    NewMasterCertPasswordRequestText = null,
                    NewProfileFilePasswordRequestText = locStrings.EnterNewProfileFilePaswordText,
                    OldCertPasswordRequestText = locStrings.EnterOldCertPaswordText,
                    OldMasterCertPasswordRequestText = null,
                    OldProfileFilePasswordRequestText = locStrings.EnterOldProfileFilePaswordText,
                    SelectPasswordToChangeFormCaption = locStrings.SelectPasswordKindText,
                    SelectProfileFormCaption = LocStrings.ExchangeServerLocStringsInstance
                        .ConnectingLocStringsInstance.SelectExchangeProfileFormCaption
                }, 
                _exchangeActionLockSem,
                GetExchangeActionInProgressDisposable,
                DefaultFolders.ExchangeProfilesFolder,
                async profileName => 
                    await Task.FromResult(
						_exchangeModel.ExchangeServerConnected 
						&& _exchangeModel.ExchangeClientProfileName == profileName
					),
                new List<EProfilePasswordKinds>()
                {
                    EProfilePasswordKinds.Profile,
                    EProfilePasswordKinds.Cert
                },
                _ => _.ExchangeClientCert,
                _ => null
            );
        }
        // Selected exchange accounts changed
        private void exchangeAccountsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleControlActionProper(() =>
            {
                var selectedItems = exchangeAccountsListView.SelectedItems;
                if (selectedItems.Count == 0)
                    _exchangeModel.SelectedAccountGuid = Guid.Empty;
                else
                    _exchangeModel.SelectedAccountGuid = (Guid) selectedItems[0].Tag;
            });
        }
        // Show security list form
        private void securityListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                if(!CheckExchangeClientConstraints())
					return;
                var locStrings = LocStrings.ExchangeServerLocStringsInstance
                    .ShowSecurityListLocStringsInstance;
                using (var wrp = await ProgressCancelFormWraper.CreateInstance(locStrings.ProgressFormCaption, this))
                {
                    wrp.ProgressInst.ReportProgress(locStrings.ProgressFormReport1,10);
                    await ExchangeServerSession.GetUpdatableValueTemplate(
                        () => _exchangeModel.SessionModel.Data.SecurityCollection,
                        _exchangeSession.UpdateSecurityList,
                        () => _exchangeModel.SessionModel.Data.SecurityListUpdated,
                        wrp.Token
                    );
                    wrp.ProgressInst.SetProgressComplete();
                }
                var form = new ExchangeSecurityListForm(
					_exchangeModel,
					() => _exchangeSession,
					this
				);
                form.Show();
            });
        }
		// Show exchange orders
		private void ordersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HandleControlActionProper(() =>
			{
				if (!CheckExchangeClientConstraints())
					return;
				new ExchangeOrderListForm(
					_exchangeModel,
					() => _exchangeSession,
					this
				).Show();
			});
		}
		//Show exchange trades
		private void tradesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HandleControlActionProper(() =>
			{
				if (!CheckExchangeClientConstraints())
					return;
				new ExchangeTradeListForm(
					_exchangeModel.SessionModel.Data
				).Show();
			});
		}

		//Withdraw list form
		private void withdrawFundsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HandleControlActionProper(() =>
			{
				if (!CheckExchangeClientConstraints())
					return;
				new ExchangeWithdrawListForm(_exchangeModel.SessionModel.Data).Show();
			});
		}

		//Deposit list form
		private void depositFundsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HandleControlActionProper(() =>
			{
				if (!CheckExchangeClientConstraints())
					return;
				new ExchangeDepositListForm(
					_exchangeModel.SessionModel.Data,
                    () => _exchangeSession
				).Show();
			});
		}

		//New deposit
		private void newDepositToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HandleControlActionProper(async () =>
			{
				var locStrings = LocStrings.ExchangeServerLocStringsInstance
					.NewDepositLocStringsInstance;
				if (!CheckExchangeClientConstraints())
					return;
				var selectedItems = exchangeAccountsListView.SelectedItems;
				if (selectedItems.Count == 0)
				{
					ShowErrorMessage(
						LocStrings.ExchangeServerLocStringsInstance
							.Messages.NoAccountSelectedError
					);
					return;
				}
				var selectedAccountGuid = (Guid)selectedItems[0].Tag;
				var selectedAccountInfo = await _exchangeModel.SessionModel.Data.AccountCollection
                    .FirstOrDefaultDeepCopyAsync(_ => _.AccountGuid == selectedAccountGuid);
				if (selectedAccountInfo == null)
					return;
			    var currencyInfo = (await
			        _exchangeModel.SessionModel.Data.CurrencyCollection.GetDeepCopyAsync().ConfigureAwait(false)).NewItems
			        .FirstOrDefault(_ => _.Code == selectedAccountInfo.CurrencyCode);
			    Action<decimal> checkDepositValueAction = depositValueTest =>
			    {
                    Assert.True(
                                depositValueTest > 0.0m
                                && depositValueTest <= ExchangeServerConstants.MaxAbsCurrencyValue
                            );
                    if (currencyInfo != null)
                    {
                        Assert.Equal(
                            depositValueTest,
                            Math.Round(
                                depositValueTest,
                                currencyInfo.Scale
                            )
                        );
                        Assert.InRange(
                            depositValueTest,
                            currencyInfo.MinDeposit,
                            currencyInfo.MaxDeposit
                        );
                    }
                };
                var inputBoxForm = new InputBoxForm(
					locStrings.InputBoxCaption,
					currencyInfo == null ? "0.0" : $"{currencyInfo.MinDeposit:G29}",
					s =>
					{
					    try
					    {
					        decimal depositValueTest = decimal.Parse(s);
					        checkDepositValueAction(depositValueTest);
                            return true;
					    }
					    catch
					    {
					        return false;
					    }
					}
				    );
				await inputBoxForm.ShowFormAsync(this);
				var depositValueString = await inputBoxForm.TaskValue;
				if (string.IsNullOrWhiteSpace(depositValueString))
					return;
				var depositValue = decimal.Parse(depositValueString);
			    checkDepositValueAction(depositValue);
                using (var wrapper = await ProgressCancelFormWraper.CreateInstance(locStrings.ProgressFormCaption, this))
				{
					await _exchangeSession.AddNewDeposit(
						selectedAccountGuid,
						depositValue,
						wrapper.Token
					);
				}
				await MessageBoxAsync.ShowAsync(
                    this,
					locStrings.DepositAddedMessage,
					"",
					icon: MessageBoxIcon.Information
				);
			});
		}

		//New withdraw
		private void newWithdrawToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HandleControlActionProper(async () =>
			{
				var locStrings = LocStrings.ExchangeServerLocStringsInstance
					.NewWithdrawLocStringsInstance;
				if (!CheckExchangeClientConstraints())
					return;
				var selectedItems = exchangeAccountsListView.SelectedItems;
				if (selectedItems.Count == 0)
				{
					ShowErrorMessage(
						LocStrings.ExchangeServerLocStringsInstance
							.Messages.NoAccountSelectedError
					);
					return;
				}
				var selectedAccountGuid = (Guid)selectedItems[0].Tag;
				var selectedAccountInfo = await _exchangeModel.SessionModel.Data.AccountCollection
                    .FirstOrDefaultDeepCopyAsync(_ => _.AccountGuid == selectedAccountGuid);
				if (selectedAccountInfo == null)
					return;
                var currencyInfo = (await
                    _exchangeModel.SessionModel.Data.CurrencyCollection.GetDeepCopyAsync().ConfigureAwait(false)).NewItems
                    .FirstOrDefault(_ => _.Code == selectedAccountInfo.CurrencyCode);
			    Action<decimal> checkWindrawValueAction = t =>
			    {
                    Assert.True(t > 0.0m && t <= ExchangeServerConstants.MaxAbsCurrencyValue);
			        if (currencyInfo != null)
			        {
                        Assert.Equal(
                            t,
                            Math.Round(
                                t,
                                currencyInfo.Scale
                            )
                        );
                        Assert.InRange(
                            t,
                            currencyInfo.MinWithdrawPos,
                            currencyInfo.MaxWithdrawPos
                        );
			        }
			    };
                var withdrawValueBoxForm = new InputBoxForm(
                    locStrings.WithdrawValueInputBoxCaption,
                    currencyInfo == null ? "0.0" : $"{currencyInfo.MinWithdrawPos:G29}",
                    s =>
                    {
                        try
                        {
                            var t = decimal.Parse(s);
                            checkWindrawValueAction(t);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                );
                await withdrawValueBoxForm.ShowFormAsync(this);
			    var withdrawValueString = await withdrawValueBoxForm.TaskValue;
			    if (string.IsNullOrWhiteSpace(withdrawValueString))
			        return;
                decimal withdrawValuePos = decimal.Parse(withdrawValueString);
			    checkWindrawValueAction(withdrawValuePos);
                ExchangePaymentDetails paymentDetails;
			    using (var wrp = await ProgressCancelFormWraper.CreateInstance(
			        locStrings.ReceivingEmptyPaymentDetailsPattern, this))
			    {
			        paymentDetails = await _exchangeSession.GetEmptyWithdrawPaymentDetailsDeepCopy(
			            selectedAccountInfo.CurrencyCode,
			            wrp.Token
			        );
			    }
			    var pdForm = new ExchangePaymentDetailsEditOrShowForm(
			        paymentDetails,
			        EExchangePaymentDetailsEditOrShowFormMode.Edit,
			        locStrings.InputBoxCaption
			    );
			    await pdForm.ShowFormAsync(this);
			    var pdFormResult = await pdForm.ClosingTask;
			    if (pdFormResult == DialogResult.Cancel)
			        return;
                paymentDetails.CheckMe();
				using (var wrapper = await ProgressCancelFormWraper.CreateInstance(locStrings.AddingWithdrawProgressFormCaption, this))
				{
					await _exchangeSession.AddNewWithdraw(
						selectedAccountGuid,
						paymentDetails,
						withdrawValuePos,
						wrapper.Token
					);
				}
				await MessageBoxAsync.ShowAsync(
                    this,
					locStrings.WithdrawAddedMessage,
					"",
					icon: MessageBoxIcon.Information
				);
			});
		}
        //Currency list form
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(() =>
            {
                if (!CheckExchangeClientConstraints())
                    return;
                new ExchangeCurrencyListForm(
                    _exchangeModel.SessionModel.Data
                ).Show();
            });
        }

        private void registerNewExchangeProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                if (!_proxyModel.ProxyServerConnected)
                {
                    ShowErrorMessage(
                        LocStrings.CommonMessages.ProxyServerIsNotConnectedError
                    );
                    return;
                }
                if (_proxyModel.ProxySessionModel.NewVersionAvailable)
                {
                    ShowErrorMessage(
                        LocStrings.CommonMessages.UpdateClientFirstError
                    );
                    return;
                }
                if (
                    _proxyModel.ProxySessionModel.Balance
                    <=
                    GlobalModelInstance
                        .CommonPublicSettings
                        .BalanceRestrictions
                        .RegisterExchangeServerMinBalance
                )
                {
                    ShowErrorMessage(
                        LocStrings.ExchangeServerLocStringsInstance
                        .RegistrationLocStringsInstance.RegistrationLowBalanceError.Inject(
                            new
                            {
                                BalanceLimit = GlobalModelInstance
                                    .CommonPublicSettings
                                    .BalanceRestrictions
                                    .RegisterExchangeServerMinBalance
                            }
                        )
                    );
                    return;
                }
                await (new RegisterExchangeClientForm(_proxySession)).ShowFormAsync(this);
            });
        }

        private void exchangeTransferLastNcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BeginInvokeProper(
                () =>
                {
                    var n = int.Parse((string)exchangeTransferLastNcomboBox.SelectedItem);
                    _proxyTransferN.N = n;
                }
            );
        }

        private void exchangeLockedFundsLastNComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BeginInvokeProper(
                () =>
                {
                    var n = int.Parse((string) exchangeLockedFundsLastNComboBox.SelectedItem);
                    _proxyLockedFundsN.N = n;
                }
            );
        }
        //Transfers
        private void columnAutowidthByHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvokeProper(() =>
            {
                exchangeTransferListView.AutoResizeColumns(
                    ColumnHeaderAutoResizeStyle.HeaderSize);
            });
        }
        //Transfers
        private void columnAutowidthByContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvokeProper(() =>
            {
                exchangeTransferListView.AutoResizeColumns(
                    ColumnHeaderAutoResizeStyle.ColumnContent);
            });
        }
        //Locked funds
        private void columnAutowidthByHeaderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            BeginInvokeProper(() =>
            {
                exchangeLockedFundsListView.AutoResizeColumns(
                    ColumnHeaderAutoResizeStyle.HeaderSize);
            });
        }
        //Locked funds
        private void columnAutowidthByContentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            BeginInvokeProper(() =>
            {
                exchangeLockedFundsListView.AutoResizeColumns(
                    ColumnHeaderAutoResizeStyle.ColumnContent);
            });
        }


        private void exchangeTransferListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            BeginInvokeProper(() =>
            {
                var columnNum = e.Column;
                if (columnNum.In(0, 1, 2))
                {
                    var newSortPropName = columnNum == 0
                        ? nameof(ExchangeAccountTranferClientInfo.SentDateTime)
                        : columnNum == 1
                            ? nameof(ExchangeAccountTranferClientInfo.TransferType)
                            : nameof(ExchangeAccountTranferClientInfo.Value);
                    var currentSortArgs = _exchangeTransfersSortedArgs;
                    var sortedAsc = true;
                    if (newSortPropName == currentSortArgs.Item1)
                        sortedAsc = !currentSortArgs.Item2;
                    _exchangeTransfersSortedArgs = Tuple.Create(
                        newSortPropName,
                        sortedAsc
                    );
                    _proxyTransferComparer.Comparer = Comparer<ExchangeAccountTranferClientInfo>.Create(
                        (a, b) =>
                        {
                            var result = columnNum == 0
                                ? Comparer<DateTime>.Default.Compare(a.SentDateTime, b.SentDateTime)
                                : columnNum == 1
                                    ? Comparer<EExchangeAccountTransferType>.Default.Compare(a.TransferType,
                                        b.TransferType)
                                    : Comparer<decimal>.Default.Compare(a.Value, b.Value);
                            if (!sortedAsc)
                                result *= -1;
                            return result;
                        }
                    );
                }
            });
        }

        private void exchangeLockedFundsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            BeginInvokeProper(() =>
            {
                var columnNum = e.Column;
                if (columnNum.In(0, 1))
                {
                    var newSortPropName = columnNum == 0
                        ? nameof(ExchangeAccountLockedFundsClientInfo.LockDate)
                        : nameof(ExchangeAccountLockedFundsClientInfo.Value);
                    var currentSortArgs = _exchangeLockedFundsSortedArgs;
                    var sortedAsc = true;
                    if (newSortPropName == currentSortArgs.Item1)
                        sortedAsc = !currentSortArgs.Item2;
                    _exchangeLockedFundsSortedArgs = Tuple.Create(
                        newSortPropName,
                        sortedAsc
                    );
                    _proxyLockedFundsComparer.Comparer = Comparer<ExchangeAccountLockedFundsClientInfo>.Create(
                        (a, b) =>
                        {
                            var result = columnNum == 0
                                ? Comparer<DateTime>.Default.Compare(a.LockDate, b.LockDate)
                                : Comparer<decimal>.Default.Compare(a.Value, b.Value);
                            if (!sortedAsc)
                                result *= -1;
                            return result;
                        }
                    );
                }
            });
        }
    }
}
