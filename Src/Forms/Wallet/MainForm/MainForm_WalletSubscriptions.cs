using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.BitMoneyClient.Gui.Forms.Wallet;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.MyNotifyPropertyChanged.MyObservableCollections;
using BtmI2p.MyNotifyPropertyChanged.Winforms;
using BtmI2p.ObjectStateLib;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private async void ProcessTransferInfoToExternalApp(
            WalletFormModelTransferInfo transferInfoForExternal
        )
        {
            var curMethodName = this.MyNameOfMethod(e => e.ProcessTransferInfoToExternalApp(null));
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    var externalProcessorSettings
                        = Forms.MainForm.ClientGuiMainForm.GlobalModelInstance.CommonPrivateSettings.ExternalTransferProcessorSettings;
                    var lockSem = externalProcessorSettings.LockSem;
                    using (await lockSem.GetDisposable())
                    {
                        if (GlobalModelInstance.CommonPublicSettings.UseExternalPaymentProcessor)
                        {
                            if (
                                (
                                    transferInfoForExternal.TransferStatus 
                                        == WalletFormModelTransferStatus.SendError  
                                    && !externalProcessorSettings.ProcessSendTransferFaults
                                )
                                ||
                                (   
                                    transferInfoForExternal.TransferStatus 
                                        == WalletFormModelTransferStatus.Sent
                                    && transferInfoForExternal.Direction
                                    && !externalProcessorSettings.ProcessSentTransfers
                                )
                                || 
                                (
                                    transferInfoForExternal.TransferStatus 
                                        == WalletFormModelTransferStatus.Received
                                    && !transferInfoForExternal.Direction
                                    && !externalProcessorSettings.ProcessReceivedTransfers
                                )
                                || 
                                (
                                    transferInfoForExternal.TransferStatus
                                        == WalletFormModelTransferStatus.PreparedToSend
                                )
                            )
                            {
                                return;
                            }
                            var commandLineArgs
                                = externalProcessorSettings.CommandLineArguments
                                    .Replace(
                                        "__PAYMENT_TYPE__",
                                        string.Format(
                                            "{0}", 
                                                transferInfoForExternal.TransferStatus 
                                                    == WalletFormModelTransferStatus.SendError 
                                                    ? 0 
                                                    : transferInfoForExternal.TransferStatus 
                                                        == WalletFormModelTransferStatus.Sent 
                                                        ? 1 : 2
                                        )
                                    ).Replace(
                                        "__REQUEST_GUID__",
                                        $"{transferInfoForExternal.RequestGuid}"
                                    )
                                    .Replace(
                                        "__TRANSFER_GUID__",
                                        $"{transferInfoForExternal.TransferGuid}"
                                    ).Replace(
                                        "__WALLET_FROM__",
                                        $"{transferInfoForExternal.WalletFrom}"
                                    ).Replace(
                                        "__WALLET_TO__",
                                        $"{transferInfoForExternal.WalletTo}"
                                    ).Replace(
                                        "__ANONYMOUS_INT__",
                                        transferInfoForExternal.AnonymousTransfer ? "1" : "0"
                                    ).Replace(
                                        "__AMOUNT__",
                                        $"{transferInfoForExternal.Amount}"
                                    ).Replace(
                                        "__FEE__",
                                        $"{transferInfoForExternal.Fee}"
                                    ).Replace(
                                        "__COMMENT_BYTES_B64__",
                                        Convert.ToBase64String(transferInfoForExternal.CommentBytes)
                                    ).Replace(
                                        "__TIME_UTC__",
                                        $"{transferInfoForExternal.SentTime:yyyyMMddHHmmss}"
                                    ).Replace(
                                        "__SEND_ERROR_CODE__",
                                        $"{(int) transferInfoForExternal.ErrCode}"
                                    ).Replace(
                                        "__SEND_SERVER_GENERAL_ERROR_CODE__",
                                        $"{(int)transferInfoForExternal.GeneralServerErrCode}"
                                    ).Replace(
                                        "__SEND_SERVER_ERROR_CODE__",
                                        $"{(int)transferInfoForExternal.ServerErrCode}"
                                    ).Replace(
                                        "__SEND_ERROR_MESSAGE__",
                                        transferInfoForExternal.ErrMessage
                                    ).Replace(
                                        "__AUTH_OTHER_WALLET_CERT__",
                                        $"{(transferInfoForExternal.AuthenticatedOtherWalletCert ? 1 : 0)}"
                                    ).Replace(
                                        "__AUTH_COMMENT_KEY__",
                                        $"{(transferInfoForExternal.AuthenticatedCommentKey ? 1 : 0)}"
                                    ).Replace(
                                        "__AUTH_PAYMENT_DETAILS__",
                                        $"{(transferInfoForExternal.AuthenticatedTransferDetails ? 1 : 0)}"
                                    );
                            /*_log.Trace(
                                "{0} args {1}",
                                this.MyNameOfMethod(
                                    e => e.ProcessTransferInfoToExternalApp(null)
                                ),
                                commandLineArgs
                            );*/
                            var processStartInfo = new ProcessStartInfo();
                            var workingDirectory = Path.GetDirectoryName(
                                externalProcessorSettings.ExternalProcessorAppPath
                            );
                            if (string.IsNullOrWhiteSpace(workingDirectory))
                                throw new ArgumentNullException(
                                    MyNameof.GetLocalVarName(() => workingDirectory));
                            processStartInfo.WorkingDirectory = workingDirectory;
                            processStartInfo.FileName 
                                = externalProcessorSettings.ExternalProcessorAppPath;
                            processStartInfo.Arguments = commandLineArgs;
                            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            Process.Start(
                                processStartInfo
                            );
                        }
                    }
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,_log);
            }
        }
        
        private async Task ReplaceOringinWalletTransferObservableCollection(Guid walletId)
        {
            using (await _walletListFormModel.LockSem.GetDisposable())
            {
                if (
                    walletId == Guid.Empty ||
                    !_walletListFormModel.WalletInfos.ContainsKey(walletId)
                    )
                {
                    await _hotSwapObservableCollection.ReplaceOriginCollectionChanged(
                        _emptyObservableCollection
                        );
                }
                else
                {
                    var walletInfo = _walletListFormModel.WalletInfos[walletId];
                    await _hotSwapObservableCollection.ReplaceOriginCollectionChanged(
                        walletInfo.TransferObservableCollection
                    );
                }
            }
        }
        
        
        /**/
        private readonly SemaphoreSlim _walletListViewItemsLockSem
            = new SemaphoreSlim(1);
        private readonly Dictionary<Guid,ListViewItem> _walletListViewItems
            = new Dictionary<Guid, ListViewItem>();

        private async Task OnWalletPropertyChanged(
            Guid walletId, 
            MyNotifyPropertyChangedArgs args
        )
        {
            if (
                args.PropertyName 
                == MyNameof<WalletFormModel>.Property(
                    e => e.Balance
                )
            )
            {
                var newBalance = (long) args.CastedNewProperty;
                using (await _walletListViewItemsLockSem.GetDisposable())
                {
                    if (!_walletListViewItems.ContainsKey(walletId))
                        return;
                    var walletListViewItem = _walletListViewItems[walletId];
                    walletListViewItem.SubItems[1].Text 
                        = string.Format(
                            "{0}", 
                            newBalance
                        );
                }
            }
        }

        private readonly MyObservableCollectionSafeAsyncImpl<WalletFormModelTransferInfo> _emptyObservableCollection
            = new MyObservableCollectionSafeAsyncImpl<WalletFormModelTransferInfo>();
        private MyHotSwapObservableCollectionImpl<WalletFormModelTransferInfo> _hotSwapObservableCollection;
        private readonly ObservableCollectionProxyFilter<WalletFormModelTransferInfo> _proxyFilter
            = new ObservableCollectionProxyFilter<WalletFormModelTransferInfo>(async _ => await Task.FromResult(true));
        private readonly MyObservableCollectionProxyComparer<WalletFormModelTransferInfo> _proxyComparer
            = new MyObservableCollectionProxyComparer<WalletFormModelTransferInfo>(
                Comparer<WalletFormModelTransferInfo>.Create(
                    (ord1, ord2) => -ord1.SentTime.CompareTo(ord2.SentTime)
                )
            );
        private readonly MyObservableCollectionProxyN _proxyN
            = new MyObservableCollectionProxyN(100);
        private async Task InitWalletSubscriptions()
        {
            _hotSwapObservableCollection = await MyHotSwapObservableCollectionImpl.CreateInstance(
                _emptyObservableCollection
            );
            var filteredCollection = await MyFilteredObservableCollectionImpl.CreateInstance(
                _hotSwapObservableCollection,
                _proxyFilter
            );
            var orderedCollection = await MyOrderedObservableCollection.CreateInstance(
                filteredCollection,
                _proxyComparer
            );
            var firstNCollection = await MyNFirstObservableCollectionImpl.CreateInstance(
                orderedCollection,
                _proxyN
            );
            var binding = await ListViewCollectionChangedOneWayBinding.CreateInstance(
                walletRecentTransferListView,
                firstNCollection,
                null,
                FullTransferHistoryForm.EditWalletTransferListViewItem
            );
            _formAsyncSubscriptions.Add(
                new CompositeMyAsyncDisposable(
                    binding,
                    firstNCollection,
                    orderedCollection,
                    filteredCollection,
                    _hotSwapObservableCollection
                )
            );
            _formSubscriptions.Add(
                _walletListFormModel.PropertyChangedSubject.Subscribe(
                    x => BeginInvokeProper(
                        async () =>
                        {
                            if (
                                x.PropertyName 
                                == _walletListFormModel.MyNameOfProperty(e => e.ActiveWalletGuid)
                            )
                            {
                                var activeWalletId = _walletListFormModel.ActiveWalletGuid;
                                await ReplaceOringinWalletTransferObservableCollection(
                                    activeWalletId
                                );
                            }
                        }
                    )
                )
            );
            _formSubscriptions.Add(
                _walletListFormModel.OnWalletInfoAdded.Subscribe(
                    x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _walletListViewItemsLockSem.GetDisposable())
                            {
                                if (!_walletListViewItems.ContainsKey(
                                    x.WalletProfileInstance.WalletCert.Id))
                                {
                                    var newListViewItem = new ListViewItem(
                                        string.Format(
                                            "{0}#{1}#{2}",
                                            x.Alias,
                                            x.Balance,
                                            x.WalletProfileInstance.WalletCert.Id
                                        ).Split('#')
                                    );
                                    newListViewItem.Font = walletListView.Font;
                                    /**/
                                    newListViewItem.Tag
                                        = x.WalletProfileInstance.WalletCert.Id;
                                    walletListView.Items.Add(newListViewItem);
                                    _walletListViewItems.Add(
                                        x.WalletProfileInstance.WalletCert.Id,
                                        newListViewItem
                                    );
                                }
                            }
                        }
                    )
                )
            );
            _formSubscriptions.Add(
                _walletListFormModel.OnWalletInfoRemoved.Subscribe(
                    x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _walletListViewItemsLockSem.GetDisposable())
                            {
                                if (_walletListViewItems.ContainsKey(x))
                                {
                                    walletListView.Items.Remove(
                                        _walletListViewItems[x]
                                    );
                                    _walletListViewItems.Remove(x);
                                }
                            }
                            if (_walletListFormModel.ActiveWalletGuid == x)
                            {
                                _walletListFormModel.ActiveWalletGuid = Guid.Empty;
                            }
                        }
                    )
                )
            );
            _formSubscriptions.Add(
                _walletListFormModel.OnWalletInfoChanged.Subscribe(
                    x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _walletListViewItemsLockSem.GetDisposable())
                            {
                                if (_walletListViewItems.ContainsKey(
                                    x.WalletProfileInstance.WalletCert.Id))
                                {
                                    var walletListViewItem =
                                        _walletListViewItems[x.WalletProfileInstance.WalletCert.Id];
                                    walletListViewItem.SubItems[0].Text 
                                        = x.Alias;
                                    walletListViewItem.SubItems[1].Text 
                                        = string.Format((string) "{0}",(object) x.Balance);
                                    walletListViewItem.SubItems[2].Text 
                                        = string.Format(
                                        (string) "{0}",
                                        (object) x.WalletProfileInstance.WalletCert.Id
                                    );
                                }
                            }
                        }
                    )
                )
            );
        }
    }
}
