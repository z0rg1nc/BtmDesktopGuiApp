using System;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Proxy;
using BtmI2p.BasicAuthHttpJsonRpc.Server;
using BtmI2p.ExternalAppsLocalApi;
using BtmI2p.MiscUtils;

namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers
{
    public partial class LocalRpcServersManager
    {
        private BasicAuthHttpJsonServerService<IProxyLocalJsonRpcApi>
            _proxyApiServer = null;
        private readonly SemaphoreSlim _proxyRpcServerLockSem = new SemaphoreSlim(1);

        public async Task StartProxyRpcServer()
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _proxyRpcServerLockSem.GetDisposable().ConfigureAwait(false))
                {
                    if(_model.ProxyRpcServerRunning)
                        throw new InvalidOperationException(LocStrings.SAlreadyRunning);
                    _proxyApiServer = BasicAuthHttpJsonServerService<
                        IProxyLocalJsonRpcApi
                        >.CreateInstance(
                            new Uri(
                                string.Format(
                                    "http://{0}:{1}/",
                                    _settings.ProxyJsonRpcSettings.HostName,
                                    _settings.ProxyJsonRpcSettings.PortNumber
                                    )
                                ),
                            new ProxyLocalJsonRpcApiImpl(
                                _settings.ProxyJsonRpcSettings,
                                _proxyModel
                                ),
                            _settings.ProxyJsonRpcSettings.Username,
                            _settings.ProxyJsonRpcSettings.Password
                        );
                    _model.ProxyRpcServerRunning = true;
                }
            }
        }
        private async Task StopProxyRpcServerInternal()
        {
            if (_model.ProxyRpcServerRunning)
            {
                await _proxyApiServer.MyDisposeAsync().ConfigureAwait(false);
                _proxyApiServer = null;
                _model.ProxyRpcServerRunning = false;
            }
        }

        public async Task StopProxyRpcServer()
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _proxyRpcServerLockSem.GetDisposable().ConfigureAwait(false))
                {
                    if (!_model.ProxyRpcServerRunning)
                        throw new InvalidOperationException(LocStrings.SNotRunning);
                    await StopProxyRpcServerInternal().ConfigureAwait(false);
                }
            }
        }
    }
}
