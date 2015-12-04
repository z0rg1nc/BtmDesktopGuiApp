using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.AesHelper;
using BtmI2p.BitMoneyClient.Lib.WalletServerSession;
using BtmI2p.ExternalAppsLocalApi;
using BtmI2p.GeneralClientInterfaces.WalletServer;
using BtmI2p.JsonRpcHelpers.Client;
using BtmI2p.MiscUtils;
using BtmI2p.ObjectStateLib;
using NLog;
using Xunit;

namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Wallet
{
    public partial class WalletLocalJsonRpcApiImpl : IWalletLocalJsonRpcApi, IMyAsyncDisposable
    {
        private readonly IWalletRpcSettings _settings;
        private readonly IWalletListFormModel _walletListFormModel;
        public WalletLocalJsonRpcApiImpl(
            IWalletRpcSettings settings,
            IWalletListFormModel walletListFormModel
        )
        {
            _walletListFormModel = walletListFormModel;
            _settings = settings;
            _stateHelper.SetInitializedState();
        }

        private async Task CheckWalletConnectedAndAllowed(Guid walletGuid)
        {
            if (!_settings.RpcAllowedWalletGuids.Contains(walletGuid))
                throw EnumException.Create(
                    EGeneralWalletLocalApiErrorCodes20151004.WalletNotAllowed
                );
            using (await _walletListFormModel.LockSem.GetDisposable().ConfigureAwait(false))
            {
                if (!_walletListFormModel.WalletInfos.ContainsKey(walletGuid))
                {
                    throw EnumException.Create(
                        EGeneralWalletLocalApiErrorCodes20151004.WalletNotConnected
                    );
                }
            }
        }

        public async Task<long> GetBalance20151004(Guid walletGuid)
        {
            try
            {
                await CheckWalletConnectedAndAllowed(walletGuid).ConfigureAwait(false);
                using (await _walletListFormModel.LockSem.GetDisposable().ConfigureAwait(false))
                {
                    var walletInfo = _walletListFormModel.WalletInfos[walletGuid];
                    return walletInfo.Balance;
                }
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<bool> IsWalletConnected20151004(Guid walletGuid)
        {
            try
            {
                using (await _walletListFormModel.LockSem.GetDisposable().ConfigureAwait(false))
                {
                    return _walletListFormModel.WalletInfos.ContainsKey(walletGuid);
                }
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<bool> IsWalletRpcAllowed20151004(Guid walletGuid)
        {
            try
            {
                return await Task.FromResult(
                    _settings.RpcAllowedWalletGuids.Contains(walletGuid)
                ).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<bool> HmacSignatureRpcRequired20151004()
        {
            try
            {
                return await Task.FromResult(!_settings.DontCheckHmac).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<Guid> SendTransfer20151004(
            Guid requestGuid,
            Guid walletFromGuid,
            Guid walletToGuid,
            long amount,
            string sentTimeString,
            bool anonymousTransfer,
            byte[] commentBytes,
            long maxFee = 0,
            byte[] hmacAuthCode = null
        )
        {
            try
            {
                await CheckWalletConnectedAndAllowed(walletFromGuid).ConfigureAwait(false);
                var args = new WalletLocalJsonRpcApiArgsChecker20151004.SendTransfer20151004Args()
                {
                    RequestGuid = requestGuid,
                    WalletFromGuid = walletFromGuid,
                    WalletToGuid = walletToGuid,
                    Amount = amount,
                    SentTimeString = sentTimeString,
                    AnonymousTransfer = anonymousTransfer,
                    CommentBytes = commentBytes,
                    MaxFee = maxFee,
                    HmacAuthCode = hmacAuthCode
                };
                WalletLocalJsonRpcApiArgsChecker20151004.CheckSentTransfer20151004Args(
                    args
                    );
                if (!_settings.DontCheckHmac)
                {
                    var rightCode = WalletLocalJsonRpcApiArgsChecker20151004.GetSentTransfer20151004ArgsHmacAuthCode(
                        args,
                        _settings.RpcSendingPaymentsHmacKeycode
                        );
                    if (
                        hmacAuthCode == null
                        || !rightCode.SequenceEqual(hmacAuthCode)
                        )
                    {
                        throw EnumException.Create(
                            EGeneralWalletLocalApiErrorCodes20151004.WrongAuthCode
                        );
                    }
                }
                var walletInfo = _walletListFormModel.WalletInfos[walletFromGuid];
                return await walletInfo.WalletSession.SendTransfer(
                    null,
                    walletToGuid,
                    amount,
                    commentBytes,
                    true,
                    anonymousTransfer,
                    requestGuid,
                    true,
                    maxFee
                ).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<long> EstimateFee20151004(
            Guid walletFrom,
            Guid walletTo, 
            long amount, 
            int commentBytesLength, 
            bool anonymousTransfer
        )
        {
            try
            {
                WalletLocalJsonRpcApiArgsChecker20151004.CheckEstimateFee20151004Args(
                    new WalletLocalJsonRpcApiArgsChecker20151004.EstimateFee20151004Args()
                    {
                        WalletFrom = walletFrom,
                        WalletTo = walletTo,
                        Amount = amount,
                        CommentBytesLength = commentBytesLength,
                        AnonymousTransfer = anonymousTransfer
                    }
                );
                var encryptedCommentLength = AesKeyIvPair.GenAesKeyIvPair().EncryptData(
                    new byte[commentBytesLength]
                ).Length;
                return await Task.FromResult(
                    WalletServerTransferFeeHelper.GetFeePos(
                        1,
                        amount,
                        encryptedCommentLength
                    )
                ).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }
        private readonly ConcurrentDictionary<Guid,bool> _walletsExistDict 
            = new ConcurrentDictionary<Guid, bool>();
        // Checkable wallet guid, base wallet guid
        private readonly ConcurrentDictionary<Guid,Guid> _walletToCheckExistance
            = new ConcurrentDictionary<Guid,Guid>();
        private readonly SemaphoreSlim _checkWalletExistBackGroundLockSem = new SemaphoreSlim(1);
        private async void CheckWalletExistBackGround()
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    var lockSemCalledWrapper = _checkWalletExistBackGroundLockSem.GetCalledWrapper();
                    lockSemCalledWrapper.Called = true;
                    using (await _checkWalletExistBackGroundLockSem.GetDisposable(true).ConfigureAwait(false))
                    {
                        while (!_cts.IsCancellationRequested && lockSemCalledWrapper.Called)
                        {
                            lockSemCalledWrapper.Called = false;
                            while (!_walletToCheckExistance.IsEmpty)
                            {
                                var firstKey = _walletToCheckExistance.Keys.First();
                                var firstKeyValue = _walletToCheckExistance[firstKey];
                                if (
                                    !_walletListFormModel.WalletInfos.ContainsKey(firstKeyValue)
                                    || _walletsExistDict.ContainsKey(firstKey)
                                    )
                                {
                                    _walletToCheckExistance.TryRemove(firstKey, out firstKeyValue);
                                    continue;
                                }
                                var walletSession = _walletListFormModel.WalletInfos[firstKeyValue].WalletSession;
                                try
                                {
                                    _walletsExistDict.TryAdd(
                                        firstKey,
                                        await walletSession.IsWalletRegistered(
                                            firstKey,
                                            _cts.Token
                                            ).ConfigureAwait(false)
                                        );
                                }
                                catch (OperationCanceledException)
                                {
                                    // If closing the walletSession, not rpc
                                    if (_cts.IsCancellationRequested)
                                        throw;
                                }
                                _walletToCheckExistance.TryRemove(firstKey, out firstKeyValue);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,Log);
            }
        }

        public async Task<bool> IsWalletRegistered20151004(
            Guid baseWalletGuid, 
            Guid walletGuid, 
            bool recheck = false
        )
        {
            try
            {
                await CheckWalletConnectedAndAllowed(baseWalletGuid).ConfigureAwait(false);
                bool t;
                if (recheck)
                {
                    _walletsExistDict.TryRemove(walletGuid, out t);
                }
                if (_walletsExistDict.TryGetValue(walletGuid, out t))
                    return t;
                _walletToCheckExistance.TryAdd(walletGuid, baseWalletGuid);
                CheckWalletExistBackGround();
                throw EnumException.Create(
                    EGeneralWalletLocalApiErrorCodes20151004.InQueueRetryLater
                );
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }
        private readonly ConcurrentDictionary<Guid, CheckSimpleTransferWasProcessedResponse> 
            _checkRequestProcessedCachedResponce = new ConcurrentDictionary<Guid, CheckSimpleTransferWasProcessedResponse>(); 
        private readonly ConcurrentDictionary<Guid,Guid> _checkRequestProcessedDict
            = new ConcurrentDictionary<Guid, Guid>();
        private readonly SemaphoreSlim _getSentTransferRequestStatusBackgroundLockSem
            = new SemaphoreSlim(1);
        private async void GetSentTransferRequestStatusBackground()
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    var lockSemCalledWrapper = _getSentTransferRequestStatusBackgroundLockSem.GetCalledWrapper();
                    lockSemCalledWrapper.Called = true;
                    using (await _getSentTransferRequestStatusBackgroundLockSem
                        .GetDisposable(true).ConfigureAwait(false))
                    {
                        while (!_cts.IsCancellationRequested && lockSemCalledWrapper.Called)
                        {
                            lockSemCalledWrapper.Called = false;
                            while (!_checkRequestProcessedDict.IsEmpty)
                            {
                                var firstKey = _checkRequestProcessedDict.Keys.First();
                                var firstKeyValue = _checkRequestProcessedDict[firstKey];
                                if (
                                    _checkRequestProcessedCachedResponce.ContainsKey(firstKey)
                                    || !_walletListFormModel.WalletInfos.ContainsKey(firstKeyValue)
                                )
                                {
                                    _checkRequestProcessedDict.TryRemove(firstKey, out firstKeyValue);
                                    continue;
                                }
                                var walletSession = _walletListFormModel.WalletInfos[firstKeyValue].WalletSession;
                                try
                                {
                                    _checkRequestProcessedCachedResponce.TryAdd(
                                        firstKey,
                                        await walletSession.WasRequestProcessed(
                                            firstKey,
                                            _cts.Token
                                        ).ConfigureAwait(false)
                                    );
                                    _checkRequestProcessedDict.TryRemove(
                                        firstKey,
                                        out firstKeyValue
                                    );
                                }
                                catch (OperationCanceledException)
                                {
                                    if (_cts.IsCancellationRequested)
                                        throw;
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc, Log);
            }
        }

        public async Task<GetSentTransferRequestStatus20151004Response> 
            GetSentTransferRequestStatus20151004(
                Guid baseWalletGuid, 
                Guid requestGuid, 
                bool recheck = false
            )
        {
            try
            {
                await CheckWalletConnectedAndAllowed(baseWalletGuid).ConfigureAwait(false);
                var walletInfo = _walletListFormModel.WalletInfos[baseWalletGuid];
                var existTransfers = await walletInfo.TransferObservableCollection.WhereAsync(
                    _ => 
                        _.WalletFrom == baseWalletGuid 
                        && _.RequestGuid == requestGuid
                ).ConfigureAwait(false);
                if (!existTransfers.Any())
                {
                    CheckSimpleTransferWasProcessedResponse t;
                    if (recheck)
                        _checkRequestProcessedCachedResponce.TryRemove(requestGuid, out t);
                    if (_checkRequestProcessedCachedResponce.ContainsKey(requestGuid))
                    {
                        t = _checkRequestProcessedCachedResponce[requestGuid];
                        return new GetSentTransferRequestStatus20151004Response()
                        {
                            Status = t.WasProcessed
                                ? ESentRequestStatus20151004.Sent
                                : ESentRequestStatus20151004.NotFound,
                            RelatedTransferGuidList = t.RelatedTransferGuidList
                        };
                    }
                    else
                    {
                        _checkRequestProcessedDict.TryAdd(
                            requestGuid,
                            baseWalletGuid
                        );
                        GetSentTransferRequestStatusBackground();
                        throw EnumException.Create(
                            EGeneralWalletLocalApiErrorCodes20151004.InQueueRetryLater);
                    }
                }
                else
                {
                    if (existTransfers.Any(_ => _.TransferStatus == WalletFormModelTransferStatus.PreparedToSend))
                    {
                        return new GetSentTransferRequestStatus20151004Response()
                        {
                            Status = ESentRequestStatus20151004.PreparedToSend
                        };
                    }
                    else if (existTransfers.Any(_ => _.TransferStatus == WalletFormModelTransferStatus.SendError))
                    {
                        var tr = existTransfers.First(_ => _.TransferStatus == WalletFormModelTransferStatus.SendError);
                        var errCode = tr.ErrCode == EPreparedToSendTransferFaultErrCodes.None
                            ? ESentRequestFaultErrCodes20151004.NoErrors
                            : tr.ErrCode == EPreparedToSendTransferFaultErrCodes.WalletToNotExist
                                ? ESentRequestFaultErrCodes20151004
                                    .WalletToNotExist
                                : tr.ErrCode == EPreparedToSendTransferFaultErrCodes.ServerException
                                    ? ESentRequestFaultErrCodes20151004
                                        .ServerException
                                    : ESentRequestFaultErrCodes20151004
                                        .UnknownError;
                        return new GetSentTransferRequestStatus20151004Response()
                        {
                            Status = ESentRequestStatus20151004.SendFault,
                            ErrCode = errCode,
                            ErrMessage = tr.ErrMessage,
                            ServerErrCode = tr.ServerErrCode,
                            GeneralServerErrCode = tr.GeneralServerErrCode
                        };
                    }
                    else
                    {
                        return new GetSentTransferRequestStatus20151004Response()
                        {
                            Status = ESentRequestStatus20151004.Sent,
                            RelatedTransferGuidList = existTransfers.Where(
                                _ => _.TransferStatus.In(
                                    WalletFormModelTransferStatus.Received,
                                    WalletFormModelTransferStatus.Sent
                                    )
                                ).Select(_ => _.TransferGuid).Distinct().ToList()
                        };
                    }
                }
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }
        /**/
        private readonly ConcurrentDictionary<Guid, TransferInfo20151004> 
            _cachedSentTransferInfoDict = new ConcurrentDictionary<Guid, TransferInfo20151004>();
        // Base wallet, list of transfer guids
        private readonly Dictionary<Guid,List<Guid>> _updateSentTransferGuids 
            = new Dictionary<Guid, List<Guid>>();
        private readonly SemaphoreSlim _updateSentTransferInfoBackgroundLockSem
            = new SemaphoreSlim(1);

        private TransferInfo20151004 FromClientTransferBase(ClientTransferBase trBase)
        {
            Assert.NotNull(trBase);
            return new TransferInfo20151004()
            {
                WalletTo = trBase.WalletTo,
                AnonymousTransfer = trBase.AnonymousTransfer,
                WalletFrom = trBase.WalletFrom,
                TransferGuid = trBase.TransferGuid,
                RequestGuid = trBase.RequestGuid,
                AuthenticatedOtherWalletCert = trBase.AuthenticatedOtherWalletCert,
                AuthenticatedCommentKey = trBase.AuthenticatedCommentKey,
                AuthenticatedTransferDetails = trBase.AuthenticatedTransferDetails,
                CommentBytes = trBase.CommentBytes,
                Amount = trBase.Amount,
                SentTimeString = trBase.SentTime.ToString(
                    WalletLocalJsonRpcApiArgsChecker20151004.DateTimeStringFormat20151004
                ),
                OutcomeTransfer = trBase.OutcomeTransfer,
                Fee = trBase.Fee,
                RelativeTransferNum = trBase.TransferNum
            };
        }

        private async void UpdateSentTransferInfoBackground()
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    var lockSemCalledWrapper 
                        = _updateSentTransferInfoBackgroundLockSem.GetCalledWrapper();
                    lockSemCalledWrapper.Called = true;
                    using (await _updateSentTransferInfoBackgroundLockSem.GetDisposable(true).ConfigureAwait(false))
                    {
                        while (!_cts.IsCancellationRequested && lockSemCalledWrapper.Called)
                        {
                            lockSemCalledWrapper.Called = false;
                            WalletServerSession baseWalletSession = null;
                            List<Guid> firstChunkToUpdate = null;
                            using (await _updateSentTransferGuids.GetLockSem().GetDisposable().ConfigureAwait(false))
                            {
                                foreach (var key in _updateSentTransferGuids.Keys)
                                {
                                    _updateSentTransferGuids[key].RemoveAll(
                                        _ => _cachedSentTransferInfoDict.ContainsKey(_)
                                    );
                                    _updateSentTransferGuids[key] 
                                        = _updateSentTransferGuids[key].Distinct().ToList();
                                    if(!_walletListFormModel.WalletInfos.ContainsKey(key))
                                        continue;
                                    if (_updateSentTransferGuids[key].Any())
                                    {
                                        baseWalletSession = _walletListFormModel.WalletInfos[key].WalletSession;
                                        firstChunkToUpdate = _updateSentTransferGuids[key].Take(20).ToList();
                                        lockSemCalledWrapper.Called = true;
                                    }
                                }
                            }
                            if (baseWalletSession != null)
                            {
                                while (firstChunkToUpdate.Any())
                                {
                                    List<ClientTransferBase> newInfos;
                                    try
                                    {
                                        newInfos = (await baseWalletSession.GetOutcomeTransfersChunk(
                                            new DateTime(2000, 1, 1),
                                            new DateTime(3000, 1, 1),
                                            Guid.Empty,
                                            _cts.Token,
                                            firstChunkToUpdate
                                        ).ConfigureAwait(false)).Item2;
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        if (_cts.IsCancellationRequested)
                                            throw;
                                        break; // WalletServerSession closed
                                    }
                                    if (!newInfos.Any())
                                    {
                                        foreach (var transferGuid in firstChunkToUpdate)
                                        {
                                            _cachedSentTransferInfoDict.TryAdd(transferGuid, null);
                                        }
                                        firstChunkToUpdate.Clear();
                                    }
                                    else
                                    {
                                        foreach (var info in newInfos)
                                        {
                                            _cachedSentTransferInfoDict.TryAdd(
                                                info.TransferGuid,
                                                FromClientTransferBase(info)
                                            );
                                        }
                                        firstChunkToUpdate = firstChunkToUpdate.Except(
                                            newInfos.Select(_ => _.TransferGuid).ToList()
                                        ).ToList();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc, Log);
            }
        }

        public async Task<TransferInfo20151004> GetSentTransferInfo20151004(
            Guid baseWalletGuid, 
            Guid trasferGuid, 
            bool recheck = false
        )
        {
            try
            {
                await CheckWalletConnectedAndAllowed(baseWalletGuid).ConfigureAwait(false);
                TransferInfo20151004 t;
                if (recheck)
                    _cachedSentTransferInfoDict.TryRemove(trasferGuid, out t);
                if (_cachedSentTransferInfoDict.TryGetValue(trasferGuid, out t))
                    return t;
                var firstExist = await _walletListFormModel.WalletInfos[baseWalletGuid]
                    .TransferObservableCollection.FirstOrDefaultDeepCopyAsync(
                        _ => _.TransferStatus == WalletFormModelTransferStatus.Sent
                             && _.TransferGuid == trasferGuid
                    ).ConfigureAwait(false);
                if (firstExist != null)
                {
                    var res = FromClientTransferBase(firstExist);
                    _cachedSentTransferInfoDict.TryAdd(trasferGuid, res);
                    return res;
                }
                using (await _updateSentTransferGuids.GetLockSem().GetDisposable().ConfigureAwait(false))
                {
                    if(!_updateSentTransferGuids.ContainsKey(baseWalletGuid))
                        _updateSentTransferGuids.Add(baseWalletGuid, new List<Guid>());
                    _updateSentTransferGuids[baseWalletGuid].Add(trasferGuid);
                }
                UpdateSentTransferInfoBackground();
                throw EnumException.Create(
                    EGeneralWalletLocalApiErrorCodes20151004.InQueueRetryLater);
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }
        /**/
        private readonly ConcurrentDictionary<Guid, TransferInfo20151004>
            _cachedReceivedTransferInfoDict = new ConcurrentDictionary<Guid, TransferInfo20151004>();
        // Base wallet, list of transfer guids
        private readonly Dictionary<Guid, List<Guid>> _updateReceivedTransferGuids
            = new Dictionary<Guid, List<Guid>>();
        private readonly SemaphoreSlim _updateReceivedTransferInfoBackgroundLockSem
            = new SemaphoreSlim(1);
        private async void UpdateReceivedTransferInfoBackground()
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    var lockSemCalledWrapper = _updateReceivedTransferInfoBackgroundLockSem.GetCalledWrapper();
                    lockSemCalledWrapper.Called = true;
                    using (await _updateReceivedTransferInfoBackgroundLockSem.GetDisposable(true).ConfigureAwait(false))
                    {
                        while (!_cts.IsCancellationRequested && lockSemCalledWrapper.Called)
                        {
                            lockSemCalledWrapper.Called = false;
                            WalletServerSession baseWalletSession = null;
                            List<Guid> firstChunkToUpdate = null;
                            using (await _updateReceivedTransferGuids.GetLockSem().GetDisposable().ConfigureAwait(false)
                                )
                            {
                                foreach (var key in _updateReceivedTransferGuids.Keys)
                                {
                                    _updateReceivedTransferGuids[key].RemoveAll(
                                        _ => _cachedReceivedTransferInfoDict.ContainsKey(_)
                                    );
                                    _updateReceivedTransferGuids[key]
                                        = _updateReceivedTransferGuids[key].Distinct().ToList();
                                    if(!_walletListFormModel.WalletInfos.ContainsKey(key))
                                        continue;
                                    if (_updateReceivedTransferGuids[key].Any())
                                    {
                                        baseWalletSession = _walletListFormModel.WalletInfos[key].WalletSession;
                                        firstChunkToUpdate = _updateReceivedTransferGuids[key].Take(20).ToList();
                                        lockSemCalledWrapper.Called = true;
                                    }
                                }
                            }
                            if (baseWalletSession != null)
                            {
                                while (firstChunkToUpdate.Any())
                                {
                                    List<ClientTransferBase> newInfos;
                                    try
                                    {
                                        newInfos = (await baseWalletSession.GetIncomeTransfersChunk(
                                            new DateTime(2000, 1, 1),
                                            new DateTime(3000, 1, 1),
                                            Guid.Empty,
                                            _cts.Token,
                                            firstChunkToUpdate
                                            ).ConfigureAwait(false)).Item2;
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        if (_cts.IsCancellationRequested)
                                            throw;
                                        break; // WalletServerSession closed
                                    }
                                    if (!newInfos.Any())
                                    {
                                        foreach (var transferGuid in firstChunkToUpdate)
                                        {
                                            _cachedReceivedTransferInfoDict.TryAdd(transferGuid, null);
                                        }
                                        firstChunkToUpdate.Clear();
                                    }
                                    else
                                    {
                                        foreach (var info in newInfos)
                                        {
                                            _cachedReceivedTransferInfoDict.TryAdd(
                                                info.TransferGuid,
                                                FromClientTransferBase(info)
                                            );
                                        }
                                        firstChunkToUpdate = firstChunkToUpdate.Except(
                                            newInfos.Select(_ => _.TransferGuid).ToList()
                                        ).ToList();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc, Log);
            }
        }

        public async Task<TransferInfo20151004> GetReceivedTransferInfo20151004(
            Guid baseWalletGuid, 
            Guid transferGuid, 
            bool recheck
        )
        {
            try
            {
                await CheckWalletConnectedAndAllowed(baseWalletGuid).ConfigureAwait(false);
                TransferInfo20151004 t;
                if (recheck)
                    _cachedReceivedTransferInfoDict.TryRemove(transferGuid, out t);
                if (_cachedReceivedTransferInfoDict.TryGetValue(transferGuid, out t))
                    return t;
                var firstExist = await _walletListFormModel.WalletInfos[baseWalletGuid].TransferObservableCollection
                    .FirstOrDefaultDeepCopyAsync(_ => _.TransferStatus == WalletFormModelTransferStatus.Received
                                                      && _.TransferGuid == transferGuid).ConfigureAwait(false);
                if (firstExist != null)
                {
                    var res = FromClientTransferBase(firstExist);
                    _cachedReceivedTransferInfoDict.TryAdd(transferGuid, res);
                    return res;
                }
                using (await _updateReceivedTransferGuids.GetLockSem().GetDisposable().ConfigureAwait(false))
                {
                    if(!_updateReceivedTransferGuids.ContainsKey(baseWalletGuid))
                        _updateReceivedTransferGuids.Add(baseWalletGuid,new List<Guid>());
                    _updateReceivedTransferGuids[baseWalletGuid].Add(transferGuid);
                }
                UpdateReceivedTransferInfoBackground();
                throw EnumException.Create(
                    EGeneralWalletLocalApiErrorCodes20151004.InQueueRetryLater);
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }
        /**/
        private int _dataCursorCounter = 0;
        // With .LockSem
        private class TransferDataCursorInternalInfo
        {
            public Guid BaseWalletGuid;
            public WalletLocalJsonRpcApiArgsChecker20151004.CreateTransferDataCursor20151004Args CursorParams;
            public ETransferDataCursorStatus20151004 Status 
                = ETransferDataCursorStatus20151004.Fetching;
            public readonly List<IDisposable> Subscriptions = new List<IDisposable>();
            public readonly List<TransferInfo20151004> TransferList 
                = new List<TransferInfo20151004>(); 
            public readonly SemaphoreSlim FetchSentTransferLockSem = new SemaphoreSlim(1);
            public Guid LastKnownSentTransferGuid = Guid.Empty;
            public bool SentFetchingComplete = false;
            public readonly SemaphoreSlim FetchReceivedTransferLockSem = new SemaphoreSlim(1);
            public Guid LastKnownReceivedTransferGuid = Guid.Empty;
            public bool ReceivedFetchingComplete = false;
            public void MarkComplete()
            {
                if (Status == ETransferDataCursorStatus20151004.Fetching)
                {
                    Status = ETransferDataCursorStatus20151004.Complete;
                    foreach (var subscription in Subscriptions)
                    {
                        subscription.Dispose();
                    }
                    Subscriptions.Clear();
                }
            }
        }
        private readonly ConcurrentDictionary<int, TransferDataCursorInternalInfo> _dataCursorInfos
            = new ConcurrentDictionary<int, TransferDataCursorInternalInfo>();

        private async void ProcessNewSentTransfersForDataCursor(
            int dataCursorNum
            )
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    TransferDataCursorInternalInfo t;
                    if(!_dataCursorInfos.TryGetValue(dataCursorNum,out t))
                        return;
                    WalletServerSession session;
                    DateTime dtFrom;
                    DateTime dtTo;
                    using (await t.GetLockSem().GetDisposable().ConfigureAwait(false))
                    {
                        if (t.Status != ETransferDataCursorStatus20151004.Fetching)
                            return;
                        if(t.SentFetchingComplete)
                            return;
                        if (!_walletListFormModel.WalletInfos.ContainsKey(t.BaseWalletGuid))
                        {
                            t.MarkComplete();
                            t.Status = ETransferDataCursorStatus20151004.WalletDisconnected;
                            return;
                        }
                        dtFrom = DateTime.ParseExact(
                            t.CursorParams.DateTimeFrom,
                            WalletLocalJsonRpcApiArgsChecker20151004.DateTimeStringFormat20151004,
                            CultureInfo.InvariantCulture
                        );
                        dtTo = DateTime.ParseExact(
                            t.CursorParams.DateTimeTo,
                            WalletLocalJsonRpcApiArgsChecker20151004.DateTimeStringFormat20151004,
                            CultureInfo.InvariantCulture
                        );
                        session = _walletListFormModel.WalletInfos[t.BaseWalletGuid].WalletSession;
                    }
                    var lockSemCalledWrapper = t.FetchSentTransferLockSem.GetCalledWrapper();
                    lockSemCalledWrapper.Called = true;
                    using (await t.FetchSentTransferLockSem.GetDisposable(true).ConfigureAwait(false))
                    {
                        while (!_cts.IsCancellationRequested && lockSemCalledWrapper.Called)
                        {
                            lockSemCalledWrapper.Called = false;
                            var lastKnownSentTransferGuid = await t.WithAsyncLockSem(
                                _ => _.LastKnownSentTransferGuid
                            ).ConfigureAwait(false);
                            Tuple<Guid, List<ClientTransferBase>> nextChunk;
                            try
                            {
                                nextChunk = await session.GetOutcomeTransfersChunk(
                                    dtFrom,
                                    dtTo,
                                    lastKnownSentTransferGuid,
                                    _cts.Token
                                ).ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                                if (_cts.IsCancellationRequested)
                                    throw;
                                await t.WithAsyncLockSem(
                                    _ =>
                                    {
                                        t.MarkComplete();
                                        t.Status = ETransferDataCursorStatus20151004.WalletDisconnected;
                                    }
                                ).ConfigureAwait(false);
                                return;
                            }
                            if (nextChunk.Item2.Count == 0)
                            {
                                if (!t.CursorParams.StayOnline && !lockSemCalledWrapper.Called)
                                {
                                    await t.WithAsyncLockSem(
                                        _ =>
                                        {
                                            t.SentFetchingComplete = true;
                                            if (t.ReceivedFetchingComplete)
                                                t.MarkComplete();
                                        }
                                    ).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                            {
                                lockSemCalledWrapper.Called = true;
                                await t.WithAsyncLockSem(
                                    _ =>
                                    {
                                        t.TransferList.AddRange(
                                            nextChunk.Item2.Select(FromClientTransferBase)
                                            );
                                        t.LastKnownSentTransferGuid = nextChunk.Item1;
                                    }
                                ).ConfigureAwait(false);
                            }
                        }
                    }
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
                MiscFuncs.HandleUnexpectedError(exc,Log);
            }
        }
        private async void ProcessNewReceivedTransfersForDataCursor(
            int dataCursorNum
        )
        {
            try
            {
                using (_stateHelper.GetFuncWrapper())
                {
                    TransferDataCursorInternalInfo t;
                    if (!_dataCursorInfos.TryGetValue(dataCursorNum, out t))
                        return;
                    WalletServerSession session;
                    DateTime dtFrom;
                    DateTime dtTo;
                    using (await t.GetLockSem().GetDisposable().ConfigureAwait(false))
                    {
                        if (t.Status != ETransferDataCursorStatus20151004.Fetching)
                            return;
                        if (t.ReceivedFetchingComplete)
                            return;
                        if (!_walletListFormModel.WalletInfos.ContainsKey(t.BaseWalletGuid))
                        {
                            t.MarkComplete();
                            t.Status = ETransferDataCursorStatus20151004.WalletDisconnected;
                            return;
                        }
                        dtFrom = DateTime.ParseExact(
                            t.CursorParams.DateTimeFrom,
                            WalletLocalJsonRpcApiArgsChecker20151004.DateTimeStringFormat20151004,
                            CultureInfo.InvariantCulture
                        );
                        dtTo = DateTime.ParseExact(
                            t.CursorParams.DateTimeTo,
                            WalletLocalJsonRpcApiArgsChecker20151004.DateTimeStringFormat20151004,
                            CultureInfo.InvariantCulture
                        );
                        session = _walletListFormModel.WalletInfos[t.BaseWalletGuid].WalletSession;
                    }
                    var lockSemCalledWrapper = t.FetchReceivedTransferLockSem.GetCalledWrapper();
                    lockSemCalledWrapper.Called = true;
                    using (await t.FetchReceivedTransferLockSem.GetDisposable(true).ConfigureAwait(false))
                    {
                        while (!_cts.IsCancellationRequested && lockSemCalledWrapper.Called)
                        {
                            lockSemCalledWrapper.Called = false;
                            var lastKnownReceivedTransferGuid = await t.WithAsyncLockSem(
                                _ => _.LastKnownReceivedTransferGuid
                            ).ConfigureAwait(false);
                            Tuple<Guid, List<ClientTransferBase>> nextChunk;
                            try
                            {
                                nextChunk = await session.GetIncomeTransfersChunk(
                                    dtFrom,
                                    dtTo,
                                    lastKnownReceivedTransferGuid,
                                    _cts.Token
                                ).ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                                if (_cts.IsCancellationRequested)
                                    throw;
                                await t.WithAsyncLockSem(
                                    _ =>
                                    {
                                        t.MarkComplete();
                                        t.Status = ETransferDataCursorStatus20151004.WalletDisconnected;
                                    }
                                ).ConfigureAwait(false);
                                return;
                            }
                            if (nextChunk.Item2.Count == 0)
                            {
                                if (!t.CursorParams.StayOnline && !lockSemCalledWrapper.Called)
                                {
                                    await t.WithAsyncLockSem(
                                        _ =>
                                        {
                                            t.ReceivedFetchingComplete = true;
                                            if (t.SentFetchingComplete)
                                                t.MarkComplete();
                                        }
                                    ).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                            {
                                lockSemCalledWrapper.Called = true;
                                await t.WithAsyncLockSem(
                                    _ =>
                                    {
                                        t.TransferList.AddRange(
                                            nextChunk.Item2.Select(FromClientTransferBase)
                                            );
                                        t.LastKnownReceivedTransferGuid = nextChunk.Item1;
                                    }
                                ).ConfigureAwait(false);
                            }
                        }
                    }
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
                MiscFuncs.HandleUnexpectedError(exc, Log);
            }
        }
        public async Task<int> CreateTransferDataCursor20151004(
            Guid baseWalletGuid, 
            string dateTimeFrom, 
            string dateTimeTo,
            bool fetchSentTransfers, 
            bool fetchReceivedTransfers,
            bool stayOnline
        )
        {
            try
            {
                await CheckWalletConnectedAndAllowed(baseWalletGuid).ConfigureAwait(false);
                var cursorParams = new WalletLocalJsonRpcApiArgsChecker20151004.CreateTransferDataCursor20151004Args()
                {
                    DateTimeFrom = dateTimeFrom,
                    DateTimeTo = dateTimeTo,
                    FetchSentTransfers = fetchSentTransfers,
                    FetchReceivedTransfers = fetchReceivedTransfers,
                    StayOnline = stayOnline
                };
                WalletLocalJsonRpcApiArgsChecker20151004.CheckCreateTransferDataCursor20151004Args(
                    cursorParams
                );
                var walletSession = _walletListFormModel.WalletInfos[baseWalletGuid].WalletSession;
                var nextCursorNum = Interlocked.Increment(ref _dataCursorCounter);
                var newCursorInfo = new TransferDataCursorInternalInfo()
                {
                    Status = ETransferDataCursorStatus20151004.Fetching,
                    BaseWalletGuid = baseWalletGuid,
                    CursorParams = cursorParams,
                    SentFetchingComplete = !fetchSentTransfers,
                    ReceivedFetchingComplete = !fetchReceivedTransfers
                };
                Assert.True(
                    _dataCursorInfos.TryAdd(
                        nextCursorNum,
                        newCursorInfo
                    )
                );
                var dtFrom = DateTime.ParseExact(
                    dateTimeFrom,
                    WalletLocalJsonRpcApiArgsChecker20151004.DateTimeStringFormat20151004,
                    CultureInfo.InvariantCulture
                );
                var dtTo = DateTime.ParseExact(
                    dateTimeTo,
                    WalletLocalJsonRpcApiArgsChecker20151004.DateTimeStringFormat20151004,
                    CultureInfo.InvariantCulture
                );
                var closeOffset = dtTo - DateTime.UtcNow;
                if (stayOnline && closeOffset > TimeSpan.Zero)
                {
                    newCursorInfo.Subscriptions.Add(
                        Observable.Timer(closeOffset)
                            .Subscribe(
                                async _ =>
                                {
                                    try
                                    {
                                        await newCursorInfo.WithAsyncLockSem(
                                            __ => __.MarkComplete()
                                        ).ConfigureAwait(false);
                                    }
                                    catch (Exception exc)
                                    {
                                        MiscFuncs.HandleUnexpectedError(exc,Log);
                                    }
                                }
                            )
                    );
                    if (fetchSentTransfers)
                    {
                        newCursorInfo.Subscriptions.Add(
                            walletSession.ServerEvents.Where(
                                _ =>
                                {
                                    var typedArgs = (MutableTuple<Guid, Guid, DateTime>) _.EventArgs;
                                    return
                                        _.EventType == EWalletClientEventTypes.NewSentTransfer
                                        && typedArgs.Item3 >= dtFrom
                                        && typedArgs.Item3 <= dtTo;
                                }
                                ).BufferNotEmpty(TimeSpan.FromSeconds(0.3))
                                .ObserveOn(TaskPoolScheduler.Default)
                                .Subscribe(_ => ProcessNewSentTransfersForDataCursor(nextCursorNum))
                            );
                    }
                    if (fetchReceivedTransfers)
                    {
                        newCursorInfo.Subscriptions.Add(
                            walletSession.ServerEvents.Where(
                                _ =>
                                {
                                    var typedArgs = (MutableTuple<Guid, Guid, DateTime>) _.EventArgs;
                                    return
                                        _.EventType == EWalletClientEventTypes.NewReceivedTransfer
                                        && typedArgs.Item3 >= dtFrom
                                        && typedArgs.Item3 <= dtTo;
                                }
                                ).BufferNotEmpty(TimeSpan.FromSeconds(0.3))
                                .ObserveOn(TaskPoolScheduler.Default)
                                .Subscribe(_ => ProcessNewReceivedTransfersForDataCursor(nextCursorNum))
                            );
                    }
                }
                if(fetchSentTransfers)
                    ProcessNewSentTransfersForDataCursor(nextCursorNum);
                if(fetchReceivedTransfers)
                    ProcessNewReceivedTransfersForDataCursor(nextCursorNum);
                return nextCursorNum;
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }
        public async Task<GetTransferDataCursorStatusResponse20151004> 
            GetTransferDataCursorStatus20151004(
                int dataCursorNum
            )
        {
            try
            {
                TransferDataCursorInternalInfo cursor;
                if (!_dataCursorInfos.TryGetValue(dataCursorNum, out cursor))
                    throw EnumException.Create(
                        EGeneralWalletLocalApiErrorCodes20151004.CursorNotFound);
                Assert.NotNull(cursor);
                using (await cursor.GetLockSem().GetDisposable().ConfigureAwait(false))
                {
                    return new GetTransferDataCursorStatusResponse20151004()
                    {
                        Status = cursor.Status,
                        BaseWalletGuid = cursor.BaseWalletGuid,
                        TotalTransferCount = cursor.TransferList.Count
                    };
                }
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<List<TransferInfo20151004>> FetchTransferFromDataCursor20151004(
            int dataCursorNum, 
            int offset, 
            int count
        )
        {
            try
            {
                TransferDataCursorInternalInfo cursor;
                if (!_dataCursorInfos.TryGetValue(dataCursorNum, out cursor))
                    throw EnumException.Create(
                        EGeneralWalletLocalApiErrorCodes20151004.CursorNotFound);
                Assert.NotNull(cursor);
                using (await cursor.GetLockSem().GetDisposable().ConfigureAwait(false))
                {
                    WalletLocalJsonRpcApiArgsChecker20151004.CheckFetchTransferFromDataCursor20151004Args(
                        new WalletLocalJsonRpcApiArgsChecker20151004.FetchTransferFromDataCursor20151004Args()
                        {
                            Offset = offset,
                            Count = count
                        }, 
                        cursor.TransferList.Count
                    );
                    return cursor.TransferList.Skip(offset).Take(count).ToList();
                }
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task CloseTransferDataCursor20151004(
            int dataCursorNum
        )
        {
            try
            {
                TransferDataCursorInternalInfo cursor;
                if (!_dataCursorInfos.TryGetValue(dataCursorNum, out cursor))
                    throw EnumException.Create(
                        EGeneralWalletLocalApiErrorCodes20151004.CursorNotFound);
                Assert.NotNull(cursor);
                cursor.MarkComplete();
                _dataCursorInfos.TryRemove(dataCursorNum, out cursor);
                await Task.Delay(0).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralWalletLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper("WalletLocalJsonRpcApiImpl");
        public async Task MyDisposeAsync()
        {
            _cts.Cancel();
            await _stateHelper.MyDisposeAsync().ConfigureAwait(false);
            _cts.Dispose();
        }
    }
}
