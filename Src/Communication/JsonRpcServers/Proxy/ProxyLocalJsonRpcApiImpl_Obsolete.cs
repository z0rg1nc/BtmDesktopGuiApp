using System;
using System.Threading.Tasks;
using BtmI2p.BasicAuthHttpJsonRpc.Client;
using BtmI2p.ExternalAppsLocalApi;
using BtmI2p.GeneralClientInterfaces.WalletServer;
using BtmI2p.JsonRpcHelpers.Client;

namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Proxy
{
    public partial class ProxyLocalJsonRpcApiImpl
    {

        private async Task CheckProxyApiRequest(
            ProxyApiBaseRequest request,
            bool checkProxyConnected = true
        )
        {
            if (request == null)
            {
                throw RpcRethrowableException.Create(
                    EGeneralProxyLocalApiErrorCodes20151004.WrongArgs
                );
            }
            try
            {
                request.CheckMe();
            }
            catch (Exception exc)
            {
                throw RpcRethrowableException.Create(
                    EGeneralProxyLocalApiErrorCodes20151004.WrongArgs,
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
                    EGeneralProxyLocalApiErrorCodes20151004.RequestLifetimeExpired
                );
            if (checkProxyConnected && !_proxyModel.ProxyServerConnected)
                throw RpcRethrowableException.Create(
                    EGeneralProxyLocalApiErrorCodes20151004.ProxyNotConnected
                );
            if (_proxyModel.ProxySessionModel == null)
                throw RpcRethrowableException.Create(
                    EGeneralProxyLocalApiErrorCodes20151004.UnknownError
                );
        }

        public async Task<ResultOrError<bool>> IsProxyServerConnected(
            ProxyApiBaseRequest request)
        {
            return await ResultOrError.GetFromFunc(
                async () =>
                {
                    await CheckProxyApiRequest(request, false);
                    return _proxyModel.ProxyServerConnected;
                }
            ).ConfigureAwait(false);
        }

        public async Task<ResultOrError<decimal>> GetProxyServerBalance(
            ProxyApiBaseRequest request)
        {
            return await ResultOrError.GetFromFunc(
                async () =>
                {
                    await CheckProxyApiRequest(request);
                    return _proxyModel.ProxySessionModel.Balance;
                }
            ).ConfigureAwait(false);
        }

        public async Task<ResultOrError<BitmoneyInvoiceData>> GetInvoiceDataForReplenishment(
            GetInvoiceDataRequest request)
        {
            return await ResultOrError.GetFromFunc(
                async () =>
                {
                    await CheckProxyApiRequest(request);
                    return _proxyModel.ProxySessionModel.IssueInvoiceToFillup(request.Amount);
                }
            ).ConfigureAwait(false);
        }

        public async Task<ResultOrError<bool>> IsNewAppVersionAvailable(
            ProxyApiBaseRequest request)
        {
            return await ResultOrError.GetFromFunc(
                async () =>
                {
                    await CheckProxyApiRequest(request);
                    return _proxyModel.ProxySessionModel.NewVersionAvailable;
                }
            ).ConfigureAwait(false);
        }
    }
}
