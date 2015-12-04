using System;
using System.Reactive.Subjects;
using System.Threading;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Models
{
    public interface IProxyModel : IMyNotifyPropertyChanged, ILockSemaphoreSlim
    {
        IProxyServerSessionModel ProxySessionModel { get; set; }
        bool ProxyServerConnected { get; set; }
    }

    public class ProxyModel : IProxyModel
    {
        public ProxyModel()
        {
            LockSem = new SemaphoreSlim(1);
            ProxySessionModel = new ProxyServerSessionModel();
        }

        public SemaphoreSlim LockSem { get; private set; }
        public IProxyServerSessionModel ProxySessionModel { get; set; }
        public bool ProxyServerConnected { get; set; }
        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
    }
}
