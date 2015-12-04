using System.Collections.Generic;
using BtmI2p.GeneralClientInterfaces.ExchangeServer;

namespace BtmI2p.BitMoneyClient.Gui.Forms.MainForm
{
    public partial class ClientGuiMainFormLocStrings
    {
        public class ExchangeServerLocStrings
        {
            public class MessagesLocStrings
            {
                public string ExchangeServerConnectedError
                    = "Exchange server connected already";
                public string ExchangeServerIsNotConnectedError
                    = "Exchange server is not connected";
                public string ExchangeServerActionInProgressError
                    = "Some exchange server action in progress, retry later";
                public string ExchangeClientGuidCopiedInfo
                    = "Exchage client GUID saved to clipboard";
                public string NoAccountSelectedError = "No account selected";
            }
            public MessagesLocStrings Messages = new MessagesLocStrings();
            /**/
            public class RegistrationLocStrings
            {
                public string RegistrationLowBalanceError
                    = "For exchange client registration server proxy" +
                      " balance should be more than {BalanceLimit}";
            }
            public RegistrationLocStrings RegistrationLocStringsInstance
                = new RegistrationLocStrings();
            /**/
            public class RegisterNewAccountLocStrings
            {
                public string ProgressFormCaption
                    = "New exchange account registration";

                public string ReportGetCurrencyList = "Receiving currency list";
                public string SelectAcccountCurrency = "Select account currency please";
                public string NoCurrencySelectedCancel = "No currency selected, cancelling";
                public string CurrencySelected = "Currency selected";
                public string ServerCommunication = "Exchange server communication, account registration";
                public string AccountRegistered = "Account {AccountGuid} registered";
            }
            public RegisterNewAccountLocStrings RegisterNewAccountLocStringsInstance
                = new RegisterNewAccountLocStrings();
            /**/
            public class ConnectingLocStrings
            {
                public string ConnectingLowBalanceError
                    = "For connecting to exchange server proxy" +
                      " balance should be more than {BalanceLimit}";

                public string ProgressFormCaption = "Exchange client connection";
                public string ProgressFormReport1 = "User input";
                public string SelectExchangeProfileFormCaption = "Select exchange profile";
                public string ProgressFormReport2 = "Exchange client settings loading";
                public string ProfileNotValid = "Exchange profile not valid {ErrorMessage}";
                public string SettingsNotValid = "Exchange settings not valid {ErrorMessage}";
                public string ProgressFormReport3 = "Exchange client session initializing";
                public string ProgressFormReport7 = "Get initial data";
                public string ProgressFormReport8 = "Connection complete";
            }
            public ConnectingLocStrings ConnectingLocStringsInstance
                = new ConnectingLocStrings();
            /**/
            public class DisconnectingLocStrings
            {
                public string ProgressFormCaption = "Exchange server disconnection";
            }
            public DisconnectingLocStrings DisconnectingLocStringsInstance
                = new DisconnectingLocStrings();
            /**/

            public class MakeAccountDefaultLocStrings
            {
                public string ProgressFormCaption = "Make account default";
                public string Report1Text = "Server communications";
            }
            public MakeAccountDefaultLocStrings MakeAccountDefaultLocStringsInstance
                = new MakeAccountDefaultLocStrings();
            /**/
            public class CopyAccountGuidLocStrings
            {
                public string CompleteMessage = "Account GUID copied to clipboard";
            }
            public CopyAccountGuidLocStrings CopyAccountGuidLocStringsInstance
                = new CopyAccountGuidLocStrings();
            /**/
            public class ExchangeChangePasswordLocStrings
            {
                public string SelectPasswordKindText = "Exchange password to change";
                public string EnterOldProfileFilePaswordText
                    = "Enter old profile file password";
                public string EnterNewProfileFilePaswordText
                    = "Enter new profile file password";
                public string EnterOldCertPaswordText
                    = "Enter old certificate password";
                public string EnterNewCertPaswordText
                    = "Enter new certificate password";
            }
            public ExchangeChangePasswordLocStrings ExchangeChangePasswordLocStringsInstance
                = new ExchangeChangePasswordLocStrings();
            /**/
            public Dictionary<EExchangeAccountTransferType,string> TransferTypeLocStrings
                = new Dictionary<EExchangeAccountTransferType, string>()
                {
                    {EExchangeAccountTransferType.ExternalReplenishment, "External replenishment"},
                    {EExchangeAccountTransferType.ExternalWithdrawal, "External withdrawal"},
                    {EExchangeAccountTransferType.Fee, "Fee"},
                    {EExchangeAccountTransferType.InitBalance, "Initial balance"},
                    {EExchangeAccountTransferType.TradeReceived, "Trade received"},
                    {EExchangeAccountTransferType.TradeSent, "Trade sent"}
                };
            /**/
            public class ShowSecurityListLocStrings
            {
                public string ProgressFormCaption = "Receiving security list";
                public string ProgressFormReport1 = "Initial loading";
            }
            public ShowSecurityListLocStrings ShowSecurityListLocStringsInstance
                = new ShowSecurityListLocStrings();
			/**/
			public class AddNewOrderLocStrings
			{
				public string InitLoadProgressFormCaption = "Loading initial data";
				public string CurrencyListLoading = "Currency list loading";
				public string SecurityListLoading = "Security list loading";
				public string AccountListLoading = "Account list loading";
				public string AddOrderProgressFormCaption = "New order";
				public string AddOrderProgressFormReport1 = "Exchange server communication";
			}
			public AddNewOrderLocStrings AddNewOrderLocStringsInstance
				= new AddNewOrderLocStrings();
			/**/
			public class CancelOrderLocStrings
			{
			    public string OrderNotActiveErrorMessage = "Order is not active";
				public string ProgressFormCaption = "Cancelling order {OrderGuid}";
				public string ProgressFormReport1 = "Server communication";
			}
			public CancelOrderLocStrings CancelOrderLocStringsInstance
				= new CancelOrderLocStrings();
			/**/
			public class NewDepositLocStrings
	        {
		        public string InputBoxCaption = "Enter deposit value";
				public string ProgressFormCaption = "Adding deposit";
				public string DepositAddedMessage = "Deposit added";
	        }
			public NewDepositLocStrings NewDepositLocStringsInstance
				= new NewDepositLocStrings();
			/**/
			public class DepositListFormLocStrings
			{
			    public string ProlongDepositProgressFormCaption = "Deposit prolongation";
			    public string ProlongDepositSuccess = "Deposit prolongation request sent, please wait a little time before deposit Valid Until field will be updated.";
			}
			public DepositListFormLocStrings DepositListFormLocStringsInstance
				= new DepositListFormLocStrings();
			/**/
			public class NewWithdrawLocStrings
			{
			    public string WithdrawValueInputBoxCaption = "Enter withdraw value";
			    public string ReceivingEmptyPaymentDetailsPattern = "Receiving empty payment details pattern";
                public string InputBoxCaption = "Enter withdraw payment details";
				public string AddingWithdrawProgressFormCaption = "Adding withdraw";
				public string WithdrawAddedMessage = "Withdraw added";
			}
			public NewWithdrawLocStrings NewWithdrawLocStringsInstance
				= new NewWithdrawLocStrings();
            /**/
            public class DepthOfMarketLocStrings
            {
                public string CancelOrderConfirmations = "Confirm cancelling {N} orders?";
                public string ConfirmationCaption = "Cancel confirmation";
            }
            public DepthOfMarketLocStrings DepthOfMarketLocStringsInstance
                = new DepthOfMarketLocStrings();
            /**/
            public class ShowTradeListFormLocStrings
            {
                public string BuySide = "Buy";
                public string SellSide = "Sell";
            }
            public ShowTradeListFormLocStrings ShowTradeListFormLocStringsInstance
                = new ShowTradeListFormLocStrings();
            /**/
            public Dictionary<EExchangeWithdrawStatus,string> ExchangeWithdrawStatusDict
                = new Dictionary<EExchangeWithdrawStatus, string>()
                {
                    { EExchangeWithdrawStatus.Complete, "Complete" },
                    { EExchangeWithdrawStatus.Error, "Fault"},
                    { EExchangeWithdrawStatus.Created, "Processing (just created)" },
                    { EExchangeWithdrawStatus.InQueue, "Processing (in queue)" },
                    { EExchangeWithdrawStatus.Processing, "Processing" }
                };
            /**/
            public Dictionary<EExchangeDepositStatus,string> ExchangeDepositStatusDict
                = new Dictionary<EExchangeDepositStatus, string>()
                {
                    { EExchangeDepositStatus.Created, "Processing (just created)" },
                    { EExchangeDepositStatus.SentToPaymentService, "Processing (sent to payment service)" },
                    { EExchangeDepositStatus.Error, "Fault" },
                    { EExchangeDepositStatus.Expired, "Expired" },
                    { EExchangeDepositStatus.PaymentDetailsReceived, "Payment details received" },
                    { EExchangeDepositStatus.PaymentReceived, "Completed successfully" }
                };
        }
        /**/
        public ExchangeServerLocStrings ExchangeServerLocStringsInstance
            = new ExchangeServerLocStrings();
    }
}
