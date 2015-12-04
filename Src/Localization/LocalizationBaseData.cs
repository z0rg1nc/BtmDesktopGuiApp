using BtmI2p.BitMoneyClient.Gui.Communication.JsonRpcServers;
using BtmI2p.BitMoneyClient.Gui.Forms;
using BtmI2p.BitMoneyClient.Gui.Forms.Exchange;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm;
using BtmI2p.BitMoneyClient.Gui.Forms.MainForm.JsonPrcServerSettings;
using BtmI2p.BitMoneyClient.Gui.Forms.Message;
using BtmI2p.BitMoneyClient.Gui.Forms.Mining;
using BtmI2p.BitMoneyClient.Gui.Forms.MiningLocal;
using BtmI2p.BitMoneyClient.Gui.Forms.User;
using BtmI2p.BitMoneyClient.Gui.Forms.Wallet;
using BtmI2p.MiscClientForms;

namespace BtmI2p.BitMoneyClient.Gui.Localization
{
    public class ClientGuiLocalizationData
    {
        public ClientGuiMainFormLocStrings 
            ClientGuiMainFormLocStringsInstance 
                = new ClientGuiMainFormLocStrings();
        public EditJsonRpcServersSettingsFormLocStrings 
            EditJsonRpcServersSettingsFormLocStringsInstance
                = new EditJsonRpcServersSettingsFormLocStrings();
        public AddContactFormLocStrings
            AddContactFormLocStringsInstance
                = new AddContactFormLocStrings();
        public EditMessageUserSettingsFormLocStrings
            EditMessageUserSettingsFormLocStringsInstance
                = new EditMessageUserSettingsFormLocStrings();
        public RegisterUserFormLocStrings
            RegisterUserFormLocStringsInstance
                = new RegisterUserFormLocStrings();
        public SelectUserProfileFormLocStrings
            SelectUserProfileFormLocStringsInstance
                = new SelectUserProfileFormLocStrings();
        public ViewUserContactInfoLocStrings
            ViewUserContactInfoLocStringsInstance
                = new ViewUserContactInfoLocStrings();
        public EnterPasswordFormLocStrings
            EnterPasswordFormLocStringsInstance
                = new EnterPasswordFormLocStrings();
        public InputBoxFormLocStrings
            InputBoxFormLocStringsInstance
                = new InputBoxFormLocStrings();
        public ProgressCancelFormLocStrings
            ProgressCancelFormLocStringsInstance
                = new ProgressCancelFormLocStrings();
        public SelectEnumValueFormLocStrings
            SelectEnumValueFormLocStringsInstance
                = new SelectEnumValueFormLocStrings();
        public RegisterMiningClientFormLocStrings
            RegisterMiningClientFormLocStringsInstance
                = new RegisterMiningClientFormLocStrings();
        public EditMiningLocalSettingsFormLocStrings
            EditMiningLocalSettingsFormLocStringsInstance
                = new EditMiningLocalSettingsFormLocStrings();
        public EditI2PSettingsFormLocStrings
            EditI2PSettingsFormLocStringsInstance
                = new EditI2PSettingsFormLocStrings();
        public EditWalletSettingsFormLocStrings
            EditWalletSettingsFormLocStringsInstance
                = new EditWalletSettingsFormLocStrings();
        public ExternalPaymentProcessorSettingsFormLocStrings
            ExternalPaymentProcessorSettingsFormLocStringsInstance
                = new ExternalPaymentProcessorSettingsFormLocStrings();
        public FullTransferHistoryFormLocStrings
            FullTransferHistoryFormLocStringsInstance
                = new FullTransferHistoryFormLocStrings();
        public ProcessWalletInvoiceFormLocStrings
            ProcessWalletInvoiceFormLocStringsInstance
                = new ProcessWalletInvoiceFormLocStrings();
        public RegisterWalletFormLocStrings
            RegisterWalletFormLocStringsInstance
                = new RegisterWalletFormLocStrings();
        public SendFundsFormLocStrings
            SendFundsFormLocStringsInstance
                = new SendFundsFormLocStrings();
        public ShowTransferInfoFormLocStrings
            ShowTransferInfoFormLocStringsInstance
                = new ShowTransferInfoFormLocStrings();
        public LocalRpcServersManagerLocStrings
            LocalRpcServersManagerLocStringsInstance
                = new LocalRpcServersManagerLocStrings();
        public RegisterExchangeClientFormLocStrings
            RegisterExchangeClientFormLocStringsInstance
                = new RegisterExchangeClientFormLocStrings();
		public ExchangeNewOrderFormLocStrings
			ExchangeNewOrderFormLocStringsInstance
				= new ExchangeNewOrderFormLocStrings();
        public RegisterExchangeClientFormDesignerLocStrings 
            RegisterExchangeClientFormDesignerLocStringsInstance 
                = new RegisterExchangeClientFormDesignerLocStrings();
        public ExchangeWithdrawListFormDesignerLocStrings ExchangeWithdrawListFormDesignerLocStringsInstance = new ExchangeWithdrawListFormDesignerLocStrings();
        public ExchangeTradeListFormDesignerLocStrings ExchangeTradeListFormDesignerLocStringsInstance = new ExchangeTradeListFormDesignerLocStrings();
        public ExchangeSecurityListFormDesignerLocStrings ExchangeSecurityListFormDesignerLocStringsInstance = new ExchangeSecurityListFormDesignerLocStrings();
        public ExchangePaymentDetailsEditOrShowFormDesignerLocStrings ExchangePaymentDetailsEditOrShowFormDesignerLocStringsInstance = new ExchangePaymentDetailsEditOrShowFormDesignerLocStrings();
        public ExchangeOrderListFormDesignerLocStrings ExchangeOrderListFormDesignerLocStringsInstance = new ExchangeOrderListFormDesignerLocStrings();
        public ExchangeDepthOfMarketFormDesignerLocStrings ExchangeDepthOfMarketFormDesignerLocStringsInstance = new ExchangeDepthOfMarketFormDesignerLocStrings();
        public ExchangeDepositListFormDesignerLocStrings ExchangeDepositListFormDesignerLocStringsInstance = new ExchangeDepositListFormDesignerLocStrings();
        public ExchangeCurrencyListFormDesignerLocStrings ExchangeCurrencyListFormDesignerLocStringsInstance = new ExchangeCurrencyListFormDesignerLocStrings();
        public ExchangeChartCandlesFormDesignerLocStrings ExchangeChartCandlesFormDesignerLocStringsInstance = new ExchangeChartCandlesFormDesignerLocStrings();
        public ClientGuiMainFormDesignerLocStrings ClientGuiMainFormDesignerLocStringsInstance = new ClientGuiMainFormDesignerLocStrings();
        public FullTransferHistoryFormDesignerLocStrings FullTransferHistoryFormDesignerLocStringsInstance = new FullTransferHistoryFormDesignerLocStrings();
        public MyMessageBoxFormDesignerLocStrings MyMessageBoxFormDesignerLocStringsInstance = new MyMessageBoxFormDesignerLocStrings();
        public ExchangeDepositListFormLocStrings ExchangeDepositListFormLocStringsInstance = new ExchangeDepositListFormLocStrings();
        /**/
        public void Init()
        {
            ClientGuiMainForm.LocStrings 
                = ClientGuiMainFormLocStringsInstance;
            EditJsonRpcServerSettingsForm.LocStrings 
                = EditJsonRpcServersSettingsFormLocStringsInstance;
            AddContactForm.LocStrings 
                = AddContactFormLocStringsInstance;
            EditMessageUserSettingsForm.LocStrings
                = EditMessageUserSettingsFormLocStringsInstance;
            RegisterMessageClientForm.LocStrings
                = RegisterUserFormLocStringsInstance;
            SelectProfileForm.LocStrings
                = SelectUserProfileFormLocStringsInstance;
            ViewUserContactInfo.LocStrings
                = ViewUserContactInfoLocStringsInstance;
            EnterPasswordForm.LocStrings
                = EnterPasswordFormLocStringsInstance;
            InputBoxForm.LocStrings
                = InputBoxFormLocStringsInstance;
            ProgressCancelForm.LocStrings
                = ProgressCancelFormLocStringsInstance;
            SelectEnumValueFormStatic.LocStrings
                = SelectEnumValueFormLocStringsInstance;
            RegisterMiningClientForm.LocStrings
                = RegisterMiningClientFormLocStringsInstance;
            EditMiningLocalSettingsForm.LocStrings
                = EditMiningLocalSettingsFormLocStringsInstance;
            EditI2PSettingsForm.LocStrings
                = EditI2PSettingsFormLocStringsInstance;
            EditWalletSettingsForm.LocStrings
                = EditWalletSettingsFormLocStringsInstance;
            ExternalPaymentProcessorSettingsForm.LocStrings
                = ExternalPaymentProcessorSettingsFormLocStringsInstance;
            FullTransferHistoryForm.LocStrings
                = FullTransferHistoryFormLocStringsInstance;
            ProcessWalletInvoiceForm.LocStrings
                = ProcessWalletInvoiceFormLocStringsInstance;
            RegisterWalletForm.LocStrings
                = RegisterWalletFormLocStringsInstance;
            SendFundsForm.LocStrings
                = SendFundsFormLocStringsInstance;
            ShowTransferInfoForm.LocStrings
                = ShowTransferInfoFormLocStringsInstance;
            LocalRpcServersManager.LocStrings
                = LocalRpcServersManagerLocStringsInstance;
            RegisterExchangeClientForm.LocStrings
                = RegisterExchangeClientFormLocStringsInstance;
	        ExchangeNewOrderForm.LocStrings
		        = ExchangeNewOrderFormLocStringsInstance;
            RegisterExchangeClientForm.DesignerLocStrings 
                = RegisterExchangeClientFormDesignerLocStringsInstance;
            ExchangeWithdrawListForm.DesignerLocStrings 
                = ExchangeWithdrawListFormDesignerLocStringsInstance;
            ExchangeTradeListForm.DesignerLocStrings 
                = ExchangeTradeListFormDesignerLocStringsInstance;
            ExchangeSecurityListForm.DesignerLocStrings 
                = ExchangeSecurityListFormDesignerLocStringsInstance;
            ExchangePaymentDetailsEditOrShowForm.DesignerLocStrings 
                = ExchangePaymentDetailsEditOrShowFormDesignerLocStringsInstance;
            ExchangeOrderListForm.DesignerLocStrings 
                = ExchangeOrderListFormDesignerLocStringsInstance;
            ExchangeDepthOfMarketForm.DesignerLocStrings 
                = ExchangeDepthOfMarketFormDesignerLocStringsInstance;
            ExchangeDepositListForm.DesignerLocStrings 
                = ExchangeDepositListFormDesignerLocStringsInstance;
            ExchangeCurrencyListForm.DesignerLocStrings 
                = ExchangeCurrencyListFormDesignerLocStringsInstance;
            ExchangeChartCandlesForm.DesignerLocStrings 
                = ExchangeChartCandlesFormDesignerLocStringsInstance;
            ClientGuiMainForm.DesignerLocStrings 
                = ClientGuiMainFormDesignerLocStringsInstance;
            FullTransferHistoryForm.DesignerLocStrings 
                = FullTransferHistoryFormDesignerLocStringsInstance;
            MyMessageBoxForm.DesignerLocStrings
                = MyMessageBoxFormDesignerLocStringsInstance;
            ExchangeDepositListForm.LocStrings
                = ExchangeDepositListFormLocStringsInstance;
        }
    }
}