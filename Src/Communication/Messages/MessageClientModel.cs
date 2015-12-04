using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using BtmI2p.BitMoneyClient.Gui.Communication.Messages;
using BtmI2p.BitMoneyClient.Lib.MessageServerSession;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Communication.Message
{
    public enum MessageSesionConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Disconnecting
    }
    
    public interface IMessageClientModel : IMyNotifyPropertyChanged, ILockSemaphoreSlim
    {
        /**/
        MessageClientProfile Profile { get; set; }
        IMessageClientSettings Settings { get; set; }
        IMessageServerSessionModel SessionModel { get; set; }
        MessageHistory MessageHistoryInstance { get; set; }
        /**/
        MessageSesionConnectionStatus ConnectionStatus { get; set; }
        DateTime ConnectedTime { get; set; }
        /**/
        Guid UserId { get; set; }
        string ProfileName { get; set; }
        /**/
        ContactInfo ActiveContactInfo { get; set; }
    }
    public class MessageClientModel : IMessageClientModel
    {
        public MessageClientModel()
        {
            LockSem = new SemaphoreSlim(1);
            SessionModel = MyNotifyPropertyChangedImpl.GetProxy(
                (IMessageServerSessionModel)new MessageServerSessionModel()
                );
            Profile = null;
            Settings = MyNotifyPropertyChangedImpl.GetProxy(
                (IMessageClientSettings)new MessageClientSettings()
                );
            MessageHistoryInstance = MessageHistory.CreateInstance();
            ConnectionStatus = MessageSesionConnectionStatus.Disconnected;
            ConnectedTime = DateTime.MinValue;
            UserId = Guid.Empty;
            ProfileName = "Profile";
            ContactInfos = new List<ContactInfo>();
            ActiveContactInfo = null;
        }

        public SemaphoreSlim LockSem { get; private set; }
        public IMessageServerSessionModel SessionModel { get; set; }
        public MessageHistory MessageHistoryInstance { get; set; }
        /**/
        public MessageClientProfile Profile { get; set; }
        public IMessageClientSettings Settings { get; set; }
        /**/
        public MessageSesionConnectionStatus ConnectionStatus { get; set; }
        public DateTime ConnectedTime { get; set; }
        /**/
        public Guid UserId { get; set; }
        public string ProfileName { get; set; }
        public List<ContactInfo> ContactInfos { get; set; }
        /**/
        public ContactInfo ActiveContactInfo { get; set; }
        // UserGuid, LastReceivedMessageTime
        public Subject<Tuple<Guid, DateTime>> MessageReceivedUserId { get; private set; }
        /**/
        /**/
        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
    }
}
