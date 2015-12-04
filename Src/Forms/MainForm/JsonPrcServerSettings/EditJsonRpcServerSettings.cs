using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers;
using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers.Wallet;
using BtmI2p.MiscUtils;
using BtmI2p.ObjectStateLib;
using NLog;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm.JsonPrcServerSettings
{
    public partial class EditJsonRpcServerSettingsForm : Form
    {
        public static EditJsonRpcServersSettingsFormLocStrings LocStrings 
            = new EditJsonRpcServersSettingsFormLocStrings();
        private readonly ICommonRpcSettings _commonRpcSettings;
        private readonly List<Guid> _walletGuidList;
        private readonly LocalRpcServersManager _rpcServersManager;
        private readonly ILocalRpcServersManagerModel _rpcServersManagerModel;
        private readonly DisposableObjectStateHelper _stateHelper
            = new DisposableObjectStateHelper("EditJsonRpcServerSettingsForm");
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        public EditJsonRpcServerSettingsForm(
            ICommonRpcSettings commonRpcSettings,
            List<Guid> walletGuidList,
            LocalRpcServersManager rpcServersManager,
            ILocalRpcServersManagerModel rpcServersManagerModel
        )
        {
            if (commonRpcSettings == null)
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => commonRpcSettings));
            if(walletGuidList == null)
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => walletGuidList));
            if(rpcServersManager == null)
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => rpcServersManager));
            if(rpcServersManagerModel == null)
                throw new ArgumentNullException(
                    MyNameof.GetLocalVarName(() => rpcServersManagerModel));
            /**/
            _rpcServersManager = rpcServersManager;
            _rpcServersManagerModel = rpcServersManagerModel;
            _walletGuidList = walletGuidList;
            _commonRpcSettings = commonRpcSettings;
            InitializeComponent();
            
        }

        //Save settings
        private void button4_Click(object sender, EventArgs e)
        {
            {
                var walletSettings = _commonRpcSettings.WalletJsonRpcSettings;
                /**/
                if (
                    ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
                        .StartAutomaticallyWalletRpcServer != autoStartCheckBox1.Checked
                )
                    ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
                        .StartAutomaticallyWalletRpcServer = autoStartCheckBox1.Checked;
                /**/
                ushort portNumber;
                if (!ushort.TryParse(portNumberTextBox1.Text, out portNumber))
                {
                    ClientGuiMainForm.ShowErrorMessage(this,
                        LocStrings.ErrorParsePortNumverMessage
                    );
                    return;
                }
                if (walletSettings.PortNumber != portNumber)
                    walletSettings.PortNumber = portNumber;
                /**/
                if (walletSettings.HostName != hostNameTextBox1.Text)
                    walletSettings.HostName = hostNameTextBox1.Text;
                /**/
                var userName = usernameTextBox1.Text;
                if (walletSettings.Username != userName)
                    walletSettings.Username = userName;
                /**/
                var password = passwordTextBox1.Text;
                if (walletSettings.Password != password)
                    walletSettings.Password = password;
                /**/
                var allowedWalletGuids = ListBox1GuidList;
                if (!walletSettings.RpcAllowedWalletGuids.SequenceEqual(allowedWalletGuids))
                    walletSettings.RpcAllowedWalletGuids = allowedWalletGuids;
                /**/
                if (walletSettings.AllowSendingPaymentsViaRpc != allowSendPaymentsRpcCheckBox1.Checked)
                    walletSettings.AllowSendingPaymentsViaRpc = allowSendPaymentsRpcCheckBox1.Checked;
                /**/
                var hmacKeyCode = Convert.FromBase64String(textBox5.Text);
                if (!walletSettings.RpcSendingPaymentsHmacKeycode.SequenceEqual(hmacKeyCode))
                    walletSettings.RpcSendingPaymentsHmacKeycode = hmacKeyCode;
                /**/
                if (walletSettings.DontCheckHmac != dontCheckHmacCheckBox1.Checked)
                    walletSettings.DontCheckHmac = dontCheckHmacCheckBox1.Checked;
            }
            {
                var proxySettings = _commonRpcSettings.ProxyJsonRpcSettings;
                /**/
                if (
                    ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
                        .StartAutomaticallyProxyRpcServer != autoStartCheckBox2.Checked
                )
                    ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
                        .StartAutomaticallyProxyRpcServer = autoStartCheckBox2.Checked;
                /**/
                ushort portNumber;
                if (!ushort.TryParse(portNumberTextBox2.Text, out portNumber))
                {
                    ClientGuiMainForm.ShowErrorMessage(this,
                        LocStrings.ErrorParsePortNumverMessage
                    );
                    return;
                }
                if (proxySettings.PortNumber != portNumber)
                    proxySettings.PortNumber = portNumber;
                /**/
                var hostName = hostNameTextBox2.Text;
                if (proxySettings.HostName != hostName)
                    proxySettings.HostName = hostName;
                /**/
                var userName = userNameTextBox2.Text;
                if (proxySettings.Username != userName)
                    proxySettings.Username = userName;
                /**/
                var password = passwordTextBox2.Text;
                if (proxySettings.Password != password)
                    proxySettings.Password = password;
            }
            Close();
        }
        //Renew username, password
        private void label7_Click(object sender, EventArgs e)
        {
            var newSettings = new WalletRpcSettings();
            usernameTextBox1.Text = newSettings.Username;
            passwordTextBox1.Text = newSettings.Password;
        }
        // Renew hmac code
        private void label9_Click(object sender, EventArgs e)
        {
            var newSettings = new WalletRpcSettings();
            textBox5.Text = Convert.ToBase64String(newSettings.RpcSendingPaymentsHmacKeycode);
        }
        // Cancel
        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }
        // Remove wallet guid
        private void label10_Click(object sender, EventArgs e)
        {
            while (listBox1.SelectedItem != null)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }

        private List<Guid> ListBox1GuidList
        {
            get
            {
                var result = new List<Guid>();
                foreach (string walletGuidS in listBox1.Items)
                {
                    result.Add(Guid.Parse(walletGuidS));
                }
                return result;
            }
        }
        // Add wallet guid
        private void button3_Click(object sender, EventArgs e)
        {
            Guid newWalletGuid;
            if (!Guid.TryParse(comboBox1.Text, out newWalletGuid))
            {
                ClientGuiMainForm.ShowErrorMessage(this, LocStrings.ParseWalletGuidErrorMessage);
                return;
            }
            if (ListBox1GuidList.Contains(newWalletGuid))
            {
                ClientGuiMainForm.ShowErrorMessage(this, LocStrings.WalletGuidAlreadyAddedMessage);
                return;
            }
            listBox1.Items.Add(newWalletGuid.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                async () =>
                {
                    await _rpcServersManager.StartWalletRpcServer();
                },
                _stateHelper,
                _log
            );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                async () =>
                {
                    await _rpcServersManager.StopWalletRpcServer();
                },
                _stateHelper,
                _log
            );
        }

        private void EditJsonRpcServerSettingsForm_Shown(object sender, EventArgs eventArgs)
        {
            {
                InitCommonView();
                _stateHelper.SetInitializedState();
                /**/
                {
                    var walletSettings = _commonRpcSettings.WalletJsonRpcSettings;
                    autoStartCheckBox1.Checked
                        = ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
                            .StartAutomaticallyWalletRpcServer;
                    portNumberTextBox1.Text = string.Format("{0}", walletSettings.PortNumber);
                    hostNameTextBox1.Text = walletSettings.HostName;
                    usernameTextBox1.Text = walletSettings.Username;
                    passwordTextBox1.Text = walletSettings.Password;
                    foreach (Guid walletGuid in walletSettings.RpcAllowedWalletGuids)
                    {
                        listBox1.Items.Add(string.Format("{0}", walletGuid));
                    }
                    allowSendPaymentsRpcCheckBox1.Checked = walletSettings.AllowSendingPaymentsViaRpc;
                    textBox5.Text = Convert.ToBase64String(
                        walletSettings.RpcSendingPaymentsHmacKeycode
                        );
                    dontCheckHmacCheckBox1.Checked = walletSettings.DontCheckHmac;
                    foreach (var guid in _walletGuidList)
                    {
                        comboBox1.Items.Add(string.Format("{0}",guid));
                    }
                    /**/
                    _rpcServersManagerModel.PropertyChangedSubject.Where(
                        x => x.PropertyName == _rpcServersManagerModel
                            .MyNameOfProperty(e => e.WalletRpcServerRunning)
                        ).Subscribe(
                            x =>
                            {
                                ClientGuiMainForm.BeginInvokeProper(
                                    this,
                                    _stateHelper,
                                    _log,
                                    async () =>
                                    {
                                        textBox1.Text =
                                            _rpcServersManagerModel.WalletRpcServerRunning
                                                ? LocStrings.RpcServerStatusWorking
                                                : LocStrings.RpcServerStatusStopped;
                                    }
                                    );
                            }
                        );
                    _rpcServersManagerModel.WalletRpcServerRunning
                        = _rpcServersManagerModel.WalletRpcServerRunning;
                }
                /**/
                {
                    var proxySettings = _commonRpcSettings.ProxyJsonRpcSettings;
                    autoStartCheckBox2.Checked
                        = ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings
                            .StartAutomaticallyProxyRpcServer;
                    portNumberTextBox2.Text = string.Format("{0}", proxySettings.PortNumber);
                    hostNameTextBox2.Text = proxySettings.HostName;
                    userNameTextBox2.Text = proxySettings.Username;
                    passwordTextBox2.Text = proxySettings.Password;
                    /**/
                    _rpcServersManagerModel.PropertyChangedSubject.Where(
                        x => x.PropertyName == _rpcServersManagerModel
                            .MyNameOfProperty(e => e.ProxyRpcServerRunning)
                        ).Subscribe(
                            x =>
                            {
                                ClientGuiMainForm.BeginInvokeProper(
                                    this,
                                    _stateHelper,
                                    _log,
                                    async () =>
                                    {
                                        textBox7.Text =
                                            _rpcServersManagerModel.ProxyRpcServerRunning
                                                ? LocStrings.RpcServerStatusWorking
                                                : LocStrings.RpcServerStatusStopped;
                                    }
                                );
                            }
                        );
                    _rpcServersManagerModel.ProxyRpcServerRunning
                        = _rpcServersManagerModel.ProxyRpcServerRunning;
                }
            }
        }

        private void startButton2_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                async () =>
                {
                    await _rpcServersManager.StartProxyRpcServer();
                },
                _stateHelper,
                _log
            );
        }

        private void stopButton2_Click(object sender, EventArgs e)
        {
            ClientGuiMainForm.HandleControlActionProper(this,
                async () =>
                {
                    await _rpcServersManager.StopProxyRpcServer();
                },
                _stateHelper,
                _log
            );
        }
    }
}
