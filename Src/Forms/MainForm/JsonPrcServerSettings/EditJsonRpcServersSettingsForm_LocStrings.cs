namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm.JsonPrcServerSettings
{
    public class EditJsonRpcServersSettingsFormLocStrings
    {
        public string ErrorParsePortNumverMessage = "Parse port number error";
        public string ParseWalletGuidErrorMessage = "Parse wallet guid error";
        public string WalletGuidAlreadyAddedMessage = "Wallet guid already added";
        public string RpcServerStatusWorking = "Working";
        public string RpcServerStatusStopped = "Stopped";
        /**/
        public string ServerStatusLabelText = "Server status:";
        public string StartButtonText = "Start";
        public string StopButtonText = "Stop";
        public string PortNumberLabelText = "Port number:";
        public string AllowRpcWalletGuidsLabelText = "Allow rpc for the next wallet GUIDs";
        public string AddButtonText = "Add";
        public string BasicHttpCredentialsLabelText = "Basic http authentication credentials:";
        public string UsernameLabelText = "Username";
        public string PasswordLabelText = "Password";
        public string RenewLabelText = "Renew";
        public string SaveSettingsButtonText = "Save settings";
        public string HmacKeyForPaymentLabelText = "HMAC-SHA256 key code for payment sending:";
        //public string Label9Text = "Renew";
        public string AllowSendPaymentsRpcCheckBoxText = "Allow rpc requests to send payments";
        public string AutoStartCheckBoxText = "Start automatically";
        public string WalletTabPageText = "Wallet";
        public string HostNameLabelText = "Host name:";
        public string RemoveLabelText = "Remove";
        public string CancelButtonText = "Cancel";
        public string TextInit = "JSON RPC server settings";
        public string DontCheckHmacCheckBoxText = "Don't check HMAC";
        /**/
        public string ProxyTabPageTest = "Proxy";
    }

    public partial class EditJsonRpcServerSettingsForm
    {
        private void InitCommonView()
        {
            serverStatusLabel1.Text = LocStrings.ServerStatusLabelText;
            startButton1.Text = LocStrings.StartButtonText;
            stopButton1.Text = LocStrings.StopButtonText;
            portNumberlabel1.Text = LocStrings.PortNumberLabelText;
            allowRpcWalletGuidsLabel1.Text = LocStrings.AllowRpcWalletGuidsLabelText;
            addButton1.Text = LocStrings.AddButtonText;
            basicHttpCredentialsLabel1.Text = LocStrings.BasicHttpCredentialsLabelText;
            usernameLabel1.Text = LocStrings.UsernameLabelText;
            passwordLabel1.Text = LocStrings.PasswordLabelText;
            renewLabel1.Text = LocStrings.RenewLabelText;
            saveSettingsButton1.Text = LocStrings.SaveSettingsButtonText;
            hmacKeyForPaymentLabel1.Text = LocStrings.HmacKeyForPaymentLabelText;
            renewLabel2.Text = LocStrings.RenewLabelText;
            allowSendPaymentsRpcCheckBox1.Text = LocStrings.AllowSendPaymentsRpcCheckBoxText;
            autoStartCheckBox1.Text = LocStrings.AutoStartCheckBoxText;
            walletTabPage1.Text = LocStrings.WalletTabPageText;
            hostNameLabel1.Text = LocStrings.HostNameLabelText;
            removeLabel1.Text = LocStrings.RemoveLabelText;
            cancelButton1.Text = LocStrings.CancelButtonText;
            dontCheckHmacCheckBox1.Text = LocStrings.DontCheckHmacCheckBoxText;
            /**/
            proxyTabPage.Text = LocStrings.ProxyTabPageTest;
            serverStatusLabel2.Text = LocStrings.ServerStatusLabelText;
            startButton2.Text = LocStrings.StartButtonText;
            stopButton2.Text = LocStrings.StopButtonText;
            autoStartCheckBox2.Text = LocStrings.AutoStartCheckBoxText;
            hostNameLabel2.Text = LocStrings.HostNameLabelText;
            portNumberLabel2.Text = LocStrings.PortNumberLabelText;
            basicHttpCredentialsLabel2.Text = LocStrings.BasicHttpCredentialsLabelText;
            renewLabel3.Text = LocStrings.RenewLabelText;
            userNameLabel2.Text = LocStrings.UsernameLabelText;
            passwordLabel2.Text = LocStrings.PasswordLabelText;
            /**/
            Text = LocStrings.TextInit;
            ClientGuiMainForm.ChangeControlFont(
                this, 
                ClientGuiMainForm.GlobalModelInstance.CommonPublicSettings.FontSizePt
            );
        }
    }
}
