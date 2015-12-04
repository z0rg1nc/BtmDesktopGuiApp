using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.Mining;
using BtmI2p.BitMoneyClient.Gui.Forms.Mining;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.ComputableTaskInterfaces.Client;
using BtmI2p.CryptFile.Lib;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.GeneralClientInterfaces;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        // Add job
        private void button2_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MiningServerLocStringsInstance;
                if (!_miningModel.MiningServerConnected)
                {
                    ShowErrorMessage(
                        locStrings.Messages.MiningServerIsNotConnectedError
                    );
                    return;
                }
                string jobName = textBox2.Text;
                if (string.IsNullOrWhiteSpace(jobName))
                {
                    ShowErrorMessage(
                        locStrings.AddJobLocStringsInstance.EmptyJobNameError
                    );
                    return;
                }
                long amount;
                if (!long.TryParse(textBox3.Text, out amount))
                {
                    ShowErrorMessage(
                        locStrings.AddJobLocStringsInstance.ParseAmountError
                    );
                    return;
                }
                if (amount <= 0)
                {
                    ShowErrorMessage(
                        locStrings.AddJobLocStringsInstance.AmountLessThanZeroError
                    );
                    return;
                }
                if (comboBox2.SelectedIndex == -1)
                {
                    ShowErrorMessage(
                        locStrings.AddJobLocStringsInstance.TaskTypeNotSelectedError
                    );
                    return;
                }
                var selectedTaskType = (ETaskTypes) comboBox2.SelectedValue;
                await _miningSession.AddJob(
                    Guid.NewGuid(),
                    jobName,
                    amount,
                    selectedTaskType
                    );
            });
        }
        // Send transfer
        private async void button3_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MiningServerLocStringsInstance;
                long amount;
                if (!long.TryParse(textBox4.Text, out amount))
                {
                    ShowErrorMessage(
                        locStrings.SendTransferLocStringsInstance.ParseTransferAmountError
                    );
                    return;
                }
                if (amount <= 0)
                {
                    ShowErrorMessage(
                        locStrings.SendTransferLocStringsInstance.AmountLessThanZeroError
                    );
                    return;
                }
                Guid walletTo;
                if (!Guid.TryParse(comboBox1.Text, out walletTo))
                {
                    ShowErrorMessage(
                        locStrings.SendTransferLocStringsInstance.ParseWalletToError
                    );
                    return;
                }
                if (amount > _miningModel.MiningAccountBalance)
                {
                    ShowErrorMessage(
                        LocStrings.CommonMessages.NotEnoughFundsError
                    );
                    return;
                }
                /**/
                var tempMiningCertPassBytes 
                    = await EnterPasswordForm.CreateAndReturnResult(
                        locStrings.SendTransferLocStringsInstance
                            .EnterMiningCertPasswordRequestText,
                        this
                    );
                if (
                    tempMiningCertPassBytes == null 
                )
                {
                    ShowErrorMessage(
                        LocStrings.CommonMessages.EmptyPasswordError
                    );
                    return;
                }
                /**/
                await _miningSession.TransferFundsToWallet(
                    Guid.NewGuid(),
                    walletTo,
                    amount,
                    tempMiningCertPassBytes
                );
            });
        }

        private async void UpdateMiningClientBalance()
        {
            HandleControlActionProper(async () =>
            {
                if (!_miningModel.MiningServerConnected)
                {
                    return;
                }
                try
                {
                    _miningModel.MiningAccountBalance =
                        await _miningSession.GetBalance();
                }
                catch (TimeoutException)
                {
                    return;
                }
            });
        }

        private void updateBalanceToolStripMenuItem1_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(() =>
            {
                if (!_miningModel.MiningServerConnected)
                {
                    ShowErrorMessage(
                        LocStrings.MiningServerLocStringsInstance
                            .Messages.MiningServerIsNotConnectedError
                    );
                    return;
                }
            });
            UpdateMiningClientBalance();
        }
        private void listView2_SelectedIndexChanged(
            object sender, EventArgs e
            )
        {
        }
        private void regisnterNewAccountToolStripMenuItem_Click(
            object sender, EventArgs e
            )
        {
            HandleControlActionProper(async () =>
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
                    Forms.MainForm.ClientGuiMainForm.GlobalModelInstance
                        .CommonPublicSettings
                        .BalanceRestrictions
                        .RegisterMiningServerMinBalance
                    )
                {
                    ShowErrorMessage(
                        LocStrings.MiningServerLocStringsInstance
                        .RegistrationLocStringsInstance.RegistrationLowBalanceError.Inject(
                            new
                            {
                                BalanceLimit 
                                    = Forms.MainForm.ClientGuiMainForm.GlobalModelInstance
                                        .CommonPublicSettings
                                        .BalanceRestrictions
                                        .RegisterMiningServerMinBalance
                            }
                        )
                    );
                    return;
                }
                await (new RegisterMiningClientForm(_proxySession)).ShowFormAsync(this);
            });
        }

        private async Task ConnectMiningSession()
        {
            var locStrings = LocStrings.MiningServerLocStringsInstance;
            using (await _miningActionLockSem.GetDisposable())
            {
                using (GetMiningActionInProgressDisposable())
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
                    using (await _miningModel.LockSem.GetDisposable())
                    {
                        if (_miningModel.MiningServerConnected)
                        {
                            ShowErrorMessage(
                                locStrings.Messages.MiningServerConnectedError
                            );
                            return;
                        }
                    }
                    if (
                        _proxyModel.ProxySessionModel.Balance
                        <=
                        GlobalModelInstance
                            .CommonPublicSettings
                            .BalanceRestrictions
                            .ConnectMiningServerMinBalance
                        )
                    {
                        ShowErrorMessage(
                            locStrings.ConnectingLocStringsInstance
                                .ConnectingLowBalanceError.Inject(
                                    new
                                    {
                                        BalanceLimit 
                                            = GlobalModelInstance
                                                .CommonPublicSettings
                                                .BalanceRestrictions
                                                .ConnectMiningServerMinBalance 
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
                            locStrings.ConnectingLocStringsInstance.ProgressFormCaption
                            );
                        progressForm.Show(this);
                        await onProgressLoadTcs.Task;
                        try
                        {
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport1,
                                0
                                );
                            DefaultFolders.CreateFoldersIfNotExist();
                            var dirInfo
                                = new DirectoryInfo(
                                    DefaultFolders.MiningProfilesFolder
                                    );
                            var miningProfileFileList = dirInfo.GetFiles(
                                "*.aes256",
                                SearchOption.TopDirectoryOnly
                                );
                            if (miningProfileFileList.Length == 0)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.NoProfileFilesFoundError
                                    );
                                return;
                            }
                            var miningProfileTcs = new TaskCompletionSource<FileInfo>();
                            await (new SelectProfileForm(
                                miningProfileTcs,
                                miningProfileFileList,
                                locStrings.ConnectingLocStringsInstance.SelectMiningProfileFormCaption
                                )).ShowFormAsync(this);
                            var miningProfileFileInfo = await miningProfileTcs.Task;
                            if (miningProfileFileInfo == null)
                            {
                                return;
                            }
                            var passForm = new EnterPasswordForm(
                                LocStrings.CommonText.EnterProfileFilePasswordRequest
                                );
                            await passForm.ShowFormAsync(this);
                            var miningProfileFilePassword
                                = await passForm.Result;
                            if (miningProfileFilePassword == null)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                    );
                                return;
                            }
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport2,
                                25
                                );
                            MiningClientProfile miningProfile;
                            try
                            {
                                miningProfile =
                                    ScryptPassEncryptedData
                                        .ReadFromFile<MiningClientProfile>(
                                            miningProfileFileInfo.FullName,
                                            miningProfileFilePassword
                                        );
                            }
                            catch (EnumException<ScryptPassEncryptedData.EGetValueT1ErrCodes> enumExc)
                            {
                                MiscFuncs.HandleUnexpectedError(enumExc, _log);
                                if (enumExc.ExceptionCode == ScryptPassEncryptedData.EGetValueT1ErrCodes.WrongPassword)
                                {
                                    ShowErrorMessage(
                                        LocStrings.CommonMessages.WrongPasswordError
                                        );
                                    return;
                                }
                                throw;
                            }
                            try
                            {
                                miningProfile.CheckMe();
                            }
                            catch (Exception exc)
                            {
                                MiscFuncs.HandleUnexpectedError(exc, _log);
                                ShowErrorMessage(
                                    locStrings.ConnectingLocStringsInstance
                                        .ProfileNotValid.Inject(
                                            new
                                            {
                                                ErrorMessage = exc.Message
                                            }
                                        )
                                    );
                                return;
                            }
                            var miningSettings =
                                ScryptPassEncryptedData
                                    .ReadFromFile<MiningClientSettings>(
                                        miningProfile.GetSettingsFilePath(),
                                        miningProfile.SettingsPass
                                    );
                            try
                            {
                                miningSettings.CheckMe();
                            }
                            catch (Exception exc)
                            {
                                ShowErrorMessage(
                                    locStrings.ConnectingLocStringsInstance
                                        .SettingsNotValid.Inject(
                                            new
                                            {
                                                ErrorMessage = exc.Message
                                            }
                                        )
                                    );
                                return;
                            }
                            var miningClientCertPassword
                                = await EnterPasswordForm.CreateAndReturnResult(
                                    LocStrings.CommonText.EnterCertPasswordRequest,
                                    this
                                    );
                            if (miningClientCertPassword == null)
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.EmptyPasswordError
                                    );
                                return;
                            }
                            try
                            {
                                using (var tempPass = miningClientCertPassword.TempData)
                                {
                                    miningProfile.MiningClientCert.CheckMe(
                                        true,
                                        tempPass.Data
                                        );
                                }
                                if (
                                    !LightCertificateRestrictions.IsValid(
                                        miningProfile.MiningClientCert
                                        )
                                    )
                                    throw new ArgumentOutOfRangeException(
                                        MyNameof.GetLocalVarName(
                                            () => miningProfile.MiningClientCert
                                            )
                                        );
                            }
                            catch
                            {
                                ShowErrorMessage(
                                    LocStrings.CommonMessages.WrongPasswordError
                                    );
                                return;
                            }
                            cts.Token.ThrowIfCancellationRequested();
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport3,
                                50
                                );
                            /**/
                            var miningServerSessionSettings
                                = new MiningServerSessionSettings()
                                {
                                    MiningClientCert = miningProfile.MiningClientCert
                                };
                            _miningSession = await MiningServerSession.CreateInstance(
                                miningServerSessionSettings,
                                _miningModel.SessionModel,
                                _proxySession,
                                miningClientCertPassword,
                                _miningManager,
                                cts.Token
                                );
                            using (await _miningModel.LockSem.GetDisposable())
                            {
                                _miningModel.MiningServerConnected = true;
                                _miningModel.MiningClientGuid =
                                    miningProfile.MiningClientCert.Id;
                                _miningModel.Profile = miningProfile;
                                _miningModel.Settings = miningSettings;
                                _miningModel.SettingsFilePath =
                                    miningProfile.GetSettingsFilePath();
                                _miningModel.MiningClientProfileName
                                    = miningProfile.ProfileName;
                                _miningModel.SettingsFilenamePassBytes =
                                    miningProfile.SettingsPass;
                            }
                            /**/
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport4,
                                60
                                );
                            miningSettings.TransferToInfos.ToList().AsParallel()
                                .ForAll(
                                    async transferToInfo =>
                                    {
                                        await _miningSession.TransferFundsToWallet(
                                            transferToInfo.TransferGuid,
                                            transferToInfo.WaletTo,
                                            transferToInfo.Amount,
                                            miningClientCertPassword
                                            );
                                    }
                                );
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport5,
                                70
                                );
                            miningSettings.TaskSolutionsToPass.ToList().AsParallel()
                                .ForAll(
                                    async taskSolution =>
                                    {
                                        await _miningSession.PassTaskSolution(
                                            taskSolution
                                            );
                                    }
                                );
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport6,
                                80
                                );
                            miningSettings.MiningJobInfos.ToList().AsParallel()
                                .ForAll(
                                    async jobInfo =>
                                    {
                                        await _miningSession.AddJob(
                                            jobInfo.JobGuid,
                                            jobInfo.JobName,
                                            jobInfo.WishfulTotalGain,
                                            jobInfo.TaskType,
                                            jobInfo.MinedGain
                                            );
                                    }
                                );
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport7,
                                90
                                );
                            try
                            {
                                var initBalance =
                                    await _miningSession.GetBalance().ThrowIfCancelled(cts.Token);
                                using (await _miningModel.LockSem.GetDisposable())
                                {
                                    _miningModel.MiningAccountBalance = initBalance;
                                }
                            }
                            catch (TimeoutException)
                            {
                            }
                            progressForm.ReportProgress(
                                locStrings.ConnectingLocStringsInstance.ProgressFormReport8,
                                100
                                );
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception exc)
                        {
                            _log.Error(
                                "Connect mining client error '{0}'",
                                exc.ToString()
                                );
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
                            progressForm.SetProgressComplete();
                        }
                    }
                }
            }
        }
        private readonly SemaphoreSlim _miningActionLockSem = new SemaphoreSlim(1);
        private bool _miningActionInProgress;
        private ActionDisposable GetMiningActionInProgressDisposable()
        {
            return new ActionDisposable(
                () => { _miningActionInProgress = true; },
                () => { _miningActionInProgress = false; }
            );
        }
        
        private void loginToolStripMenuItem2_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MiningServerLocStringsInstance;
                if (_miningActionInProgress)
                {
                    ShowErrorMessage(
                        locStrings.Messages.MiningServerActionInProgressError
                    );
                    return;
                }
                await ConnectMiningSession();
            });
        }

        private async Task DisconnectMiningSession(bool dontShowProgressForm = false)
        {
            var locStrings = LocStrings.MiningServerLocStringsInstance;
            using (await _miningActionLockSem.GetDisposable())
            {
                using (GetMiningActionInProgressDisposable())
                {
                    if (!_miningModel.MiningServerConnected)
                    {
                        ShowErrorMessage(
                            locStrings.Messages.MiningServerIsNotConnectedError
                        );
                        return;
                    }
                    ProgressCancelForm progressForm = null;
                    using (var cts = new CancellationTokenSource())
                    {
                        if (!dontShowProgressForm)
                        {
                            var onProgressLoadTcs = new TaskCompletionSource<object>();
                            progressForm = new ProgressCancelForm(
                                cts,
                                onProgressLoadTcs,
                                locStrings.DisconnectingLocStringsInstance.ProgressFormCaption
                                );
                            progressForm.Show(this);
                            await onProgressLoadTcs.Task;
                        }
                        try
                        {
                            await _miningSession.MyDisposeAsync();
                        }
                        finally
                        {
                            if (!dontShowProgressForm)
                            {
                                progressForm.SetProgressComplete();
                            }
                        }
                    }
                    _miningSession = null;
                    using (await _miningModel.LockSem.GetDisposable())
                    {
                        _miningModel.MiningServerConnected = false;
                        _miningModel.Settings = null;
                        _miningModel.SettingsFilePath = string.Empty;
                        _miningModel.SettingsFilenamePassBytes = null;
                    }
                }
            }
        }

        private void logoutToolStripMenuItem2_Click(
            object sender, EventArgs e
        )
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MiningServerLocStringsInstance;
                if (_miningActionInProgress)
                {
                    ShowErrorMessage(
                        locStrings.Messages.MiningServerActionInProgressError
                    );
                    return;
                }
                await DisconnectMiningSession();
            });
        }
        // Resume job
        private void button5_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MiningServerLocStringsInstance;
                if (!_miningModel.MiningServerConnected)
                {
                    ShowErrorMessage(
                        locStrings.Messages.MiningServerIsNotConnectedError
                    );
                    return;
                }
                foreach (ListViewItem item in miningJobListView.SelectedItems)
                {
                    await _miningSession.ResumeJob((Guid)item.Tag);
                }
            });
        }
        // Pause job
        private void button4_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MiningServerLocStringsInstance;
                if (!_miningModel.MiningServerConnected)
                {
                    ShowErrorMessage(
                        locStrings.Messages.MiningServerIsNotConnectedError
                    );
                    return;
                }
                foreach (ListViewItem item in miningJobListView.SelectedItems)
                {
                    await _miningSession.PauseJob((Guid)item.Tag);
                }
            });
        }
        // Remove job
        private void button1_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(async () =>
            {
                var locStrings = LocStrings.MiningServerLocStringsInstance;
                if (!_miningModel.MiningServerConnected)
                {
                    ShowErrorMessage(
                        locStrings.Messages.MiningServerIsNotConnectedError
                    );
                    return;
                }
                foreach (ListViewItem item in miningJobListView.SelectedItems)
                {
                    await _miningSession.RemoveJob((Guid)item.Tag);
                }
            });
        }

        // Change passwords
        private void changePasswordToolStripMenuItem_Click(
            object sender, EventArgs e
        )
        {
            var locStrings = LocStrings.MiningServerLocStringsInstance
                .ChangePasswordLocStringsInstance;
            ChangePasswordTemplate<MiningClientProfile>(
                new ChangePasswordTemplateLocStrings()
                {
                    NewCertPasswordRequestText = locStrings.EnterNewCertPaswordText,
                    NewMasterCertPasswordRequestText = null,
                    NewProfileFilePasswordRequestText = locStrings.EnterNewProfileFilePaswordText,
                    OldCertPasswordRequestText = locStrings.EnterOldCertPaswordText,
                    OldMasterCertPasswordRequestText = null,
                    OldProfileFilePasswordRequestText = locStrings.EnterOldProfileFilePaswordText,
                    SelectPasswordToChangeFormCaption = locStrings.SelectPasswordKindText,
                    SelectProfileFormCaption = LocStrings.MiningServerLocStringsInstance
                        .ConnectingLocStringsInstance.SelectMiningProfileFormCaption
                },
                _miningActionLockSem,
                GetMiningActionInProgressDisposable,
                DefaultFolders.MiningProfilesFolder,
                async profileName => 
                    _miningModel.MiningServerConnected
                    && _miningModel.MiningClientProfileName == profileName,
                new List<EProfilePasswordKinds>()
                {
                    EProfilePasswordKinds.Profile,
                    EProfilePasswordKinds.Cert
                },
                _ => _.MiningClientCert,
                _ => null
            );
        }
        private void copyMiningClientGUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleControlActionProper(
                () =>
                {
                    if (_miningModel.MiningServerConnected)
                    {
                        Clipboard.SetText($"{_miningModel.MiningClientGuid}");
                    }
                }
            );
        }
    }
}
