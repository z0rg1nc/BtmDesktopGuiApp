using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;
using BtmI2p.GeneralClientInterfaces;
using BtmI2p.LightCertificates.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Communication.Wallet
{
    public class WalletProfile : ICheckable
    {
        public WalletProfile()
        {
            SettingsPass = new byte[32];
            MiscFuncs.GetRandomBytes(SettingsPass);
            SettingsFileName = 
                string.Format(
                    "{0}.aes256",
                    Guid.NewGuid()
                );
        }

        public string ProfileName = string.Empty;
        public LightCertificate WalletCert = null;
        public LightCertificate MasterWalletCert = null;
        public string SettingsFileName;
        public string GetSettingsFilePath()
        {
            return Path.Combine(
                DefaultFolders.WalletSettingsFolder,
                SettingsFileName
            );
        }

        public byte[] SettingsPass;
        public void CheckMe()
        {
            if (
                WalletCert == null
                || MasterWalletCert == null
                || ProfileName == null
            )
                throw new ArgumentNullException();
            WalletCert.CheckMe();
            if (
                !LightCertificateRestrictions.IsValid(
                    WalletCert
                )
            )
                throw new ArgumentOutOfRangeException(
                    this.MyNameOfProperty(e => e.WalletCert)
                );
            MasterWalletCert.CheckMe();
            if (
                !LightCertificateRestrictions.IsValid(
                    MasterWalletCert
                )
            )
                throw new ArgumentOutOfRangeException(
                    this.MyNameOfProperty(e => e.MasterWalletCert)
                );
        }
    }
    
    public interface IWalletSettings : IMyNotifyPropertyChanged, ICheckable
    {
        bool SaveTransferHistory { get; set; }
        string TransferHistoryFilename { get; set; }
        string GetTransferHistoryFilePath();
        byte[] TransferHistoryPass { get; set; }
        int TransferHistoryVersion { get; set; }
    }

    public class WalletSettings : IWalletSettings
    {
        public WalletSettings()
        {
            TransferHistoryPass = new byte[16];
            MiscFuncs.GetRandomBytes(TransferHistoryPass);
            TransferHistoryFilename =
                string.Format(
                    "{0}.sdb",
                    Guid.NewGuid()
                );
            SaveTransferHistory = true;
            TransferHistoryVersion = 0;
        }
        public bool SaveTransferHistory { get; set; }
        public string TransferHistoryFilename { get; set; }

        public string GetTransferHistoryFilePath()
        {
            return Path.Combine(
                DefaultFolders.WalletTransferHistoryFolder,
                TransferHistoryFilename
            );
        }

        public byte[] TransferHistoryPass { get; set; }
        public int TransferHistoryVersion { get; set; }
        public const int CurrentTransferHistoryVersion = 20151002;
        /**/
        public void CheckMe()
        {
        }
        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
    }

    public interface IExternalPaymentProcessorSettings : 
        IMyNotifyPropertyChanged, 
        ICheckable, 
        ILockSemaphoreSlim
    {
        string ExternalProcessorAppPath { get; set; }
        string CommandLineArguments { get; set; }
        /**/
        bool ProcessSentTransfers { get; set; }
        bool ProcessReceivedTransfers { get; set; }
        bool ProcessSendTransferFaults { get; set; }
    }

    public class ExternalPaymentProcessorSettings : IExternalPaymentProcessorSettings
    {
        public ExternalPaymentProcessorSettings()
        {
            LockSem = new SemaphoreSlim(1);
            UserExternalProcessor = false;
            ExternalProcessorAppPath = string.Empty;
            CommandLineArguments = string.Empty;
            ProcessSentTransfers = true;
            ProcessReceivedTransfers = true;
            ProcessSendTransferFaults = true;
        }
        public SemaphoreSlim LockSem { get; private set; }
        /**/
        public bool UserExternalProcessor { get; set; }
        public string ExternalProcessorAppPath { get; set; }
        public string CommandLineArguments { get; set; }
        /**/
        public bool ProcessSentTransfers { get; set; }
        public bool ProcessReceivedTransfers { get; set; }
        public bool ProcessSendTransferFaults { get; set; }
        /**/
        public void CheckMe()
        {
            
        }
        public Subject<MyNotifyPropertyChangedArgs> PropertyChangedSubject
        {
            get
            {
                throw new Exception(MyNotifyPropertyChangedArgs.DefaultNotProxyExceptionString);
            }
        }
    }
}
