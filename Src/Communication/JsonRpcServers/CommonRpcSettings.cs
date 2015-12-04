using System;
using System.Reactive.Subjects;
using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Proxy;
using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Wallet;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.Newtonsoft.Json;

namespace BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers
{
    public interface ICommonRpcSettings : IMyNotifyPropertyChanged
    {
        [JsonConverter(typeof(ConcreteTypeConverter<
            WalletRpcSettings, IWalletRpcSettings
        >))]
        IWalletRpcSettings WalletJsonRpcSettings { get; set; }
        [JsonConverter(typeof(ConcreteTypeConverter<
            ProxyRpcSettings, IProxyRpcSettings
        >))]
        IProxyRpcSettings ProxyJsonRpcSettings { get; set; }
    }

    public class CommonRpcSettings : ICommonRpcSettings
    {
        public CommonRpcSettings()
        {
            WalletJsonRpcSettings = MyNotifyPropertyChangedImpl.GetProxy(
                (IWalletRpcSettings) new WalletRpcSettings()
            );
            ProxyJsonRpcSettings = MyNotifyPropertyChangedImpl.GetProxy(
                (IProxyRpcSettings)new ProxyRpcSettings()
            );
        }

        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject {
            get
            {
                throw new Exception(
                    MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString
                );
            }
        }
        public IWalletRpcSettings WalletJsonRpcSettings { get; set; }
        public IProxyRpcSettings ProxyJsonRpcSettings { get; set; }
    }
}
