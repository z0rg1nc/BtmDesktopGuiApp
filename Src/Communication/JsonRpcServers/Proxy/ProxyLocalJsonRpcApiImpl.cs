using System;
using System.Globalization;
using System.Threading.Tasks;
using BtmI2p.BitMoneyClient.Gui.Models;
using BtmI2p.ExternalAppsLocalApi;
using BtmI2p.JsonRpcHelpers.Client;
using BtmI2p.MiscUtils;

namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Proxy
{
    public partial class ProxyLocalJsonRpcApiImpl : IProxyLocalJsonRpcApi
    {
        private readonly IProxyRpcSettings _settings;
        private readonly IProxyModel _proxyModel;
        public ProxyLocalJsonRpcApiImpl(
            IProxyRpcSettings settings,
            IProxyModel proxyModel
        )
        {
            _settings = settings;
            _proxyModel = proxyModel;
        }

        public async Task<bool> IsProxyServerConnected20151004()
        {
            try
            {
                return await Task.FromResult(
                    _proxyModel.ProxyServerConnected
                ).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralProxyLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<decimal> GetProxyServerBalance20151004()
        {
            try
            {
                return await Task.FromResult(
                    _proxyModel.ProxySessionModel.Balance
                ).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralProxyLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<GetInvoiceDataForReplenishmentResponse20151004> GetInvoiceDataForReplenishment20151004(long amount)
        {
            try
            {
                var args = new ProxyLocalJsonRpcApiArgsChecker20151004.GetInvoiceDataForReplenishment20151004Args
                {
                    Amount = amount
                };
                ProxyLocalJsonRpcApiArgsChecker20151004.CheckGetInvoiceDataForReplenishment20151004Args(
                    args
                );
                var res = _proxyModel.ProxySessionModel.IssueInvoiceToFillup(amount);
                return await Task.FromResult(new GetInvoiceDataForReplenishmentResponse20151004()
                {
                    WalletTo = res.WalletTo,
                    TransferAmount = res.TransferAmount,
                    CommentBytes = res.CommentBytes,
                    ForceAnonymousTransfer = res.ForceAnonymousTransfer
                }).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralProxyLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<bool> IsNewAppVersionAvailable20151004()
        {
            try
            {
                return await Task.FromResult(
                    _proxyModel.ProxySessionModel.NewVersionAvailable
                ).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralProxyLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<string> GetNowLocalTime20151004()
        {
            try
            {
                return await Task.FromResult(
                    DateTime.UtcNow.ToString(
                        ProxyLocalJsonRpcApiArgsChecker20151004.DateTimeStringFormat20151004,
                        CultureInfo.InvariantCulture
                    )
                ).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralProxyLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<string> GetNowServerTime20151004()
        {
            try
            {
                return await Task.FromResult(
                    (DateTime.UtcNow + _proxyModel.ProxySessionModel.ClientServerTimeDifference)
                        .ToString(
                            ProxyLocalJsonRpcApiArgsChecker20151004.DateTimeStringFormat20151004,
                            CultureInfo.InvariantCulture
                        )
                ).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralProxyLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }

        public async Task<int> GetServerLocalTimeDiffSeconds20151004()
        {
            try
            {
                return await Task.FromResult((int)_proxyModel.ProxySessionModel
                    .ClientServerTimeDifference.TotalSeconds).ConfigureAwait(false);
            }
            catch (EnumException<EGeneralProxyLocalApiErrorCodes20151004> enumExc)
            {
                throw JsonRpcException.Create(enumExc);
            }
        }
    }
}
