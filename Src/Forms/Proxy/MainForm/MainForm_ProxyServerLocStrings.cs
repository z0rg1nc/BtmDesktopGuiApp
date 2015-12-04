namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainFormLocStrings
    {
        public class ProxyServerLocStrings
        {
            public class MessagesLocStrings
            {
                public string ProxyServerActionInProgressError
                    = "Some proxy server action in progress, retry later";
                public string ProxyServerConnectedError
                    = "Proxy server is already connected";

                public string NewClientVersionAvailableQuestion
                    = "New client version available, update now?";
                public string NewClientVersionAvailableCaption
                    = "Update";

                public string SamConnectionFailedError
                    = "Establishing TCP connection to SAM server failed. Check SAM parameters and try again.";
            }
            public MessagesLocStrings Messages = new MessagesLocStrings();
            /**/
            public class DisconnectingLocStrings
            {
                public string WalletServerConnectedError
                    = "Wallet server connected";
                public string MiningServerConnectedError
                    = "Mining server connected";
                public string MessageServerConnectedError
                    = "Message server connected";
                public string ExchangeServerConnectedError
                    = "Exchange server connected";
                public string ProgressFormCaption
                    = "Proxy server disconnection";
                public string ProgressFormReport1
                    = "Start proxy server disconnecting";
            }
            public DisconnectingLocStrings DisconnectingLocStringsInstance
                = new DisconnectingLocStrings();
            /**/
            public class ConnectingLocStrings
            {
                public string ProgressFormCaption = "Proxy server connection";
                public string ProgressFormReport1 = "Proxy server connecting";
                public string ProgressFormReport2 = "Proxy server connected";
            }
            public ConnectingLocStrings ConnectingLocStringsInstance
                = new ConnectingLocStrings();
            /**/
            public class IssueInvoiceToRefillLocStrings
            {
                public string RefillAmount = "Refill amount";
                public string WrongTransferAmountError = "Wrong transfer amount";
            }
            public IssueInvoiceToRefillLocStrings IssueInvoiceToRefillLocStringsInstance
                = new IssueInvoiceToRefillLocStrings();
            /**/
            public string LagOfLocalTime = "Lag of ";
            public string LeadOfLocalTime = "Lead of ";
            public string I2PDestinationBalance = "I2p destination balance {0:0.00}";
        }
        public ProxyServerLocStrings ProxyServerLocStringsInstance
            = new ProxyServerLocStrings();
    }
}
