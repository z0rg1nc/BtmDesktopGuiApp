using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Subjects;
using BtmI2p.BitMoneyClient.Lib.MessageServerSession;
using BtmI2p.GeneralClientInterfaces;
using BtmI2p.GeneralClientInterfaces.MessageServer;
using BtmI2p.LightCertificates.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Communication.Message
{
    public class ContactInfo
    {
        public Guid UserId = Guid.Empty;
        public string Alias = string.Empty;
        public MessageClientSettingsOnServerClientInfo SettingsOnServer 
            = new MessageClientSettingsOnServerClientInfo();
        public bool AmIAuthorizedWriteToUser = false;
        public bool IsUserAuthorizedWriteToMe = false;
        public int UnreadMessagesCount = 0;
        public bool IsFavorite = false;
    }

    public class MessageClientProfile : ICheckable
    {
        public MessageClientProfile()
        {
            SettingsPass = new byte[32];
            MiscFuncs.GetRandomBytes(SettingsPass);
            SettingsFileName 
                = $"{MiscFuncs.GenGuidWithFirstBytes(0)}.aes256";
        }
        public string ProfileName = string.Empty;
        public LightCertificate UserCert = null;
        public LightCertificate MasterUserCert = null;
        public string SettingsFileName;

        public string GetSettingsFilePath()
        {
            return Path.Combine(
                DefaultFolders.MessageSettingsFolder, 
                SettingsFileName
            );
        }

        public byte[] SettingsPass;
        public void CheckMe()
        {
            if(UserCert == null || MasterUserCert == null)
                throw new ArgumentNullException();
            try
            {
                UserCert.CheckMe();
                if (
                    !LightCertificateRestrictions.IsValid(
                        UserCert
                    )
                )
                    throw new ArgumentOutOfRangeException(
                        this.MyNameOfProperty(e => e.UserCert)
                    );
            }
            catch
            {
                throw new ArgumentOutOfRangeException(
                    this.MyNameOfProperty(e => e.UserCert)
                );
            }
            try
            {
                MasterUserCert.CheckMe();
                if (
                    !LightCertificateRestrictions.IsValid(
                        MasterUserCert
                    )
                )
                    throw new ArgumentOutOfRangeException(
                        this.MyNameOfProperty(e => e.MasterUserCert)
                    );
            }
            catch
            {
                throw new ArgumentOutOfRangeException(
                    this.MyNameOfProperty(e => e.MasterUserCert)
                );
            }
        }
    }
    
    public interface IMessageClientSettings : IMyNotifyPropertyChanged, ICheckable
    {
        List<ContactInfo> ContactInfoList { get; set; }
        List<PreparedToSendMessage> OfflineMessages { get; set; }
        bool SaveMessageHistory { get; set; }
        string MessageHistoryFileName { get; set; }
        string GetMessageHistoryFilePath();
        byte[] MessageHistoryPass { get; set; }
        /**/
        bool SendIAmOnlineMessages { get; set; }
        int MessageHistoryVersion { get; set; }
    }
    public class MessageClientSettings : IMessageClientSettings
    {
        public MessageClientSettings()
        {
            MessageHistoryPass = new byte[16];
            MiscFuncs.GetRandomBytes(MessageHistoryPass);
            MessageHistoryFileName 
                = $"{MiscFuncs.GenGuidWithFirstBytes(0)}.sdb";
            ContactInfoList = new List<ContactInfo>();
            OfflineMessages = new List<PreparedToSendMessage>();
            SaveMessageHistory = true;
            SendIAmOnlineMessages = true;
            MessageHistoryVersion = 0;
        }

        public const int CurrentMessageHistoryVersion = 2; //20150927
        public List<ContactInfo> ContactInfoList { get; set; }
        public List<PreparedToSendMessage> OfflineMessages { get; set; }
        public bool SaveMessageHistory { get; set; }
        public string MessageHistoryFileName { get; set; }

        public string GetMessageHistoryFilePath()
        {
            return Path.Combine(
                DefaultFolders.MessageHistoryFolder,
                MessageHistoryFileName
            );
        }

        public byte[] MessageHistoryPass { get; set; }
        /**/
        public bool SendIAmOnlineMessages { get; set; }
        public int MessageHistoryVersion { get; set; }
        /**/
        public void CheckMe()
        {
            if(ContactInfoList == null
                || OfflineMessages == null
                || MessageHistoryFileName == null
                || MessageHistoryPass == null
                )
                throw new ArgumentNullException();
            if(string.IsNullOrWhiteSpace(MessageHistoryFileName))
                throw new Exception(
                    this.MyNameOfProperty(e => e.MessageHistoryFileName)
                );
            if (MessageHistoryPass.Length == 0)
                throw new Exception(
                    this.MyNameOfProperty(e => e.MessageHistoryPass)
                );
        }
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
