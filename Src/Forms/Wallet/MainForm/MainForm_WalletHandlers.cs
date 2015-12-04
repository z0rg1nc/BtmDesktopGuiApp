using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.BitMoneyClient.Gui.Forms.Wallet;
using BtmI2p.BitMoneyClient.Lib.WalletServerSession;
using BtmI2p.CryptFile.Lib;
using BtmI2p.GeneralClientInterfaces.WalletServer;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private void registerNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.WalletServerLocStringsInstance;
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
                        .RegisterWalletServerMinBalance
                    )
                {
                    ShowErrorMessage(
                        locStrings.RegistrationLocStringsInstance
                            .RegistrationLowBalanceError.Inject(
                                new
                                {
                                    BalanceLimit 
                                        = GlobalModelInstance
                                            .CommonPublicSettings
                                            .BalanceRestrictions
                                            .RegisterWalletServerMinBalance 
                                }
                            )
                    );
                    return;
                }
                await (new RegisterWalletForm(_proxySession)).ShowFormAsync(this);
            });
        }
        private readonly SemaphoreSlim _walletActionLockSem = new SemaphoreSlim(1);
        private bool _walletActionInProgress;
        private ActionDisposable GetWalletActionInProgressDisposable()
        {
            return new ActionDisposable(
                () => { _walletActionInProgress = true; },
                () => { _walletActionInProgress = false; }
            );
        }

        private async Task ConnectWalletSession()
        {
            var locStrings = LocStrings.WalletServerLocStringsInstance;
            using (await _walletActionLockSem.GetDisposable())
            {
                using (GetWalletActionInProgressDisposable())
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
                            .ConnectWalletServerMinBalance
                        )
                    {
                        ShowErrorMessage(
                            locStrings.ConnectingLocStringsInstance
                                .ConnectLowBalanceError.Inject(
                                    new
                                    {
                                        BalanceLimit 
                                            = GlobalModelInstance
                                                .CommonPublicSettings
                                                .BalanceRestrictions
                                                .ConnectWalletServerMinBalance
                                    }
                                )   
                        );
                        return;
                    }
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
                            var dirInfo = new DirectoryInfo(
                                DefaultFolders.WalletProfilesFolder
                                );
                            var walletSettingsFileList = dirInfo.GetFiles(
                                "*.aes256",
                                SearchOption.TopDirectoryOnly
                                );
                            if (walletSettingsFileList.Length == 0)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.NoProfileFilesFoundError
                                    );
                                return;
                            }
                            var walletProfileTcs = new TaskCompletionSource<FileInfo>();
                            await (
                                new SelectProfileForm
                                    (
                                    walletProfileTcs,
                                    walletSettingsFileList,
                                    locStrings.ConnectingLocStringsInstance.SelectWalletProfile
                                    )
                                ).ShowFormAsync(this);
                            var walletProfileFileInfo = await walletProfileTcs.Task;
                            if (walletProfileFileInfo == null)
                            {
                                return;
                            }
                            var selectedProfileFilename = walletProfileFileInfo.Name;
                            // trim .aes256
                            var walletProfileName =
                                new string(
                                    selectedProfileFilename
                                        .Take(selectedProfileFilename.Length - 7)
                                        .ToArray()
                                    );
                            using (await _walletListFormModel.LockSem.GetDisposable())
                            {
                                foreach (
                                    var walletInfo 
                                        in _walletListFormModel.WalletInfos.Values
                                    )
                                {
                                    using (await walletInfo.LockSem.GetDisposable())
                                    {
                                        if (walletInfo.Alias == walletProfileName)
                                        {
                                            ShowErrorMessage(
                                                locStrings.Messages.WalletAlreadyConnectedError
                                                );
                                            return;
                                        }
                                    }
                                }
                            }
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport2,
                                10
                                );
                            var walletProfileFilePassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    LocStrings.CommonText.EnterProfileFilePasswordRequest,
                                    this
                                    );
                            if (walletProfileFilePassword == null)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                    );
                                return;
                            }
                            WalletProfile walletProfileInstance;
                            try
                            {
                                using (var tempPass = walletProfileFilePassword.TempData)
                                {
                                    walletProfileInstance
                                        = ScryptPassEncryptedData.ReadFromFile<WalletProfile>(
                                            walletProfileFileInfo.FullName,
                                            tempPass.Data
                                            );
                                }
                                walletProfileInstance.CheckMe();
                            }
                            catch (EnumException<ScryptPassEncryptedData.EGetValueT1ErrCodes> enumExc)
                            {
                                string errorMessage;
                                if (enumExc.ExceptionCode == ScryptPassEncryptedData.EGetValueT1ErrCodes.WrongPassword)
                                    errorMessage = LocStrings.CommonMessages.WrongPasswordError;
                                else
                                {
                                    errorMessage = locStrings.ConnectingLocStringsInstance
                                        .ParseProfileError.Inject(
                                            new
                                            {
                                                ErrorMessage = enumExc.Message
                                            }
                                        );
                                }
                                MiscFuncs.HandleUnexpectedError(enumExc, _log);
                                ShowErrorMessage(
                                    errorMessage
                                    );
                                return;
                            }
                            catch (Exception exc)
                            {
                                MiscFuncs.HandleUnexpectedError(exc, _log);
                                ShowErrorMessage(
                                    locStrings.ConnectingLocStringsInstance
                                        .ParseProfileError.Inject(
                                            new
                                            {
                                                ErrorMessage = exc.Message
                                            }
                                        )
                                    );
                                return;
                            }
                            IWalletSettings walletSettings;
                            try
                            {
                                walletSettings =
                                    MyNotifyPropertyChangedImpl.GetProxy(
                                        (IWalletSettings) ScryptPassEncryptedData
                                            .ReadFromFile<WalletSettings>(
                                                walletProfileInstance.GetSettingsFilePath(),
                                                walletProfileInstance.SettingsPass
                                            )
                                        );
                                walletSettings.CheckMe();
                            }
                            catch (Exception exc)
                            {
                                ShowErrorMessage(
                                    locStrings.ConnectingLocStringsInstance
                                        .DecryptOrParseWalletSettingsError.Inject(
                                            new
                                            {
                                                ErrorMessage = exc.Message
                                            }
                                        )
                                    );
                                return;
                            }
                            using (await _walletListFormModel.LockSem.GetDisposable())
                            {
                                foreach (
                                    var walletInfo 
                                        in _walletListFormModel.WalletInfos.Values
                                    )
                                {
                                    using (await walletInfo.LockSem.GetDisposable())
                                    {
                                        if (
                                            walletInfo.WalletProfileInstance.WalletCert.Id
                                            == walletProfileInstance.WalletCert.Id
                                            )
                                        {
                                            ShowErrorMessage(
                                                locStrings.ConnectingLocStringsInstance
                                                    .WalletAlreadyConnectedError
                                                );
                                            return;
                                        }
                                    }
                                }
                            }
                            var walletCertPassword =
                                await EnterPasswordForm.CreateAndReturnResult(
                                    LocStrings.CommonText.EnterCertPasswordRequest,
                                    this
                                    );
                            if (walletCertPassword == null)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                    );
                                return;
                            }
                            using (var tempPass = walletCertPassword.TempData)
                            {
                                if (
                                    !walletProfileInstance.WalletCert.IsPassValid(
                                        tempPass.Data
                                        )
                                    )
                                {
                                    ShowErrorMessage(
                                        LocStrings.CommonMessages.WrongPasswordError
                                        );
                                    return;
                                }
                            }
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport3,
                                30
                                );
                            var newWalletFormModel
                                = MyNotifyPropertyChangedImpl.GetProxy(
                                    (IWalletFormModel) WalletFormModel.CreateInstance(this)
                                    );
                            newWalletFormModel.WalletProfileInstance = walletProfileInstance;
                            newWalletFormModel.WalletSetting = walletSettings;
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport4,
                                40
                                );
                            WalletTransferHistory transferHistory;
                            if (walletSettings.SaveTransferHistory)
                            {
                                DefaultFolders.CreateFoldersIfNotExist();
                                if (
                                    walletSettings.TransferHistoryVersion
                                    < WalletSettings.CurrentTransferHistoryVersion
                                    )
                                {
                                    if (File.Exists(walletSettings.GetTransferHistoryFilePath()))
                                        File.Delete(walletSettings.GetTransferHistoryFilePath());
                                    var tempSettings = new WalletSettings();
                                    walletSettings.TransferHistoryFilename
                                        = tempSettings.TransferHistoryFilename;
                                    walletSettings.TransferHistoryPass
                                        = tempSettings.TransferHistoryPass;
                                    walletSettings.TransferHistoryVersion
                                        = WalletSettings.CurrentTransferHistoryVersion;
                                }
                                transferHistory = await WalletTransferHistory.CreateInstance(
                                    walletSettings.GetTransferHistoryFilePath(),
                                    walletSettings.TransferHistoryPass
                                    );
                            }
                            else
                            {
                                transferHistory = WalletTransferHistory.CreateInstance();
                            }
                            newWalletFormModel.WalletTransferHistoryInstance
                                = transferHistory;
                            newWalletFormModel.WalletSettingsFilePath
                                = walletProfileInstance.GetSettingsFilePath();
                            newWalletFormModel.WalletSettingsFilePassword
                                = walletProfileInstance.SettingsPass;
                            newWalletFormModel.Subscriptions.Add(
                                newWalletFormModel
                                    .WalletSetting
                                    .PropertyChangedSubject
                                    .Subscribe(
                                        i => newWalletFormModel.OnSettingsChanged()
                                    )
                                );
                            /* To save file */
                            MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                                newWalletFormModel.WalletSetting,
                                e => e.SaveTransferHistory
                                );
                            newWalletFormModel.Alias = walletProfileName;
                            /**/
                            var lastTransfersFromHistory = await transferHistory.GetLastTransfers();
                            using (await newWalletFormModel.LockSem.GetDisposable().ConfigureAwait(false))
                            {
                                await newWalletFormModel.TransferObservableCollection.RemoveWhereAsync(
                                    _ => lastTransfersFromHistory.Any(__ => __.TransferGuid == _.TransferGuid)
                                    ).ConfigureAwait(false);
                                await newWalletFormModel.TransferObservableCollection.AddRangeAsync(
                                    lastTransfersFromHistory.Select(
                                        transfer => transfer.OutcomeTransfer
                                            ? WalletFormModelTransferInfo.FromOutcomeTransfer(
                                                transfer,
                                                walletProfileInstance.WalletCert.Id
                                                )
                                            : WalletFormModelTransferInfo.FromIncomeTransfer(
                                                transfer,
                                                walletProfileInstance.WalletCert.Id
                                                )).ToList()
                                    ).ConfigureAwait(false);
                            }
                            /**/
                            newWalletFormModel.Subscriptions.Add(
                                newWalletFormModel.PropertyChangedSubject.Subscribe(
                                    x => BeginInvokeProper(
                                        async () =>
                                            await OnWalletPropertyChanged(
                                                walletProfileInstance.WalletCert.Id,
                                                x
                                                )
                                        )
                                    )
                                );
                            /**/
                            var modelForWalletSession = MyNotifyPropertyChangedImpl.GetProxy(
                                (IWalletServerSessionModel) (new WalletServerSessionModel())
                                );
                            newWalletFormModel.SessionModel = modelForWalletSession;
                            if (walletSettings.SaveTransferHistory)
                            {
                                newWalletFormModel.Subscriptions.Add(
                                    modelForWalletSession.OnTransferSent
                                        .Subscribe(
                                            x =>
                                                newWalletFormModel
                                                    .OnTransferSentToTransferHistory(
                                                        new[] {x},
                                                        newWalletFormModel
                                                    )
                                        )
                                    );
                                newWalletFormModel.Subscriptions.Add(
                                    modelForWalletSession.OnTransferReceived
                                        .Subscribe(
                                            x =>
                                                newWalletFormModel
                                                    .OnTransferReceivedToTransferHistory(
                                                        new[] {x},
                                                        newWalletFormModel
                                                    )
                                        )
                                    );
                            }
                            newWalletFormModel.Subscriptions.Add(
                                modelForWalletSession.PropertyChangedSubject.Subscribe(
                                    x =>
                                    {
                                        if (
                                            x.PropertyName
                                            == modelForWalletSession.MyNameOfProperty(
                                                e => e.Balance
                                                )
                                            )
                                        {
                                            newWalletFormModel.Balance
                                                = (long) x.CastedNewProperty;
                                        }
                                    }
                                    )
                                );
                            /**/
                            newWalletFormModel.Subscriptions.Add(
                                modelForWalletSession.OnTransferReceived.Subscribe(
                                    x =>
                                    {
                                        foreach (var receivedTransfer in x)
                                        {
                                            ProcessTransferInfoToExternalApp(
                                                WalletFormModelTransferInfo.FromIncomeTransfer(
                                                    receivedTransfer,
                                                    walletProfileInstance.WalletCert.Id
                                                    )
                                                );
                                        }
                                    }
                                    )
                                );
                            newWalletFormModel.Subscriptions.Add(
                                modelForWalletSession
                                    .OnTransferSent.Subscribe(
                                        x =>
                                        {
                                            foreach (var sentTransfer in x)
                                            {
                                                ProcessTransferInfoToExternalApp(
                                                    WalletFormModelTransferInfo.FromOutcomeTransfer(
                                                        sentTransfer,
                                                        walletProfileInstance.WalletCert.Id
                                                        )
                                                    );
                                            }
                                        }
                                    )
                                );
                            newWalletFormModel.Subscriptions.Add(
                                modelForWalletSession
                                    .OnPreparedToSendTransferFault.Subscribe(
                                        x =>
                                        {
                                            var transferInfo = WalletFormModelTransferInfo.FromPrepared(
                                                x.PreparedTransfer,
                                                walletProfileInstance.WalletCert.Id
                                                );
                                            transferInfo.SetFaulted(x.FaultCode, x.FaultMessage, x.ServerFaultCode,
                                                x.ServerGeneralFaultCode);
                                            ProcessTransferInfoToExternalApp(
                                                transferInfo
                                                );
                                        }
                                    )
                                );
                            /**/
                            newWalletFormModel.Subscriptions.Add(
                                modelForWalletSession.OnTransferReceived.Subscribe(
                                    x => BeginInvokeProper(
                                        () =>
                                            newWalletFormModel
                                                .SessionModel_OnTransferReceived(x)
                                        )
                                    )
                                );
                            newWalletFormModel.Subscriptions.Add(
                                modelForWalletSession
                                    .OnTransferSent.Subscribe(
                                        x => BeginInvokeProper(
                                            () =>
                                                newWalletFormModel
                                                    .SessionModel_OnTransferSent(x)
                                            )
                                    )
                                );
                            /**/
                            newWalletFormModel.Subscriptions.Add(
                                modelForWalletSession
                                    .OnPreparedToSendTransferAdded.Subscribe(
                                        x => BeginInvokeProper(
                                            () =>
                                                newWalletFormModel
                                                    .SessionModel_OnPreparedToSendTransferAdded(x)
                                            )
                                    )
                                );
                            newWalletFormModel.Subscriptions.Add(
                                modelForWalletSession
                                    .OnPreparedToSendTransferComplete.Subscribe(
                                        x => BeginInvokeProper(
                                            () =>
                                                newWalletFormModel
                                                    .SessionModel_OnPreparedToSendTransferComplete(x)
                                            )
                                    )
                                );
                            newWalletFormModel.Subscriptions.Add(
                                modelForWalletSession
                                    .OnPreparedToSendTransferFault.Subscribe(
                                        x => BeginInvokeProper(
                                            () =>
                                                newWalletFormModel
                                                    .SessionModel_OnPreparedToSendTransferFault(x)
                                            )
                                    )
                                );
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport5,
                                50
                                );
                            /**/
                            WalletServerSession walletSession;
                            var walletSessionSettings = new WalletServerSessionSettings
                            {
                                WalletCert = walletProfileInstance.WalletCert,
                                MasterWalletCert = walletProfileInstance.MasterWalletCert
                            };
                            walletSessionSettings.LastKnownReceivedTransferGuid
                                = await transferHistory.GetLastKnownTransferGuid(false);
                            walletSessionSettings.LastKnownSentTransferGuid
                                = await transferHistory.GetLastKnownTransferGuid(true);
                            walletSessionSettings.TransferInitCounter
                                = await transferHistory.GetInitCounter();
                            try
                            {
                                walletSession = await WalletServerSession.CreateInstance(
                                    walletSessionSettings,
                                    modelForWalletSession,
                                    _proxySession,
                                    walletCertPassword,
                                    cts.Token
                                    );
                            }
                            catch (OperationCanceledException)
                            {
                                return;
                            }
                            catch (Exception exc)
                            {
                                _log.Error(
                                    "WalletServerSession.CreateInstance {0}",
                                    exc.ToString()
                                    );
                                ShowErrorMessage(
                                    locStrings.ConnectingLocStringsInstance
                                        .CreateWalletServerSessionError.Inject(
                                            new
                                            {
                                                ErrorMessage = exc.Message
                                            }
                                        )
                                    );
                                return;
                            }
                            newWalletFormModel.WalletSession = walletSession;
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport6,
                                90
                                );
                            walletSession.UpdateBalance();
                            /**/
                            using (await _walletListFormModel.LockSem.GetDisposable())
                            {
                                _walletListFormModel.WalletInfos.Add(
                                    walletProfileInstance.WalletCert.Id,
                                    newWalletFormModel
                                    );
                            }
                            _walletListFormModel.OnWalletInfoAdded
                                .OnNext(newWalletFormModel);
                            progressForm.ReportProgress(
                                LocStrings.CommonText.Finish,
                                100
                                );
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        finally
                        {
                            progressForm.SetProgressComplete();
                        }
                    }
                }
            }
        }

        private void loginToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.WalletServerLocStringsInstance;
                if (_walletActionInProgress)
                {
                    ShowErrorMessage(
                        locStrings.Messages.WalletServerActionInProgressError
                    );
                    return;
                }
                await ConnectWalletSession();
            });
        }

        private async Task DisconnectWalletSession(Guid walletId)
        {
            var locStrings = LocStrings.WalletServerLocStringsInstance;
            using (await _walletActionLockSem.GetDisposable())
            {
                using (GetWalletActionInProgressDisposable())
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        var onProgressLoadTcs = new TaskCompletionSource<object>();
                        var progressForm = new ProgressCancelForm(
                            cts,
                            onProgressLoadTcs,
                            locStrings.DisconnectingLocStringsInstance.ProgressFormCaption
                            );
                        progressForm.Show(this);
                        await onProgressLoadTcs.Task;
                        try
                        {
                            progressForm.ReportProgress(
                                LocStrings.CommonText.Start,
                                10
                                );
                            using (await _walletListFormModel.LockSem.GetDisposable())
                            {
                                if (!_walletListFormModel.WalletInfos.ContainsKey(walletId))
                                    return;
                                var walletInfo = _walletListFormModel.WalletInfos[walletId];
                                progressForm.ReportProgress(
                                    locStrings.DisconnectingLocStringsInstance.ProgressFormReport1,
                                    35
                                    );
                                await walletInfo.MyDisposeAsync();
                                _walletListFormModel.WalletInfos.Remove(walletId);
                                _walletListFormModel.OnWalletInfoRemoved.OnNext(walletId);
                            }
                            progressForm.ReportProgress(
                                LocStrings.CommonText.Finish,
                                100
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

        private void logoutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.WalletServerLocStringsInstance;
                if (_walletActionInProgress)
                {
                    ShowErrorMessage(
                        locStrings.Messages.WalletServerActionInProgressError
                    );
                    return;
                }
                Guid walletGuid;
                using (await _focusedItemContextMenu2LockSem.GetDisposable())
                {
                    if (_focusedItemContextMenu2 != null)
                    {
                        walletGuid = (Guid) _focusedItemContextMenu2.Tag;
                    }
                    else
                        return;
                }
                await DisconnectWalletSession(walletGuid);
            });
        }
        private readonly SemaphoreSlim _focusedItemContextMenu2LockSem 
            = new SemaphoreSlim(1);
        private ListViewItem _focusedItemContextMenu2 = null;
        private void listView3_MouseClick(object sender, MouseEventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    var focusedItem = walletListView.FocusedItem;
                    if (
                        focusedItem != null
                        && focusedItem.Bounds.Contains(e.Location)
                        )
                    {
                        using (await _focusedItemContextMenu2LockSem.GetDisposable())
                        {
                            _focusedItemContextMenu2 = focusedItem;
                        }
                        contextMenu_Wallet.Show(Cursor.Position);
                    }
                }
            });
        }
        /**/
        private void showTranferInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var focusedItem = walletRecentTransferListView.FocusedItem;
                if (focusedItem == null)
                {
                    return;
                }
                var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                if (transferInfo == null)
                {
                    return;
                }
                await (new ShowTransferInfoForm(transferInfo)).ShowFormAsync(this);
            });
        }
        private void copyWalletToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(() =>
            {
                var locStrings = LocStrings.WalletServerLocStringsInstance;
                var focusedItem = walletRecentTransferListView.FocusedItem;
                if (focusedItem == null)
                {
                    ShowErrorMessage(
                        locStrings.Messages.NoTransferSelectedError
                    );
                    return;
                }
                var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                if (transferInfo == null)
                {
                    ShowErrorMessage(
                        locStrings.Messages.NoDataAttachedError
                    );
                    return;
                }
                Clipboard.SetText(
                    string.Format("{0}", transferInfo.WalletTo)
                );
            });
        }

        private void copyWalletFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(() =>
            {
                var locStrings = LocStrings.WalletServerLocStringsInstance;
                var focusedItem = walletRecentTransferListView.FocusedItem;
                if (focusedItem == null)
                {
                    ShowErrorMessage(
                        locStrings.Messages.NoTransferSelectedError
                    );
                    return;
                }
                var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                if (transferInfo == null)
                {
                    ShowErrorMessage(
                        locStrings.Messages.NoDataAttachedError
                    );
                    return;
                }
                Clipboard.SetText(
                    string.Format("{0}",transferInfo.WalletFrom)
                );
            });
        }

        private void copyCommentStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(() =>
            {
                var locStrings = LocStrings.WalletServerLocStringsInstance;
                var focusedItem = walletRecentTransferListView.FocusedItem;
                if (focusedItem == null)
                {
                    ShowErrorMessage(
                        locStrings.Messages.NoTransferSelectedError
                    );
                    return;
                }
                var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                if (transferInfo == null)
                {
                    ShowErrorMessage(
                        locStrings.Messages.NoDataAttachedError
                    );
                    return;
                }
                string commentString = null;
                if (transferInfo.CommentBytes == null
                    || transferInfo.CommentBytes.Length == 0)
                {
                    commentString = string.Empty;
                }
                else
                {
                    try
                    {
                        commentString = Encoding.UTF8.GetString(transferInfo.CommentBytes);
                    }
                    catch
                    {
                        ShowErrorMessage(
                            locStrings.Messages.CommentIsNotUtf8StringError
                        );
                        return;
                    }
                }
                Clipboard.SetText(commentString.With(
                                _ => _ == "" ? " " : _
                            ));
            });
        }
        private void listView4_MouseClick(object sender, MouseEventArgs e)
        {
            HandleControlActionProper(() =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    var focusedItem = walletRecentTransferListView.FocusedItem;
                    if (
                        focusedItem != null
                        && focusedItem.Bounds.Contains(e.Location)
                    )
                    {
                        contextMenuStrip_ShowTransferInfo.Show(
                            Cursor.Position
                        );
                    }
                }
            });
        }
        private void listView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var focusedItem = walletRecentTransferListView.FocusedItem;
                if (focusedItem == null)
                {
                    return;
                }
                var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                if (transferInfo == null)
                {
                    return;
                }
                await (new ShowTransferInfoForm(transferInfo)).ShowFormAsync(this);
            });
        }
        /**/
        private async void SendFundsFromWalletToWallet(
            Guid senderWalletGuid,
            SendFundsFormData transferData
        )
        {
            var locStrings = LocStrings.WalletServerLocStringsInstance;
            var initData = transferData;
	        bool whileCondition = true;
            while (whileCondition)
            {
                var transferInfoTcs
                    = new TaskCompletionSource<SendFundsFormData>();
                await new SendFundsForm(transferInfoTcs, senderWalletGuid, initData)
                    .ShowFormAsync(this);
                var transferInfo = await transferInfoTcs.Task;
	            if (transferInfo == null)
		            break;
	            try
		        {
					using (await _walletListFormModel.LockSem.GetDisposable())
					{
						if (!_walletListFormModel.WalletInfos.ContainsKey(senderWalletGuid))
						{
							whileCondition = false;
							return;
						}
						var walletFormInfo
							= _walletListFormModel.WalletInfos[senderWalletGuid];
						using (await walletFormInfo.LockSem.GetDisposable())
						{
							// <= cause fees
							if (walletFormInfo.Balance <= transferData.Amount)
							{
                                if (
                                    await MessageBoxAsync.ShowAsync(this,
                                        locStrings.Messages.NotEnoughFundsError,
                                        LocStrings.Messages.ErrorMessageCaption,
                                        MessageBoxButtons.RetryCancel,
                                        MessageBoxIcon.Error
                                    ) == DialogResult.Cancel
                                )
                                {
                                    whileCondition = false;
                                    return;
                                }
                                continue;
							}
						    try
						    {
						        await walletFormInfo.WalletSession.SendTransfer(
						            transferData.WalletCertPass,
						            transferData.ReceiverWalletGuid,
						            transferData.Amount,
						            transferData.CommentBytes,
						            true,
						            transferData.AnonymousTransfer
						            );
						    }
						    catch (EnumException<WalletServerSession.ESendTransferErrCodes> enumExc)
						    {
						        if (enumExc.ExceptionCode == WalletServerSession.ESendTransferErrCodes.WrongCertPass)
						        {
                                    if (
                                        await MessageBoxAsync.ShowAsync(this,
                                            ClientGuiMainForm.LocStrings.CommonMessages.WrongPasswordError,
                                            LocStrings.Messages.ErrorMessageCaption,
                                            MessageBoxButtons.RetryCancel,
                                            MessageBoxIcon.Error
                                        ) == DialogResult.Cancel
                                    )
                                    {
                                        whileCondition = false;
                                        return;
                                    }
                                    continue;
                                }
						        else
						        {
						            throw;
						        }
						    }
						}
					}
			        whileCondition = false;
		        }
				catch (Exception exc)
		        {
					if (
						await MessageBoxAsync.ShowAsync(this,
                            locStrings.Messages.SendTransferError.Inject(
								new
								{
									ErrorMessage = exc.Message
								}
							),
							LocStrings.Messages.ErrorMessageCaption,
							MessageBoxButtons.RetryCancel,
							MessageBoxIcon.Error
						) == DialogResult.Cancel
					)
					{
						whileCondition = false;
					}
		        }
            }
            
        }
        private void sendFundsToToolStripMenuItem_Click(
            object sender, 
            EventArgs e
        )
        {
            HandleControlActionProper(async () =>
            {
                Guid walletGuid;
                using (await _focusedItemContextMenu2LockSem.GetDisposable())
                {
                    if (_focusedItemContextMenu2 != null)
                    {
                        walletGuid = (Guid) _focusedItemContextMenu2.Tag;
                    }
                    else
                    {
                        return;
                    }
                }
                SendFundsFromWalletToWallet(
                    walletGuid,
                    new SendFundsFormData()
                );
            });
        }
        private void copyWalletGUIDToolStripMenuItem_Click(
            object sender, 
            EventArgs e
        )
        {
            HandleControlActionProper(async () =>
            {
                using (await _focusedItemContextMenu2LockSem.GetDisposable())
                {
                    if (_focusedItemContextMenu2 != null)
                    {
                        Clipboard.SetText(
                            $"{(Guid) _focusedItemContextMenu2.Tag}"
                            );
                    }
                }
            });
        }
        private void updateBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                using (await _focusedItemContextMenu2LockSem.GetDisposable())
                {
                    if (_focusedItemContextMenu2 != null)
                    {
                        var walletId = (Guid) _focusedItemContextMenu2.Tag;
                        using (await _walletListFormModel.LockSem.GetDisposable())
                        {
                            if (
                                !_walletListFormModel.WalletInfos.ContainsKey(
                                    walletId
                                    )
                                )
                                return;
                            var walletInfo
                                = _walletListFormModel.WalletInfos[walletId];
                            using (await walletInfo.LockSem.GetDisposable())
                            {
                                walletInfo.WalletSession.UpdateBalance();
                            }
                        }
                    }
                }
            });
        }
        private void listView3_SelectedIndexChanged(
            object sender, 
            EventArgs e
        )
        {
            HandleControlActionProper(async () =>
            {
                var selectedItems = walletListView.SelectedItems;
                Guid selectedWalletId;
                if (selectedItems.Count == 0)
                {
                    selectedWalletId = Guid.Empty;
                }
                else
                {
                    selectedWalletId = (Guid) selectedItems[0].Tag;
                }
                using (await _walletListFormModel.LockSem.GetDisposable())
                {
                    _walletListFormModel.ActiveWalletGuid = selectedWalletId;
                }
            });
        }
        public void ProcessWalletInvoice(
            BitmoneyInvoiceData initData,
            Guid initWalletGuid = default(Guid)
        )
        {
            BeginInvokeProper(async () =>
            {
                Assert.NotNull(initData);
                var walletListData = new List<Tuple<string, Guid>>();
                using (await _walletListFormModel.LockSem.GetDisposable())
                {
                    walletListData.AddRange(
                        _walletListFormModel.WalletInfos
                            .Select(walletInfo => Tuple.Create(
                                $"{walletInfo.Value.Balance} {walletInfo.Value.Alias}({walletInfo.Key})".Replace(" ", "_"), 
                                walletInfo.Key
                            )
                        )
                    );
                }
                var tcs = new TaskCompletionSource<Tuple<BitmoneyInvoiceData,Guid>>();
                await (new ProcessWalletInvoiceForm(initData,tcs,walletListData,initWalletGuid))
                    .ShowFormAsync(this);
                var invoiceAndWalletFromData = await tcs.Task;
                if (invoiceAndWalletFromData == null)
                    return;
                SendFundsFromWalletToWallet(
                    invoiceAndWalletFromData.Item2,
                    new SendFundsFormData
                    {
                        Amount = invoiceAndWalletFromData.Item1.TransferAmount,
                        AnonymousTransfer = invoiceAndWalletFromData.Item1.ForceAnonymousTransfer,
                        CommentBytes = invoiceAndWalletFromData.Item1.CommentBytes,
                        ReceiverWalletGuid = invoiceAndWalletFromData.Item1.WalletTo
                    }
                );
            });
        }

        private void processInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(
                () => ProcessWalletInvoice(
                    new BitmoneyInvoiceData()
                )
            );
        }
        private void walletSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                Guid walletGuid;
                using (await _focusedItemContextMenu2LockSem.GetDisposable())
                {
                    if (_focusedItemContextMenu2 != null)
                    {
                        walletGuid = (Guid) _focusedItemContextMenu2.Tag;
                    }
                    else
                    {
                        return;
                    }
                }
                IWalletServerSessionModel sessionModel;
                using (await SemaphoreSlimExtensions.GetDisposable(_walletListFormModel.LockSem))
                {
                    if (!_walletListFormModel.WalletInfos.ContainsKey(walletGuid))
                        return;
                    var walletFormModel = 
                        _walletListFormModel.WalletInfos[walletGuid];
                    sessionModel = walletFormModel.SessionModel;
                }
                await new EditWalletSettingsForm(sessionModel)
                    .ShowFormAsync(this);
            });
        }
        private void changePasswordToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var locStrings = LocStrings.WalletServerLocStringsInstance
                .ChangePasswordsLocStringsInstance;
            ChangePasswordTemplate<WalletProfile>(
                new ChangePasswordTemplateLocStrings()
                {
                    NewCertPasswordRequestText = locStrings.EnterNewCertificatePasswordText,
                    NewMasterCertPasswordRequestText = locStrings.EnterNewMasterCertificatePasswordText,
                    NewProfileFilePasswordRequestText = locStrings.EnterNewProfileFilePasswordText,
                    OldCertPasswordRequestText = locStrings.EnterOldCertificatePasswordText,
                    OldMasterCertPasswordRequestText = locStrings.EnterOldMasterCertificatePasswordText,
                    OldProfileFilePasswordRequestText = locStrings.EnterOldProfileFilePasswordText,
                    SelectPasswordToChangeFormCaption = locStrings.SelectWalletPasswordToChange,
                    SelectProfileFormCaption = LocStrings.WalletServerLocStringsInstance
                        .ConnectingLocStringsInstance.SelectWalletProfile
                },
                _walletActionLockSem, 
                GetWalletActionInProgressDisposable,
                DefaultFolders.WalletProfilesFolder,
                async walletProfileName =>
                {
                    using (await _walletListFormModel.LockSem.GetDisposable())
                    {
                        foreach (
                            var walletInfo
                                in _walletListFormModel.WalletInfos.Values
                            )
                        {
                            using (await walletInfo.LockSem.GetDisposable())
                            {
                                if (walletInfo.Alias == walletProfileName)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                },
                new List<EProfilePasswordKinds>()
                {
                    EProfilePasswordKinds.Profile,
                    EProfilePasswordKinds.Cert,
                    EProfilePasswordKinds.MasterCert
                },
                _ => _.WalletCert,
                _ => _.MasterWalletCert
            );
        }
        private void externalPaymentProcessorSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                if (!await CheckPrivateCommonSettingsPassword())
                    return;
                await new ExternalPaymentProcessorSettingsForm(
                    GlobalModelInstance.CommonPrivateSettings.ExternalTransferProcessorSettings
                ).ShowFormAsync(this);
            });
        }
        private void repeatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(() =>
            {
                var focusedItem = walletRecentTransferListView.FocusedItem;
                if (focusedItem == null)
                {
                    return;
                }
                var transferInfo = focusedItem.Tag as WalletFormModelTransferInfo;
                if (transferInfo == null)
                {
                    return;
                }
                if (
                    !(transferInfo.TransferStatus == WalletFormModelTransferStatus.Sent ||
                    transferInfo.TransferStatus == WalletFormModelTransferStatus.SendError)
                )
                {
                    ShowErrorMessage(
                        LocStrings.WalletServerLocStringsInstance
                            .Messages.RepeatTransfersTypeError
                    );
                    return;
                }
                ProcessWalletInvoice(
                    new BitmoneyInvoiceData
                    {
                        CommentBytes = transferInfo.CommentBytes,
                        ForceAnonymousTransfer = transferInfo.AnonymousTransfer,
                        TransferAmount = transferInfo.Amount,
                        WalletTo = transferInfo.WalletTo
                    },
                    transferInfo.WalletFrom
                );
            });
        }
        private void showFullTransferHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                Guid walletGuid;
                using (await _focusedItemContextMenu2LockSem.GetDisposable())
                {
                    if (_focusedItemContextMenu2 != null)
                    {
                        walletGuid = (Guid)_focusedItemContextMenu2.Tag;
                    }
                    else
                    {
                        return;
                    }
                }
                IWalletFormModel walletFormModel;
                using (await _walletListFormModel.LockSem.GetDisposable())
                {
                    if (!_walletListFormModel.WalletInfos.ContainsKey(walletGuid))
                        return;
                    walletFormModel =
                        _walletListFormModel.WalletInfos[walletGuid];
                }
                new FullTransferHistoryForm(walletFormModel).Show(this);
            });
        }

        private async void processInvoiceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Guid walletGuid;
            using (await _focusedItemContextMenu2LockSem.GetDisposable())
            {
                if (_focusedItemContextMenu2 != null)
                {
                    walletGuid = (Guid)_focusedItemContextMenu2.Tag;
                }
                else
                {
                    return;
                }
            }
            ProcessWalletInvoice(new BitmoneyInvoiceData(),walletGuid);
        }
        private void columnAutowidthByHeaderToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            walletRecentTransferListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void columnAutowidthByContentToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            walletRecentTransferListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
}
