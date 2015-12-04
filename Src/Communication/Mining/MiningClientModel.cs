using System;
using System.Reactive.Subjects;
using System.Threading;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Communication.Mining
{
    public interface IMiningClientModel : IMyNotifyPropertyChanged, ILockSemaphoreSlim
    {
        /**/
        MiningServerSessionModel SessionModel { get; set; }
        MiningClientSettings Settings { get; set; }
        MiningClientProfile Profile { get; set; }
        string SettingsFilePath { get; set; }
        byte[] SettingsFilenamePassBytes { get; set; }
        /**/
        bool MiningServerConnected { get; set; }
        long MiningAccountBalance { get; set; }
        string MiningClientProfileName { get; set; }
        Guid MiningClientGuid { get; set; }
    }

    public class MiningClientModel : IMiningClientModel
    {
        public MiningClientModel()
        {
            LockSem = new SemaphoreSlim(1);
            SessionModel = new MiningServerSessionModel();
            Settings = null;
            Profile = null;
            SettingsFilePath = string.Empty;
            SettingsFilenamePassBytes = null;
            MiningServerConnected = false;
            MiningAccountBalance = 0;
            MiningClientProfileName = string.Empty;
            MiningClientGuid = Guid.Empty;
        }
        /**/
        public SemaphoreSlim LockSem { get; private set; }
        public MiningServerSessionModel SessionModel { get; set; }
        public MiningClientSettings Settings { get; set; }
        public MiningClientProfile Profile { get; set; }
        public string SettingsFilePath { get; set; }
        public byte[] SettingsFilenamePassBytes { get; set; }
        /**/
        public bool MiningServerConnected { get; set; }
        public long MiningAccountBalance { get; set; }
        public string MiningClientProfileName { get; set; }
        public Guid MiningClientGuid { get; set; }
        /**/
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
