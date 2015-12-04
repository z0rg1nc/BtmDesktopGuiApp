using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Message;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.GeneralClientInterfaces;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.SamHelper;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        //Context menu for i2p destination balance label
        private void toolStripStatusLabel2_MouseUp(
            object sender, MouseEventArgs e
        )
        {
            HandleControlActionProper(() =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    var label = (ToolStripItem) sender;
                    contextMenuStrip_i2pDestinationBalanceLavel.Show(
                        statusStrip1,
                        label.Bounds.X + e.X,
                        label.Bounds.Y + e.Y
                        );
                }
            });
        }
        private IProxyServerSessionSettings ProxySessionSettings
        {
            get
            {
                if (
                    Forms.MainForm.ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings == null
                    || Forms.MainForm.ClientGuiMainForm.GlobalModelInstance.CommonPrivateSettings == null
                )
                    throw new Exception(
                        "publicCommonSettings == null || privateCommonSettings == null"
                    );
                var publicProxySettings =
                    Forms.MainForm.ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.ProxySettings;
                var privateProxySettings =
                    Forms.MainForm.ClientGuiMainForm.GlobalModelInstance.CommonPrivateSettings.ProxySettings;
                if (publicProxySettings == null || privateProxySettings == null)
                    throw new Exception(
                        "publicProxySettings == null || privateProxySettings == null"
                    );
                var proxySessionSettings = new ProxyServerSessionSettings()
                {
                    SamServerAddress = publicProxySettings.SamServerAddress,
                    SamServerPort = publicProxySettings.SamServerPort,
                    ProxyServerClientInfos =
                        CommonClientConstants.ProxyServerClientInfos,
                    ClientI2PPrivateKeys = null /*privateProxySettings.ClientPrivKeys*/,
                    AutoFillup = false /*publicProxySettings.AutoFillup*/,
                    AutoFillupMinBalance = 0.0m
                        /* publicProxySettings.AutoFillupMinBalance */
                };
                var proxyProxySessionSettings = new MyPropertiesProxy<IProxyServerSessionSettings>(
                    proxySessionSettings,
                    new List<PropertyBindingInfo>()
                    {
                        PropertyBindingInfo.Create(
                            publicProxySettings,
                            new List<Tuple<string, string>>()
                            {
                                new Tuple<string, string>(
                                    proxySessionSettings.MyNameOfProperty(e => e.AutoFillup),
                                    publicProxySettings.MyNameOfProperty(e => e.AutoFillup)
                                ),
                                new Tuple<string, string>(
                                    proxySessionSettings.MyNameOfProperty(e => e.AutoFillupMinBalance),
                                    publicProxySettings.MyNameOfProperty(e => e.AutoFillupMinBalance)
                                )
                            }
                        ),
                        PropertyBindingInfo.Create(
                            privateProxySettings,
                            new List<Tuple<string, string>>()
                            {
                                new Tuple<string, string>(
                                    proxySessionSettings.MyNameOfProperty(e => e.ClientI2PPrivateKeys),
                                    privateProxySettings.MyNameOfProperty(e => e.ClientPrivKeys)
                                )
                            }
                        )
                    }
                ).GetProxy();
                return proxyProxySessionSettings;
            }
        }
        private void i2PSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                if (!await CheckPrivateCommonSettingsPassword())
                    return;
                await (
                    new EditI2PSettingsForm(
                        GlobalModelInstance.CommonPublicSettings.ProxySettings,
                        GlobalModelInstance.CommonPrivateSettings.ProxySettings
                    )
                ).ShowFormAsync(this);
            });
        }
        private readonly SemaphoreSlim _proxyActionLockSem = new SemaphoreSlim(1);
        private bool _proxyActionInProgress = false;
        private ActionDisposable GetProxyActionInProgressDisposable()
        {
            return new ActionDisposable(
                () => { _proxyActionInProgress = true; },
                () => { _proxyActionInProgress = false; }
            );
        }
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.ProxyServerLocStringsInstance;
                if (_proxyActionInProgress)
                {
                    ShowErrorMessage(
                        locStrings.Messages.ProxyServerActionInProgressError
                    );
                    return;
                }
                await ConnectProxySession();
            });
        }
        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.ProxyServerLocStringsInstance;
                if (_proxyActionInProgress)
                {
                    ShowErrorMessage(
                        locStrings.Messages.ProxyServerActionInProgressError
                    );
                    return;
                }
                await DisconnectProxySession();
            });
        }

        private async Task DisconnectProxySession(bool dontShowProgressForm = false)
        {
            var locStrings = LocStrings.ProxyServerLocStringsInstance;
            using (await _proxyActionLockSem.GetDisposable())
            {
                using (GetProxyActionInProgressDisposable())
                {
                    using (await _walletListFormModel.LockSem.GetDisposable())
                    {
                        if (_walletListFormModel.WalletInfos.Count > 0)
                        {
                            ShowErrorMessage(
                                locStrings.DisconnectingLocStringsInstance
                                    .WalletServerConnectedError
                            );
                            return;
                        }
                    }
                    if (_exchangeModel.ExchangeServerConnected)
                    {
                        ShowErrorMessage(
                            locStrings.DisconnectingLocStringsInstance
                                .ExchangeServerConnectedError
                        );
                        return;
                    }
                    if (_miningModel.MiningServerConnected)
                    {
                        ShowErrorMessage(
                            locStrings.DisconnectingLocStringsInstance
                                .MiningServerConnectedError
                        );
                        return;
                    }
                    if (
                        _messageClientModel.ConnectionStatus 
                        != MessageSesionConnectionStatus.Disconnected
                    )
                    {
                        ShowErrorMessage(
                            locStrings.DisconnectingLocStringsInstance
                                .MessageServerConnectedError
                        );
                        return;
                    }
                    if (!_proxyModel.ProxyServerConnected)
                    {
                        ShowErrorMessage(
                            LocStrings.CommonMessages.ProxyServerIsNotConnectedError
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
                            if (!dontShowProgressForm)
                            {
                                progressForm.ReportProgress(
                                    locStrings.DisconnectingLocStringsInstance.ProgressFormReport1,
                                    0
                                    );
                            }
                            await _proxySession.MyDisposeAsync();
                        }
                        finally
                        {
                            if (!dontShowProgressForm)
                            {
                                progressForm.SetProgressComplete();
                            }
                        }
                    }
                    using (await _proxyModel.LockSem.GetDisposable())
                    {
                        _proxyModel.ProxyServerConnected = false;
                    }
                    _proxySession = null;
                }
            }
        }

        private async Task ConnectProxySession()
        {
            var locStrings = LocStrings.ProxyServerLocStringsInstance;
            using (await _proxyActionLockSem.GetDisposable())
            {
                using (GetProxyActionInProgressDisposable())
                {
                    if (_proxyModel.ProxyServerConnected)
                    {
                        ShowErrorMessage(
                            locStrings.Messages.ProxyServerConnectedError
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
                                LocStrings.CommonText.Start,
                                0
                                );
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport1,
                                20
                                );
                            _proxySession = await ProxyServerSessionOverI2P.CreateInstance(
                                _miningManager,
                                ProxySessionSettings,
                                _proxyModel.ProxySessionModel,
                                cts.Token
                                );
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport2,
                                100
                                );
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        catch (Exception exc)
                        {
                            var enExc = exc as EnumException<SamHelper.SamHelper.ECreateInstanceErrCodes>;
                            if (
                                enExc != null
                                &&
                                enExc.ExceptionCode
                                == SamHelper.SamHelper.ECreateInstanceErrCodes.ConnectTcp
                                )
                            {
                                ShowErrorMessage(
                                    LocStrings.ProxyServerLocStringsInstance
                                        .Messages.SamConnectionFailedError
                                    );
                            }
                            else
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.UnexpectedErrorMessage.Inject(
                                        new
                                        {
                                            ErrorMessage = exc.Message
                                        }
                                        )
                                    );
                                _log.Error(
                                    "ConnectProxySession ProxyServerSession.CreateInstance '{0}'",
                                    exc.ToString()
                                    );
                            }
                            return;
                        }
                        finally
                        {
                            progressForm.SetProgressComplete();
                        }
                    }
                    using (await _proxyModel.LockSem.GetDisposable())
                    {
                        _proxyModel.ProxyServerConnected = true;
                    }
                    //_proxySession.UpdateBalance();
                }
            }
        }

        private void forcePingProxyToolStripMenuItem_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(() =>
            {
                if (!_proxyModel.ProxyServerConnected)
                {
                    ShowErrorMessage(
                        LocStrings.CommonMessages.ProxyServerIsNotConnectedError
                    );
                    return;
                }
                _proxySession.ForcePing();
            });
        }
        private void issueInvoiceToFillupToolStripMenuItem_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(
                async () =>
            {
                var locStrings = LocStrings.ProxyServerLocStringsInstance
                    .IssueInvoiceToRefillLocStringsInstance;
                if (!_proxyModel.ProxyServerConnected)
                {
                    ShowErrorMessage(
                        LocStrings.CommonMessages.ProxyServerIsNotConnectedError
                    );
                    return;
                }
                var inputBoxForm = new InputBoxForm(
                    locStrings.RefillAmount, "1", @"^[1-9]{1}\d*$"
                );
                await inputBoxForm.ShowFormAsync(this);
                string transferAmountString = await inputBoxForm.TaskValue;
                if (transferAmountString == string.Empty)
                    return;
                long transferAmount;
                if (
                    !long.TryParse(
                        transferAmountString, 
                        out transferAmount
                    )
                    || transferAmount <= 0
                    || transferAmount > int.MaxValue
                )
                {
                    Forms.MainForm.ClientGuiMainForm.ShowErrorMessage(
                        this,
                        locStrings.WrongTransferAmountError
                    );
                    return;
                }
                var issueData = await _proxySession.IssueInvoiceToFillup(
                    transferAmount
                );
                ProcessWalletInvoice(issueData);
            });
        }
        private void updateBalanceToolStripMenuItem2_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(() =>
            {
                if (!_proxyModel.ProxyServerConnected)
                {
                    ShowErrorMessage(
                        LocStrings.CommonMessages.ProxyServerIsNotConnectedError
                    );
                    return;
                }
                _proxySession.UpdateBalance();
            });
        }
    }
}
