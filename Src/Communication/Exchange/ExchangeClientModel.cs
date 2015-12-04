using System;
using System.Reactive.Subjects;
using System.Threading;
using BtmI2p.BitMoneyClient.Lib.ExchangeServerSession;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;

namespace BtmI2p.BitMoneyClient.Gui.Communication.Exchange
{
    public interface IExchangeClientModel : 
        IMyNotifyPropertyChanged, ILockSemaphoreSlim
    {
        /**/
        ExchangeServerSessionModel SessionModel { get; }
        ExchangeClientSettings Settings { get; set; }
        ExchangeClientProfile Profile { get; set; }
        string SettingsFilePath { get; set; }
        byte[] SettingsFilenamePassBytes { get; set; }
        /**/
        bool ExchangeServerConnected { get; set; }
        string ExchangeClientProfileName { get; set; }
        Guid ExchangeClientGuid { get; set; }
        /**/
        Guid SelectedAccountGuid { get; set; }
    }

    public class ExchangeClientModel : IExchangeClientModel
    {
        public static void Reset(IExchangeClientModel model)
        {
            model.SelectedAccountGuid = Guid.Empty;
            model.ExchangeServerConnected = false;
            model.ExchangeClientProfileName = "";
            model.ExchangeClientGuid = Guid.Empty;
            model.Settings = null;
            model.SettingsFilePath = string.Empty;
            model.SettingsFilenamePassBytes = null;
            model.Profile = null;
        }
        public ExchangeClientModel()
        {
            LockSem= new SemaphoreSlim(1);
            SessionModel = new ExchangeServerSessionModel();
            Reset(this);
        }

        public SemaphoreSlim LockSem { get; private set; }
        public ExchangeServerSessionModel SessionModel { get; private set; }
        /**/
        public ExchangeClientSettings Settings { get; set; }
        public ExchangeClientProfile Profile { get; set; }
        public string SettingsFilePath { get; set; }
        public byte[] SettingsFilenamePassBytes { get; set; }
        public bool ExchangeServerConnected { get; set; }
        public string ExchangeClientProfileName { get; set; }
        public Guid ExchangeClientGuid { get; set; }
        public Guid SelectedAccountGuid { get; set; }

        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(
                    MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString
                );
            }
        }
    }
}
