using System;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Wallet;
using BtmI2p.BasicAuthHttpJsonRpc.Server;
using BtmI2p.ExternalAppsLocalApi;
using BtmI2p.MiscUtils;

namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers
{
    public partial class LocalRpcServersManager
    {
        private BasicAuthHttpJsonServerService<IWalletLocalJsonRpcApi>
            _walletApiServer = null;
        private readonly SemaphoreSlim _walletRpcServerLockSem = new SemaphoreSlim(1);
        private WalletLocalJsonRpcApiImpl _apiImplInstance = null;
        public async Task StartWalletRpcServer()
        {
            //var curMethodName = this.MyNameOfMethod(e => e.StartWalletRpcServer());
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _walletRpcServerLockSem.GetDisposable().ConfigureAwait(false))
                {
                    if (_model.WalletRpcServerRunning)
                        throw new InvalidOperationException(LocStrings.SAlreadyRunning);
                    _apiImplInstance = new WalletLocalJsonRpcApiImpl(
                        _settings.WalletJsonRpcSettings,
                        _walletListFormModel
                        );
                    _walletApiServer =
                        BasicAuthHttpJsonServerService<
                            IWalletLocalJsonRpcApi
                        >.CreateInstance(
                            new Uri(
                                string.Format(
                                    "http://{0}:{1}/",
                                    _settings.WalletJsonRpcSettings.HostName,
                                    _settings.WalletJsonRpcSettings.PortNumber
                                )
                            ),
                            _apiImplInstance,
                            _settings.WalletJsonRpcSettings.Username,
                            _settings.WalletJsonRpcSettings.Password
                        );
                    _model.WalletRpcServerRunning = true;
                }
            }
        }

        private async Task StopWalletRpcServerInternal()
        {
            if (_model.WalletRpcServerRunning)
            {
                await _walletApiServer.MyDisposeAsync().ConfigureAwait(false);
                _walletApiServer = null;
                if (_apiImplInstance != null)
                {
                    await _apiImplInstance.MyDisposeAsync().ConfigureAwait(false);
                    _apiImplInstance = null;
                }
                _model.WalletRpcServerRunning = false;
            }
        }

        public async Task StopWalletRpcServer()
        {
            using (_stateHelper.GetFuncWrapper())
            {
                using (await _walletRpcServerLockSem.GetDisposable().ConfigureAwait(false))
                {
                    if (!_model.WalletRpcServerRunning)
                        throw new InvalidOperationException(LocStrings.SNotRunning);
                    await StopWalletRpcServerInternal().ConfigureAwait(false);
                }
            }
        }
    }
}
