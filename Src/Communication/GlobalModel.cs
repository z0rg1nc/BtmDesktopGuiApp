using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Communication
{
    public class GlobalModel
    {
        public IPrivateCommonSettings CommonPrivateSettings = null;
        public IPublicCommonSettings CommonPublicSettings = null;
        /**/
        public LocalRpcServersManager RpcServersManager = null;
        public ILocalRpcServersManagerModel RpcServersManagerModel 
            = MyNotifyPropertyChangedImpl.GetProxy(
                (ILocalRpcServersManagerModel) new LocalRpcServersManagerModel()
            );
    }
}
