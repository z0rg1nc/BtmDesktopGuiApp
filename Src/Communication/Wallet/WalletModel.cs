using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Lib.WalletServerSession;
using BtmI2p.CryptFile.Lib;
using BtmI2p.GeneralClientInterfaces.WalletServer;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.MyNotifyPropertyChanged.MyObservableCollections;
using BtmI2p.Newtonsoft.Json;
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Communication.Wallet
{
    public enum WalletFormModelTransferStatus
    {
        PreparedToSend,
        SendError,
        Sent,
        Received
    }
    public class WalletFormModelTransferInfo : ClientTransferBase
    {
        private WalletFormModelTransferInfo()
        {
        }

        public static WalletFormModelTransferInfo FromOutcomeTransfer(
            ClientTransferBase outcomeTransfer,
            Guid walletFrom
        )
        {
            return new WalletFormModelTransferInfo()
            {
                Amount = outcomeTransfer.Amount,
                Fee = outcomeTransfer.Fee,
                AnonymousTransfer =
                    outcomeTransfer.AnonymousTransfer,
                CommentBytes = outcomeTransfer.CommentBytes,
                Direction = true,
                SentTime = outcomeTransfer.SentTime,
                TransferGuid = outcomeTransfer.TransferGuid,
                TransferStatus =
                    WalletFormModelTransferStatus.Sent,
                WalletFrom = walletFrom,
                WalletTo = outcomeTransfer.WalletTo,
                RequestGuid = outcomeTransfer.RequestGuid,
                AuthenticatedOtherWalletCert = outcomeTransfer.AuthenticatedOtherWalletCert,
                AuthenticatedCommentKey = outcomeTransfer.AuthenticatedCommentKey,
                AuthenticatedTransferDetails = outcomeTransfer.AuthenticatedTransferDetails
            };
        }

        public static WalletFormModelTransferInfo FromIncomeTransfer(
            ClientTransferBase incomeTransfer,
            Guid walletTo
        )
        {
            return new WalletFormModelTransferInfo()
            {
                Amount = incomeTransfer.Amount,
                CommentBytes = incomeTransfer.CommentBytes,
                Direction = false,
                SentTime = incomeTransfer.SentTime,
                TransferGuid = incomeTransfer.TransferGuid,
                TransferStatus =
                    WalletFormModelTransferStatus.Received,
                WalletFrom = incomeTransfer.WalletFrom,
                WalletTo = walletTo,
                AnonymousTransfer = incomeTransfer.AnonymousTransfer,
                RequestGuid = Guid.Empty,
                AuthenticatedOtherWalletCert = incomeTransfer.AuthenticatedOtherWalletCert,
                AuthenticatedCommentKey = incomeTransfer.AuthenticatedCommentKey,
                AuthenticatedTransferDetails = incomeTransfer.AuthenticatedTransferDetails
            };
        }

        public static WalletFormModelTransferInfo FromPrepared(
            PreparedToSendTransfer preparedTransfer,
            Guid walletFrom
        )
        {
            return new WalletFormModelTransferInfo()
            {
                Amount = preparedTransfer.Amount,
                Direction = true,
                CommentBytes = preparedTransfer.CommentBytes,
                SentTime = DateTime.UtcNow,
                TransferGuid = Guid.NewGuid(),
                TransferStatus
                    = WalletFormModelTransferStatus.PreparedToSend,
                WalletFrom = walletFrom,
                WalletTo = preparedTransfer.WalletTo,
                AnonymousTransfer = preparedTransfer.AnonymousTransfer,
                RequestGuid = preparedTransfer.RequestGuid
            };
        }

        public bool Direction; // true = sent, false - receive
        public WalletFormModelTransferStatus TransferStatus;
        /**/
        public EPreparedToSendTransferFaultErrCodes ErrCode 
            = EPreparedToSendTransferFaultErrCodes.None;
        public string ErrMessage = string.Empty;
        /**/
        public EProcessSimpleTransferErrCodes ServerErrCode
            = EProcessSimpleTransferErrCodes.NoErrors;
        public EWalletGeneralErrCodes GeneralServerErrCode
            = EWalletGeneralErrCodes.NoErrors;
        /**/
        public void SetFaulted(
            EPreparedToSendTransferFaultErrCodes errCode,
            string errMessage,
            EProcessSimpleTransferErrCodes serverErrCode,
            EWalletGeneralErrCodes generalServerErrCode
        )
        {
            if(TransferStatus != WalletFormModelTransferStatus.PreparedToSend)
                throw new InvalidOperationException(
                    this.MyNameOfProperty(e => e.TransferStatus));
            ErrCode = errCode;
            ErrMessage = errMessage;
            TransferStatus = WalletFormModelTransferStatus.SendError;
            ServerErrCode = serverErrCode;
            GeneralServerErrCode = generalServerErrCode;
        }
    }
    
    public interface IWalletFormModel : 
        IMyNotifyPropertyChanged, 
        IMyAsyncDisposable, 
        ILockSemaphoreSlim
    {
        List<IDisposable> Subscriptions { get;  }
        /**/
        string Alias { get; set; }
        WalletServerSession WalletSession { get; set; }
        IWalletServerSessionModel SessionModel { get; set; }
        WalletTransferHistory WalletTransferHistoryInstance { get; set; }
        /**/
        WalletProfile WalletProfileInstance { get; set; }
        /**/
        IWalletSettings WalletSetting { get; set; }
        string WalletSettingsFilePath { get; set; }
        byte[] WalletSettingsFilePassword { get; set; }
        /**/
        long Balance { get; set; }

        MyObservableCollectionSafeAsyncImpl<WalletFormModelTransferInfo> 
            TransferObservableCollection { get; }
        /*Key - TransferGuid for send\received, RequestGuid for PreparedToSend,SendFaulted
        Dictionary<Guid, WalletFormModelTransferInfo> TransferInfos { get; set; }
        Subject<WalletFormModelTransferInfo> OnTransferAdded { get;  }
        Subject<Guid> OnTransferRemoved { get; }
        Subject<WalletFormModelTransferInfo> OnTransferChanged { get; }
        */
        void OnSettingsChanged();

        void SessionModel_OnPreparedToSendTransferFault(
            OnPreparedToSendTransferFaultArgs trasferFaultInfo
        );

        void SessionModel_OnPreparedToSendTransferComplete(
            Guid requestGuid
        );

        void SessionModel_OnPreparedToSendTransferAdded(
            PreparedToSendTransfer preparedTransfer
        );

        void SessionModel_OnTransferReceived(
            List<ClientTransferBase> incomeTransfers
        );

        void SessionModel_OnTransferSent(
            List<ClientTransferBase> transferInfos
        );

        void OnTransferReceivedToTransferHistory(
            IList<List<ClientTransferBase>> x,
            IWalletFormModel proxy
        );
        void OnTransferSentToTransferHistory(
            IList<List<ClientTransferBase>> x,
            IWalletFormModel proxy
        );

    }

    public class WalletFormModel : IWalletFormModel
    {
        private WalletFormModel()
        {
        }

        private Form _owner;
        public static WalletFormModel CreateInstance(Form owner)
        {
            var result = new WalletFormModel();
            result._owner = owner;
            result._stateHelper.SetInitializedState();
            return result;
        }
        private readonly DisposableObjectStateHelper _stateHelper 
            = new DisposableObjectStateHelper("WalletFormModel");
        public List<IDisposable> Subscriptions { get; private set; } = new List<IDisposable>();
        /**/
        public string Alias { get; set; }
        public WalletServerSession WalletSession { get; set; }
        public IWalletServerSessionModel SessionModel { get; set; }
        public WalletTransferHistory WalletTransferHistoryInstance { get; set; }
        /**/
        public WalletProfile WalletProfileInstance { get; set; }
        /**/
        public IWalletSettings WalletSetting { get; set; }
        public string WalletSettingsFilePath { get; set; }
        public byte[] WalletSettingsFilePassword { get; set; }
        /**/
        public long Balance { get; set; } = 0;
        /**/
        public SemaphoreSlim LockSem { get; } = new SemaphoreSlim(1);
        public MyObservableCollectionSafeAsyncImpl<WalletFormModelTransferInfo> TransferObservableCollection { get; } 
            = new MyObservableCollectionSafeAsyncImpl<WalletFormModelTransferInfo>();
        /* Key - Request guid if prepared, Transfer guid of sent
        public Dictionary<Guid, WalletFormModelTransferInfo> TransferInfos { get; set; }
        public Subject<WalletFormModelTransferInfo> OnTransferAdded { get; private set; }
        public Subject<Guid> OnTransferRemoved { get; private set; }
        public Subject<WalletFormModelTransferInfo> OnTransferChanged { get; private set; }
        */
        /**/
        private static readonly Logger _logger
            = LogManager.GetCurrentClassLogger();
        public async void SessionModel_OnTransferSent(
            List<ClientTransferBase> transferInfos
        )
        {
            try
            {
                using(_stateHelper.GetFuncWrapper())
                using (await LockSem.GetDisposable().ConfigureAwait(false))
                {
                    await TransferObservableCollection.RemoveWhereAsync(
                        _ => transferInfos.Any(__ => __.RequestGuid == _.RequestGuid)
                    ).ConfigureAwait(false);
                    await TransferObservableCollection.AddRangeAsync(
                        transferInfos
                            .Select(
                                _ => WalletFormModelTransferInfo.FromOutcomeTransfer(
                                    _, 
                                    WalletProfileInstance.WalletCert.Id
                                )
                            ).ToList()
                    ).ConfigureAwait(false);
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,_logger);
            }
        }

        public void OnSettingsChanged()
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    ScryptPassEncryptedData.WriteToFile(
                        WalletSetting,
                        WalletSettingsFilePath,
                        WalletSettingsFilePassword
                    );
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,_logger);
            }
        }

        public async void SessionModel_OnPreparedToSendTransferFault(
			OnPreparedToSendTransferFaultArgs trasferFaultInfo
        )
        {
            try{
                using(_stateHelper.GetFuncWrapper())
                using (await LockSem.GetDisposable().ConfigureAwait(false))
                {
                    var requestGuid = trasferFaultInfo.RequestGuid;
                    var preparedTransfer = trasferFaultInfo.PreparedTransfer;
                    var errCode = trasferFaultInfo.FaultCode;
                    var errMessage = trasferFaultInfo.FaultMessage;
                    var indices = await TransferObservableCollection.IndexesOfAsync(
                        _ => _.RequestGuid == requestGuid
                        ).ConfigureAwait(false);
                    if (indices.Count == 0)
                        return;
                    var transferInfo = await TransferObservableCollection.GetItemDeepCopyAtAsync(
                        indices.First()
                        ).ConfigureAwait(false);
                    Assert.NotNull(transferInfo);
                    if (
                        transferInfo.TransferStatus
                        != WalletFormModelTransferStatus.PreparedToSend
                    )
                        return;
                    transferInfo.SetFaulted(errCode,errMessage,trasferFaultInfo.ServerFaultCode,trasferFaultInfo.ServerGeneralFaultCode);
                    await TransferObservableCollection.ReplaceItemAtAsync(
                        indices.First(),
                        transferInfo
                    ).ConfigureAwait(false);
                    ClientGuiMainForm.ShowErrorMessage(
                        _owner,
                        ClientGuiMainForm.LocStrings.WalletServerLocStringsInstance
                            .Messages.SendTransferExtendedError.Inject(
                                new
                                {
                                    RequestGuid = requestGuid,
                                    ErrorCode = errCode,
                                    ErrorMessage = errMessage
                                }
                            )
                    );
                    _logger.Trace(
                        "SessionModel_OnPreparedToSendTransferFault complete"
                    );
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,_logger);
            }
        }

        public async void SessionModel_OnPreparedToSendTransferComplete(
            Guid requestGuid
        )
        {
            try{
                using(_stateHelper.GetFuncWrapper())
                using (await LockSem.GetDisposable().ConfigureAwait(false))
                {
                    var indices = await TransferObservableCollection.IndexesOfAsync(
                        _ => _.RequestGuid == requestGuid).ConfigureAwait(false);
                    if(indices.Count == 0) 
                        return;
                    var transferInfo = await TransferObservableCollection.GetItemDeepCopyAtAsync(
                        indices.First()
                        ).ConfigureAwait(false);
                    if(
                        transferInfo.TransferStatus 
                        != WalletFormModelTransferStatus.PreparedToSend
                    )
                        return;
                    await TransferObservableCollection.RemoveRangeAsync(
                        indices.First(),
                        1
                        ).ConfigureAwait(false);
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,_logger);
            }
        }

        public async void SessionModel_OnPreparedToSendTransferAdded(
            PreparedToSendTransfer preparedTransfer
        )
        {
            try{
                using(_stateHelper.GetFuncWrapper())
                using (await LockSem.GetDisposable().ConfigureAwait(false))
                {
                    var indices = await TransferObservableCollection.IndexesOfAsync(
                        _ => _.RequestGuid == preparedTransfer.RequestGuid
                        ).ConfigureAwait(false);
                    if(indices.Count > 0)
                        return;
                    var newTransferInfo = WalletFormModelTransferInfo.FromPrepared(
                        preparedTransfer,
                        WalletProfileInstance.WalletCert.Id
                    );
                    await TransferObservableCollection.AddRangeAsync(
                        new[] {newTransferInfo}
                        ).ConfigureAwait(false);
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,_logger);
            }
        }

        public async void SessionModel_OnTransferReceived(
            List<ClientTransferBase> incomeTransfers
        )
        {
            try{
                using(_stateHelper.GetFuncWrapper())
                using (await LockSem.GetDisposable().ConfigureAwait(false))
                {
                    await TransferObservableCollection.RemoveWhereAsync(
                        _ => incomeTransfers.Any(__ => __.TransferGuid == _.TransferGuid)
                        ).ConfigureAwait(false);
                    await TransferObservableCollection.AddRangeAsync(
                        incomeTransfers.Select(_ =>
                            WalletFormModelTransferInfo
                                .FromIncomeTransfer(
                                    _,
                                    WalletProfileInstance.WalletCert.Id
                                )
                            ).ToList()
                        ).ConfigureAwait(false);
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,_logger);
            }
        }
        public async void OnTransferReceivedToTransferHistory(
            IList<List<ClientTransferBase>> x,
            IWalletFormModel proxy
        )
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    foreach (
                        List<ClientTransferBase> incomeTransfers in x
                    )
                    {
                        await WalletTransferHistoryInstance.AddTransfers(
                            incomeTransfers
                        );
                    }
                    MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                        proxy,
                        e => e.WalletTransferHistoryInstance
                    );
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exc)
            {
                _logger.Error(
                    "OnTransferReceivedToTransferHistory" +
                        " unexpected error '{0}'",
                    exc.ToString()
                );
            }
        }
        public async void OnTransferSentToTransferHistory(
            IList<List<ClientTransferBase>> x,
            IWalletFormModel proxy
        )
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    foreach (
                        List<ClientTransferBase> outcomeTransfers in x
                    )
                    {
                        await WalletTransferHistoryInstance.AddTransfers(
                            outcomeTransfers
                        );
                    }
                    MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                        proxy,
                        e => e.WalletTransferHistoryInstance
                    );
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exc)
            {
                _logger.Error(
                    "OnTransferSentToTransferHistory" +
                        " unexpected error '{0}'",
                    exc.ToString()
                );
            }
        }

        public async Task MyDisposeAsync()
        {
            await _stateHelper.MyDisposeAsync();
            foreach (IDisposable subscription in Subscriptions)
            {
                subscription.Dispose();
            }
            Subscriptions.Clear();
            await WalletSession.MyDisposeAsync();
            await WalletTransferHistoryInstance.MyDisposeAsync();
        }
        /**/
        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
    }
    
    public interface IWalletListFormModel : IMyNotifyPropertyChanged, ILockSemaphoreSlim
    {
        Dictionary<Guid, IWalletFormModel> WalletInfos { get; set; }
        Guid ActiveWalletGuid { get; set; }
        [JsonIgnore]
        Subject<IWalletFormModel> OnWalletInfoAdded { get; }
        [JsonIgnore]
        Subject<Guid> OnWalletInfoRemoved { get; }
        [JsonIgnore]
        Subject<WalletFormModel> OnWalletInfoChanged { get; }
    }

    public class WalletListFormModel : IWalletListFormModel
    {
        public WalletListFormModel()
        {
            LockSem = new SemaphoreSlim(1);
            WalletInfos = new Dictionary<Guid, IWalletFormModel>();
            ActiveWalletGuid = Guid.Empty;
            OnWalletInfoAdded = new Subject<IWalletFormModel>();
            OnWalletInfoRemoved = new Subject<Guid>();
            OnWalletInfoChanged = new Subject<WalletFormModel>();
        }
        /**/
        public SemaphoreSlim LockSem { get; private set; }
        public Dictionary<Guid, IWalletFormModel> WalletInfos { get; set; }
        public Guid ActiveWalletGuid { get; set; }
        public Subject<IWalletFormModel> OnWalletInfoAdded { get; private set; }
        public Subject<Guid> OnWalletInfoRemoved { get; private set; }
        public Subject<WalletFormModel> OnWalletInfoChanged { get; private set; }
        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
    }
}
