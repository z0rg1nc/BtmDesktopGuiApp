using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Message;
using BtmI2p.BitMoneyClient.Gui.Forms.Message;
using BtmI2p.BitMoneyClient.Gui.Forms.User;
using BtmI2p.AesHelper;
using BtmI2p.BitMoneyClient.Lib.MessageServerSession;
using BtmI2p.GeneralClientInterfaces.MessageServer;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;
using BtmI2p.ObjectStateLib;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        /**/
        //Register message server account
        private async void addNewToolStripMenuItem_Click(
            object sender, EventArgs e
        )
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
                _proxyModel.ProxySessionModel.Balance
                <=
                GlobalModelInstance
                    .CommonPublicSettings
                    .BalanceRestrictions
                    .RegisterMessageServerMinBalance
            )
            {
                ShowErrorMessage(
                    LocStrings.MessageServerLocStringsInstance
                        .Messages.RegistrationLowBalanceError.Inject(
                            new
                            {
                                BalanceLimit 
                                    = GlobalModelInstance
                                        .CommonPublicSettings
                                        .BalanceRestrictions
                                        .RegisterMessageServerMinBalance
                            }
                        )
                );
                return;
            }
            await new RegisterMessageClientForm(_proxySession).ShowFormAsync(this);
        }
        private readonly SemaphoreSlim _messageActionLockSem = new SemaphoreSlim(1);
        private bool _messageActionInProgress = false;
        private ActionDisposable GetMessageActionInProgressDisposable()
        {
            return new ActionDisposable(
                () => { _messageActionInProgress = true; },
                () => { _messageActionInProgress = false; }
            );
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                if (_messageActionInProgress)
                {
                    ShowErrorMessage(
                        LocStrings.MessageServerLocStringsInstance
                            .Messages.MessageServerActionInProgressError
                    );
                    return;
                }
                await ConnectMessageSession();
            });
        }

        private static void AppendColorTextRichTextBox(
            RichTextBox box, 
            string text, 
            Color color
        )
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            var defaultFont = box.SelectionFont;
            var boldDefaultFont = new Font(
                defaultFont,
                FontStyle.Italic
            );
            box.SelectionFont = boldDefaultFont;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.SelectionFont = defaultFont;
        }

        private readonly SemaphoreSlim _richTextBox1LockSem 
            = new SemaphoreSlim(1);
        private async Task AddIncomeMessageToRichTextBox(
            ClientTextMessage incomeMessage
        )
        {
            var contactInfo = 
                _messageClientModel.Settings.ContactInfoList.FirstOrDefault(x => x.UserId == incomeMessage.UserFrom);
            using(await _richTextBox1LockSem.GetDisposable())
            {
                AppendColorTextRichTextBox(richTextBox1,
                    LocStrings.MessageServerLocStringsInstance.IncomeMessageHeader.Inject(
                        new
                        {
                            SentTime = incomeMessage.SentTime,
                            LifetimeMinutes = (int)((incomeMessage.SaveUntil - incomeMessage.SentTime).TotalMinutes),
                            UserFrom = contactInfo == null
                                ? incomeMessage.UserFrom.ToString()
                                : contactInfo.Alias,
                            AuthUser = incomeMessage.OtherUserCertAuthenticated ? "+" : "-",
                            AuthKey = incomeMessage.MessageKeyAuthenticated ? "+" : "-",
                            AuthMessage = incomeMessage.MessageAuthenticated ? "+" : "-"
                        }
                    ),
                    Color.Red
                );
                richTextBox1.AppendText(
                    string.Format(
                        Environment.NewLine + "{0}" + Environment.NewLine,
                        incomeMessage.MessageText
                    )
                );
            }
        }

        private async Task AddOutcomeMessageToRichTextBox(
            ClientTextMessage outcomeMessage
        )
        {
            var curMethodName = this.MyNameOfMethod(e => e.AddOutcomeMessageToRichTextBox(null));
            try
            {
                var contactInfo =
                    _messageClientModel.Settings.ContactInfoList.FirstOrDefault(
                        x => x.UserId == outcomeMessage.UserTo
                        );
                using (await _richTextBox1LockSem.GetDisposable())
                {
                    AppendColorTextRichTextBox(
                        richTextBox1,
                        LocStrings.MessageServerLocStringsInstance.OutcomeMessageHeader.Inject(
                            new
                            {
                                SentTime = outcomeMessage.SentTime,
                                LifetimeMinutes = (int)((outcomeMessage.SaveUntil - outcomeMessage.SentTime).TotalMinutes),
                                UserTo =
                                    contactInfo == null
                                        ? outcomeMessage.UserTo.ToString()
                                        : contactInfo.Alias,
                                AuthUser = outcomeMessage.OtherUserCertAuthenticated ? "+" : "-",
                                AuthKey = outcomeMessage.MessageKeyAuthenticated ? "+" : "-",
                                AuthMessage = outcomeMessage.MessageAuthenticated ? "+" : "-"
                            }
                        ),
                        Color.CornflowerBlue
                    );
                    richTextBox1.AppendText(
                        string.Format(
                            Environment.NewLine + "{0}" + Environment.NewLine
                            , outcomeMessage.MessageText
                            )
                        );
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
                _log.Error("{0} unexpected error '{1}'", curMethodName, exc.ToString());
            }
        }
        private readonly
            SemaphoreSlim _userSettingsFileLockSem = new SemaphoreSlim(1);
        private readonly List<IDisposable> _userSessionSubscriptions 
            = new List<IDisposable>(); 
        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(
                async () =>
                {
                    if (_messageActionInProgress)
                    {
                        ShowErrorMessage(
                            LocStrings.MessageServerLocStringsInstance
                                .Messages.MessageServerActionInProgressError
                        );
                        return;
                    }
                    await DisconnectMessageSession();
                }
            );
        }
        private void copyMyUserIdToClipboardToolStripMenuItem_Click(
            object sender, 
            EventArgs e
        )
        {
            HandleControlActionProper(
                () =>
                {
                    if (
                        _messageClientModel.ConnectionStatus
                        != MessageSesionConnectionStatus.Connected
                        )
                    {
                        ShowErrorMessage(
                            LocStrings.CommonMessages.NotLoggedYetError
                        );
                        return;
                    }
                    Clipboard.SetText($"{_messageClientModel.UserId}");
                }
            );
        }
        /**/
        private void addContactToolStripMenuItem_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(
                async () =>
                {
                    if (
                        _messageClientModel.ConnectionStatus
                        != MessageSesionConnectionStatus.Connected
                        )
                    {
                        ShowErrorMessage(
                            LocStrings.MessageServerLocStringsInstance
                                .Messages.MessageServerIsNotConnectedError
                        );
                        return;
                    }

                    var resultTcs = new TaskCompletionSource<AddContactFormData>();
                    await (new AddContactForm(resultTcs)).ShowFormAsync(this);
                    var newContactInfo = await resultTcs.Task;
                    if (newContactInfo == null)
                    {
                        throw new ArgumentNullException(
                            MyNameof.GetLocalVarName(() => newContactInfo)
                        );
                    }
                    if (_messageClientModel.Settings.ContactInfoList.Any(
                        x => x.UserId == newContactInfo.UserGuid))
                    {
                        ShowErrorMessage(
                            LocStrings.MessageServerLocStringsInstance
                                .Messages.ContactAlreadyAddedError
                        );
                        return;
                    }
                    if (
						await MessageBoxAsync.ShowAsync(this,
                            LocStrings.MessageServerLocStringsInstance
                                .AddContactLocStringsInstance.AddUserThePermissionToWriteMeQuestion,
                            string.Empty,
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question
                        ) == DialogResult.OK)
                    {
                        using (var cts = new CancellationTokenSource())
                        {
                            var onProgressLoadTcs = new TaskCompletionSource<object>();
                            var progressForm = new ProgressCancelForm(
                                cts,
                                onProgressLoadTcs,
                                LocStrings.MessageServerLocStringsInstance
                                    .AddContactLocStringsInstance.GrantPermissionProgressFormCaption
                                );
                            progressForm.Show(this);
                            await onProgressLoadTcs.Task;
                            try
                            {
                                progressForm.ReportProgress(
                                    LocStrings.MessageServerLocStringsInstance
                                        .AddContactLocStringsInstance.GrantPermissionProgressFormReport1,
                                    10
                                    );
                                await _messageSession.GrantUserPermissionWriteToMe(
                                    newContactInfo.UserGuid,
                                    cts.Token
                                    );
                                progressForm.ReportProgress(
                                    LocStrings.MessageServerLocStringsInstance
                                        .AddContactLocStringsInstance.GrantPermissionProgressFormReport2,
                                    100
                                    );
                            }
                            finally
                            {
                                progressForm.SetProgressComplete();
                            }
                        }
                    }
                    _messageClientModel.Settings.ContactInfoList.Add(new ContactInfo()
                    {
                        UserId = newContactInfo.UserGuid,
                        Alias = newContactInfo.Alias
                    });
                    MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                        _messageClientModel.Settings,
                        x => x.ContactInfoList
                    );
                    UpdateContactInfoPermissions();
                }
            );
        }

        //Send message
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            HandleControlActionProper(
                async () =>
                {
                    if (e.Control && e.KeyCode == Keys.Enter)
                    {
                        try
                        {
                            if (
                                _messageClientModel.ConnectionStatus
                                != MessageSesionConnectionStatus.Connected
                            )
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.NotLoggedYetError
                                );
                                return;
                            }
                            var activeContactInfo = _messageClientModel.ActiveContactInfo;
                            if (activeContactInfo == null)
                            {
                                ShowErrorMessage(
                                    LocStrings.MessageServerLocStringsInstance
                                        .Messages.ContactNotSelectedError
                                );
                                return;
                            }
                            var textToSend = textBox1.Text;
                            if (string.IsNullOrWhiteSpace(textToSend))
                            {
                                ShowErrorMessage(
                                    LocStrings.MessageServerLocStringsInstance
                                        .Messages.EmptyMessageStringError
                                );
                                return;
                            }
                            var tempMessageInDb = new AesKeyIvPair().EncryptData(
                                Encoding.UTF8.GetBytes((string) textToSend)
                            );
                            if(tempMessageInDb.Length > MessageServerClientLimitations.MaxMessageSize)
                            {
                                ShowErrorMessage(
                                    LocStrings.MessageServerLocStringsInstance
                                        .Messages.TooBigMessageError
                                );
                                return;
                            }
                            textBox1.Clear();
                            var receiverGuid = activeContactInfo.UserId;
                            try
                            {
                                var keepForTs = (TimeSpan) comboBox3.SelectedValue;
                                var maxFee = MessageServerMessageFeeHelper.GetFee(
                                    tempMessageInDb.Length,
                                    keepForTs,
                                    !activeContactInfo.AmIAuthorizedWriteToUser,
                                    activeContactInfo.SettingsOnServer.UnauthorizedIncomeMessageFee
                                ) + MessageServerSession.MaxFeeSurplus;
                                await _messageSession.SendTextMessage(
                                    textToSend,
                                    receiverGuid,
                                    keepForTs,
                                    true,
                                    maxFee
                                );
                            }
                            catch (Exception exc)
                            {
                                ShowErrorMessage(
                                    LocStrings.MessageServerLocStringsInstance
                                        .Messages.SendMessageError.Inject(
                                        new
                                        {
                                            ErrorMessage = exc.Message
                                        }
                                    )
                                );
                                return;
                            }
                        }
                        finally
                        {
                            e.SuppressKeyPress = true;
                        }
                    }
                }
            );
        }

        //Select contact info
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleControlActionProper(
                () =>
                {
                    if (
                        _messageClientModel.ConnectionStatus
                        != MessageSesionConnectionStatus.Connected
                    )
                        return;
                    var selectedItems = messageContactsListView.SelectedItems;
                    if (selectedItems.Count == 0)
                    {
                        if (_messageClientModel.ActiveContactInfo != null)
                        {
                            _messageClientModel.ActiveContactInfo = null;
                        }
                    }
                    else
                    {
                        var selectedItem = selectedItems[0];
                        var selectedContactGuid = (Guid) selectedItem.Tag;
                        var contactInfo =
                            _messageClientModel.Settings.ContactInfoList
                                .First(x => x.UserId == selectedContactGuid);
                        if (
                            _messageClientModel.ActiveContactInfo == null 
                            ||
                            _messageClientModel.ActiveContactInfo.UserId
                                != selectedContactGuid
                        )
                        {
                            _messageClientModel.ActiveContactInfo = contactInfo;
                        }
                    }
                }
            );
        }
        private void editContactDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(
                async () =>
                {
                    if (
                        _messageClientModel.ConnectionStatus
                        != MessageSesionConnectionStatus.Connected
                        )
                        return;
                    if (_messageClientModel.ActiveContactInfo == null)
                        return;
                    await (new ViewUserContactInfo(_messageClientModel.ActiveContactInfo, _messageClientModel))
                        .ShowFormAsync(this);
                }
            );
        }
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(
                async () =>
                {
                    if (
                        _messageClientModel.ConnectionStatus
                        != MessageSesionConnectionStatus.Connected
                    )
                    {
                        ShowErrorMessage(
                            LocStrings.MessageServerLocStringsInstance
                                .Messages.MessageServerIsNotConnectedError
                        );
                        return;
                    }
                    var activeContactInfo = _messageClientModel.ActiveContactInfo;
                    if (activeContactInfo == null)
                    {
                        ShowErrorMessage(
                            LocStrings.MessageServerLocStringsInstance
                                .Messages.ContactNotSelectedError
                        );
                        return;
                    }
                    if (
						await MessageBoxAsync.ShowAsync(this,
                            LocStrings.MessageServerLocStringsInstance
                                .RemoveContactLocStringsInstance.RevokePermissionTooQuestion,
                            string.Empty,
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question
                        ) == DialogResult.OK)
                    {
                        using (var cts = new CancellationTokenSource())
                        {
                            var onProgressLoadTcs = new TaskCompletionSource<object>();
                            var progressForm = new ProgressCancelForm(
                                cts,
                                onProgressLoadTcs,
                                LocStrings.MessageServerLocStringsInstance
                                    .RemoveContactLocStringsInstance.ProgressFormCaption
                                );
                            progressForm.Show(this);
                            await onProgressLoadTcs.Task;
                            try
                            {
                                progressForm.ReportProgress(
                                    LocStrings.MessageServerLocStringsInstance
                                        .RemoveContactLocStringsInstance.ProgressFormReport1,
                                    10
                                    );
                                await _messageSession.RevokeUserPermissionWriteToMe(
                                    activeContactInfo.UserId, CancellationToken.None
                                    );
                                progressForm.ReportProgress(
                                    LocStrings.MessageServerLocStringsInstance
                                        .RemoveContactLocStringsInstance.ProgressFormReport2,
                                    100
                                    );
                                UpdateContactInfoPermissions();
                            }
                            finally
                            {
                                progressForm.SetProgressComplete();
                            }
                        }
                    }
                    _messageClientModel.Settings.ContactInfoList.RemoveAll(
                        x => 
                            x.Alias == activeContactInfo.Alias 
                            && x.UserId == activeContactInfo.UserId
                    );
                    MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                        _messageClientModel.Settings,
                         x => x.ContactInfoList
                    );
                    UpdateContactInfoPermissions();
                }
            );
        }
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            HandleControlActionProper(() =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (_messageClientModel.ActiveContactInfo == null)
                    {
                        contextMenu_AddUser.Show(
                            Cursor.Position
                        );
                    }
                    else
                    {
                        contextMenu_EditUserContact.Show(
                            Cursor.Position
                        );
                    }
                }
            });
        }
        
        private void changePasswordToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var locStrings = LocStrings.MessageServerLocStringsInstance
                .ChangePasswordLocStringsInstance;
            ChangePasswordTemplate<MessageClientProfile>(
                new ChangePasswordTemplateLocStrings()
                {
                    NewCertPasswordRequestText = locStrings.NewCertPasswordRequestText,
                    NewMasterCertPasswordRequestText = locStrings.NewMasterCertPasswordRequestText,
                    NewProfileFilePasswordRequestText = locStrings.NewProfileFilePasswordRequestText,
                    OldCertPasswordRequestText = locStrings.OldCertPasswordRequestText,
                    OldMasterCertPasswordRequestText = locStrings.OldMasterCertPasswordRequestText,
                    OldProfileFilePasswordRequestText = locStrings.OldProfileFilePasswordRequestText,
                    SelectPasswordToChangeFormCaption = locStrings.SelectPasswordToChangeFormCaption,
                    SelectProfileFormCaption = LocStrings.MessageServerLocStringsInstance
                        .ConnectLocStringsInstance.SelectUserProfileFormCaption
                },
                _messageActionLockSem,
                GetMessageActionInProgressDisposable,
                DefaultFolders.MessageProfilesFolder,
                async profileName => 
                    (
                        _messageClientModel.ConnectionStatus == MessageSesionConnectionStatus.Connected
                        || _messageClientModel.ConnectionStatus == MessageSesionConnectionStatus.Connecting
                    ) && _messageClientModel.ProfileName == profileName,
                new List<EProfilePasswordKinds>()
                {
                    EProfilePasswordKinds.Profile,
                    EProfilePasswordKinds.Cert,
                    EProfilePasswordKinds.MasterCert
                },
                _ => _.UserCert,
                _ => _.MasterUserCert
            );
        }
        private void updateBalanceToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(() =>
            {
                if (
                    _messageClientModel.ConnectionStatus
                    != MessageSesionConnectionStatus.Connected
                )
                {
                    ShowErrorMessage(
                        LocStrings.MessageServerLocStringsInstance
                            .Messages.MessageServerIsNotConnectedError
                    );
                    return;
                }
                _messageSession.UpdateBalance();
            });
        }

        private void issueInvoiceToFillupToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MessageServerLocStringsInstance;
                if (
                    _messageClientModel.ConnectionStatus
                    != MessageSesionConnectionStatus.Connected
                )
                {
                    ShowErrorMessage(
                        locStrings.Messages.MessageServerIsNotConnectedError
                    );
                    return;
                }
                var inputBoxForm = new InputBoxForm(
                    locStrings.IssueInvoiceLocStringsInstance.RefillAmount, 
                    "1", 
                    @"^[1-9]{1}\d*$"
                );
                await inputBoxForm.ShowFormAsync(this);
                string transferAmountString = await inputBoxForm.TaskValue;
                if (transferAmountString == string.Empty)
                    return;
                long transferAmount;
                if (
                    !long.TryParse(
                        transferAmountString,
                        out transferAmount
                    )
                    || transferAmount <= 0
                    || transferAmount > int.MaxValue
                )
                {
                    ShowErrorMessage(
                        locStrings.IssueInvoiceLocStringsInstance.WrongTransferAmountError
                    );
                    return;
                }
                var issueData = await _messageSession.IssueInvoiceTo(
                    transferAmount
                );
                ProcessWalletInvoice(issueData);
            });
        }

        private void UpdateMessageFee()
        {
            BeginInvokeProper(() =>
            {
                var messageLength = Encoding.UTF8.GetBytes((string) textBox1.Text).Length;
                var keepForTs = (TimeSpan) comboBox3.SelectedValue;
                var activeContactInfo = _messageClientModel.ActiveContactInfo;
                var amIGrantedWriteToThisUser 
                    = activeContactInfo == null || activeContactInfo.AmIAuthorizedWriteToUser;
                var additionalFee 
                    = activeContactInfo?.SettingsOnServer.UnauthorizedIncomeMessageFee ?? 0.0m;
                var fee = MessageServerMessageFeeHelper.GetFee(
                    messageLength,
                    keepForTs,
                    !amIGrantedWriteToThisUser,
                    additionalFee
                );
                label19.Text = $"{fee:0.00}";
            });
        }

        private readonly Subject<object> _textBox1TestChangedSubject
            = new Subject<object>();
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _textBox1TestChangedSubject.OnNext(null);
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMessageFee();
        }
        private void messageUserSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MessageServerLocStringsInstance;
                if (
                    _messageClientModel.ConnectionStatus
                    != MessageSesionConnectionStatus.Connected
                )
                {
                    ShowErrorMessage(
                        locStrings.Messages.MessageServerIsNotConnectedError
                    );
                    return;
                }
                using (var cts = new CancellationTokenSource())
                {
                    var onProgressLoadTcs = new TaskCompletionSource<object>();
                    var progressForm = new ProgressCancelForm(
                        cts,
                        onProgressLoadTcs,
                        locStrings.EditUserSettingsLocStringsInstance.ProgressFormCaption
                        );
                    progressForm.Show(this);
                    await onProgressLoadTcs.Task;
                    MessageClientSettingsOnServerClientInfo settingsOnServer;
                    try
                    {
                        progressForm.ReportProgress(
                            locStrings.EditUserSettingsLocStringsInstance.ProgressFormReport1,
                            10
                            );
                        progressForm.ReportProgress(
                            locStrings.EditUserSettingsLocStringsInstance.ProgressFormReport2,
                            30
                            );
                        settingsOnServer = await _messageSession.GetMySettingsOnServer(
                            cts.Token
                            );
                        progressForm.ReportProgress(
                            locStrings.EditUserSettingsLocStringsInstance.ProgressFormReport3,
                            100
                            );
                    }
                    finally
                    {
                        progressForm.SetProgressComplete();
                    }
                    await (
                        new EditMessageUserSettingsForm(
                            _messageClientModel.Settings, 
                            settingsOnServer, 
                            _messageSession.SetMySettingsOnServer
                        )
                    ).ShowFormAsync(this);
                }
            });
        }

        private async Task UpdateContactInfoPermissionsFromServer(CancellationToken token)
        {
            var contactInfos = _messageClientModel.Settings.ContactInfoList.ToList();
            var contactInfosOnServer
                = await _messageSession.GetContactUserInfosOnServer(
                    contactInfos.Select(x => x.UserId).ToList(),
                    token
                );
            foreach (ContactInfo contactInfo in contactInfos)
            {
                if (contactInfosOnServer.ContainsKey(contactInfo.UserId))
                {
                    var contactInfoOnServer
                        = contactInfosOnServer[contactInfo.UserId];
                    contactInfo.SettingsOnServer = contactInfoOnServer.Item1;
                    contactInfo.AmIAuthorizedWriteToUser = contactInfoOnServer.Item2;
                    contactInfo.IsUserAuthorizedWriteToMe = contactInfoOnServer.Item3;
                }
            }
            MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                _messageClientModel.Settings,
                x => x.ContactInfoList
            );
        }

        private async void UpdateContactInfoPermissions()
        {
            BeginInvokeProper(async () =>
            {
                var locStrings = LocStrings.MessageServerLocStringsInstance
                    .UpdateContactInfosLocStringsInstance;
                if (
                    _messageClientModel.ConnectionStatus
                    != MessageSesionConnectionStatus.Connected
                )
                {
                    ShowErrorMessage(
                        LocStrings.MessageServerLocStringsInstance
                            .Messages.MessageServerIsNotConnectedError
                    );
                    return;
                }

                /**/
                using (var cts = new CancellationTokenSource())
                {
                    var onProgressLoadTcs = new TaskCompletionSource<object>();
                    var progressForm = new ProgressCancelForm(
                        cts,
                        onProgressLoadTcs,
                        locStrings.ProgressFormCaption
                        );
                    progressForm.Show(this);
                    await onProgressLoadTcs.Task;
                    try
                    {
                        progressForm.ReportProgress(locStrings.ProgressFormReport1, 10);
                        await UpdateContactInfoPermissionsFromServer(cts.Token);
                        progressForm.ReportProgress(locStrings.ProgressFormReport2, 100);
                    }
                    finally
                    {
                        progressForm.SetProgressComplete();
                    }
                }
            });
        }

        private void updateContactInfosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateContactInfoPermissions();
        }
        //Grant user permission write to me
        private void authorizeUserWriteToMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MessageServerLocStringsInstance;
                if (
                    _messageClientModel.ConnectionStatus
                    != MessageSesionConnectionStatus.Connected
                )
                {
                    ShowErrorMessage(
                        locStrings.Messages.MessageServerIsNotConnectedError
                    );
                    return;
                }
                var activeContactInfo = _messageClientModel.ActiveContactInfo;
                if (activeContactInfo == null)
                {
                    ShowErrorMessage(
                        locStrings.Messages.ContactNotSelectedError
                    );
                    return;
                }
                if (activeContactInfo.IsUserAuthorizedWriteToMe)
                {
                    ShowErrorMessage(
                        locStrings.PermissionsLocStringsInstance.PermissionAlreadyGrantedError
                    );
                    return;
                }
                /**/
                using (var cts = new CancellationTokenSource())
                {
                    var onProgressLoadTcs = new TaskCompletionSource<object>();
                    var progressForm = new ProgressCancelForm(
                        cts,
                        onProgressLoadTcs,
                        locStrings.PermissionsLocStringsInstance.GrantToWriteMeProgressFormCaption
                        );
                    progressForm.Show(this);
                    await onProgressLoadTcs.Task;
                    try
                    {
                        progressForm.ReportProgress(LocStrings.CommonText.Start, 10);
                        await _messageSession.GrantUserPermissionWriteToMe(
                            activeContactInfo.UserId,
                            cts.Token
                            );
                        progressForm.ReportProgress(LocStrings.CommonText.Finish, 100);
                        UpdateContactInfoPermissions();
                    }
                    finally
                    {
                        progressForm.SetProgressComplete();
                    }
                }
            });
        }
        // Revoke permission from user write to me
        private void revokeAuthorizationFromUserWriteToMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MessageServerLocStringsInstance;
                if (
                    _messageClientModel.ConnectionStatus
                    != MessageSesionConnectionStatus.Connected
                )
                {
                    ShowErrorMessage(
                       locStrings.Messages.MessageServerIsNotConnectedError
                    );
                    return;
                }
                var activeContactInfo = _messageClientModel.ActiveContactInfo;
                if (activeContactInfo == null)
                {
                    ShowErrorMessage(
                        locStrings.Messages.ContactNotSelectedError
                    );
                    return;
                }
                if (!activeContactInfo.IsUserAuthorizedWriteToMe)
                {
                    ShowErrorMessage(
                        locStrings.PermissionsLocStringsInstance.PermissionIsNotGrantedError
                    );
                    return;
                }
                /**/
                using (var cts = new CancellationTokenSource())
                {
                    var onProgressLoadTcs = new TaskCompletionSource<object>();
                    var progressForm = new ProgressCancelForm(
                        cts,
                        onProgressLoadTcs,
                        locStrings.PermissionsLocStringsInstance.RevokeProgressFormCaption
                        );
                    progressForm.Show(this);
                    await onProgressLoadTcs.Task;
                    try
                    {
                        progressForm.ReportProgress(LocStrings.CommonText.Start, 10);
                        await _messageSession.RevokeUserPermissionWriteToMe(
                            activeContactInfo.UserId,
                            cts.Token
                            );
                        progressForm.ReportProgress(LocStrings.CommonText.Finish, 100);
                        UpdateContactInfoPermissions();
                    }
                    finally
                    {
                        progressForm.SetProgressComplete();
                    }
                }
            });
        }
        // Get permission write to user
        private void authorizeMeWriteToUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MessageServerLocStringsInstance;
                if (
                    _messageClientModel.ConnectionStatus
                    != MessageSesionConnectionStatus.Connected
                )
                {
                    ShowErrorMessage(
                        locStrings.Messages.MessageServerIsNotConnectedError
                    );
                    return;
                }
                var activeContactInfo = _messageClientModel.ActiveContactInfo;
                if (activeContactInfo == null)
                {
                    ShowErrorMessage(
                        locStrings.Messages.ContactNotSelectedError
                    );
                    return;
                }
                if (activeContactInfo.AmIAuthorizedWriteToUser)
                {
                    ShowErrorMessage(
                        locStrings.PermissionsLocStringsInstance.PermissionAlreadyGrantedError
                    );
                    return;
                }
                /**/
                var permissionGuidForm = new InputBoxForm(
                    locStrings.PermissionsLocStringsInstance
                        .AuthorizeMePermissionGuidRequestFormCaption,
                    "",
                    @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}"
                );
                await permissionGuidForm.ShowFormAsync(this);
                var permissionGuidString =
                    await permissionGuidForm.TaskValue;
                Guid permissionGuid;
                if (!Guid.TryParse(permissionGuidString, out permissionGuid))
                {
                    ShowErrorMessage(
                        locStrings.PermissionsLocStringsInstance.ParsePermissionGuidError
                    );
                    return;
                }

                /**/
                using (var cts = new CancellationTokenSource())
                {
                    var onProgressLoadTcs = new TaskCompletionSource<object>();
                    var progressForm = new ProgressCancelForm(
                        cts,
                        onProgressLoadTcs,
                        locStrings.PermissionsLocStringsInstance.GetPermissionProgressFormCaption
                        );
                    progressForm.Show(this);
                    await onProgressLoadTcs.Task;
                    try
                    {
                        progressForm.ReportProgress(LocStrings.CommonText.Start, 10);
                        await _messageSession.GetPermissionWriteToUser(
                            activeContactInfo.UserId,
                            permissionGuid,
                            cts.Token
                            );
                        progressForm.ReportProgress(LocStrings.CommonText.Finish, 100);
                        UpdateContactInfoPermissions();
                    }
                    finally
                    {
                        progressForm.SetProgressComplete();
                    }
                }
            });
        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                textBox1.SelectAll();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateContactListViews();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateContactListViews();
        }

        private void addToFavoriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(
                () =>
                {
                    var contactInfo = _messageClientModel.ActiveContactInfo;
                    if (contactInfo != null)
                    {
                        contactInfo.IsFavorite = true;
                        MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                            _messageClientModel.Settings,
                            x => x.ContactInfoList
                        );
                    }
                }
            );
        }
        private void removeFromFavoriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(
                () =>
                {
                    var senderLabel = (Label)(((ContextMenuStrip)(((ToolStripMenuItem)sender).Owner)).SourceControl);
                    var contactInfo = _messageClientModel.Settings.ContactInfoList.First(
                        x => x.UserId == ((Guid) (senderLabel.Tag)));
                    contactInfo.IsFavorite = false;
                    UpdateContactListViews();
                }
            );
        }
        /*Favorite message contact click*/
        private void FavoriteContactLabelOnClick(object sender, EventArgs eventArgs)
        {
            HandleControlActionProper(() =>
            {
                var contactGuid = (Guid)(((Label)sender).Tag);
                var contactInfo
                    = _messageClientModel.Settings.ContactInfoList
                        .First(x => x.UserId == contactGuid);
                _messageClientModel.ActiveContactInfo = contactInfo;
            });
        }
    }
}
