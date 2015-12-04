using System;
using System.IO;
using System.Reactive.Subjects;
using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.BitMoneyClient.Gui.Proxy;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.Newtonsoft.Json;

namespace BtmI2p.BitMoneyClient.Gui
{
    public static class DefaultFolders
    {
        public static string SettingsRoot => Path.Combine(".", "Settings");
        /**/
        public static string MiningProfilesFolder => Path.Combine(SettingsRoot, "MiningClientProfiles");
        public static string MiningSettingsFolder => Path.Combine(MiningProfilesFolder, "Settings");
        /**/
        public static string MessageProfilesFolder => Path.Combine(SettingsRoot, "MessageProfiles");

        public static string MessageSettingsFolder => Path.Combine(MessageProfilesFolder, "Settings");

        public static string MessageHistoryFolder => Path.Combine(MessageProfilesFolder, "MessageHistory");
        /**/
        public static string WalletProfilesFolder => Path.Combine(SettingsRoot, "WalletProfiles");

        public static string WalletSettingsFolder => Path.Combine(WalletProfilesFolder, "Settings");
        public static string WalletTransferHistoryFolder => Path.Combine(WalletProfilesFolder, "TransferHistory");
        /**/
        public static string LocalMiningManagerSettingsRoot => Path.Combine(SettingsRoot, "LocalMiningManager");

        public static string LocalMiningManagerSolutions => Path.Combine(LocalMiningManagerSettingsRoot, "Solutions");
        /**/
        public static string UpdaterFolder => Path.Combine(".", "Updater");

        public static string UpdaterArchivesFolder => Path.Combine(UpdaterFolder, "Archives");
        /**/
        public static string LanguagePacksFolder => Path.Combine(SettingsRoot, "LanguagePacks");
        /**/
        public static string ExchangeProfilesFolder => Path.Combine(SettingsRoot, "ExchangeClientProfiles");
        public static string ExchangeSettingsFolder => Path.Combine(ExchangeProfilesFolder, "Settings");
        /**/
        public static void CreateFoldersIfNotExist()
        {
            foreach (string folderName in new []
            {
                SettingsRoot,
                MiningProfilesFolder,
                MiningSettingsFolder,
                MessageProfilesFolder,
                MessageSettingsFolder,
                MessageHistoryFolder,
                WalletProfilesFolder,
                WalletSettingsFolder,
                WalletTransferHistoryFolder,
                LocalMiningManagerSettingsRoot,
                LocalMiningManagerSolutions,
                UpdaterFolder,
                UpdaterArchivesFolder,
                LanguagePacksFolder,
                ExchangeProfilesFolder,
                ExchangeSettingsFolder
            })
            {
                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);
            }
        }
    }
    public class BalanceRestrictionsSettings : ICheckable
    {
        public long ConnectExchangeServerMinBalance = 7;
        public long RegisterExchangeServerMinBalance = 5;
        /**/
        public long ConnectMiningServerMinBalance = 7;
        public long RegisterMiningServerMinBalance = 5;
        /**/
        public long ConnectUserServerMinBalance = 7;
        public long RegisterMessageServerMinBalance = 5;
        /**/
        public long ConnectWalletServerMinBalance = 7;
        public long RegisterWalletServerMinBalance = 5;
        /**/
        public long UpdateClientMinBalance = 7;
        /**/
        public void CheckMe()
        {
            if (ConnectMiningServerMinBalance <= 0
                || ConnectUserServerMinBalance <= 0
                || ConnectWalletServerMinBalance <= 0)
            {
                throw new Exception(
                    "Restriction connect min balance <= 0"
                );
            }
        }
    }

    public static class UpdaterSettings
    {
        public static string UpdaterExecutablePath => Path.Combine(DefaultFolders.UpdaterFolder, "BitMoneyUpdater.exe");

        public static string UpdaterExecutablePathOnMono => Path.Combine(DefaultFolders.UpdaterFolder, "run_on_mono.sh");
        public static string ExecuteOnExitPath => Path.Combine(".", "BitMoneyClient.exe");

        /*public static readonly string ExecuteOnExitPathOnMono
            = "";  execute in .sh instead of process start, old: Path.Combine(".", "run_on_mono.sh");
         */
    }
    
    public interface IPublicCommonSettings : ICheckable, IMyNotifyPropertyChanged
    {
        float FontSizePt { get; set; }
        string ApplicationInstanceName { get; set; }
        BalanceRestrictionsSettings BalanceRestrictions { get; set; }
        [JsonConverter(typeof(ConcreteTypeConverter<
            PublicProxySettings, IPublicProxySettings
        >))]
        IPublicProxySettings ProxySettings { get; set; }
        [JsonConverter(typeof(ConcreteTypeConverter<
            MiningTaskManagerSettings, IMiningTaskManagerSettings
        >))]
        IMiningTaskManagerSettings MiningManagerSettings { get; set; }
        string LocalizationLanguage { get; set; }
        /**/
        bool UseExternalPaymentProcessor { get; set; }
        bool StartAutomaticallyWalletRpcServer { get; set; }
        bool StartAutomaticallyProxyRpcServer { get; set; }
    }

    public class PublicCommonSettings : IPublicCommonSettings
    {
        public PublicCommonSettings()
        {
            ApplicationInstanceName = Guid.NewGuid().ToString().Substring(0,8);
            BalanceRestrictions = new BalanceRestrictionsSettings();
            ProxySettings = MyNotifyPropertyChangedImpl.GetProxy(
                (IPublicProxySettings)new PublicProxySettings()
            );
            MiningManagerSettings = MyNotifyPropertyChangedImpl.GetProxy(
                (IMiningTaskManagerSettings)(
                    new MiningTaskManagerSettings()
                    {
                        SolutionsFolder = DefaultFolders.LocalMiningManagerSolutions
                    }
                )
            );
            LocalizationLanguage = "default_en";
            FontSizePt = 9.0f;
            UseExternalPaymentProcessor = false;
            StartAutomaticallyWalletRpcServer = false;
            StartAutomaticallyProxyRpcServer = false;
        }

        public float FontSizePt { get; set; }
        public string ApplicationInstanceName { get; set; }
        public BalanceRestrictionsSettings BalanceRestrictions { get; set; }
        public IPublicProxySettings ProxySettings { get; set; }
        public IMiningTaskManagerSettings MiningManagerSettings { get; set; }
        public string LocalizationLanguage { get; set; }
        public bool UseExternalPaymentProcessor { get; set; }
        public bool StartAutomaticallyWalletRpcServer { get; set; }
        public bool StartAutomaticallyProxyRpcServer { get; set; }

        /**/
        public void CheckMe()
        {
            if (ProxySettings == null)
                throw new ArgumentNullException(
                    this.MyNameOfProperty(e => e.ProxySettings)
                );
            ProxySettings.CheckMe();
            if(BalanceRestrictions == null)
                throw new ArgumentNullException(
                    this.MyNameOfProperty(e => e.BalanceRestrictions)
                );
            BalanceRestrictions.CheckMe();
            if (string.IsNullOrWhiteSpace(LocalizationLanguage))
            {
                throw new ArgumentNullException(
                    this.MyNameOfProperty(e => e.LocalizationLanguage)
                );
            }
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
    
    public interface IPrivateCommonSettings :
        IMyNotifyPropertyChanged
        , ICheckable
    {
        [JsonConverter(typeof(ConcreteTypeConverter<
            PrivateProxySettings, IPrivateProxySettings
        >))]
        IPrivateProxySettings ProxySettings { get; set; }
        [JsonConverter(typeof(ConcreteTypeConverter<
            ExternalPaymentProcessorSettings, IExternalPaymentProcessorSettings
        >))]
        IExternalPaymentProcessorSettings ExternalTransferProcessorSettings { get; set; }
        [JsonConverter(typeof(ConcreteTypeConverter<
            CommonRpcSettings, ICommonRpcSettings
        >))]
        ICommonRpcSettings CommonJsonRpcSettings { get; set; }
    }

    public class PrivateCommonSettings : IPrivateCommonSettings
    {
        public PrivateCommonSettings()
        {
            ProxySettings = MyNotifyPropertyChangedImpl.GetProxy(
                (IPrivateProxySettings)new PrivateProxySettings()
            );
            ExternalTransferProcessorSettings = MyNotifyPropertyChangedImpl.GetProxy(
                (IExternalPaymentProcessorSettings)new ExternalPaymentProcessorSettings()
            );
            CommonJsonRpcSettings = MyNotifyPropertyChangedImpl.GetProxy(
                (ICommonRpcSettings)new CommonRpcSettings()
            );
        }
        /**/
        public IPrivateProxySettings ProxySettings { get; set; }
        public IExternalPaymentProcessorSettings ExternalTransferProcessorSettings { get; set; }
        public ICommonRpcSettings CommonJsonRpcSettings { get; set; }
        /**/
        public void CheckMe()
        {
            if (ProxySettings == null)
                throw new ArgumentNullException(
                    this.MyNameOfProperty(e => e.ProxySettings)
                );
            ProxySettings.CheckMe();
            if (ExternalTransferProcessorSettings == null)
                throw new ArgumentNullException(
                    this.MyNameOfProperty(e => e.ExternalTransferProcessorSettings)
                );
            ExternalTransferProcessorSettings.CheckMe();
            if(CommonJsonRpcSettings == null)
                throw new ArgumentNullException(
                    this.MyNameOfProperty(e => e.CommonJsonRpcSettings));
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
