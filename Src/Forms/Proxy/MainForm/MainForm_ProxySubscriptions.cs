using System;
using System.Drawing;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Models;
using BtmI2p.BitMoneyClient.Lib;
using BtmI2p.MiscClientForms;
using BtmI2p.MiscUtils;
using BtmI2p.MyNotifyPropertyChanged;


namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainForm
    {
        private bool _newVersionAvailableHandled = false;
        private void InitProxySubscriptions()
        {
            _formSubscriptions.Add(
                ObservableExtensions.Subscribe<MyNotifyPropertyChangedArgs>(_proxyModel.ProxySessionModel.PropertyChangedSubject, x => BeginInvokeProper(
                        async () =>
                        {
                            var locStrings = LocStrings.ProxyServerLocStringsInstance;
                            if(
                                x.PropertyName 
                                == _proxyModel.ProxySessionModel.MyNameOfProperty(e => e.TaskComputing
                                )
                            )
                            {
                                ProxyTaskComputingSubscriptionAction(
                                    (bool) x.CastedNewProperty
                                );
                            }
                            else if (
                                x.PropertyName
                                == _proxyModel.ProxySessionModel.MyNameOfProperty(e => e.Balance
                                )
                            )
                            {
                                UpdateProxyServerStatusLabel();
                            }
                            else if (
                                x.PropertyName
                                == _proxyModel.ProxySessionModel.MyNameOfProperty(e => e.ProxyServerStatus
                                )
                            )
                            {
                                UpdateProxyServerStatusLabel();
                            }
                            else if (
                                x.PropertyName
                                == MyExtensionMethods.MyNameOfProperty<IProxyServerSessionModel, TimeSpan>(_proxyModel.ProxySessionModel, e => e.ClientServerTimeDifference
                                )
                            )
                            {
                                var castedTimeSpan = (TimeSpan) x.CastedNewProperty;
                                var toolStripText = string.Format(
                                    "{0}{1}:{2:mm}:{3:ss}",
                                    (castedTimeSpan >= TimeSpan.Zero 
                                        ? locStrings.LagOfLocalTime 
                                        : locStrings.LeadOfLocalTime),
                                    (int)Math.Abs(castedTimeSpan.TotalHours),
                                    castedTimeSpan,
                                    castedTimeSpan
                                );
                                toolStripStatusLabel5.Text = toolStripText;
                            }
                            else if (
                                x.PropertyName
                                == MyExtensionMethods.MyNameOfProperty<IProxyServerSessionModel, bool>(_proxyModel.ProxySessionModel, e => e.NewVersionAvailable
                                )
                            )
                            {
                                var newVersionAvailable = (bool) x.CastedNewProperty;
                                if (
                                    newVersionAvailable 
                                    && !_newVersionAvailableHandled
                                )
                                {
                                    _newVersionAvailableHandled = true;
                                    if (
										await MessageBoxAsync.ShowAsync(this,
                                            locStrings.Messages.NewClientVersionAvailableQuestion,
                                            locStrings.Messages.NewClientVersionAvailableCaption,
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question
                                        ) == DialogResult.Yes
                                    )
                                    {
                                        await CheckForUpdates();
                                    }
                                }
                            }
                        }
                    )
                )
            );
            _formSubscriptions.Add(
                ObservableExtensions.Subscribe<MyNotifyPropertyChangedArgs>(_proxyModel.PropertyChangedSubject, x => BeginInvokeProper(
                            async () =>
                            {
                                if (
                                    x.PropertyName 
                                    == MyExtensionMethods.MyNameOfProperty<IProxyModel, bool>(_proxyModel, e => e.ProxyServerConnected
                                    )
                                )
                                {
                                    UpdateProxyServerStatusLabel();
                                    if (!((bool) x.CastedNewProperty))
                                    {
                                        _proxyModel.ProxySessionModel.Balance = 0;
                                    }
                                    toolStripStatusLabel5.Visible = (bool)x.CastedNewProperty;
                                }
                            }
                        )
                    )
                );
        }

        private void UpdateProxyServerStatusLabel()
        {
            var locStrings = LocStrings.ProxyServerLocStringsInstance;
            toolStripStatusLabel2.ForeColor = 
                !_proxyModel.ProxyServerConnected ? Color.Black 
                    : !_proxyModel.ProxySessionModel.ProxyServerStatus ? Color.Orange 
                        : Color.ForestGreen;

            toolStripStatusLabel2.Text
                = string.Format(
                    locStrings.I2PDestinationBalance,
                    _proxyModel.ProxySessionModel.Balance
                );
        }
        
        private void ProxyTaskComputingSubscriptionAction(bool x)
        {
            toolStripStatusLabel3.Visible = x;
        }
    }
}
