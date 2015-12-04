using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.MiscUtils;
using BtmI2p.ComputableTaskInterfaces.Client;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        // Table for mining task's status 
        private readonly SemaphoreSlim _listView2LockSem = new SemaphoreSlim(1);
        private readonly Dictionary<Guid,ListViewItem> _miningTaskListViewItemDb
            = new Dictionary<Guid, ListViewItem>();
        private void InitMiningManagerSubscription()
        {
            _formSubscriptions.Add(
                ObservableExtensions.Subscribe<IMiningTaskInfo>(_miningManagerModel.OnTaskStatusChanged, x => BeginInvokeProper(
                    async () =>
                    {
                        /*
                            _logger.Trace(
                                "InitMiningManagerSubscription task status changed {0} {1}",
                                x.CommonInfo.TaskGuid,
                                x.Status
                            );
                         */
                        using (await _listView2LockSem.GetDisposable())
                        {
                            if (
                                _miningTaskListViewItemDb.ContainsKey(
                                    x.CommonInfo.TaskGuid
                                )
                            )
                            {
                                var itemToChange =
                                    _miningTaskListViewItemDb[x.CommonInfo.TaskGuid];
                                itemToChange.BackColor = 
                                    (x.Status == EMiningTaskStatus.Running) ? Color.Yellow
                                    : (x.Status == EMiningTaskStatus.Complete) ? Color.LawnGreen
                                    : (x.Status == EMiningTaskStatus.Fault) ? Color.Red 
                                    : Color.White;
                                itemToChange.SubItems[0].Text = 
                                    string.Format((string) "{0}", (object) x.Priority);
                                itemToChange.SubItems[1].Text = 
                                    string.Format((string) "{0}", (object) x.Status);
                                itemToChange.SubItems[3].Text = 
                                    string.Format((string) "{0}", (object) x.CommonInfo.BalanceGain);
                            }
                        }
                    })));
            _formSubscriptions.Add(
                ObservableExtensions.Subscribe<Guid>(_miningManagerModel.OnTaskRemoved, x => BeginInvokeProper(
                    async () =>
                    {
                        /*_logger.Trace(
                            "InitMiningManagerSubscription task removed {0}",
                            x
                        );*/
                        using (await _listView2LockSem.GetDisposable())
                        {
                            if (_miningTaskListViewItemDb.ContainsKey(x))
                            {
                                var itemToRemove = _miningTaskListViewItemDb[x];
                                miningLocalTaskListView.Items.Remove(itemToRemove);
                                _miningTaskListViewItemDb.Remove(x);
                            }
                        }
                    }
            )));
            _formSubscriptions.Add(
                ObservableExtensions.Subscribe<IMiningTaskInfo>(_miningManagerModel.OnTaskAdded, x => BeginInvokeProper(
                        async () =>
                        {
                            /*_logger.Trace(
                                "InitMiningManagerSubscription task added {0}",
                                x.CommonInfo.TaskGuid
                            );*/
                            using (await _listView2LockSem.GetDisposable())
                            {
                                var newItem = new ListViewItem(
                                    new []
                                    {
                                        string.Format((string) "{0}", (object) x.Priority),
                                        string.Format((string) "{0}", (object) x.Status),
                                        string.Format("{0}", (ETaskTypes) x.CommonInfo.TaskType),
                                        string.Format((string) "{0}", (object) x.CommonInfo.BalanceGain),
                                        string.Format((string) "{0}", (object) x.CommonInfo.TaskGuid)
                                    }
                                );
                                newItem.Font = miningLocalTaskListView.Font;
                                newItem.Tag = x;
                                _miningTaskListViewItemDb.Add(x.CommonInfo.TaskGuid, newItem);
                                miningLocalTaskListView.Items.Add(newItem);
                            }
                        }
                    )
                )
            );
        }
    }
}
