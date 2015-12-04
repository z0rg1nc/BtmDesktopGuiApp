using System;
using System.Linq;
using System.Threading.Tasks;
using BtmI2p.BasicAuthHttpJsonRpc.Client;
using BtmI2p.ExternalAppsLocalApi;
using BtmI2p.JsonRpcHelpers.Client;
using BtmI2p.MiscUtils;

namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Wallet
{
    public partial class WalletLocalJsonRpcApiImpl
    {
        private async Task CheckWalletApiRequestObsolete(
            WalletApiBaseRequest request,
            bool dontCheckWalletGuid = false
        )
        {
            if (request == null)
            {
                throw RpcRethrowableException.Create(
                    EGeneralWalletLocalApiErrorCodes20151004.WrongArgs
                );
            }
            try
            {
                request.CheckMe();
            }
            catch (Exception exc)
            {
                throw RpcRethrowableException.Create(
                    EGeneralWalletLocalApiErrorCodes20151004.WrongArgs,
                    exc.Message
                );
            }
            var nowTime = DateTime.UtcNow;
            var requestSentUtcTime = new DateTime(request.RequestSentUtcTimeTicks);
            if (
                requestSentUtcTime > (nowTime + _settings.RequestSentUtcTimeLimit)
                || requestSentUtcTime < (nowTime - _settings.RequestSentUtcTimeLimit)
            )
                throw RpcRethrowableException.Create(
                    EGeneralWalletLocalApiErrorCodes20151004.RequestLifetimeExpired
                );
            if (!dontCheckWalletGuid)
            {
                if (!_settings.RpcAllowedWalletGuids.Contains(request.BaseWalletGuid))
                    throw RpcRethrowableException.Create(
                        EGeneralWalletLocalApiErrorCodes20151004.WalletNotAllowed
                    );
                using (await _walletListFormModel.LockSem.GetDisposable().ConfigureAwait(false))
                {
                    if (!_walletListFormModel.WalletInfos.ContainsKey(request.BaseWalletGuid))
                    {
                        throw RpcRethrowableException.Create(
                            EGeneralWalletLocalApiErrorCodes20151004.WalletNotConnected
                        );
                    }
                }
            }
        }

        public async Task<ResultOrError<bool>> IsWalletConnected(WalletApiBaseRequest request)
        {
            return await ResultOrError.GetFromFunc(
                async () =>
                {
                    await CheckWalletApiRequestObsolete(request, true);
                    if (_walletListFormModel.WalletInfos.ContainsKey(request.BaseWalletGuid))
                        return true;
                    return false;
                }
            ).ConfigureAwait(false);
        }

        public async Task<ResultOrError<long>> GetBalance(
            WalletApiBaseRequest request
        )
        {
            return await ResultOrError.GetFromFunc(
                async () =>
                {
                    await CheckWalletApiRequestObsolete(request);
                    var walletInfo = _walletListFormModel.WalletInfos[request.BaseWalletGuid];
                    return walletInfo.Balance;
                }
            ).ConfigureAwait(false);
        }

        public async Task<ResultOrError<VoidResult>> SendTransfer(
            SendTransferRequest request
        )
        {
            var curMethodName = this.MyNameOfMethod(e => e.SendTransfer(null));
            var result = await ResultOrError.GetFromFunc(
                async () =>
                {
                    await CheckWalletApiRequestObsolete(request);
                    if (!_settings.DontCheckHmac)
                    {
                        var rightCode = SendTransferRequest.GetHmacAuthCode(
                            request,
                            _settings.RpcSendingPaymentsHmacKeycode
                        );
                        if (
                            !rightCode.SequenceEqual(request.HmacAuthCode))
                        {
                            throw RpcRethrowableException.Create(
                                EGeneralWalletLocalApiErrorCodes20151004.WrongAuthCode
                            );
                        }
                    }
                    var walletInfo = _walletListFormModel.WalletInfos[request.BaseWalletGuid];
                    await walletInfo.WalletSession.SendTransfer(
                        null,
                        request.WalletToGuid,
                        request.Amount,
                        request.CommentBytes,
                        true,
                        request.ForceAnonymousTransfer,
                        request.RequestGuid,
                        true
                    ).ConfigureAwait(false);
                    return new VoidResult();
                }
            ).ConfigureAwait(false);
            //_log.Trace("{0} result {1}", curMethodName, result.WriteObjectToJson());
            return result;
        }
    }
}
