using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication;
using BtmI2p.BitMoneyClient.Gui.Communication.Exchange;
using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers;
using BtmI2p.BitMoneyClient.Gui.Communication.Message;
using BtmI2p.BitMoneyClient.Gui.Communication.Mining;
using BtmI2p.BitMoneyClient.Gui.Communication.Wallet;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm.JsonPrcServerSettings;
using BtmI2p.BitMoneyClient.Gui.Forms.MiningLocal;
using BtmI2p.BitMoneyClient.Gui.Forms.Models;
using BtmI2p.BitMoneyClient.Gui.Localization;
using BtmI2p.BitMoneyClient.Gui.Models;
using BtmI2p.AesHelper;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.BitMoneyClient.Lib.ExchangeServerSession;
using BtmI2p.BitMoneyClient.Lib.MessageServerSession;
using BtmI2p.CryptFile.Lib;
using BtmI2p.GeneralClientInterfaces;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyFileManager;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.ObjectStateLib;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm : Form
    {
        public ClientGuiMainForm()
        {
            InitializeComponent();
        }
        public static ClientGuiMainFormLocStrings LocStrings 
            = new ClientGuiMainFormLocStrings();
        private readonly SemaphoreSlim _publicCommonSettingsWriteLock 
            = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _privateCommonSettingsWriteLock
            = new SemaphoreSlim(1);
        /*
        public static void ShowInfoMessage(
            IWin32Window owner,
            string formatString,
            object[] args,
            string caption = null
        )
        {
            if (caption == null)
                caption = LocStrings.Messages.InformationMessageCaption;
            var message = string.Format(formatString, args);
            ShowInfoMessage(owner,
                message,
                caption
            );
        }
        */
        public static async void ShowInfoMessage(
            Form owner,
            string message, 
            string caption = null
        )
        {
	        try
	        {
		        if (caption == null)
			        caption = LocStrings.Messages.InformationMessageCaption;
		        await MessageBoxAsync.ShowAsync(
                    owner,
                    message,
			        caption,
			        MessageBoxButtons.OK,
			        MessageBoxIcon.Information
			    ).ConfigureAwait(false);
	        }
	        catch (Exception exc)
	        {
                MiscFuncs.HandleUnexpectedError(exc,_log);
	        }
        }

        private void ShowInfoMessage(
            string message,
            string caption = null
            )
        {
            try
            {
                ShowInfoMessage(
                    this,
                    message,
                    caption
                );
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc, _log);
            }
        }

        public static async void ShowErrorMessage(
            Form owner,
            string message,
            string caption = null
        )
        {
	        try
	        {
		        if (caption == null)
			        caption = LocStrings.Messages.ErrorMessageCaption;
	            await MessageBoxAsync.ShowAsync(
	                owner,
	                message,
	                caption,
	                MessageBoxButtons.OK,
	                MessageBoxIcon.Error
	            );
	        }
	        catch (Exception exc)
	        {
                MiscFuncs.HandleUnexpectedError(exc,_log);
	        }
        }
        private void ShowErrorMessage(
            string message, 
            string caption = null
        )
        {
	        try
	        {
		        if (caption == null)
			        caption = LocStrings.Messages.ErrorMessageCaption;
		        ShowErrorMessage(
                    this,
                    message,
                    caption
                );
	        }
	        catch (Exception exc)
	        {
                MiscFuncs.HandleUnexpectedError(exc,_log);
	        }
        }
        /**/
        public static readonly GlobalModel GlobalModelInstance
            = new GlobalModel();
        private readonly MainFormModel _formModel 
            = new MainFormModel();
        AesProtectedByteArray _privateCommonSettingsFilePassword;
        private async Task AsyncInit()
        {
            try
            {
                _log.Trace("===============START================");
                DefaultFolders.CreateFoldersIfNotExist();
                string publicCommonSettingsFileName
                    = Path.Combine(DefaultFolders.SettingsRoot, "PublicCommonSettings.json");
                if (!File.Exists(publicCommonSettingsFileName))
                {
                    File.Create(publicCommonSettingsFileName).Close();
                    GlobalModelInstance.CommonPublicSettings = 
                        MyNotifyPropertyChangedImpl.GetProxy(
                            (IPublicCommonSettings)
                            new PublicCommonSettings()
                        );
                }
                else
                {
                    try
                    {
                        GlobalModelInstance.CommonPublicSettings =
                            MyNotifyPropertyChangedImpl.GetProxy(
                                (IPublicCommonSettings)File.ReadAllText(
                                    publicCommonSettingsFileName,
                                    Encoding.UTF8
                                ).ParseJsonToType<PublicCommonSettings>()
                            );
                    }
                    catch(Exception exc)
                    {
                        MiscFuncs.HandleUnexpectedError(exc,_log);
                        await MessageBoxAsync.ShowAsync(
                            this,
                            LocStrings.Messages.ReadingPublicCommonSettingsFromFileError
                        );
                        _formClosingActionComplete = true;
                        Close();
                        return;
                    }
                }
                try
                {
                    GlobalModelInstance.CommonPublicSettings.CheckMe();
                }
                catch (Exception exc)
                {
                    MiscFuncs.HandleUnexpectedError(exc,_log);
                    await MessageBoxAsync.ShowAsync(
                            this,
                        LocStrings.Messages.PublicCommonSettingsCheckError.Inject(
                            new
                            {
                                ErrorMessage = exc.Message
                            }
                        )
                    );
                    _formClosingActionComplete = true;
                    Close();
                    return;
                }
                GlobalModelInstance.CommonPublicSettings.PropertyChangedSubject
                    .Throttle(TimeSpan.FromSeconds(5.0d)).Subscribe(
                        i => BeginInvokeProper(async () =>
                        {
                            using (await _publicCommonSettingsWriteLock.GetDisposable())
                            {
                                File.WriteAllText(
                                    publicCommonSettingsFileName,
                                    GlobalModelInstance.CommonPublicSettings.WriteObjectToJson(),
                                    Encoding.UTF8
                                );
                            }
                        },
                        useAnyContext: true
                    )
                );
                
                File.WriteAllText(
                    Path.Combine(DefaultFolders.LanguagePacksFolder,"default_en.json"),
                    new ClientGuiLocalizationData().WriteObjectToJson()
                );
                File.WriteAllText(
                    Path.Combine(DefaultFolders.LanguagePacksFolder,"empty.json"),
                    LocalizationBaseFuncs.GetStringEmptyInitObj(
                        typeof(ClientGuiLocalizationData)
                    ).WriteObjectToJson()
                );
                File.WriteAllText(
                    Path.Combine(DefaultFolders.LanguagePacksFolder,"numbered_en.json"),
                    LocalizationBaseFuncs
                        .GetNumberedNamesLocalization(
                            new ClientGuiLocalizationData()
                        ).WriteObjectToJson()
                );
                var languagePackFileName = Path.Combine(
                    DefaultFolders.LanguagePacksFolder,
                    string.Format(
                        "{0}.json",
                        GlobalModelInstance.CommonPublicSettings.LocalizationLanguage
                    )
                );
                try
                {
                    var languagePack =
                        File.ReadAllText(languagePackFileName, Encoding.UTF8)
                            .ParseJsonToType<ClientGuiLocalizationData>();
                    LocalizationBaseFuncs.CheckFieldsAndStringsNotNull(languagePack);
                    languagePack.Init();
                }
                catch (Exception exc)
                {
                    MiscFuncs.HandleUnexpectedError(exc,_log);
                    await MessageBoxAsync.ShowAsync(
                            this,
                        LocStrings.Messages.LoadingLanguagePackError.Inject(
                            new
                            {
                                ErrorMessage = exc.Message
                            }
                        )
                    );
                    _formClosingActionComplete = true;
                    Close();
                    return;
                }
                /**/
                InitCommonView();
                this.Text = ClientLifecycleEnvironment.TitlesPrefix + LocStrings.Text.Inject(
                    new
                    {
                        AppId = GlobalModelInstance
                            .CommonPublicSettings
                            .ApplicationInstanceName,
                        AppVersion = CommonClientConstants.CurrentClientVersionString
                    }
                );
                /**/
                await InitExchangeSubscriptions();
                InitMiningSubscriptions();
                InitMiningManagerSubscription();
                InitProxySubscriptions();
                InitMessageServerSubscriptions();
                await InitWalletSubscriptions();
                /**/
                string privateCommonSettingsFileName
                    = Path.Combine(
                        DefaultFolders.SettingsRoot, 
                        "PrivateCommonSettings.aes256"
                    );
                if (File.Exists(privateCommonSettingsFileName))
                {
                    while (true)
                    {
                        _privateCommonSettingsFilePassword =
                            await EnterPasswordForm.CreateAndReturnResult(
                                LocStrings.Messages.PrivateCommonSettingsPasswordRequestCaption,
                                this
                            );
                        if (
                            _privateCommonSettingsFilePassword
                                == null
                        )
                        {
                            if (
                                await MessageBoxAsync.ShowAsync(this,
                                    LocStrings.CommonMessages.EmptyPasswordError,
                                    LocStrings.Messages.ErrorMessageCaption,
                                    MessageBoxButtons.RetryCancel,
                                    MessageBoxIcon.Error
                                )
                                == DialogResult.Cancel
                            )
                            {
                                _formClosingActionComplete = true;
                                Close();
                                return;
                            }
                            continue;
                        }
                        break;
                    }
                    ScryptPassEncryptedData scryptPassEncryptedData;
                    try
                    {
                        scryptPassEncryptedData =
                            File.ReadAllText(
                                privateCommonSettingsFileName,
                                Encoding.UTF8
                            ).ParseJsonToType<ScryptPassEncryptedData>();
                    }
                    catch (Exception exc)
                    {
                        MiscFuncs.HandleUnexpectedError(exc,_log);
                        await MessageBoxAsync.ShowAsync(
                            this,
                            LocStrings.Messages.PrivateSettingsFileParseError.Inject(
                                new
                                {
                                    ErrorMessage = exc.Message
                                }
                            )
                        );
                        _formClosingActionComplete = true;
                        Close();
                        return;
                    }
                    try
                    {
                        try
                        {
                            using (
                                var tempPass
                                    = _privateCommonSettingsFilePassword.TempData
                                )
                            {
                                GlobalModelInstance.CommonPrivateSettings =
                                    MyNotifyPropertyChangedImpl.GetProxy(
                                        (IPrivateCommonSettings)
                                            scryptPassEncryptedData.GetValue<PrivateCommonSettings>(
                                                tempPass.Data
                                                )
                                        );
                            }
                        }
                        catch (EnumException<ScryptPassEncryptedData.EGetValueT1ErrCodes> enumExc)
                        {
                            MiscFuncs.HandleUnexpectedError(enumExc, _log);
                            if (enumExc.ExceptionCode == ScryptPassEncryptedData.EGetValueT1ErrCodes.WrongPassword)
                            {
                                await MessageBoxAsync.ShowAsync(
                                    this,
                                    LocStrings.CommonMessages.WrongPasswordError
                                );
                                _formClosingActionComplete = true;
                                Close();
                                return;
                            }
                            throw;
                        }
                    }
                    catch (Exception exc)
                    {
                        MiscFuncs.HandleUnexpectedError(exc,_log);
                        await MessageBoxAsync.ShowAsync(
                            this,
                            LocStrings.Messages.PrivateSettingsDecryptError.Inject(
                                new
                                {
                                    ErrorMessage = exc.Message
                                }
                            )
                        );
                        _formClosingActionComplete = true;
                        Close();
                        return;
                    }
                }
                else
                {
                    ShowInfoMessage(
                        LocStrings.Messages.PrivateSettingsFileNotFoundNewCreatedInfo
                    );
                    GlobalModelInstance.CommonPrivateSettings = 
                        MyNotifyPropertyChangedImpl.GetProxy(
                            (IPrivateCommonSettings)new PrivateCommonSettings()
                        );
                    while (true)
                    {
                        _privateCommonSettingsFilePassword =
                            await EnterPasswordForm.CreateAndReturnResult(
                                LocStrings.Messages.NewPrivateSettingsFilePasswordRequestCaption,
                                this
                            );
                        if (
                            _privateCommonSettingsFilePassword
                                == null
                        )
                        {
                            if (
                                await MessageBoxAsync.ShowAsync(this,
                                    LocStrings.Messages.NewPrivateSettingsFilePasswordIsEmptyError,
                                    LocStrings.Messages.ErrorMessageCaption,
                                    MessageBoxButtons.RetryCancel,
                                    MessageBoxIcon.Error
                                )
                                == DialogResult.Cancel
                            )
                            {
                                _formClosingActionComplete = true;
                                Close();
                                return;
                            }
                            continue;
                        }
                        break;
                    }
                }
                try
                {
                    GlobalModelInstance.CommonPrivateSettings.CheckMe();
                }
                catch (Exception exc)
                {
                    MiscFuncs.HandleUnexpectedError(exc,_log);
                    await MessageBoxAsync.ShowAsync(
                            this,
                        LocStrings.Messages.PrivateCommonSettingsCheckError.Inject(
                            new
                            {
                                ErrorMessage = exc.Message
                            }
                        )
                    );
                    _formClosingActionComplete = true;
                    Close();
                    return;
                }
                GlobalModelInstance.CommonPrivateSettings.PropertyChangedSubject
                    .Throttle(TimeSpan.FromSeconds(5.0d)).Subscribe(
                        i => BeginInvokeProper(
                            async () =>
                            {
                                using (await _privateCommonSettingsWriteLock.GetDisposable())
                                {
                                    using (var tempPass = _privateCommonSettingsFilePassword.TempData)
                                    {
                                        ScryptPassEncryptedData.WriteToFile(
                                            GlobalModelInstance.CommonPrivateSettings,
                                            privateCommonSettingsFileName,
                                            tempPass.Data
                                            );
                                    }
                                }
                            },
                            useAnyContext: true
                        )
                    );
                
                /**/
                _formSubscriptions.Add(
                    Observable.Interval(TimeSpan.FromSeconds(1.0f))
                        .Subscribe(
                            x => BeginInvokeProper(
                                () =>
                                {
                                    toolStripStatusLabel1.Text =
	                                    $"{DateTime.UtcNow:G} UTC";
                                }
                            )
                        )
                    );
                DefaultFolders.CreateFoldersIfNotExist();
                _miningManager 
                    = await MiningTaskManager.CreateInstance(
                        GlobalModelInstance
                            .CommonPublicSettings
                            .MiningManagerSettings,
                        _miningManagerModel
                    );
                GlobalModelInstance.RpcServersManager 
                    = await LocalRpcServersManager.CreateInstance(
                        GlobalModelInstance.CommonPrivateSettings.CommonJsonRpcSettings,
                        GlobalModelInstance.RpcServersManagerModel,
                        _walletListFormModel,
                        _proxyModel
                    );
                /**/
                InitMiningView();
                InitMiningManagerView();
                InitMessageClientView();
                //////////////////////////////////
                _stateHelper.SetInitializedState();
                /*Just for writing to file*/
                MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                    GlobalModelInstance.CommonPublicSettings,
                    e => e.ApplicationInstanceName
                );
                /*Just for writing to file*/
                MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                    GlobalModelInstance.CommonPrivateSettings,
                    e => e.ProxySettings
                );
                InitProxyModel();
                InitMessageClientModel();
                InitMiningModel();
                InitExchangeModel();
                try
                {
                    await ConnectProxySession();
                }
                catch (Exception exc)
                {
                    ShowErrorMessage(
                        LocStrings.Messages.CreateProxySessionError.Inject(
                            new
                            {
                                ErrorMessage = exc.Message
                            }
                        )
                    );
                    _proxyModel.ProxyServerConnected = false;
                }
            }
            catch (Exception exc)
            {
                MiscFuncs.HandleUnexpectedError(exc,_log);
                await MessageBoxAsync.ShowAsync(
                            this,
                    LocStrings.Messages.MainFormInitializationError.Inject(
                        new
                        {
                            ErrorMessage = exc.Message
                        }
                    )
                );
                _formClosingActionComplete = true;
                Close();
            }
        }
        private static readonly Logger _log
            = LogManager.GetCurrentClassLogger();
        /**/
        private readonly IExchangeClientModel _exchangeModel
            = MyNotifyPropertyChangedImpl.GetProxy(
                (IExchangeClientModel) new ExchangeClientModel()
            );
        private ExchangeServerSession _exchangeSession
            = null;
        /**/
        private readonly MiningTaskManagerModel _miningManagerModel
            = new MiningTaskManagerModel();
        private MiningTaskManager _miningManager;
        /**/
        private readonly IMiningClientModel _miningModel
            = MyNotifyPropertyChangedImpl.GetProxy(
                (IMiningClientModel)new MiningClientModel()
            );
        private MiningServerSession _miningSession
            = null;
        /**/
        private readonly IProxyModel _proxyModel
            = MyNotifyPropertyChangedImpl.GetProxy(
                (IProxyModel)new ProxyModel()
                {
                    ProxyServerConnected = false
                }
            );
        private ProxyServerSessionOverI2P _proxySession
            = null;
        /**/
        private readonly IMessageClientModel _messageClientModel
            = MyNotifyPropertyChangedImpl.GetProxy(
                (IMessageClientModel)(new MessageClientModel())
            );
        private MessageServerSession _messageSession
            = null;
        /**/
        private readonly IWalletListFormModel _walletListFormModel
            = MyNotifyPropertyChangedImpl.GetProxy(
                (IWalletListFormModel)new WalletListFormModel()
            );
        /**/
        private readonly List<IDisposable> _formSubscriptions
            = new List<IDisposable>();
        private readonly List<IMyAsyncDisposable> _formAsyncSubscriptions
            = new List<IMyAsyncDisposable>();
        /**/
        private readonly DisposableObjectStateHelper _stateHelper 
            = new DisposableObjectStateHelper("ClientGuiMainForm");
        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
        }
        
        private bool _formClosingActionComplete = false;
        private readonly CancellationTokenSource _cts 
            = new CancellationTokenSource();
        private async void FormClosingAsyncAction()
        {
            var curMethodName = this.MyNameOfMethod(e => e.FormClosingAsyncAction());
            using (await _lockSemSet.GetDisposable(curMethodName).ConfigureAwait(false))
            {
                try
                {
                    if (_formClosingActionComplete)
                        return;
                    _cts.Cancel();
                    _formSubscriptions.ForEach(x => x.Dispose());
                    _formSubscriptions.Clear();
                    foreach (var asyncSubscription in _formAsyncSubscriptions)
                    {
                        await asyncSubscription.MyDisposeAsync().ConfigureAwait(false);
                    }
                    _formAsyncSubscriptions.Clear();
                    _log.Trace("{0} _formSubscriptions", curMethodName);
                    /**/
                    _log.Trace("{0} _stateHelper start", curMethodName);
                    await _stateHelper.MyDisposeAsync().ConfigureAwait(false);
                    _log.Trace("{0} _stateHelper end", curMethodName);
                    /**/
                    _log.Trace("{0} Disconnect wallets begin", curMethodName);
                    using (await _walletListFormModel.LockSem.GetDisposable().ConfigureAwait(false))
                    {
                        if (_walletListFormModel.WalletInfos.Any())
                        {
                            foreach (
                                var walletId
                                    in _walletListFormModel.WalletInfos.Keys.ToList()
                            )
                            {
                                var walletInfo = _walletListFormModel.WalletInfos[walletId];
                                using (await walletInfo.LockSem.GetDisposable().ConfigureAwait(false))
                                {
                                    await walletInfo.MyDisposeAsync().ConfigureAwait(false);
                                }
                                _walletListFormModel.WalletInfos.Remove(walletId);
                                _walletListFormModel.OnWalletInfoRemoved.OnNext(walletId);
                            }
                        }
                    }
                    _log.Trace("{0} Disconnect wallets end", curMethodName);
                    /**/
                    if (_exchangeModel.ExchangeServerConnected)
                    {
                        _log.Trace("{0} DisconnectExchangeSession start", curMethodName);
                        await DisconnectExchangeSession(true).ConfigureAwait(false);
                        _log.Trace("{0} DisconnectExchangeSession end", curMethodName);
                    }
                    /**/
                    if (_miningModel.MiningServerConnected)
                    {
                        _log.Trace("{0} DisconnectMiningSession start", curMethodName);
                        await DisconnectMiningSession(true).ConfigureAwait(false);
                        _log.Trace("{0} DisconnectMiningSession end", curMethodName);
                    }
                    if (
                        _messageClientModel.ConnectionStatus
                        != MessageSesionConnectionStatus.Disconnected
                    )
                    {
                        _log.Trace("{0} DisconnectMessageSession start", curMethodName);
                        await DisconnectMessageSession(true).ConfigureAwait(false);
                        _log.Trace("{0} DisconnectMessageSession end", curMethodName);
                    }
                    if (_proxyModel.ProxyServerConnected)
                    {
                        _log.Trace("{0} DisconnectProxySession start", curMethodName);
                        await DisconnectProxySession(true).ConfigureAwait(false);
                        _log.Trace("{0} DisconnectProxySession end", curMethodName);
                    }
                    _log.Trace("{0} _miningManager start", curMethodName);
                    await _miningManager.MyDisposeAsync().ConfigureAwait(false);
                    _log.Trace("{0} _miningManager end", curMethodName);
                    _cts.Dispose();
                }
                catch (Exception exc)
                {
                    MiscFuncs.HandleUnexpectedError(exc,_log);
                    await MessageBoxAsync.ShowAsync(
                        this,
                        LocStrings.CommonMessages.UnexpedterErrorIdMessage.Inject(
                            new
                            {
                                Id = curMethodName,
                                ErrorMessage = exc.Message
                            }
                        )
                    );
                }
                finally
                {
                    _formClosingActionComplete = true;
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
        }

        private bool _closeDontAsk = false;
        private readonly SemaphoreSlimSet _lockSemSet = new SemaphoreSlimSet();

        private void ClientGuiForm1_FormClosing(
            object sender, 
            FormClosingEventArgs e
        )
        {
            if(_formClosingActionComplete)
                return;
            if (
                !_closeDontAsk
				&& MessageBox.Show(
                    LocStrings.Messages.AreYouSureQuestion,
                    LocStrings.Messages.QuitQuestionCaption,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                ) == DialogResult.Cancel
            )
            {
                e.Cancel = true;
                return;
            }
            if (!_formClosingActionComplete)
            {
                FormClosingAsyncAction();
            }
        }

        private static async Task HandleControlActionProperInternal(
            Form owner,
            Func<Task> action,
            DisposableObjectStateHelper stateHelper,
            Logger log,
            string idForExcHandling = "",
            string memberName = "",
            int sourceLineNumber = 0
        )
        {
            try
            {
                using (stateHelper.GetFuncWrapper(memberName, sourceLineNumber))
                {
                    await action();
                }
            }
            catch (WrongDisposableObjectStateException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exc)
            {
                ShowErrorMessage(
                    owner,
                    LocStrings.CommonMessages.UnexpedterErrorIdMessage.Inject(
                        new
                        {
                            Id = idForExcHandling,
                            ErrorMessage = exc.Message
                        }
                    )
                );
                log.Error(
                    "HCAP: {1} unexpected error: '{0}'",
                    exc.ToString(),
                    idForExcHandling
                );
            }
        }

        public static async void HandleControlActionProper(
            Form owner,
            Func<Task> action,
            DisposableObjectStateHelper stateHelper,
            Logger log,
            string idForExcHandling = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0
        )
        {
            await HandleControlActionProperInternal(
                owner,
                action,
                stateHelper,
                log,
                idForExcHandling,
                memberName,
                sourceLineNumber
            );
        }

	    public static void HandleControlActionProper(
            Form owner,
            Action action,
			DisposableObjectStateHelper stateHelper,
			Logger log,
			string idForExcHandling = "",
		    [CallerMemberName] string memberName = "",
		    [CallerLineNumber] int sourceLineNumber = 0
		    )
	    {
		    HandleControlActionProper(
                owner,
			    async () =>
			    {
				    await Task.FromResult(0);
				    action();
			    },
				stateHelper,
				log,
			    idForExcHandling,
			    memberName,
			    sourceLineNumber
			);
	    }
		private async void HandleControlActionProper(
			Action action,
			string idForExcHandling = "",
			[CallerMemberName] string memberName = "",
			[CallerLineNumber] int sourceLineNumber = 0
		)
		{
			await HandleControlActionProperInternal(
                this,
				async() => action(),
				_stateHelper,
				_log,
				idForExcHandling,
				memberName,
				sourceLineNumber
			);
		}
		private async void HandleControlActionProper(
            Func<Task> action,
            string idForExcHandling = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0
        )
        {
            await HandleControlActionProperInternal(
                this,
                action,
                _stateHelper,
                _log,
                idForExcHandling,
                memberName,
                sourceLineNumber
            );
        }

        private static void BeginInvokeProperInternal(
            Control control,
            DisposableObjectStateHelper stateHelper,
            Logger log,
            Func<Task> action,
            string idForExcHandling = "",
            string memberName = "",
            int sourceLineNumber = 0,
            bool useAnyContext = false
        )
        {
            var func
                = new Action(
                    async () =>
                    {
                        try
                        {
                            using (stateHelper.GetFuncWrapper(memberName, sourceLineNumber))
                            {
                                await action();
                            }
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (WrongDisposableObjectStateException)
                        {
                        }
                        catch (Exception exc)
                        {
                            log.Error(
                                "BIP: {1} unexpected error: '{0}'",
                                exc.ToString(),
                                idForExcHandling
                            );
                        }
                    }
                );
            if (useAnyContext)
                func();
            else
            {
                if(control.IsHandleCreated)
                    control.BeginInvoke(
                        func
                    );
            }
        }

        public static void BeginInvokeProper(
            Control control,
            DisposableObjectStateHelper stateHelper,
            Logger log,
            Func<Task> action,
            string idForExcHandling = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0
        )
        {
            BeginInvokeProperInternal(
                control,
                stateHelper,
                log,
                action,
                idForExcHandling,
                memberName,
                sourceLineNumber
            );
        }
        public static void BeginInvokeProper(
            Control control,
            DisposableObjectStateHelper stateHelper,
            Logger log,
            Action action,
            string idForExcHandling = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0
        )
        {
            BeginInvokeProperInternal(
                control,
                stateHelper,
                log,
                async () => action(),
                idForExcHandling,
                memberName,
                sourceLineNumber
            );
        }
        private void BeginInvokeProper(
			Action action,
			string idForExcHandling = "",
			[CallerMemberName] string memberName = "",
			[CallerLineNumber] int sourceLineNumber = 0,
			bool useAnyContext = false
		)
		{
			BeginInvokeProperInternal(
				this,
				_stateHelper,
				_log,
				async () =>
				{
					await Task.Delay(0);
					action();
				},
				idForExcHandling,
				memberName,
				sourceLineNumber,
				useAnyContext
			);
		}
		private void BeginInvokeProper(
            Func<Task> action,
            string idForExcHandling = "",
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            bool useAnyContext = false
        )
        {
            BeginInvokeProperInternal(
                this,
                _stateHelper,
                _log,
                action,
                idForExcHandling,
                memberName,
                sourceLineNumber,
                useAnyContext
            );
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void changePrivateCommonSettingsPasswordToolStripMenuItem_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(async () =>
            {
                DefaultFolders.CreateFoldersIfNotExist();
                /**/
                var oldPassword
                    = await EnterPasswordForm.CreateAndReturnResult(
                        LocStrings.Messages.OldPrivateCommonSettingsPasswordRequestCaption,
                        this
                    );
                /**/
                if (oldPassword == null)
                {
                    ShowErrorMessage(
                        LocStrings.Messages.EmptyOldPasswordError
                    );
                    return;
                }
                using (var tempOldPass = _privateCommonSettingsFilePassword.TempData)
                {
                    using (var testOldPass = oldPassword.TempData)
                    {
                        if (!testOldPass.Data.SequenceEqual(tempOldPass.Data))
                        {
                            ShowErrorMessage(
                                LocStrings.Messages.WrongOldPasswordError
                            );
                            return;
                        }
                    }
                }
                /**/
                var newPassword
                    = await EnterPasswordForm.CreateAndReturnResult(
                        LocStrings.Messages.NewPrivateSettingsFilePasswordRequestCaption,
                        this
                    );
                /**/
                if (
                    newPassword
                        == null
                )
                {
                    ShowErrorMessage(
                        LocStrings.CommonMessages.EmptyPasswordError
                    );
                    return;
                }
                using (var tempNewPass = newPassword.TempData)
                {
                    _privateCommonSettingsFilePassword.TempData = tempNewPass;
                }
                /*To rewrite file*/
                MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                    GlobalModelInstance.CommonPrivateSettings,
                    x => x.ExternalTransferProcessorSettings
                );
                ShowInfoMessage(
                    LocStrings.Messages.PasswordChangedInfo
                );
            });
        }

        private void localMiningSettingsToolStripMenuItem_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(
                async () =>
                    await (
                        new EditMiningLocalSettingsForm(
                            GlobalModelInstance.CommonPublicSettings
                                .MiningManagerSettings
                        )
                    ).ShowFormAsync(this)
            );
        }

        private async Task CheckForUpdates()
        {
            if (!_proxyModel.ProxyServerConnected)
            {
                ShowErrorMessage(
                    LocStrings.CommonMessages.ProxyServerIsNotConnectedError
                );
                return;
            }
            if (
                _proxyModel.ProxySessionModel.Balance
                <=
                GlobalModelInstance
                    .CommonPublicSettings
                    .BalanceRestrictions
                    .UpdateClientMinBalance
            )
            {
                ShowErrorMessage(
                    LocStrings.Messages.UpdateClientLowProxyBalanceError.Inject(
                        new
                        {
                            BalaceLimit = 
                                GlobalModelInstance
                                    .CommonPublicSettings
                                    .BalanceRestrictions
                                    .UpdateClientMinBalance
                        }
                    )
                );
                return;
            }
            using (var cts = new CancellationTokenSource())
            {
                var onProgressLoadTcs = new TaskCompletionSource<object>();
                var progressForm = new ProgressCancelForm(
                    cts,
                    onProgressLoadTcs,
                    LocStrings.UpdateClientLocStringsInstance.ProgressFormCaption
                    );
                progressForm.Show(this);
                await onProgressLoadTcs.Task;
                try
                {
                    progressForm.ReportProgress(
                        LocStrings.UpdateClientLocStringsInstance.ProgressFormReport1,
                        10
                        );
                    var curVersionOnServer =
                        await _proxySession.GetClientVersionFromServer(
                            cts.Token
                            );
                    bool forceUpdate = false;
                    if (
                        curVersionOnServer
                        <= CommonClientConstants.CurrentClientVersion
                        )
                    {
                        await MessageBoxAsync.ShowAsync(this,
                            LocStrings.UpdateClientLocStringsInstance
                                .YouHaveLastVersionAlreadyInfo,
                            string.Empty,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                            );
                        return;
                    }
                    else
                    {
                        if (
                            await MessageBoxAsync.ShowAsync(this,
                                LocStrings.UpdateClientLocStringsInstance.NewVersionUpdateQuestion,
                                string.Empty,
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Question
                                ) == DialogResult.Cancel
                            )
                            return;
                    }
                    progressForm.ReportProgress(
                        LocStrings.UpdateClientLocStringsInstance.ProgressFormReport2,
                        40
                        );
                    var progressSubject = new Subject<DownloadFileProgressInfo>();
                    Tuple<Version, byte[]> newVersionData;
                    /**/
                    var oldArchiveFilePath = Path.Combine(
                        DefaultFolders.UpdaterArchivesFolder,
                        CommonClientConstants.CurrentClientVersionString + ".zip"
                        );
                    using (
                        progressSubject.Subscribe(
                            i => BeginInvokeProper(
                                async () =>
                                {
                                    await Task.Delay(0);
                                    progressForm.ReportProgress(
                                        LocStrings.UpdateClientLocStringsInstance.DownloadProgressText.Inject(
                                            new
                                            {
                                                DownloadedKb = i.DownloadedSize/1024,
                                                TotalKb = i.TotalSize/1024
                                            }
                                            ),
                                        50 + (int) ((i.DownloadedSize/(decimal) i.TotalSize)*30)
                                        );
                                }
                                )
                            )
                        )
                    {
                        bool useDiffOnly = false;
                        byte[] oldVersionArchiveData = null;
                        if (File.Exists(oldArchiveFilePath) && !forceUpdate)
                        {
                            oldVersionArchiveData = File.ReadAllBytes(oldArchiveFilePath);
                            useDiffOnly = true;
                        }
                        newVersionData = await _proxySession.GetNewVersionArchive(
                            cts.Token,
                            useDiffOnly,
                            CommonClientConstants.CurrentClientVersion,
                            oldVersionArchiveData,
                            progressSubject
                            );
                    }
                    progressForm.ReportProgress(
                        LocStrings.UpdateClientLocStringsInstance.ProgressFormReport3,
                        80
                        );
                    DefaultFolders.CreateFoldersIfNotExist();

                    var currentProcessPid = Process.GetCurrentProcess().Id;
                    var currentAppFileDir = Path.GetDirectoryName(
                        Environment.GetCommandLineArgs()[0]
                        );

                    string updaterCommandLineArgs = "";
                    if (File.Exists(oldArchiveFilePath))
                    {
                        if (forceUpdate)
                        {
                            var moveOldArchivePath = Path.Combine(
                                DefaultFolders.UpdaterArchivesFolder,
                                CommonClientConstants.CurrentClientVersionString
                                + string.Format("_{0:yyyyMMddHHmmss}.zip", DateTime.UtcNow)
                                );
                            File.Move(oldArchiveFilePath, moveOldArchivePath);
                            oldArchiveFilePath = moveOldArchivePath;
                        }
                        updaterCommandLineArgs += string.Format(
                            " --old-version-archive=\"{0}\"",
                            Path.GetFullPath(oldArchiveFilePath)
                            );
                    }
                    /**/
                    var newArchiveFilePath = Path.Combine(
                        DefaultFolders.UpdaterArchivesFolder,
                        newVersionData.Item1.ToString() + ".zip"
                        );
                    if (File.Exists(newArchiveFilePath))
                        File.Delete(newArchiveFilePath);
                    File.WriteAllBytes(
                        newArchiveFilePath,
                        newVersionData.Item2
                        );
                    /**/
                    updaterCommandLineArgs += string.Format(
                        " --pid={0}" +
                        " --new-version-archive=\"{1}\"" +
                        " --app-dir-path=\"{2}\"" +
                        " --execute-on-exit=\"{3}\"",
                        currentProcessPid,
                        Path.GetFullPath(newArchiveFilePath),
                        currentAppFileDir,
                        BitMoneyClientProgram.CommandLineOptions.UseMonoRun == 1
                            ? ""
                            : Path.GetFullPath(
                                UpdaterSettings.ExecuteOnExitPath
                                )
                        );
                    var updaterExecutableFullPath = Path.GetFullPath(
                        BitMoneyClientProgram.CommandLineOptions.UseMonoRun == 1
                            ? UpdaterSettings.UpdaterExecutablePathOnMono
                            : UpdaterSettings.UpdaterExecutablePath
                        );
                    var processStartInfo = new ProcessStartInfo();
                    var workingDirectory = Path.GetDirectoryName(
                        updaterExecutableFullPath
                        );
                    if (string.IsNullOrWhiteSpace(workingDirectory))
                        throw new ArgumentNullException(
                            MyNameof.GetLocalVarName(() => workingDirectory));
                    processStartInfo.WorkingDirectory = workingDirectory;
                    processStartInfo.FileName = updaterExecutableFullPath;
                    processStartInfo.Arguments = updaterCommandLineArgs;
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    Process.Start(
                        processStartInfo
                        );
                    _closeDontAsk = true;
                    Close();
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    progressForm.SetProgressComplete();
                }
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(
                async () =>
                {
                    await CheckForUpdates();
                }
            );
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async Task<bool> CheckPrivateCommonSettingsPassword()
        {
            var checkPrivateCommonSettingsFilePassword =
                await EnterPasswordForm.CreateAndReturnResult(
                    LocStrings.Messages.PrivateCommonSettingsPasswordRequestCaption,
                    this
                );
            if (checkPrivateCommonSettingsFilePassword == null)
            {
                ShowErrorMessage(
                    LocStrings.CommonMessages.EmptyPasswordError
                );
                return false;
            }
            using (var tempPass1 = _privateCommonSettingsFilePassword.TempData)
            {
                using (var tempPass2 = checkPrivateCommonSettingsFilePassword.TempData)
                {
                    if (!tempPass1.Data.SequenceEqual(tempPass2.Data))
                    {
                        ShowErrorMessage(
                            LocStrings.Messages.WrongPrivateCommonSettingsPasswordError
                        );
                        return false;
                    }
                }
            }
            return true;
        }
        // Edit json rpc server settings
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(
                async () =>
                {
                    if (!await CheckPrivateCommonSettingsPassword())
                        return;
                    await new EditJsonRpcServerSettingsForm(
                        GlobalModelInstance
                            .CommonPrivateSettings.CommonJsonRpcSettings,
                        _walletListFormModel.WalletInfos.Keys.ToList(),
                        GlobalModelInstance.RpcServersManager,
                        GlobalModelInstance.RpcServersManagerModel
                    ).ShowFormAsync(this);
                }
            );
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            BeginInvoke(new Action(async () => await AsyncInit()));
        }

        public static void ChangeControlFont(
            Control control,
            float newFontSize,
            float oldFontSize = 9.0f
        )
        {
            var oldControlFont = control.Font;
            if (Math.Abs(oldControlFont.SizeInPoints - newFontSize) > 0.01)
            {
                var newControlFont = new Font(
                    oldControlFont.FontFamily,
                    newFontSize,
                    oldControlFont.Style
                    );
                control.Font = newControlFont;
            }
            if(control.ContextMenuStrip != null)
                ChangeControlFont(
                    control.ContextMenuStrip,
                    newFontSize,
                    oldFontSize
                );
            foreach (Control internalControl in control.Controls)
            {
                ChangeControlFont(
                    internalControl,
                    newFontSize,
                    oldFontSize
                );
            }
        }
        public static ClientGuiMainFormDesignerLocStrings DesignerLocStrings = new ClientGuiMainFormDesignerLocStrings();
        private void InitCommonView()
        {
            this.statusStrip1.Text = DesignerLocStrings.StatusStrip1Text;
            this.toolStripStatusLabel2.Text = DesignerLocStrings.ToolStripStatusLabel2Text;
            this.toolStripStatusLabel5.Text = DesignerLocStrings.ToolStripStatusLabel5Text;
            this.toolStripStatusLabel1.Text = DesignerLocStrings.ToolStripStatusLabel1Text;
            this.toolStripStatusLabel4.Text = DesignerLocStrings.ToolStripStatusLabel4Text;
            this.menuStrip1.Text = DesignerLocStrings.MenuStrip1Text;
            this.mainToolStripMenuItem.Text = DesignerLocStrings.MainToolStripMenuItemText;
            this.proxyToolStripMenuItem.Text = DesignerLocStrings.ProxyToolStripMenuItemText;
            this.connectToolStripMenuItem.Text = DesignerLocStrings.ConnectToolStripMenuItemText;
            this.disconnectToolStripMenuItem.Text = DesignerLocStrings.DisconnectToolStripMenuItemText;
            this.i2PSettingsToolStripMenuItem.Text = DesignerLocStrings.I2PSettingsToolStripMenuItemText;
            this.localMiningToolStripMenuItem.Text = DesignerLocStrings.LocalMiningToolStripMenuItemText;
            this.localMiningSettingsToolStripMenuItem.Text = DesignerLocStrings.LocalMiningSettingsToolStripMenuItemText;
            this.jSONRpcServersettignsToolStripMenuItem.Text = DesignerLocStrings.JSONRpcServersettignsToolStripMenuItemText;
            this.settingsToolStripMenuItem.Text = DesignerLocStrings.SettingsToolStripMenuItemText;
            this.changePrivateCommonSettingsPasswordToolStripMenuItem.Text = DesignerLocStrings.ChangePrivateCommonSettingsPasswordToolStripMenuItemText;
            this.checkForUpdatesToolStripMenuItem.Text = DesignerLocStrings.CheckForUpdatesToolStripMenuItemText;
            this.quitToolStripMenuItem.Text = DesignerLocStrings.QuitToolStripMenuItemText;
            this.walletToolStripMenuItem.Text = DesignerLocStrings.WalletToolStripMenuItemText;
            this.registerNewToolStripMenuItem.Text = DesignerLocStrings.RegisterNewToolStripMenuItemText;
            this.loginToolStripMenuItem1.Text = DesignerLocStrings.LoginToolStripMenuItem1Text;
            this.processInvoiceToolStripMenuItem.Text = DesignerLocStrings.ProcessInvoiceToolStripMenuItemText;
            this.changePasswordToolStripMenuItem2.Text = DesignerLocStrings.ChangePasswordToolStripMenuItem2Text;
            this.externalPaymentProcessorSettingsToolStripMenuItem.Text = DesignerLocStrings.ExternalPaymentProcessorSettingsToolStripMenuItemText;
            this.userToolStripMenuItem.Text = DesignerLocStrings.UserToolStripMenuItemText;
            this.addNewToolStripMenuItem.Text = DesignerLocStrings.AddNewToolStripMenuItemText;
            this.loginToolStripMenuItem.Text = DesignerLocStrings.LoginToolStripMenuItemText;
            this.logoutToolStripMenuItem.Text = DesignerLocStrings.LogoutToolStripMenuItemText;
            this.copyMyUserIdToClipboardToolStripMenuItem.Text = DesignerLocStrings.CopyMyUserIdToClipboardToolStripMenuItemText;
            this.changePasswordToolStripMenuItem1.Text = DesignerLocStrings.ChangePasswordToolStripMenuItem1Text;
            this.messageUserSettingsToolStripMenuItem.Text = DesignerLocStrings.MessageUserSettingsToolStripMenuItemText;
            this.miningToolStripMenuItem.Text = DesignerLocStrings.MiningToolStripMenuItemText;
            this.regisnterNewAccountToolStripMenuItem.Text = DesignerLocStrings.RegisnterNewAccountToolStripMenuItemText;
            this.loginToolStripMenuItem2.Text = DesignerLocStrings.LoginToolStripMenuItem2Text;
            this.logoutToolStripMenuItem2.Text = DesignerLocStrings.LogoutToolStripMenuItem2Text;
            this.changePasswordToolStripMenuItem.Text = DesignerLocStrings.ChangePasswordToolStripMenuItemText;
            this.exchangeToolStripMenuItem.Text = DesignerLocStrings.ExchangeToolStripMenuItemText;
            this.registerNewExchangeProfileToolStripMenuItem.Text = DesignerLocStrings.RegisterNewExchangeProfileToolStripMenuItemText;
            this.loginToolStripMenuItem3.Text = DesignerLocStrings.LoginToolStripMenuItem3Text;
            this.logoutToolStripMenuItem3.Text = DesignerLocStrings.LogoutToolStripMenuItem3Text;
            this.changePasswordToolStripMenuItem3.Text = DesignerLocStrings.ChangePasswordToolStripMenuItem3Text;
            this.currencyListMenuItem.Text = DesignerLocStrings.CurrencyListMenuItemText;
            this.securityListToolStripMenuItem.Text = DesignerLocStrings.SecurityListToolStripMenuItemText;
            this.ordersToolStripMenuItem.Text = DesignerLocStrings.OrdersToolStripMenuItemText;
            this.tradesToolStripMenuItem.Text = DesignerLocStrings.TradesToolStripMenuItemText;
            this.depositFundsToolStripMenuItem.Text = DesignerLocStrings.DepositFundsToolStripMenuItemText;
            this.withdrawFundsToolStripMenuItem.Text = DesignerLocStrings.WithdrawFundsToolStripMenuItemText;
            this.columnHeader1.Text = DesignerLocStrings.ColumnHeader1Text;
            this.columnHeader2.Text = DesignerLocStrings.ColumnHeader2Text;
            this.columnHeader3.Text = DesignerLocStrings.ColumnHeader3Text;
            this.columnHeader4.Text = DesignerLocStrings.ColumnHeader4Text;
            this.columnHeader5.Text = DesignerLocStrings.ColumnHeader5Text;
            this.columnHeader11.Text = DesignerLocStrings.ColumnHeader11Text;
            this.columnHeader12.Text = DesignerLocStrings.ColumnHeader12Text;
            this.columnHeader13.Text = DesignerLocStrings.ColumnHeader13Text;
            this.transferStatusHeader.Text = DesignerLocStrings.TransferStatusHeaderText;
            this.transferAmountHeader.Text = DesignerLocStrings.TransferAmountHeaderText;
            this.transferFeeHeader.Text = DesignerLocStrings.TransferFeeHeaderText;
            this.transferWalletFromHeader.Text = DesignerLocStrings.TransferWalletFromHeaderText;
            this.transferWalletToHeader.Text = DesignerLocStrings.TransferWalletToHeaderText;
            this.transferCommentHeader.Text = DesignerLocStrings.TransferCommentHeaderText;
            this.transferSentTimeHeader.Text = DesignerLocStrings.TransferSentTimeHeaderText;
            this.transferAuthenticationsHeader.Text = DesignerLocStrings.TransferAuthenticationsHeaderText;
            this.showTranferInfoToolStripMenuItem.Text = DesignerLocStrings.ShowTranferInfoToolStripMenuItemText;
            this.copyWalletToToolStripMenuItem.Text = DesignerLocStrings.CopyWalletToToolStripMenuItemText;
            this.copyWalletFromToolStripMenuItem.Text = DesignerLocStrings.CopyWalletFromToolStripMenuItemText;
            this.copyCommentStringToolStripMenuItem.Text = DesignerLocStrings.CopyCommentStringToolStripMenuItemText;
            this.repeatToolStripMenuItem.Text = DesignerLocStrings.RepeatToolStripMenuItemText;
            this.columnAutowidthByHeaderToolStripMenuItem2.Text = DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItem2Text;
            this.columnAutowidthByContentToolStripMenuItem2.Text = DesignerLocStrings.ColumnAutowidthByContentToolStripMenuItem2Text;
            this.label3.Text = DesignerLocStrings.Label3Text;
            this.label2.Text = DesignerLocStrings.Label2Text;
            this.sendFundsToToolStripMenuItem.Text = DesignerLocStrings.SendFundsToToolStripMenuItemText;
            this.copyWalletGUIDToolStripMenuItem.Text = DesignerLocStrings.CopyWalletGUIDToolStripMenuItemText;
            this.processInvoiceToolStripMenuItem1.Text = DesignerLocStrings.ProcessInvoiceToolStripMenuItem1Text;
            this.updateBalanceToolStripMenuItem.Text = DesignerLocStrings.UpdateBalanceToolStripMenuItemText;
            this.walletSettingsToolStripMenuItem.Text = DesignerLocStrings.WalletSettingsToolStripMenuItemText;
            this.showFullTransferHistoryToolStripMenuItem.Text = DesignerLocStrings.ShowFullTransferHistoryToolStripMenuItemText;
            this.logoutToolStripMenuItem1.Text = DesignerLocStrings.LogoutToolStripMenuItem1Text;
            this.label20.Text = DesignerLocStrings.Label20Text;
            this.label19.Text = DesignerLocStrings.Label19Text;
            this.label18.Text = DesignerLocStrings.Label18Text;
            this.label17.Text = DesignerLocStrings.Label17Text;
            this.updateBalanceToolStripMenuItem3.Text = DesignerLocStrings.UpdateBalanceToolStripMenuItem3Text;
            this.issueInvoiceToFillupToolStripMenuItem1.Text = DesignerLocStrings.IssueInvoiceToFillupToolStripMenuItem1Text;
            this.label16.Text = DesignerLocStrings.Label16Text;
            this.label12.Text = DesignerLocStrings.Label12Text;
            this.label4.Text = DesignerLocStrings.Label4Text;
            this.addContactToolStripMenuItem.Text = DesignerLocStrings.AddContactToolStripMenuItemText;
            this.updateContactInfosToolStripMenuItem.Text = DesignerLocStrings.UpdateContactInfosToolStripMenuItemText;
            this.label14.Text = DesignerLocStrings.Label14Text;
            this.label13.Text = DesignerLocStrings.Label13Text;
            this.groupBox2.Text = DesignerLocStrings.GroupBox2Text;
            this.label8.Text = DesignerLocStrings.Label8Text;
            this.button2.Text = DesignerLocStrings.Button2Text;
            this.label9.Text = DesignerLocStrings.Label9Text;
            this.label7.Text = DesignerLocStrings.Label7Text;
            this.columnHeader14.Text = DesignerLocStrings.ColumnHeader14Text;
            this.columnHeader15.Text = DesignerLocStrings.ColumnHeader15Text;
            this.columnHeader21.Text = DesignerLocStrings.ColumnHeader21Text;
            this.columnHeader16.Text = DesignerLocStrings.ColumnHeader16Text;
            this.button5.Text = DesignerLocStrings.Button5Text;
            this.button4.Text = DesignerLocStrings.Button4Text;
            this.button1.Text = DesignerLocStrings.Button1Text;
            this.groupBox1.Text = DesignerLocStrings.GroupBox1Text;
            this.label11.Text = DesignerLocStrings.Label11Text;
            this.label10.Text = DesignerLocStrings.Label10Text;
            this.label15.Text = DesignerLocStrings.Label15Text;
            this.button3.Text = DesignerLocStrings.Button3Text;
            this.label6.Text = DesignerLocStrings.Label6Text;
            this.updateBalanceToolStripMenuItem1.Text = DesignerLocStrings.UpdateBalanceToolStripMenuItem1Text;
            this.label5.Text = DesignerLocStrings.Label5Text;
            this.forcePingProxyToolStripMenuItem.Text = DesignerLocStrings.ForcePingProxyToolStripMenuItemText;
            this.issueInvoiceToFillupToolStripMenuItem.Text = DesignerLocStrings.IssueInvoiceToFillupToolStripMenuItemText;
            this.updateBalanceToolStripMenuItem2.Text = DesignerLocStrings.UpdateBalanceToolStripMenuItem2Text;
            this.editContactDataToolStripMenuItem.Text = DesignerLocStrings.EditContactDataToolStripMenuItemText;
            this.removeToolStripMenuItem.Text = DesignerLocStrings.RemoveToolStripMenuItemText;
            this.authorizeUserWriteToMeToolStripMenuItem.Text = DesignerLocStrings.AuthorizeUserWriteToMeToolStripMenuItemText;
            this.authorizeMeWriteToUserToolStripMenuItem.Text = DesignerLocStrings.AuthorizeMeWriteToUserToolStripMenuItemText;
            this.revokeAuthorizationFromUserWriteToMeToolStripMenuItem.Text = DesignerLocStrings.RevokeAuthorizationFromUserWriteToMeToolStripMenuItemText;
            this.addToFavoriteToolStripMenuItem.Text = DesignerLocStrings.AddToFavoriteToolStripMenuItemText;
            this.tabPage4.Text = DesignerLocStrings.TabPage4Text;
            this.tabPage6.Text = DesignerLocStrings.TabPage6Text;
            this.checkBox1.Text = DesignerLocStrings.CheckBox1Text;
            this.checkBox2.Text = DesignerLocStrings.CheckBox2Text;
            this.tabPage7.Text = DesignerLocStrings.TabPage7Text;
            this.tabPage8.Text = DesignerLocStrings.TabPage8Text;
            this.tabPage1.Text = DesignerLocStrings.TabPage1Text;
            this.exchangeAccountsLabel.Text = DesignerLocStrings.ExchangeAccountsLabelText;
            this.exchangeAccountGuidHeader.Text = DesignerLocStrings.ExchangeAccountGuidHeaderText;
            this.exchangeAccountCurrencyHeader.Text = DesignerLocStrings.ExchangeAccountCurrencyHeaderText;
            this.exchangeAccountBalanceHeader.Text = DesignerLocStrings.ExchangeAccountBalanceHeaderText;
            this.exchangeAccountLockedHeader.Text = DesignerLocStrings.ExchangeAccountLockedHeaderText;
            this.exchangeAccountTotalHeader.Text = DesignerLocStrings.ExchangeAccountTotalHeaderText;
            this.exchangeTransferDateHeader.Text = DesignerLocStrings.ExchangeTransferDateHeaderText;
            this.exchangeTransferTypeHeader.Text = DesignerLocStrings.ExchangeTransferTypeHeaderText;
            this.exchangeTransferValueHeader.Text = DesignerLocStrings.ExchangeTransferValueHeaderText;
            this.exchangeTransferNoteHeader.Text = DesignerLocStrings.ExchangeTransferNoteHeaderText;
            this.exchangeTransferGuidHeader.Text = DesignerLocStrings.ExchangeTransferGuidHeaderText;
            this.columnAutowidthByHeaderToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItemText;
            this.columnAutowidthByContentToolStripMenuItem.Text = DesignerLocStrings.ColumnAutowidthByContentToolStripMenuItemText;
            this.cashFlowLabel.Text = DesignerLocStrings.CashFlowLabelText;
            this.exchangeLockedFundsDateHeader.Text = DesignerLocStrings.ExchangeLockedFundsDateHeaderText;
            this.exchangeLockedFundsValueHeader.Text = DesignerLocStrings.ExchangeLockedFundsValueHeaderText;
            this.exchangeLockedFundsNoteHeader.Text = DesignerLocStrings.ExchangeLockedFundsNoteHeaderText;
            this.exchangeLockedFundsGuidHeader.Text = DesignerLocStrings.ExchangeLockedFundsGuidHeaderText;
            this.columnAutowidthByHeaderToolStripMenuItem1.Text = DesignerLocStrings.ColumnAutowidthByHeaderToolStripMenuItem1Text;
            this.columnAutowidthByContentToolStripMenuItem1.Text = DesignerLocStrings.ColumnAutowidthByContentToolStripMenuItem1Text;
            this.lockedFundsLabel.Text = DesignerLocStrings.LockedFundsLabelText;
            this.exchangeClientLabel.Text = DesignerLocStrings.ExchangeClientLabelText;
            this.exchangeProfileNameLabel.Text = DesignerLocStrings.ExchangeProfileNameLabelText;
            this.copyExchangeClientGUIDToolStripMenuItem.Text = DesignerLocStrings.CopyExchangeClientGUIDToolStripMenuItemText;
            this.removeFromFavoriteToolStripMenuItem.Text = DesignerLocStrings.RemoveFromFavoriteToolStripMenuItemText;
            this.createNewAccountToolStripMenuItem.Text = DesignerLocStrings.CreateNewAccountToolStripMenuItemText;
            this.makeDefaultForTheCurrencyToolStripMenuItem.Text = DesignerLocStrings.MakeDefaultForTheCurrencyToolStripMenuItemText;
            this.copyAccountGUIDToolStripMenuItem.Text = DesignerLocStrings.CopyAccountGUIDToolStripMenuItemText;
            this.newDepositToolStripMenuItem.Text = DesignerLocStrings.NewDepositToolStripMenuItemText;
            this.newWithdrawToolStripMenuItem.Text = DesignerLocStrings.NewWithdrawToolStripMenuItemText;
            this.copyMiningClientGUIDToolStripMenuItem.Text = DesignerLocStrings.CopyMiningClientGUIDToolStripMenuItemText;
            this.Text = DesignerLocStrings.Text;
            ClientGuiMainForm.ChangeControlFont(this, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_AddUser, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_Wallet, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenuStrip_miningBalance, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenuStrip_i2pDestinationBalanceLavel, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_EditUserContact, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenuStrip_ShowTransferInfo, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_MessageBalance, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_FavoriteLabel, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_exchangeProfileNameLabel, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_exchangeAccounts_Empty, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_exchangeAccounts_Entry, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_exchangeTransferListView, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_exchangeLockedFundsListView, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
            ClientGuiMainForm.ChangeControlFont(contextMenu_miningServerClientName, ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt);
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

    }
    public class ClientGuiMainFormDesignerLocStrings
    {
        public string StatusStrip1Text = "statusStrip1";
        public string ToolStripStatusLabel2Text = "I2p destination balance";
        public string ToolStripStatusLabel5Text = "Relative time difference";
        public string ToolStripStatusLabel1Text = "Time";
        public string ToolStripStatusLabel4Text = "User status";
        public string MenuStrip1Text = "menuStrip1";
        public string MainToolStripMenuItemText = "Main";
        public string ProxyToolStripMenuItemText = "Proxy";
        public string ConnectToolStripMenuItemText = "Connect";
        public string DisconnectToolStripMenuItemText = "Disconnect";
        public string I2PSettingsToolStripMenuItemText = "I2p settings";
        public string LocalMiningToolStripMenuItemText = "Local mining";
        public string LocalMiningSettingsToolStripMenuItemText = "Settings";
        public string JSONRpcServersettignsToolStripMenuItemText = "JSON RPC servers";
        public string SettingsToolStripMenuItemText = "Settings";
        public string ChangePrivateCommonSettingsPasswordToolStripMenuItemText = "Change private common settings password";
        public string CheckForUpdatesToolStripMenuItemText = "Check for updates";
        public string QuitToolStripMenuItemText = "Quit";
        public string WalletToolStripMenuItemText = "Wallet";
        public string RegisterNewToolStripMenuItemText = "Register new";
        public string LoginToolStripMenuItem1Text = "Login";
        public string ProcessInvoiceToolStripMenuItemText = "Process invoice";
        public string ChangePasswordToolStripMenuItem2Text = "Change password";
        public string ExternalPaymentProcessorSettingsToolStripMenuItemText = "External payment processor settings";
        public string UserToolStripMenuItemText = "Messages";
        public string AddNewToolStripMenuItemText = "Register new";
        public string LoginToolStripMenuItemText = "Login";
        public string LogoutToolStripMenuItemText = "Logout";
        public string CopyMyUserIdToClipboardToolStripMenuItemText = "Copy my user GUID to clipboard";
        public string ChangePasswordToolStripMenuItem1Text = "Change password";
        public string MessageUserSettingsToolStripMenuItemText = "Message user settings";
        public string MiningToolStripMenuItemText = "Mining";
        public string RegisnterNewAccountToolStripMenuItemText = "Register new account";
        public string LoginToolStripMenuItem2Text = "Login";
        public string LogoutToolStripMenuItem2Text = "Logout";
        public string ChangePasswordToolStripMenuItemText = "Change password";
        public string ExchangeToolStripMenuItemText = "Exchange";
        public string RegisterNewExchangeProfileToolStripMenuItemText = "Register new profile";
        public string LoginToolStripMenuItem3Text = "Login";
        public string LogoutToolStripMenuItem3Text = "Logout";
        public string ChangePasswordToolStripMenuItem3Text = "Change password";
        public string CurrencyListMenuItemText = "Currency list";
        public string SecurityListToolStripMenuItemText = "Security list";
        public string OrdersToolStripMenuItemText = "Orders";
        public string TradesToolStripMenuItemText = "Trades";
        public string DepositFundsToolStripMenuItemText = "Deposits";
        public string WithdrawFundsToolStripMenuItemText = "Withdraws";
        public string ColumnHeader1Text = "Priority";
        public string ColumnHeader2Text = "Status";
        public string ColumnHeader3Text = "Type";
        public string ColumnHeader4Text = "Balance gain";
        public string ColumnHeader5Text = "Task GUID";
        public string ColumnHeader11Text = "Alias";
        public string ColumnHeader12Text = "Balance";
        public string ColumnHeader13Text = "Wallet GUID";
        public string TransferStatusHeaderText = "Status";
        public string TransferAmountHeaderText = "Amount";
        public string TransferFeeHeaderText = "Fee";
        public string TransferWalletFromHeaderText = "From";
        public string TransferWalletToHeaderText = "To";
        public string TransferCommentHeaderText = "Comment";
        public string TransferSentTimeHeaderText = "Time";
        public string TransferAuthenticationsHeaderText = "Authentications";
        public string ShowTranferInfoToolStripMenuItemText = "Show full tranfer info";
        public string CopyWalletToToolStripMenuItemText = "Copy wallet To";
        public string CopyWalletFromToolStripMenuItemText = "Copy wallet From";
        public string CopyCommentStringToolStripMenuItemText = "Copy comment string";
        public string RepeatToolStripMenuItemText = "Repeat";
        public string ColumnAutowidthByHeaderToolStripMenuItem2Text = "Column autowidth by header";
        public string ColumnAutowidthByContentToolStripMenuItem2Text = "Column autowidth by content";
        public string Label3Text = "Recent transfers:";
        public string Label2Text = "Wallets:";
        public string SendFundsToToolStripMenuItemText = "Send funds";
        public string CopyWalletGUIDToolStripMenuItemText = "Copy wallet GUID";
        public string ProcessInvoiceToolStripMenuItem1Text = "Process invoice";
        public string UpdateBalanceToolStripMenuItemText = "Update balance";
        public string WalletSettingsToolStripMenuItemText = "Wallet settings on server";
        public string ShowFullTransferHistoryToolStripMenuItemText = "Full transfer history";
        public string LogoutToolStripMenuItem1Text = "Logout";
        public string Label20Text = "Keep for:";
        public string Label19Text = "0";
        public string Label18Text = "Message fee:";
        public string Label17Text = "0";
        public string UpdateBalanceToolStripMenuItem3Text = "Update balance";
        public string IssueInvoiceToFillupToolStripMenuItem1Text = "Issue invoice to refill";
        public string Label16Text = "Balance:";
        public string Label12Text = "Waiting to send:";
        public string Label4Text = "0";
        public string AddContactToolStripMenuItemText = "Add contact";
        public string UpdateContactInfosToolStripMenuItemText = "Update contact info data from server";
        public string Label14Text = "___";
        public string Label13Text = "Mining client:";
        public string GroupBox2Text = "Mining jobs";
        public string Label8Text = "Job name:";
        public string Button2Text = "Add job";
        public string Label9Text = "Wishful income:";
        public string Label7Text = "Job type";
        public string ColumnHeader14Text = "Job name";
        public string ColumnHeader15Text = "Mined\\Total gain";
        public string ColumnHeader21Text = "Task type";
        public string ColumnHeader16Text = "Status";
        public string Button5Text = "Resume";
        public string Button4Text = "Pause";
        public string Button1Text = "Remove";
        public string GroupBox1Text = "Transfer to wallet";
        public string Label11Text = "Wallet GUID:";
        public string Label10Text = "Transfer amount:";
        public string Label15Text = "Waiting to send transfers:";
        public string Button3Text = "Send funds";
        public string Label6Text = "0.0";
        public string UpdateBalanceToolStripMenuItem1Text = "Update balance";
        public string Label5Text = "Account balance:";
        public string ForcePingProxyToolStripMenuItemText = "Force ping proxy";
        public string IssueInvoiceToFillupToolStripMenuItemText = "Issue invoice to refill";
        public string UpdateBalanceToolStripMenuItem2Text = "Update balance";
        public string EditContactDataToolStripMenuItemText = "Contact data";
        public string RemoveToolStripMenuItemText = "Remove";
        public string AuthorizeUserWriteToMeToolStripMenuItemText = "Grant the user the permission to write me";
        public string AuthorizeMeWriteToUserToolStripMenuItemText = "Get the permission write to user";
        public string RevokeAuthorizationFromUserWriteToMeToolStripMenuItemText = "Revoke the permission from the user to write me";
        public string AddToFavoriteToolStripMenuItemText = "Add to favorites";
        public string TabPage4Text = "Wallets";
        public string TabPage6Text = "Messages";
        public string CheckBox1Text = "Online only";
        public string CheckBox2Text = "New messages only";
        public string TabPage7Text = "Mining server";
        public string TabPage8Text = "Mining local";
        public string TabPage1Text = "Exchange";
        public string ExchangeAccountsLabelText = "Exchange accounts:";
        public string ExchangeAccountGuidHeaderText = "Account GUID";
        public string ExchangeAccountCurrencyHeaderText = "Currency";
        public string ExchangeAccountBalanceHeaderText = "Balance";
        public string ExchangeAccountLockedHeaderText = "Locked funds";
        public string ExchangeAccountTotalHeaderText = "Total available";
        public string ExchangeTransferDateHeaderText = "Date";
        public string ExchangeTransferTypeHeaderText = "Type";
        public string ExchangeTransferValueHeaderText = "Value";
        public string ExchangeTransferNoteHeaderText = "Note";
        public string ExchangeTransferGuidHeaderText = "Transfer GUID";
        public string ColumnAutowidthByHeaderToolStripMenuItemText = "Column autowidth by header";
        public string ColumnAutowidthByContentToolStripMenuItemText = "Column autowidth by content";
        public string CashFlowLabelText = "Transfers:";
        public string ExchangeLockedFundsDateHeaderText = "Date";
        public string ExchangeLockedFundsValueHeaderText = "Value";
        public string ExchangeLockedFundsNoteHeaderText = "Note";
        public string ExchangeLockedFundsGuidHeaderText = "Locked fund GUID";
        public string ColumnAutowidthByHeaderToolStripMenuItem1Text = "Column autowidth by header";
        public string ColumnAutowidthByContentToolStripMenuItem1Text = "Column autowidth by content";
        public string LockedFundsLabelText = "Locked funds";
        public string ExchangeClientLabelText = "Exchange profile";
        public string ExchangeProfileNameLabelText = "Profile name";
        public string CopyExchangeClientGUIDToolStripMenuItemText = "Copy exchange client GUID";
        public string RemoveFromFavoriteToolStripMenuItemText = "Remove from favorites";
        public string CreateNewAccountToolStripMenuItemText = "Create new account";
        public string MakeDefaultForTheCurrencyToolStripMenuItemText = "Make default for the currency";
        public string CopyAccountGUIDToolStripMenuItemText = "Copy account GUID";
        public string NewDepositToolStripMenuItemText = "New deposit";
        public string NewWithdrawToolStripMenuItemText = "New withdraw";
        public string CopyMiningClientGUIDToolStripMenuItemText = "Copy mining client GUID";
        public string Text = "BitMoney GUI";
    }
}
