using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Gui.Models;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.ObjectStateLib;

namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers
{
    public interface ILocalRpcServersManagerModel : IMyNotifyPropertyChanged
    {
        bool WalletRpcServerRunning { get; set; }
        bool ProxyRpcServerRunning { get; set; }
    }

    public class LocalRpcServersManagerModel : ILocalRpcServersManagerModel
    {
        public LocalRpcServersManagerModel()
        {
            WalletRpcServerRunning = false;
            ProxyRpcServerRunning = false;
        }

        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject {
            get { 
                throw new Exception(
                    MyNotifyPropertyChangedArgs
                        .DefaultNotProxyExceptionString
                );
            }
        }

        public bool WalletRpcServerRunning { get; set; }
        public bool ProxyRpcServerRunning { get; set; }
    }

    public class LocalRpcServersManagerLocStrings
    {
        public string SAlreadyRunning = "Already running";
        public string SNotRunning = "Is not running";
    }

    public partial class LocalRpcServersManager : IMyAsyncDisposable
    {
        public static LocalRpcServersManagerLocStrings LocStrings 
            = new LocalRpcServersManagerLocStrings();

        private LocalRpcServersManager()
        {
        }

        private ILocalRpcServersManagerModel _model;
        private ICommonRpcSettings _settings;
        private IWalletListFormModel _walletListFormModel;
        private IProxyModel _proxyModel;
        public async static Task<LocalRpcServersManager> CreateInstance(
            ICommonRpcSettings settings,
            ILocalRpcServersManagerModel model,
            IWalletListFormModel walletListFormModel,
            IProxyModel proxyModel
        )
        {
            if(settings == null)
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => settings));
            if(model == null)
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => model));
            if(walletListFormModel == null)
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => walletListFormModel));
            if(proxyModel == null)
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => proxyModel));
            var instance = new LocalRpcServersManager();
            instance._walletListFormModel = walletListFormModel;
            instance._settings = settings;
            instance._model = model;
            instance._proxyModel = proxyModel;
            instance._stateHelper.SetInitializedState();
            if (
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
                    .StartAutomaticallyWalletRpcServer
            )
                await instance.StartWalletRpcServer();
            if (
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
                    .StartAutomaticallyProxyRpcServer
            )
                await instance.StartProxyRpcServer();
            return instance;
        }
        private readonly CancellationTokenSource _cts 
            = new CancellationTokenSource();
        private readonly DisposableObjectStateHelper _stateHelper 
            = new DisposableObjectStateHelper("LocalRpcServersManager");
        public async Task MyDisposeAsync()
        {
            _cts.Cancel();
            await _stateHelper.MyDisposeAsync().ConfigureAwait(false);
            await StopWalletRpcServerInternal().ConfigureAwait(false);
            await StopProxyRpcServerInternal().ConfigureAwait(false);
            _cts.Dispose();
        }
    }
}
