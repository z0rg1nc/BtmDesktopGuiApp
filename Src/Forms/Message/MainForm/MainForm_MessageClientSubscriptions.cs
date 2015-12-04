using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Message;
using BtmI2p.AesHelper;
using BtmI2p.BitMoneyClient.Lib.MessageServerSession;
using BtmI2p.CryptFile.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private void InitMessageSessionModelSubscription(
            IMessageServerSessionModel sessionModel
        )
        {
            var locStrings = LocStrings.MessageServerLocStringsInstance;
            _userSessionSubscriptions.AddRange(
                new[]{
                    sessionModel.PropertyChangedSubject.Subscribe(
                        x =>  BeginInvokeProper(
                            async () =>
                            {
                                if (
                                    x.PropertyName 
                                    == sessionModel.MyNameOfProperty(e => e.OnlineContactGuids)
                                )
                                {
                                    if(checkBox1.Checked)
                                        UpdateContactListViews();
                                    else
                                        UpdateContactsStatus();
                                }
                                else if (
                                    x.PropertyName 
                                    == sessionModel.MyNameOfProperty(e => e.OfflineMessages)
                                )
                                {
                                    var offlineMessages =
                                        (Queue<PreparedToSendMessage>)x.CastedNewProperty;
                                    var activeContactInfo = _messageClientModel.ActiveContactInfo;
                                    var contactOfflineMessageCount =
                                        activeContactInfo != null
                                        ? offlineMessages.Count(
                                            y =>
                                                y.UserTo == activeContactInfo.UserId
                                        )
                                        : 0;
                                    label4.Text =
                                        string.Format("{0}", contactOfflineMessageCount);
                                }
                                else if (
                                    x.PropertyName
                                    == sessionModel.MyNameOfProperty(e => e.Balance)
                                    )
                                {
                                    label17.Text = string.Format(
                                        "{0:0.00}", 
                                        (decimal) x.CastedNewProperty
                                    );
                                }
                            }
                        )
                    ),
                    sessionModel.NeedToUpdateContactInfos.Subscribe(x => BeginInvokeProper(
                        async () => await UpdateContactInfoPermissionsFromServer(_cts.Token)
                    )),
                    sessionModel.SendingMessageError
                        .Throttle(TimeSpan.FromSeconds(5.0d))
                        .Subscribe(
                            i => sessionModel.NeedToUpdateContactInfos.OnNext(null)
                        ),
                    sessionModel.SendingMessageError.Subscribe(
                        x => BeginInvokeProper(
                            async () => ShowErrorMessage(
                                locStrings.Messages.SendMessageError.Inject(
                                    new
                                    {
                                        ErrorMessage = x.Item2
                                    }
                                )
                            )
                        )
                    ),
                    sessionModel.OutcomeMessageSent.Subscribe(
                        x => BeginInvokeProper(async () =>
                            {
                                richTextBox1.SuspendLayout();
                                try
                                {
                                    await _messageClientModel.MessageHistoryInstance
                                        .AddTextMessages(
                                            x
                                        );
                                    foreach (var outcomeMessage in x)
                                    {
                                        if (_messageClientModel.Settings.ContactInfoList.All(y => y.UserId != outcomeMessage.UserTo))
                                        {
                                            _messageClientModel.Settings.ContactInfoList.Add(
                                                new ContactInfo()
                                                {
                                                    UserId = outcomeMessage.UserTo,
                                                    Alias =
                                                        outcomeMessage
                                                            .UserTo
                                                            .ToString()
                                                            .Substring(0, 8)
                                                }
                                            );
                                            MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                                                _messageClientModel.Settings,
                                                e => e.ContactInfoList
                                            );
                                        }
                                        if (
                                            _messageClientModel.ActiveContactInfo != null
                                            && 
                                            _messageClientModel.ActiveContactInfo.UserId
                                                == outcomeMessage.UserTo
                                        )
                                        {
                                            await AddOutcomeMessageToRichTextBox(outcomeMessage);
                                        }
                                    }

                                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                                    richTextBox1.ScrollToCaret();
                                }
                                finally
                                {
                                    richTextBox1.ResumeLayout(true);
                                }
                            }
                        )
                    ),
                    sessionModel.IncomeMessageReceived.Subscribe(
                    x => BeginInvokeProper(async () =>
                    {
                        richTextBox1.SuspendLayout();
                        try
                        {
                            await _messageClientModel.MessageHistoryInstance
                                .AddTextMessages(
                                    x
                                );
                            bool updateContactList = false;
                            bool updatePermissions = false;
                            foreach (var incomeMessage in x)
                            {
                                if (
                                    _messageClientModel.Settings.ContactInfoList.All(
                                        y => y.UserId != incomeMessage.UserFrom
                                    )
                                )
                                {
                                    _messageClientModel.Settings.ContactInfoList.Add(
                                        new ContactInfo()
                                        {
                                            UserId = incomeMessage.UserFrom,
                                            Alias =
                                                incomeMessage
                                                    .UserFrom
                                                    .ToString()
                                                    .Substring(0, 8)
                                        }
                                    );
                                    updateContactList = true;
                                    updatePermissions = true;
                                }
                                if (
                                    _messageClientModel.ActiveContactInfo != null
                                    && _messageClientModel.ActiveContactInfo.UserId
                                        == incomeMessage.UserFrom
                                )
                                {
                                    await AddIncomeMessageToRichTextBox(
                                        incomeMessage
                                    );
                                }
                                else
                                {
                                    var contactInfo
                                        = _messageClientModel.Settings.ContactInfoList
                                            .FirstOrDefault(y => y.UserId == incomeMessage.UserFrom);
                                    if (contactInfo != null)
                                    {
                                        contactInfo.UnreadMessagesCount++;
                                        updateContactList = true;
                                    }
                                }
                            }
                            if (updateContactList)
                            {
                                MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                                    _messageClientModel.Settings,
                                    e => e.ContactInfoList
                                );
                            }
                            if (updatePermissions)
                                UpdateContactInfoPermissions();
                            richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            richTextBox1.ScrollToCaret();
                        }
                        finally
                        {
                            richTextBox1.ResumeLayout(true);
                        }
                    }))
                }
            );
        }

        private void SetControlPropertiesFromContactInfo(
            dynamic control, 
            ContactInfo contactInfo,
            List<Guid> onlineContacts 
        )
        {
            var text = contactInfo.Alias;
            if (contactInfo.UnreadMessagesCount > 0)
            {
                text += string.Format(" ({0})", contactInfo.UnreadMessagesCount);
                control.Font = new Font(control.Font, FontStyle.Bold);
            }
            else
            {
                control.Font = new Font(control.Font, FontStyle.Regular);
            }
            if (
                !contactInfo.IsUserAuthorizedWriteToMe
                || !contactInfo.AmIAuthorizedWriteToUser
                )
            {
                text +=
                    " "
                    + (contactInfo.AmIAuthorizedWriteToUser ? "+" : "-")
                    + (contactInfo.IsUserAuthorizedWriteToMe ? "+" : "-");
            }
            control.Text = text;
            control.ForeColor
                = onlineContacts.Contains(contactInfo.UserId)
                    ? Color.Green
                    : Color.Red;
        }
        
        private void UpdateContactsStatus()
        {
            HandleControlActionProper(
                async () =>
                {
                    var contactInfosDict 
                        = _messageClientModel.Settings.ContactInfoList
                            .ToDictionary(x => x.UserId);
                    var onlineContacts = _messageClientModel.SessionModel.OnlineContactGuids.ToList();
                    foreach (ListViewItem item in messageContactsListView.Items)
                    {
                        var itemUserGuid = (Guid) item.Tag;
                        if(!contactInfosDict.ContainsKey(itemUserGuid))
                            continue;
                        var contactInfo = contactInfosDict[itemUserGuid];
                        SetControlPropertiesFromContactInfo(
                            item,
                            contactInfo,
                            onlineContacts
                        );
                    }
                    messageContactsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    foreach (Control control in flowLayoutPanel3.Controls)
                    {
                        var itemUserGuid = (Guid)control.Tag;
                        if (!contactInfosDict.ContainsKey(itemUserGuid))
                            continue;
                        var contactInfo = contactInfosDict[itemUserGuid];
                        SetControlPropertiesFromContactInfo(
                            control,
                            contactInfo,
                            onlineContacts
                        );
                    }
                }
            );
        }

        private void UpdateContactListViews()
        {
            HandleControlActionProper(
                async () =>
                {
                    var contactInfos = _messageClientModel.Settings.ContactInfoList.ToList();
                    var onlineContacts = _messageClientModel.SessionModel.OnlineContactGuids.ToList();
                    bool onlineOnly = checkBox1.Checked;
                    bool newMessagesOnly = checkBox2.Checked;
                    var activeContact = _messageClientModel.ActiveContactInfo;
                    var contactsToShow = contactInfos
                        .Where(x => !onlineOnly || onlineContacts.Contains(x.UserId))
                        .Where(x => !newMessagesOnly || x.UnreadMessagesCount > 0)
                        .ToList();
                    /**/
                    var currentContactsToShow 
                        = messageContactsListView.Items.Cast<ListViewItem>().Select(x => (Guid) x.Tag);
                    if (
                        !contactsToShow.Select(x => x.UserId)
                        .SequenceEqual(currentContactsToShow)
                    )
                    {
                        messageContactsListView.SuspendLayout();
                        try
                        {
                            messageContactsListView.Items.Clear();
                            foreach (var contactInfo in contactsToShow)
                            {
                                var newItem = new ListViewItem();
                                newItem.Font = messageContactsListView.Font;
                                newItem.Tag = contactInfo.UserId;
                                messageContactsListView.Items.Add(newItem);
                                if (activeContact != null && activeContact.UserId == contactInfo.UserId)
                                    newItem.Selected = true;
                            }
                        }
                        finally
                        {
                            messageContactsListView.ResumeLayout();
                        }
                    }
                    /**/
                    var favoriteContacts = contactInfos.Where(x => x.IsFavorite).ToList();
                    var currentFavoriteContacts
                        = flowLayoutPanel3.Controls.Cast<Label>().Select(x => (Guid) x.Tag);
                    if (
                        !favoriteContacts.Select(x => x.UserId)
                            .SequenceEqual(currentFavoriteContacts)
                    )
                    {
                        flowLayoutPanel3.SuspendLayout();
                        try
                        {
                            flowLayoutPanel3.Controls.Clear();
                            foreach (var favoriteContact in favoriteContacts)
                            {
                                var label = new Label();
                                label.Font = this.Font;
                                label.Tag = favoriteContact.UserId;
                                label.BorderStyle = BorderStyle.FixedSingle;
                                label.Click += FavoriteContactLabelOnClick;
                                label.ContextMenuStrip = contextMenu_FavoriteLabel;
                                label.AutoSize = true;
                                flowLayoutPanel3.Controls.Add(label);
                            }
                        }
                        finally
                        {
                            flowLayoutPanel3.ResumeLayout();
                        }
                    }
                    UpdateContactsStatus();
                }
            );
        }
        

        private static readonly SemaphoreSlim _saveSettingsLockSem
            = new SemaphoreSlim(1);
        private void InitMessageClientSettingsSubscription(
            IMessageClientSettings settings,
            string settingsFileName,
            AesProtectedByteArray settingsFilePass
            )
        {
            _userSessionSubscriptions.Add(
                settings.PropertyChangedSubject
                    .Throttle(TimeSpan.FromSeconds(1.0d))
                    .Subscribe(
                        x => BeginInvokeProper(
                            async () =>
                            {
                                using (await _saveSettingsLockSem.GetDisposable())
                                {
                                    using (var pass = settingsFilePass.TempData)
                                    {
                                        ScryptPassEncryptedData.WriteToFile(
                                            settings,
                                            settingsFileName,
                                            pass.Data
                                        );
                                    }
                                }
                            }
                        )
                    )
            );
            _userSessionSubscriptions.Add(
                settings.PropertyChangedSubject
                    .Subscribe(
                        x => BeginInvokeProper(
                            async () =>
                            {
                                if (
                                    x.PropertyName
                                    == settings.MyNameOfProperty(e => e.ContactInfoList)
                                )
                                {
                                    /**/
                                    var contactInfos =
                                        settings.ContactInfoList;
                                    if (_messageSession != null)
                                        await _messageSession.SetContactsList(
                                            contactInfos.Select(y => y.UserId).ToList()
                                        );
                                    if (
                                        _messageClientModel.ActiveContactInfo != null
                                        &&
                                        contactInfos.All(y =>
                                            y.UserId
                                            !=
                                            _messageClientModel.ActiveContactInfo.UserId)
                                        )
                                    {
                                        _messageClientModel.ActiveContactInfo = null;
                                    }
                                    UpdateContactListViews();
                                }
                            }
                        )
                    )
            );
        }

        private void InitMessageServerSubscriptions()
        {
            _formSubscriptions.Add(
                _textBox1TestChangedSubject
                    .Throttle(TimeSpan.FromSeconds(2.0f))
                    .Subscribe(
                        i => UpdateMessageFee()
                    )
            );
            _formSubscriptions.Add(
                _messageClientModel.PropertyChangedSubject.Subscribe(x => BeginInvokeProper(
                    async () =>
                    {
                        if (
                            x.PropertyName 
                            == _messageClientModel.MyNameOfProperty(e => e.ConnectionStatus)
                            || x.PropertyName 
                            == _messageClientModel.MyNameOfProperty(e => e.UserId)
                            || x.PropertyName 
                            == _messageClientModel.MyNameOfProperty(e => e.ProfileName)
                            )
                        {
                            UpdateUserStatusToolStrip();
                        }
                        else if (
                            x.PropertyName 
                            == _messageClientModel.MyNameOfProperty(e => e.ActiveContactInfo)
                        )
                        {
                            richTextBox1.Clear();
                            var activeContactInfo = _messageClientModel.ActiveContactInfo;
                            if (activeContactInfo != null)
                            {
                                var mergedMessages =
                                    (await _messageClientModel.MessageHistoryInstance.GetLastTextMessages(
                                        activeContactInfo.UserId,
                                        _messageClientModel.Profile.UserCert.Id
                                        )).OrderBy(
                                            y =>
                                                (!y.OutcomeMessage)
                                                    ? y.SentTime
                                                      + TimeSpan.FromMilliseconds(1.0f)
                                                    : y.SentTime
                                        );
                                richTextBox1.SuspendLayout();
                                try
                                {
                                    foreach (var message in mergedMessages)
                                    {
                                        if (!message.OutcomeMessage)
                                        {
                                            await AddIncomeMessageToRichTextBox(
                                                message
                                            );
                                        }
                                        else
                                        {
                                            await AddOutcomeMessageToRichTextBox(
                                                message
                                            );
                                        }
                                    }
                                    richTextBox1.SelectionStart
                                        = richTextBox1.Text.Length;
                                    richTextBox1.ScrollToCaret();
                                }
                                finally
                                {
                                    richTextBox1.ResumeLayout(false);
                                }
                                foreach (ListViewItem item in messageContactsListView.Items)
                                {
                                    var userId = (Guid)item.Tag;
                                    if (
                                        userId ==
                                        _messageClientModel.ActiveContactInfo.UserId
                                        )
                                    {
                                        item.ImageIndex = -1;
                                    }
                                }
                                textBox1.BackColor = activeContactInfo.AmIAuthorizedWriteToUser
                                    ? Color.White
                                    : Color.LightPink;
                                if (activeContactInfo.UnreadMessagesCount > 0)
                                {
                                    activeContactInfo.UnreadMessagesCount = 0;
                                    MyNotifyPropertyChangedArgs.RaiseProperyChanged(
                                        _messageClientModel.Settings,
                                        y => y.ContactInfoList
                                    );
                                    UpdateContactsStatus();
                                }
                            }
                            else
                            {
                                textBox1.BackColor = Color.White;
                            }
                            UpdateMessageFee();
                        }
                    },
                    "_userModel.PropertyChangedSubject.Subscribe"
                    ))
            );
               
        }

        private void UpdateUserStatusToolStrip()
        {
            if (
                _messageClientModel.ConnectionStatus 
                == MessageSesionConnectionStatus.Connected
            )
                toolStripStatusLabel4.ForeColor = Color.ForestGreen;
            else if (
                _messageClientModel.ConnectionStatus
                == MessageSesionConnectionStatus.Disconnected
                )
                toolStripStatusLabel4.ForeColor = Color.Black;
            else
            {
                toolStripStatusLabel4.ForeColor = Color.Orange;
            }
            if (
                _messageClientModel.ConnectionStatus 
                == MessageSesionConnectionStatus.Disconnected
            )
            {
                toolStripStatusLabel4.Text 
                    = LocStrings.MessageServerLocStringsInstance.StatusNotLogged;
            }
            else
            {
                toolStripStatusLabel4.Text =
                    LocStrings.MessageServerLocStringsInstance.StatusLogged.Inject(
                        new
                        {
                            ProfileName = _messageClientModel.ProfileName,
                            ConectionStatus =
                                _messageClientModel.ConnectionStatus
                                == MessageSesionConnectionStatus.Connected
                                    ? LocStrings.CommonText.Logged
                                    : _messageClientModel.ConnectionStatus
                                      == MessageSesionConnectionStatus.Connecting
                                        ? LocStrings.CommonText.Connecting
                                        : LocStrings.CommonText.Disconnecting
                        }
                    );
            }
        }
    }
}
