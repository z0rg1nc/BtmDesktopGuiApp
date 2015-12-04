using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BtmI2p.BitMoneyClient.Gui.Communication.Message;
using BtmI2p.BitMoneyClient.Gui.Communication.Messages;
using BtmI2p.AesHelper;
using BtmI2p.BitMoneyClient.Lib.MessageServerSession;
using BtmI2p.CryptFile.Lib;
using BtmI2p.GeneralClientInterfaces;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private async Task ConnectMessageSession()
        {
            using (await _messageActionLockSem.GetDisposable())
            {
                using (GetMessageActionInProgressDisposable())
                {
                    if (!_proxyModel.ProxyServerConnected)
                    {
                        ShowErrorMessage(
                            LocStrings.CommonMessages.ProxyServerIsNotConnectedError
                        );
                        return;
                    }
                    if (_proxyModel.ProxySessionModel.NewVersionAvailable)
                    {
                        ShowErrorMessage(
                            LocStrings.CommonMessages.UpdateClientFirstError
                        );
                        return;
                    }
                    if (
                        _messageClientModel.ConnectionStatus
                        != MessageSesionConnectionStatus.Disconnected
                    )
                    {
                        ShowErrorMessage(
                            LocStrings.MessageServerLocStringsInstance
                                .Messages.MessageServerIsConnectedError
                        );
                        return;
                    }
                    if (
                        _proxyModel.ProxySessionModel.Balance
                        <=
                        GlobalModelInstance
                            .CommonPublicSettings
                            .BalanceRestrictions
                            .ConnectUserServerMinBalance
                        )
                    {
                        ShowErrorMessage(
                            LocStrings.MessageServerLocStringsInstance
                                .Messages.LoginLowBalanceError.Inject(
                                new
                                {
                                    BalanceLimit
                                        = GlobalModelInstance
                                            .CommonPublicSettings
                                            .BalanceRestrictions
                                            .ConnectUserServerMinBalance
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
                            LocStrings.MessageServerLocStringsInstance
                                .ConnectLocStringsInstance.ProgressFormCaption
                            );
                        progressForm.Show(this);
                        await onProgressLoadTcs.Task;
                        _messageClientModel.ConnectionStatus
                            = MessageSesionConnectionStatus.Connecting;
                        try
                        {
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                LocStrings.MessageServerLocStringsInstance
                                    .ConnectLocStringsInstance.ProgressFormReport1,
                                0
                                );
                            /**/
                            DefaultFolders.CreateFoldersIfNotExist();
                            var dirInfo = new DirectoryInfo(
                                DefaultFolders.MessageProfilesFolder
                                );
                            var profileFileList = dirInfo.GetFiles(
                                "*.aes256",
                                SearchOption.TopDirectoryOnly
                                );
                            if (profileFileList.Length == 0)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.NoProfileFilesFoundError
                                    );
                                return;
                            }
                            var userProfileTcs = new TaskCompletionSource<FileInfo>();
                            await (new SelectProfileForm(
                                userProfileTcs,
                                profileFileList,
                                LocStrings.MessageServerLocStringsInstance
                                    .ConnectLocStringsInstance.SelectUserProfileFormCaption
                                )).ShowFormAsync(this);
                            var userProfileFileInfo = await userProfileTcs.Task;
                            if (userProfileFileInfo == null)
                            {
                                return;
                            }
                            var userProfileFilePassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    LocStrings.MessageServerLocStringsInstance
                                        .ConnectLocStringsInstance.UserProfileFilePasswordRequestText,
                                    this
                                    );
                            if (userProfileFilePassword == null)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                    );
                                return;
                            }
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                LocStrings.MessageServerLocStringsInstance
                                    .ConnectLocStringsInstance.ProgressFormReport2,
                                10
                                );
                            MessageClientProfile messageClientProfile;
                            try
                            {
                                using (var tempPass = userProfileFilePassword.TempData)
                                {
                                    messageClientProfile = ScryptPassEncryptedData
                                        .ReadFromFile<MessageClientProfile>(
                                            userProfileFileInfo.FullName,
                                            tempPass.Data
                                        );
                                }
                                messageClientProfile.CheckMe();
                            }
                            catch (EnumException<ScryptPassEncryptedData.EGetValueT1ErrCodes> enumExc)
                            {
                                MiscFuncs.HandleUnexpectedError(enumExc, _log);
                                if (enumExc.ExceptionCode == ScryptPassEncryptedData.EGetValueT1ErrCodes.WrongPassword)
                                    ShowErrorMessage(
                                        LocStrings.CommonMessages.WrongPasswordError
                                        );
                                else
                                {
                                    ShowErrorMessage(
                                        LocStrings.MessageServerLocStringsInstance
                                            .ConnectLocStringsInstance.LoadingUserProfileError
                                        );
                                }
                                return;
                            }
                            catch (Exception exc)
                            {
                                MiscFuncs.HandleUnexpectedError(exc, _log);
                                ShowErrorMessage(
                                    LocStrings.MessageServerLocStringsInstance
                                        .ConnectLocStringsInstance.LoadingUserProfileError
                                    );
                                return;
                            }
                            IMessageClientSettings messageClientSettings;
                            try
                            {
                                messageClientSettings = MyNotifyPropertyChangedImpl.GetProxy(
                                    (IMessageClientSettings) ScryptPassEncryptedData
                                        .ReadFromFile<MessageClientSettings>(
                                            messageClientProfile.GetSettingsFilePath(),
                                            messageClientProfile.SettingsPass
                                        )
                                    );
                                messageClientSettings.CheckMe();
                            }
                            catch (Exception)
                            {
                                ShowErrorMessage(
                                    LocStrings.MessageServerLocStringsInstance
                                        .ConnectLocStringsInstance.LoadingUserProfileError
                                    );
                                return;
                            }
                            var userCertPassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    LocStrings.MessageServerLocStringsInstance
                                        .ConnectLocStringsInstance.UserCertPasswordRequestText,
                                    this
                                    );
                            if (userCertPassword == null)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                    );
                                return;
                            }
                            try
                            {
                                using (var tempPass = userCertPassword.TempData)
                                {
                                    messageClientProfile.UserCert.CheckMe(
                                        true,
                                        tempPass.Data
                                        );
                                }
                                if (
                                    !LightCertificateRestrictions.IsValid(
                                        messageClientProfile.UserCert
                                        )
                                    )
                                    throw new ArgumentOutOfRangeException(
                                        MyNameof.GetLocalVarName(
                                            () => messageClientProfile.UserCert
                                            )
                                        );
                            }
                            catch
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages
                                        .WrongPasswordError
                                    );
                                return;
                            }
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                LocStrings.MessageServerLocStringsInstance
                                    .ConnectLocStringsInstance.ProgressFormReport3,
                                25
                                );
                            if (
                                messageClientSettings.MessageHistoryVersion
                                < MessageClientSettings.CurrentMessageHistoryVersion
                                )
                            {
                                if (File.Exists(messageClientSettings.GetMessageHistoryFilePath()))
                                    File.Delete(messageClientSettings.GetMessageHistoryFilePath());
                                var tempSettings = new MessageClientSettings();
                                messageClientSettings.MessageHistoryFileName
                                    = tempSettings.MessageHistoryFileName;
                                messageClientSettings.MessageHistoryPass
                                    = tempSettings.MessageHistoryPass;
                                messageClientSettings.MessageHistoryVersion
                                    = MessageClientSettings.CurrentMessageHistoryVersion;
                            }
                            MessageHistory messageHistory;
                            if (messageClientSettings.SaveMessageHistory)
                            {
                                try
                                {
                                    messageHistory = await MessageHistory.CreateInstance(
                                        messageClientSettings.GetMessageHistoryFilePath(),
                                        messageClientSettings.MessageHistoryPass
                                        );
                                }
                                catch (Exception exc)
                                {
                                    _log.Error(
                                        string.Format(
                                            "Message history loading " +
                                            "from file error: '{0}'",
                                            exc.ToString()
                                            )
                                        );
                                    ShowErrorMessage(
                                        LocStrings.MessageServerLocStringsInstance
                                            .ConnectLocStringsInstance.MessagesHistoryLoadingError.Inject(
                                                new
                                                {
                                                    ErrorMessage = exc.Message
                                                }
                                            )
                                        );
                                    return;
                                }
                            }
                            else
                            {
                                messageHistory = MessageHistory.CreateInstance();
                            }
                            var messageSessionSettings = new MessageServerSessionSettings
                            {
                                UserCert = messageClientProfile.UserCert,
                                MessagesInitCounter = await messageHistory.GetInitCounter(),
                                SendIAmOnlineMessages
                                    = messageClientSettings.SendIAmOnlineMessages
                            };
                            messageSessionSettings.LastKnownReceivedMessageGuid
                                = await messageHistory.GetLastKnownMessageGuid(false)
                                    .ConfigureAwait(false);
                            messageSessionSettings.LastKnownSentMessageGuid
                                = await messageHistory.GetLastKnownMessageGuid(true)
                                    .ConfigureAwait(false);
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                LocStrings.MessageServerLocStringsInstance
                                    .ConnectLocStringsInstance.ProgressFormReport4,
                                45
                                );
                            using (await _messageClientModel.LockSem.GetDisposable())
                            {
                                _messageClientModel.Profile
                                    = messageClientProfile;
                                _messageClientModel.ProfileName
                                    = messageClientProfile.ProfileName;
                                _messageClientModel.Settings
                                    = messageClientSettings;
                                _messageClientModel.ProfileName
                                    = messageClientProfile.ProfileName;
                                _messageClientModel.UserId
                                    = messageClientProfile.UserCert.Id;
                                _messageClientModel.MessageHistoryInstance
                                    = messageHistory;
                                _messageClientModel.SessionModel
                                    = MyNotifyPropertyChangedImpl.GetProxy(
                                        (IMessageServerSessionModel)
                                            new MessageServerSessionModel()
                                        );
                            }
                            InitMessageClientSettingsSubscription(
                                messageClientSettings,
                                messageClientProfile.GetSettingsFilePath(),
                                new AesProtectedByteArray(
                                    new TempByteArray(
                                        messageClientProfile.SettingsPass.ToArray()
                                        )
                                    )
                                );
                            /*Save possible changes*/
                            MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                                messageClientSettings,
                                e => e.SaveMessageHistory
                                );
                            InitMessageSessionModelSubscription(
                                _messageClientModel.SessionModel
                                );
                            _messageSession = await MessageServerSession.CreateInstance(
                                messageSessionSettings,
                                _messageClientModel.SessionModel,
                                _proxySession,
                                userCertPassword,
                                cts.Token
                                );
                            await _messageSession.SetContactsList(
                                messageClientSettings
                                    .ContactInfoList
                                    .Select(x => x.UserId)
                                    .ToList()
                                );
                            UpdateContactListViews();
                            _messageClientModel.ConnectionStatus
                                = MessageSesionConnectionStatus.Connected;
                            _messageClientModel.ConnectedTime
                                = await _proxySession.GetNowTime();
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                LocStrings.MessageServerLocStringsInstance
                                    .ConnectLocStringsInstance.ProgressFormReport5,
                                100
                                );
                            UpdateContactInfoPermissions();
                        }
                        catch (Exception exc)
                        {
                            _log.Error(
                                "Connect user error '{0}'", exc.ToString()
                                );
                            _messageSession = null;
                            ShowErrorMessage(
                                LocStrings.CommonMessages.UnexpectedErrorMessage.Inject(
                                    new
                                    {
                                        ErrorMessage = exc.Message
                                    }
                                    )
                                );
                        }
                        finally
                        {
                            if (
                                _messageClientModel.ConnectionStatus
                                != MessageSesionConnectionStatus.Connected
                                )
                            {
                                _messageClientModel.ConnectedTime
                                    = DateTime.MinValue;
                                _messageClientModel.ConnectionStatus
                                    = MessageSesionConnectionStatus.Disconnected;
                            }
                            progressForm.SetProgressComplete();
                        }
                    }
                }
            }
        }

        private async Task DisconnectMessageSession(bool dontShowProgressForm = false)
        {
            using (await _messageActionLockSem.GetDisposable())
            {
                using (GetMessageActionInProgressDisposable())
                {
                    if (
                        _messageClientModel.ConnectionStatus
                        != MessageSesionConnectionStatus.Connected
                    )
                    {
                        ShowErrorMessage(
                            LocStrings.CommonMessages.NotConnecterYetError
                        );
                        return;
                    }
                    _messageClientModel.ConnectionStatus
                        = MessageSesionConnectionStatus.Disconnecting;
                    /**/
                    using (var cts = new CancellationTokenSource())
                    {
                        ProgressCancelForm progressForm = null;
                        if (!dontShowProgressForm)
                        {
                            var onProgressLoadTcs = new TaskCompletionSource<object>();
                            progressForm = new ProgressCancelForm(
                                cts,
                                onProgressLoadTcs,
                                LocStrings.MessageServerLocStringsInstance
                                    .DisconnectLocStringsInstance.ProgressFormCaption
                                );
                            progressForm.Show(this);
                            await onProgressLoadTcs.Task;
                        }
                        try
                        {
                            if (!dontShowProgressForm)
                            {
                                cts.Token.ThrowIfCancellationRequested();
                                progressForm.ReportProgress(
                                    LocStrings.MessageServerLocStringsInstance
                                        .DisconnectLocStringsInstance.ProgressFormReport1,
                                    0
                                    );
                            }

                            if (!dontShowProgressForm)
                            {
                                progressForm.ReportProgress(
                                    LocStrings.MessageServerLocStringsInstance
                                        .DisconnectLocStringsInstance.ProgressFormReport2,
                                    30
                                    );
                            }
                            await _messageSession.MyDisposeAsync();
                            await _messageClientModel.MessageHistoryInstance.MyDisposeAsync()
                                .ConfigureAwait(false);
                            _messageSession = null;
                            _messageClientModel.ActiveContactInfo = null;
                            _messageClientModel.SessionModel.Balance = 0.0m;
                            BeginInvokeProper(async () =>
                            {
                                richTextBox1.Clear();
                                messageContactsListView.Items.Clear();
                                flowLayoutPanel3.Controls.Clear();
                            });
                            foreach (IDisposable subscription in _userSessionSubscriptions)
                            {
                                subscription.Dispose();
                            }
                            _userSessionSubscriptions.Clear();
                            if (!dontShowProgressForm)
                            {
                                progressForm.ReportProgress(
                                    LocStrings.MessageServerLocStringsInstance
                                        .DisconnectLocStringsInstance.ProgressFormReport3,
                                    100
                                    );
                            }
                        }
                        finally
                        {
                            _messageClientModel.ConnectionStatus
                                = MessageSesionConnectionStatus.Disconnected;
                            _messageClientModel.ConnectedTime
                                = DateTime.MinValue;
                            if (!dontShowProgressForm)
                            {
                                progressForm.SetProgressComplete();
                            }
                        }
                    }
                }
            }
        }
    }
}
