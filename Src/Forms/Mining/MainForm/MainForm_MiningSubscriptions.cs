using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private readonly SemaphoreSlim _miningViewJobListItemsLockSem
            = new SemaphoreSlim(1);
        public readonly Dictionary<Guid,ListViewItem> _miningViewJobListItems
            = new Dictionary<Guid, ListViewItem>();

        private async void OnMiningJobInfoAdded(
            IMiningJobInfo jobInfo
        )
        {
            HandleControlActionProper(async () =>
            {
                using (await _miningViewJobListItemsLockSem.GetDisposable())
                {
                    if (_miningViewJobListItems.ContainsKey(jobInfo.JobGuid))
                        return;
                    var newListViewItem = new ListViewItem(
                        string.Format(
                            "{0}#{1}\\{2}#{3}#{4}",
                            jobInfo.JobName,
                            jobInfo.MinedGain,
                            jobInfo.WishfulTotalGain,
                            jobInfo.TaskType,
                            jobInfo.Status
                        ).Split('#')
                    );
                    newListViewItem.Font = miningJobListView.Font;
                    /**/
                    newListViewItem.Tag = jobInfo.JobGuid;
                    _miningViewJobListItems.Add(
                        jobInfo.JobGuid,
                        newListViewItem
                    );
                    miningJobListView.Items.Add(newListViewItem);
                }
            });
        }
        private async void OnMiningJobInfoChanged(
            IMiningJobInfo jobInfo
        )
        {
            BeginInvokeProper(async () =>
            {
                using (await _miningViewJobListItemsLockSem.GetDisposable())
                {
                    if (!_miningViewJobListItems.ContainsKey(jobInfo.JobGuid))
                        return;
                    var currentListItem =
                        _miningViewJobListItems[jobInfo.JobGuid];
                    currentListItem.SubItems[1].Text =
                        string.Format(
                            "{0}\\{1}",
                            jobInfo.MinedGain,
                            jobInfo.WishfulTotalGain
                        );
                    currentListItem.SubItems[3].Text
                        = string.Format(
                            "{0}", jobInfo.Status
                        );
                }
            });
        }

        public async void OnMiningJobInfoRemoved(
            Guid jobGuid
            )
        {
            BeginInvokeProper(async () =>
            {
                using (await _miningViewJobListItemsLockSem.GetDisposable())
                {
                    if (!_miningViewJobListItems.ContainsKey(jobGuid))
                        return;
                    miningJobListView.Items.Remove(
                        _miningViewJobListItems[jobGuid]
                        );
                    _miningViewJobListItems.Remove(jobGuid);
                }
            });
        }

        /**/
        private readonly SemaphoreSlim _miningViewTransferListItemsLockSem
            = new SemaphoreSlim(1);
        private readonly Dictionary<Guid,ListViewItem> _miningViewTransferToListItems
            = new Dictionary<Guid, ListViewItem>();
        private async void OnMiningTransferToInfoAdded(
            MiningTransferToWalletInfo transferToInfo
        )
        {
            BeginInvokeProper(async () =>
            {
                using (await _miningViewTransferListItemsLockSem.GetDisposable())
                {
                    if (
                        _miningViewTransferToListItems.ContainsKey(
                            transferToInfo.TransferGuid
                        )
                    )
                        return;
                    var newListViewItem = new ListViewItem(
                        string.Format(
                            "{0} {1}",
                            transferToInfo.Amount,
                            transferToInfo.WaletTo
                            )
                        );
                    newListViewItem.Font = miningOutTransferListView.Font;
                    miningOutTransferListView.Items.Add(
                        newListViewItem
                    );
                    _miningViewTransferToListItems.Add(
                        transferToInfo.TransferGuid,
                        newListViewItem
                    );
                }
            });
        }
        private async void OnMiningTransferToInfoRemoved(
            Guid tranferGuid
            )
        {
            BeginInvokeProper(async () =>
            {
                using (await _miningViewTransferListItemsLockSem.GetDisposable())
                {
                    if (
                        !_miningViewTransferToListItems.ContainsKey(
                            tranferGuid
                            )
                        )
                        return;
                    miningOutTransferListView.Items.Remove(
                        _miningViewTransferToListItems[tranferGuid]
                        );
                    _miningViewTransferToListItems.Remove(
                        tranferGuid
                    );
                }
            });
        }
        /**/
        private async void MiningModelPropertyChangedAction(
            MyNotifyPropertyChangedArgs args
        )
        {
            BeginInvokeProper(async () =>
            {
                if(
                    args.PropertyName 
                    == _miningModel.MyNameOfProperty(e => e.MiningServerConnected)
                )
                {
                    var miningServerConnected =
                        (bool) args.CastedNewProperty;
                    if (!miningServerConnected)
                    {
                        using (
                            await _miningViewTransferListItemsLockSem.GetDisposable()
                        )
                        {
                            foreach (
                                var transferToListItem 
                                    in _miningViewTransferToListItems
                                )
                            {
                                miningOutTransferListView.Items.Remove(
                                    transferToListItem.Value
                                );
                            }
                            _miningViewTransferToListItems.Clear();
                        }
                        using (
                            await _miningViewJobListItemsLockSem.GetDisposable()
                        )
                        {
                            foreach (
                                var viewJobListItem 
                                    in _miningViewJobListItems
                                )
                            {
                                miningJobListView.Items.Remove(
                                    viewJobListItem.Value
                                    );
                            }
                            _miningViewJobListItems.Clear();
                        }
                        _miningModel.MiningAccountBalance = 0;
                        _miningModel.MiningClientGuid = Guid.Empty;
                        _miningModel.MiningClientProfileName = string.Empty;
                    }
                }
                else if(
                    args.PropertyName 
                    == _miningModel.MyNameOfProperty(e => e.MiningClientGuid)
                )
                {
                    
                }
                else if (
                    args.PropertyName
                    == _miningModel.MyNameOfProperty(e => e.MiningClientProfileName)
                    )
                {
                    label14.Text =
                        (string) args.CastedNewProperty;
                }
                else if (
                    args.PropertyName
                    == _miningModel.MyNameOfProperty(e => e.MiningAccountBalance)
                    )
                {
                    label6.Text
                        = string.Format(
                            "{0}",
                            (long) args.CastedNewProperty
                            );
                }
            });
        }
        private void InitMiningSubscriptions()
        {
            _formSubscriptions.AddRange(
                new []
                {
                    _miningModel.SessionModel.MiningJobAdded.Subscribe(x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _miningModel.LockSem.GetDisposable())
                            {
                                if(_miningModel.Settings == null)
                                    return;
                                using (await _miningModel.Settings.LockSem.GetDisposable())
                                {
                                    _miningModel.Settings.MiningJobInfos.RemoveAll(
                                        y => y.JobGuid == x.JobGuid
                                        );
                                    _miningModel.Settings.MiningJobInfos.Add(
                                        new MiningJobInfo(x)
                                    );
                                }
                                BeginInvoke(
                                    new Action(
                                        () => OnMiningJobInfoAdded(x)
                                        )
                                    );
                                await _miningModel.Settings.SaveToFile(
                                    _miningModel.SettingsFilePath,
                                    _miningModel.SettingsFilenamePassBytes
                                    );
                            }
                        },
                        useAnyContext: true
                        )),
                    _miningModel.SessionModel.MiningJobRemoved.Subscribe(x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _miningModel.LockSem.GetDisposable())
                            {
                                if (_miningModel.Settings == null)
                                    return;
                                using (await _miningModel.Settings.LockSem.GetDisposable())
                                {
                                    _miningModel.Settings.MiningJobInfos.RemoveAll(
                                        y => y.JobGuid == x
                                        );
                                }
                                BeginInvoke(
                                    new Action(
                                        () => OnMiningJobInfoRemoved(x)
                                        )
                                    );
                                await _miningModel.Settings.SaveToFile(
                                    _miningModel.SettingsFilePath,
                                    _miningModel.SettingsFilenamePassBytes
                                    );
                            }
                        },
                        useAnyContext: true
                        )),
                    _miningModel.SessionModel.MiningJobChanged.Subscribe(x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _miningModel.LockSem.GetDisposable())
                            {
                                if (_miningModel.Settings == null)
                                    return;
                                using (await _miningModel.Settings.LockSem.GetDisposable())
                                {
                                    var miningJobInfo =
                                        _miningModel.Settings
                                            .MiningJobInfos.FirstOrDefault<IMiningJobInfo>(y =>
                                                y.JobGuid == x.JobGuid);
                                    if (miningJobInfo == null)
                                    {
                                        _miningModel.Settings.MiningJobInfos.Add(
                                            new MiningJobInfo(x));
                                    }
                                    else
                                    {
                                        miningJobInfo.MinedGain = x.MinedGain;
                                    }
                                }
                                BeginInvoke(
                                    new Action(
                                        () => OnMiningJobInfoChanged(x)
                                        )
                                    );
                                await _miningModel.Settings.SaveToFile(
                                    _miningModel.SettingsFilePath,
                                    _miningModel.SettingsFilenamePassBytes
                                    );
                            }
                        },
                        useAnyContext: true
                        )),
                    _miningModel.SessionModel.TaskSolutionAdded.Subscribe(x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _miningModel.LockSem.GetDisposable())
                            {
                                if (_miningModel.Settings == null)
                                    return;
                                using (await _miningModel.Settings.LockSem.GetDisposable())
                                {
                                    _miningModel.Settings.TaskSolutionsToPass.RemoveAll(
                                        y =>
                                            y.CommonInfo.TaskGuid
                                            == x.CommonInfo.TaskGuid
                                        );
                                    _miningModel.Settings.TaskSolutionsToPass.Add(
                                        x
                                        );
                                }
                                await _miningModel.Settings.SaveToFile(
                                    _miningModel.SettingsFilePath,
                                    _miningModel.SettingsFilenamePassBytes
                                );
                            }
                        },
                        useAnyContext: true
                        )),
                    _miningModel.SessionModel.TaskSolutionRemoved.Subscribe(x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _miningModel.LockSem.GetDisposable())
                            {
                                if (_miningModel.Settings == null)
                                    return;
                                using (await _miningModel.Settings.LockSem.GetDisposable())
                                {
                                    _miningModel.Settings.TaskSolutionsToPass.RemoveAll(
                                        y => y.CommonInfo.TaskGuid == x
                                        );
                                }
                                UpdateMiningClientBalance();
                                await _miningModel.Settings.SaveToFile(
                                    _miningModel.SettingsFilePath,
                                    _miningModel.SettingsFilenamePassBytes
                                    );
                            }
                        },
                        useAnyContext: true
                        )),
                    _miningModel.SessionModel.TransferToWalletAdded.Subscribe(x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _miningModel.LockSem.GetDisposable())
                            {
                                if (_miningModel.Settings == null)
                                    return;
                                using (await _miningModel.Settings.LockSem.GetDisposable())
                                {
                                    _miningModel.Settings.TransferToInfos.RemoveAll(
                                        y => y.TransferGuid == x.TransferGuid
                                        );
                                    _miningModel.Settings.TransferToInfos.Add(x);
                                }
                                BeginInvoke(
                                    new Action(() => OnMiningTransferToInfoAdded(x))
                                    );
                                await _miningModel.Settings.SaveToFile(
                                    _miningModel.SettingsFilePath,
                                    _miningModel.SettingsFilenamePassBytes
                                    );
                            }
                        },
                        useAnyContext: true
                        )),
                    _miningModel.SessionModel.TransferToWalletRemoved.Subscribe(x => BeginInvokeProper(
                        async () =>
                        {
                            using (await _miningModel.LockSem.GetDisposable())
                            {
                                if (_miningModel.Settings == null)
                                    return;
                                using (await _miningModel.Settings.LockSem.GetDisposable())
                                {
                                    _miningModel.Settings.TransferToInfos.RemoveAll(
                                        y => y.TransferGuid == x
                                        );
                                }
                                BeginInvoke(
                                    new Action(() => OnMiningTransferToInfoRemoved(x))
                                    );
                                UpdateMiningClientBalance();
                                await _miningModel.Settings.SaveToFile(
                                    _miningModel.SettingsFilePath,
                                    _miningModel.SettingsFilenamePassBytes
                                    );
                            }
                        },
                        useAnyContext: true
                        ))
                }
            );
            _formSubscriptions.Add(
                _miningModel.PropertyChangedSubject.Subscribe(MiningModelPropertyChangedAction)
            );
            _formSubscriptions.Add(
                _walletListFormModel.OnWalletInfoAdded.Subscribe(x => BeginInvokeProper(
                    async () => 
                        comboBox1.Items.Add(
                            x.WalletProfileInstance.WalletCert.Id.ToString()
                            )
                    ))
                );
            _formSubscriptions.Add(
                _walletListFormModel.OnWalletInfoRemoved.Subscribe(x => BeginInvokeProper(
                    async () => 
                        comboBox1.Items.Remove(
                            x.ToString()
                            )
                    ))
                );
        }
    }
}
